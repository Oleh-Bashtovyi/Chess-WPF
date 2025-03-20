using Chess_game.Behaviour.Interfaces;
using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Collections.Generic;

namespace Chess_game.Behaviour.Commands
{
    public class PawnEnPassantCommand : IPieceMoveCommand
    {
        public List<MoveDirection> Directions { get; private set; }

        public IEnumerable<IMove> Get_AvailableMoves_ForCell(ICell cell, GameBoard gameBoard)
        {
            IPlayableChessPiece? piece = cell.Piece;
            if (piece == null) yield break;

            //check for moves to beat oponents pieces
            foreach (MoveDirection EnPassantDirection in Directions)
            {
                //координати клітини зліва чи справа (де буде ворожа фігура для en passant)
                int x = cell.X;
                int y = cell.Y + EnPassantDirection.Y_Direction;

                if (!gameBoard.TryGet_Cell_AtPosition(out ICell? enPassantEnemyCell, x, y) ||
                   enPassantEnemyCell!.IsEmpty || enPassantEnemyCell.Piece!.PieceColor == piece.PieceColor ||
                   enPassantEnemyCell.Piece.EnPassantCellToBeatPiece == null ||
                   gameBoard.MoveNumber - enPassantEnemyCell.Piece.WasFirstlyPlayed != 0 ||
                   !gameBoard.TryGet_Cell_AtPosition( out ICell? cellToBeat,
                   enPassantEnemyCell.Piece.EnPassantCellToBeatPiece.Value.Item1,
                   enPassantEnemyCell.Piece.EnPassantCellToBeatPiece.Value.Item2)) continue;

                Move move = new Move(piece.PieceColor, piece, cell, cellToBeat!, MoveType.EnPassantCapture);
                move.Add_Update(new MoveUpdate(cellToBeat!, null, cell.Piece, MoveType.Move));
                move.Add_Update(new MoveUpdate(cell, cell.Piece, null, MoveType.PieceDeletion));
                move.Add_Update(new MoveUpdate(enPassantEnemyCell, enPassantEnemyCell.Piece, null, MoveType.Capture));
                yield return move;
                yield break;
            }
        }


        public IEnumerable<ICell> Get_AllCellsThatUnderAtack_OfCell(ICell cell, GameBoard gameBoard, ICell? victim = null)
        {
            yield break;
        }

        public IPieceMoveCommand Clone()
        {
            return new PawnEnPassantCommand(Directions.Clone());
        }



        public PawnEnPassantCommand(List<MoveDirection> moves)
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
