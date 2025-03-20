using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Chess_game.Models.Info_objects
{
    public class WindowsThemeInfo
    {
        private static readonly BrushConverter brushConverter = new BrushConverter();


        public string Name { get; private set; }
        public SolidColorBrush WindowBackgroundColor { get; private set; }
        public SolidColorBrush LightBoardCellColor { get; private set; }
        public SolidColorBrush DarkBoardCellColor { get; private set; }
        public SolidColorBrush TextColor { get; private set; }
        public SolidColorBrush ToolTipColor { get; private set; }
        public SolidColorBrush ToolTipTextColor { get; private set; }




        public WindowsThemeInfo(
            string name,
            SolidColorBrush windowBackgroundColor,
            SolidColorBrush lightBoardCellColor,
            SolidColorBrush darkBoardCellColor,
            SolidColorBrush textColor,
            SolidColorBrush toolTipColor,
            SolidColorBrush toolTipTextColor)
        {
            Name = name;
            WindowBackgroundColor = windowBackgroundColor;
            LightBoardCellColor = lightBoardCellColor;
            DarkBoardCellColor = darkBoardCellColor;
            TextColor = textColor;
            ToolTipColor = toolTipColor;
            ToolTipTextColor = toolTipTextColor;
        }




        public WindowsThemeInfo(
            string name,
            string hexWindowBackgroundColor,
            string hexLightBoardCellColor,
            string hexDarkBoardCellColor,
            string hexTextColor,
            string hexToolTipColor,
            string hexToolTipTextColor)
        {
            Name = name;
            WindowBackgroundColor = (brushConverter.ConvertFromString(hexWindowBackgroundColor) as SolidColorBrush ?? Brushes.White);
            LightBoardCellColor = (brushConverter.ConvertFromString(hexLightBoardCellColor) as SolidColorBrush ?? Brushes.LightYellow);
            DarkBoardCellColor = (brushConverter.ConvertFromString(hexDarkBoardCellColor) as SolidColorBrush ?? Brushes.SandyBrown);
            TextColor = (brushConverter.ConvertFromString(hexTextColor) as SolidColorBrush ?? Brushes.Black);
            ToolTipColor = (brushConverter.ConvertFromString(hexToolTipColor) as SolidColorBrush ?? Brushes.LightGoldenrodYellow);
            ToolTipTextColor = (brushConverter.ConvertFromString(hexToolTipTextColor) as SolidColorBrush ?? Brushes.Black);
        }
    }
}
