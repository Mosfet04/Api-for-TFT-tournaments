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
    public class connectToApi : Requisitions
    {

        public static JogadorLol GetPlayerData(string player)
        {
            try
            {
                JogadorLol jogador = JsonSerializer.Deserialize<JogadorLol>(player);
                return jogador;
            }
            catch (Exception err)
            {
                Console.WriteLine("Resposta da API para players fora do esperado\n" + err);
                throw;
            }
            /*Console.WriteLine($"ID: {jogador.id}\naccountId: " +
                $"{jogador.accountId}\npuuid: {jogador.puuid}\nname:" +
                $"{jogador.name}\nprofileIconId: {jogador.profileIconId}\nrevisionDate:" +
                $"{jogador.revisionDate}\nLevel: {jogador.summonerLevel}");*/

        }
        // Agiles

        public async Task<JogadorLol> connectAPIAsyncPlayer(string nomePlayer)
        {

            string url = "https://br1.api.riotgames.com/tft/summoner/v1/summoners/by-name/" + nomePlayer;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);

            string res = await client.GetStringAsync(url);
            NumberRequests++;
            JogadorLol jogador = GetPlayerData(res);
            //Console.WriteLine(NumberRequests);
            return jogador;

        }


        public static Rootobject GetMatchData(string matchData)
        {
            try
            {
                Rootobject match = JsonSerializer.Deserialize<Rootobject>(matchData);
                //Console.WriteLine(match.metadata.match_id);
                return match;
            }
            catch (Exception err)
            {
                Console.WriteLine("Resposta da API para partidas fora do esperado\n" + err);
                throw;
            }
        }
        public async Task<string[]> getMatchID(string puuid)
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
        public async Task<Rootobject> connectAPIAsyncMatch(string matchID)
        {
            /*//CATCH MATCH ID
            string url = "https://americas.api.riotgames.com/tft/match/v1/matches/by-puuid/" + puuid + "/ids?count=1";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);
            string matchID = await client.GetStringAsync(url);
            NumberRequests++;
            //Console.WriteLine(NumberRequests);

            List<char> charsToRemove = new List<char>() { '[', ']', '"' };
            matchID = matchID.Filter(charsToRemove);
            */

            //CATCH MATCH DATA
            string url = "https://americas.api.riotgames.com/tft/match/v1/matches/" + matchID;
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", keyRiot);
            string matchData = await client.GetStringAsync(url);
            Rootobject match = new Rootobject();
            match = GetMatchData(matchData);
            // Console.WriteLine(NumberRequests);

            return match;

        }

    }
}
