using Chess_game.Behaviour.Commands.Action_after_move_Command;
using Chess_game.Controls;
using Chess_game.Models;

namespace Chess_game.Behaviour.Interfaces
{
    public interface IMoveUpdate
    {
        public ICell Cell { get; }
        public IPlayableChessPiece? Old_Piece { get; }
        public IPlayableChessPiece? New_Piece { get; }
        public MoveType MoveType { get; }

        public IAdditionAction_Command? AdditionAction { get; }
        //public MoveUpdate? NextUpdate { get; }
    }
}
