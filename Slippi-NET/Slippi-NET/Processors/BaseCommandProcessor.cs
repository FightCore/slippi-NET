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
            var endIndex = startIndex + length;

            if (startIndex >= payload.Length || endIndex > payload.Length)
            {
                return null;
            }
            
            var buffer = payload[startIndex..endIndex];
            var resultString = SlippiConstants.Encoding.GetString(buffer);
            if (string.IsNullOrWhiteSpace(resultString))
            {
                return null;
            }

            return ConvertToHalfWidth(resultString);
        }

        protected static byte ReadUInt8(byte[] payload, int startIndex)
        {
            if (startIndex >= payload.Length)
            {
                return default;
            }

            return payload[startIndex];
        }

        protected static sbyte ReadInt8(byte[] payload, int startIndex)
        {
            if (startIndex >= payload.Length || startIndex + 1 > payload.Length)
            {
                return default;
            }
            return (sbyte)BitConverter.ToChar(payload, startIndex);
        }

        protected static bool ReadBool(byte[] payload, int startIndex)
        {
            if (startIndex >= payload.Length)
            {
                return default;
            }
            return BitConverter.ToBoolean(payload, startIndex);
        }

        protected static float ReadFloat(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            if (endIndex >= payload.Length)
            {
                return default;
            }
            return BinaryPrimitives.ReadSingleBigEndian(payload.AsSpan(startIndex..endIndex));
        }

        protected static ushort ReadUShort(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 2;
            if (startIndex >= payload.Length || endIndex > payload.Length)
            {
                return default;
            }
            return BinaryPrimitives.ReadUInt16BigEndian(payload.AsSpan(startIndex..endIndex));
        }

        protected static int ReadInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            if (startIndex >= payload.Length || endIndex > payload.Length)
            {
                return default;
            }
            return BinaryPrimitives.ReadInt32BigEndian(payload.AsSpan(startIndex..endIndex));
        }

        protected static uint ReadUInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            if (startIndex >= payload.Length || endIndex > payload.Length)
            {
                return default;
            }
            return BinaryPrimitives.ReadUInt32BigEndian(payload.AsSpan(startIndex..endIndex));
        }
    }
}
