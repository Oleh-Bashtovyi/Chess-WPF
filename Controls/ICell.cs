using Chess_game.Behaviour;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Controls
{
    public interface ICell
    {
        byte X { get;}
        byte Y { get;}
        bool IsPromotionCell { get;}
        bool IsEmpty { get;}
        IPlayableChessPiece? Piece { get; }
        string Abreviation { get; }

       
        string Get_Encoding();
    }
}
