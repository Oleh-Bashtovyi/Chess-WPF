using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess_game.Extensions
{
    public static class MoveTypeToString_Extension
    {
        public static string ToName(this MoveType moveType)
        {
            switch (moveType)
            {
                case MoveType.Move:
                case MoveType.EnPassantMove:
                    return "Move";
                case MoveType.Capture:
                    return "Capture";
                case MoveType.EnPassantCapture:
                    return "En-passant capture";
                case MoveType.Promotion:
                    return "Promotion";
                case MoveType.Castling:
                    return "Castle";
                case MoveType.CapturePromotion:
                    return "Capture-promotion";
                default:
                    return "";
            }
        }
    }
}
