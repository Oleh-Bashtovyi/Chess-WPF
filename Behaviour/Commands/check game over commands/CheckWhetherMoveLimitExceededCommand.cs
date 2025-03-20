using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckWhetherMoveLimitExceededCommand : ICheckGameOverCommand
    {

        public int Priority => 9;
        public int ID => 6;


        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {
            if (gameSession.MovesLeft <= 0) return new EndGameInfo(GameOverType.Stalemate, "Unfortunately moves limit have been exceeded, so it is STALEMATE");
            else return EndGameInfo.GameContinues;
        }
    }


}
