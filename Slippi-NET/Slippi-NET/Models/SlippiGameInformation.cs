using SlippiReader.Models.Melee;

namespace SlippiReader.Models
{
    public record SlippiGameInformation(string SlpVersion, bool IsTeams, bool IsPAL, ushort StageId, SlippiPlayer[] Players, byte Scene, MeleeMajorScene GameMode)
    {
    }
}
