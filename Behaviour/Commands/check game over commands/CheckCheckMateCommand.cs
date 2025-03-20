using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Extensions;
using Chess_game.Models;
using System.Linq;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckCheckMateCommand : ICheckGameOverCommand
    {
        public int Priority => 3;
        public int ID => 1;


        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {

            switch (CheckForCheckMateAndStalemate(gameSession, board))
            {
                case GameOverType.Stalemate:
                    return new EndGameInfo(GameOverType.Stalemate, "Oponent doesnt have moves to perform! It is STALEMATE");
                case GameOverType.White_Win:
                    return new EndGameInfo(GameOverType.White_Win, "WHITE side have checkmated oponent! \nThe winner is: WHITE side!");
                case GameOverType.Black_Win:
                    return new EndGameInfo(GameOverType.Black_Win, "BLACK side have checkmated oponent! \nThe winner is: BLACK side!");
                case GameOverType.None:
                default:
                    return EndGameInfo.GameContinues;
            }

        }

        private GameOverType CheckForCheckMateAndStalemate(AbstractGameSession gameSession, GameBoard board)
        {
            GameOverType gameoverForWhite = Check_CheckMateAndStalemate_ForColor(gameSession, board, Color.White);
            GameOverType gameoverForBlack = Check_CheckMateAndStalemate_ForColor(gameSession, board, Color.Black);

            if (gameoverForWhite != GameOverType.None || gameoverForBlack != GameOverType.None)
            {
                if (gameoverForWhite == GameOverType.Stalemate || gameoverForBlack == GameOverType.Stalemate) return GameOverType.Stalemate;
                else return (gameoverForWhite == GameOverType.None) ? gameoverForBlack : gameoverForWhite;

            }
            return GameOverType.None;
        }




        public GameOverType Check_CheckMateAndStalemate_ForColor(AbstractGameSession gameSession, GameBoard board, Color color)
        {
            bool[,] dangerousCellsForPlayer = board.Get_MatrixOfCellsThatUnderAtackByPieces_OfColor( color.GetOpositeColor() );
            bool isAtLeastOneKingInDanger = false;


            foreach (ICell cell in board.Get_AllCellsWherePiecesIsKingFigure_OfColor(color))
            {
                if (dangerousCellsForPlayer[cell.X, cell.Y])
                {
                    isAtLeastOneKingInDanger = true;
                    break;
                }
            }


            //if checked at least one king
            if (isAtLeastOneKingInDanger)
            {
                //and there no moves that will make all kings unchecked, then it is checkmate
                if (!ExistAtLeastOneMoveForPiecesWithColor(board, color))
                {
                    return (color == Color.Black) ? GameOverType.White_Win : GameOverType.Black_Win;
                }
            }
            //if all kings not in danger but there are no moves for player, then it is a stealmate
            else if (gameSession.CurrentPlayerMove != color && !ExistAtLeastOneMoveForPiecesWithColor(board, color))
            {
                return GameOverType.Stalemate;
            }
            return GameOverType.None;
        }



        private bool ExistAtLeastOneMoveForPiecesWithColor(GameBoard board, Color color)
        {
            foreach (ICell cell in board.Get_AllCellsWherePieces_OfColor(color))
            {
                if (board.Get_AllPossibleMoves_WithKingsFiguresProtection_ForCell(cell).Any()) return true;
            }
            return false;
        }

    }
}
