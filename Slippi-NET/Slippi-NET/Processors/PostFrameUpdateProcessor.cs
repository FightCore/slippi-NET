using SlippiNET.Models.Commands;

namespace SlippiNET.Processors
{
    public class PostFrameUpdateProcessor : BaseCommandProcessor<SlippiPostFrameUpdateCommand>
    {
        public override SlippiPostFrameUpdateCommand Process(byte[] payload)
        {
            return new SlippiPostFrameUpdateCommand(
                ReadInt(payload, 0x1),
                ReadUInt8(payload, 0x5),
                ReadBool(payload, 0x6),
                ReadUInt8(payload, 0x7),
                ReadUShort(payload, 0x8),
                ReadFloat(payload, 0xa),
                ReadFloat(payload, 0xe),
                ReadFloat(payload, 0x12),
                ReadFloat(payload, 0x16),
                ReadFloat(payload, 0x1a),
                ReadUInt8(payload, 0x1e),
                ReadUInt8(payload, 0x1f),
                ReadUInt8(payload, 0x20),
                ReadUInt8(payload, 0x21),
                ReadFloat(payload, 0x2b),
                ReadBool(payload, 0x2f),
                ReadUShort(payload, 0x30),
                ReadUInt8(payload, 0x32),
                ReadUInt8(payload, 0x33),
                ReadUInt8(payload, 0x34)
            );
        }
    }
}
