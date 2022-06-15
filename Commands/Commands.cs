using System;
using System.IO;
using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;

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

        //converts summoner name into puuid
        private string RetrievePUUID(string summonerName)
        {
            var endpoint = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + summonerName + "?api_key=" + apiKey;
            var summonerResult = client.GetAsync(endpoint).Result;
            var nameJson = summonerResult.Content.ReadAsStringAsync().Result;

            var name = JsonConvert.DeserializeObject<summonerV4>(nameJson);

            string puuid = name.puuid;

            return puuid;
        }

        //converting champion key to champion name
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
        
        private string getProperName(string casual)
        {
            var endpoint = "https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/" + casual + "?api_key=" + apiKey;
            var summonerResult = client.GetAsync(endpoint).Result;
            var nameJson = summonerResult.Content.ReadAsStringAsync().Result;

            var name = JsonConvert.DeserializeObject<summonerV4>(nameJson);

            return name.name;
        }



        //randomizer, wip
        //to do - add more things to randomize
        //currently only champion is functioning
        [Command("random")]
        public async Task randomizer(CommandContext ctx, string toBeRandomized)
        {
            if (toBeRandomized.Equals("champion", StringComparison.OrdinalIgnoreCase) || toBeRandomized.Equals("champ", StringComparison.OrdinalIgnoreCase))
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

                //string url = "https://ddragon.leagueoflegends.com/cdn/img/champion/splash/" + traversal.First()["id"].ToString() + "_0.jpg";

                var msgEmbed = new DiscordEmbedBuilder() {
                    Color = DiscordColor.Blurple,
                    Description = traversal.First()["id"].ToString(),
                    Title = "Random Champ:",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = "https://ddragon.leagueoflegends.com/cdn/img/champion/tiles/" + traversal.First()["id"].ToString() + "_0.jpg"
                    }

                };
                await ctx.RespondAsync(msgEmbed);
            }
            /*
            else if (toBeRandomized.Equals("profile", StringComparison.OrdinalIgnoreCase) || toBeRandomized.Equals("avi", StringComparison.OrdinalIgnoreCase)
                || toBeRandomized.Equals("icon", StringComparison.OrdinalIgnoreCase))
            {
                Random rnd = new Random();
                int fCount = Directory.GetFiles("profileicon/", "*", SearchOption.TopDirectoryOnly).Length;
                int iconNumber = rnd.Next(0, fCount);

                //string url = "https://ddragon.leagueoflegends.com/cdn/img/champion/splash/" + traversal.First()["id"].ToString() + "_0.jpg";
                var filename = Path.GetFileName("C:/Users/matth/source/repos/Discord Bot/bin/Debug/net6.0/profileicon");
                Console.WriteLine(filename.ToString());

                var msgEmbed = new DiscordEmbedBuilder()
                {
                    Color = DiscordColor.Blurple,
                    //Description = traversal.First()["id"].ToString(),
                    Title = "Random Icon:",
                    Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                    {
                        Url = $"attachment:{filename}" + iconNumber.ToString() + ".png"
                    }
                };

                await ctx.RespondAsync(msgEmbed);

            }
            */
            else
            {
                await ctx.RespondAsync(">>> Not a valid command! Try: **champion, icon**.");
            }
        }

        //add better conventions
        //make a "translate" command that takes a language/champion, and translates respectively
        [Command("translate")]
        public async Task chineseName(CommandContext ctx, [RemainingText] string champName)
        {
            var jToken = JToken.Parse(championText);
            var data = jToken["data"];
            var jTokenCH = JToken.Parse(championTextCH);
            var dataCH = jTokenCH["data"];
            string cleanedName = CleanString.RemoveCharacters(CleanString.RemoveSpace(champName));

            //Console.WriteLine(cleanedName);
            if (((JObject)data).GetValue(cleanedName, StringComparison.OrdinalIgnoreCase) == null)
            {
                //await ctx.RespondAsync(">>> Not a valid champion.");
                var msgEmbedFail = new DiscordEmbedBuilder();
                msgEmbedFail.Color = DiscordColor.Red;
                msgEmbedFail.Description = "*Not a valid champion.*";
                msgEmbedFail.Title = "Error";
                msgEmbedFail.Build();

                await ctx.RespondAsync(msgEmbedFail);
                return;
            }
            var temp = ((JObject)data).GetValue(cleanedName, StringComparison.OrdinalIgnoreCase);
            //Console.WriteLine(temp.ToString());
            //var champContainer = data[champName];
            string champKey = temp["key"].ToString();
            //Console.WriteLine(champKey);
            //Console.WriteLine(champContainer.ToString());
            //await ctx.RespondAsync(">>> " + temp["id"].ToString() + " - **" + keyToName(champKey, 1).ToString() + "**");
            var msgEmbed = new DiscordEmbedBuilder();
            msgEmbed.Color = DiscordColor.Blurple;
            msgEmbed.Description = temp["id"].ToString() + " - **" + keyToName(champKey, 1).ToString() + "**";
            msgEmbed.Title = "Translation";
            msgEmbed.Build();

            await ctx.RespondAsync(msgEmbed);
            //var champID = data["id"]
        }

        //returns ranked stats of summoner
        [Command("rank")]
        public async Task rankedInfo(CommandContext ctx, [RemainingText]string summonerName)
        {
            string encryptedSummoner = RetrieveEncryptedSummoner(summonerName);

            if (encryptedSummoner == null || encryptedSummoner == "")
            {
                //await ctx.RespondAsync(">>> Not a valid summoner.");
                
                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Red;
                msgEmbed.Description = "*Not a valid summoner.*";
                msgEmbed.Title = "Error";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);
               

                return;
            }

            var leagueEndpoint = "https://na1.api.riotgames.com/lol/league/v4/entries/by-summoner/" + encryptedSummoner + "?api_key=" + apiKey;
            var leagueResult = client.GetAsync(leagueEndpoint).Result;
            var leagueJson = leagueResult.Content.ReadAsStringAsync().Result;
            var leagueInfo = JsonConvert.DeserializeObject<List<leagueV4>>(leagueJson);

            //Console.WriteLine(leagueJson);

            string info = ""; 

            if (leagueInfo.Count == 0 || leagueInfo == null)
            {
                //info = summonerName + ":";
                //await ctx.RespondAsync(">>> " + info + "\n   *This user does not have any ranked data.*");
                

                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Red;
                msgEmbed.Description = "*This user does not have any ranked data.*";
                msgEmbed.Title = getProperName(summonerName) + "'s Ranked Info";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);
                
            } else {
                //info += leagueInfo[0].summonerName + ":";
                for (int i = 0; i < leagueInfo.Count; i++)
                {
                    var league = leagueInfo[i];
                    double wr = Math.Round((((double)league.wins / ((double)league.wins + (double)league.losses)) * 100), 2);

                    info += "\n   *" + CleanString.RegexClean(league.queueType) + "* - **" + CleanString.RegexClean(league.tier) + " " + CleanString.RegexClean(league.rank) + "** (wr: " + wr + "%)";
                }
                //await ctx.RespondAsync(">>> " + info);
                
                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Blurple;
                msgEmbed.Description = info;
                msgEmbed.Title = leagueInfo[0].summonerName + "'s Ranked Info";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);
                
            }


        }

        //returns top 3 champion mastery
        [Command("mastery")]
        public async Task championMastery(CommandContext ctx, [RemainingText] string summonerName)
        {
            string encryptedSummoner = RetrieveEncryptedSummoner(summonerName);

            if (encryptedSummoner == null)
            {
                //await ctx.RespondAsync(">>> Not a valid summoner.");
                var msgEmbedFail = new DiscordEmbedBuilder();
                msgEmbedFail.Color = DiscordColor.Red;
                msgEmbedFail.Description = "*Not a valid summoner.*";
                msgEmbedFail.Title = "Error";
                msgEmbedFail.Build();

                await ctx.RespondAsync(msgEmbedFail);
                return;
            }

            var championEndpoint = "https://na1.api.riotgames.com/lol/champion-mastery/v4/champion-masteries/by-summoner/" + encryptedSummoner + "?api_key=" + apiKey;
            var championResult = client.GetAsync(championEndpoint).Result;
            var championJson = championResult.Content.ReadAsStringAsync().Result;
            var championInfo = JsonConvert.DeserializeObject<List<championV4>>(championJson);

            var jToken = JToken.Parse(championText);
            var data = jToken["data"];

            string info = "";

            for (int i = 0; i < 3; i++)
            {
                if (championInfo[i] != null)
                {
                    var champion = championInfo[i];
                    //Console.WriteLine(temp["title"].ToString());
                    var champContainer = data[keyToName(champion.championId.ToString())];


                    info += "\n   " + (i+1) + ": **" + keyToName(champion.championId.ToString())
                        + "**, " + champContainer["title"].ToString() + " - **" + champion.championPoints + "** mastery points.";
                }
            }
            var msgEmbed = new DiscordEmbedBuilder();
            msgEmbed.Color = DiscordColor.Blurple;
            msgEmbed.Description = info;
            msgEmbed.Title = getProperName(summonerName) + "'s Most Played:";
            msgEmbed.Build();

            await ctx.RespondAsync(msgEmbed);
        }

        //returns last game info of given summoner
        [Command("last")]
        public async Task lastGame(CommandContext ctx, [RemainingText] string summonerName)
        {
            string puuid = RetrievePUUID(summonerName);

            if (puuid == null || puuid == "")
            {
                //await ctx.RespondAsync(">>> Not a valid summoner.");

                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Red;
                msgEmbed.Description = "*Not a valid summoner.*";
                msgEmbed.Title = "Error";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);

                return;
            }

            //HttpClient client = new HttpClient();
            //Console.WriteLine(puuid);

            var leagueEndpoint = "https://americas.api.riotgames.com/lol/match/v5/matches/by-puuid/" + puuid + "/ids?start=0&count=20&api_key=" + apiKey;
            var leagueResult = client.GetAsync(leagueEndpoint).Result;
            var leagueJson = leagueResult.Content.ReadAsStringAsync().Result;
            var matchName = JsonConvert.DeserializeObject<String[]>(leagueJson);
            string matchId = matchName[0].ToString();
            
            Console.WriteLine(matchId);

            var matchEndpoint = "https://americas.api.riotgames.com/lol/match/v5/matches/" + matchId + "?api_key=" + apiKey;
            var matchResult = client.GetAsync(matchEndpoint).Result;
            var matchJson = matchResult.Content.ReadAsStringAsync().Result;
            var matchInfo = JsonConvert.DeserializeObject(matchJson);

            Console.WriteLine(matchInfo);

            /*
            string info = "";

            if (leagueInfo.Count == 0 || leagueInfo == null)
            {
                //info = summonerName + ":";
                //await ctx.RespondAsync(">>> " + info + "\n   *This user does not have any ranked data.*");


                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Red;
                msgEmbed.Description = "*This user does not have any ranked data.*";
                msgEmbed.Title = getProperName(summonerName) + "'s Ranked Info";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);

            }
            else
            {
                //info += leagueInfo[0].summonerName + ":";
                for (int i = 0; i < leagueInfo.Count; i++)
                {
                    var league = leagueInfo[i];
                    double wr = Math.Round((((double)league.wins / ((double)league.wins + (double)league.losses)) * 100), 2);

                    info += "\n   *" + CleanString.RegexClean(league.queueType) + "* - **" + CleanString.RegexClean(league.tier) + " " + CleanString.RegexClean(league.rank) + "** (wr: " + wr + "%)";
                }
                //await ctx.RespondAsync(">>> " + info);

                var msgEmbed = new DiscordEmbedBuilder();
                msgEmbed.Color = DiscordColor.Blurple;
                msgEmbed.Description = info;
                msgEmbed.Title = leagueInfo[0].summonerName + "'s Ranked Info";
                msgEmbed.Build();

                await ctx.RespondAsync(msgEmbed);

            }
            */


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
