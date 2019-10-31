using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    public static class StringToFaciesTileConverter
    {
        public static System.Windows.Media.Brush Convert(string code)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Media\Brushes\LithologicalSections", code + ".png");

            if(File.Exists(path))
            {
                return new ImageBrush(new BitmapImage(new Uri(path, UriKind.Relative))) { Stretch=Stretch.Fill,
                    TileMode =TileMode.Tile,
                    Viewport = new System.Windows.Rect(0,0,60,60),
                    ViewportUnits =BrushMappingMode.Absolute};
            }
            else
            {
                return Brushes.LightGray;
            }
        }

        public static System.Windows.Media.Brush ConvertLegend(string code)
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Media\Brushes\LithologicalSections", code + ".png");

            if (File.Exists(path))
            {
                return new ImageBrush(new BitmapImage(new Uri(path, UriKind.Relative)))
                {
                    Stretch = Stretch.Fill,
                    TileMode = TileMode.Tile,
                    Viewport = new System.Windows.Rect(0, 0, 20, 20),
                    ViewportUnits = BrushMappingMode.Absolute };
            }
            else
            {
                return Brushes.LightGray;
            }
        }
    }
}
