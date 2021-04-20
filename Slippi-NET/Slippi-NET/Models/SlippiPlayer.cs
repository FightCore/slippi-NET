using SlippiReader.Models.Melee;

namespace SlippiReader.Models
{
    public record SlippiPlayer(int Index, MeleeCharacter Character, sbyte CharacterColor, sbyte StartStocks,
        sbyte Type, sbyte TeamId, string ControllerFix, string NameTag, string DisplayName, string ConnectCode)
    {
        public int Port => Index + 1;
    }
}
