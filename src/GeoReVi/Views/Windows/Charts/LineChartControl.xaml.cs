using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Specialized;
using Caliburn.Micro;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für LineChartControlView.xaml
    /// </summary>
    public partial class LineChartControl : UserControl
    {
        #region Private members

        private ChartStyle cs;
        private Legend lg;
        private bool _handle = true;

        private Task drawing { get; set; }

        private Point previousPosition = new Point(0, 0);

        #endregion

        #region Public properties

        //Xmin property as dependency property
        public static DependencyProperty XminProperty = DependencyProperty.Register("Xmin", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmin { get { return (double)GetValue(XminProperty); } set { SetValue(XminProperty, value); } }

        //Xmax property as dependency property
        public static DependencyProperty XmaxProperty = DependencyProperty.Register("Xmax", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmax { get { return (double)GetValue(XmaxProperty); } set { SetValue(XmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty YminProperty = DependencyProperty.Register("Ymin", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymin { get { return (double)GetValue(YminProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty YmaxProperty = DependencyProperty.Register("Ymax", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymax { get { return (double)GetValue(YmaxProperty); } set { SetValue(YmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty Y2minProperty = DependencyProperty.Register("Y2min", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2min { get { return (double)GetValue(Y2minProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty Y2maxProperty = DependencyProperty.Register("Y2max", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2max { get { return (double)GetValue(Y2maxProperty); } set { SetValue(YmaxProperty, value); } }

        //YTick property as dependency property
        public static DependencyProperty Y2TickProperty = DependencyProperty.Register("Y2Tick", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2Tick { get { return (double)GetValue(Y2TickProperty); } set { SetValue(Y2TickProperty, value); } }

        //XTick property as dependency property
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set { SetValue(XTickProperty, value); }
        }

        //YTick property as dependency property
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double), typeof(LineChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double YTick { get { return (double)GetValue(YTickProperty); } set { SetValue(YTickProperty, value); } }

        //XLabel property as dependency property
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(LineChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }

        //XLabels property as dependency property
        public static DependencyProperty XLabelsProperty = DependencyProperty.Register("XLabels", typeof(IList<string>), typeof(LineChartControl), new FrameworkPropertyMetadata(new string[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public IList<string> XLabels { get { return (IList<string>)GetValue(XLabelsProperty); } set { SetValue(XLabelsProperty, value); } }


        //Label property as dependency property
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(LineChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty YLabelsProperty = DependencyProperty.Register("YLabels", typeof(IList<string>), typeof(LineChartControl), new FrameworkPropertyMetadata(new string[] { }, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public IList<string> YLabels { get { return (IList<string>)GetValue(YLabelsProperty); } set { SetValue(YLabelsProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(LineChartControl), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set { SetValue(IsXGridProperty, value); }
        }

        //IsXLog property as dependency property
        public static DependencyProperty IsXLogProperty = DependencyProperty.Register("IsXLog", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsXLog
        {
            get { return (bool)GetValue(IsXLogProperty); }
            set { SetValue(IsXLogProperty, value); }
        }

        //IsYGrid property as dependency property
        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsYGrid { get { return (bool)GetValue(IsYGridProperty); } set { SetValue(IsYGridProperty, value); } }

        //IsXLog property as dependency property
        public static DependencyProperty IsYLogProperty = DependencyProperty.Register("IsYLog", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsYLog
        {
            get { return (bool)GetValue(IsYLogProperty); }
            set { SetValue(IsYLogProperty, value); }
        }

        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(LineChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //GridlineColor property as dependency property
        public Brush GridlineColor { get { return (Brush)GetValue(GridlineColorProperty); } set { SetValue(GridlineColorProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(LineChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //LinePatternEnum property as dependency property
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }


        // IsLegend property as dependency property
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }

        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(LineChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        //Colormap property as dependency property
        public ColormapBrush ColorMap { get { return (ColormapBrush)GetValue(ColorMapProperty); } set { SetValue(ColorMapProperty, value); } }
        public static DependencyProperty ColorMapProperty = DependencyProperty.Register("ColorMap", typeof(ColormapBrush), typeof(LineChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// Bubble chart property
        /// </summary>
        public static DependencyProperty IsBubbleChartProperty = DependencyProperty.Register("IsBubbleChart", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsBubbleChart
        {
            get { return (bool)GetValue(IsBubbleChartProperty); }
            set { SetValue(IsBubbleChartProperty, value); }
        }

        //Data collection property as dependency property
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(BindableCollection<LineSeries>), typeof(LineChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));

        //Data collection for the chart control
        public BindableCollection<LineSeries> DataCollection
        {
            get
            {
                return (BindableCollection<LineSeries>)GetValue(DataCollectionProperty);
            }
            set
            {
                SetValue(DataCollectionProperty, value);
            }
        }

        //Check Count property as dependency property
        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(LineChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        private int CheckCount { get { return (int)GetValue(CheckCountProperty); } set { SetValue(CheckCountProperty, value); } }

        //The unit displayed
        private static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(LineChartControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Unit
        {
            get
            {
                return (string)GetValue(UnitProperty);
            }
            set
            {
                SetValue(UnitProperty, value);
            }
        }

        //Show convex hull property as dependency property
        public static DependencyProperty ShowConvexHullProperty = DependencyProperty.Register("ShowConvexHull", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ShowConvexHull
        {
            get { return (bool)GetValue(ShowConvexHullProperty); }
            set { SetValue(ShowConvexHullProperty, value); }
        }

        //IsDrawing property as dependency property
        public static DependencyProperty IsDrawingProperty = DependencyProperty.Register("IsDrawing", typeof(bool), typeof(LineChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineChartControl()
        {
            InitializeComponent();
            this.cs = new ChartStyle();
            this.lg = new Legend();
            cs.TextCanvas = textCanvas;
            cs.ChartCanvas = chartCanvas;
        }


        #endregion

        #region Methods

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (_handle)
            {
                _handle = false;

                SetChart();

                _handle = true;
            }
        }

        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_handle)
            {
                _handle = false;

                drawing = ResizeChart();

                _handle = true;
            }
        }

        private void SetChart()
        {
            cs.Xmin = this.Xmin;
            cs.Xmax = this.Xmax;
            cs.Ymin = this.Ymin;
            cs.Ymax = this.Ymax;
            cs.Y2min = this.Y2min;
            cs.Y2max = this.Y2max;
            cs.Y2Tick = this.Y2Tick;
            cs.XTick = this.XTick;
            cs.YTick = this.YTick;
            cs.XLabel = this.XLabel;
            cs.YLabel = this.YLabel;
            cs.YLabels = (string[])this.YLabels;
            cs.XLabels = (string[])this.XLabels;
            cs.Title = this.Title;
            cs.IsXGrid = this.IsXGrid;
            cs.IsXLog = this.IsXLog;
            cs.IsYGrid = this.IsYGrid;
            cs.IsYLog = this.IsYLog;
            cs.GridlineColor = this.GridlineColor;
            cs.GridlinePattern = this.GridlinePattern;
            cs.IsBubbleChart = this.IsBubbleChart;
            cs.ShowConvexHull = this.ShowConvexHull;
            lg.IsLegend = this.IsLegend;
            lg.LegendPosition = this.LegendPosition;

            drawing = ResizeChart();
        }

        private async Task ResizeChart()
        {

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsDrawing, async () =>
             {
                 this.Dispatcher.Invoke(() =>
                 {
                    //Filtering data based on the selection
                    try
                     {
                         chartCanvas.Children.Clear();
                         textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
                         cs.AddChartStyle();

                         if (DataCollection != null)
                         {
                             if (DataCollection.Count > 0)
                             {
                                 cs.SetLinesControl(new BindableCollection<LineSeries>(DataCollection));

                                 if (!IsBubbleChart)
                                     lg.AddLegend(chartCanvas, new BindableCollection<LineSeries>(DataCollection));
                                 else
                                 {
                                     lg.AddLegend(chartCanvas, Unit);
                                     lg.AddColorBar(textCanvas, ColorMap);
                                 }
                             }
                         }
                     }
                     catch
                     {
                     }
                     finally
                     {
                     }
                 });
             });
        }

        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as LineChartControl;
            var dc = e.NewValue as BindableCollection<LineSeries>;

            if (dc != null) dc.CollectionChanged += lc.dc_CollectionChanged;
        }

        private void dc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (DataCollection != null)
            {
                CheckCount = 0;
                if (DataCollection.Count > 0)
                    CheckCount = DataCollection.Count;
            }
        }
        #endregion

        /// <summary>
        /// Exporting the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|EMF (*.emf)|*.emf|PDF (*.pdf)|*.pdf|XAML (*.xaml)|*.xaml";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                //Getting the extension
                var ext = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                try
                {
                    //Getting the extension
                    var ext1 = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                    switch (ext1.ToString())
                    {
                        case ".png":
                            //Downloading to the specific folder
                            ImageCapturer.SaveToPng((FrameworkElement)this, saveFileDialog1.FileName);
                            break;
                        case ".bmp":
                            ImageCapturer.SaveToBmp((FrameworkElement)this, saveFileDialog1.FileName);
                            break;
                        case ".emf":
                            ImageCapturer.SaveToEmf(textCanvas, saveFileDialog1.FileName);
                            break;
                        case ".xaml":
                            ImageCapturer.SaveToXaml(textCanvas, saveFileDialog1.FileName);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
                }
            }
        }

        /// <summary>
        /// Switching axes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            double xmin = Xmin;
            double ymin = Ymin;
            double xmax = Xmax;
            double ymax = Ymax;
            double xtick = XTick;
            double ytick = YTick;

            Xmin = ymin;
            Xmax = ymax;
            Ymin = xmin;
            Ymax = xmax;
            XTick = ytick;
            YTick = xtick;


            string lab = XLabel.ToString();
            XLabel = YLabel.ToString();
            YLabel = lab.ToString();

            foreach (LineSeries d in DataCollection)
            {
                d.LinePoints = new BindableCollection<Point>(d.LinePoints.Select(x => new Point(x.Y, x.X)).ToList());
            }

            DataCollection.UpdateChart();
        }

        /// <summary>
        /// Zooming into the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {

            if (IsBubbleChart)
                return;

            double delta = e.Delta;

            Point a = Mouse.GetPosition(chartCanvas);
            Point max = cs.NormalizePoint(new Point(Xmax, Ymax));
            Point min = cs.NormalizePoint(new Point(Xmin, Ymin));

            double diffX = Math.Round((cs.Xmax - cs.Xmin) / 12, 15);
            double diffY = Math.Round((cs.Ymax - cs.Ymin) / 12, 15);

            double posRelativeX = 0;
            double posRelativeY = 0;

            try
            {
                posRelativeX = Math.Abs(max.X - a.X) / Math.Abs(min.X - a.X);
                posRelativeY = Math.Abs(max.Y - a.Y) / Math.Abs(min.Y - a.Y);
            }
            catch
            {

            }

            if (delta > 0)
            {
                Xmax -= diffX * (IsXLog ? Math.Pow(10, 1 / posRelativeX) : posRelativeX);
                Xmin += diffX / (IsXLog ? diffX : posRelativeX) * (IsXLog ? Xmin / 8 : 1);

                Ymax -= diffY * (IsYLog ? Math.Pow(10, 1 / posRelativeY) : posRelativeY);
                Ymin += diffY / (IsYLog ? diffY : posRelativeY) * (IsYLog ? Ymin / 8 : 1);

            }
            else
            {
                Xmax += diffX * (IsXLog ? Math.Pow(10, 1 / posRelativeX) : posRelativeX);
                Xmin -= diffX / (IsXLog ? diffX : posRelativeX) * (IsXLog ? Xmin / 8 : 1);

                Ymax += diffY * (IsYLog ? Math.Pow(10, 1 / posRelativeY) : posRelativeY);
                Ymin -= diffY / (IsYLog ? diffY : posRelativeY) * (IsYLog ? Ymin / 8 : 1);
            }

            drawing = ResizeChart();
        }

        private void chartGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsBubbleChart)
                return;

            if (!(e.LeftButton == MouseButtonState.Pressed))
                return;

            Point currentPosition = Mouse.GetPosition(chartCanvas);

            if (previousPosition.X == 0 && previousPosition.Y == 0)
            {
                previousPosition = currentPosition;
                return;
            }

            double deltaDirectionX = previousPosition.X - currentPosition.X;
            double deltaDirectionY = previousPosition.Y - currentPosition.Y;

            double diffX = Math.Round((cs.Xmax - cs.Xmin) / 8, 15);
            double diffY = Math.Round((cs.Ymax - cs.Ymin) / 8, 15);


            if (deltaDirectionX > 0 && Math.Abs(deltaDirectionX) > 10)
            {
                Xmax += IsXLog ? Xmax / 8 : diffX;
                Xmin += IsXLog ? Xmin / 4 : diffX;
            }
            else if (Math.Abs(deltaDirectionX) > 10)
            {
                Xmax -= IsXLog ? Xmax / 8 : diffX;
                Xmin -= IsXLog ? Xmin / 4 : diffX;
            }

            if (deltaDirectionY < 0 && Math.Abs(deltaDirectionY) > 10)
            {
                Ymax += IsYLog ? Ymax / 8 : diffY;
                Ymin += IsYLog ? Ymin / 4 : diffY;
            }
            else if (Math.Abs(deltaDirectionY) > 10)
            {
                Ymax -= IsYLog ? Ymax / 8 : diffY;
                Ymin -= IsYLog ? Ymin / 4 : diffY;
            }

            previousPosition = currentPosition;

            drawing = ResizeChart();
        }
    }
}
