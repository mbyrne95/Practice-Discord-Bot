using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = System.IO.File.ReadAllText("token.txt"),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<PrimaryModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}