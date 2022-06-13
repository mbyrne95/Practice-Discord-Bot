using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot
{
    internal class championV4
    {
        public long? championId { get; set; }
        public int? championLevel { get; set; }
        public int? championPoints { get; set; }
        public long? lastPlayTime { get; set; }
        public long? championPointsSinceLastLevel { get; set; }
        public long? championPointsUntilNextLevel { get; set; }
        public bool chestGranted { get; set; }
        public int? tokensEarned { get; set; }
        public string? summonerId { get; set; }



    }
}
