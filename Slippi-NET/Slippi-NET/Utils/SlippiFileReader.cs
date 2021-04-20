using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SlippiNET.Models;
using SlippiNET.Models.Melee;

namespace SlippiNET.Utils
{
    public class SlippiFileReader
    {
        private static readonly Encoding _encoding = Encoding.GetEncoding("shift_jis");
        // Contains the indexes of the active players.
        private readonly HashSet<int> _playerIndex = new HashSet<int>();
        private bool _gameHasEnded = false;
        public void Read(Stream stream, SlippiFileType fileType)
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
                // TODO Replace with exception.
                if (!fileType.messageSizes.ContainsKey(commandByte))
                {
                    return;
                }

                // Create a message buffer the size given by the command byte type.
                // TODO Why +1?
                var messageBuffer = new byte[fileType.messageSizes[commandByte] + 1];

                // Set the stream position back to the read position
                // All messages include the commandByte as the first byte.
                stream.Position = readPosition;
                stream.Read(messageBuffer);
                ParseMessage(Enum.Parse<SlippiCommand>(commandByte.ToString()), messageBuffer);
                readPosition += messageBuffer.Length;
            }
        }

        private void ParseMessage(SlippiCommand command, byte[] payload)
        {
            var players = new SlippiPlayer[4];
            switch (command)
            {
                case SlippiCommand.GAME_START:
                    for (var i = 0; i < 4; i++)
                    {
                        var nametagLength = 0x10;
                        var nametagOffset = i * nametagLength;
                        var nametagStart = 0x161 + nametagOffset;
                        var nameTag = ReadString(payload, nametagStart, nametagLength);
                        if (!string.IsNullOrWhiteSpace(nameTag))
                        {
                            Console.WriteLine($"Display name {i}: {ToHalfwidth(nameTag)}");
                        }

                        // Display name
                        var displayNameLength = 0x1f;
                        var displayNameOffset = i * displayNameLength;
                        var displayNameStart = 0x1a5 + displayNameOffset;
                        var displayName = ReadString(payload, displayNameStart, displayNameLength);
                        if (!string.IsNullOrWhiteSpace(displayName))
                        {
                            Console.WriteLine($"Display name {i}: {ToHalfwidth(displayName)}");
                        }

                        // Connect code
                        var connectCodeLength = 0xa;
                        var connectCodeOffset = i * connectCodeLength;
                        var connectCodeStart = 0x221 + connectCodeOffset;
                        var connectCode = ReadString(payload, connectCodeStart, connectCodeLength);
                        if (!string.IsNullOrWhiteSpace(connectCode))
                        {
                            Console.WriteLine($"Connect code {i}: {ToHalfwidth(connectCode)}");
                        }

                        var offset = i * 0x24;
                        players[i] = new SlippiPlayer(
                            i,
                            (MeleeCharacter)ReadUInt8(payload, 0x65 + offset),
                            ReadUInt8(payload, 0x68 + offset),
                            ReadUInt8(payload, 0x67 + offset),
                            ReadUInt8(payload, 0x66 + offset),
                            ReadUInt8(payload, 0x6e + offset),
                            "Not Implemented",
                            nameTag,
                            displayName,
                            connectCode
                        );
                        if (players[i].Type != 3)
                        {
                            _playerIndex.Add(i);
                        }
                    }
                    var gameInformation = new SlippiGameInformation(
                        $"{ReadUInt8(payload, 0x1)}.{ReadUInt8(payload, 0x2)}.{ReadUInt8(payload, 0x3)}",
                        ReadBool(payload, 0xd),
                        ReadBool(payload, 0x1a1),
                        ReadUShort(payload, 0x13),
                        players,
                        payload[0x1a3],
                        (MeleeMajorScene)payload[0x1a4]
                    );

                    break;
                case SlippiCommand.PRE_FRAME_UPDATE:
                    var playerIndexlo = payload[0x5];
                    if (!_playerIndex.Contains(playerIndexlo))
                    {
                        break;
                    }
                    var percentage = ReadFloat(payload, 0x3C);
                    Console.WriteLine(percentage);
                    break;
                case SlippiCommand.POST_FRAME_UPDATE:
                    var playerIndex = payload[0x5];
                    if (!_playerIndex.Contains(playerIndex))
                    {
                        break;
                    }
                    var percent = ReadFloat(payload, 0x16);
                    //Console.WriteLine(percent);
                    var stocksRemaining = ReadUInt8(payload, 0x21);
                    var isAirborne = ReadBool(payload, 0x2f);
                    var lCancelStatus = ReadUInt8(payload, 0x33);
                    break;
                case SlippiCommand.GAME_END:
                    _gameHasEnded = true;
                    break;
                case SlippiCommand.ITEM_UPDATE:
                    break;
                case SlippiCommand.FRAME_BOOKEND:
                    break;
                default:
                    break;
            }
        }

        private static string ToHalfwidth(string value)
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

                /**
                 * Exceptions found in Melee/Japanese keyboards
                 */
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
            }).ToArray());
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

            return ToHalfwidth(resultString);
        }

        private static sbyte ReadUInt8(byte[] payload, int startIndex)
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
    }
}
