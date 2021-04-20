using System.Collections.Generic;

namespace SlippiNET.Models
{
    public record SlippiFileType(long RawDataPosition, long RawDataLength, long MetadataPosition, long MetadataLength, Dictionary<int, int> messageSizes);
}
