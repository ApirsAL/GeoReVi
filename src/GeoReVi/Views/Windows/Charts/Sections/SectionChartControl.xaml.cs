using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für SectionChartControl.xaml
    /// </summary>
    public partial class SectionChartControl : UserControl
    {
        #region Private members

        private BarChartStyle bcs;
        private Legend lg;

        private Task drawing { get; set; }

        #endregion

        #region Public properties

        //Xmin property as dependency property
        public static DependencyProperty XminProperty = DependencyProperty.Register("Xmin", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmin { get { return (double)GetValue(XminProperty); } set { SetValue(XminProperty, value); } }

        //Xmax property as dependency property
        public static DependencyProperty XmaxProperty = DependencyProperty.Register("Xmax", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmax { get { return (double)GetValue(XmaxProperty); } set { SetValue(XmaxProperty, value); } }

        //Ymin property as dependency property
        public static DependencyProperty YminProperty = DependencyProperty.Register("Ymin", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymin { get { return (double)GetValue(YminProperty); } set { SetValue(YminProperty, value); } }

        //Ymax property as dependency property
        public static DependencyProperty YmaxProperty = DependencyProperty.Register("Ymax", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymax { get { return (double)GetValue(YmaxProperty); } set { SetValue(YmaxProperty, value); } }

        //XTick property as dependency property
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double XTick
        {
            get { return (double)GetValue(XTickProperty); }
            set { SetValue(XTickProperty, value); }
        }

        //YTick property as dependency property
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double), typeof(SectionChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double YTick { get { return (double)GetValue(YTickProperty); } set { SetValue(YTickProperty, value); } }

        //XLabel property as dependency property
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(SectionChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }

        //Label property as dependency property
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(SectionChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(SectionChartControl), new FrameworkPropertyMetadata("My Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }

        //Title property as dependency property
        public static DependencyProperty MetaInformationProperty = DependencyProperty.Register("MetaInformation", typeof(string), typeof(SectionChartControl), new FrameworkPropertyMetadata("Coordinates", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string MetaInformation { get { return (string)GetValue(MetaInformationProperty); } set { SetValue(MetaInformationProperty, value); } }


        //IsXGrid property as dependency property
        public static DependencyProperty IsXGridProperty = DependencyProperty.Register("IsXGrid", typeof(bool), typeof(SectionChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsXGrid
        {
            get { return (bool)GetValue(IsXGridProperty); }
            set { SetValue(IsXGridProperty, value); }
        }
        public static DependencyProperty IsYGridProperty = DependencyProperty.Register("IsYGrid", typeof(bool), typeof(SectionChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //IsYGrid property as dependency property
        public bool IsYGrid { get { return (bool)GetValue(IsYGridProperty); } set { SetValue(IsYGridProperty, value); } }
        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(SectionChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //GridlineColor property as dependency property
        public Brush GridlineColor { get { return (Brush)GetValue(GridlineColorProperty); } set { SetValue(GridlineColorProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(SectionChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //LinePatternEnum property as dependency property
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(SectionChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // IsLegend property as dependency property
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }
        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(SectionChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        //Section Data collection property as dependency property
        public static readonly DependencyProperty SectionDataCollectionProperty = DependencyProperty.Register("SectionDataCollection", typeof(BindableCollection<SectionLayerSeries>), typeof(SectionChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));
        //Section Data collection for the chart control
        public BindableCollection<SectionLayerSeries> SectionDataCollection
        {
            get
            {
                return (BindableCollection<SectionLayerSeries>)GetValue(SectionDataCollectionProperty);
            }
            set
            {
                SetValue(SectionDataCollectionProperty, value);
            }
        }

        //Section Data collection property as dependency property
        public static readonly DependencyProperty LogDataCollectionProperty = DependencyProperty.Register("LogDataCollection", typeof(BindableCollection<LogLineSeries>), typeof(SectionChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnLogDataChanged));
        //Section Data collection for the chart control
        public BindableCollection<LogLineSeries> LogDataCollection
        {
            get
            {
                return (BindableCollection<LogLineSeries>)GetValue(LogDataCollectionProperty);
            }
            set
            {
                SetValue(LogDataCollectionProperty, value);
            }
        }

        //LinePatternEnum property as dependency property
        public static DependencyProperty BarTypeProperty = DependencyProperty.Register("BarType", typeof(BarTypeEnum), typeof(SectionChartControl), new FrameworkPropertyMetadata(BarTypeEnum.Vertical, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public BarTypeEnum BarType { get { return (BarTypeEnum)GetValue(BarTypeProperty); } set { SetValue(BarTypeProperty, value); } }

        //LinePatternEnum property as dependency property
        public static DependencyProperty PolygonCollectionProperty = DependencyProperty.Register("PolygonCollection", typeof(BindableCollection<BarPolygon>), typeof(SectionChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public BindableCollection<BarPolygon> PolygonCollection { get { return (BindableCollection<BarPolygon>)GetValue(PolygonCollectionProperty); } set { SetValue(PolygonCollectionProperty, value); } }

        //Check Count property as dependency property
        private static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(SectionChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        private int CheckCount { get { return (int)GetValue(CheckCountProperty); } set { SetValue(CheckCountProperty, value); } }

        //IsXGrid property as dependency property
        public static DependencyProperty ShallRenderProperty = DependencyProperty.Register("ShallRender", typeof(bool), typeof(SectionChartControl), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool ShallRender
        {
            get { return (bool)GetValue(ShallRenderProperty); }
            set { SetValue(ShallRenderProperty, value); }
        }

        //IsXGrid property as dependency property
        public static DependencyProperty IsDrawingProperty = DependencyProperty.Register("IsDrawing", typeof(bool), typeof(SectionChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
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
        public SectionChartControl()
        {
            InitializeComponent();
            this.bcs = new BarChartStyle();
            this.lg = new Legend();
            bcs.TextCanvas = textCanvas;
            bcs.ChartCanvas = chartCanvas;
        }

        #endregion

        #region Methods

        //Event that is fired, when the control renders
        protected override void OnRender(DrawingContext drawingContext)
        {
            SetChart();
        }

        //Event that is fired when the size of the chart changed
        private void chartGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            drawing = ResizeChart();
        }

        //Assigning the chart properties
        private void SetChart()
        {
            bcs.Xmin = this.Xmin;
            bcs.Xmax = this.Xmax;
            bcs.Ymin = this.Ymin;
            bcs.Ymax = this.Ymax;
            bcs.XTick = this.XTick;
            bcs.YTick = this.YTick;
            bcs.XLabel = this.XLabel;
            bcs.YLabel = this.YLabel;
            bcs.Title = this.Title;
            bcs.MetaInformation = this.MetaInformation;
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

                        if(LogDataCollection != null)
                            bcs.AddSectionChartStyle(LogDataCollection.Count);
                        else
                            bcs.AddSectionChartStyle(0);


                        if (SectionDataCollection != null)
                        {
                            if (SectionDataCollection.Count > 0)
                            {
                                bcs.SetBars(SectionDataCollection);
                                bcs.SetBarsControl();
                                if (IsLegend)
                                    lg.AddLegend(chartCanvas, SectionDataCollection);
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

        //Event that is fired when data collection changed
        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as SectionChartControl;
            var dc = e.NewValue as BindableCollection<SectionLayerSeries>;

            if (dc != null) dc.CollectionChanged += lc.dc_CollectionChanged;
        }

        //Event that is fired when data collection changed
        private static void OnLogDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as SectionChartControl;
            var dc = e.NewValue as BindableCollection<LineSeries>;

            if (dc != null) dc.CollectionChanged += lc.dc_CollectionChanged;
        }

        //Event that is fired when data collection changed
        private void dc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SectionDataCollection != null && LogDataCollection != null)
            {
                CheckCount = 0;
                if (SectionDataCollection.Count > 0 || LogDataCollection.Count > 0)
                    CheckCount = SectionDataCollection.Count + LogDataCollection.Count;
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
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
                }
            }
        }
    }
}
