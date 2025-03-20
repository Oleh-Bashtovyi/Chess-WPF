using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    internal class MoveAndAtackCommand : IPieceMoveCommand
    {
        public List<MoveDirection> Directions { get; private set; }

        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard gameBoard)
        {
            IPlayableChessPiece? piece = cell.Piece;
            if (piece == null) yield break;

            ICell[][] board = gameBoard.GameGrid;

            // searching for possible cells in all piece directions
            foreach (var moveDirection in Directions)
            {
                //remember piece coordinates
                int x = cell.X;
                int y = cell.Y;

                for (int i = 0; i < moveDirection.MoveDistance; i++)
                {
                    x += moveDirection.X_Direction;
                    y += moveDirection.Y_Direction;

                    //if it is out of border
                    if (!gameBoard.TryGet_Cell_AtPosition( out ICell? atackAndMoveCell, x, y)) break;
                    //if it is empty cell
                    else if (atackAndMoveCell!.Piece == null)
                    {
                        Move move = new Move(piece.PieceColor, piece, cell, atackAndMoveCell, MoveType.Move);
                        move.Add_Update(new MoveUpdate(atackAndMoveCell, null, piece, MoveType.Move));
                        move.Add_Update(new MoveUpdate(cell, piece, null, MoveType.PieceDeletion));
                        yield return move;
                    }
                    //if its is oponent piece, than we can beat it, and can not move further
                    else if (atackAndMoveCell.Piece?.PieceColor != piece.PieceColor)
                    {

                        Move move = new Move(piece.PieceColor, piece, cell, atackAndMoveCell, MoveType.Capture);
                        move.Add_Update(new MoveUpdate(atackAndMoveCell, atackAndMoveCell.Piece, piece, MoveType.Capture));
                        move.Add_Update(new MoveUpdate(cell, piece, null, MoveType.PieceDeletion));
                        yield return move;
                        break;
                    }
                    //it is friendly piece we can not move further
                    else break;
                }
            }
        }


        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            ICell[][] board = gameBoard.GameGrid;

            foreach (var direction in Directions)
            {
                int x = cell.X;
                int y = cell.Y;

                for (int i = 0; i < direction.MoveDistance; i++)
                {
                    x += direction.X_Direction;
                    y += direction.Y_Direction;

                    if ( gameBoard.TryGet_Cell_AtPosition(out ICell? atackedCell, x, y) )
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
            return new MoveAndAtackCommand(Directions.Clone());
        }


        public MoveAndAtackCommand(List<MoveDirection> moves)
        {
            if (moves != null)
            {
                Directions = moves;
            }
            else Directions = new List<MoveDirection>();
        }


        public void Dispose()
        {
            Directions.Clear();
        }
    }
}
