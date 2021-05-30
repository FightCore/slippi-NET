using System;
using System.Collections.Generic;
using System.IO;
using SlippiNET.Models;

namespace SlippiNET.Utils
{
    public class SlippiTypeFileReader
    {
        public SlippiFileType GetFileType(Stream stream)
        {
            var rawDataPosition = GetRawDataPosition(stream);
            var rawDataLength = GetRawDataLength(stream, rawDataPosition);
            var metadataPosition = rawDataPosition + rawDataLength + 10 <= stream.Length ?  rawDataPosition + rawDataLength + 10 : -1; // remove metadata string
            var metadataLength = metadataPosition == -1 ? -1 : stream.Length - metadataPosition - 1;
            var messageSizes = GetMessageSizes(stream, rawDataPosition);

            return new SlippiFileType(rawDataPosition, rawDataLength, metadataPosition, metadataLength, messageSizes);
        }

        private static long GetRawDataPosition(Stream stream)
        {
            var buffer = new byte[1];
            stream.Read(buffer, 0, buffer.Length);

            // Equals to 54 decimal.
            if (buffer[0] == 0x36)
            {
                return 0;
            }
            if (buffer[0] != '{')
            {
                throw new Exception();
            }

            return 15;
        }

        private static long GetRawDataLength(Stream stream, long position)
        {
            if (position == 0)
            {
                return stream.Length;
            }

            var buffer = new byte[4];
            stream.Position = position - 4;
            stream.Read(buffer);
            var rawDataLen = (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];

            if (rawDataLen > 0)
            {
                // If this method manages to read a number, it's probably trustworthy
                return rawDataLen;
            }

            // If the above does not return a valid data length,
            // return a file size based on file length. This enables
            // some support for severed files
            return stream.Length - position;
        }

        private static Dictionary<int, int> GetMessageSizes(Stream stream, long position)
        {
            var messageSizes = new Dictionary<int, int>();
            if (position == 0)
            {
                messageSizes[0x36] = 0x140;
                messageSizes[0x37] = 0x6;
                messageSizes[0x38] = 0x46;
                messageSizes[0x39] = 0x1;
                return messageSizes;
            }

            var buffer = new byte[2];
            stream.Position = position;
            stream.Read(buffer);

            if (buffer[0] != (int)SlippiCommand.MESSAGE_SIZES)
            {
                throw new Exception("Buffer is not the message size");
            }

            var payloadLength = Convert.ToInt32(buffer[1]);
            messageSizes[0x35] = payloadLength;

            var messageSizesBuffer = new byte[payloadLength];
            stream.Position = position + 2;
            stream.Read(messageSizesBuffer);

            for (var i = 0; i < payloadLength - 1; i += 3)
            {
                var command = messageSizesBuffer[i];

                // Get size of command
                messageSizes[command] = (messageSizesBuffer[i + 1] << 8) | messageSizesBuffer[i + 2];
            }

            return messageSizes;
        }
    }
}
