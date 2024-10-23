using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace Storytime_Bot.Commands
{
    /* Code Library
     * #1 Building a slash command
     * [SlashCommand("Command name","Description")]
     * public async Task Method(InteractionContext ctx, [Option("Name","Description")] datatype defaultval = 0)
     * {
     *  
     * }
     * 
     * #2 Sending the command
     * var eMessage = new DiscordEmbedBuilder()
        {
            Title = "Title",
            Description = string,
            Color = DiscordColor.White
        };
        await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Message optional"));
        await ctx.Channel.SendMessageAsync(embed: eMessage);
     */

    public class RollCommands : ApplicationCommandModule
    {
        [SlashCommand("roll", "To roll dice")]
        public async Task Roll(InteractionContext ctx, [Option("success_range", "Only enter the beginning of the range, 12 by default")] double SR = 12, [Option("dice_amount", "Enter the number of dice being rolled, 1 by default")] double dice = 1)
        {
            List<int> rollList = new List<int>();
            string str = "{";
            int successes = 0;
            Random rand = new Random();

            //Fills the list with all dice rolls.
            for (int i = 0; i < dice; i++)
            {
                rollList.Add(rand.Next(1, 13));
            }

            //Adds the list of rolls to the string then compares them to the SR.
            foreach (int roll in rollList)
            {
                int index = rollList.IndexOf(roll);
                if (index != rollList.Count - 1)
                {
                    str += $"{roll}, ";
                }
                else
                {
                    str += $"{roll}";
                }

                if (roll >= SR)
                {
                    successes++;
                }
            }
            str += "}\n\n";

            //Adds success or fail message to the string.
            if (successes == 0)
            {
                str += "Fail";
            }
            else
            {
                str += $"Successes: {successes}";
            }

            //Builds the discord message.
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Results:",
                Description = str,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Rolling dice..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("create_character", "Use this to create your character data")]
        public async Task CreateCharacter(InteractionContext ctx, 
            [Option("name", "Enter the name")] string name = "Unknown", 
            [Option("expertise", "Enter the prowess")] double expertise = 0, 
            [Option("expertise_prestige", "Enter the prestige")] double expertisePrestige = 0, 
            [Option("Faith", "Enter the prowess")] double faith = 0, 
            [Option("faith_prestige", "Enter the prestige")] double faithPrestige = 0, 
            [Option("influence", "Enter the prowess")] double influence = 0, 
            [Option("influence_prestige", "Enter the prestige")] double influencePrestige = 0, 
            [Option("perception", "Enter the prowess")] double perception = 0, 
            [Option("perception_prestige", "Enter the prestige")] double perceptionPrestige = 0, 
            [Option("physique", "Enter the prowess")] double physique = 0, 
            [Option("physique_prestige", "Enter the prowess")] double physiquePrestige = 0, 
            [Option("tag1", "Enter a tag")] string tag1 = "", 
            [Option("tag2", "Enter a tag")] string tag2 = "", 
            [Option("tag3", "Enter a tag")] string tag3 = "")
        {
            name.Trim();
            tag1.Trim();
            tag2.Trim();
            tag3.Trim();

            List<string> tags = new List<string>();
            if (tag1 != "")
            {
                tags.Add(tag1);
            }
            if (tag2 != "")
            {
                tags.Add(tag2);
            }
            if (tag3 != "")
            {
                tags.Add(tag3);
            }

            string user = $"{ctx.Member}";
            string characterPath = CharacterPath(user);
            Character character = new Character(name, expertise, expertisePrestige, faith, faithPrestige, influence, influencePrestige, perception, perceptionPrestige, physique, physiquePrestige, tags);
            List<Character> characterList = Import(characterPath);
            characterList.Add(character);
            Export(characterPath, characterList);

            string msgStr = character.ToString();

            //Builds the discord message.
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Character Creation",
                Description = $"Complete!\n\n{msgStr}",
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Creating character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("Display_character", "Use this to display all of a character's data")]
        public async Task DisplayCharacter(InteractionContext ctx, [Option("name", "Enter character's name")] string nameToFind)
        {
            nameToFind.Trim();
            string user = $"{ctx.Member}";
            string msgStr = string.Empty;
            string characterPath = CharacterPath(user);
            if (!File.Exists(characterPath))
            {
                msgStr = "Character file not found.";
            }
            else if (Import(characterPath).Count == 0)
            {
                msgStr = "No created characters.";
            }
            else
            {
                List<Character> characterList = Import(characterPath);
                foreach (Character character in characterList)
                {
                    if (character.Name == nameToFind)
                    {
                        msgStr = character.ToString();
                    }
                    else
                    {
                        msgStr = "Character not found.";
                    }
                }
            }

            //Builds discord message
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = nameToFind,
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Loading character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("list_all_characters", "Use this to list the names of all your characters")]
        public async Task ListAllCharacters(InteractionContext ctx)
        {
            string user = $"{ctx.Member}";
            string characterPath = CharacterPath(user);
            string msgStr = string.Empty;
            if (!File.Exists(characterPath))
            {
                msgStr = "Character file not found.";
            }
            else if (Import(characterPath).Count == 0)
            {
                msgStr = "No created characters.";
            }
            else
            {
                foreach (Character character in Import(characterPath))
                {
                    msgStr += $"- {character.Name}\n";
                }
            }

            //Builds discord message
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = "Your characters",
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Loading character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("edit_character", "Use to edit a character")]
        public async Task EditCharacter(InteractionContext ctx, [Option("name", "Enter the name of the character you'd like to edit")] string nameToEdit, 
            [Choice("name", "name")]
            [Choice("expertise", "expertise")][Choice("expertise_reserve", "expertise_reserve")][Choice("expertise_prestige", "expertise_prestige")]
            [Choice("faith", "faith")][Choice("faith_reserve", "faith_reserve")][Choice("faith_prestige", "faith_prestige")]
            [Choice("influence", "influence")][Choice("influence_reserve", "influence_reserve")][Choice("influence_prestige", "influence_prestige")]
            [Choice("perception", "perception")][Choice("perception_reserve", "perception_reserve")][Choice("perception_prestige", "perception_prestige")]
            [Choice("physique", "physique")][Choice("physique_reserve", "physique_reserve")][Choice("physique_prestige", "physique_prestige")]
            [Choice("add_tag", "add_tag")][Choice("remove_tag", "remove_tag")][Option("property", "choose which property to edit")] string choice, 
            [Option("Input", "Enter the new value")] string input)
        {
            nameToEdit.Trim();
            string user = $"{ctx.Member}";
            string msgStr = string.Empty;
            string characterPath = CharacterPath(user);
            if (!File.Exists(characterPath))
            {
                msgStr = "Character file not found.";
            }
            else if (Import(characterPath).Count == 0)
            {
                msgStr = "No created characters.";
            }
            else
            {
                List<Character> characterList = Import(characterPath);
                for (int i = 0; i < characterList.Count; i++)
                {
                    if (characterList[i].Name == nameToEdit)
                    {
                        EditCharacterProperty(characterList[i], choice, input);
                        Export(characterPath, characterList);
                        msgStr = "Complete!\n\n";
                        msgStr += characterList[i].ToString();
                    }
                    else
                    {
                        msgStr = "Character not found.";
                    }
                }
            }

            //Builds discord message
            var eMessage = new DiscordEmbedBuilder()
            {
                Title = $"Edit {nameToEdit}",
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Editing character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("delete_character", "Use to delete a character")]
        public async Task DeleteCharacter(InteractionContext ctx, [Option("name", "Enter the name of the character you'd like to delete")] string nameToDelete)
        {
            nameToDelete.Trim();
            string msgStr = string.Empty;
            string user = $"{ctx.Member}";
            string characterPath = CharacterPath(user);
            List<Character> characterList = Import(characterPath);

            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].Name == nameToDelete)
                {
                    characterList.Remove(characterList[i]);
                    Export(characterPath, characterList);
                    msgStr = "Complete!";
                }
                else
                {
                    msgStr = "Character not found.";
                }
            }

            var eMessage = new DiscordEmbedBuilder()
            {
                Title = $"Delete {nameToDelete}",
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Deleting character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("rest_character","Use to give your character a rest")]
        public async Task RestCharacter(InteractionContext ctx, [Option("name", "Enter the name of the character you'd like to rest")] string nameToRest)
        {
            nameToRest.Trim();
            string msgStr = string.Empty;
            string user = $"{ctx.Member}";
            string characterPath = CharacterPath(user);
            List<Character> characterList = Import(characterPath);

            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].Name == nameToRest)
                {
                    EditCharacterProperty(characterList[i], "expertise_reserve", $"{characterList[i].Expertise}");
                    EditCharacterProperty(characterList[i], "faith_reserve", $"{characterList[i].Faith}");
                    EditCharacterProperty(characterList[i], "influence_reserve", $"{characterList[i].Influence}");
                    EditCharacterProperty(characterList[i], "perception_reserve", $"{characterList[i].Perception}");
                    EditCharacterProperty(characterList[i], "physique_reserve", $"{characterList[i].Physique}");
                    Export(characterPath, characterList);
                    msgStr = "Complete!\n\n";
                    msgStr += characterList[i].ToString();
                }
                else
                {
                    msgStr = "Character not found.";
                }
            }

            var eMessage = new DiscordEmbedBuilder()
            {
                Title = $"Rest {nameToRest}",
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Resting character..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        [SlashCommand("roll_using_character","Use this to roll using a character's stats")]
        public async Task RollUsingCharacter(InteractionContext ctx, 
            [Option("name", "Enter the name of the character you'd like to use")] string name,
            [Choice("expertise","expertise")][Choice("faith","faith")][Choice("influence","influence")][Choice("perception","perception")]
            [Choice("physique","physique")][Option("using_attribute","Pick the attribute used for the roll")] string choice,
            [Option("use_dice_reserve", "Enter the amount of dice in the attribute's reserve to use")] double reserveDiceUsed = 0, [Option("pressure","Enter the pressure")] double pressure = 0)
        {
            name.Trim();
            string msgStr = string.Empty;
            string user = $"{ctx.Member}";
            string characterPath = CharacterPath(user);
            List<Character> characterList = Import(characterPath);
            List<double> rolls = new List<double>();
            int successes = 0;

            for (int i = 0; i < characterList.Count; i++)
            {
                if (characterList[i].Name == name)
                {
                    msgStr = "{";
                    double successRange = 12 - pressure;
                    //TODO integrate faith being used
                    switch (choice)
                    {
                        case "expertise":
                            successRange -= characterList[i].Expertise;
                            EditCharacterProperty(characterList[i], "expertise_reserve", $"{characterList[i].ExpertiseReserve - reserveDiceUsed}");
                            break;
                        case "faith":
                            successRange -= characterList[i].Faith;
                            EditCharacterProperty(characterList[i], "faith_reserve", $"{characterList[i].FaithReserve - reserveDiceUsed}");
                            break;
                        case "influence":
                            successRange -= characterList[i].Influence;
                            EditCharacterProperty(characterList[i], "influence_reserve", $"{characterList[i].InfluenceReserve - reserveDiceUsed}");
                            break;
                        case "perception":
                            successRange -= characterList[i].Perception;
                            EditCharacterProperty(characterList[i], "perception_reserve", $"{characterList[i].PerceptionReserve - reserveDiceUsed}");
                            break;
                        case "physique":
                            successRange -= characterList[i].Physique;
                            EditCharacterProperty(characterList[i], "physique_reserve", $"{characterList[i].PhysiqueReserve - reserveDiceUsed}");
                            break;
                        default:
                            break;
                    }

                    Random rand = new Random();
                    for (int r = 0; r < reserveDiceUsed + 1; r++)
                    {
                        rolls.Add(rand.Next(1, 13));
                    }

                    //Adds the list of rolls to the string then compares them to the SR.
                    foreach (int roll in rolls)
                    {
                        int index = rolls.IndexOf(roll);
                        if (index != rolls.Count - 1)
                        {
                            msgStr += $"{roll}, ";
                        }
                        else
                        {
                            msgStr += $"{roll}";
                        }

                        if (roll >= successRange)
                        {
                            successes++;
                        }
                    }
                    msgStr += "}\n\n";

                    //Adds success or fail message to the string.
                    if (successes == 0)
                    {
                        msgStr += "Fail";
                    }
                    else
                    {
                        msgStr += $"Successes: {successes}";

                        //Puts the extra successes back into the dice reserve.
                        if (successes > 1)
                        {
                            double successRem = successes - 1;
                            switch (choice)
                            {
                                case "expertise":
                                    if (successRem > characterList[i].Expertise)
                                    {
                                        successRem = characterList[i].Expertise;
                                        EditCharacterProperty(characterList[i], "expertise_reserve", $"{successRem}");
                                    }
                                    else
                                    {
                                        EditCharacterProperty(characterList[i], "expertise_reserve", $"{characterList[i].ExpertiseReserve + successRem}");
                                    }
                                    break;
                                case "faith":
                                    if (successRem > characterList[i].Faith)
                                    {
                                        successRem = characterList[i].Faith;
                                        EditCharacterProperty(characterList[i], "faith_reserve", $"{successRem}");
                                    }
                                    else
                                    {
                                        EditCharacterProperty(characterList[i], "faith_reserve", $"{characterList[i].FaithReserve + successRem}");
                                    }
                                    break;
                                case "influence":
                                    if (successRem > characterList[i].Influence)
                                    {
                                        successRem = characterList[i].Influence;
                                        EditCharacterProperty(characterList[i], "influence_reserve", $"{successRem}");
                                    }
                                    else
                                    {
                                        EditCharacterProperty(characterList[i], "influence_reserve", $"{characterList[i].InfluenceReserve + successRem}");
                                    }
                                    break;
                                case "perception":
                                    if (successRem > characterList[i].Perception)
                                    {
                                        successRem = characterList[i].Perception;
                                        EditCharacterProperty(characterList[i], "perception_reserve", $"{successRem}");
                                    }
                                    else
                                    {
                                        EditCharacterProperty(characterList[i], "perception_reserve", $"{characterList[i].PerceptionReserve + successRem}");
                                    }
                                    break;
                                case "physique":
                                    if (successRem > characterList[i].Physique)
                                    {
                                        successRem = characterList[i].Physique;
                                        EditCharacterProperty(characterList[i], "physique_reserve", $"{successRem}");
                                    }
                                    else
                                    {
                                        EditCharacterProperty(characterList[i], "physique_reserve", $"{characterList[i].PhysiqueReserve + successRem}");
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    Export(characterPath, characterList);
                }
                else
                {
                    msgStr = "Character not found.";
                }
            }

            var eMessage = new DiscordEmbedBuilder()
            {
                Title = $"Rolling using {name}",
                Description = msgStr,
                Color = DiscordColor.White
            };
            await ctx.CreateResponseAsync(DSharpPlus.InteractionResponseType.ChannelMessageWithSource, new DSharpPlus.Entities.DiscordInteractionResponseBuilder().WithContent("Rolling dice..."));
            await ctx.Channel.SendMessageAsync(embed: eMessage);
        }

        //Command Methods
        //Creates a path with discord user id.
        private string CharacterPath(string user)
        {
            return $@"C:\Users\Logan\OneDrive\Documents\Private Projects\Discord\Storytime Bot\Storytime Bot\Commands\CharacterData\{user}.txt";
        }
        
        //Exports character data to a user file.
        private void Export(string characterPath, List<Character> characterList)
        {
            //Writes the inputs into the user's character file.;
            if (!File.Exists(characterPath))
            {
                //Create character file
                File.Create(characterPath).Close();
            }

            using (StreamWriter sw = new StreamWriter(characterPath))
            {
                foreach (Character character in characterList)
                {
                    sw.WriteLine(character.FileToString());
                }
                sw.Close();
            }
        }

        //Imports all character data in a list.
        private List<Character> Import(string characterPath)
        {
            List<Character> list = new List<Character>();
            if (File.Exists(characterPath))
            {
                using (StreamReader sr = new StreamReader(characterPath))
                {
                    string line = string.Empty;
                    while (line != null)
                    {
                        line = sr.ReadLine();
                        if (line != null)
                        {
                            string[] dataArray = line.Split(',');
                            List<string> tagData = new List<string>();
                            if (dataArray.Length > 16)
                            {
                                for (int i = 16; i < dataArray.Length; i++)
                                {
                                    tagData.Add(dataArray[i]);
                                }
                            }

                            Character character = new Character(dataArray[0], double.Parse(dataArray[1]), double.Parse(dataArray[2]), double.Parse(dataArray[3]),
                                double.Parse(dataArray[4]), double.Parse(dataArray[5]), double.Parse(dataArray[6]), double.Parse(dataArray[7]), double.Parse(dataArray[8]),
                                double.Parse(dataArray[9]), double.Parse(dataArray[10]), double.Parse(dataArray[11]), double.Parse(dataArray[12]), double.Parse(dataArray[13]),
                                double.Parse(dataArray[14]), double.Parse(dataArray[15]), tagData);
                            list.Add(character);
                        }
                    }
                    sr.Close();
                }
            }
            return list;
        }

        //For handling the long code to handle user choice in editing which attribute.
        private void EditCharacterProperty(Character character, string choice, string input)
        {
            switch (choice)
            {
                case "name":
                    character.Name = input;
                    break;
                case "expertise":
                    character.Expertise = double.Parse(input);
                    break;
                case "expertise_reserve":
                    character.ExpertiseReserve = double.Parse(input);
                    break;
                case "expertise_prestige":
                    character.ExpertisePrestige = double.Parse(input);
                    break;
                case "faith":
                    character.Faith = double.Parse(input); 
                    break;
                case "faith_reserve":
                    character.FaithReserve = double.Parse(input); 
                    break;
                case "faith_prestige":
                    character.FaithPrestige = double.Parse(input); 
                    break;
                case "influence":
                    character.Influence = double.Parse(input); 
                    break;
                case "influence_reserve":
                    character.InfluenceReserve = double.Parse(input); 
                    break;
                case "influence_prestige":
                    character.InfluencePrestige = double.Parse(input); 
                    break;
                case "perception":
                    character.Perception = double.Parse(input); 
                    break;
                case "perception_reserve":
                    character.PerceptionReserve = double.Parse(input); 
                    break;
                case "perception_prestige":
                    character.PerceptionPrestige = double.Parse(input); 
                    break;
                case "physique":
                    character.Physique = double.Parse(input); 
                    break;
                case "physique_reserve":
                    character.PhysiqueReserve = double.Parse(input); 
                    break;
                case "physique_prestige":
                    character.PhysiquePrestige = double.Parse(input); 
                    break;
                case "add_tag":
                    character.AddTag(input);
                    break;
                case "remove_tag":
                    character.RemoveTag(input);
                    break;
                default:
                    break;
            }
        }
    }
}
