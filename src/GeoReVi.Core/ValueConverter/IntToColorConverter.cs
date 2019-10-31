using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GeoReVi
{
    public static class IntToColorConverter
    {
        public static System.Windows.Media.SolidColorBrush Convert(int number)
        {
            switch (number)
            {
                case 0:
                    return System.Windows.Media.Brushes.DimGray;
                case 1:
                    return System.Windows.Media.Brushes.Gray;
                case 2:
                    return System.Windows.Media.Brushes.DarkGray;
                case 3:
                    return System.Windows.Media.Brushes.Gainsboro;
                case 4:
                    return System.Windows.Media.Brushes.LightGray;
                case 5:
                    return System.Windows.Media.Brushes.LightSteelBlue;
                case 6:
                    return System.Windows.Media.Brushes.Silver;
                case 7:
                    return System.Windows.Media.Brushes.WhiteSmoke;
                case 8:
                    return System.Windows.Media.Brushes.Ivory;
                case 9:
                    return System.Windows.Media.Brushes.Cornsilk;
                case 10:
                    return System.Windows.Media.Brushes.GhostWhite;
                case 11:
                    return System.Windows.Media.Brushes.AliceBlue;
                default:
                    return System.Windows.Media.Brushes.Gray;
            }
        }

        public static int ConvertBack(System.Windows.Media.Brush color)
        {

            if (color == System.Windows.Media.Brushes.DimGray)
                return 0;
            if (color == System.Windows.Media.Brushes.Gray)
                return 1;
            if (color == System.Windows.Media.Brushes.DarkGray)
                return 2;
            if (color == System.Windows.Media.Brushes.Gainsboro)
                return 3;
            if (color == System.Windows.Media.Brushes.LightGray)
                return 4;
            if (color == System.Windows.Media.Brushes.LightSteelBlue)
                return 5;
            if (color == System.Windows.Media.Brushes.Silver)
                return 6;
            if (color == System.Windows.Media.Brushes.WhiteSmoke)
                return 7;
            if (color == System.Windows.Media.Brushes.Ivory)
                return 8;
            if (color == System.Windows.Media.Brushes.Cornsilk)
                return 9;
            if (color == System.Windows.Media.Brushes.GhostWhite)
                return 10;
            if (color == System.Windows.Media.Brushes.AliceBlue)
                return 11;
            if (color == System.Windows.Media.Brushes.Gray)
                return 12;
            else
                return 0;
        }
    }
}
