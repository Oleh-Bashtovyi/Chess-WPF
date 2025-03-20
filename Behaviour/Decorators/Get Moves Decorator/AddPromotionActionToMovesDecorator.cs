using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Factories;
using Chess_game.Models;
using System.Collections.Generic;
using System.Linq;

namespace Chess_game.Behaviour.Decorators.Get_Moves_Decorator
{
    public class AddPromotionActionToMovesDecorator : IGetMovesDecorator
    {
        public int ID => 2;

        public int Priority => 10;

        public IGetMovesDecorator Clone()
        {
            return new AddPromotionActionToMovesDecorator();
        }

        public List<IMove> Execute(List<IMove> moves, GameBoard board, Color ExecutorColor)
        {

            List<IMove> result = new();

            //отримуємо список усіх ходів
            foreach (IMove move in moves)
            {
                //якщо фігура повинна перетворитись, то отримуємо копії ходу з усіма перетвореннями
                if (board.Check_OnPromotion(move, out IMoveUpdate? updateWhenPromotionBegins))
                {
                    //ітеруємось по кожному id з списку перетворень, щоб перетворитись на фігуру з тим самим id
                    foreach (int id in PromotionLists_Factory.Get_PromotionList_ByID(updateWhenPromotionBegins!.New_Piece!.PromotionListID))
                    {
                        IMove copy = move.Clone();

                        copy.Add_Update(new MoveUpdate(updateWhenPromotionBegins.Cell, updateWhenPromotionBegins.New_Piece, Pieces_Factory.Create_ChessPiece_UsingID_ForColor(id, updateWhenPromotionBegins.New_Piece.PieceColor), MoveType.Promotion));

                        if (move.MainMoveType == MoveType.Capture) copy.MainMoveType = MoveType.CapturePromotion;
                        else copy.MainMoveType = MoveType.Promotion;

                        result.Add(copy);
                    }

                }
                else result.Add(move);
            }

            return result.OrderByDescending(x => x.MainMoveType).ToList();
        }
    }
}
