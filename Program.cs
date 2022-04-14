using APIconsult.CreateBracket;
using APIconsult.methodConectToApi;
using System;
using System.Threading.Tasks;

namespace APIconsult
{

    class Program : ConnectToApi
    {
        static async Task Main()
        {
            dynamic dinamico = 1;
            dinamico = dinamico + 3;
            Type tp = dinamico.GetType();
            Console.WriteLine(tp);

            CreateNewBracket bracket = new CreateNewBracket();
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

            int[] bracketPlayers = bracket.confirmBracket(NomesPlayers);


            while (exit == "s")
            {
                Console.Clear();
                await VerifyPositionTornamentAsync(NomesPlayers, bracketPlayers);
                System.Threading.Thread.Sleep(5000);
                //System.Threading.Thread.Sleep(3600000);
                Console.WriteLine("Continuar mapeando?");
                exit = Console.ReadLine();
            }


        }
    }
}

