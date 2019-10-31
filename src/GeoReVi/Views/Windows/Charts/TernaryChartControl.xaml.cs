using Caliburn.Micro;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für TernaryChartControl.xaml
    /// </summary>
    public partial class TernaryChartControl : UserControl
    {
        #region Private members

        private ChartStyle cs;
        private Legend lg;
        private bool _handle = true;

        private Task drawing { get; set; }

        #endregion

        #region Public properties

        //XLabel property as dependency property
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(TernaryChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(TernaryChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty ZLabelProperty = DependencyProperty.Register("ZLabel", typeof(string), typeof(TernaryChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string ZLabel { get { return (string)GetValue(ZLabelProperty); } set { SetValue(ZLabelProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(TernaryChartControl), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty IsGridProperty = DependencyProperty.Register("IsGrid", typeof(bool), typeof(TernaryChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsGrid
        {
            get { return (bool)GetValue(IsGridProperty); }
            set { SetValue(IsGridProperty, value); }
        }


        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(TernaryChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //GridlineColor property as dependency property
        public Brush GridlineColor { get { return (Brush)GetValue(GridlineColorProperty); } set { SetValue(GridlineColorProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(TernaryChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //LinePatternEnum property as dependency property
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(TernaryChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // IsLegend property as dependency property
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }
        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(TernaryChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        //GridlineColor property as dependency property
        public ColormapBrush ColorMap { get { return (ColormapBrush)GetValue(ColorMapProperty); } set { SetValue(ColorMapProperty, value); } }
        public static DependencyProperty ColorMapProperty = DependencyProperty.Register("ColorMap", typeof(ColormapBrush), typeof(TernaryChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        //Data collection property as dependency property
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(BindableCollection<LineSeries>), typeof(TernaryChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));

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
        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(TernaryChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        private int CheckCount { get { return (int)GetValue(CheckCountProperty); } set { SetValue(CheckCountProperty, value); } }

        //The unit displayed
        private static DependencyProperty UnitProperty = DependencyProperty.Register("Unit", typeof(string), typeof(TernaryChartControl), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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
        public static DependencyProperty ShowConvexHullProperty = DependencyProperty.Register("ShowConvexHull", typeof(bool), typeof(TernaryChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ShowConvexHull
        {
            get { return (bool)GetValue(ShowConvexHullProperty); }
            set { SetValue(ShowConvexHullProperty, value); }
        }

        //IsXGrid property as dependency property
        public static DependencyProperty IsDrawingProperty = DependencyProperty.Register("IsDrawing", typeof(bool), typeof(TernaryChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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
        public TernaryChartControl()
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
            cs.XLabel = this.XLabel;
            cs.YLabel = this.YLabel;
            cs.ZLabel = this.ZLabel;
            cs.Title = this.Title;
            cs.IsGrid = this.IsGrid;
            cs.GridlineColor = this.GridlineColor;
            cs.GridlinePattern = this.GridlinePattern;
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
                        drawingBackground.Visibility = Visibility.Visible;

                        chartCanvas.Children.Clear();
                        textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
                        cs.AddTernaryChartStyle();

                        if (DataCollection != null)
                        {
                            if (DataCollection.Count > 0)
                            {
                                cs.SetLinesControl(new BindableCollection<LineSeries>(DataCollection));

                                lg.AddLegend(chartCanvas, new BindableCollection<LineSeries>(DataCollection));
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
            var lc = sender as TernaryChartControl;
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
