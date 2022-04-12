using System;
using System.Linq;

namespace APIconsult.CreateBracket
{
    public class CreateNewBracket
    {
        private static int[] SetPlayersLobby(string[] arrayJogadorName)
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

            return sala;
        }

        private static string[] randomizePlayers(string[] arrayJogadorName)
        {

            Random random = new Random();
            arrayJogadorName = arrayJogadorName.OrderBy(x => random.Next()).ToArray();
            return arrayJogadorName;
        }

        public int[] confirmBracket(string[] NomesPlayers)
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


    }
}
