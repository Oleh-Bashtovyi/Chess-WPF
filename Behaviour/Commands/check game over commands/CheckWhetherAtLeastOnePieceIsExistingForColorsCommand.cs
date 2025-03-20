using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;
using System.Linq;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckWhetherAtLeastOnePieceIsExistingForColorsCommand : ICheckGameOverCommand
    {
        public int Priority => 4;
        public int ID => 5;

        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {
            bool atLeastOneWhitePiece = board.Get_AllCellsWherePieces_OfColor(Color.White).Any();
            bool atLeastOneBlackPiece = board.Get_AllCellsWherePieces_OfColor(Color.Black).Any();

            if (!atLeastOneWhitePiece && !atLeastOneBlackPiece)
            {
                return new EndGameInfo(GameOverType.Stalemate, "Both sides dont have resources to play a game!");
            }
            if (!atLeastOneWhitePiece)
            {
                return new EndGameInfo(GameOverType.Black_Win, "WHITE side dont have resources to continue game!");
            }
            if (!atLeastOneBlackPiece)
            {
                return new EndGameInfo(GameOverType.White_Win, "BLACK side dont have resources to continue game!");
            }
            return EndGameInfo.GameContinues;
        }
    }
}
