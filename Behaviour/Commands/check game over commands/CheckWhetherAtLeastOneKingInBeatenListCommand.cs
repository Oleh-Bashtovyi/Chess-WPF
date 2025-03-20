using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;
using System.Linq;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckWhetherAtLeastOneKingInBeatenListCommand : ICheckGameOverCommand
    {
        public int Priority => 1;
        public int ID => 3;



        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {
            bool killedAtLeastoneWhiteKingFigure = gameSession.BeatenPieces_White.Any(x => x.IsKingFigure);
            bool killedAtLeastoneBlackKingFigure = gameSession.BeatenPieces_Black.Any(x => x.IsKingFigure);

            if (killedAtLeastoneWhiteKingFigure && killedAtLeastoneBlackKingFigure)
            {
                return new EndGameInfo(GameOverType.Stalemate, "Both sides have lost at least one king, so it is STALEMATE");
            }
            if (killedAtLeastoneWhiteKingFigure)
            {
                return new EndGameInfo(GameOverType.Black_Win, "BLACK side killed oponent king! \nThe winner is: BLACK side!");
            }
            if (killedAtLeastoneBlackKingFigure)
            {
                return new EndGameInfo(GameOverType.White_Win, "WHITE side killed oponent king! \nThe winner is: WHITE side!");
            }
            return EndGameInfo.GameContinues;
        }
    }
    
}
