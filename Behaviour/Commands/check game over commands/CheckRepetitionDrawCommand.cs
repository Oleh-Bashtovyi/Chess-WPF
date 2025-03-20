using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;
using System.Linq;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckRepetitionDrawCommand : ICheckGameOverCommand
    {
        public int Priority => 10;
        public int ID => 2;

        
        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {
            if (CheckRepetitionDraw(gameSession))
            {
                return new EndGameInfo(GameOverType.Draw, "Same board positions repeated 3 times in a row. \nIt is STALEMATE!");
            }
            return EndGameInfo.GameContinues;

        }


        private bool CheckRepetitionDraw(AbstractGameSession gameSession)
        {
            if(gameSession.Undo_BoardHashList.Count > 0)
            {
                uint lastHash = gameSession.Undo_BoardHashList.Last();
                int count = 0;
                foreach(uint hash in gameSession.Undo_BoardHashList)
                {
                    if(hash == lastHash) count++;
                }
                return count >= 3;
            }
            return false;
        }
    }
}
