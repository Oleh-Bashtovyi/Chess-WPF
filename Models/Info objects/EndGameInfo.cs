namespace Chess_game.Models
{
    public class EndGameInfo
    {

        public static readonly EndGameInfo GameContinues = new EndGameInfo(GameOverType.None, "Game continues");


        public GameOverType GameOverType { get;}
        public string Message { get; }


        public EndGameInfo(GameOverType gameOverType, string message)
        {
            GameOverType = gameOverType;
            Message = message;
        }

    }
}
