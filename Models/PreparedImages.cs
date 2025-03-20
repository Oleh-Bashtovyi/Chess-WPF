using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Chess_game.Models
{
    public static class PreparedImages
    {

        public static readonly Dictionary<string, BitmapSource?> PieceNameToBitmap = new()
        {
            { "White_King", new BitmapImage(new Uri("Assets/White_King.png", UriKind.Relative)) },
            { "White_Queen", new BitmapImage(new Uri("Assets/White_Queen.png", UriKind.Relative)) },
            { "White_Bishop", new BitmapImage(new Uri("Assets/White_Bishop.png", UriKind.Relative)) },
            { "White_Knight", new BitmapImage(new Uri("Assets/White_Knight.png", UriKind.Relative)) },
            { "White_Rock", new BitmapImage(new Uri("Assets/White_Rock.png", UriKind.Relative)) },
            { "White_Pawn", new BitmapImage(new Uri("Assets/White_Pawn.png", UriKind.Relative)) },

            { "Black_King", new BitmapImage(new Uri("Assets/Black_King.png", UriKind.Relative)) },
            { "Black_Queen", new BitmapImage(new Uri("Assets/Black_Queen.png", UriKind.Relative)) },
            { "Black_Bishop", new BitmapImage(new Uri("Assets/Black_Bishop.png", UriKind.Relative)) },
            { "Black_Knight", new BitmapImage(new Uri("Assets/Black_Knight.png", UriKind.Relative)) },
            { "Black_Rock", new BitmapImage(new Uri("Assets/Black_Rock.png", UriKind.Relative)) },
            { "Black_Pawn", new BitmapImage(new Uri("Assets/Black_Pawn.png", UriKind.Relative)) },
            { "Fog", new BitmapImage(new Uri("Assets/Fog.png", UriKind.Relative)) },
            { "None", null },
        };
    }
}
