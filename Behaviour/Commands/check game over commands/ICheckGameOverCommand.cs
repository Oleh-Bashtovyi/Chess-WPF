using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;
using Newtonsoft.Json;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public interface ICheckGameOverCommand
    {
        [JsonIgnore]
        public int Priority { get; }
        public int ID { get; }
                
        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board);
    }
}
