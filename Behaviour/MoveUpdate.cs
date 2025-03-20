using Chess_game.Behaviour.Commands.Action_after_move_Command;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour
{
    public class MoveUpdate : IMoveUpdate
    {
        public ICell Cell { get; set; }
        public IPlayableChessPiece? Old_Piece { get; set; }
        public IPlayableChessPiece? New_Piece { get; set; }
        public MoveType MoveType { get; set; }
        public IAdditionAction_Command? AdditionAction { get; set; }



        public MoveUpdate(ICell cell, IPlayableChessPiece? old_Piece, IPlayableChessPiece? new_Piece, MoveType moveType, IAdditionAction_Command? additionAction = null)
        {
            Cell = cell;
            Old_Piece = old_Piece;
            New_Piece = new_Piece;
            MoveType = moveType;
            AdditionAction = additionAction;
        }



    }
}
