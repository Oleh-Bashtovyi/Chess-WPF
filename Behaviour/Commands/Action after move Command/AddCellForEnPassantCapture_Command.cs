using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands.Action_after_move_Command
{
    class AddCellForEnPassantCapture_Command : IAdditionAction_Command
    {
        public short ID => 2;
        public IPlayableChessPiece TargetPiece { get; set; }
        public (byte, byte) CellThatWillBeatTargetPiece { get; set; }

        public AddCellForEnPassantCapture_Command(IPlayableChessPiece targetPiece, (byte, byte) cellThatWillBeatTargetPiece)
        {
            TargetPiece = targetPiece;
            CellThatWillBeatTargetPiece = cellThatWillBeatTargetPiece;
        }


        public AddCellForEnPassantCapture_Command(IPlayableChessPiece targetPiece, ICell cellThatWillBeatTargetPiece)
        {
            TargetPiece = targetPiece;
            CellThatWillBeatTargetPiece = (cellThatWillBeatTargetPiece.X, cellThatWillBeatTargetPiece.Y);
        }


        public void DoAction(IMove sender, GameBoard gameBoard)
        {
            TargetPiece.EnPassantCellToBeatPiece = CellThatWillBeatTargetPiece;
        }

        public void UndoAction(IMove sender, GameBoard gameBoard)
        {
            TargetPiece.EnPassantCellToBeatPiece = null;
        }
    }
}
