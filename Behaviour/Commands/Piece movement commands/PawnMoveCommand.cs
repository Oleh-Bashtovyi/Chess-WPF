using Chess_game.Behaviour.Commands.Action_after_move_Command;
using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    internal class PawnMoveCommand : IPieceMoveCommand
    {


        public List<MoveDirection> Directions { get; private set; }
        public Direction MoveDirection;


        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard gameBoard)
        {
            IPlayableChessPiece? piece = cell.Piece;
            if(piece == null) yield break;


            int x = cell.X;
            int y = cell.Y;

            for (int i = 0; i < (piece.IsPlayedAtLeastOnce ? 1 : 2); i++)
            {
                x += Directions[0].X_Direction;
                y += Directions[0].Y_Direction;

                if (!gameBoard.TryGet_Cell_AtPosition(out ICell? pawnMoveCell, x, y) || !pawnMoveCell!.IsEmpty)
                {
                    yield break;
                }
                else
                {
                    MoveUpdate update = new MoveUpdate(pawnMoveCell, null, piece, MoveType.Move);
                    if (i == 1)
                    {
                        if (gameBoard.TryGet_Cell_AtPosition(out ICell? enPassantCellForCapture, x - Directions[0].X_Direction, y - Directions[0].Y_Direction))
                        {
                            AddCellForEnPassantCapture_Command command = new(piece, enPassantCellForCapture!);
                            update.AdditionAction = command;
                        }
                    }
                    Move move = new Move(piece.PieceColor, piece, cell, pawnMoveCell, MoveType.Move);
                    move.Add_Update(update);
                    move.Add_Update(new MoveUpdate(cell, piece, null, MoveType.PieceDeletion));

                    yield return move;
                }
            }
        }


        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            yield break;
        }

        public IPieceMoveCommand Clone()
        {
            return new PawnMoveCommand(MoveDirection);
        }


        public PawnMoveCommand(Direction moveDirection)
        {
            MoveDirection = moveDirection;
            switch (moveDirection)
            {
                case Direction.Up:
                    {
                        Directions = new List<MoveDirection>
                        {
                            new MoveDirection(-1, 0, 1),
                        };
                        break;
                    }
                case Direction.Down:
                    {
                        Directions = new List<MoveDirection>
                        {
                            new MoveDirection(1, 0, 1),
                        };
                        break;
                    }
                default:
                    {
                        Directions = new List<MoveDirection>
                        {
                            new MoveDirection(0, 0, 1),
                        };
                        break;
                    }
                    //continue...
            }
        }

        public void Dispose()
        {
            Directions.Clear();
        }

    }
}
