using SlippiNET.Models.Commands;

namespace SlippiNET.Processors
{
    public class SlippiPreFrameUpdateProcessor : BaseCommandProcessor<SlippiPreFrameUpdateCommand>
    {
        public override SlippiPreFrameUpdateCommand Process(byte[] payload)
        {
            return new SlippiPreFrameUpdateCommand(
                ReadInt(payload, 0x1),
                ReadUInt8(payload, 0x5),
                ReadBool(payload, 0x6),
                ReadUInt(payload, 0x7),
                ReadUShort(payload, 0xb),
                ReadFloat(payload, 0xd),
                ReadFloat(payload, 0x11),
                ReadFloat(payload, 0x15),
                ReadFloat(payload, 0x19),
                ReadFloat(payload, 0x1d),
                ReadFloat(payload, 0x21),
                ReadFloat(payload, 0x25),
                ReadFloat(payload, 0x29),
                ReadUInt(payload, 0x2d),
                ReadUShort(payload, 0x31),
                ReadFloat(payload, 0x33),
                ReadFloat(payload, 0x37),
                ReadFloat(payload, 0x3c)
            );
        }
    }
}
