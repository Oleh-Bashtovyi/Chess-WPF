using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Extensions
{
    public static class Byte_Extension
    {

        public static bool Byte_ToBoolean(this byte value)
        {
            return value != 0;
        }
    }
}
