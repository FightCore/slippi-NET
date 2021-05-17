namespace SlippiNET.Models.Commands
{
    public record SlippiGameEndCommand(byte Frame, sbyte LatestFinalizedFrame) : BaseSlippiCommand(SlippiCommand.GAME_END);
}
