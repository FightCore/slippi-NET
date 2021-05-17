using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlippiNET.Models;
using SlippiNET.Models.Commands;
using SlippiNET.Models.Melee;

namespace SlippiNET.Processors
{
    public class SlippiGameStartProcessor : BaseCommandProcessor<SlippiGameInformation>
    {
        public override SlippiGameInformation Process(byte[] payload)
        {
            var players = new SlippiPlayer[4];
            for (var i = 0; i < 4; i++)
            {
                const int nameTagLength = 0x10;
                var nameTagOffset = i * nameTagLength;
                var nameTagStart = 0x161 + nameTagOffset;
                var nameTag = ReadString(payload, nameTagStart, nameTagLength);

                // Display name
                const int displayNameLength = 0x1f;
                var displayNameOffset = i * displayNameLength;
                var displayNameStart = 0x1a5 + displayNameOffset;
                var displayName = ReadString(payload, displayNameStart, displayNameLength);

                // Connect code
                const int connectCodeLength = 0xa;
                var connectCodeOffset = i * connectCodeLength;
                var connectCodeStart = 0x221 + connectCodeOffset;
                var connectCode = ReadString(payload, connectCodeStart, connectCodeLength);

                var offset = i * 0x24;
                players[i] = new SlippiPlayer(
                    i,
                    (MeleeCharacter)ReadUInt8(payload, 0x65 + offset),
                    ReadUInt8(payload, 0x68 + offset),
                    ReadUInt8(payload, 0x67 + offset),
                    (MeleePlayerType)ReadUInt8(payload, 0x66 + offset),
                    ReadUInt8(payload, 0x6e + offset),
                    "Not Implemented",
                    nameTag,
                    displayName,
                    connectCode
                );

                // If the player slot is empty, there is no need to gather PRE-POST FRAME
                // or any other commands for it. Only non-empty player types are added
                // to the list.
                if (players[i].Type != MeleePlayerType.Empty)
                {
                    //_playerIndex.Add(i);
                }
            }
            return new SlippiGameInformation(
                $"{ReadUInt8(payload, 0x1)}.{ReadUInt8(payload, 0x2)}.{ReadUInt8(payload, 0x3)}",
                ReadBool(payload, 0xd),
                ReadBool(payload, 0x1a1),
                (MeleeStage)ReadUShort(payload, 0x13),
                players,
                payload[0x1a3],
                (MeleeMajorScene)payload[0x1a4]
            );
        }
    }
}
