using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlippiNET.Models.Commands;

namespace SlippiNET.Processors
{
    public class SlippiGameEndProcessor : BaseCommandProcessor<SlippiGameEndCommand>
    {
        public override SlippiGameEndCommand Process(byte[] payload)
        {
            return new SlippiGameEndCommand(ReadUInt8(payload, 0x1), ReadInt8(payload, 0x2));
        }
    }
}
