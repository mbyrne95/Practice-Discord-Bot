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
        
        private string RetrieveEncryptedSummoner(string summonerName)
        {
            var endpoint = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName + "?api_key=" + apiKey;
            var summonerResult = client.GetAsync(endpoint).Result;
            var nameJson = summonerResult.Content.ReadAsStringAsync().Result;

            var name = JsonConvert.DeserializeObject<summonerV4>(nameJson);
            
            string encryptedSummoner = name.id;

            return encryptedSummoner;
        }
        
        [Command("rank")]
        public async Task rankedInfo(CommandContext ctx, [RemainingText]string summonerName)
        {
            string encryptedSummoner = RetrieveEncryptedSummoner(summonerName);

            var leagueEndpoint = "https://na1.api.riotgames.com/lol/league/v4/entries/by-summoner/" + encryptedSummoner + "?api_key=" + apiKey;
            var leagueResult = client.GetAsync(leagueEndpoint).Result;
            var leagueJson = leagueResult.Content.ReadAsStringAsync().Result;
            var leagueInfo = JsonConvert.DeserializeObject<List<leagueV4>>(leagueJson);

            //Console.WriteLine(leagueJson);

            string info = ""; 

            if (leagueInfo.Count == 0 || leagueInfo == null)
            {
                info = summonerName + ":";
                await ctx.RespondAsync(">>> " + info + "\n   *This user does not have any ranked data.*");
            } else {
                info += leagueInfo[0].summonerName + ":";
                for (int i = 0; i < leagueInfo.Count; i++)
                {
                    var league = leagueInfo[i];
                    double wr = Math.Round((((double)league.wins / ((double)league.wins + (double)league.losses)) * 100), 2);

                    info += "\n   *" + CleanString.RegexClean(league.queueType) + "* - **" + CleanString.RegexClean(league.tier) + " " + CleanString.RegexClean(league.rank) + "** (wr: " + wr + "%)";
                }
                await ctx.RespondAsync(">>> " + info);
            }
        }

        [Command("mastery")]
        public async Task championMastery(CommandContext ctx, [RemainingText] string summonerName)
        {
            string encryptedSummoner = RetrieveEncryptedSummoner(summonerName);

            var championEndpoint = "https://na1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/" + encryptedSummoner + "?api_key=" + apiKey;
            var championResult = client.GetAsync(championEndpoint).Result;
            var championJson = championResult.Content.ReadAsStringAsync().Result;
            var championInfo = JsonConvert.DeserializeObject<List<championV4>>(championJson);

            string info = summonerName + "'s most played champions:";

            for (int i = 0; i < 3; i++)
            {
                if (championInfo[i] != null)
                {
                    var champion = championInfo[i];
                    info += "\n   **" + champion.championId + "** - " + champion.championPoints + " mastery points.";
                }
            }
            await ctx.RespondAsync(">>> " + info);
        }

    }
}
