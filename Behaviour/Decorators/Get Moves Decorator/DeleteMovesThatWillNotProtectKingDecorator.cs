using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Decorators.Get_Moves_Decorator
{
    public class DeleteMovesThatWillNotProtectKingDecorator : IGetMovesDecorator
    {
        public int ID => 1;

        public int Priority => 1;

        public List<IMove> Execute(List<IMove> moves, GameBoard board, Color ExecutorColor)
        {
            //Проходимось по усім ходам з кінця (щоб можна було легко видалити хід зі списку)
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                //робимо хід
                board.Make_Move(moves[i]);

                //отримуємо список клітин, які перебувають під атакою після виконаного ходу
                bool[,] dangerouseCells = board.Get_MatrixOfCellsThatUnderAtackByPieces_OfColor(ExecutorColor.GetOpositeColor());

                //дивимось, чи хоча б 1 король залишився під атакою після виконаного ходу, якщо так, то переходимо до іншого можливого ходу
                bool isAtLeastOneKingUnderAttack = false;


                foreach (ICell kingCell in board.Get_AllCellsWherePiecesIsKingFigure_OfColor(ExecutorColor))
                {
                    if (dangerouseCells[kingCell.X, kingCell.Y])
                    {
                        isAtLeastOneKingUnderAttack = true;
                        break;
                    }
                }

                board.Undo_Move(moves[i]);

                //Якщо після виконання ходу хоча б 1 з королів залишиться під шахом, то такий хід не можна робити, видаляємо його
                if (isAtLeastOneKingUnderAttack)
                {
                    moves.RemoveAt(i);
                }
            }

            return moves;
        }



        public IGetMovesDecorator Clone()
        {
            return new DeleteMovesThatWillNotProtectKingDecorator();
        }
    }
}
