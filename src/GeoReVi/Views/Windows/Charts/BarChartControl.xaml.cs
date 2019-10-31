using Caliburn.Micro;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für BarChartControl.xaml
    /// </summary>
    public partial class BarChartControl : UserControl
    {
        #region Private members

        private BarChartStyle bcs;
        private Legend lg;

        private Task drawing { get; set; }

        #endregion

        #region Public properties

        //Xmin property as dependency property
        public static DependencyProperty XminProperty = DependencyProperty.Register("Xmin", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmin { get { return (double)GetValue(XminProperty); } set { SetValue(XminProperty, value); } }

        //Xmax property as dependency property
        public static DependencyProperty XmaxProperty = DependencyProperty.Register("Xmax", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmax { get { return (double)GetValue(XmaxProperty); } set { SetValue(XmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty YminProperty = DependencyProperty.Register("Ymin", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymin { get { return (double)GetValue(YminProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty YmaxProperty = DependencyProperty.Register("Ymax", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymax { get { return (double)GetValue(YmaxProperty); } set { SetValue(YmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty Y2minProperty = DependencyProperty.Register("Y2min", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2min { get { return (double)GetValue(Y2minProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty Y2maxProperty = DependencyProperty.Register("Y2max", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2max { get { return (double)GetValue(Y2maxProperty); } set { SetValue(YmaxProperty, value); } }


        //XTick property as dependency property
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set { SetValue(XTickProperty, value); }
        }

        //YTick property as dependency property
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double YTick { get { return (double)GetValue(YTickProperty); } set { SetValue(YTickProperty, value); } }

        //YTick property as dependency property
        public static DependencyProperty Y2TickProperty = DependencyProperty.Register("Y2Tick", typeof(double), typeof(BarChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Y2Tick { get { return (double)GetValue(Y2TickProperty); } set { SetValue(Y2TickProperty, value); } }

        //XLabel property as dependency property
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(BarChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(BarChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BarChartControl), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool), typeof(BarChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set { SetValue(IsXGridProperty, value); }
        }
        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool), typeof(BarChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //IsYGrid property as dependency property
        public bool IsYGrid { get { return (bool)GetValue(IsYGridProperty); } set { SetValue(IsYGridProperty, value); } }
        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(BarChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //GridlineColor property as dependency property
        public Brush GridlineColor { get { return (Brush)GetValue(GridlineColorProperty); } set { SetValue(GridlineColorProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(BarChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //LinePatternEnum property as dependency property
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(BarChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // IsLegend property as dependency property
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }
        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(BarChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }


        //Data collection property as dependency property
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(BindableCollection<BarSeries>), typeof(BarChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));
        //Data collection for the chart control
        public BindableCollection<BarSeries> DataCollection
        {
            get
            {
                return (BindableCollection<BarSeries>)GetValue(DataCollectionProperty);
            }
            set
            {
                SetValue(DataCollectionProperty, value);
            }
        }

        //LinePatternEnum property as dependency property
        public static DependencyProperty BarTypeProperty = DependencyProperty.Register("BarType", typeof(BarTypeEnum), typeof(BarChartControl), new FrameworkPropertyMetadata(BarTypeEnum.Vertical, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public BarTypeEnum BarType { get { return (BarTypeEnum)GetValue(BarTypeProperty); } set { SetValue(BarTypeProperty, value); } }

        //Polygon collection property as dependency property
        public static DependencyProperty PolygonCollectionProperty = DependencyProperty.Register("PolygonCollection", typeof(BindableCollection<BarPolygon>), typeof(BarChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public BindableCollection<BarPolygon> PolygonCollection { get { return (BindableCollection<BarPolygon>)GetValue(PolygonCollectionProperty); } set { SetValue(PolygonCollectionProperty, value); } }

        //Check Count property as dependency property
        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(BarChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        private int CheckCount { get { return (int)GetValue(CheckCountProperty); } set { SetValue(CheckCountProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty IsDrawingProperty = DependencyProperty.Register("IsDrawing", typeof(bool), typeof(BarChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public BarChartControl()
        {
            InitializeComponent();
            this.bcs = new BarChartStyle();
            this.lg = new Legend();
            bcs.TextCanvas = textCanvas;
            bcs.ChartCanvas = chartCanvas;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SetChart();
        }

        private async void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            drawing = ResizeChart();
        }

        private void SetChart()
        {
            bcs.Xmin = this.Xmin;
            bcs.Xmax = this.Xmax;
            bcs.Ymin = this.Ymin;
            bcs.Ymax = this.Ymax;
            bcs.Y2max = this.Y2max;
            bcs.Y2min = this.Y2min;
            bcs.Y2Tick = this.Y2Tick;
            bcs.XTick = this.XTick;
            bcs.YTick = this.YTick;
            bcs.XLabel = this.XLabel;
            bcs.YLabel = this.YLabel;
            bcs.Title = this.Title;
            bcs.IsXGrid = this.IsXGrid;
            bcs.IsYGrid = this.IsYGrid;
            bcs.GridlineColor = this.GridlineColor;
            bcs.GridlinePattern = this.GridlinePattern;
            bcs.BarType = this.BarType;
            lg.IsLegend = this.IsLegend;
            lg.LegendPosition = this.LegendPosition;

            drawing = ResizeChart();
        }

        //Resizing the chart dependend on the new format of the control 
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
                        drawingBackground.Visibility = Visibility.Visible;
                        chartCanvas.Children.Clear();

                        textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);

                        bcs.AddChartStyle();

                        if (DataCollection != null)
                        {
                            if (DataCollection.Count > 0)
                            {

                                bcs.SetBars(new BindableCollection<BarSeries>(DataCollection));
                                bcs.SetBarsControl();
                                lg.AddLegend(chartCanvas, DataCollection);
                            }
                        }
                    }
                    catch
                    {

                    }
                    finally
                    {
                        drawingBackground.Visibility = Visibility.Hidden;
                    }
                });
            });
        }

        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as BarChartControl;
            var dc = e.NewValue as BindableCollection<BarSeries>;

            if (dc != null)
                dc.CollectionChanged += lc.dc_CollectionChanged;
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|EMF (*.emf)|*.emf|PDF (*.pdf)|*.pdf";
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
                    }
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                }
            }
        }
    }
}
