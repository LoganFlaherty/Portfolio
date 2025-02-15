using Newtonsoft.Json;
using System.Collections.Generic;

namespace Storytime_Bot.Commands
{
    public class CharactersJSON
    {
        //Characters : {} --> userID : {} --> character name : {} --> character data
        [JsonProperty("Characters")]
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> Characters { get; set; }
    }
}
