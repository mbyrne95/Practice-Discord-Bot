using System;
using System.IO;
using System;
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
        string championTextCH = System.IO.File.ReadAllText(@"championChinese.json");
        HttpClient client = new HttpClient();
        
        //converts summoner name into encrypted summoner
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
        //combine keylookups?
        private string keyToName(string champKey)
        {
            var jToken = JToken.Parse(championText);
            var data = jToken["data"];
            var traversal = data.First();
            var count = data.Children().Count();
            string myChamp = "";

            //Console.WriteLine(traversal.ToString());

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

        //converting champion key to CH champion name - RETURNS TITLE (name and title reversed in chinese json??)
        //1 = chinese
        private string keyToName(string champKey, int i)
        {
            if (i == 1)
            {
                var jToken = JToken.Parse(championTextCH);
                var data = jToken["data"];
                var traversal = data.First();
                var count = data.Children().Count();
                string myChamp = "";

                //Console.WriteLine(traversal.ToString());

                for (int j = 0; j < count; j++)
                {

                    if (traversal.First()["key"].ToString().Equals(champKey, StringComparison.OrdinalIgnoreCase))
                    {
                        //Console.WriteLine(traversal.ToString());
                        myChamp += traversal.First()["title"].ToString();
                        return myChamp;
                    }

                    traversal = traversal.Next;

                }
                return "doesn't exist";
            }
            return keyToName(champKey);
        }
        


        //randomizer, wip
        //to do - add more things to randomize
        [Command("random")]
        public async Task randomizer(CommandContext ctx, string toBeRandomized)
        {
            if(toBeRandomized.Equals("champion", StringComparison.OrdinalIgnoreCase))
            {
                var jToken = JToken.Parse(championText);
                var data = jToken["data"];
                var traversal = data.First();
                var count = data.Children().Count();
                Random rnd = new Random();

                int range = rnd.Next(0, count);

                for (int i = 0; i < range; i++)
                {
                    traversal = traversal.Next;
                }

                //Console.WriteLine(traversal.ToString());
                await ctx.RespondAsync(">>> **" + traversal.First()["id"].ToString() + "**");
            }
        }

        //add better conventions
        //fix case sensitivity
        [Command("chinese")]
        public async Task chineseName(CommandContext ctx, string champName)
        {
            var jToken = JToken.Parse(championText);
            var data = jToken["data"];
            var jTokenCH = JToken.Parse(championTextCH);
            var dataCH = jTokenCH["data"];

            var temp = ((JObject)data).GetValue(champName, StringComparison.OrdinalIgnoreCase);
            //Console.WriteLine(temp.ToString());
            //var champContainer = data[champName];
            string champKey = temp["key"].ToString();
            //Console.WriteLine(champKey);
            //Console.WriteLine(champContainer.ToString());
            await ctx.RespondAsync(">>> " + temp["id"].ToString() + " - **" + keyToName(champKey, 1).ToString() + "**");

            //var champID = data["id"]
        }

        //returns ranked stats of summoner
        //handle exceptions
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
        //handle exceptions
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
                    info += "\n   **" + keyToName(champion.championId.ToString()) + "** - " + champion.championPoints + " mastery points.";
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
