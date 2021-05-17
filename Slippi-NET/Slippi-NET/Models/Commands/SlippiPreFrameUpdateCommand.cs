namespace SlippiNET.Models.Commands
{
    public record SlippiPreFrameUpdateCommand(int Frame, byte PlayerIndex, bool IsFollower, uint Seed, ushort ActionStateId,
        float PositionX, float PositionY, float FacingDirection, float JoystickX, float JoystickY, float CStickX, float CStickY,
        float Trigger, uint Buttons, ushort PhysicalButtons, float PhysicalLTrigger, float PhysicalRTrigger, float Percent)
        : BaseSlippiCommand(SlippiCommand.PRE_FRAME_UPDATE)
    {
    }
}
