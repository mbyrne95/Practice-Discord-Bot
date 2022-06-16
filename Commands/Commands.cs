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
using System.Net;

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

        //hard coded relative to https://static.developer.riotgames.com/docs/lol/queues.json (6/15)
        private string getQueueType(string id)
        {
            switch (id)
            {
                case "400":
                    return "5v5 Draft Pick";
                    break;
                case "420":
                    return "5v5 Ranked Solo";
                    break;
                case "430":
                    return "5v5 Blind Pick";
                    break;
                case "440":
                    return "5v5 Ranked Flex";
                    break;
                case "450":
                    return "5v5 ARAM";
                    break;
                default: return "Summoner's Rift";
            }
        }

        //returns the "serveremoji" version of the respective summoner - needs to be edited relative to server info
        private string getSummonerSpell(string id)
        {
            switch (id)
            {
                case "21":
                    return "<:Barrier:986740464281600030>";
                    break;
                case "1":
                    return "<:Cleanse:986740465233694770>";
                    break;
                case "14":
                    return "<:Ignite:986740465908998274>";
                    break;
                case "3":
                    return "<:Exhaust:986740466726871090>";
                    break;
                case "4":
                    return "<:Flash:986740467297316875>";
                    break;
                case "6":
                    return "<:Ghost:986740468794683422>";
                    break;
                case "7":
                    return "<:Heal:986740470099116082>";
                    break;
                case "12":
                    return "<:Teleport:986855983559081984>";
                    break;
                case "13":
                    return "<:Clarity:986740471273488484>";
                    break;
                case "11":
                    return "<:Smite:986740472187850752>";
                    break;
                case "32":
                    return "<:Snowball:986740472930250832>";
                    break;
                case "39":
                    return "<:Snowball:986740472930250832>";
                    break;
                default: return "Unknown";
            }
        }

        
        //wip, turn item id into name
        //will become private method when complete
        [Command("test")]
        public async Task itemIdtoName(CommandContext ctx)
        {
            String[] items = new String[6];
            String[] itemName = new String[6];

            /*
            var json = new WebClient().DownloadString("http://ddragon.leagueoflegends.com/cdn/12.11.1/data/en_US/item.json");
            var itemJson = JsonConvert.DeserializeObject(json);
            var itemJsonitemJObject = (JObject)JsonConvert.DeserializeObject(json);
            var itemBucket = itemJsonitemJObject.Children().First().Next.Next.Next;
            //var itemContainer = itemBucket.Children().First();

            using (var reader = new JsonTextReader(new StringReader(json)))
            {
                while (reader.Read())
                {
                    for (int i = 0; i < itemBucket.Children().Count(); i++)
                    {
                        for (int j = 0; j < items.Length; j++)
                        {
                            if (items[j] == reader.Value)
                            {
                                itemName[j] =
                            }
                        }
                    }


                }
            }

            */

            //


            /*
            for (int i = 0; i < itemBucket.Children().Count(); i++)
            {
                if itemContainer[""]
            }
            */
            //Console.WriteLine(itemContainer[""].ToString());
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
                msgEmbedFail.Color = DiscordColor.Black;
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
                msgEmbed.Color = DiscordColor.Black;
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
                msgEmbedFail.Color = DiscordColor.Black;
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
        //data to add: item build, skill order
        [Command("last")]
        public async Task lastGame(CommandContext ctx, [RemainingText] string summonerName)
        {
            string puuid = RetrievePUUID(summonerName);

            //check null condition
            if (puuid == null || puuid == "")
            {
                var msgEmbedFail = new DiscordEmbedBuilder() {
                    Color = DiscordColor.Black,
                    Description = "*Not a valid summoner.*",
                    Title = "Error",
                };
                msgEmbedFail.Build();
                await ctx.RespondAsync(msgEmbedFail);

                return;
            }

            //getting match name
            var leagueEndpoint = "https://americas.api.riotgames.com/lol/match/v5/matches/by-puuid/" + puuid + "/ids?start=0&count=20&api_key=" + apiKey;
            var leagueResult = client.GetAsync(leagueEndpoint).Result;
            var leagueJson = leagueResult.Content.ReadAsStringAsync().Result;
            var matchName = JsonConvert.DeserializeObject<String[]>(leagueJson);
            string matchId = matchName[0].ToString();

            //getting match info
            var matchEndpoint = "https://americas.api.riotgames.com/lol/match/v5/matches/" + matchId + "?api_key=" + apiKey;
            var matchResult = client.GetAsync(matchEndpoint).Result;
            var matchJson = matchResult.Content.ReadAsStringAsync().Result;
            var matchInfo = (JObject)JsonConvert.DeserializeObject(matchJson);

            //navigating to champion bucket
            var infoBucket = matchInfo.Children().First();
            infoBucket = infoBucket.Next;
            var participantBucket = infoBucket.Children()["participants"].Children().First();

            string gameMode = getQueueType(infoBucket.Children()["queueId"].First().ToString());

            for (int i = 0; i < infoBucket.Children()["participants"].Children().Count(); i++)
            {

                var participantPUUID = participantBucket["puuid"].ToString();

                if (puuid == participantPUUID)
                {
                    break;
                }

                participantBucket = participantBucket.Next;
            }

            //getting data
            string assists = participantBucket["assists"].ToString();
            string deaths = participantBucket["deaths"].ToString();
            string kills = participantBucket["kills"].ToString();

            string kda = kills + "/" + deaths + "/" + assists;
            string champName = participantBucket["championName"].ToString();

            string winData = participantBucket["win"].ToString();
            bool win = true;
            var thisColor = DiscordColor.Blurple;

            //item build
            String[] itemBuild = { participantBucket["item0"].ToString(), participantBucket["item1"].ToString(), participantBucket["item2"].ToString(),
                participantBucket["item3"].ToString(), participantBucket["item4"].ToString(), participantBucket["item5"].ToString() };

            //checking win condition
            if (winData.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                thisColor = DiscordColor.Red;
                win = false;
            }

            string description = "Win! :partying_face:";
            if (!win)
            {
                description = "Loss! :persevere:";
            }

            //MESSAGE BUILDING

            var msgEmbed = new DiscordEmbedBuilder() {
                
                Color = thisColor,
                Description = description,
                Title = getProperName(summonerName) + "'s Last Game:",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
                {
                    Url = "https://ddragon.leagueoflegends.com/cdn/img/champion/tiles/" + champName + "_0.jpg"
                }
            };

            string fieldDescriptor = "";
            string fieldValues = "";

            string fieldDescriptorBase = "Level: " + 
                "\nKDA: " +
                "\nTotal Gold: " +
                "\nLargest MultiKill: " +
                "\n\nVision Score: " + 
                "\nWards Placed: " + 
                "\nWards Killed: ";

            string fieldValuesBase = participantBucket["champLevel"].ToString() + "\n" + 
                kda + "\n" +
                participantBucket["goldEarned"].ToString() + "\n" +
                participantBucket["largestMultiKill"].ToString() + "\n\n" +
                participantBucket["visionScore"].ToString() + "\n" +
                participantBucket["wardsPlaced"].ToString() + "\n" +
                participantBucket["wardsKilled"].ToString();

            string summonerSpells = getSummonerSpell(participantBucket["summoner1Id"].ToString()) + " " +
                getSummonerSpell(participantBucket["summoner2Id"].ToString());

            msgEmbed.AddField("Champion:", champName, true);
            msgEmbed.AddField("Game Mode:", gameMode, true);
            msgEmbed.AddField("Summoner Spells:", summonerSpells, false);
            
            if (participantBucket["firstBloodKill"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                fieldDescriptor += "**First Blood!**\n";
                fieldValues += "\u200b\n";
            }
            if (participantBucket["firstTowerKill"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase) ||
                participantBucket["firstTowerAssist"].ToString().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                fieldDescriptor += "**First Tower!**\n";
                fieldValues += "\u200b\n";
            }

            fieldDescriptor += fieldDescriptorBase;
            fieldValues += fieldValuesBase;

            msgEmbed.AddField("Info:", fieldDescriptor, true);
            msgEmbed.AddField("\u200b", fieldValues, true);

            msgEmbed.Build();

            await ctx.RespondAsync(msgEmbed);

        }
    }
}
