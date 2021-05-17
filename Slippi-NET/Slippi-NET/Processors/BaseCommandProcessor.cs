using System;
using System.Buffers.Binary;
using System.Linq;
using SlippiNET.Models.Commands;
using SlippiNET.Utils;

namespace SlippiNET.Processors
{
    public interface IBaseCommandProcessor<out TCommand>
        where TCommand : BaseSlippiCommand
    {
        TCommand Process(byte[] payload);
    }

    public abstract class BaseCommandProcessor<TCommand> : IBaseCommandProcessor<TCommand>
        where TCommand : BaseSlippiCommand
    {

        public abstract TCommand Process(byte[] payload);

        /// <summary>
        /// Converts the string into a HalfWidth string as what Melee uses.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// Method implementation taken from Slippi-Js
        /// https://github.com/project-slippi/slippi-js/blob/11990cf4e92b79bc0e369ca94fbbe0ee747c1423/src/utils/fullwidth.ts#L3
        /// TODO: Comment what this does/might do.
        /// </remarks>
        protected static string ConvertToHalfWidth(string value)
        {
            return new string(value.Select(charCode =>
            {
                if (charCode > 0xff00 && charCode < 0xff5f)
                {
                    return (char)(0x0020 + (charCode - 0xff00));
                }

                // space:
                if (charCode == 0x3000)
                {
                    return (char)0x0020;
                }

                // Exceptions found in Melee/Japanese keyboards
                // single quote: '
                if (charCode == 0x2019)
                {
                    return (char)0x0027;
                }

                // double quote: "
                if (charCode == 0x201d)
                {
                    return (char)0x0022;
                }

                return charCode;
            }).ToArray()).Trim('\0');
        }

        protected static string ReadString(byte[] payload, int startIndex, int length)
        {
            var end = startIndex + length;
            var buffer = payload[startIndex..end];
            var resultString = SlippiConstants.Encoding.GetString(buffer);
            if (string.IsNullOrWhiteSpace(resultString))
            {
                return null;
            }

            return ConvertToHalfWidth(resultString);
        }

        protected static byte ReadUInt8(byte[] payload, int startIndex)
        {
            return (byte)BitConverter.ToChar(payload, startIndex);
        }

        protected static sbyte ReadInt8(byte[] payload, int startIndex)
        {
            return (sbyte)BitConverter.ToChar(payload, startIndex);
        }

        protected static bool ReadBool(byte[] payload, int startIndex)
        {
            return BitConverter.ToBoolean(payload, startIndex);
        }

        protected static float ReadFloat(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadSingleBigEndian(payload[startIndex..endIndex]);
        }

        protected static ushort ReadUShort(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 2;
            return BinaryPrimitives.ReadUInt16BigEndian(payload[startIndex..endIndex]);
        }

        protected static int ReadInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadInt32BigEndian(payload[startIndex..endIndex]);
        }

        protected static uint ReadUInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadUInt32BigEndian(payload[startIndex..endIndex]);
        }
    }
}
