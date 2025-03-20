using Chess_game.Behaviour;
using Chess_game.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Controls
{
    public class GameCell : ICell, IDisposable
    {

        //=================================================================================================================
        // PROPERTIES
        //=================================================================================================================
        public byte X { get; }
        public byte Y { get; }
        public bool IsPromotionCell { get; set; } = false;
        public IPlayableChessPiece? Piece { get; set; }

        [JsonIgnore]
        public string Abreviation { get; set; }
        [JsonIgnore]
        public bool IsEmpty => (Piece == null);



        //=================================================================================================================
        // CONSTRUCTOR
        //=================================================================================================================
        public GameCell(byte x, byte y, bool isPromotionCell = false, string abreviation = "a0", IPlayableChessPiece? piece = null)
        {
            X = x;
            Y = y;
            Piece = piece;
            IsPromotionCell = isPromotionCell;
            Abreviation= abreviation;
        }







        //=================================================================================================================
        // METHODS
        //=================================================================================================================
        //
        //
        //
        //  
        //=================================================================================================================
        //Game cell will be coded like:    1:5:1_7:1:18:b6
        //it means: 
        //first number = 1 - X coordinate
        //second number = 5 - Y coordinate
        //third bool = 1 - is promotion cell (0 = NO,  1 = YES)

        //'_' - SPLITER. It split cell code and piece code

        //Next is piece encoding:
        //Piece will be saved in code, like:     7:1:18:b6
        //first number = 7 - ID
        //second number = 1 - Color (0 = black, 1 = white)
        //third number = 18 - when piece was first played (-3000 = havent played yet)
        //fourth abreviation - enpassant cell (X or else - no such cell)
        public string Get_Encoding()
        {
            return $"{X}:{Y}:{(IsPromotionCell ? 1 : 0)}_" + (Piece?.Get_PieceEncoding() ?? "null");
        }
        //=================================================================================================================



        //7:8:1_3:1|fpl=16|enp=b2




        public GameCell Clone()
        {
            return new GameCell(X, Y, IsPromotionCell, Abreviation, Piece?.Clone());
        }


        public void Dispose()
        {
            Piece?.Dispose();
        }







        //=================================================================================================================
        // OVERRIDE
        //=================================================================================================================
        public override bool Equals(object? obj)
        {
            if(obj is ICell other)
            {
                return(X == other.X && Y== other.Y && Abreviation == other.Abreviation && IsPromotionCell == other.IsPromotionCell);
            }
            return false;
        }
    }
}
