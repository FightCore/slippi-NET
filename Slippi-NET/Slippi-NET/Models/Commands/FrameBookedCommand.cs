namespace SlippiNET.Models.Commands
{
    public record FrameBookedCommand(int Frame, int LatestFinalizedFrame) : BaseSlippiCommand(SlippiCommand
        .FRAME_BOOKEND);
}
