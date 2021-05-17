using SlippiNET.Models.Commands;

namespace SlippiNET.Processors.Collections
{
    public interface IProcessors
    {
        IBaseCommandProcessor<SlippiGameInformation> SlippiStartProcessor { get; }

        IBaseCommandProcessor<SlippiPreFrameUpdateCommand> SlippiPreFrameUpdateProcessor { get; }

        IBaseCommandProcessor<SlippiPostFrameUpdateCommand> SlippiPostFrameUpdateProcessor { get; }

        IBaseCommandProcessor<SlippiGameEndCommand> SlippiGameEndProcessor { get; }

        IBaseCommandProcessor<SlippiFrameBookedCommand> SlippiFrameBookedProcessor { get; }
    }
}
