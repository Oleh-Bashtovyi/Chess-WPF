using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Chess_game.Converters
{

    //=================================================================================================================
    // FOR TIMER
    //=================================================================================================================
    class Number_ToTime_Converter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int timeLeft)
            {
                if (timeLeft < 0) return "00:00";
                else return $"{(timeLeft / 60):00}:{(timeLeft % 60):00}";
            }
            return null;
        }



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


