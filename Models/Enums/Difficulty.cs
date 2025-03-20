using System.ComponentModel;

namespace Chess_game.Models
{
    public enum Difficulty
    {
        [Description("Easyiest difficulty")]
        Easy,
        [Description("Medium difficulty")]
        Medium,
        [Description("The hardest difficulty")]
        Hard,
        Extream
    }
}
