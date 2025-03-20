using Chess_game.Behaviour.Decorators.Get_Moves_Decorator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Models.Comparers
{
    class GetMoves_Decorator_Comparer : IComparer<IGetMovesDecorator>
    {
        public int Compare(IGetMovesDecorator? x, IGetMovesDecorator? y)
        {
            return (x?.Priority ?? 0) - (y?.Priority ?? 0);
        }
    }
}
