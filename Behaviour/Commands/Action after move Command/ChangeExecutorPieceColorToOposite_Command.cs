using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands.Action_after_move_Command
{
    class ChangeExecutorPieceColorToOposite_Command : IAdditionAction_Command
    {
        public short ID => 4;

        public void DoAction(IMove sender, GameBoard gameBoard)
        {
            if(sender.Executor_Piece is ChessPiece piece)
            {
                piece.PieceColor = piece.PieceColor.GetOpositeColor();
            }
        }

        public void UndoAction(IMove sender, GameBoard gameBoard)
        {
            if (sender.Executor_Piece is ChessPiece piece)
            {
                piece.PieceColor = piece.PieceColor.GetOpositeColor();
            }
        }
    }
}
