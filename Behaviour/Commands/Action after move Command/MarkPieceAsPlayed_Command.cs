using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands.Action_after_move_Command
{
    class MarkPieceAsPlayed_Command : IAdditionAction_Command
    {
        public short ID => 1;


        public IPlayableChessPiece TargetPiece;
        short oldValue;
        public MarkPieceAsPlayed_Command(IPlayableChessPiece targetPiece) => this.TargetPiece = targetPiece;


        public void DoAction(IMove sender, GameBoard gameBoard)
        {
            oldValue = TargetPiece.WasFirstlyPlayed;
            TargetPiece.MarkAsPlayed(gameBoard.MoveNumber);
        }

        public void UndoAction(IMove sender, GameBoard gameBoard)
        {
            TargetPiece.UnMarkAsPlayed(oldValue);
        }
    }
}
