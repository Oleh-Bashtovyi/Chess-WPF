using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Game_Data;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Chess_game.Factories
{
    public static class GameSession_Factory
    {
        private static Board_Builder boardBuilder = new Board_Builder();





        public static AbstractGameSession Create_GameSession_UsingPath(string path, GameStartupSettings gameStartupSettings)
        {
            string path1 = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Game Data");

            path = System.IO.Path.Combine(path1, path);

            return LoadService.Load_Game_UsingPath(path, gameStartupSettings);
        }




        public static AbstractGameSession Create_ClassicBoard_GameSession(GameStartupSettings gameStartupSettings)
        {
            boardBuilder.Build_ClassicBoard();
            GameBoard board = boardBuilder.GetGameBoard();

            switch (gameStartupSettings.GameMode)
            {
                case GameModeType.PvP:
                    return new PVP_GameSession(board, gameStartupSettings);
                case GameModeType.PvE:
                    return new PVE_GameSession(board, gameStartupSettings);
                case GameModeType.EvE:
                    return new EVE_GameSession(board, gameStartupSettings);
                default:
                    throw new ArgumentException("No data about such game session");
            }
        }



    }
}
