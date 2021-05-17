using System;
using System.Collections.Generic;
using System.IO;
using SlippiNET.Models;
using SlippiNET.Models.Commands;
using SlippiNET.Processors.Collections;

namespace SlippiNET.Utils
{
    public class SlippiFileReader
    {
        // Contains the indexes of the active players.
        private readonly HashSet<int> _playerIndex = new HashSet<int>();
        private readonly IProcessors _processors;
        private bool _gameHasEnded = false;

        public SlippiFileReader(IProcessors processors = null)
        {
            _processors = processors ?? new BasicProcessors();
        }

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
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (command)
            {
                case SlippiCommand.GAME_START:
                    return _processors.SlippiStartProcessor.Process(payload);
                case SlippiCommand.PRE_FRAME_UPDATE:
                    return _processors.SlippiPreFrameUpdateProcessor.Process(payload);
                case SlippiCommand.POST_FRAME_UPDATE:
                    return _processors.SlippiPostFrameUpdateProcessor.Process(payload);
                case SlippiCommand.GAME_END:
                    _gameHasEnded = true;
                    break;
                    return _processors.SlippiGameEndProcessor.Process(payload);
                case SlippiCommand.ITEM_UPDATE:
                    break;
                case SlippiCommand.FRAME_BOOKEND:
                    return _processors.SlippiFrameBookedProcessor.Process(payload);
                case SlippiCommand.MESSAGE_SIZES:
                    break;
            }

            return null;
        }
    }
}
