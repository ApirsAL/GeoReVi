using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;
using System.Windows.Controls;
using System;
using System.Windows.Shapes;
using System.Linq;

namespace GeoReVi
{
    public class BoxPlotChartStyle : BarChartStyle
    {
        #region private members
        //Whisker
        private Line whisker = new Line();

        //Chart styles
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 10;
        private Line gridline = new Line();

        //The bar polygon collection
        private BindableCollection<BoxWhiskerPlot> boxWhiskerCollection = new BindableCollection<BoxWhiskerPlot>();

        #endregion

        #region Public properties

        //A list of bar polygons
        public BindableCollection<BoxWhiskerPlot> BoxWhiskerCollection
        {
            get
            {
                return this.boxWhiskerCollection;
            }
            set
            {
                this.boxWhiskerCollection = value;
            }
        }

        #endregion

        #region Constructor

        #endregion

        #region Methods

        //Setting the Boxplots of the chart
        public void SetBoxPlots(BindableCollection<BoxPlotSeries> BoxPlotCollection)
        {
            int nSeries = BoxPlotCollection.Count;
            int j = 0;

            BoxWhiskerCollection.Clear();

            switch (BarType)
            {
                //Setting the lithological layers
                case BarTypeEnum.HorizontalBoxPlot:

                    foreach (var boxPlotSeries in BoxPlotCollection)
                    {
                        try
                        {
                            SetHorizontalBoxPlot(j+1, boxPlotSeries);
                            j++;
                        }
                        catch
                        {
                            return;
                        }
                    }
                    break;

            }
        }

