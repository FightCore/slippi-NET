using System.Collections.Generic;
using System.Text.Json.Serialization;
using SlippiNET.Models.Melee;
using UBJSON.Net;

namespace SlippiNET.Models.MetaData
{
    public class SlippiPlayerMetaData
    {
        public Dictionary<MeleeInternalCharacter, int> Characters { get; set; }

        public Dictionary<string, string> Names { get; set; }
    }
}
