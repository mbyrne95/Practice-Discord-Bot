using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;

namespace DiscordBot
{
    public class HelpFormatter : DefaultHelpFormatter
    {
        public HelpFormatter(CommandContext ctx) : base(ctx)
        {

        }
        
        public override CommandHelpMessage Build()
        {
            
            return base.Build();
        }
    }
}
