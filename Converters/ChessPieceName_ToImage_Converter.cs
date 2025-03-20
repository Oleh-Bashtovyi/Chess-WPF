using Chess_game.Behaviour;
using Chess_game.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Chess_game.Converters
{
    public class ChessPieceName_ToImage_Converter : IValueConverter
    {



        //=================================================================================================================
        // FOR BEATEN LIST TO STORE IMAGES OF BEATEN PIECES
        //=================================================================================================================
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            //if (value is IChessPiece piece)
            if (value is string distinguishingName)
            {

                return PreparedImages.PieceNameToBitmap[distinguishingName];
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
