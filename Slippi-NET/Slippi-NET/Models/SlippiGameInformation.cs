using SlippiNET.Models.Melee;

namespace SlippiNET.Models
{
    public record SlippiGameInformation(string SlpVersion, bool IsTeams, bool IsPAL, MeleeStage Stage, SlippiPlayer[] Players, byte Scene, MeleeMajorScene GameMode)
    {
    }
}
