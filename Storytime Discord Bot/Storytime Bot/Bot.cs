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
        //Properties
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        //Methods
        //Reads from the config.json file to set the client, registers commands, and connects to discord.
        public async Task RunAsync()
        {
            //Loads config and intializes
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

            //New client with config
            Client = new DiscordClient(config);

            //Set timeout
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(5)
            });

            //Register slash commands to a specified guild.
            var slashCommandsConfig = Client.UseSlashCommands();
            slashCommandsConfig.RegisterCommands<SlashCommands>(1047929615827619890); //Insert your server Id here. For testing the commands immediately.
            slashCommandsConfig.RegisterCommands<SlashCommands>(); //Global registration of commands, but take up to 24 hours to fulfill.

            //Connects client
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
