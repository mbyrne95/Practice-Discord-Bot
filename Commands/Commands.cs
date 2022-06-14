using System;
using System.IO;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordBot
{
    public class PrimaryModule : BaseCommandModule
    {

        string apiKey = System.IO.File.ReadAllText("apiKey.txt");
        string championText = System.IO.File.ReadAllText(@"champion.json");
        HttpClient client = new HttpClient();
        
        //converts summoner name into encrtyped summoner
        private string RetrieveEncryptedSummoner(string summonerName)
        {
            var endpoint = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName + "?api_key=" + apiKey;
            var summonerResult = client.GetAsync(endpoint).Result;
            var nameJson = summonerResult.Content.ReadAsStringAsync().Result;

            var name = JsonConvert.DeserializeObject<summonerV4>(nameJson);
            
            string encryptedSummoner = name.id;

            return encryptedSummoner;
        }

        //converting champion key to champion name
        private string keyLookUp(string champKey)
        {
            var jToken = JToken.Parse(championText);

            var data = jToken["data"];

            var traversal = data.First();
            var count = data.Children().Count();

            string myChamp = "";

            //Console.WriteLine(tempnode.ToString());
            //tempnode = tempnode.First;
            //Console.Write(data.ToString());

            for (int i = 0; i < count; i++)
            {

                if (traversal.First()["key"].ToString().Equals(champKey, StringComparison.OrdinalIgnoreCase))
                {
                    myChamp += traversal.First()["id"].ToString();
                    return myChamp;
                }

                traversal = traversal.Next;

            }
            return "doesn't exist";
        }

        //returns ranked stats of summoner
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

        //returns top 3 champion mastery
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
                    info += "\n   **" + keyLookUp(champion.championId.ToString()) + "** - " + champion.championPoints + " mastery points.";
                }
            }
            await ctx.RespondAsync(">>> " + info);
        }

        
        /*
        [Command("test")]
        public async Task championtest(CommandContext ctx, string thischampion)
        {
            string championText = System.IO.File.ReadAllText(@"champion.json");

            dynamic jToken = JToken.Parse(championText);

            var data = jToken["data"];

            var champ = ((JObject)data).GetValue(thischampion, StringComparison.OrdinalIgnoreCase);
            var title = champ["title"];

            if (champ == null)
            {
                Console.WriteLine("doesn't exist");
            } 
            else
            {
                Console.WriteLine(title.ToString());
            }
        }
        */

    }
}
