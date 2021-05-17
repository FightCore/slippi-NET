using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlippiNET.Models.Commands;

namespace SlippiNET.Processors
{
    public class SlippiFrameBookedProcessor : BaseCommandProcessor<SlippiFrameBookedCommand>
    {
        public override SlippiFrameBookedCommand Process(byte[] payload)
        {
            return new SlippiFrameBookedCommand(
                ReadInt(payload, 0x1),
                ReadInt(payload, 0x5));
        }
    }
}
