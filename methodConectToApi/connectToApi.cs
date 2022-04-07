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
    }
    public class connectToApi : Requisitions
    {

        public static JogadorLol GetPlayerData(string player)
        {
            JogadorLol jogador = JsonSerializer.Deserialize<JogadorLol>(player);
            /*Console.WriteLine($"ID: {jogador.id}\naccountId: " +
                $"{jogador.accountId}\npuuid: {jogador.puuid}\nname:" +
                $"{jogador.name}\nprofileIconId: {jogador.profileIconId}\nrevisionDate:" +
                $"{jogador.revisionDate}\nLevel: {jogador.summonerLevel}");*/
            return jogador;
        }

     
        public async Task<JogadorLol> connectAPIAsyncPlayer(string nomePlayer)
        {

            string url = "https://br1.api.riotgames.com/tft/summoner/v1/summoners/by-name/" + nomePlayer;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", "RGAPI-f2e2e21a-3a0b-476d-8409-17286c9cad12");

            string res = await client.GetStringAsync(url);
            NumberRequests++;
            JogadorLol jogador = GetPlayerData(res);
            //Console.WriteLine(NumberRequests);
            return jogador;

        }


        public static Rootobject GetMatchData(string matchData)
        {
            Rootobject match = JsonSerializer.Deserialize<Rootobject>(matchData);
            //Console.WriteLine(match.metadata.match_id);
            return match;
        }
        public async Task<Rootobject> connectAPIAsyncMatch(string puuid)
        {
            //CATCH MATCH ID
            string url = "https://americas.api.riotgames.com/tft/match/v1/matches/by-puuid/" + puuid+ "/ids?count=1";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Riot-Token", "RGAPI-f2e2e21a-3a0b-476d-8409-17286c9cad12");
            string matchID = await client.GetStringAsync(url);
            NumberRequests++;
            //Console.WriteLine(NumberRequests);

            List<char> charsToRemove = new List<char>() { '[', ']', '"'};
            matchID = matchID.Filter(charsToRemove);
            
            //CATCH MATCH DATA
            url = "https://americas.api.riotgames.com/tft/match/v1/matches/" + matchID;
            string matchData = await client.GetStringAsync(url);
            NumberRequests++;
            Rootobject match = new Rootobject();
            match = GetMatchData(matchData);
           // Console.WriteLine(NumberRequests);

            return match;

        }

    }
}
