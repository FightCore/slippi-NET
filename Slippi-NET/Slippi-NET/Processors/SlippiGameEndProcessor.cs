using SlippiNET.Models.Commands;
using SlippiNET.Models.Melee;

namespace SlippiNET.Processors
{
    public class SlippiGameEndProcessor : BaseCommandProcessor<SlippiGameEndCommand>
    {
        public override SlippiGameEndCommand Process(byte[] payload)
        {
            if (payload.Length == 3)
            {
                return new SlippiGameEndCommand((MeleeGameEndMethods)ReadUInt8(payload, 0x1), -1);
            }

            return new SlippiGameEndCommand((MeleeGameEndMethods)ReadUInt8(payload, 0x1), ReadInt8(payload, 0x2));
        }
    }
}
