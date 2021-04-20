namespace SlippiNET.Models
{
    public enum SlippiCommand
    {
        MESSAGE_SIZES = 0x35,
        GAME_START = 0x36,
        PRE_FRAME_UPDATE = 0x37,
        POST_FRAME_UPDATE = 0x38,
        GAME_END = 0x39,
        ITEM_UPDATE = 0x3b,
        FRAME_BOOKEND = 0x3c,
    }
}
