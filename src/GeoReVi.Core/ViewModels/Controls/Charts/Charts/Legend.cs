using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GeoReVi
{
    public class Legend
    {
        #region Private members

        private bool isLegend;
        private bool isBorder;
        private Canvas legendCanvas;
        private LegendPositionEnum legendPosition;

        #endregion

        #region Public properties

        public Legend() { isLegend = false; isBorder = true; legendPosition = LegendPositionEnum.NorthEast; }
        public LegendPositionEnum LegendPosition { get { return legendPosition; } set { legendPosition = value; } }
        public bool IsLegend { get { return isLegend; } set { isLegend = value; } }
        public bool IsBorder { get { return isBorder; } set { isBorder = value; } }

        #endregion

        #region Helpers

        //Adding a layout for line based on a LineSeries instance
        private void AddLinePattern(Line line, LineSeries ds)
        {
            line.Stroke = ds.LineColor;
            line.StrokeThickness = ds.LineThickness;
            switch (ds.LinePattern)
            {
                case LinePatternEnum.Dash:
                    line.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case LinePatternEnum.Dot:
                    line.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case LinePatternEnum.DashDot:
                    line.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        //Adding a layout for rectangles based on a BarSeries instance
        private void AddRectanglePattern(Rectangle rc, BarSeries bs)
        {
            rc.Fill = bs.FillColor;
            rc.StrokeThickness = bs.BorderThickness * 0.2;
            rc.Stroke = bs.BorderColor;
        }

        //Adding a layout for rectangles based on a BarSeries instance
        private void AddRectanglePattern(Rectangle rc, string name)
        {
            rc.Fill = StringToFaciesTileConverter.ConvertLegend(name);
            rc.StrokeThickness = 1;
            rc.Stroke = Brushes.Black;
        }

        //Adding a layout for rectangles based on a BarSeries instance
        private void AddRectanglePattern(Rectangle rc, BoxPlotSeries bs)
        {
            rc.Fill = bs.FillColor;
            rc.StrokeThickness = bs.BorderThickness * 0.2;
            rc.Stroke = bs.BorderColor;
        }

        private void AddTickLine(Canvas cs, Point pt1, Point pt2)
        {
            Line line = new Line();
            line.X1 = pt1.X;
            line.Y1 = pt1.Y;
            line.X2 = pt2.X;
            line.Y2 = pt2.Y;
            line.Stroke = Brushes.Black;
            cs.Children.Add(line);
        }

        #endregion
    }

    //Position where the Legend will be displayed
    public enum LegendPositionEnum
    {
        North,
        NorthWest,
        West,
        SouthWest,
        South,
        SouthEast,
        East,
        NorthEast
    }
}