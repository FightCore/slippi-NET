using System.IO;
using SlippiNET.Models;
using SlippiNET.Models.MetaData;
using UBJSON.Net;

namespace SlippiNET.Utils
{
    public class SlippiMetaDataReader
    {
        public SlippiMetaData Read(Stream stream, SlippiFileType fileType)
        {
            var metaDataBuffer = new byte[fileType.MetadataLength];
            stream.Position = fileType.MetadataPosition;
            stream.Read(metaDataBuffer);
            var reader = new UBJSONReader(metaDataBuffer);
            return reader.Read<SlippiMetaData>();
        }
    }
}
