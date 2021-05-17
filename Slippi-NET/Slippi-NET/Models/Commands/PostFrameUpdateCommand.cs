namespace SlippiNET.Models.Commands
{
    public record PostFrameUpdateCommand(int Frame, byte PlayerIndex, bool IsFolower, byte InternalCharacterId,
        ushort ActionStateId,
        float PositionX, float PositionY, float FacingDirection, float Percent, float ShieldSize, byte LastAttackLanded,
        byte CurrentComboCount, byte LastHitBy, byte StocksRemaining, float MiscActionState, bool IsAirborne,
        ushort LastGroundId,
        byte JumpsRemaining, byte LCancelStatus, byte HurtboxCollisionState) : BaseSlippiCommand(SlippiCommand.POST_FRAME_UPDATE);
}
