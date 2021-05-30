using System.Collections.Generic;
using System.Text.Json.Serialization;
using UBJSON.Net;

namespace SlippiNET.Models.MetaData
{
    public class SlippiMetaData
    {
        [JsonPropertyName("startAt")]
        public string StartAt { get; set; }

        [JsonPropertyName("lastFrame")]

        public int LastFrame { get; set; }

        [UbjsonProperty(Name = "playedOn")]

        public string PlayedOn { get; set; }

        [UbjsonProperty(Name = "consoleNick")]

        public string ConsoleNickname { get; set; }

        [UbjsonProperty(Name = "players")]
        public Dictionary<int, SlippiPlayerMetaData> Players { get; set; }
    }
}
