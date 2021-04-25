using SlippiNET.Models.Melee;

namespace SlippiNET.Models
{
    public record SlippiPlayer(int Index, MeleeCharacter Character, sbyte CharacterColor, sbyte StartStocks,
        MeleePlayerType Type, sbyte TeamId, string ControllerFix, string NameTag, string DisplayName, string ConnectCode)
    {
        public int Port => Index + 1;
    }
}
