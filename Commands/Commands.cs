using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;

namespace DiscordBot
{
    public class PrimaryModule : BaseCommandModule
    {

        string apiKey = System.IO.File.ReadAllText("apiKey.txt");
        HttpClient client = new HttpClient();
        
        // client.DefaultRequestHeaders.Add("", apiKey);
        [Command("info")]
        public async Task apiQuery(CommandContext ctx, string summonerName)
        {            
            var endpoint = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName + "?api_key=" + apiKey;
            //var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            //request.Headers.Add("X-Riot-Token", apiKey);            
            // var nameEndpoint = new Uri("na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName);
            var summonerResult = client.GetAsync(endpoint).Result;
            var nameJson = summonerResult.Content.ReadAsStringAsync().Result;

            var name = JsonConvert.DeserializeObject<summonerV4>(nameJson);
            string encryptedAcct = name.id;

            var leagueEndpoint = "https://na1.api.riotgames.com/lol/league/v4/entries/by-summoner/" + encryptedAcct + "?api_key=" + apiKey;
            var leagueResult = client.GetAsync(leagueEndpoint).Result;
            var leagueJson = leagueResult.Content.ReadAsStringAsync().Result;

            var leagueInfo = JsonConvert.DeserializeObject<List<leagueV4>>(leagueJson);

            string info = "";

            for (int i = 0; i < leagueInfo.Count; i++)
            { 
                var league = leagueInfo[i];
                info += league.queueType + " rank: " + league.tier + " " + league.rank + " ";
            }

            Console.WriteLine(info);
        }
    }
}
