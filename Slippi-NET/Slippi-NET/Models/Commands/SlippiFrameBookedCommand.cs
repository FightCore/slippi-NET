namespace SlippiNET.Models.Commands
{
    public record SlippiFrameBookedCommand(int Frame, int LatestFinalizedFrame) : BaseSlippiCommand(SlippiCommand
        .FRAME_BOOKEND);
}
