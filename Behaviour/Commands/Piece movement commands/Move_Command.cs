using Chess_game.Controls;
using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands
{
/*    public class Move_Command : IPieceMove_Command
    {

        public List<MoveDirection> Directions { get; private set; }



        public IEnumerable<MoveUpdate> GetAvailableMovesForCell(ICell cell, GameBoard gameBoard)
        {
            ICell[][] board = gameBoard.Board;
            int Rows = board.Length;
            int Columns = board[0].Length;
            MoveUpdate pawnDeletion = new MoveUpdate(cell, cell.Piece, null);


            foreach (MoveDirection moveDirection in Directions)
            {
                int x = cell.X;
                int y = cell.Y;

                for (int i = 0; i < moveDirection.MoveDistance; i++)
                {
                    x += moveDirection.X_Direction;
                    y += moveDirection.Y_Direction;

                    if (!(x < 0 || x >= Rows || y < 0 || y >= Columns) && board[x][y].Piece == null)
                    {
                        MoveUpdate move = new MoveUpdate(board[x][y], board[x][y].Piece, board[cell.X][cell.Y].Piece);
                        move.NextUpdate = pawnDeletion;
                        yield return move;
                    }
                    else break;
                }
            }
        }





        public IEnumerable<ICell> GetCellsThatUnderAtackOfCell(ICell cell, GameBoard board, ICell? victim = null)
        {
            yield break;
        }


        public IPieceMove_Command Clone()
        {
            return new Move_Command(Directions.Clone());
        }



        public Move_Command(List<MoveDirection> moves)
        {
            if (moves != null)
            {
                Directions = moves;
            }
            else Directions = new List<MoveDirection>();
        }


    }*/
}
