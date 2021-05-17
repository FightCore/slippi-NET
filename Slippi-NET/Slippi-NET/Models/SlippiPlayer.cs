using SlippiNET.Models.Melee;

namespace SlippiNET.Models
{
    public record SlippiPlayer(int Index, MeleeCharacter Character, byte CharacterColor, byte StartStocks,
        MeleePlayerType Type, byte TeamId, string ControllerFix, string NameTag, string DisplayName, string ConnectCode)
    {
        public int Port => Index + 1;
    }
}
