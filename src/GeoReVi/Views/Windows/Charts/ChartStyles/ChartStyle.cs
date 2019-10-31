using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GeoReVi
{
    public class ChartStyle : ChartStyleBase
    {
        #region Private members
        private string metaInformation = "Coordinates";

        private bool isGrid = true;
        private bool isXGrid = true;
        private bool isXLog = false;
        private bool isYGrid = true;
        private bool isYLog = false;
        private bool isY2Log = false;
        private bool isZGrid = true;
        private bool isZLog = false;
        private bool showConvexHull = false;
        private Brush gridlineColor = Brushes.LightGray;
        private double elevation = 0.5;
        private double azimuth = 0.5;
        private LinePatternEnum gridlinePattern;
        private double gridlineThickness = 1;
        private double leftOffset = 20;
        private double bottomOffset = 15;
        private double rightOffset = 0;
        private double topOffset = 10;
        private Line gridline = new Line();
        private bool shallRender = true;
        private bool isBubbleChart = false;
        #endregion

        #region Public properties

        public Canvas TextCanvas { get; set; }

        private string title = "Title";
        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        private FontFamily titleFont = new FontFamily("Arial Narrow");
        public FontFamily TitleFont
        {
            get { return titleFont; }
            set { titleFont = value; }
        }

        private Brush titleColor = Brushes.Black;
        public Brush TitleColor
        {
            get { return titleColor; }
            set { titleColor = value; }
        }

        private double titleFontSize = 11;
        public double TitleFontSize
        {
            get { return titleFontSize; }
            set { titleFontSize = value; }
        }

        public string MetaInformation
        {
            get => metaInformation;
            set
            {
                metaInformation = value;
            }
        }

        private double labelFontSize = (double)new FontSizeConverter().ConvertFrom("10pt");
        public double LabelFontSize
        {
            get => this.labelFontSize;
            set
            {
                this.labelFontSize = value;
            }
        }

        private Brush labelColor = Brushes.Black;
        public Brush LabelColor
        {
            get => labelColor;
            set
            {
                this.labelColor = value;
            }
        }

        private FontFamily labelFont = new FontFamily("Arial Narrow");
        public FontFamily LabelFont
        {
            get => labelFont;
            set
            {
                this.labelFont = value;
            }
        }

        private string xLabel = "X";
        public string XLabel
        {
            get => xLabel;
            set
            {
                xLabel = value;
            }
        }

        private string[] xLabels = new string[] { };
        public string[] XLabels
        {
            get
            {
                return xLabels;
            }
            set
            {
                xLabels = value;
            }
        }

        private string yLabel = "Y";
        public string YLabel
        {
            get
            {
                return yLabel;
            }
            set
            {
                yLabel = value;
            }
        }

        private string[] yLabels = new string[] { };
        public string[] YLabels
        {
            get
            {
                return yLabels;
            }
            set
            {
                yLabels = value;
            }
        }

        private string zLabel = "Z";
        public string ZLabel
        {
            get => zLabel;
            set
            {
                zLabel = value;
            }
        }

        public LinePatternEnum GridlinePattern { get { return gridlinePattern; } set { gridlinePattern = value; } }

        private double xTick = 1;
        public double XTick
        {
            get => xTick;
            set
            {
                xTick = value;
            }
        }

        private double yTick = 0.5;
        public double YTick
        {
            get => yTick;
            set
            {
                yTick = value;
            }
        }

        private double y2Tick = 0.5;
        public double Y2Tick
        {
            get => y2Tick;
            set
            {
                y2Tick = value;
            }
        }

        private double zTick = 0.5;
        public double ZTick
        {
            get
            {
                return zTick;
            }
            set
            {
                zTick = value;
            }
        }

        public double Elevation { get { return elevation; } set { elevation = value; } }
        public double Azimuth { get { return azimuth; } set { azimuth = value; } }
        public double GridlineThickness { get { return gridlineThickness; } set { gridlineThickness = value; } }
        public Brush GridlineColor { get { return gridlineColor; } set { gridlineColor = value; } }

        private FontFamily tickFont = new FontFamily("Arial Narrow");
        public FontFamily TickFont
        {
            get { return tickFont; }
            set { tickFont = value; }
        }

        private Brush tickColor = Brushes.Black;
        public Brush TickColor
        {
            get { return tickColor; }
            set { tickColor = value; }
        }

        private double tickFontSize = 10;
        public double TickFontSize
        {
            get { return tickFontSize; }
            set { tickFontSize = value; }
        }

        public bool IsXGrid { get { return isXGrid; } set { isXGrid = value; } }
        public bool IsXLog { get { return isXLog; } set { isXLog = value; } }
        public bool IsYGrid { get { return isYGrid; } set { isYGrid = value; } }
        public bool IsYLog { get { return isYLog; } set { isYLog = value; } }
        public bool IsY2Log { get { return isY2Log; } set { isY2Log = value; } }
        public bool IsZGrid { get { return isZGrid; } set { isZGrid = value; } }
        public bool IsZLog { get { return isZLog; } set { isZLog = value; } }
        public bool IsGrid { get { return isGrid; } set { isGrid = value; } }
        public bool ShallRender { get { return shallRender; } set { shallRender = value; } }
        public bool IsBubbleChart { get { return isBubbleChart; } set { isBubbleChart = value; } }
        public bool ShowConvexHull { get { return showConvexHull; } set { showConvexHull = value; } }

        #region 3D Properties

        private Brush axisColor = Brushes.Black;
        public Brush AxisColor
        {
            get { return axisColor; }
            set { axisColor = value; }
        }

        private LinePatternEnum axisPattern = LinePatternEnum.Solid;
        public LinePatternEnum AxisPattern
        {
            get { return axisPattern; }
            set { axisPattern = value; }
        }

        private double axisThickness = 1;
        public double AxisThickness
        {
            get { return axisThickness; }
            set { axisThickness = value; }
        }
        #endregion

        #endregion

        #region Public methods

        /// <summary>
        /// Adds a chart style to the chart object
        /// </summary>
        /// <param name="tbTitle"></param>
        /// <param name="tbXLabel"></param>
        /// <param name="tbYLabel"></param>
        public void AddChartStyle()
        {
            //point, line, offset, dx, dy and textblock variables
            Size size = new Size();
            Point pt = new Point();
            Line tick = new Line();
            TextBlock tb = new TextBlock();

            double offset = 0, legendOffset = 0, rightOffset = 0;
            double dx, dy;

            if (IsBubbleChart)
                legendOffset = 70;


            #region Title, X Label and Y Label

            TextBlock tbYLabel = new TextBlock();
            tbYLabel.Text = yLabel;

            tbYLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tbYLabel.LayoutTransform = new RotateTransform(270);

            size = tbYLabel.DesiredSize;

            if (IsXLog && Xmin == 0)
                Xmin = 0.1;

            if (IsYLog && Ymin == 0)
                Ymin = 0.1;

            XTick = (XTick <= 0) ? 0.5 : XTick;
            YTick = (YTick <= 0) ? 0.5 : YTick;
            Xmax = (Xmax == Xmin) ? Xmax + 1 : Xmax;
            Ymax = (Ymax == Ymin) ? Ymax + 1 : Ymax;

            if (IsYLog && Xmax <= Xmin)
                return;

           if (IsYLog && Ymin <= 0)
                return; 

            if ((Ymax - Ymin) / YTick > 1E+2)
                YTick = (Ymax - Ymin) / 10;

            if ((Xmax - Xmin) / XTick > 1E+2)
                XTick = (Xmax - Xmin) / 10;

            int i = 0;

            // Determine left offset: 

            for (dy = Ymin; dy <= Ymax; dy += IsYLog ? 9*dy : (YTick == 0) ? 1 : YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));

                tb = new TextBlock();

                if (YLabels.Length == 0)
                    tb.Text = Math.Round(dy, 2).ToString();
                else
                    if(dy != Ymin && dy != Ymax)
                        tb.Text = YLabels[XLabels.Length - i].ToString();

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                size = tb.DesiredSize;

                if (offset < size.Width)
                    offset = size.Width +15;

                i++;
            }

            // Determine right offset: 
            if (Y2max != 10 || Y2min != 0)
            for (dy = Y2min; dy <= Y2max; dy += (Y2Tick == 0) ? 1 : Y2Tick)
            {
                pt = NormalizePoint(new Point(Xmax, dy));

                tb = new TextBlock();

                tb.Text = Math.Round(dy, 2).ToString();

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

                if (rightOffset < size.Width)
                    rightOffset = size.Width + size.Width;
            }


            TextBlock tbTitle = new TextBlock();
            tbTitle.Text = Title;

            tbTitle.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbTitle.DesiredSize;

            topOffset = size.Height + 3;

            TextCanvas.Children.Add(tbTitle);

            TextBlock tbXLabel = new TextBlock();
            tbXLabel.Text = XLabel;

            tbXLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbXLabel.DesiredSize;
            TextCanvas.Children.Add(tbXLabel);

            bottomOffset = 15 + tbXLabel.DesiredSize.Height;

            if(XLabels.Length != 0)
            {
                string a = XLabels.OrderByDescending(x => x.Length).First();

                TextBox tbx = new TextBox();

                tb.Text = a;

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                tb.LayoutTransform = new RotateTransform(270);
                size = tb.DesiredSize;

                if (bottomOffset < size.Width + 15)
                    bottomOffset = size.Width + 15;
            }

            TextCanvas.Children.Add(tbYLabel);

            leftOffset = tbYLabel.DesiredSize.Height + offset + 5;

            #endregion


            // determine right offset: 
            tb.Text = Xmax.ToString();

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tb.DesiredSize;
            rightOffset = rightOffset + size.Width / 2 + 2;

            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);

            if (IsBubbleChart)
                ChartCanvas.Width = Math.Abs(TextCanvas.Width - leftOffset - rightOffset - legendOffset);
            else
                ChartCanvas.Width = Math.Abs(TextCanvas.Width - leftOffset - rightOffset);

            ChartCanvas.Height = Math.Abs(TextCanvas.Height - bottomOffset - topOffset - size.Height / 2);

            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            Canvas.SetLeft(tbTitle, leftOffset - (tbTitle.DesiredSize.Width / 2) + (ChartCanvas.Width / 2));
            Canvas.SetTop(tbTitle, 2);

            Canvas.SetLeft(tbYLabel, 2);
            Canvas.SetTop(tbYLabel, bottomOffset - tbYLabel.DesiredSize.Width / 2 + ChartCanvas.Height / 2);

            Canvas.SetLeft(tbXLabel, leftOffset - tbXLabel.DesiredSize.Width / 2 + ChartCanvas.Width / 2);
            Canvas.SetTop(tbXLabel, TextCanvas.Height - size.Height + 2);

            // Create vertical gridlines: 
            if (IsYGrid == true)
            {
                if (!IsXLog)
                    for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                    {
                        gridline = new Line();
                        AddLinePattern();
                        gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                        gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                        gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                        gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                        ChartCanvas.Children.Add(gridline);
                    }
                else
                {
                    double a = Xmin;
                    for (dx = Xmin; dx <= Xmax; dx += a)
                    {
                        if (a == 0)
                            break;

                        if (Math.Round(dx / a, 0) == 10)
                            a = dx;

                        gridline = new Line();
                        AddLinePattern();
                        gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                        gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                        gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                        gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                        ChartCanvas.Children.Add(gridline);
                    }

                }
            }

            // Create horizontal gridlines: 
            if (IsXGrid == true)
            {
                if (!IsYLog)
                    for (dy = Ymin + YTick; dy < Ymax; dy += YTick)
                    {
                        gridline = new Line();
                        AddLinePattern();
                        gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                        gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                        gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                        gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                        ChartCanvas.Children.Add(gridline);
                    }
                else
                {
                    double a = Ymin == 0 ? 0.1 : Ymin;
                    for (dy = Ymin; dy <= Ymax; dy += a)
                    {
                        if (a == 0)
                            break;

                        if (Math.Round(dy / a, 0) == 10)
                            a = dy;

                        gridline = new Line();
                        AddLinePattern();
                        gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;
                        gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;
                        gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;
                        gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;
                        ChartCanvas.Children.Add(gridline);
                    }
                }
            }


            i = 0;

            // Create x-axis tick marks: 
            if (!IsXLog)
                for (dx = Xmin; dx <= Xmax; dx += xTick)
                {
                    if (xTick == 0)
                        xTick += 1;

                    pt = NormalizePoint(new Point(dx, Ymin));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X;
                    tick.Y2 = pt.Y - 5;
                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();

                    if (XLabels.Length < 1)
                        tb.Text = Math.Round(dx, 2).ToString();
                    else
                        if(dx != Xmax && dx != Xmin)
                            tb.Text = XLabels[i-1];

                    if (tb.Text == "0" && dx != Xmin)
                        tb.Text = dy.ToString("E0");

                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                    if (XLabels.Length > 0)
                        tb.LayoutTransform = new RotateTransform(270);

                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);

                    Canvas.SetLeft(tb, leftOffset + pt.X - (XLabels.Length < 1 ? size.Width : size.Height) / 2);
                    Canvas.SetTop(tb, pt.Y + topOffset + 2 + size.Height / 2);

                    i++;
                }
            else
            {
                double a = Xmin == 0 ? 0.1 : Xmin;
                for (dx = Xmin; dx <= Xmax; dx += 9 * dx)
                {
                    if (a == 0)
                        break;

                    pt = NormalizePoint(new Point(dx, Ymin));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X;
                    tick.Y2 = pt.Y - 5;
                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();

                    if (XLabels.Length < 1)
                        tb.Text = Math.Round(dx, 2).ToString();
                    else
                        if (dx != Xmin && dx != Xmax)
                            tb.Text = XLabels[i - 1];

                    if (tb.Text == "0" && dx != Xmin)
                        tb.Text = dx.ToString("E0");

                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                    if(XLabels.Length > 0)
                        tb.LayoutTransform = new RotateTransform(270);

                    size = tb.DesiredSize;
                    TextCanvas.Children.Add(tb);

                    Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                    Canvas.SetTop(tb, pt.Y + topOffset + 2 + size.Height / 2);

                    i++;
                }
            }

            i = 0;

            // Create y-axis tick marks: 
            if (!IsYLog)
            {
                for (dy = Ymin; dy <= Ymax; dy += YTick)
                {
                    if (YTick == 0)
                        YTick += 1;

                    //pt = NormalizePoint(new Point(Xmin, dy));
                    pt = NormalizePoint(new Point(Xmin, dy));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X + 5;
                    tick.Y2 = pt.Y;

                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();

                    if (YLabels.Length == 0)
                        tb.Text = Math.Round(dy, 2).ToString();
                    else
                        if (dy != Ymin && dy != Ymax)
                            tb.Text = YLabels[YLabels.Length - i].ToString();

                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;

                    TextCanvas.Children.Add(tb);

                    if (IsBubbleChart)
                        Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + 2 + legendOffset);
                    else
                        Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + 2);

                    Canvas.SetTop(tb, pt.Y + topOffset);

                    i++;

                }
            }
            else
            {
                double a = Ymin == 0 ? 0.1 : Ymin;
                for (dy = Ymin; dy <= Ymax; dy += 9 * dy)
                {
                    if (YTick == 0)
                        YTick += 1;

                    if (a == 0)
                        break;

                    //pt = NormalizePoint(new Point(Xmin, dy));
                    pt = NormalizePoint(new Point(Xmin, dy));
                    tick = new Line();
                    tick.Stroke = Brushes.Black;
                    tick.X1 = pt.X;
                    tick.Y1 = pt.Y;
                    tick.X2 = pt.X + 5;
                    tick.Y2 = pt.Y;

                    ChartCanvas.Children.Add(tick);

                    tb = new TextBlock();

                    if (YLabels.Length == 0)
                        tb.Text = Math.Round(dy, 2).ToString();
                    else
                        if (dy != Ymin && dy != Ymax)
                            tb.Text = YLabels[YLabels.Length - i].ToString();

                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;

                    TextCanvas.Children.Add(tb);

                    if (IsBubbleChart)
                        Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + 2 + legendOffset);
                    else
                        Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + 2);

                    Canvas.SetTop(tb, pt.Y + topOffset);

                }
            }

            if(Y2max != 10 || Y2min !=0)
            for (dy = Y2min; dy <= Y2max; dy += Y2Tick > 0 ? Y2Tick : Y2max - Y2min)
            {
                if (Y2Tick == 0)
                    Y2Tick += 1;

                //pt = NormalizePoint(new Point(Xmin, dy));
                pt = NormalizeSeconOrdinatePoint(new Point(Xmax, dy));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X - 5;
                tick.Y2 = pt.Y;

                ChartCanvas.Children.Add(tick);

                tb = new TextBlock();
                tb.Text = Math.Round(dy, 2).ToString();

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

                TextCanvas.Children.Add(tb);

                if (IsBubbleChart)
                    Canvas.SetLeft(tb, ChartCanvas.Width + rightOffset + 2 + legendOffset);
                else
                    Canvas.SetLeft(tb, ChartCanvas.Width + rightOffset + 2);

                Canvas.SetTop(tb, pt.Y + topOffset);

            }
        }

        /// <summary>
        /// Adds a chart style to the chart object
        /// </summary>
        /// <param name="tbTitle"></param>
        /// <param name="tbXLabel"></param>
        /// <param name="tbYLabel"></param>
        public void AddTernaryChartStyle()
        {
            //point, line, offset, dx, dy and textblock variables
            Point pt = new Point();
            Line tick = new Line();
            Size size = new Size();

            double offset = 0,
                legendOffset = 0;
            double dx, dy;

            TextBlock tb = new TextBlock();

            #region Title, X Label and Y Label

            TextBlock tbYLabel = new TextBlock();
            tbYLabel.Text = yLabel;

            tbYLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            size = tbYLabel.DesiredSize;

            // Determine left offset: 
            for (dy = Ymin; dy <= Ymax; dy += (YTick == 0) ? 1 : YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));

                tb = new TextBlock();

                tb.Text = dy.ToString();

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

                if (offset < size.Width)
                    offset = size.Width + size.Width;
            }

            TextBlock tbTitle = new TextBlock();
            tbTitle.Text = Title;
            tbTitle.FontWeight = FontWeights.Bold;

            tbTitle.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbTitle.DesiredSize;

            TextBlock tbZLabel = new TextBlock();
            tbZLabel.Text = ZLabel;

            tbZLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbTitle.DesiredSize;

            topOffset = size.Height + tbZLabel.DesiredSize.Height + 3;

            TextCanvas.Children.Add(tbTitle);
            TextCanvas.Children.Add(tbZLabel);

            TextBlock tbXLabel = new TextBlock();
            tbXLabel.Text = XLabel;

            tbXLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbXLabel.DesiredSize;
            TextCanvas.Children.Add(tbXLabel);

            bottomOffset = 15 + tbXLabel.DesiredSize.Height;

            TextCanvas.Children.Add(tbYLabel);

            leftOffset = tbYLabel.DesiredSize.Height / 2 + offset + 5;

            #endregion

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            size = tb.DesiredSize;
            rightOffset = size.Width / 2 + 2;

            ChartCanvas.Width = Math.Abs(TextCanvas.Width - leftOffset - rightOffset - topOffset - bottomOffset);
            ChartCanvas.Height = Math.Sqrt(3) / 2 * ChartCanvas.Width;

            Canvas.SetLeft(ChartCanvas, (TextCanvas.Width - ChartCanvas.Width) / 2);
            Canvas.SetBottom(ChartCanvas, bottomOffset + 5);

            Xmin = 0;
            Ymin = 0;
            Xmax = 1;
            Ymax = 1;

            Polygon chartPolygon = new Polygon();
            chartPolygon.Points = new PointCollection()
            {
                new Point(0, ChartCanvas.Height),
                new Point(ChartCanvas.Width, ChartCanvas.Height),
                new Point(ChartCanvas.Width/2,0),
                new Point(0, ChartCanvas.Height),
            };

            chartPolygon.Stroke = Brushes.Black;

            ChartCanvas.Children.Add(chartPolygon);

            Canvas.SetLeft(tbTitle, leftOffset + (ChartCanvas.Width / 2));
            Canvas.SetTop(tbTitle, 2);

            Canvas.SetLeft(tbZLabel, leftOffset + (ChartCanvas.Width / 2));
            Canvas.SetTop(tbZLabel, tbTitle.DesiredSize.Height + 4);

            Canvas.SetLeft(tbYLabel, 2);
            Canvas.SetTop(tbYLabel, TextCanvas.Height - bottomOffset);

            Canvas.SetLeft(tbXLabel, leftOffset + ChartCanvas.Width);
            Canvas.SetTop(tbXLabel, TextCanvas.Height - bottomOffset);

            // Create vertical gridlines: 
            if (IsGrid == true)
            {
                for (double i = 0; i < 1; i = i + 0.3333333)
                {
                    i = Math.Round(i, 5);

                    gridline = new Line();

                    AddLinePattern();

                    gridline.X1 = NormalizePoint(new Point(i / 2, i)).X;

                    gridline.Y1 = NormalizePoint(new Point(i / 2, i)).Y;

                    gridline.X2 = NormalizePoint(new Point(i, 0)).X;

                    gridline.Y2 = NormalizePoint(new Point(i, 0)).Y;

                    gridline.StrokeThickness = 0.5;

                    ChartCanvas.Children.Add(gridline);

                    gridline = new Line();

                    AddLinePattern();

                    gridline.X1 = NormalizePoint(new Point(i / 2, i)).X;

                    gridline.Y1 = NormalizePoint(new Point(i / 2, i)).Y;

                    gridline.X2 = NormalizePoint(new Point(1 - i / 2, i)).X;

                    gridline.Y2 = NormalizePoint(new Point(1 - i / 2, i)).Y;

                    ChartCanvas.Children.Add(gridline);

                    gridline = new Line();

                    AddLinePattern();

                    gridline.StrokeThickness = 0.5;

                    gridline.X1 = NormalizePoint(new Point(1 - i / 2, i)).X;

                    gridline.Y1 = NormalizePoint(new Point(1 - i / 2, i)).Y;

                    gridline.X2 = NormalizePoint(new Point(1 - i, 0)).X;

                    gridline.Y2 = NormalizePoint(new Point(1 - i, 0)).Y;

                    gridline.StrokeThickness = 0.5;

                    ChartCanvas.Children.Add(gridline);
                }
            }
        }

        /// <summary>
        /// Adds a chart style to the chart object
        /// </summary>
        /// <param name="tbTitle"></param>
        /// <param name="tbXLabel"></param>
        /// <param name="tbYLabel"></param>
        public void AddSectionChartStyle(int countLogs = 0)
        {
            //point, line, offset, dx, dy and textblock variables
            Size size = new Size();
            Point pt = new Point();
            Line tick = new Line();
            TextBlock tb = new TextBlock();

            double offset = 0, legendOffset = 0, rightOffset = 0;
            double dx, dy;

            #region Title, X Label and Y Label

            TextBlock tbYLabel = new TextBlock();
            tbYLabel.Text = yLabel;

            tbYLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            tbYLabel.LayoutTransform = new RotateTransform(270);

            size = tbYLabel.DesiredSize;

            if (IsXLog && Xmin == 0)
                Xmin = 0.1;

            if (IsYLog && Ymin == 0)
                Ymin = 0.1;

            XTick = (XTick <= 0) ? 0.5 : XTick;
            YTick = (YTick <= 0) ? 0.5 : YTick;
            Xmax = (Xmax == Xmin) ? Xmax + 1 : Xmax;
            Ymax = (Ymax == Ymin) ? Ymax + 1 : Ymax;

            if (IsYLog && Xmax <= Xmin)
                return;

            if (IsYLog && Ymin <= 0)
                return;

            if ((Ymax - Ymin) / YTick > 1E+2)
                YTick = (Ymax - Ymin) / 10;

            if ((Xmax - Xmin) / XTick > 1E+2)
                XTick = (Xmax - Xmin) / 10;

            int i = 0;

            // Determine left offset: 

            for (dy = Ymin; dy <= Ymax; dy += IsYLog ? 9 * dy : (YTick == 0) ? 1 : YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));

                tb = new TextBlock();

                if (YLabels.Length == 0)
                    tb.Text = Math.Round(dy, 2).ToString();
                else
                    if (dy != Ymin && dy != Ymax)
                    tb.Text = YLabels[XLabels.Length - i].ToString();

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                size = tb.DesiredSize;

                if (offset < size.Width)
                    offset = size.Width + 15;

                i++;
            }

            // Determine right offset: 
            if (Y2max != 10 || Y2min != 0)
                for (dy = Y2min; dy <= Y2max; dy += (Y2Tick == 0) ? 1 : Y2Tick)
                {
                    pt = NormalizePoint(new Point(Xmax, dy));

                    tb = new TextBlock();

                    tb.Text = Math.Round(dy, 2).ToString();

                    if (tb.Text == "0" && dy != Ymin)
                        tb.Text = dy.ToString("E0");

                    tb.TextAlignment = TextAlignment.Right;

                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;

                    if (rightOffset < size.Width)
                        rightOffset = size.Width + size.Width;
                }


            TextBlock tbTitle = new TextBlock();
            tbTitle.Text = Title;

            tbTitle.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbTitle.DesiredSize;

            topOffset = size.Height + 3;

            TextCanvas.Children.Add(tbTitle);

            TextBlock tbXLabel = new TextBlock();
            tbXLabel.Text = XLabel;

            tbXLabel.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tbXLabel.DesiredSize;
            TextCanvas.Children.Add(tbXLabel);

            bottomOffset = 15 + tbXLabel.DesiredSize.Height;

            if (XLabels.Length != 0)
            {
                string a = XLabels.OrderByDescending(x => x.Length).First();

                TextBox tbx = new TextBox();

                tb.Text = a;

                if (tb.Text == "0" && dy != Ymin)
                    tb.Text = dy.ToString("E0");

                tb.TextAlignment = TextAlignment.Right;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                tb.LayoutTransform = new RotateTransform(270);
                size = tb.DesiredSize;

                if (bottomOffset < size.Width + 15)
                    bottomOffset = size.Width + 15;
            }

            TextCanvas.Children.Add(tbYLabel);

            leftOffset = tbYLabel.DesiredSize.Height + offset + 5;

            #endregion


            //point, line, offset, dx, dy and textblock variables
            Rectangle tickY = new Rectangle();
            double recHeight = 0;

            XTick = (XTick == 0) ? 0.5 : XTick;
            YTick = (YTick == 0) ? 0.5 : YTick;
            Xmax = (Xmax <= Xmin) ? Xmin + 1 : Xmax;
            Ymax = (Ymax <= Ymin) ? Ymin + 1 : Ymax;

            tb = new TextBlock();
            GrainSizeToIntConverter gsi = new GrainSizeToIntConverter();

            // determine right offset: 
            tb.Text = gsi.ConvertBack(Convert.ToInt32(Xmax));
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            size = tb.DesiredSize;

            rightOffset = size.Width / 2 + 15;

            // Determine left offset through iterating through the whole y axis labels: 
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                pt = NormalizePoint(new Point(Xmin, dy));
                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.TextAlignment = TextAlignment.Right;
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

                if (offset < size.Width)
                    offset = size.Width;
            }

            leftOffset = offset + 15;
            bottomOffset = 60;

            Canvas.SetLeft(ChartCanvas, leftOffset);
            Canvas.SetBottom(ChartCanvas, bottomOffset);

            ChartCanvas.Width = Math.Abs(TextCanvas.Width - leftOffset - rightOffset);
            ChartCanvas.Height = Math.Abs(TextCanvas.Height - bottomOffset - size.Height / 2);

            //The chart rectangle
            Rectangle chartRect = new Rectangle();
            chartRect.Stroke = Brushes.Black;
            chartRect.Width = ChartCanvas.Width;
            chartRect.Height = ChartCanvas.Height;
            ChartCanvas.Children.Add(chartRect);

            // Create vertical gridlines: 
            if (IsYGrid == true)
            {
                for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                {
                    gridline = new Line();
                    AddLinePattern();
                    gridline.X1 = NormalizePoint(new Point(dx, Ymin)).X;
                    gridline.Y1 = NormalizePoint(new Point(dx, Ymin)).Y;
                    gridline.X2 = NormalizePoint(new Point(dx, Ymax)).X;
                    gridline.Y2 = NormalizePoint(new Point(dx, Ymax)).Y;
                    ChartCanvas.Children.Add(gridline);
                }
            }

            int counter = 0;
            recHeight = Math.Abs(NormalizePoint(new Point(Xmin, Ymin + YTick)).Y - NormalizePoint(new Point(Xmin, Ymin)).Y);

            // Create horizontal gridlines: 
            if (IsXGrid == true)
            {
                for (dy = Ymin + YTick; dy <= Ymax; dy += YTick)
                {
                    gridline = new Line();

                    AddLinePattern();

                    gridline.X1 = NormalizePoint(new Point(Xmin, dy)).X;

                    gridline.Y1 = NormalizePoint(new Point(Xmin, dy)).Y;

                    gridline.X2 = NormalizePoint(new Point(Xmax, dy)).X;

                    gridline.Y2 = NormalizePoint(new Point(Xmax, dy)).Y;

                    ChartCanvas.Children.Add(gridline);

                    if (counter == 0 || counter % 2 == 0)
                    {
                        tickY = new Rectangle();
                        tickY.Fill = Brushes.White;
                        tickY.Stroke = Brushes.Black;
                        tickY.Width = 10;
                        tickY.Height = recHeight;
                        TextCanvas.Children.Add(tickY);

                        Canvas.SetRight(tickY, ChartCanvas.Width + rightOffset);
                        Canvas.SetTop(tickY, NormalizePoint(new Point(Xmin, dy)).Y + size.Height / 2);
                    }
                    else
                    {
                        tickY = new Rectangle();
                        tickY.Fill = Brushes.Black;
                        tickY.Width = 10;
                        tickY.Height = recHeight;
                        TextCanvas.Children.Add(tickY);

                        Canvas.SetRight(tickY, ChartCanvas.Width + rightOffset);
                        Canvas.SetTop(tickY, NormalizePoint(new Point(Xmin, dy)).Y + size.Height / 2);
                    }

                    counter++;

                    if (dy > Ymax - YTick && dy + YTick != Ymax)
                    {
                        recHeight = Math.Abs(NormalizePoint(new Point(Xmin, Ymax)).Y - NormalizePoint(new Point(Xmin, Ymax - (Ymax - dy))).Y);

                        if (counter == 0 || counter % 2 == 0)
                        {

                            tickY = new Rectangle();
                            tickY.Fill = Brushes.White;
                            tickY.Stroke = Brushes.Black;
                            tickY.Width = 10;
                            tickY.Height = recHeight;
                            TextCanvas.Children.Add(tickY);

                            Canvas.SetRight(tickY, ChartCanvas.Width + rightOffset);
                            Canvas.SetTop(tickY, NormalizePoint(new Point(Xmin, Ymax)).Y + size.Height / 2);
                        }
                        else
                        {
                            tickY = new Rectangle();
                            tickY.Fill = Brushes.Black;
                            tickY.Width = 10;
                            tickY.Height = recHeight;
                            TextCanvas.Children.Add(tickY);

                            Canvas.SetRight(tickY, ChartCanvas.Width + rightOffset);
                            Canvas.SetTop(tickY, NormalizePoint(new Point(Xmin, Ymax)).Y + size.Height / 2);
                        }
                    }
                }
            }

            // Create x-axis tick marks: 
            for (dx = Xmin; dx <= Xmax; dx += xTick)
            {
                if (xTick == 0)
                    xTick += 1;

                //Configuring ticks
                pt = NormalizePoint(new Point(dx, Ymin));
                tick = new Line();
                tick.Stroke = Brushes.Black;
                tick.X1 = pt.X;
                tick.Y1 = pt.Y;
                tick.X2 = pt.X;
                tick.Y2 = pt.Y - 5;
                ChartCanvas.Children.Add(tick);

                //Configuring x axis blocks
                tb = new TextBlock();
                tb.Text = gsi.ConvertBack(Convert.ToInt32(dx));
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                tb.LayoutTransform = new RotateTransform(45, pt.X, pt.Y);

                TextCanvas.Children.Add(tb);
                //Defining the position
                Canvas.SetLeft(tb, leftOffset + pt.X - size.Width / 2);
                Canvas.SetTop(tb, pt.Y + 10);
            }

            // Create y-axis tick marks: 
            for (dy = Ymin; dy <= Ymax; dy += YTick)
            {
                if (YTick == 0)
                    YTick += 1;

                pt = NormalizePoint(new Point(Xmin, dy));

                tb = new TextBlock();
                tb.Text = dy.ToString();
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;
                TextCanvas.Children.Add(tb);
                Canvas.SetRight(tb, ChartCanvas.Width + rightOffset + tickY.Width + 2);
                Canvas.SetTop(tb, pt.Y);

            }


            // Add title and labels: 
            tbTitle.Text = Title;
            tbTitle.FontSize = 20;
            tbXLabel.Text = XLabel;
            tbXLabel.FontSize = 14;
            tbXLabel.FontWeight = FontWeights.Bold;
            tbYLabel.Text = YLabel;
            tbYLabel.FontSize = 14;
            tbYLabel.FontWeight = FontWeights.Bold;
            tbXLabel.Margin = new Thickness(leftOffset + 2, 2, 2, 2);
            tbTitle.Margin = new Thickness(leftOffset + 2, 2, 2, 2);

            Canvas.SetLeft(tbTitle, leftOffset - (tbTitle.DesiredSize.Width / 2) + (ChartCanvas.Width / 2));
            Canvas.SetTop(tbTitle, 2);

            Canvas.SetLeft(tbYLabel, 2);
            Canvas.SetTop(tbYLabel, bottomOffset - tbYLabel.DesiredSize.Width / 2 + ChartCanvas.Height / 2);

            Canvas.SetLeft(tbXLabel, leftOffset - tbXLabel.DesiredSize.Width / 2 + ChartCanvas.Width / 2);
            Canvas.SetTop(tbXLabel, TextCanvas.Height - size.Height + 2);
        }

        /// <summary>
        /// Setting a 3D chartstyle
        /// </summary>
        public void SetChartStyle()
        {
            AddTicks();
            AddGridlines();
            AddAxes();
            AddLabels();
        }

        //Setting the lines of the Chart control
        public void SetLines(BindableCollection<LineSeries> dc)
        {
            if (dc.Count <= 0)
                return;

            for (int i = 0; i < dc.Count; i++)
            {
                if (dc[i].SeriesName == "Default")
                    dc[i].SeriesName = "LineSeries" + i.ToString();

                dc[i].SetLinePattern();

                for (int j = 0; j < dc[i].LinePoints.Count; j++)
                {
                    dc[i].LinePoints[j] = NormalizePoint(dc[i].LinePoints[j]);

                    if (dc[i].Symbols.SymbolType != SymbolTypeEnum.None)
                        dc[i].Symbols.AddSymbol(ChartCanvas, dc[i].LinePoints[j]);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dc"></param>
        public void SetLinesControl(BindableCollection<LineSeries> dc, ColormapBrush ColorMap = null)
        {
            if (dc.Count <= 0)
                return;

            int i = 0;

            foreach (var ds in dc)
            {
                if (ShowConvexHull)
                {
                    if (ds.Hull.Count == 0)
                        ds.ComputeComplexHull();

                    PointCollection polpts = new PointCollection();

                    for (int j = 0; j < ds.Hull.Count; j++)
                    {
                        try
                        {
                            var pt = NormalizePoint(ds.Hull[j]);
                            polpts.Add(pt);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    Polygon polygon = new Polygon();
                    polygon.Stroke = ds.Symbols.FillColor;
                    polygon.StrokeThickness = 1;
                    polygon.Fill = ds.Symbols.FillColor.CloneCurrentValue();
                    polygon.Fill.Opacity = 0.1;

                    polygon.Points = polpts;
                    ChartCanvas.Children.Add(polygon);
                }

                    PointCollection pts = new PointCollection();
                PointCollection pts2 = new PointCollection();

                if (ds.SeriesName == "Default")
                    ds.SeriesName = "LineSeries" + i.ToString();

                ds.SetLinePattern();

                for (int j = 0; j < ds.LinePoints.Count; j++)
                {
                    try
                    {
                        var pt = NormalizePoint(ds.LinePoints[j]);
                        pts.Add(pt);

                        if (ds.Symbols.SymbolType != SymbolTypeEnum.None)
                        {
                            ds.Symbols.AddSymbol(ChartCanvas, pt);
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }
                for (int j = 0; j < ds.LinePoints2.Count; j++)
                {
                    try
                    {
                        var pt = NormalizeSeconOrdinatePoint(ds.LinePoints2[j]);
                        pts2.Add(pt);

                        if (ds.Symbols.SymbolType != SymbolTypeEnum.None)
                            ds.Symbols.AddSymbol(ChartCanvas, pt);
                    }
                    catch
                    {
                        continue;
                    }

                }

                Polyline line = new Polyline();

                line.Points = pts;

                line.Stroke = ds.LineColor;

                if (ds.LinePattern != LinePatternEnum.None)
                    line.StrokeThickness = ds.LineThickness;
                else
                    line.StrokeThickness = 0;


                line.StrokeDashArray = ds.LineDashPattern;
                ChartCanvas.Children.Add(line);


                Polyline line2 = new Polyline();

                line2.Points = pts2;

                line2.Stroke = ds.LineColor;

                if (ds.LinePattern != LinePatternEnum.None)
                    line2.StrokeThickness = ds.LineThickness;
                else
                    line2.StrokeThickness = 0;


                line2.StrokeDashArray = ds.LineDashPattern;

                ChartCanvas.Children.Add(line2);

                Canvas.SetZIndex(line2, 1);

                i++;

            }
        }

        public void Set2DMatrixControl(BindableCollection<MatrixSeries> dc, ColormapBrush ColorMap = null)
        {
            if (dc.Count <= 0)
                return;

            int i = 0;

            foreach (var ds in dc)
            {
                BitmapImage bitmap = ds.LinePoints.ToBitmap(ColorMap).ToBitmapImage();
                Image image = new Image();
                image.Source = bitmap;

                ChartCanvas.Children.Add(image);
                i++;

            }
        }

        /// <summary>
        /// Adding a line pattern to the 
        /// </summary>
        public void AddLinePattern()
        {
            gridline.Stroke = GridlineColor;
            gridline.StrokeThickness = 1;

            switch (GridlinePattern)
            {
                case LinePatternEnum.Dash:
                    gridline.StrokeDashArray = new DoubleCollection() { 4, 3 };
                    break;
                case LinePatternEnum.Dot:
                    gridline.StrokeDashArray = new DoubleCollection() { 1, 2 };
                    break;
                case LinePatternEnum.DashDot:
                    gridline.StrokeDashArray = new DoubleCollection() { 4, 2, 1, 2 };
                    break;
            }
        }

        /// <summary>
        /// Adding a pattern to a line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineColor"></param>
        /// <param name="lineThickness"></param>
        /// <param name="linePattern"></param>
        public void AddLinePattern(Line line, Brush lineColor, double lineThickness, LinePatternEnum linePattern)
        {
            line.Stroke = lineColor;
            line.StrokeThickness = lineThickness;

            switch (linePattern)
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

        private void AddTicks()
        {
            Matrix3D m = Chart3DHelper.AzimuthElevation(Elevation, Azimuth);

            Point3D[] pta = new Point3D[2];

            Point3D[] pts = ChartBoxCoordinates();

            XTick = (XTick == 0) ? 0.5 : XTick;
            YTick = (YTick == 0) ? 0.5 : YTick;
            ZTick = (ZTick == 0) ? 0.5 : ZTick;
            Xmax = (Xmax <= Xmin) ? Xmin + 1 : Xmax;
            Ymax = (Ymax <= Ymin) ? Ymin + 1 : Ymax;
            Zmax = (Zmax <= Zmin) ? Zmin + 1 : Zmax;

            if ((Ymax - Ymin) / YTick > 1E+2)
                YTick = (Ymax - Ymin) / 10;

            if ((Xmax - Xmin) / XTick > 1E+2)
                XTick = (Xmax - Xmin) / 10;

            if ((Zmax - Zmin) / ZTick > 1E+2)
                ZTick = (Zmax - Zmin) / 10;

            // Add x ticks: 
            double offset = (Ymax - Ymin) / 30.0;

            double ticklength = offset;

            for (double x = Xmin; x <= Xmax; x = x + XTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -90 && Azimuth < 90)
                        ticklength = -offset;
                }
                else if (Elevation < 0)
                {
                    if ((Azimuth >= -180 && Azimuth < -90) || Azimuth >= 90 && Azimuth <= 180)
                        ticklength = -(Ymax - Ymin) / 30;
                }
                pta[0] = new Point3D(x, pts[1].Y + ticklength, pts[1].Z);

                pta[1] = new Point3D(x, pts[1].Y, pts[1].Z);

                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i] = Normalize3D(m, pta[i]);
                }

                DrawLine(pta[0], pta[1], AxisColor, 1, LinePatternEnum.Solid);

            }

            // Add y ticks: 
            offset = (Xmax - Xmin) / 30.0;

            ticklength = offset;

            for (double y = Ymin; y <= Ymax; y = y + YTick)
            {
                pts = ChartBoxCoordinates(); if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < 0) ticklength = -offset;
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= 0 && Azimuth < 180)
                        ticklength = -offset;
                }

                pta[0] = new Point3D(pts[1].X + ticklength, y, pts[1].Z);
                pta[1] = new Point3D(pts[1].X, y, pts[1].Z);

                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i] = Normalize3D(m, pta[i]);
                }
                DrawLine(pta[0], pta[1], AxisColor, 1, LinePatternEnum.Solid);
            }

            // Add z ticks: 
            double xoffset = (Xmax - Xmin) / 45.0;

            double yoffset = (Ymax - Ymin) / 20.0;

            double xticklength = xoffset;

            double yticklength = yoffset;

            for (double z = Zmin; z <= Zmax; z = z + ZTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        xticklength = 0;
                        yticklength = yoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        xticklength = xoffset;
                        yticklength = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        xticklength = 0;
                        yticklength = -yoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        xticklength = -xoffset;
                        yticklength = 0;
                    }
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        yticklength = 0;
                        xticklength = xoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        yticklength = -yoffset;
                        xticklength = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        yticklength = 0;
                        xticklength = -xoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        yticklength = yoffset;
                        xticklength = 0;
                    }
                }

                pta[0] = new Point3D(pts[2].X, pts[2].Y, z);

                pta[1] = new Point3D(pts[2].X + yticklength, pts[2].Y + xticklength, z);

                for (int i = 0; i < pta.Length; i++)
                {
                    pta[i] = Normalize3D(m, pta[i]);
                }

                DrawLine(pta[0], pta[1], AxisColor, 1, LinePatternEnum.Solid);
            }
        }

        #endregion

        #region Helper

        //Normalizes a point information based on the relative width and height of the canvas object
        public Point NormalizePoint(Point pt)
        {
            if (Double.IsNaN(ChartCanvas.Width) || ChartCanvas.Width <= 0)
                ChartCanvas.Width = 270;

            if (Double.IsNaN(ChartCanvas.Height) || ChartCanvas.Height <= 0)
                ChartCanvas.Height = 250;

            Point result = new Point();

            if (IsXLog)
                result.X = (Math.Log10(pt.X) - Math.Log10(Xmin)) * ChartCanvas.Width / (Math.Log10(Xmax) - Math.Log10(Xmin));
            else
                result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);

            if (IsYLog)
                result.Y = ChartCanvas.Height - (Math.Log10(pt.Y) - Math.Log10(Ymin)) * ChartCanvas.Height / (Math.Log10(Ymax) - Math.Log10(Ymin));
            else
                result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);

            if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
                return new Point(0, 0);

            return result;
        }

        //Normalizes a point information based on the relative width and height of the canvas object
        public Point NormalizeMultiChartPoint(Point pt, 
            double localXmin, 
            double localXmax,
            double elementsCount,
            double elementIndex)
        {
            if (Double.IsNaN(ChartCanvas.Width) || ChartCanvas.Width <= 0)
                ChartCanvas.Width = 270;

            if (Double.IsNaN(ChartCanvas.Height) || ChartCanvas.Height <= 0)
                ChartCanvas.Height = 250;

            Point result = new Point();

            if (IsXLog)
                result.X = (Math.Log10(pt.X) - Math.Log10(Xmin)) * ChartCanvas.Width / (Math.Log10(Xmax) - Math.Log10(Xmin));
            else
                result.X = (pt.X - localXmin) * ((ChartCanvas.Width/elementsCount) + ((ChartCanvas.Width/elementsCount)*elementIndex)) / (localXmax - localXmin);

            if (IsYLog)
                result.Y = ChartCanvas.Height - (Math.Log10(pt.Y) - Math.Log10(Ymin)) * ChartCanvas.Height / (Math.Log10(Ymax) - Math.Log10(Ymin));
            else
                result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);

            if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
                return new Point(0, 0);

            return result;
        }

        //Normalizes a point information based on the relative width and height of the canvas object
        public Point NormalizeSeconOrdinatePoint(Point pt)
        {
            if (Double.IsNaN(ChartCanvas.Width) || ChartCanvas.Width <= 0)
                ChartCanvas.Width = 270;

            if (Double.IsNaN(ChartCanvas.Height) || ChartCanvas.Height <= 0)
                ChartCanvas.Height = 250;

            Point result = new Point();

            if (IsXLog)
                result.X = (Math.Log10(pt.X) - Math.Log10(Xmin)) * ChartCanvas.Width / (Math.Log10(Xmax) - Math.Log10(Xmin));
            else
                result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);

            if (IsY2Log)
                result.Y = ChartCanvas.Height - (Math.Log10(pt.Y) - Math.Log10(Y2min)) * ChartCanvas.Height / (Math.Log10(Y2max) - Math.Log10(Y2min));
            else
                result.Y = ChartCanvas.Height - (pt.Y - Y2min) * ChartCanvas.Height / (Y2max - Y2min);

            if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
                return new Point(0, 0);

            return result;
        }

        public Point3D Normalize3D(Matrix3D m, Point3D pt)
        {
            Point3D result = new Point3D();
            // Normalize the point:
            double x1 = (pt.X - Xmin) / (Xmax - Xmin) - 0.5;
            double y1 = (pt.Y - Ymin) / (Ymax - Ymin) - 0.5;
            double z1 = (pt.Z - Zmin) / (Zmax - Zmin) - 0.5;
            // Perform transformation on the point using matrix m:
            result.X = m.Transform(new Point3D(x1, y1, z1)).X;
            result.Y = m.Transform(new Point3D(x1, y1, z1)).Y;
            // Coordinate transformation from World to Device system:
            double xShift = 1.05;
            double xScale = 1;
            double yShift = 1.05;
            double yScale = 0.9;

            if (Title == "No Title")
            {
                yShift = 0.95;
                yScale = 1;
            }

            if (IsBubbleChart)
            {
                xShift = 0.95;
                xScale = 0.9;
            }

            result.X = (xShift + xScale * result.X) * ChartCanvas.Width / 2;
            result.Y = (yShift - yScale * result.Y) * ChartCanvas.Height / 2;
            return result;
        }

        //Adding axes to the chartCanvas
        private void AddAxes()
        {
            Matrix3D m = Chart3DHelper.AzimuthElevation(Elevation, Azimuth);

            Point3D[] pts = ChartBoxCoordinates();

            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = Normalize3D(m, pts[i]);
            }

            DrawLine(pts[0], pts[1], AxisColor, AxisThickness, AxisPattern);
            DrawLine(pts[1], pts[2], AxisColor, AxisThickness, AxisPattern);
            DrawLine(pts[2], pts[3], AxisColor, AxisThickness, AxisPattern);

        }

        private void AddLabels()
        {
            Matrix3D m = Chart3DHelper.AzimuthElevation(Elevation, Azimuth);

            Point3D pt = new Point3D();

            Point3D[] pts = ChartBoxCoordinates();

            TextBlock tb = new TextBlock();

            // Add x tick labels: 
            double offset = (Ymax - Ymin) / 20;

            double labelSpace = offset;

            for (double x = Xmin; x <= Xmax; x = x + XTick)
            {
                if (Elevation >= 0)
                {
                    if (Azimuth >= -90 && Azimuth < 90)
                        labelSpace = -offset;
                }
                else if (Elevation < 0)
                {
                    if ((Azimuth >= -180 && Azimuth < -90) || Azimuth >= 90 && Azimuth <= 180)
                        labelSpace = -offset;
                }

                pt = new Point3D(x, pts[1].Y + labelSpace, pts[1].Z);

                pt = Normalize3D(m, pt);

                tb = new TextBlock();

                tb.Text = x.ToString();


                if (tb.Text == "" && x != Xmin)
                    tb.Text = x.ToString("E0");

                tb.Foreground = TickColor;
                tb.FontFamily = TickFont;
                tb.FontSize = TickFontSize;
                tb.TextAlignment = TextAlignment.Center;

                ChartCanvas.Children.Add(tb); Canvas.SetLeft(tb, pt.X);
                Canvas.SetTop(tb, pt.Y);
            }

            // Add y tick labels: 
            offset = (Xmax - Xmin) / 20;

            labelSpace = offset;

            for (double y = Ymin; y <= Ymax; y = y + YTick)
            {
                pts = ChartBoxCoordinates();

                if (elevation >= 0)
                {
                    if (azimuth >= -180 && azimuth < 0)
                        labelSpace = -offset;
                }
                else if (elevation < 0)
                {
                    if (azimuth >= 0 && azimuth < 180) labelSpace = -offset;
                }

                pt = new Point3D(pts[1].X + labelSpace, y, pts[1].Z); pt = Normalize3D(m, pt);

                tb = new TextBlock();

                tb.Text = y.ToString();

                if (tb.Text == "0" && y != Ymin)
                    tb.Text = y.ToString("E0");

                tb.Foreground = TickColor;

                tb.FontFamily = TickFont;

                tb.FontSize = TickFontSize;

                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

                Size ytickSize = tb.DesiredSize;

                ChartCanvas.Children.Add(tb); Canvas.SetLeft(tb, pt.X - ytickSize.Width / 2); Canvas.SetTop(tb, pt.Y);
            }


            // Add z tick labels: 
            double xoffset = (Xmax - Xmin) / 30.0;

            double yoffset = (Ymax - Ymin) / 15.0;

            double xlabelSpace = xoffset;

            double ylabelSpace = yoffset;

            tb = new TextBlock();

            tb.Text = "A";

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            Size size = tb.DesiredSize;

            for (double z = Zmin; z <= Zmax; z = z + ZTick)
            {
                pts = ChartBoxCoordinates();

                if (Elevation >= 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        xlabelSpace = 0;
                        ylabelSpace = yoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        xlabelSpace = xoffset;
                        ylabelSpace = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        xlabelSpace = 0; ylabelSpace = -yoffset;
                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        xlabelSpace = -xoffset; ylabelSpace = 0;
                    }
                }
                else if (Elevation < 0)
                {
                    if (Azimuth >= -180 && Azimuth < -90)
                    {
                        ylabelSpace = 0; xlabelSpace = xoffset;
                    }
                    else if (Azimuth >= -90 && Azimuth < 0)
                    {
                        ylabelSpace = -yoffset; xlabelSpace = 0;
                    }
                    else if (Azimuth >= 0 && Azimuth < 90)
                    {
                        ylabelSpace = 0;

                        xlabelSpace = -xoffset;

                    }
                    else if (Azimuth >= 90 && Azimuth <= 180)
                    {
                        ylabelSpace = yoffset;
                        xlabelSpace = 0;
                    }
                }

                pt = new Point3D(pts[2].X + ylabelSpace, pts[2].Y + xlabelSpace, z);

                pt = Normalize3D(m, pt); tb = new TextBlock();

                tb.Text = z.ToString();

                if (tb.Text == "" && z != Zmin)
                    tb.Text = z.ToString("E0");

                tb.Foreground = TickColor;

                tb.FontFamily = TickFont;

                tb.FontSize = TickFontSize;

                tb.Measure(new Size(Double.PositiveInfinity,
                    Double.PositiveInfinity));

                Size ztickSize = tb.DesiredSize;

                ChartCanvas.Children.Add(tb);

                Canvas.SetLeft(tb, pt.X - ztickSize.Width - 1);

                Canvas.SetTop(tb, pt.Y - ztickSize.Height / 2);
            }

            // Add Title: 
            tb = new TextBlock();
            tb.Text = Title;
            tb.Foreground = TitleColor;
            tb.FontSize = TitleFontSize;
            tb.FontFamily = TitleFont;
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            Size titleSize = tb.DesiredSize;

            if (tb.Text != "No Title")
            {
                ChartCanvas.Children.Add(tb);
                Canvas.SetLeft(tb, ChartCanvas.Width / 2 - titleSize.Width / 2);
                Canvas.SetTop(tb, ChartCanvas.Height / 30);
            }

            // Add x axis label: 
            offset = (Ymax - Ymin) / 3; labelSpace = offset; double offset1 = (Xmax - Xmin) / 10; double xc = offset1; if (Elevation >= 0) { if (Azimuth >= -90 && Azimuth < 90) labelSpace = -offset; if (Azimuth >= 0 && Azimuth <= 180) xc = -offset1; } else if (Elevation < 0) { if ((Azimuth >= -180 && Azimuth < -90) || Azimuth >= 90 && Azimuth <= 180) labelSpace = -offset; if (Azimuth >= -180 && Azimuth <= 0) xc = -offset1; }
            Point3D[] pta = new Point3D[2]; pta[0] = new Point3D(Xmin, pts[1].Y + labelSpace, pts[1].Z); pta[1] = new Point3D((Xmin + Xmax) / 2 - xc, pts[1].Y + labelSpace, pts[1].Z); pta[0] = Normalize3D(m, pta[0]); pta[1] = Normalize3D(m, pta[1]); double theta = Math.Atan((pta[1].Y - pta[0].Y) / (pta[1].X - pta[0].X)); theta = theta * 180 / Math.PI; tb = new TextBlock(); tb.Text = XLabel; tb.Foreground = LabelColor; tb.FontFamily = LabelFont; tb.FontSize = LabelFontSize;
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity)); Size xLabelSize = tb.DesiredSize;
            TransformGroup tg = new TransformGroup(); RotateTransform rt = new RotateTransform(theta, 0.5, 0.5); TranslateTransform tt = new TranslateTransform(pta[1].X + xLabelSize.Width / 2, pta[1].Y - xLabelSize.Height / 2); tg.Children.Add(rt); tg.Children.Add(tt); tb.RenderTransform = tg; ChartCanvas.Children.Add(tb);
            // Add y axis label: 
            offset = (Xmax - Xmin) / 3; offset1 = (Ymax - Ymin) / 5; labelSpace = offset; double yc = YTick; if (Elevation >= 0) { if (Azimuth >= -180 && Azimuth < 0) labelSpace = -offset; if (Azimuth >= -90 && Azimuth <= 90) yc = -offset1; } else if (Elevation < 0) { yc = -offset1; if (Azimuth >= 0 && Azimuth < 180) labelSpace = -offset; if (Azimuth >= -90 && Azimuth <= 90) yc = offset1; }
            pta[0] = new Point3D(pts[1].X + labelSpace, Ymin, pts[1].Z); pta[1] = new Point3D(pts[1].X + labelSpace, (Ymin + Ymax) / 2 + yc, pts[1].Z); pta[0] = Normalize3D(m, pta[0]); pta[1] = Normalize3D(m, pta[1]);
            theta = (double)Math.Atan((pta[1].Y - pta[0].Y) / (pta[1].X - pta[0].X)); theta = theta * 180 / (double)Math.PI; tb = new TextBlock(); tb.Text = YLabel; tb.Foreground = LabelColor; tb.FontFamily = LabelFont; tb.FontSize = LabelFontSize; tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity)); Size yLabelSize = tb.DesiredSize;
            tg = new TransformGroup(); tt = new TranslateTransform(pta[1].X - yLabelSize.Width / 2, pta[1].Y - yLabelSize.Height / 2); rt = new RotateTransform(theta, 0.5, 0.5); tg.Children.Add(rt); tg.Children.Add(tt);
            tb.RenderTransform = tg; ChartCanvas.Children.Add(tb);
            // Add z axis labels: 
            double zticklength = 10; labelSpace = -1.3f * offset; offset1 = (Zmax - Zmin) / 8; double zc = -offset1; for (double z = Zmin; z < Zmax; z = z + ZTick) { tb = new TextBlock(); tb.Text = z.ToString(); tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity)); Size size1 = tb.DesiredSize; if (zticklength < size1.Width) zticklength = size1.Width; }
            double zlength = -zticklength; if (Elevation >= 0) { if (Azimuth >= -180 && Azimuth < -90) { zlength = -zticklength; labelSpace = -1.3f * offset; zc = -offset1; } else if (Azimuth >= -90 && Azimuth < 0) { zlength = zticklength; labelSpace = 2 * offset / 3; zc = offset1; } else if (Azimuth >= 0 && Azimuth < 90) { zlength = zticklength; labelSpace = 2 * offset / 3; zc = -offset1; } else if (Azimuth >= 90 && Azimuth <= 180) { zlength = -zticklength; labelSpace = -1.3f * offset; zc = offset1; } }
            else if (Elevation < 0)
            {
                if (Azimuth >= -180 && Azimuth < -90) { zlength = -zticklength; labelSpace = -1.3f * offset; zc = offset1; }
                else if (Azimuth >= -90 && Azimuth < 0) { zlength = zticklength; labelSpace = 2 * offset / 3; zc = -offset1; } else if (Azimuth >= 0 && Azimuth < 90) { zlength = zticklength; labelSpace = 2 * offset / 3; zc = offset1; } else if (Azimuth >= 90 && Azimuth <= 180) { zlength = -zticklength; labelSpace = -1.3f * offset; zc = -offset1; }
            }
            pta[0] = new Point3D(pts[2].X - labelSpace, pts[2].Y, (Zmin + Zmax) / 2 + zc);
            pta[0] = Normalize3D(m, pta[0]); tb = new TextBlock(); tb.Text = ZLabel; tb.Foreground = LabelColor; tb.FontFamily = LabelFont; tb.FontSize = LabelFontSize; tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity)); Size zLabelSize = tb.DesiredSize;
            tg = new TransformGroup(); tt = new TranslateTransform(pta[0].X - zlength, pta[0].Y + zLabelSize.Width / 2); rt = new RotateTransform(270, 0.5, 0.5); tg.Children.Add(rt); tg.Children.Add(tt); tb.RenderTransform = tg; ChartCanvas.Children.Add(tb);
        }
        private void AddGridlines()
        {
            Matrix3D m = Chart3DHelper.AzimuthElevation(Elevation, Azimuth); Point3D[] pta = new Point3D[3]; Point3D[] pts = ChartBoxCoordinates();
            // Draw x gridlines: 
            if (IsXGrid)
            {
                for (double x = Xmin; x <= Xmax; x = x + XTick)
                {
                    pts = ChartBoxCoordinates();
                    pta[0] = new Point3D(x, pts[1].Y, pts[1].Z);

                    if (Elevation >= 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) || (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3D(x, pts[0].Y, pts[1].Z);
                            pta[2] = new Point3D(x, pts[0].Y, pts[3].Z);
                        }
                        else
                        {
                            pta[1] = new Point3D(x, pts[2].Y, pts[1].Z);
                            pta[2] = new Point3D(x, pts[2].Y, pts[3].Z);
                        }
                    }
                    else if (Elevation < 0)
                    {
                        if ((Azimuth >= -180 && Azimuth < -90) || (Azimuth >= 0 && Azimuth < 90))
                        {
                            pta[1] = new Point3D(x, pts[2].Y, pts[1].Z);
                            pta[2] = new Point3D(x, pts[2].Y, pts[3].Z);
                        }
                        else
                        {
                            pta[1] = new Point3D(x, pts[0].Y, pts[1].Z);
                            pta[2] = new Point3D(x, pts[0].Y, pts[3].Z);
                        }
                    }

                    for (int i = 0; i < pta.Length; i++)
                    {
                        pta[i] = Normalize3D(m, pta[i]);
                    }

                    DrawLine(pta[0], pta[1], GridlineColor, GridlineThickness, GridlinePattern);
                    DrawLine(pta[1], pta[2], GridlineColor, GridlineThickness, GridlinePattern);

                }
                // Draw y gridlines: 
                if (IsYGrid)
                {
                    for (double y = Ymin; y <= Ymax; y = y + YTick)
                    {
                        pts = ChartBoxCoordinates();
                        pta[0] = new Point3D(pts[1].X, y, pts[1].Z);

                        if (Elevation >= 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) || (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3D(pts[2].X, y, pts[1].Z);
                                pta[2] = new Point3D(pts[2].X, y, pts[3].Z);
                            }
                            else
                            {
                                pta[1] = new Point3D(pts[0].X, y, pts[1].Z);
                                pta[2] = new Point3D(pts[0].X, y, pts[3].Z);
                            }
                        }
                        if (elevation < 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) || (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3D(pts[0].X, y, pts[1].Z);
                                pta[2] = new Point3D(pts[0].X, y, pts[3].Z);
                            }
                            else
                            {
                                pta[1] = new Point3D(pts[2].X, y, pts[1].Z);
                                pta[2] = new Point3D(pts[2].X, y, pts[3].Z);
                            }
                        }
                        for (int i = 0; i < pta.Length; i++)
                        {
                            pta[i] = Normalize3D(m, pta[i]);
                        }

                        DrawLine(pta[0], pta[1], GridlineColor, GridlineThickness, GridlinePattern);
                        DrawLine(pta[1], pta[2], GridlineColor, GridlineThickness, GridlinePattern);
                    }
                }
                // Draw Z gridlines: 
                if (IsZGrid)
                {
                    for (double z = Zmin; z <= Zmax; z = z + ZTick)
                    {
                        pts = ChartBoxCoordinates();

                        pta[0] = new Point3D(pts[2].X, pts[2].Y, z); if (Elevation >= 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) || (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3D(pts[2].X, pts[0].Y, z);
                                pta[2] = new Point3D(pts[0].X, pts[0].Y, z);
                            }
                            else
                            {
                                pta[1] = new Point3D(pts[0].X, pts[2].Y, z);
                                pta[2] = new Point3D(pts[0].X, pts[1].Y, z);
                            }
                        }
                        if (Elevation < 0)
                        {
                            if ((Azimuth >= -180 && Azimuth < -90) ||
                                (Azimuth >= 0 && Azimuth < 90))
                            {
                                pta[1] = new Point3D(pts[0].X, pts[2].Y, z);
                                pta[2] = new Point3D(pts[0].X, pts[0].Y, z);
                            }
                            else
                            {
                                pta[1] = new Point3D(pts[2].X, pts[0].Y, z);
                                pta[2] = new Point3D(pts[0].X, pts[0].Y, z);
                            }
                        }
                        for (int i = 0; i < pta.Length; i++)
                        {
                            pta[i] = Normalize3D(m, pta[i]);
                        }

                        DrawLine(pta[0], pta[1], GridlineColor, GridlineThickness, GridlinePattern);
                        DrawLine(pta[1], pta[2], GridlineColor, GridlineThickness, GridlinePattern);
                    }
                }
            }
        }


        /// <summary>
        /// Draws a line on the chart canvas
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="lineColor"></param>
        /// <param name="lineThickness"></param>
        /// <param name="linePattern"></param>
        private void DrawLine(Point3D pt1, Point3D pt2, Brush lineColor, double lineThickness, LinePatternEnum linePattern)
        {
            var line = new Line();

            AddLinePattern(line, lineColor, lineThickness, linePattern);

            line.X1 = pt1.X;
            line.Y1 = pt1.Y;
            line.X2 = pt2.X;
            line.Y2 = pt2.Y;

            ChartCanvas.Children.Add(line);
        }

        /// <summary>
        /// Calculating a set of edge coordinates
        /// </summary>
        /// <returns></returns>
        private Point3D[] ChartBoxCoordinates()
        {
            Point3D[] pta = new Point3D[8];

            pta[0] = new Point3D(Xmax, Ymin, Zmin);

            pta[1] = new Point3D(Xmin, Ymin, Zmin);

            pta[2] = new Point3D(Xmin, Ymax, Zmin);

            pta[3] = new Point3D(Xmin, Ymax, Zmax);

            pta[4] = new Point3D(Xmin, Ymin, Zmax);

            pta[5] = new Point3D(Xmax, Ymin, Zmax);

            pta[6] = new Point3D(Xmax, Ymax, Zmax);

            pta[7] = new Point3D(Xmax, Ymax, Zmin);

            Point3D[] pts = new Point3D[4];

            int[] npts = new int[] { 0, 1, 2, 3 };

            if (elevation >= 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                    npts = new int[] { 1, 2, 7, 6 };
                else if (azimuth >= -90 && azimuth < 0)
                    npts = new int[] { 0, 1, 2, 3 };
                else if (azimuth >= 0 && azimuth < 90)
                    npts = new int[] { 7, 0, 1, 4 };
                else if (azimuth >= 90 && azimuth <= 180)
                    npts = new int[] { 2, 7, 0, 5 };
            }

            else if (elevation < 0)
            {
                if (azimuth >= -180 && azimuth < -90)
                    npts = new int[] { 1, 0, 7, 6 };
                else if (azimuth >= -90 && azimuth < 0)
                    npts = new int[] { 0, 7, 2, 3 };
                else if (azimuth >= 0 && azimuth < 90)
                    npts = new int[] { 7, 2, 1, 4 };
                else if (azimuth >= 90 && azimuth <= 180)
                    npts = new int[] { 2, 1, 0, 5 };
            }

            for (int i = 0; i < 4; i++)
                pts[i] = pta[npts[i]];

            return pts;
        }

        #endregion
    }
}
