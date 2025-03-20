using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Models
{
    public enum MoveType
    {
        [Description("move.mp3")]
        PieceDeletion = 0,
        [Description("move.mp3")]
        EnPassantMove = 1,
        [Description("move.mp3")]
        Move = 2,
        [Description("castle.mp3")]
        Castling = 3,
        [Description("capture.mp3")]
        Capture = 4,
        [Description("promote.mp3")]
        Promotion = 5,
        [Description("promote.mp3")]
        CapturePromotion = 6,
        [Description("capture.mp3")]
        EnPassantCapture = 7
    }
}
