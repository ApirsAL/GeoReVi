namespace GeoReVi
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Markup;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

  public enum ScaleTO
  {
    OriginalSize=1,
    TargetSize=2
  }

  /// <summary>
  /// Source: http://blog.galasoft.ch/archive/2008/10/10/converting-and-customizing-xaml-to-png-with-server-side-wpf.aspx
  /// </summary>
  public class XamlToPngConverter
  {
    public void Convert(Stream xamlInput,
      double width, 
      double height,
      double dpiX,
      double dpiY,
      ScaleTO thisScale,
      Stream pngOutput)
    {
      try
      {
        // Round width and height to simplify
        width = Math.Round(width);
        height = Math.Round(height);

        Thread pngCreationThread = new Thread((ThreadStart) delegate()
        {
          FrameworkElement element = null;

          try
          {
            element = XamlReader.Load(xamlInput) as FrameworkElement;
          }
          catch (XamlParseException)
          {
          }

          if (element != null)
          {
            Size renderingSize = new Size(width, height);
            element.Measure(renderingSize);
            Rect renderingRectangle = new Rect(renderingSize);
            element.Arrange(renderingRectangle);

            BitmapSource xamlBitmap = RenderToBitmap(element, width, height, dpiX, dpiY, thisScale);
            try
            {
              PngBitmapEncoder enc = new PngBitmapEncoder();
              enc.Frames.Add(BitmapFrame.Create(xamlBitmap));
              enc.Save(pngOutput);
            }
            catch (ObjectDisposedException)
            {
              // IF the operation lasted too long, the object might be disposed already
            }
          }
        });

        pngCreationThread.IsBackground = true;
        pngCreationThread.SetApartmentState(ApartmentState.STA);
        pngCreationThread.Start();
        pngCreationThread.Join();
      }
      catch (Exception Exp)
      {
        throw Exp;
      }

      try
      {
        if (pngOutput.Length == 0)
        {
          throw new TimeoutException();
        }
      }
      catch (ObjectDisposedException)
      {
        // If the object was disposed, it means that the Stream is ready.
      }
    }

    private BitmapSource RenderToBitmap(FrameworkElement target,
                                        double width, 
                                        double height,
                                        double dpiX,
                                        double dpiY,
                                        ScaleTO thisScale)
    {
      Rect bounds;

      switch (thisScale)
      {
        case ScaleTO.OriginalSize:
          bounds = VisualTreeHelper.GetDescendantBounds(target);
          break;
        case ScaleTO.TargetSize:
          bounds = new Rect(0, 0, width,height);
          break;
        default:
          throw new ArgumentException(string.Format("The scaling mode: {0} is not supported.", thisScale.ToString()));
      }

      RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int) target.ActualWidth,
        (int)target.ActualHeight, dpiX, dpiY, PixelFormats.Pbgra32);

      DrawingVisual visual = new DrawingVisual();
      using (DrawingContext context = visual.RenderOpen())
      {
        VisualBrush brush = new VisualBrush(target);
        context.DrawRectangle(brush, null, new Rect(new Point(), bounds.Size));
      }

      renderBitmap.Render(visual);
      return renderBitmap;
    }
  }
}
