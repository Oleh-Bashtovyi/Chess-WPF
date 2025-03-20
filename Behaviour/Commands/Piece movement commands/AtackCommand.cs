using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    public class AtackCommand : IPieceMoveCommand
    {
        public List<MoveDirection> Directions { get; private set; }


        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard gameBoard)
        {
            IPlayableChessPiece? piece = cell.Piece;
            if (piece == null) yield break;


            //check for moves to beat oponents pieces
            foreach (MoveDirection AtackMove in Directions)
            {

                int x = cell.X;
                int y = cell.Y;

                for (int i = 0; i < AtackMove.MoveDistance; i++)
                {
                    x += AtackMove.X_Direction;
                    y += AtackMove.Y_Direction;

                    //out of border
                    if (!gameBoard.TryGet_Cell_AtPosition(out ICell? atackedCell, x, y))
                    {
                        break;
                    }
                    //empty cell
                    else if (atackedCell!.Piece == null)
                    {
                        continue;
                    }
                    //enemy figure
                    else if (atackedCell.Piece.PieceColor != piece.PieceColor)
                    {
                        Move move = new Move(piece.PieceColor, piece, cell, atackedCell, MoveType.Capture);
                        move.Add_Update(new MoveUpdate(atackedCell, atackedCell.Piece, piece, MoveType.Capture));
                        move.Add_Update(new MoveUpdate(cell, piece, null, MoveType.PieceDeletion));
                        yield return move;
                        break;
                    }
                    //friendly figure
                    else break;

                }
            }
        }






        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            foreach (var direction in Directions)
            {
                int x = cell.X;
                int y = cell.Y;

                for (int i = 0; i < direction.MoveDistance; i++)
                {
                    x += direction.X_Direction;
                    y += direction.Y_Direction;

                    if (gameBoard.TryGet_Cell_AtPosition(out ICell? atackedCell, x, y))
                    {
                        yield return atackedCell!;

                        //якщо клітина не пуста
                        if (!atackedCell!.IsEmpty)
                        {
                            //потрібена для перевірки клітин за фігурою жертви (наприклад короля)
                            if (victim != null && victim.X == x && victim.Y == y) continue;
                            //якщо жертви нема і ми наткнулись на будь яку фігуру, то зупиняємо пошук
                            else break;
                        }
                    }
                    else break;
                }
            }
        }


        public IPieceMoveCommand Clone()
        {
            return new AtackCommand(Directions.Clone());
        }



        public AtackCommand(List<MoveDirection> moves)
        {
            if (moves != null)
            {
                Directions = moves;
            }
            else Directions = new List<MoveDirection>();
        }




        public  void Dispose()
        {
            Directions.Clear();
        }
    }
}
