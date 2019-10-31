using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeoReVi
{
    /// <summary>
    /// A canvas painter which transforms WPF shapes into GDI+ objects
    /// </summary>
    public class CanvasPainter
    {
        public System.Drawing.Graphics DrawGraphics;
        public Canvas Container = new Canvas();
        private Bitmap bm = null;
        List<Shape> ShapeList = new List<Shape>();
        List<TextBlock> TextList = new List<TextBlock>();


        public CanvasPainter(System.Drawing.Graphics _DrawGraphics, Canvas _Container, Bitmap _bm = null)
        {
            DrawGraphics = _DrawGraphics;
            Container = _Container;
            bm = _bm;
            //Get the Shape elements in the Canvas
            ShapeList = AnalyseShape();
            TextList = AnalyseText();
        }

        public List<TextBlock> AnalyseText()
        {
            List<TextBlock> list = new List<TextBlock>();

            //Container.Children;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                UIElement element = Container.Children[i];

                Type t = element.GetType();

                if (t == typeof(TextBlock))
                {
                    list.Add(element as TextBlock);
                }
                else if (t == typeof(System.Windows.Controls.Canvas))
                {
                    Canvas can = (Canvas)element;

                    //Container.Children;
                    for (int j = 0; j < can.Children.Count; j++)
                    {
                        element = can.Children[j];

                        t = element.GetType();

                        if (t == typeof(TextBlock))
                        {
                            list.Add(element as TextBlock);
                        }
                        else if (t == typeof(System.Windows.Controls.Canvas))
                        {
                            can = (Canvas)element;

                            //Container.Children;
                            for (int k = 0; k < can.Children.Count; k++)
                            {
                                element = can.Children[k];

                                t = element.GetType();

                                if (t == typeof(TextBlock))
                                {
                                    list.Add(element as TextBlock);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public List<Shape> AnalyseShape()
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                try
                {

                    UIElement element = Container.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        Canvas can = (Canvas)element;

                        //Container.Children;
                        for (int k = 0; k < can.Children.Count; k++)
                        {
                            try
                            {
                                element = can.Children[k];

                                t = element.GetType();

                                if (t == typeof(System.Windows.Shapes.Ellipse))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Shapes.Rectangle))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Shapes.Line))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Shapes.Polyline))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Shapes.Polygon))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Shapes.Rectangle))
                                {
                                    list.Add(element as Shape);
                                }
                                else if (t == typeof(System.Windows.Controls.Canvas))
                                {
                                    Canvas can2 = (Canvas)element;

                                    //Container.Children;
                                    for (int j = 0; j < can2.Children.Count; j++)
                                    {
                                        try
                                        {
                                            element = can2.Children[j];

                                            t = element.GetType();

                                            if (t == typeof(System.Windows.Shapes.Ellipse))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Shapes.Rectangle))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Shapes.Line))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Shapes.Polyline))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Shapes.Polygon))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Shapes.Rectangle))
                                            {
                                                list.Add(element as Shape);
                                            }
                                            else if (t == typeof(System.Windows.Controls.Canvas))
                                            {
                                                Canvas can3 = (Canvas)element;

                                                //Container.Children;
                                                for (int l = 0; l < can3.Children.Count; l++)
                                                {
                                                    try
                                                    {
                                                        element = can3.Children[l];

                                                        t = element.GetType();

                                                        if (t == typeof(System.Windows.Shapes.Ellipse))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Shapes.Rectangle))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Shapes.Line))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Shapes.Polyline))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Shapes.Polygon))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Shapes.Rectangle))
                                                        {
                                                            list.Add(element as Shape);
                                                        }
                                                        else if (t == typeof(System.Windows.Controls.Canvas))
                                                        {
                                                            Canvas can4 = (Canvas)element;

                                                            //Container.Children;
                                                            for (int m = 0; m < can4.Children.Count; m++)
                                                            {
                                                                try
                                                                {
                                                                    element = can.Children[m];

                                                                    t = element.GetType();

                                                                    if (t == typeof(System.Windows.Shapes.Ellipse))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                    else if (t == typeof(System.Windows.Shapes.Line))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                    else if (t == typeof(System.Windows.Shapes.Polyline))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                    else if (t == typeof(System.Windows.Shapes.Polygon))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                                                                    {
                                                                        list.Add(element as Shape);
                                                                    }
                                                                }
                                                                catch
                                                                {
                                                                    continue;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
                // You can write more code to support more type here
                //... ...
            }
            return list;
        }

        public void Draw()
        {
            //Convert WPF shape to GDI+ shape and Draw GDI+
            for (int i = 0; i < ShapeList.Count; i++)
            {
                try
                {
                    Shape element = ShapeList[i];
                    Type t = element.GetType();
                    Type tParent1 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element)).GetType();
                    Type tParent2 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element))).GetType();


                    float x = 0;
                    float y = 0;
                    float w = 0;
                    float h = 0;

                    if (tParent1 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y);
                    }
                    else if (tParent1 == typeof(Canvas) && tParent2 != typeof(Canvas))
                    {
                        float x1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        float y1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y);
                        float x2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        float y2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).Y);

                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y);
                    }
                    else
                    {
                        float x1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        float x2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        float x3 = Convert.ToSingle(VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y);

                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y);
                    }

                    w = Convert.ToSingle(element.Width);
                    h = Convert.ToSingle(element.Height);

                    System.Drawing.SolidBrush GDIStroke = ConvertSolidColorBrush(element.Stroke as SolidColorBrush);

                    System.Drawing.SolidBrush GDIFill = ConvertSolidColorBrush(element.Fill as SolidColorBrush);

                    float Thickness = Convert.ToSingle(element.StrokeThickness);

                    System.Drawing.Pen pen = new System.Drawing.Pen(GDIStroke, Thickness);

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        DrawEllipse(x, y, w, h, pen, GDIFill);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        System.Windows.Shapes.Rectangle myRectangle = (System.Windows.Shapes.Rectangle)element;

                        DrawRectangle(x, y, w, h, pen, GDIFill);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        Polyline myPolyline = (System.Windows.Shapes.Polyline)element;
                        List<PointF> pts = new List<PointF>();

                        foreach (System.Windows.Point point in myPolyline.Points)
                        {
                            if (x + (float)point.X <= x || y + (float)point.Y <= y || (float)point.X >= ((Canvas)VisualTreeHelper.GetParent(element)).ActualWidth)
                                continue;

                            PointF pt = new PointF((float)point.X + x, (float)point.Y + y);
                            pts.Add(pt);
                        }

                        //Check if points are ordered
                        var ptsA = pts.OrderByDescending(f => f.X).ToList();
                        var ptsB = pts.OrderByDescending(f => f.Y).ToList();

                        if (ptsA.SequenceEqual(pts))
                            pts = ptsA;
                        else if (ptsB.SequenceEqual(pts))
                            pts = ptsB;
                        else
                            pts = ptsA;

                        System.Drawing.PointF[] points = pts.ToArray();

                        DrawPolyline(pen, points);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        Line myLine = (System.Windows.Shapes.Line)element;

                        PointF pt1 = new PointF(Convert.ToSingle(myLine.X1 + x), Convert.ToSingle(myLine.Y1 + y));
                        PointF pt2 = new PointF(Convert.ToSingle(myLine.X2 + x), Convert.ToSingle(myLine.Y2 + y));

                        DrawLine(pen, pt1, pt2);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        Polygon myPolygon = (System.Windows.Shapes.Polygon)element;
                        List<PointF> pts = new List<PointF>();

                        foreach (System.Windows.Point point in myPolygon.Points)
                        {
                            PointF pt = new PointF((float)point.X + x, (float)point.Y + y);
                            pts.Add(pt);
                        }

                        System.Drawing.PointF[] points = pts.ToArray();

                        DrawPolygon(pen, points, GDIFill);
                    }
                }
                catch
                {
                    continue;
                }
            }

            for (int i = 0; i < TextList.Count; i++)
            {
                try
                {
                    TextBlock element = TextList[i];
                    Type tParent1 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element)).GetType();
                    Type tParent2 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element))).GetType();

                    float x = 0;
                    float y = 0;
                    float w = 0;
                    float h = 0;

                    if (tParent1 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + element.RenderTransform.Value.OffsetX - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + element.RenderTransform.Value.OffsetY - element.DesiredSize.Height / 4);
                    }
                    else if (tParent1 == typeof(Canvas) && tParent2 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y - element.DesiredSize.Height / 4);
                    }
                    else
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).X - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y - element.DesiredSize.Height / 4);
                    }

                    //x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X - element.RenderTransform.Value.OffsetX);

                    //y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y - element.RenderTransform.Value.OffsetY);

                    Font font = new Font(new FontConverter.FontNameConverter().ConvertToString(element.FontFamily),
                                         Convert.ToSingle((double)new FontSizeConverter().ConvertFrom(element.FontSize - 2)),
                                         System.Drawing.FontStyle.Regular);

                    System.Drawing.SolidBrush GDIStroke = ConvertSolidColorBrush(element.Foreground as SolidColorBrush);

                    try
                    {
                        TransformGroup rotate = new TransformGroup();
                        rotate = (TransformGroup)element.RenderTransform;
                        var radians = Math.Atan2(rotate.Value.M21, rotate.Value.M11);
                        float degree = Convert.ToSingle(radians * 180 / Math.PI);
                        DrawText(element.Text, font, GDIStroke, new PointF(x, y), degree);

                    }
                    catch
                    {
                        MatrixTransform rotate = new MatrixTransform();
                        rotate = (MatrixTransform)element.RenderTransform;
                        var radians = Math.Atan2(rotate.Value.M21, rotate.Value.M11);
                        float degree = Convert.ToSingle(radians * 180 / Math.PI);
                        DrawText(element.Text, font, GDIStroke, new PointF(x, y), degree);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        public void DrawEllipse(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawEllipse(pen, x, y, w, h);
            DrawGraphics.FillEllipse(GDIFill, x, y, w, h);
        }
        public void DrawRectangle(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawRectangle(pen, x, y, w, h);
            DrawGraphics.FillRectangle(GDIFill, x, y, w, h);
        }

        public void DrawPolygon(System.Drawing.Pen pen, System.Drawing.PointF[] points, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawPolygon(pen, points);
            DrawGraphics.FillPolygon(GDIFill, points);
        }

        public void DrawLine(System.Drawing.Pen pen, System.Drawing.PointF pt1, System.Drawing.PointF pt2)
        {
            DrawGraphics.DrawLine(pen, pt1, pt2);
        }

        public void DrawPolyline(System.Drawing.Pen pen, System.Drawing.PointF[] points)
        {
            DrawGraphics.DrawLines(pen, points);
        }

        public void DrawText(string text, Font font, System.Drawing.SolidBrush GDIStroke, System.Drawing.PointF point, float rotationAngle = 0)
        {
            if (rotationAngle == 0)
                DrawGraphics.DrawString(text, font, GDIStroke, point);
            else
            {
                if (bm != null)
                {
                    //Translating the graphics object to the spot of the object
                    //where the object will be rotated
                    DrawGraphics.TranslateTransform(point.X, point.Y);

                    //Rotating by the object angle
                    DrawGraphics.RotateTransform(-rotationAngle);

                    //Translating back to the original position
                    DrawGraphics.TranslateTransform(-point.X, -point.Y);

                    //Drawing the text to the object
                    DrawGraphics.DrawString(text, font, GDIStroke, point);

                    //Resetting the transformation
                    DrawGraphics.ResetTransform();

                }
                else
                {
                    //Drawing the text to the object
                    DrawGraphics.DrawString(text, font, GDIStroke, point);
                }
            }
        }

        public System.Drawing.SolidBrush ConvertSolidColorBrush(SolidColorBrush scb)
        {
            if (scb != null)
                return new SolidBrush(ConvertColor(scb.Color));

            else
                return new SolidBrush(ConvertColor(Colors.Transparent));
        }

        public System.Drawing.Color ConvertColor(System.Windows.Media.Color WPFColor)
        {
            return System.Drawing.Color.FromArgb(WPFColor.A, WPFColor.R, WPFColor.G, WPFColor.B);
        }
    }

    /// <summary>
    /// A canvas painter which transforms WPF shapes into GDI+ objects
    /// </summary>
    public class GridPainter
    {
        public System.Drawing.Graphics DrawGraphics;
        public Grid Container = new Grid();
        private Bitmap bm = null;
        List<Shape> ShapeList = new List<Shape>();
        List<TextBlock> TextList = new List<TextBlock>();


        public GridPainter(System.Drawing.Graphics _DrawGraphics, Grid _Container, Bitmap _bm = null)
        {
            DrawGraphics = _DrawGraphics;
            Container = _Container;
            bm = _bm;
            //Get the Shape elements in the Canvas
            ShapeList = AnalyseShape();
        }

        public List<Shape> AnalyseShape()
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < Container.Children.Count; i++)
            {
                try
                {
                    UIElement element = Container.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape((ItemsControl)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Border)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
                // You can write more code to support more type here
                //... ...
            }

            return list;
        }

        public List<Shape> AnalyseShape(ItemsControl itemsControl)
        {
            List<Shape> list = new List<Shape>();

            ItemsControl can = (ItemsControl)itemsControl;

            //Container.Children;
            for (int k = 0; k < can.Items.Count; k++)
            {
                try
                {
                    UIElement element = (UIElement)itemsControl.ItemContainerGenerator.ContainerFromItem(can.Items[k]);

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape());
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.ContentPresenter))
                    {
                        list.AddRange(AnalyseShape((ContentPresenter)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {

                }
            }

            return list;
        }

        public List<Shape> AnalyseShape(Canvas canvas)
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < canvas.Children.Count; i++)
            {
                try
                {
                    UIElement element = canvas.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape((ItemsControl)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
                // You can write more code to support more type here
                //... ...
            }

            return list;
        }

        public List<Shape> AnalyseShape(Border border)
        {
            List<Shape> list = new List<Shape>();

            try
            {
                UIElement element = border.Child;

                Type t = element.GetType();

                if (t == typeof(System.Windows.Shapes.Ellipse))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Rectangle))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Polygon))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Polyline))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Line))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Controls.ItemsControl))
                {
                    list.AddRange(AnalyseShape((ItemsControl)element));
                }
                else if (t == typeof(System.Windows.Controls.Canvas))
                {
                    list.AddRange(AnalyseShape((Canvas)element));
                }
                else if (t == typeof(System.Windows.Controls.Border))
                {
                    list.AddRange(AnalyseShape((Canvas)element));
                }
                else if (t == typeof(System.Windows.Controls.Grid))
                {
                    list.AddRange(AnalyseShape((Grid)element));
                }
                else if (t == typeof(System.Windows.Controls.DockPanel))
                {
                    list.AddRange(AnalyseShape((DockPanel)element));
                }
                else if (t == typeof(System.Windows.Controls.StackPanel))
                {
                    list.AddRange(AnalyseShape((StackPanel)element));
                }
                else if (t == typeof(TextBlock))
                {
                    TextList.Add(element as TextBlock);
                }
                else if (t == typeof(System.Windows.Controls.ContentPresenter))
                {
                    list.AddRange(AnalyseShape((ContentPresenter)element));
                }
                else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                {
                    list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                }
                else
                {
                    
                }
            }
            catch
            {

            }
            // You can write more code to support more type here
            //... ...

            return list;
        }

        public List<Shape> AnalyseShape(Grid grid)
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < grid.Children.Count; i++)
            {
                try
                {
                    UIElement element = grid.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape((ItemsControl)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.ContentPresenter))
                    {
                        list.AddRange(AnalyseShape((ContentPresenter)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
            // You can write more code to support more type here
            //... ...

            return list;
        }

        public List<Shape> AnalyseShape(DockPanel dockPanel)
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < dockPanel.Children.Count; i++)
            {
                try
                {
                    UIElement element = dockPanel.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape((ItemsControl)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.ContentPresenter))
                    {
                        list.AddRange(AnalyseShape((ContentPresenter)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
            // You can write more code to support more type here
            //... ...

            return list;
        }

        public List<Shape> AnalyseShape(StackPanel stackPanel)
        {
            List<Shape> list = new List<Shape>();

            //Container.Children;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                try
                {
                    UIElement element = stackPanel.Children[i];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape((ItemsControl)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.ContentPresenter))
                    {
                        list.AddRange(AnalyseShape((ContentPresenter)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {
                    continue;
                }
            }
            // You can write more code to support more type here
            //... ...

            return list;
        }

        public List<Shape> AnalyseShape(ContentPresenter contentPresenter)
        {
            List<Shape> list = new List<Shape>();

            try
            {
                UIElement element = (UIElement)VisualTreeHelper.GetChild(contentPresenter, 0);

                Type t = element.GetType();

                if (t == typeof(System.Windows.Shapes.Ellipse))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Rectangle))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(TextBlock))
                {
                    TextList.Add(element as TextBlock);
                }
                else if (t == typeof(System.Windows.Shapes.Polygon))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Polyline))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Shapes.Line))
                {
                    list.Add(element as Shape);
                }
                else if (t == typeof(System.Windows.Controls.ItemsControl))
                {
                    list.AddRange(AnalyseShape((ItemsControl)element));
                }
                else if (t == typeof(System.Windows.Controls.Canvas))
                {
                    list.AddRange(AnalyseShape((Canvas)element));
                }
                else if (t == typeof(System.Windows.Controls.Border))
                {
                    list.AddRange(AnalyseShape((Border)element));
                }
                else if (t == typeof(System.Windows.Controls.Grid))
                {
                    list.AddRange(AnalyseShape((Grid)element));
                }
                else if (t == typeof(System.Windows.Controls.DockPanel))
                {
                    list.AddRange(AnalyseShape((DockPanel)element));
                }
                else if (t == typeof(System.Windows.Controls.StackPanel))
                {
                    list.AddRange(AnalyseShape((StackPanel)element));
                }
                else if (t == typeof(System.Windows.Controls.ContentPresenter))
                {
                    list.AddRange(AnalyseShape((ContentPresenter)element));
                }
                else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                {
                    list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                }
                else
                {

                }

            }
            catch
            {

            }
            // You can write more code to support more type here
            //... ...

            return list;
        }

        public List<Shape> AnalyseShape(UniformGrid uniformGrid)
        {
            List<Shape> list = new List<Shape>();

            UniformGrid can = (UniformGrid)uniformGrid;

            //Container.Children;
            for (int k = 0; k < can.Children.Count; k++)
            {
                try
                {
                    UIElement element = (UIElement)uniformGrid.Children[k];

                    Type t = element.GetType();

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        list.Add(element as Shape);
                    }
                    else if (t == typeof(System.Windows.Controls.ItemsControl))
                    {
                        list.AddRange(AnalyseShape());
                    }
                    else if (t == typeof(System.Windows.Controls.Canvas))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Border))
                    {
                        list.AddRange(AnalyseShape((Canvas)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Grid))
                    {
                        list.AddRange(AnalyseShape((Grid)element));
                    }
                    else if (t == typeof(System.Windows.Controls.DockPanel))
                    {
                        list.AddRange(AnalyseShape((DockPanel)element));
                    }
                    else if (t == typeof(TextBlock))
                    {
                        TextList.Add(element as TextBlock);
                    }
                    else if (t == typeof(System.Windows.Controls.StackPanel))
                    {
                        list.AddRange(AnalyseShape((StackPanel)element));
                    }
                    else if (t == typeof(System.Windows.Controls.ContentPresenter))
                    {
                        list.AddRange(AnalyseShape((ContentPresenter)element));
                    }
                    else if (t == typeof(System.Windows.Controls.Primitives.UniformGrid))
                    {
                        list.AddRange(AnalyseShape((System.Windows.Controls.Primitives.UniformGrid)element));
                    }
                    else
                    {
                        continue;
                    }
                }
                catch
                {

                }
            }

            return list;
        }

        public void Draw()
        {
            //Convert WPF shape to GDI+ shape and Draw GDI+
            for (int i = 0; i < ShapeList.Count; i++)
            {
                try
                {
                    Shape element = ShapeList[i];
                    Type t = element.GetType();
                    Type tParent1 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element)).GetType();
                    Type tParent2 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element))).GetType();

                    if (element.Visibility == Visibility.Hidden || element.Visibility == Visibility.Collapsed)
                        continue;

                    float x = 0;
                    float y = 0;
                    float w = 0;
                    float h = 0;

                    if (tParent1 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y);
                    }
                    else if (tParent1 == typeof(Canvas) && tParent2 != typeof(Canvas))
                    {
                        float x1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        float y1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y);
                        float x2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        float y2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).Y);

                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y);
                    }
                    else
                    {
                        float x1 = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X);
                        float x2 = Convert.ToSingle(VisualTreeHelper.GetOffset((UIElement)element.Parent).X);
                        float x3 = Convert.ToSingle(VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y);

                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).X);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y);
                    }

                    w = Convert.ToSingle(element.ActualWidth);
                    h = Convert.ToSingle(element.ActualHeight);

                    System.Drawing.SolidBrush GDIStroke = ConvertSolidColorBrush(element.Stroke as SolidColorBrush);

                    System.Drawing.SolidBrush GDIFill = ConvertSolidColorBrush(element.Fill as SolidColorBrush);

                    float Thickness = Convert.ToSingle(element.StrokeThickness);

                    System.Drawing.Pen pen = new System.Drawing.Pen(GDIStroke, Thickness);

                    if (t == typeof(System.Windows.Shapes.Ellipse))
                    {
                        DrawEllipse(x, y, w, h, pen, GDIFill);
                    }
                    else if (t == typeof(System.Windows.Shapes.Rectangle))
                    {
                        System.Windows.Shapes.Rectangle myRectangle = (System.Windows.Shapes.Rectangle)element;

                        DrawRectangle(x, y, w, h, pen, GDIFill);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polyline))
                    {
                        Polyline myPolyline = (System.Windows.Shapes.Polyline)element;
                        List<PointF> pts = new List<PointF>();

                        foreach (System.Windows.Point point in myPolyline.Points)
                        {
                            if (x + (float)point.X < x || y + (float)point.Y < y || (float)point.X > ((Canvas)VisualTreeHelper.GetParent(element)).ActualWidth)
                                continue;

                            PointF pt = new PointF((float)point.X + x, (float)point.Y + y);
                            pts.Add(pt);
                        }

                        //Check if points are ordered
                        var ptsA = pts.OrderByDescending(f => f.X).ToList();
                        var ptsB = pts.OrderByDescending(f => f.Y).ToList();

                        if (ptsA.SequenceEqual(pts))
                            pts = ptsA;
                        else if (ptsB.SequenceEqual(pts))
                            pts = ptsB;
                        else
                            pts = ptsA;

                        System.Drawing.PointF[] points = pts.ToArray();

                        DrawPolyline(pen, points);
                    }
                    else if (t == typeof(System.Windows.Shapes.Line))
                    {
                        Line myLine = (System.Windows.Shapes.Line)element;

                        PointF pt1 = new PointF(Convert.ToSingle(myLine.X1 + x), Convert.ToSingle(myLine.Y1 + y));
                        PointF pt2 = new PointF(Convert.ToSingle(myLine.X2 + x), Convert.ToSingle(myLine.Y2 + y));

                        DrawLine(pen, pt1, pt2);
                    }
                    else if (t == typeof(System.Windows.Shapes.Polygon))
                    {
                        Polygon myPolygon = (System.Windows.Shapes.Polygon)element;
                        List<PointF> pts = new List<PointF>();

                        var polygonGeometryTransform = myPolygon.RenderedGeometry.Transform;
                        var polygonToGridTransform = myPolygon.TransformToAncestor((Canvas)myPolygon.Parent);

                        foreach (System.Windows.Point point in myPolygon.Points)
                        {
                            var transformedPoint = polygonToGridTransform.Transform(
                           polygonGeometryTransform.Transform(point));

                            PointF pt = new PointF((float)transformedPoint.X + x, (float)transformedPoint.Y + y);
                            pts.Add(pt);
                        }

                        System.Drawing.PointF[] points = pts.ToArray();

                        DrawPolygon(pen, points, GDIFill);
                    }
                }
                catch
                {
                    continue;
                }
            }

            for (int i = 0; i < TextList.Count; i++)
            {
                try
                {
                    TextBlock element = TextList[i];
                    Type tParent1 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element)).GetType();
                    Type tParent2 = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(element))).GetType();

                    if (element.Foreground.Equals(System.Windows.Media.Brushes.Transparent))
                        continue;

                    float x = 0;
                    float y = 0;
                    float w = 0;
                    float h = 0;

                    if (tParent1 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + element.RenderTransform.Value.OffsetX - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + element.RenderTransform.Value.OffsetY - element.DesiredSize.Height / 4);
                    }
                    else if (tParent1 == typeof(Canvas) && tParent2 != typeof(Canvas))
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y - element.DesiredSize.Height / 4);
                    }
                    else
                    {
                        x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X + VisualTreeHelper.GetOffset((UIElement)element.Parent).X + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).X - element.DesiredSize.Width / 4);
                        y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y + VisualTreeHelper.GetOffset((UIElement)element.Parent).Y + VisualTreeHelper.GetOffset((Canvas)((Canvas)element.Parent).Parent).Y - element.DesiredSize.Height / 4);
                    }

                    //x = Convert.ToSingle(VisualTreeHelper.GetOffset(element).X - element.RenderTransform.Value.OffsetX);

                    //y = Convert.ToSingle(VisualTreeHelper.GetOffset(element).Y - element.RenderTransform.Value.OffsetY);

                    Font font = new Font(new FontConverter.FontNameConverter().ConvertToString(element.FontFamily),
                                         Convert.ToSingle((double)new FontSizeConverter().ConvertFrom(element.FontSize - 2)),
                                         System.Drawing.FontStyle.Regular);

                    System.Drawing.SolidBrush GDIStroke = ConvertSolidColorBrush(element.Foreground as SolidColorBrush);

                    try
                    {
                        TransformGroup rotate = new TransformGroup();
                        rotate = (TransformGroup)element.RenderTransform;
                        var radians = Math.Atan2(rotate.Value.M21, rotate.Value.M11);
                        float degree = Convert.ToSingle(radians * 180 / Math.PI);
                        DrawText(element.Text, font, GDIStroke, new PointF(x, y), degree);

                    }
                    catch
                    {
                        MatrixTransform rotate = new MatrixTransform();
                        rotate = (MatrixTransform)element.RenderTransform;
                        var radians = Math.Atan2(rotate.Value.M21, rotate.Value.M11);
                        float degree = Convert.ToSingle(radians * 180 / Math.PI);
                        DrawText(element.Text, font, GDIStroke, new PointF(x, y), degree);
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        public void DrawEllipse(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawEllipse(pen, x, y, w, h);
            DrawGraphics.FillEllipse(GDIFill, x, y, w, h);
        }
        public void DrawRectangle(float x, float y, float w, float h, System.Drawing.Pen pen, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawRectangle(pen, x, y, w, h);
            DrawGraphics.FillRectangle(GDIFill, x, y, w, h);
        }

        public void DrawPolygon(System.Drawing.Pen pen, System.Drawing.PointF[] points, System.Drawing.SolidBrush GDIFill)
        {
            DrawGraphics.DrawPolygon(pen, points);
            DrawGraphics.FillPolygon(GDIFill, points);
        }

        public void DrawLine(System.Drawing.Pen pen, System.Drawing.PointF pt1, System.Drawing.PointF pt2)
        {
            DrawGraphics.DrawLine(pen, pt1, pt2);
        }

        public void DrawPolyline(System.Drawing.Pen pen, System.Drawing.PointF[] points)
        {
            DrawGraphics.DrawLines(pen, points);
        }

        public void DrawText(string text, Font font, System.Drawing.SolidBrush GDIStroke, System.Drawing.PointF point, float rotationAngle = 0)
        {
            if (rotationAngle == 0)
                DrawGraphics.DrawString(text, font, GDIStroke, point);
            else
            {
                if (bm != null)
                {
                    //Translating the graphics object to the spot of the object
                    //where the object will be rotated
                    DrawGraphics.TranslateTransform(point.X, point.Y);

                    //Rotating by the object angle
                    DrawGraphics.RotateTransform(-rotationAngle);

                    //Translating back to the original position
                    DrawGraphics.TranslateTransform(-point.X, -point.Y);

                    //Drawing the text to the object
                    DrawGraphics.DrawString(text, font, GDIStroke, point);

                    //Resetting the transformation
                    DrawGraphics.ResetTransform();

                }
                else
                {
                    //Drawing the text to the object
                    DrawGraphics.DrawString(text, font, GDIStroke, point);
                }
            }
        }

        public System.Drawing.SolidBrush ConvertSolidColorBrush(SolidColorBrush scb)
        {
            if (scb != null)
                return new SolidBrush(ConvertColor(scb.Color));

            else
                return new SolidBrush(ConvertColor(Colors.Transparent));
        }

        public System.Drawing.Color ConvertColor(System.Windows.Media.Color WPFColor)
        {
            return System.Drawing.Color.FromArgb(WPFColor.A, WPFColor.R, WPFColor.G, WPFColor.B);
        }
    }
}
