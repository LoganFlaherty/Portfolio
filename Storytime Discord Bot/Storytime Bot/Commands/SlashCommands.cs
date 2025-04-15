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
        /* Code Templates
          #1 Building a slash command: 
          [SlashCommand("command_name","Description")]
          public async Task Method(InteractionContext ctx, [Option("name","Description")] datatype defaultval = 0)
          {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);
            string user = $"{ctx.Member.Id}"; //Gets discord ID.
                
            //Other Code
            
            await SendMessage(ctx, "", msgStr);
          }
         */

        readonly string characterJSONPath = $@"..\..\Commands\character_data.json";

        //Commands
        [SlashCommand("create_character", "To initialize your character with up to five custom properties.")]
        public async Task CreateCharacter(InteractionContext ctx, [Option("name", "Your character's name.")] string name = "",
            [Option("property_name_one", "The name of the property.")] string propName1 = "", [Option("property_value_one", "The value of said property.")] string propValue1 = "",
            [Option("property_name_two", "The name of the property.")] string propName2 = "", [Option("property_value_two", "The value of said property.")] string propValue2 = "",
            [Option("property_name_three", "The name of the property.")] string propName3 = "", [Option("property_value_three", "The value of said property.")] string propValue3 = "",
            [Option("property_name_four", "The name of the property.")] string propName4 = "", [Option("property_value_four", "The value of said property.")] string propValue4 = "",
            [Option("property_name_five", "The name of the property.")] string propName5 = "", [Option("property_value_five", "The value of said property.")] string propValue5 = "")
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            
            name.Trim();
            propName1.Trim();
            propValue1.Trim();
            propName2.Trim();
            propValue2.Trim();
            propName3.Trim();
            propValue3.Trim();
            propName4.Trim();
            propValue4.Trim();
            propName5.Trim();
            propValue5.Trim();

            string user = $"{ctx.Member.Id}"; //Gets discord ID.
            string msgStr = "";
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);

            //checks if the user and character exists.
            if (!charactersJson.Characters.ContainsKey(user))
            {
                charactersJson.Characters.Add(user, new Dictionary<string, Dictionary<string, string>>());
            }
            if (!charactersJson.Characters[user].ContainsKey(name))
            {
                //initalizes dict with all property variables
                Dictionary<string, string> characterDict = new Dictionary<string, string>();
                //Add filled properties
                if (propName1 != "")
                {
                    characterDict.Add(propName1, propValue1);
                }
                if (propName2 != "")
                {
                    characterDict.Add(propName2, propValue2);
                }
                if (propName3 != "")
                {
                    characterDict.Add(propName3, propValue3);
                }
                if (propName4 != "")
                {
                    characterDict.Add(propName4, propValue4);
                }
                if (propName5 != "")
                {
                    characterDict.Add(propName5, propValue5);
                }

                //Saves to JSON
                charactersJson.Characters[user].Add(name, characterDict);
                await ExportJSON(characterJSONPath, charactersJson);
                msgStr = $"Creation of {name} complete!";
            }
            else
            {
                msgStr = $"{name} already exists.";
            }

            await SendMessage(ctx, "Creating Character", msgStr);
        }

        [SlashCommand("delete_character", "To delete your characters.")]
        public async Task DeleteCharacter(InteractionContext ctx, [Option("confirmation", "Enter 'yes' to confirm")] string confirmation, [Option("name", "Character's name")] string name)
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);
            string user = $"{ctx.Member.Id}"; //Gets discord ID.

            confirmation.Trim().ToLower();

            if (confirmation == "yes" && charactersJson.Characters[user].ContainsKey(name))
            {
                charactersJson.Characters[user].Remove(name);
                msgStr += $"{name} deleted.";
            }
            else if (confirmation != "yes")
            {
                msgStr += "Not confirmed";
            }
            else
            {
                msgStr += "Character not found.";
            }

            await ExportJSON(characterJSONPath, charactersJson);
            await SendMessage(ctx, $"Deleting {name}", msgStr);
        }

        [SlashCommand("display_character", "Displays a character's sheet.")]
        public async Task DisplayCharacter(InteractionContext ctx, [Option("name", "The character's name to display.")] string name = "")
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);
            string user = $"{ctx.Member.Id}"; //Gets discord ID.

            if (charactersJson.Characters[user].ContainsKey(name))
            {
                foreach (var property in charactersJson.Characters[user][name])
                {
                    msgStr += $"    {property.Key}: {property.Value}";
                }
            }
            else
            {
                msgStr += "Character does not exist.";
            }

            await SendMessage(ctx, $"{name}", msgStr);
        }

        [SlashCommand("edit_character", "Edit a character by editing a properties value, removing a property, and or adding a new one.")]
        public async Task EditCharacter(InteractionContext ctx,
            [Option("character_name", "enter the name of the character you'd like to edit.")] string characterName = "",
            [Option("existing_property", "enter the name of an existing property.")] string exProp = "", [Option("existing_property_value", "enter the value of the existing property.")] string exPropVal = "",
            [Option("remove_existing_property", "True or False?")] bool removeProp = false,
            [Option("new_property", "enter the name of a new property.")] string newProp = "", [Option("new_property_value", "enter the value of the new property.")] string newPropVal = "")
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";
            string user = $"{ctx.Member.Id}"; //Gets discord ID.
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);

            //checks if the user and character exists.
            if (!charactersJson.Characters.ContainsKey(user) || !charactersJson.Characters[user].ContainsKey(characterName))
            {
                msgStr += "Character does not exist.";
            }
            else
            {
                if (charactersJson.Characters[user][characterName].ContainsKey(exProp) && removeProp == false)
                {
                    charactersJson.Characters[user][characterName][exProp] = exPropVal;
                    msgStr += $"{exProp} edited with value: {exPropVal}";
                }
                else if (removeProp && charactersJson.Characters[user][characterName].ContainsKey(exProp))
                {
                    charactersJson.Characters[user][characterName].Remove(exProp);
                    msgStr += $"{exProp} removed.";
                }
                
                if (newProp != "" && !charactersJson.Characters[user][characterName].ContainsKey(newProp))
                {
                    charactersJson.Characters[user][characterName].Add(newProp, newPropVal);
                    msgStr += $"{newProp} added with value: {newPropVal}";
                }
                
                if (msgStr == "")
                {
                    msgStr += "No edits could be made.";
                }
                
            }

            await ExportJSON(characterJSONPath, charactersJson);
            msgStr += "Character editing complete!";

            await SendMessage(ctx, "Editing Character", msgStr);
        }

        [SlashCommand("roll", "Roll abitrary dice.")]
        public async Task Roll(InteractionContext ctx, [Option("amount", "Amount of dice to roll.")] double amount = 1, [Option("sides", "numbers of sides the dice have.")] double sides = 20, 
            [Option("modifier", "Total of modifiers to the roll.")] double mod = 0)
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";

            int sum = RollDice(sides, amount, out msgStr);

            msgStr += $"Total = {sum} + {mod} = {sum + mod}";

            await SendMessage(ctx, "Rolling Dice", msgStr);
        }

        [SlashCommand("roll_with_character", "Roll abitrary dice with up to two character properties.")]
        public async Task RollWithCharacter(InteractionContext ctx, [Option("amount", "Amount of dice to roll.")] double amount = 1, [Option("sides", "numbers of sides the dice have.")] double sides = 20,
            [Option("name", "The character's name to display.")] string name = "", [Option("property_one", "Number property to use.")] string prop1 = "", 
            [Option("property_two", "Number property to use.")] string prop2 = "")
        {
            //Defers response so it doesn't time out in 3 seconds.
            await ctx.DeferAsync();
            string msgStr = "";
            string user = $"{ctx.Member.Id}"; //Gets discord ID.
            CharactersJSON charactersJson = await GetJSON(characterJSONPath);

            if (charactersJson.Characters[user].ContainsKey(name))
            {
                int sum = RollDice(sides, amount, out msgStr);
                bool prop1Exist = charactersJson.Characters[user][name].ContainsKey(prop1);
                bool prop2Exist = charactersJson.Characters[user][name].ContainsKey(prop2);
                double prop1Val = 0;
                double prop2Val = 0;

                //Checks if the properties passed exist, then parses them.
                if (prop1Exist)
                {
                    try
                    {
                        prop1Val = Int32.Parse(charactersJson.Characters[user][name][prop1]);
                    }
                    catch
                    {
                        msgStr = $"{prop1} is not a number.\n";
                    }
                }

                if (prop2Exist)
                {
                    try
                    {
                        prop2Val = Int32.Parse(charactersJson.Characters[user][name][prop1]);
                    }
                    catch
                    {
                        msgStr = $"{prop2} is not a number.\n";
                    }
                }

                //Evals which properties are true to build message string.
                if (prop1Exist && prop2Exist)
                {
                    msgStr += $"Modifiers: {prop1Val} + {prop2Val}";
                    msgStr += $"Total = {sum + prop1Val + prop2Val}";
                }
                else if (prop1Exist)
                {
                    msgStr += $"Modifiers: {prop1Val}";
                    msgStr += $"Total = {sum + prop1Val}";
                }
                else if (prop2Exist)
                {
                    msgStr += $"Modifiers: {prop2Val}";
                    msgStr += $"Total = {sum + prop2Val}";
                }
                else
                {
                    msgStr += $"Modifiers:";
                    msgStr += $"Total = {sum}";
                }
            }
            else
            {
                msgStr += "Character or property does not exist";
            }

            await SendMessage(ctx, "Rolling Dice With Character", msgStr);
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
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading from JSON: {ex.Message}");
                return new CharactersJSON();  // Return an empty object to avoid null reference
            }
        }

        //Exports new character data to a specified JSON
        private async Task ExportJSON(string path, CharactersJSON characterJson)
        {
            try
            {
                string serializedJson = JsonConvert.SerializeObject(characterJson, Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(path))
                {
                    await sw.WriteLineAsync(serializedJson);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to JSON: {ex.Message}");
            }
        }

        //Rolls arbitrary dice
        private int RollDice(double sides, double amount, out string msgStr)
        {
            msgStr = "Rolls: ";
            sides += 1;
            Random rand = new Random();
            int sum = 0;
            for (int i = 0; i < amount; i++)
            {
                int roll = rand.Next(1, (int)sides);
                sum += roll;
                if (i == amount - 1)
                {
                    msgStr += $"{roll}      ";
                }
                else
                {
                    msgStr += $"{roll}, ";
                }
            }

            return sum;
        }

        //Sends an embedded message to the interaction.
        private async Task SendMessage(InteractionContext ctx, string title, string msgStr)
        {
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = msgStr,
                Color = DiscordColor.Purple
            };
            await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(eMessage));
        }
    }
}
