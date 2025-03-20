using Chess_game.Extensions;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Chess_game.Controls.GameSessions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Chess_game.Controls
{
    public class Sound_Handler
    {
        private static Sound_Handler _instance = new Sound_Handler();

        #region SOUND EVENT
        public event Action<string>? OnSoundPlay;
        #endregion

        private Sound_Handler() { }

        public static Sound_Handler Get_Instance() => _instance;





        public void PlaySound_UsingMoveType(AbstractGameSession sender, MoveType MoveType)
        {
            string filename;

            if (sender.IsGameOver && sender.RedoList.Count() == 0)
            {
                filename = "game-end.mp3";
            }
            else if (sender.IsKingInDangerShouldBeShown &&
                sender.Board.Get_AllCellsWithKingFiguresThatInDanger_ForColor(sender.CurrentPlayerMove.GetOpositeColor()).Count() > 0)
            {
                filename = "move_check.mp3";
            }
            else
            {
                switch (MoveType)
                {
                    case MoveType.Move:
                    case MoveType.EnPassantMove:
                        filename = "move.mp3";
                        break;
                    case MoveType.Capture:
                    case MoveType.EnPassantCapture:
                        filename = "capture.mp3";
                        break;
                    case MoveType.Castling:
                        filename = "castle.mp3";
                        break;
                    case MoveType.Promotion:
                    case MoveType.CapturePromotion:
                        filename = "promote.mp3";
                        break;
                    default:
                        return;
                }
            }
            OnSoundPlay?.Invoke(filename);
        }


        public void PlayEndGameSound()
        {
            OnSoundPlay?.Invoke("game-end.mp3");
        }
    }
}
