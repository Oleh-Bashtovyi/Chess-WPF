using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Extensions
{
    public static class Color_Extension
    {

        public static Color GetOpositeColor(this Color color)
        {
            if (color == Color.White) return Color.Black;
            if (color == Color.Black) return Color.White;
            else throw new ArgumentException($"Can not get oposite color to color {color}");
        }


        public static int GetInitialScore(this Color color)
        {
            if(color == Color.White) return -int.MaxValue;

            if(color == Color.Black) return int.MaxValue;

            throw new ArgumentException($"Can not get initial score for color {color}");
        }
    }
}
