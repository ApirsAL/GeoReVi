using Caliburn.Micro;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für MatrixChartControl.xaml
    /// </summary>
    public partial class MatrixChartControl : UserControl
    {
        #region Private members

        private ChartStyle cs;
        private Legend lg;

        #endregion

        #region Public properties

        //Xmin property as dependency property
        public static DependencyProperty XminProperty = DependencyProperty.Register("Xmin", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmin { get { return (double)GetValue(XminProperty); } set { SetValue(XminProperty, value); } }

        //Xmax property as dependency property
        public static DependencyProperty XmaxProperty = DependencyProperty.Register("Xmax", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmax { get { return (double)GetValue(XmaxProperty); } set { SetValue(XmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty YminProperty = DependencyProperty.Register("Ymin", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymin { get { return (double)GetValue(YminProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty YmaxProperty = DependencyProperty.Register("Ymax", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymax { get { return (double)GetValue(YmaxProperty); } set { SetValue(YmaxProperty, value); } }

        //XTick property as dependency property
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set { SetValue(XTickProperty, value); }
        }

        //YTick property as dependency property
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double), typeof(MatrixChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double YTick { get { return (double)GetValue(YTickProperty); } set { SetValue(YTickProperty, value); } }

        //XLabel property as dependency property
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(MatrixChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(MatrixChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(MatrixChartControl), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool), typeof(MatrixChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set { SetValue(IsXGridProperty, value); }
        }
        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool), typeof(MatrixChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //IsYGrid property as dependency property
        public bool IsYGrid { get { return (bool)GetValue(IsYGridProperty); } set { SetValue(IsYGridProperty, value); } }
        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(MatrixChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //GridlineColor property as dependency property
        public Brush GridlineColor { get { return (Brush)GetValue(GridlineColorProperty); } set { SetValue(GridlineColorProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(MatrixChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //LinePatternEnum property as dependency property
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(MatrixChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // IsLegend property as dependency property
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }
        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(MatrixChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        //Data collection property as dependency property
        public static readonly DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(BindableCollection<MatrixSeries>), typeof(MatrixChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));

        //Data collection for the chart control
        public BindableCollection<MatrixSeries> DataCollection
        {
            get
            {
                return (BindableCollection<MatrixSeries>)GetValue(DataCollectionProperty);
            }
            set
            {
                SetValue(DataCollectionProperty, value);
            }
        }

        //Check Count property as dependency property
        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(MatrixChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        private int CheckCount { get { return (int)GetValue(CheckCountProperty); } set { SetValue(CheckCountProperty, value); } }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MatrixChartControl()
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
            SetChart();
        }

        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeChart();
        }

        private void SetChart()
        {
            cs.Xmin = this.Xmin;
            cs.Xmax = this.Xmax;
            cs.Ymin = this.Ymin;
            cs.Ymax = this.Ymax;
            cs.XTick = this.XTick;
            cs.YTick = this.YTick;
            cs.XLabel = this.XLabel;
            cs.YLabel = this.YLabel;
            cs.Title = this.Title;
            cs.IsXGrid = this.IsXGrid;
            cs.IsYGrid = this.IsYGrid;
            cs.GridlineColor = this.GridlineColor;
            cs.GridlinePattern = this.GridlinePattern;
            lg.IsLegend = this.IsLegend;
            lg.LegendPosition = this.LegendPosition;

            ResizeChart();
        }

        private void ResizeChart()
        {
            chartCanvas.Children.Clear();
            textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
            cs.AddChartStyle();

            if (DataCollection != null)
            {
                if (DataCollection.Count > 0)
                {
                    cs.Set2DMatrixControl(DataCollection);
                }
            }
        }

        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as MatrixChartControl;
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
    }
}
