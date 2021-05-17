using SlippiNET.Models.Commands;

namespace SlippiNET.Processors.Collections
{
    public class BasicProcessors : IProcessors
    {
        public IBaseCommandProcessor<SlippiGameInformation> SlippiStartProcessor { get; } = new SlippiGameStartProcessor();

        public IBaseCommandProcessor<SlippiPreFrameUpdateCommand> SlippiPreFrameUpdateProcessor { get; } = new SlippiPreFrameUpdateProcessor();

        public IBaseCommandProcessor<SlippiPostFrameUpdateCommand> SlippiPostFrameUpdateProcessor { get; } = new PostFrameUpdateProcessor();

        public IBaseCommandProcessor<SlippiGameEndCommand> SlippiGameEndProcessor { get; } = new SlippiGameEndProcessor();

        public IBaseCommandProcessor<SlippiFrameBookedCommand> SlippiFrameBookedProcessor { get; } = new SlippiFrameBookedProcessor();
    }
}
