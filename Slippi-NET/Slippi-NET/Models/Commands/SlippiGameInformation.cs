using SlippiNET.Models.Melee;

namespace SlippiNET.Models.Commands
{
    public record SlippiGameInformation
        (string SlpVersion, bool IsTeams, bool IsPAL, MeleeStage Stage, SlippiPlayer[] Players, byte Scene, MeleeMajorScene GameMode)
            : BaseSlippiCommand(SlippiCommand.GAME_START)
    {
    }
}
