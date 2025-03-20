using Chess_game.Behaviour.Commands.Action_after_move_Command;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Decorators.Get_Moves_Decorator
{
    class ChangeColorOfExecutorPiecesAfterMoveMadeDecorator : IGetMovesDecorator
    {
        public int ID => 3;

        public int Priority => 100;

        public List<IMove> Execute(List<IMove> moves, GameBoard board, Color ExecutorColor)
        {
            foreach (var move in moves)
            {
                MoveUpdate update = new(move.From_Cell, move.From_Cell.Piece, move.From_Cell.Piece, MoveType.Move, new ChangeExecutorPieceColorToOposite_Command());
                move.Add_Update(update);
            }
            return moves;
        }


        public IGetMovesDecorator Clone()
        {
            return new ChangeColorOfExecutorPiecesAfterMoveMadeDecorator();
        }
    }
}
