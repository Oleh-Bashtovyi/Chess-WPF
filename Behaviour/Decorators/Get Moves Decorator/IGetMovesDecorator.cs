using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Decorators.Get_Moves_Decorator
{
    public interface IGetMovesDecorator
    {
        int ID { get; }
        int Priority { get; }


        List<IMove> Execute(List<IMove> moves, GameBoard board, Color ExecutorColor);

        IGetMovesDecorator Clone();
    }
}
