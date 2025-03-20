using Chess_game.Controls;
using Chess_game.Controls.GameSessions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Behaviour.Commands.check_game_over_commands
{
    public class CheckWhetherAtLeastOnePieceIsExistingCommand : ICheckGameOverCommand
    {
        public int Priority => 2;
        public int ID => 4;




        public EndGameInfo CheckGameOver(AbstractGameSession gameSession, GameBoard board)
        {
            bool atLeastOneWhitePiece = board.Get_AllCellsWherePieces_OfColor(Color.White).Any();
            bool atLeastOneBlackPiece = board.Get_AllCellsWherePieces_OfColor(Color.Black).Any();

            if (!atLeastOneWhitePiece && !atLeastOneBlackPiece)
            {
                return new EndGameInfo(GameOverType.Stalemate, "There is no pieces existing to play a game!");
            }
            return EndGameInfo.GameContinues;
        }
    }
}
