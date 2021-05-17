using SlippiNET.Models.Melee;

namespace SlippiNET.Models.Commands
{
    public record SlippiGameEndCommand(MeleeGameEndMethods GameEndMethod, sbyte LRASInitiator) : BaseSlippiCommand(SlippiCommand.GAME_END);
}
