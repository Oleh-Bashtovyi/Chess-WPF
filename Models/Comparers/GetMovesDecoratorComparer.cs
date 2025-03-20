using Chess_game.Behaviour.Decorators.Get_Moves_Decorator;
using System.Collections.Generic;

namespace Chess_game.Models.Comparers;

class GetMovesDecoratorComparer : IComparer<IGetMovesDecorator>
{
    public int Compare(IGetMovesDecorator? x, IGetMovesDecorator? y)
    {
        return (x?.Priority ?? 0) - (y?.Priority ?? 0);
    }
}