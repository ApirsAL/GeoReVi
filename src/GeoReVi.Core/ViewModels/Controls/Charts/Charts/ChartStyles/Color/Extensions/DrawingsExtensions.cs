using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    public static class DrawingsExtensions
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Bitmap ToBitmap(this double[,] array, ColormapBrush ColorMap)
        {
            int width = array.GetLength(1);
            int height = array.GetLength(0);

            System.Drawing.Bitmap Image = new System.Drawing.Bitmap(width, height);
            System.Drawing.Imaging.BitmapData bitmapData = Image.LockBits(
                new System.Drawing.Rectangle(0, 0, width, height),
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb
            );

            //Iterating over height
            for (int j = 0; j < height; j++)
            {
                //Iterating over width
                for (int k = 0; k < width; k++)

                    try
                    {
                        System.Windows.Media.Brush br = ColorMapHelper.GetBrush((double)array[j, k], ColorMap.Ymin, ColorMap.Ymax, ColorMap);
                        byte a = ((Color)br.GetValue(System.Windows.Media.SolidColorBrush.ColorProperty)).A;
                        byte g = ((Color)br.GetValue(System.Windows.Media.SolidColorBrush.ColorProperty)).G;
                        byte r = ((Color)br.GetValue(System.Windows.Media.SolidColorBrush.ColorProperty)).R;
                        byte b = ((Color)br.GetValue(System.Windows.Media.SolidColorBrush.ColorProperty)).B;

                        Image.SetPixel(k, j, System.Drawing.Color.FromArgb(a, r, g, b));
                    }
                    catch
                    {
                        continue;
                    }
            }

            return Image;
        }

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapImage retval;

            try
            {
                retval = (BitmapImage)Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }
    }
}
