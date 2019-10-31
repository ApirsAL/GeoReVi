using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GeoReVi
{
    /// <summary>
    /// Interaktionslogik für Line3D.xaml
    /// </summary>
    public partial class Line3DChartControl : UserControl
    {
        #region Private members

        private Chart3DChartStyle cs;
        private Legend lg;

        private bool _handle = false;

        private Task drawing { get; set; }

        private Point previousPosition = new Point(0, 0);
        List<UIElement> symbols = new List<UIElement>();

        CancellationTokenSource cts = new CancellationTokenSource();

        #endregion

        #region Dependency properties

        public static DependencyProperty ElevationProperty = DependencyProperty.Register("Elevation", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Elevation { get { return (double)GetValue(ElevationProperty); } set { SetValue(ElevationProperty, value); } }
        public static DependencyProperty AzimuthProperty = DependencyProperty.Register("Azimuth", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Azimuth { get { return (double)GetValue(AzimuthProperty); } set { SetValue(AzimuthProperty, value); } }
        public static DependencyProperty XminProperty = DependencyProperty.Register("Xmin", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmin { get { return (double)GetValue(XminProperty); } set { SetValue(XminProperty, value); } }
        public static DependencyProperty XmaxProperty = DependencyProperty.Register("Xmax", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Xmax { get { return (double)GetValue(XmaxProperty); } set { SetValue(XmaxProperty, value); } }
        public static DependencyProperty YminProperty = DependencyProperty.Register("Ymin", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymin { get { return (double)GetValue(YminProperty); } set { SetValue(YminProperty, value); } }
        public static DependencyProperty YmaxProperty = DependencyProperty.Register("Ymax", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Ymax { get { return (double)GetValue(YmaxProperty); } set { SetValue(YmaxProperty, value); } }
        public static DependencyProperty ZminProperty = DependencyProperty.Register("Zmin", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Zmin { get { return (double)GetValue(ZminProperty); } set { SetValue(ZminProperty, value); } }
        public static DependencyProperty ZmaxProperty = DependencyProperty.Register("Zmax", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double Zmax { get { return (double)GetValue(ZmaxProperty); } set { SetValue(ZmaxProperty, value); } }
        public static DependencyProperty XTickProperty = DependencyProperty.Register("XTick", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double XTick { get { return (double)GetValue(XTickProperty); } set { SetValue(XTickProperty, value); } }
        public static DependencyProperty YTickProperty = DependencyProperty.Register("YTick", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double YTick { get { return (double)GetValue(YTickProperty); } set { SetValue(YTickProperty, value); } }
        public static DependencyProperty ZTickProperty = DependencyProperty.Register("ZTick", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double ZTick { get { return (double)GetValue(ZTickProperty); } set { SetValue(ZTickProperty, value); } }
        public static DependencyProperty XLabelProperty = DependencyProperty.Register("XLabel", typeof(string), typeof(Line3DChartControl), new FrameworkPropertyMetadata("X Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string XLabel { get { return (string)GetValue(XLabelProperty); } set { SetValue(XLabelProperty, value); } }
        public static DependencyProperty YLabelProperty = DependencyProperty.Register("YLabel", typeof(string), typeof(Line3DChartControl), new FrameworkPropertyMetadata("Y Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string YLabel { get { return (string)GetValue(YLabelProperty); } set { SetValue(YLabelProperty, value); } }

        public static DependencyProperty ZLabelProperty = DependencyProperty.Register("ZLabel", typeof(string), typeof(Line3DChartControl), new FrameworkPropertyMetadata("Z Axis", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string ZLabel { get { return (string)GetValue(ZLabelProperty); } set { SetValue(ZLabelProperty, value); } }


        public static DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Line3DChartControl), new FrameworkPropertyMetadata("No Title", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
        public static DependencyProperty GridlineColorProperty = DependencyProperty.Register("GridlineColor", typeof(Brush), typeof(Line3DChartControl), new FrameworkPropertyMetadata(Brushes.Gray, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Brush GridlineColor
        {
            get
            {
                return (Brush)GetValue(GridlineColorProperty);
            }
            set
            {
                SetValue(GridlineColorProperty, value);
            }
        }

        public static DependencyProperty GridLineThicknessProperty = DependencyProperty.Register("GridLineThickness", typeof(double), typeof(Line3DChartControl), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public double GridLineThickness { get { return (double)GetValue(GridLineThicknessProperty); } set { SetValue(GridLineThicknessProperty, value); } }
        public static DependencyProperty GridlinePatternProperty = DependencyProperty.Register("GridlinePattern", typeof(LinePatternEnum), typeof(Line3DChartControl), new FrameworkPropertyMetadata(LinePatternEnum.Solid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public LinePatternEnum GridlinePattern { get { return (LinePatternEnum)GetValue(GridlinePatternProperty); } set { SetValue(GridlinePatternProperty, value); } }
        private static DependencyProperty DataCollectionProperty = DependencyProperty.Register("DataCollection", typeof(BindableCollection<LineSeries3D>), typeof(Line3DChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDataChanged));
        public BindableCollection<LineSeries3D> DataCollection { get { return (BindableCollection<LineSeries3D>)GetValue(DataCollectionProperty); } set { SetValue(DataCollectionProperty, value); } }

        //IsBubble property as dependency property
        public static DependencyProperty IsBubbleProperty = DependencyProperty.Register("IsBubble", typeof(bool), typeof(Line3DChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsBubble
        {
            get { return (bool)GetValue(IsBubbleProperty); }
            set { SetValue(IsBubbleProperty, value); }
        }
        //IsDrawing property as dependency property
        public static DependencyProperty IsDrawingProperty = DependencyProperty.Register("IsDrawing", typeof(bool), typeof(Line3DChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsDrawing
        {
            get { return (bool)GetValue(IsDrawingProperty); }
            set { SetValue(IsDrawingProperty, value); }
        }

        // IsLegend property as dependency property
        public static DependencyProperty IsLegendProperty = DependencyProperty.Register("IsLegend", typeof(bool), typeof(Line3DChartControl), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public bool IsLegend { get { return (bool)GetValue(IsLegendProperty); } set { SetValue(IsLegendProperty, value); } }

        public static DependencyProperty LegendPositionProperty = DependencyProperty.Register("LegendPosition", typeof(LegendPositionEnum), typeof(Line3DChartControl), new FrameworkPropertyMetadata(LegendPositionEnum.NorthEast, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        //Legend position property as dependency property
        public LegendPositionEnum LegendPosition
        {
            get { return (LegendPositionEnum)GetValue(LegendPositionProperty); }
            set { SetValue(LegendPositionProperty, value); }
        }

        //Colormap property as dependency property
        public ColormapBrush ColorMap { get { return (ColormapBrush)GetValue(ColorMapProperty); } set { SetValue(ColorMapProperty, value); } }
        public static DependencyProperty ColorMapProperty = DependencyProperty.Register("ColorMap", typeof(ColormapBrush), typeof(Line3DChartControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        public static DependencyProperty CheckCountProperty = DependencyProperty.Register("CheckCount", typeof(int), typeof(Line3DChartControl), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender));
        public int CheckCount
        {
            get
            {
                return (int)GetValue(CheckCountProperty);
            }
            set
            {
                SetValue(CheckCountProperty, value);
            }
        }

        #endregion

        #region Constructor

        public Line3DChartControl()
        {
            InitializeComponent();
            _handle = true;

            this.cs = new Chart3DChartStyle();
            this.lg = new Legend();

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
            drawing = ResizeChart();
        }

        private void SetChart()
        {
            cs.Elevation = this.Elevation;
            cs.Azimuth = this.Azimuth;
            cs.Xmin = this.Xmin;
            cs.Xmax = this.Xmax;
            cs.Ymin = this.Ymin;
            cs.Ymax = this.Ymax;
            cs.Zmin = this.Zmin;
            cs.Zmax = this.Zmax;
            cs.XTick = this.XTick;
            cs.YTick = this.YTick;
            cs.ZTick = this.ZTick;
            cs.XLabel = this.XLabel;
            cs.YLabel = this.YLabel;
            cs.ZLabel = this.ZLabel;
            cs.Title = this.Title;
            cs.IsBubbleChart = this.IsBubble;
            cs.GridlineColor = this.GridlineColor;
            cs.GridlinePattern = this.GridlinePattern;
            cs.GridlineThickness = this.GridLineThickness;
            lg.IsLegend = this.IsLegend;
            lg.LegendPosition = this.LegendPosition;

            drawing = ResizeChart();
        }

        private async Task ResizeChart()
        {
            cts = new CancellationTokenSource();
            var token = cts.Token;

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsDrawing, async () =>
            {
                //Filtering data based on the selection
                try
                {
                    await this.Dispatcher.InvokeAsync(async () =>
                    {
                        int count = 0;

                        count = chartCanvas.Children.Count;

                        chartCanvas.Children.RemoveRange(0, chartCanvas.Children.Count);

                        cs.SetChartStyle();

                        var dc = new BindableCollection<LineSeries3D>(DataCollection);

                        if (dc != null)
                        {
                            if (dc.Count > 0)
                            {
                                symbols.Clear();
                                ColormapBrush cm = new ColormapBrush(ColorMap);

                                for (int i = 0; i < dc.Count; i++)
                                {
                                    try
                                    {
                                         var symbolCollection = await cs.AddLine3D(dc[i], cm, token).ConfigureAwait(false);
                                         symbols.AddRange(symbolCollection);
                                    }
                                    catch
                                    {

                                    }
                                }

                                foreach (UIElement x in symbols)
                                {
                                    if (!cts.IsCancellationRequested)
                                        try
                                        {
                                            await Task.Delay(1);
                                            this.Dispatcher.Invoke(() => chartCanvas.Children.Add(x));

                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    else
                                    {
                                        symbols.Clear();
                                        break;
                                    }
                                }

                                this.Dispatcher.Invoke(() =>
                                {
                                    if (IsBubble)
                                        lg.AddColorBar(chartCanvas, cm);
                                });
                            }
                        }
                    });
                }
                catch
                {

                }
                finally
                {

                }
            });
        }

        private static void OnDataChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var lc = sender as Line3DChartControl;
            var dc = e.NewValue as BindableCollection<LineSeries3D>;
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


        #endregion

        /// <summary>
        /// Zooming into the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chartGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double delta = e.Delta;

            Point a = Mouse.GetPosition(chartCanvas);
            Point max = cs.NormalizePoint(new Point(Xmax, Ymax));
            Point min = cs.NormalizePoint(new Point(Xmin, Ymin));

            double diffX = Math.Round((cs.Xmax - cs.Xmin) / 12, 10);
            double diffY = Math.Round((cs.Ymax - cs.Ymin) / 12, 10);
            double diffZ = Math.Round((cs.Zmax - cs.Zmin) / 12, 10);

            double posRelativeX = 0;
            double posRelativeY = 0;
            double posRelativeZ = 0;

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
                Xmax -= diffX * posRelativeX;
                Xmin += diffX / posRelativeX;

                Ymax -= diffY * posRelativeY;
                Ymin += diffY / posRelativeY;

                Zmax -= diffY * posRelativeX;
                Zmin += diffY / posRelativeX;
            }
            else
            {
                Xmax += diffX * posRelativeX;
                Xmin -= diffX / posRelativeX;

                Ymax += diffY * posRelativeY;
                Ymin -= diffY / posRelativeY;

                Zmax += diffZ * posRelativeX;
                Zmin -= diffZ / posRelativeX;
            }

            SetChart();
        }

        /// <summary>
        /// Translating within the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void chartGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(e.LeftButton == MouseButtonState.Pressed))
                return;

            cts.Cancel();

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


            if (deltaDirectionX > 0 && Math.Abs(deltaDirectionX) > 6)
            {
                Azimuth += 8;
            }
            else if (Math.Abs(deltaDirectionX) > 6)
            {
                Azimuth -= 8;
            }

            if (deltaDirectionY < 0 && Math.Abs(deltaDirectionY) > 6)
            {
                Elevation += 8;
            }
            else if (Math.Abs(deltaDirectionY) > 6)
            {
                Elevation -= 8;
            }

            previousPosition = currentPosition;

            SetChart();
        }

        /// <summary>
        /// Exporting the chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                            ImageCapturer.SaveToEmf(chartCanvas, saveFileDialog1.FileName);
                            break;
                    }
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                }
            }
        }

        /// <summary>
        /// Rearranging the view to 30/30
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Azimuth = 30;
            Elevation = 30;
        }

        /// <summary>
        /// Cancelling the running task
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (cts != null)
                cts.Cancel();
        }
    }
}