        /// <summary>
        /// Setting a square in the control
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="ds"></param>
        private void SetHorizontalBoxPlot(double y, BoxPlotSeries bs)
        {
            BoxWhiskerPlot bwp = new BoxWhiskerPlot();

            if (bs.BoxPlotStatisticsCollection.Count() == 0)
                return;

            BoxPlotStatistics bst = bs.BoxPlotStatisticsCollection[0];

            //layouting the polygon
            bwp.FillColor = bs.FillColor;
            bwp.BorderColor = bs.BorderColor;
            bwp.BorderThickness = bs.BorderThickness;
            bwp.Name = bst.Name;

            double width = bs.BarWidth;

            //Defining the central bottom point of the Boxplot
            y -= 0.5 * YTick;

            //Defining the edges of the bar
            bwp.PolygonPoints.Add(NormalizePoint(new Point(bst.LowerQuartile, y - width / 2)));
            bwp.PolygonPoints.Add(NormalizePoint(new Point(bst.LowerQuartile, y + width / 2)));
            bwp.PolygonPoints.Add(NormalizePoint(new Point(bst.UpperQuartile, y + width / 2)));
            bwp.PolygonPoints.Add(NormalizePoint(new Point(bst.UpperQuartile, y - width / 2)));

            //Adding the whiskers
            LineSeries pl = new LineSeries();

            //Lower whisker
            pl.Symbols.SymbolType = SymbolTypeEnum.None;
            pl.LinePattern = LinePatternEnum.Solid;
            pl.LineThickness = bs.BorderThickness;
            pl.LineColor = bs.BorderColor;
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Min, y)));
            pl.LinePoints.Add(NormalizePoint(new Point(bst.LowerQuartile, y)));
            bwp.WhiskerLines.Add(pl);

            pl = new LineSeries();
            pl.Symbols.SymbolType = SymbolTypeEnum.None;
            pl.LinePattern = LinePatternEnum.Solid;
            pl.LineThickness = bs.BorderThickness;
            pl.LineColor = bs.BorderColor;
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Min, y - width / 4)));
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Min, y + width / 4)));
            bwp.WhiskerLines.Add(pl);

            //Middle Whisker
            pl = new LineSeries();
            pl.Symbols.SymbolType = SymbolTypeEnum.None;
            pl.LinePattern = LinePatternEnum.Solid;
            pl.LineThickness = bs.BorderThickness;
            pl.LineColor = bs.BorderColor;
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Mean, y - width/2)));
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Mean, y + width/2)));
            bwp.WhiskerLines.Add(pl);

            //Upper Whisker
            pl = new LineSeries();
            pl.Symbols.SymbolType = SymbolTypeEnum.None;
            pl.LinePattern = LinePatternEnum.Solid;
            pl.LineThickness = bs.BorderThickness;
            pl.LineColor = bs.BorderColor;
            pl.LinePoints.Add(NormalizePoint(new Point(bst.UpperQuartile, y)));
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Max, y)));
            bwp.WhiskerLines.Add(pl);

            pl = new LineSeries();
            pl.Symbols.SymbolType = SymbolTypeEnum.None;
            pl.LinePattern = LinePatternEnum.Solid;
            pl.LineThickness = bs.BorderThickness;
            pl.LineColor = bs.BorderColor;
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Max, y - width / 4)));
            pl.LinePoints.Add(NormalizePoint(new Point(bst.Max, y + width / 4)));
            bwp.WhiskerLines.Add(pl);

            //Adding outliers
            LineSeries plOut = new LineSeries();
            plOut.Symbols.SymbolType = SymbolTypeEnum.Circle;
            plOut.Symbols.SymbolSize = 6;
            plOut.Symbols.FillColor = Brushes.Black;
            plOut.Symbols.BorderThickness = 2;
            plOut.Symbols.BorderColor = Brushes.Black;
            plOut.LinePattern = LinePatternEnum.None;
            plOut.LineThickness = 0;

            foreach (var pt in bst.Outliers)
            {
                plOut.LinePoints.Add(NormalizePoint(new Point(pt, y)));
            }
            bwp.Outliers = plOut;

            //Adding the object to the box whisker collection
            BoxWhiskerCollection.Add(bwp);
        }

        //Adding style to the bars control
        public void SetBoxPlotControl(BindableCollection<BoxWhiskerPlot> BarSeries = null)
        {

            // determine right offset:
            TextBlock tb1 = new TextBlock();
            tb1.Text = Xmax.ToString();
            tb1.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            Size size = tb1.DesiredSize;

            double offset = 20;
            double rightOffset = size.Width / 2 + 15;

            // Determine left offset through iterating through all y axis labels: 
            for (double dy = Ymin; dy <= Ymax; dy += YTick)
            {
                Point pt = NormalizePoint(new Point(Xmin, dy));
                tb1.Text = dy.ToString();
                tb1.TextAlignment = TextAlignment.Right;
                tb1.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb1.DesiredSize;

                if (offset < size.Width)
                    offset = size.Width;
            }

            double leftOffset = offset + 15;

            foreach (var bar in BoxWhiskerCollection)
            {
                Polygon rect = new Polygon();
                rect.Stroke = bar.BorderColor;
                rect.Fill = bar.FillColor;
                rect.StrokeThickness = bar.BorderThickness;
                rect.Points = bar.PolygonPoints;

                ChartCanvas.Children.Add(rect);

                Polyline pl = new Polyline();

                TextBlock tb = new TextBlock();

                try
                {
                    tb.Text = bar.Name.ToString();
                }
                catch
                {
                    tb.Text = "";
                }

                foreach (var whiskerLine in bar.WhiskerLines)
                {
                    whisker = new Line();
                    whisker.StrokeThickness = bar.BorderThickness;
                    whisker.Stroke = bar.BorderColor;

                    whisker.X1 = whiskerLine.LinePoints[0].X;
                    whisker.Y1 = whiskerLine.LinePoints[0].Y;

                    whisker.X2 = whiskerLine.LinePoints[1].X;
                    whisker.Y2 = whiskerLine.LinePoints[1].Y;

                    ChartCanvas.Children.Add(whisker);
                }

                foreach(var lin in bar.Outliers.LinePoints)
                {
                    bar.Outliers.Symbols.AddSymbol(ChartCanvas, lin);
                }

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                ChartCanvas.Children.Add(tb);

                Canvas.SetLeft(tb, 4);
                Canvas.SetTop(tb, Math.Abs((bar.PolygonPoints.Min(y => y.Y)))-tb.DesiredSize.Height);


            }
        }

        #endregion

        #region Helper

        /// <summary>
        /// Adding a line pattern to the 
        /// </summary>
        private void AddLinePattern()
        {
            whisker.Stroke = GridlineColor;

            if (BoxWhiskerCollection.Count > 0)
                whisker.StrokeThickness = BoxWhiskerCollection.FirstOrDefault().BorderThickness;
            else whisker.StrokeThickness = 1;
        }

        #endregion
    }
}
