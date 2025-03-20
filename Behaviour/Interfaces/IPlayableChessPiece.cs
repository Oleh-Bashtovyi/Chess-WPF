using Chess_game.Models;
using System;

namespace Chess_game.Behaviour
{
    public interface IPlayableChessPiece : IChessPiece, IDisposable
    {

        short WasFirstlyPlayed { get; }
        bool IsPlayedAtLeastOnce { get;}
        Color PieceColor { get; }
        string GetDistinguishName { get; }
        string PieceEncoding { get; }
        (byte,byte)? EnPassantCellToBeatPiece { get; set; }

        public int GetValuability();
        public void MarkAsPlayed(short moveWhenItWasFirstlyPlayed);
        public void UnMarkAsPlayed(short moveWhenItWasPlayed);
        public IPlayableChessPiece Clone();
        public string Get_PieceEncoding();
        public bool Has_SameColor_With(IPlayableChessPiece otherPiece) => PieceColor == otherPiece.PieceColor;
        public bool Has_SameColor_With(Color color) => PieceColor == color;
    }
}
