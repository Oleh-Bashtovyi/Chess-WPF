using Chess_game.Behaviour.Commands.Action_after_move_Command;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    public class CastleMoveCommand : IPieceMoveCommand
    {

        public List<MoveDirection> Directions { get; private set; }


        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard gameBoard)
        {
            IPlayableChessPiece? piece = cell.Piece;
            if(piece == null) yield break;


            bool[,] DangerousCells = gameBoard.Get_MatrixOfCellsThatUnderAtackByPieces_OfColor(piece.PieceColor.GetOpositeColor(), cell);

            //якщо фігура не зіграна і (це королівська фігура, яка не під шахом/ або звичайна) і рокировка ще не була зроблена
            if ( piece.IsAbleToCastle && !piece.IsPlayedAtLeastOnce &&
                ((piece.IsKingFigure && !DangerousCells[cell.X, cell.Y]) || !piece.IsKingFigure) &&
                ((piece.PieceColor == Color.Black) ? !gameBoard.IsBlackTeamCastled : !gameBoard.IsWhiteTeamCastled))
            {

                ICell[][] board = gameBoard.GameGrid;


                //перевіряємо кожен напрям рокировки
                foreach (MoveDirection direction in Directions)
                {
                    int xDirection = direction.X_Direction;
                    int yDirection = direction.Y_Direction;

                    //місце для переміщення від час рокіровки повинно бути мінімум 2 клітини від виконавця
                    int x = cell.X + 2 * xDirection;
                    int y = cell.Y + 2 * yDirection;

                    //якщо можна перейти на сусідні 2 клітини і вони не під атакою(для королівської фігури), то перевіримо можливість на рокировку
                    if (!gameBoard.TryGet_Cell_AtPosition(out ICell? castleMoveCell, x, y) ||
                            !castleMoveCell!.IsEmpty || !board[x - xDirection][y - yDirection].IsEmpty ||
                            (piece.IsKingFigure && (DangerousCells[x, y] || DangerousCells[x - xDirection, y - yDirection]))) continue;
                    else
                    {
                        //джерело рокировки повинно знаходитись мінімум 3 клітини від фігури, що її робить
                        x += xDirection;
                        y += yDirection;

                        while (gameBoard.TryGet_Cell_AtPosition(out ICell? castleSourceCell, x, y))
                        {
                            //клітина не пуста
                            if (castleSourceCell!.Piece != null)
                            {
                                //якщо це джерело рокировки того самого кольору та не зігране
                                if (castleSourceCell.Piece.IsCastleSource  && castleSourceCell.Piece.PieceColor == piece.PieceColor && 
                                    (!castleSourceCell.Piece.IsPlayedAtLeastOnce))
                                {
                                    Move move = new Move(piece.PieceColor, piece, cell, castleMoveCell, MoveType.Castling);

                                    //king figure
                                    //робимо команду яка помітить певну сторону, що вона вже рокіровалась
                                    MarkSideAsCastledCommand castleCommand = new(piece.PieceColor);
                                    move.Add_Update(new MoveUpdate(castleMoveCell, null, piece, MoveType.Castling, castleCommand));
                                    move.Add_Update(new MoveUpdate(cell, piece, null, MoveType.PieceDeletion));

                                    //castle source
                                    //робимо команду, яка помітить джерело для рокіровки (наприклад башню) як зіграну фігуру
                                    MarkPieceAsPlayed_Command command = new(castleSourceCell.Piece);
                                    move.Add_Update(new MoveUpdate(board[cell.X + xDirection][cell.Y + yDirection], null, castleSourceCell.Piece, MoveType.Castling, command));
                                    move.Add_Update(new MoveUpdate(castleSourceCell, castleSourceCell.Piece, null, MoveType.PieceDeletion));

                                    yield return move;
                                    break;

                                }
                                //інакше це чужа фігура, припиняємо рокировку
                                else break;
                            }

                            x += xDirection;
                            y += yDirection;
                        }
                    }
                }
            }


        }




        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            yield break;
        }

        public IPieceMoveCommand Clone()
        {
            return new CastleMoveCommand(Directions.Clone());
        }

        public CastleMoveCommand(List<MoveDirection> moves)
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
