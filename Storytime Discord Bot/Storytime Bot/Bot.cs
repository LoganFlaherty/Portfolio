using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Storytime_Bot.Commands;

namespace Storytime_Bot
{
    public class Bot
    {
        //Fields
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        //Methods
        public async Task RunAsync()
        {
            //Load config and intialize
            var json = string.Empty;
            string relativePath = Path.Combine("..", "..", "config.json");
            using (var fs = File.OpenRead(relativePath))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            json = await sr.ReadToEndAsync();

            var configJson = JsonConvert.DeserializeObject<ConfigJSON>(json);

            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.AllUnprivileged.AddIntent(DiscordIntents.GuildMembers),
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(1)
            });

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
            };

            //Register Command to a specified guild.
            Commands = Client.UseCommandsNext(commandsConfig);
            var slashCommandsConfig = Client.UseSlashCommands();
            slashCommandsConfig.RegisterCommands<RollCommands>(guildId); //Insert your server Id here.

            //Connects client
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
