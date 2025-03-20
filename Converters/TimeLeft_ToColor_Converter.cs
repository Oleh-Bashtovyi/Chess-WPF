using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Chess_game.Converters
{
    class TimeLeft_ToColor_Converter :IValueConverter
    {


        //=================================================================================================================
        // FOR TIMER COLOR
        //=================================================================================================================

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int timeLeft)
            {
                if (timeLeft <= 60) return Brushes.Red;
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
