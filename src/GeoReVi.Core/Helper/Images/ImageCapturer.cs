using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    public static class ImageCapturer
    {
        #region UI Elements
        //saving a framework element to bmp
        public static void SaveToBmp(UIElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        //Saving a framework element as png
        public static void SaveToPng(UIElement visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        public static void SaveToXaml(UIElement visual, string fileName)
        {
            Canvas can = new Canvas();

            try
            {
                can = (Canvas)visual;
            }
            catch
            {
                try
                {
                    can = FindVisualChild<Canvas>(visual);
                }
                catch
                {
                    return;
                }
            }

            try
            {
                FileStream f = new FileStream(fileName + ".xaml",
                    FileMode.Create, FileAccess.Write);
                XamlWriter.Save(can, f);
            }
            catch
            {
                return;
            }

        }

        //saving a framework element to bmp
        public static void SaveToPdf(UIElement visual, string fileName)
        {
            var encoder = new BmpBitmapEncoder();
            List<Bitmap> bitm = new List<Bitmap>() { UIElementToBitmap(visual, encoder, 300) };
            ExportHelper.ExportImageToPdf(bitm, fileName);

        }

        /// <summary>
        /// Saves a ui elemtn to an editable emf file
        /// </summary>
        /// <param name="visual"></param>
        /// <param name="fileName"></param>
        public static void SaveToEmf(UIElement visual, string fileName)
        {
            Canvas can = new Canvas();
            Grid grid = new Grid();

            try
            {
                can = (Canvas)visual;
                DrawCanvas(can, fileName);
            }
            catch
            {
                try
                {
                    can = FindVisualChild<Canvas>(visual);
                    DrawCanvas(can, fileName);
                }
                catch
                {
                    grid = (Grid)visual;
                    DrawCanvas(grid, fileName);
                }
            }
        }

        /// <summary>
        /// Draws a canvas
        /// </summary>
        /// <param name="can"></param>
        /// <param name="fileName"></param>
        private static void DrawCanvas(Canvas can, string fileName)
        {
            
            int w = Convert.ToInt32(can.Width);
            int h = Convert.ToInt32(can.Height);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);

            Bitmap bmp = new Bitmap(w, h);

            Graphics gs = Graphics.FromImage(bmp);

            Metafile mf = new Metafile(fileName, gs.GetHdc(), rect, MetafileFrameUnit.Pixel);

            Graphics g = Graphics.FromImage(mf);

            CanvasPainter painter = new CanvasPainter(g, can, bmp);

            painter.Draw();
            g.Save();
            g.Dispose();
            mf.Dispose();
        }

        /// <summary>
        /// Draws a grid
        /// </summary>
        /// <param name="can"></param>
        /// <param name="fileName"></param>
        private static void DrawCanvas(Grid can, string fileName)
        {
            int w = Convert.ToInt32(can.ActualWidth);
            int h = Convert.ToInt32(can.ActualHeight);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);

            Bitmap bmp = new Bitmap(w, h);

            Graphics gs = Graphics.FromImage(bmp);

            Metafile mf = new Metafile(fileName, gs.GetHdc(), rect, MetafileFrameUnit.Pixel);

            Graphics g = Graphics.FromImage(mf);

            GridPainter painter = new GridPainter(g, can, bmp);

            painter.Draw();
            g.Save();
            g.Dispose();
            mf.Dispose();
        }

        private static void SaveUsingEncoder(UIElement visual,
                     string fileName, BitmapEncoder encoder)
        {

            BitmapFrame frame = RenderVisual(visual,0,0,600);

            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        //Converting a frameworkelement to a bitmap
        public static Bitmap UIElementToBitmap(UIElement visual, BitmapEncoder encoder, int dpi)
        {
            //Memory stream for bitmap data
            MemoryStream stream = new MemoryStream();

            //Creating a bitmap frame
            BitmapFrame frame = RenderVisual(visual,0,0,dpi);

            encoder.Frames.Add(frame);
            frame = null;
            encoder.Save(stream);
            encoder = null;
            
            //Creating bitmap from memorystream
            return new Bitmap(stream);
        }

        //Returns a rescaled bitmapframe
        public static BitmapFrame RenderVisual(UIElement visual, int targetSizeHeight = 0, int targetSizeWidth = 0, double dpi = 300)
        {
            double dpiScale = dpi / 96;
            System.Windows.Size actualSize = new System.Windows.Size(visual.RenderSize.Width, visual.RenderSize.Height);

            if (targetSizeWidth == 0)
            {
                targetSizeHeight = (int)Math.Round(visual.RenderSize.Height * dpiScale, 0);
                targetSizeWidth = (int)Math.Round(visual.RenderSize.Width * dpiScale, 0);
            }

            System.Windows.Size targetSize = new System.Windows.Size(targetSizeWidth, targetSizeHeight);

            try
            {
                RenderTargetBitmap bmp = new RenderTargetBitmap(
                    Convert.ToInt32(targetSize.Width),
                    Convert.ToInt32(targetSize.Height),
                    dpi, 
                    dpi, 
                    PixelFormats.Default);

                bmp.Render(visual);

                return BitmapFrame.Create(bmp);
            }
            catch
            {
                return RenderVisual(visual, targetSizeHeight, targetSizeWidth, dpi - 100);
            }
            finally
            {
            }
        }

        #endregion

        #region Images in memory

        //Saving a Bitmap image as png
        public static void SaveToPng(BitmapImage visual, string fileName)
        {
            var encoder = new PngBitmapEncoder();
            SaveUsingEncoder(visual, fileName, encoder);
        }

        private static void SaveUsingEncoder(BitmapImage visual,
                     string fileName, BitmapEncoder encoder)
        {
            try
            {
                encoder.Frames.Add(BitmapFrame.Create(visual));

                using (var stream = File.Create(fileName))
                {
                    encoder.Save(stream);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Export not possible");
            }
        }

        #endregion

        #region Helpers

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        #endregion
    }
}
