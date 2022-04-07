using System;
using APIconsult.Jogador;
using System.Threading.Tasks;
using APIconsult.Match;
using APIconsult.methodConectToApi;

namespace APIconsult
{
    class Program
    {
        public static async Task VerificacaoPosicaoTorneioAsync(string[] NomesPlayers)
        {
            JogadorLol[] jogador = new JogadorLol[32];
            connectToApi connect = new connectToApi();
            
            //ARRAY WITH NAMES
            NomesPlayers[0] = "ptinvocador";
            NomesPlayers[1] = "LuxPetistaSafada";
            NomesPlayers[2] = "rouwrouw";
            NomesPlayers[3] = "LLSS";
            NomesPlayers[4] = "przGOD";
            NomesPlayers[5] = "darknero";


            for (int i = 0; i < 6; i++)
            {
                //FOR GET DATA PLAYER SET NAMEPROFILLE
                jogador[i] = await await Task.FromResult(connect.connectAPIAsyncPlayer(NomesPlayers[i]));
            }
            for (int i = 0; i < 6; i++)
            {
                Rootobject[] match = new Rootobject[32];
                match[i] = await await Task.FromResult(connect.connectAPIAsyncMatch(jogador[i].puuid));
                for (int j = 0; j < 8; j++)
                {
                    if (match[i].info.participants[j].puuid == jogador[i].puuid)
                    {
                        jogador[i].lastPosition = match[i].info.participants[j].placement;
                        Console.WriteLine("Posição do participante " + jogador[i].name + ":\n" + jogador[i].lastPosition);
                    }
                }
            }

        }
        static async Task Main(string[] args) 
        {
            string[] Names = new string[32];
            await VerificacaoPosicaoTorneioAsync(Names);
            Console.ReadLine();
        }
    }
}

