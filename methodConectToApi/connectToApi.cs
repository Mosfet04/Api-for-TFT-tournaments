using System;
using APIconsult.Jogador;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using APIconsult.Match;

namespace APIconsult.methodConectToApi
{

    public static class Extensions
    {
        public static string Filter(this string str, List<char> charsToRemove)
        {
            charsToRemove.ForEach(c => str = str.Replace(c.ToString(), String.Empty));
            return str;
        }
    }
    public class Requisitions
    {
        public static int NumberRequests { get; set; }
        public static string keyRiot = "RGAPI-37031f0c-2f2e-4ef4-b191-cc74e00bb5e8";
    }
    public class ConnectToApi : Requisitions
    {
        //Metodo para request de dados de players em formato individual
        private static async Task<JogadorLol> ConnectAPIAsyncPlayer(string nomePlayer)
        {

            string url = "https://br1.api.riotgames.com/tft/summoner/v1/summoners/by-name/" + nomePlayer;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);


            try
            {
                string player = await client.GetStringAsync(url);
                NumberRequests++;
                JogadorLol jogador = JsonSerializer.Deserialize<JogadorLol>(player);
                return jogador;
            }
            catch (Exception err)
            {
                Console.WriteLine("Resposta da API para players fora do esperado\n" + err);
                throw;
            }

        }
        //Metodo para request de ID de partidas de cada player individualmente
        private static async Task<string[]> GetMatchID(string puuid)
        {
            //CATCH MATCH ID
            string url = "https://americas.api.riotgames.com/tft/match/v1/matches/by-puuid/" + puuid + "/ids?count=5";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);
            string responseMatchID = await client.GetStringAsync(url);
            NumberRequests++;
            //Console.WriteLine(NumberRequests);

            List<char> charsToRemove = new List<char>() { '[', ']', '"' };
            responseMatchID = responseMatchID.Filter(charsToRemove);
            string[] matchID = responseMatchID.Split(',');
            return matchID;
        }
        //Metodo para request de dados de partida em formato individual
        private static async Task<Rootobject> ConnectAPIAsyncMatch(string matchID)
        {

            //CATCH MATCH DATA
            string url = "https://americas.api.riotgames.com/tft/match/v1/matches/" + matchID;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);
            try
            {
                string matchData = await client.GetStringAsync(url);
                Rootobject match = new Rootobject();
                match = JsonSerializer.Deserialize<Rootobject>(matchData);
                return match;
            }
            catch (Exception err)
            {
                Console.WriteLine("Resposta da API para partidas fora do esperado\n" + err);
                throw;
            }

        }
        //Metodo que seleciona o ID de partida mais comum
        private static async Task<JogadorLol[]> SelectBestMatchID(string[] NomesPlayers, int[] positionBracket)
        {
            JogadorLol[] jogador = new JogadorLol[NomesPlayers.Length];
            string[,] matchIDAll = new string[8 * 5, positionBracket[^1]];
            int aux2 = 0;
            for (int i = 0; i < NomesPlayers.Length; i++)
            {

                try
                {
                    if (positionBracket[i] != positionBracket[i - 1])
                    {
                        aux2 = 0;
                    }
                }
                catch { }
                try
                {
                    jogador[i] = await await Task.FromResult(ConnectAPIAsyncPlayer(NomesPlayers[i]));
                    string[] auxMatchID = await await Task.FromResult(GetMatchID(jogador[i].puuid));
                    for (int j = 0; j < 5; j++)
                    {
                        try
                        {
                            matchIDAll[5 * aux2 + j, positionBracket[i] - 1] = auxMatchID[j];
                        }
                        catch
                        {
                            matchIDAll[5 * aux2 + j, positionBracket[i] - 1] = "";

                        }

                    }
                    aux2++;
                }
                catch (Exception err)
                {
                    Console.WriteLine("Erro ao buscar dados do player\n" + err.Message);
                    throw;
                }
                jogador[i].bracketPosition = positionBracket[i];

            }

            string[,] auxComparacao = matchIDAll;
            string[] moreCommomMatchIDs = new string[positionBracket[^1]];
            for (int i = 0; i < positionBracket[^1]; i++)
            {
                int aux = 0;
                for (int j = 0; j < 8 * 5; j++)
                {
                    int quantidade = 0;
                    for (int k = 0; k < 8 * 5; k++)
                    {
                        if (matchIDAll[j, i] == auxComparacao[k, i] && auxComparacao[k, i] != null && matchIDAll[j, i] != null)
                        {
                            quantidade++;
                            if (aux < quantidade)
                            {
                                aux = quantidade;
                                moreCommomMatchIDs[i] = matchIDAll[j, i];
                                //Console.WriteLine(moreCommomMatchIDs[i] + "\tPosition: " + i);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < positionBracket[^1]; i++)
            {
                for (int j = 0; j < NomesPlayers.Length; j++)
                {
                    if (jogador[j].bracketPosition == i + 1)
                    {
                        jogador[j].matchIDTornament = moreCommomMatchIDs[i];
                    }
                }

            }

            return jogador;
        }
        //Metodo para agrupamento de todos os jogadores e posicionamento dos mesmos
        public static async Task VerifyPositionTornamentAsync(string[] NomesPlayers, int[] positionBracket)
        {
            JogadorLol[] jogador = await await Task.FromResult(SelectBestMatchID(NomesPlayers, positionBracket));

            Rootobject[] match = new Rootobject[NomesPlayers.Length];
            Console.WriteLine("Capturando dados das partidas");

            for (int i = 0; i < NomesPlayers.Length; i++)
            {
                try
                {
                    match[i] = await await Task.FromResult(ConnectAPIAsyncMatch(jogador[i].matchIDTornament));
                }
                catch (Exception err)
                {
                    Console.WriteLine("Erro ao buscar dados de partida" + err);
                    throw;
                }
                for (int j = 0; j < 8; j++)
                {
                    if (match[i].info.participants[j].puuid == jogador[i].puuid)
                    {
                        jogador[i].lastPosition = match[i].info.participants[j].placement;
                        Console.WriteLine("Posição do participante " + jogador[i].name + " no Bracket " + jogador[i].bracketPosition + ":\n" + jogador[i].lastPosition);
                    }
                }
            }
            for (int i = 1; i < NomesPlayers.Length; i++)
            {
                if ((match[i].metadata.match_id != match[i - 1].metadata.match_id) && (positionBracket[i] == positionBracket[i - 1]))
                {
                    Console.WriteLine("Atenção, jogadores com ID de partida capturado diferente no Bracket: " + positionBracket[i]);
                }
            }
        }


    }
}
