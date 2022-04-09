using System;
using APIconsult.Jogador;
using System.Threading.Tasks;
using APIconsult.Match;
using APIconsult.methodConectToApi;
using System.Linq;

namespace APIconsult
{

    class Program
    {
        public static async Task<JogadorLol[]> SelectMatchIDAsync(string[] NomesPlayers, int[] positionBracket)
        {
            JogadorLol[] jogador = new JogadorLol[NomesPlayers.Length];
            connectToApi connect = new connectToApi();
            Console.WriteLine("Capturando dados dos jogadores");
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
                //FOR GET DATA PLAYER SET NAMEPROFILLE
                try
                {
                    jogador[i] = await await Task.FromResult(connect.connectAPIAsyncPlayer(NomesPlayers[i]));
                    string[] auxMatchID = await await Task.FromResult(connect.getMatchID(jogador[i].puuid));
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
        public static async Task VerificacaoPosicaoTorneioAsync(string[] NomesPlayers, int[] positionBracket)
        {
            JogadorLol[] jogador = await await Task.FromResult(SelectMatchIDAsync(NomesPlayers, positionBracket));
            connectToApi connect = new connectToApi();

            Rootobject[] match = new Rootobject[NomesPlayers.Length];
            Console.WriteLine("Capturando dados das partidas");

            for (int i = 0; i < NomesPlayers.Length; i++)
            {
                try
                {
                    match[i] = await await Task.FromResult(connect.connectAPIAsyncMatch(jogador[i].matchIDTornament));
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

        public static int[] SetPlayersLobby(string[] arrayJogadorName)
        {
            int[] sala = new int[arrayJogadorName.Length];
            int aux = 0;
            int enchendoASala = arrayJogadorName.Length / 8;//1
            int sobraramAposEncher = arrayJogadorName.Length - (enchendoASala * 8);//7
            int[] salaPlayers = new int[enchendoASala + 1];
            for (int i = 0; i < salaPlayers.Length; i++)
            {
                if (i < enchendoASala)
                {
                    salaPlayers[i] = 8;
                }
                else
                {
                    salaPlayers[i] = sobraramAposEncher;
                }
            }
            if (sobraramAposEncher > 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    for (int i = salaPlayers.Length - 2; i >= 0; i--)
                    {
                        if (salaPlayers[^1] < salaPlayers[i] && salaPlayers[^1] < 9)
                        {
                            salaPlayers[^1]++;
                            salaPlayers[i]--;
                        }
                        /*if (i == 0 && (salaPlayers[salaPlayers.Length - 1] + 1) < salaPlayers[salaPlayers.Length - 2])
                        {
                            i = salaPlayers.Length - 2;
                        }*/
                    }
                }
            }
            for (int i = 0; i < salaPlayers.Length; i++)
            {
                for (int j = 0; j < salaPlayers[i]; j++)
                {
                    sala[aux] = (i + 1);
                    aux++;
                }
            }
            /*foreach (int v in sala)
            {
                Console.WriteLine(v);
            }*/

            return sala;
        }

        public static string[] randomizePlayers(string[] arrayJogadorName)
        {

            Random random = new Random();
            arrayJogadorName = arrayJogadorName.OrderBy(x => random.Next()).ToArray();
            return arrayJogadorName;
        }

        public static int[] confirmBracket(string[] NomesPlayers)
        {
            NomesPlayers = randomizePlayers(NomesPlayers);
            int[] salaPlayers = SetPlayersLobby(NomesPlayers);
            string confirm = null;
            while (confirm != "s")
            {
                for (int j = 0; j < NomesPlayers.Length; j++)
                {
                    Console.WriteLine("Participante " + NomesPlayers[j] + " no Bracket " + salaPlayers[j] + "\n");
                }
                Console.WriteLine("Chaves confirmadas?");
                confirm = Console.ReadLine();
            }
            return salaPlayers;
        }

        static async Task Main(string[] args)
        {

            string exit = "s";
            string[] NomesPlayers = new string[16];
            NomesPlayers[0] = "ptinvocador";
            NomesPlayers[1] = "gabrieluz20";
            NomesPlayers[2] = "Gloriem";
            NomesPlayers[3] = "PeTT";
            NomesPlayers[4] = "Lukrox";
            NomesPlayers[5] = "Xamllew";
            NomesPlayers[6] = "Kozmic";
            NomesPlayers[7] = "Chibsbr";

            NomesPlayers[8] = "EvandroFocking";
            NomesPlayers[9] = "Scoorpioon15";
            NomesPlayers[10] = "Tijuilhos";
            NomesPlayers[11] = "bardosa";
            NomesPlayers[12] = "WRWRWR";
            NomesPlayers[13] = "xiaobaishu";
            NomesPlayers[14] = "xiaobaishu";
            NomesPlayers[15] = "EvandroFocking";

            int[] salaPlayers = confirmBracket(NomesPlayers);


            while (exit == "s")
            {
                Console.Clear();
                await VerificacaoPosicaoTorneioAsync(NomesPlayers, salaPlayers);
                System.Threading.Thread.Sleep(5000);
                //System.Threading.Thread.Sleep(3600000);
                Console.WriteLine("Continuar mapeando?");
                exit = Console.ReadLine();
            }


        }
    }
}

