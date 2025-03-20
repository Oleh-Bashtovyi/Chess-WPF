using Chess_game.Behaviour;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Factories
{
    public static class GameCells_Factory
    {

        public static GameCell Create_Cell_UsingCode(string encoding)
        {
            string[] list = encoding.Split('_', StringSplitOptions.None);

            if(list.Length != 2)
            {
                throw new ArgumentException("Invalid game cell encoding!");
            }

            string[] cellEncoding = list[0].Split(":", StringSplitOptions.None);



            if(cellEncoding.Length != 3 || 
                !byte.TryParse(cellEncoding[0], out var x) ||
                !byte.TryParse(cellEncoding[1], out var y) ||
                !byte.TryParse(cellEncoding[2], out byte isPromotionCell))
            {
                throw new ArgumentException("Invalid game cell encoding!");
            }


            return new GameCell(x, y, isPromotionCell.Byte_ToBoolean(), string.Empty, Pieces_Factory.Create_ChessPiece_UsingEncoding(list[1]));
        }

        
    }
}
