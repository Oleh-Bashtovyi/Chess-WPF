using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
