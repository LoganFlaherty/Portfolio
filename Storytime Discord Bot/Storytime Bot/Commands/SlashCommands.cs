using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;

namespace Storytime_Bot.Commands
{
    public class SlashCommands : ApplicationCommandModule
    {
        /* Code Library
          #1 Building a slash command: 
          [SlashCommand("command_name","Description")]
          public async Task Method(InteractionContext ctx, [Option("name","Description")] datatype defaultval = 0)
          {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr;
                
            //Other Code
            
            //Sends a response to the interaction.
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Title",
                Description = msgStr,
                Color = DiscordColor.Purple
            };
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(eMessage));
          }
         */

        readonly string characterJSONPath = $@"..\..\Commands\character_data.json";

        //Commands
        [SlashCommand("create_character", "To store your character data into the bot.")]
        public async Task CreateCharacter(InteractionContext ctx, [Option("name", "Your characters name.")] string name = "", [Option("property_name", "The name of the property.")] string propName = "", 
            [Option("property_value", "The value of said property.")] string propValue = "")
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            
            name.Trim();
            propName.Trim();
            propValue.Trim();
            string user = $"{ctx.Member.Id}"; //Gets discord ID.
            string msgStr;
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);

            //checks if the user and character exists.
            if (!charactersJson.Characters.ContainsKey(user))
            {
                charactersJson.Characters.Add(user, new Dictionary<string, Dictionary<string, string>>());
            }
            if (!charactersJson.Characters[user].ContainsKey(name))
            {
                Dictionary<string, string> characterDict = new Dictionary<string, string> {
                    { propName, propValue }
                };
                charactersJson.Characters[user].Add(name, characterDict);
                await ExportCharacter(charactersJson);
                msgStr = $"Creation of {name} complete!";
            }
            else
            {
                msgStr = $"{name} already exists.";
            }

            //Sends a response to the interaction.
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Character Creation",
                Description = msgStr,
                Color = DiscordColor.Purple
            };
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(eMessage));
        }

        [SlashCommand("roll", "Roll abitrary dice.")]
        public async Task Roll(InteractionContext ctx, [Option("amount", "Amount of dice to roll.")] double amount = 1, [Option("sides", "numbers of sides the dice have.")] double sides = 20, 
            [Option("modifier", "Total modifiers to the roll.")] double mod = 0)
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";

            sides += 1;
            Random rand = new Random();
            int sum = 0;
            for (int i = 0; i < amount; i++) 
            {
                int temp = rand.Next(1, (int)sides);
                sum += temp;
                if (i == amount - 1)
                {
                    msgStr += $"{temp}\n";
                }
                else
                {
                    msgStr += $"{temp}, ";
                }
            }

            msgStr += $"Total = {sum} + {mod} = {sum + mod}";

            //Sends a response to the interaction.
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Rolling Dice",
                Description = msgStr,
                Color = DiscordColor.Purple
            };
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(eMessage));
        }

        //Helper functions
        //Gets a specified JSON file.
        private async Task<CharactersJSON> GetJSON(string path)
        {
            try
            {
                string json;
                using (var fs = File.OpenRead(path))
                using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                {
                    json = await sr.ReadToEndAsync();
                    CharactersJSON charactersJson = JsonConvert.DeserializeObject<CharactersJSON>(json);
                    sr.Close();
                    return charactersJson ?? new CharactersJSON
                    {
                        Characters = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>()
                    }; ;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading JSON: {ex.Message}");
                return new CharactersJSON();  // Return an empty object to avoid null reference
            }
        }

        //Exports new data to charater_data.json
        private async Task ExportCharacter(CharactersJSON characterJson)
        {
            try
            {
                string serializedJson = JsonConvert.SerializeObject(characterJson, Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(characterJSONPath))
                {
                    await sw.WriteLineAsync(serializedJson);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing JSON: {ex.Message}");
            }
        }
    }
}
