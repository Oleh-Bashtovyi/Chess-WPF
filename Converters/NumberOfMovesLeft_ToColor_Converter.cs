using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Chess_game.Converters
{



    //=================================================================================================================
    // FOR MOVES LEFT TEXT BOX
    //=================================================================================================================
    class NumberOfMovesLeft_ToColor_Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int movesLeft)
            {
                if (movesLeft <= 20) return Brushes.Red; // Return the Red brush if movesLeft is 0
                else return Brushes.Black;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
