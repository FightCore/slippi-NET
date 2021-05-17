using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SlippiNET.Models;
using SlippiNET.Models.Commands;
using SlippiNET.Models.Melee;

namespace SlippiNET.Utils
{
    public class SlippiFileReader
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("shift_jis");
        // Contains the indexes of the active players.
        private readonly HashSet<int> _playerIndex = new HashSet<int>();
        private bool _gameHasEnded = false;

        public IEnumerable<BaseSlippiCommand> Read(Stream stream, SlippiFileType fileType)
        {
            var readPosition = fileType.RawDataPosition;
            var stopReadingAt = fileType.RawDataPosition + fileType.RawDataLength;
            var commandByteBuffer = new byte[1];

            while (!_gameHasEnded || readPosition < stopReadingAt)
            {
                // If we are streaming the game live but there is nothing to read, don't output, just return.
                if (readPosition >= stream.Length)
                {
                    continue;
                }

                // Set the stream position to the new read position.
                stream.Position = readPosition;
                // Read the bytes into the commandByteBuffer.
                stream.Read(commandByteBuffer);

                // Get the command byte out of the buffer.
                var commandByte = commandByteBuffer[0];

                // If the command byte is an unknown message, return and stop reading.
                // Most likely something is wrong
                // TODO Replace with custom exception.
                if (!fileType.messageSizes.ContainsKey(commandByte))
                {
                    throw new Exception();
                }

                // Create a message buffer the size given by the command byte type.
                // +1 is done because we select the command byte as well.
                var messageBuffer = new byte[fileType.messageSizes[commandByte] + 1];

                // Set the stream position back to the read position
                // All messages include the commandByte as the first byte.
                stream.Position = readPosition;
                stream.Read(messageBuffer);
                yield return ParseMessage(Enum.Parse<SlippiCommand>(commandByte.ToString()), messageBuffer);
                readPosition += messageBuffer.Length;
            }
        }

        private BaseSlippiCommand ParseMessage(SlippiCommand command, byte[] payload)
        {
            var players = new SlippiPlayer[4];
            switch (command)
            {
                case SlippiCommand.GAME_START:
                    for (var i = 0; i < 4; i++)
                    {
                        const int nameTagLength = 0x10;
                        var nameTagOffset = i * nameTagLength;
                        var nameTagStart = 0x161 + nameTagOffset;
                        var nameTag = ReadString(payload, nameTagStart, nameTagLength);

                        // Display name
                        const int displayNameLength = 0x1f;
                        var displayNameOffset = i * displayNameLength;
                        var displayNameStart = 0x1a5 + displayNameOffset;
                        var displayName = ReadString(payload, displayNameStart, displayNameLength);

                        // Connect code
                        const int connectCodeLength = 0xa;
                        var connectCodeOffset = i * connectCodeLength;
                        var connectCodeStart = 0x221 + connectCodeOffset;
                        var connectCode = ReadString(payload, connectCodeStart, connectCodeLength);

                        var offset = i * 0x24;
                        players[i] = new SlippiPlayer(
                            i,
                            (MeleeCharacter)ReadUInt8(payload, 0x65 + offset),
                            ReadUInt8(payload, 0x68 + offset),
                            ReadUInt8(payload, 0x67 + offset),
                            (MeleePlayerType)ReadUInt8(payload, 0x66 + offset),
                            ReadUInt8(payload, 0x6e + offset),
                            "Not Implemented",
                            nameTag,
                            displayName,
                            connectCode
                        );

                        // If the player slot is empty, there is no need to gather PRE-POST FRAME
                        // or any other commands for it. Only non-empty player types are added
                        // to the list.
                        if (players[i].Type != MeleePlayerType.Empty)
                        {
                            _playerIndex.Add(i);
                        }
                    }
                    return new SlippiGameInformation(
                        $"{ReadUInt8(payload, 0x1)}.{ReadUInt8(payload, 0x2)}.{ReadUInt8(payload, 0x3)}",
                        ReadBool(payload, 0xd),
                        ReadBool(payload, 0x1a1),
                        (MeleeStage)ReadUShort(payload, 0x13),
                        players,
                        payload[0x1a3],
                        (MeleeMajorScene)payload[0x1a4]
                    );
                case SlippiCommand.PRE_FRAME_UPDATE:
                    return new SlippiPreFrameUpdate(
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
                case SlippiCommand.POST_FRAME_UPDATE:
                    var postFramePlayerIndex = payload[0x5];
                    if (!_playerIndex.Contains(postFramePlayerIndex))
                    {
                        break;
                    }

                    return new PostFrameUpdateCommand(
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
                case SlippiCommand.GAME_END:
                    _gameHasEnded = true;
                    break;
                    return new SlippiGameEndCommand(ReadUInt8(payload, 0x1), ReadInt8(payload, 0x2));
                case SlippiCommand.ITEM_UPDATE:
                    break;
                case SlippiCommand.FRAME_BOOKEND:
                    return new FrameBookedCommand(
                        ReadInt(payload, 0x1),
                        ReadInt(payload, 0x5));
                case SlippiCommand.MESSAGE_SIZES:
                    break;
            }

            return null;
        }

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
        private static string ConvertToHalfWidth(string value)
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

        private static string ReadString(byte[] payload, int startIndex, int length)
        {
            var end = startIndex + length;
            var buffer = payload[startIndex..end];
            var resultString = _encoding.GetString(buffer);
            if (string.IsNullOrWhiteSpace(resultString))
            {
                return null;
            }

            return ConvertToHalfWidth(resultString);
        }

        private static byte ReadUInt8(byte[] payload, int startIndex)
        {
            return (byte)BitConverter.ToChar(payload, startIndex);
        }

        private static sbyte ReadInt8(byte[] payload, int startIndex)
        {
            return (sbyte)BitConverter.ToChar(payload, startIndex);
        }

        private static bool ReadBool(byte[] payload, int startIndex)
        {
            return BitConverter.ToBoolean(payload, startIndex);
        }

        private static float ReadFloat(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadSingleBigEndian(payload[startIndex..endIndex]);
        }

        private static ushort ReadUShort(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 2;
            return BinaryPrimitives.ReadUInt16BigEndian(payload[startIndex..endIndex]);
        }

        private static int ReadInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadInt32BigEndian(payload[startIndex..endIndex]);
        }

        private static uint ReadUInt(byte[] payload, int startIndex)
        {
            var endIndex = startIndex + 4;
            return BinaryPrimitives.ReadUInt32BigEndian(payload[startIndex..endIndex]);
        }
    }
}
