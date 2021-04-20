using System;
using System.Collections.Generic;
using System.Text;

namespace SlippiReader.Models
{
    public record SlippiFileType(long RawDataPosition, long RawDataLength, long MetadataPosition, long MetadataLength, Dictionary<int, int> messageSizes);
}
