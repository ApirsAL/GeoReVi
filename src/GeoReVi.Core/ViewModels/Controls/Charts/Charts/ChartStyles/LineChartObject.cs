using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Xml.Serialization;

namespace GeoReVi
{
    [Serializable]
    public class LineChartObject : ChartObject<LineSeries>, INotifyPropertyChanged
    {
        #region Public properties

        /// <summary>
        /// Checks if the line chart should be a colormap
        /// </summary>
        private bool isColorMap = false;
        public bool IsColorMap
        {
            get => this.isColorMap;
            set
            {
                this.isColorMap = value;
                NotifyOfPropertyChange(() => IsColorMap);
            }
        }

        /// <summary>
        /// ColorMap
        /// </summary>
        private ColormapBrush colorMap = new ColormapBrush();
        public ColormapBrush ColorMap
        {
            get
            {
                return colorMap;
            }
            set
            {
                colorMap = value;
                NotifyOfPropertyChange(() => ColorMap);
            }
        }

        //Providing information about the direction
        private DirectionEnum direction = DirectionEnum.Directionless;
        [XmlIgnore]
        public DirectionEnum Direction
        {
            get => this.direction;
            set
            {
                this.direction = value;

                switch (value)
                {
                    case DirectionEnum.X:
                        YLabel.Text = "x-direction [m]";
                        break;
                    case DirectionEnum.Y:
                        YLabel.Text = "y-direction [m]";
                        break;
                    case DirectionEnum.Z:
                        YLabel.Text = "z-direction [m]";
                        break;
                    case DirectionEnum.XY:
                        YLabel.Text = "y-direction [m]";
                        XLabel.Text = "x-direction [m]"; ;
                        break;
                    case DirectionEnum.XZ:
                        YLabel.Text = "z-direction [m]";
                        XLabel.Text = "x-direction [m]";
                        break;
                    case DirectionEnum.YZ:
                        YLabel.Text = "z-direction [m]";
                        XLabel.Text = "y-direction [m]";
                        break;
                }

                NotifyOfPropertyChange(() => Direction);
            }
        }


        /// <summary>
        /// Fill color of the line 
        /// </summary>
        [XmlIgnore()]
        private Brush fillColor;
        public Brush FillColor
        {
            get => this.fillColor;
            set
            {
                this.fillColor = value;
                NotifyOfPropertyChange(() => FillColor);
            }
        }

        /// <summary>
        /// A list of the data series of the chart
        /// </summary>
        private BindableCollection<LineSeries> ds = new BindableCollection<LineSeries>();
        public BindableCollection<LineSeries> Ds
        {
            get
            {
                return this.ds;
            }
            set
            {
                this.ds = value;
                NotifyOfPropertyChange(() => Ds);
            }
        }

        /// <summary>
        /// Determines if a convex hull around the points is shown or not
        /// </summary>
        private bool showConvexHull = false;
        public bool ShowConvexHull
        {
            get => this.showConvexHull;
            set
            {
                this.showConvexHull = value;
                NotifyOfPropertyChange(() => ShowConvexHull);
            }
        }

        /// <summary>
        /// Point series for the line series
        /// </summary>
        private ObservableCollection<ObservableCollection<LocationTimeValue>> spatialPointSeries = new ObservableCollection<ObservableCollection<LocationTimeValue>>();
        public ObservableCollection<ObservableCollection<LocationTimeValue>> SpatialPointSeries
        {
            get => this.spatialPointSeries;
            set
            {
                this.spatialPointSeries = value;
                NotifyOfPropertyChange(() => SpatialPointSeries);
            }
        }

        /// <summary>
        /// Shows or hides the regression line
        /// </summary>
        private bool showRegression = false;
        public bool ShowRegression
        {
            get => this.showRegression;
            set
            {
                this.showRegression = value;
                NotifyOfPropertyChange(() => ShowRegression);
            }
        }

        #region Meta properties

        private double maxx;
        public double Maxx
        {
            get => this.maxx;
            set => maxx = value;
        }

        private double minx;
        public double Minx
        {
            get => this.minx;
            set => minx = value;
        }

        private double maxy;
        public double Maxy
        {
            get => this.maxy;
            set => maxy = value;
        }

        private double miny;
        public double Miny
        {
            get => this.miny;
            set => miny = value;
        }

        #endregion


        #endregion

        #region Constructor

        public LineChartObject(LineChartObject _lco)
        {
            DataCollection = new BindableCollection<LineSeries>();
            SpatialPointSeries = new ObservableCollection<ObservableCollection<LocationTimeValue>>();

            DataSet = _lco.DataSet;
            Title = _lco.Title;
            GridlineColor = _lco.GridlineColor;
            GridlinePattern = _lco.GridlinePattern;
            Legend.IsLegend = _lco.Legend.IsLegend;
            Legend.LegendPosition = _lco.Legend.LegendPosition;
            ShallRender = _lco.ShallRender;

            ChartHeight = _lco.ChartHeight;
            ChartWidth = _lco.ChartWidth;

            Y2max = _lco.Y2max;
            Y2min = _lco.Y2min;
            Y2Tick = _lco.Y2Tick;

            Ymax = _lco.Ymax;
            Ymin = _lco.Ymin;
            YTick = _lco.YTick;
            YLabel = _lco.YLabel;
            YLabels = _lco.YLabels;
            IsYLog = _lco.IsYLog;
            IsYGrid = _lco.IsYGrid;

            XLabel = _lco.XLabel;
            XLabels = _lco.XLabels;
            Xmax = _lco.Xmax;
            Xmin = _lco.Xmin;
            XTick = _lco.XTick;
            IsXLog = _lco.IsXLog;
            IsXGrid = _lco.IsXGrid;

            Direction = _lco.Direction;
            Ds = _lco.Ds;
            FillColor = _lco.FillColor;

            Maxx = _lco.Maxx;
            Maxy = _lco.Maxy;
            Minx = _lco.Minx;
            Miny = _lco.Miny;
            SpatialPointSeries = _lco.SpatialPointSeries;

            BarType = _lco.BarType;

            DataCollection = _lco.DataCollection;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineChartObject()
        {
            DataCollection = new BindableCollection<LineSeries>();
            SpatialPointSeries = new ObservableCollection<ObservableCollection<LocationTimeValue>>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adding a data series to the histogram object
        /// </summary>
        public virtual void AddDataSeries()
        {
            LineSeries i = new LineSeries(DataCollection);
            i.Symbols.FillColor = System.Windows.Media.Brushes.DarkBlue;
            i.Symbols.SymbolSize = 6;
            i.Symbols.SymbolType = SymbolTypeEnum.Dot;
            i.Symbols.BorderThickness = 0;

            switch (Direction)
            {
                case DirectionEnum.X:
                    YLabel.Text = "x-direction [m]";
                    break;
                case DirectionEnum.Y:
                    YLabel.Text = "y-direction [m]";
                    break;
                case DirectionEnum.Z:
                    YLabel.Text = "z-direction [m]";
                    break;
                case DirectionEnum.XY:
                    YLabel.Text = "y-direction [m]";
                    XLabel.Text = "x-direction [m]"; ;
                    break;
                case DirectionEnum.XZ:
                    YLabel.Text = "z-direction [m]";
                    XLabel.Text = "x-direction [m]";
                    break;
                case DirectionEnum.YZ:
                    YLabel.Text = "z-direction [m]";
                    XLabel.Text = "y-direction [m]";
                    break;
            }

            Ds.Add(i);
        }

        //Initializing a standard 2D scatter plot
        public void InitializeStandardScatterplot()
        {
            Title = "";
            DataCollection = null;
            DataCollection = new BindableCollection<LineSeries>();
            Ds = null;
            Ds = new BindableCollection<LineSeries>();
        }
        //Initializing a standard spatial log
        public void InitializeStandardSpatialLog()
        {
            Title = "";
            DataCollection = null;
            DataCollection = new BindableCollection<LineSeries>();
            Ds = null;
            Ds = new BindableCollection<LineSeries>();
            SpatialPointSeries.Clear();
        }

        public override void Initialize()
        {
            Maxx = 0;
            Minx = 0;
            Maxy = 0;
            Miny = 0;
        }

        /// <summary>
        /// Updating the chart
        /// </summary>
        public override async Task UpdateChart()
        {
            await Task.Delay(200);
            CreateChart();
            await base.UpdateChart();
        }

        /// <summary>
        /// Creating a scatter chart from the data set
        /// </summary>
        public void CreateMatrixChart()
        {
            try
            {
                if (!ShallRender)
                    return;

                Direction = DirectionEnum.XY;

                Initialize();
                InitializeStandardSpatialLog();

                SpatialPointSeries = new ObservableCollection<ObservableCollection<LocationTimeValue>>();
                int i = 0;

                foreach (var meas in DataSet)
                {
                    SpatialPointSeries.Add(new ObservableCollection<LocationTimeValue>());

                    try
                    {
                        for(int j = 0; j<meas.Data.Rows.Count; j++)
                        {
                            for (int k = 0; k < meas.Data.Columns.Count; k++)
                                SpatialPointSeries[i].Add(new LocationTimeValue(k, j, 0, "", Convert.ToDouble(meas.Data.Rows[j][k])));
                        }

                        AddDataSeries();

                        Ds[i].SeriesName = meas.Name;
                    }
                    catch
                    {
                    }

                    i = Ds.Count();
                }

                if (SpatialPointSeries.Count() != 0)
                    CreateChart();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Creating a line chart
        /// </summary>
        public void CreateLineChart()
        {

            if (!ShallRender)
                return;

            Initialize();
            InitializeStandardSpatialLog();

            SpatialPointSeries = new ObservableCollection<ObservableCollection<LocationTimeValue>>();
            int i = 0;

            foreach (var meas in DataSet)
            {
                try
                {
                    SpatialPointSeries.Add(new ObservableCollection<LocationTimeValue>(meas.Data.AsEnumerable().OrderBy(x => x.Field<double>(1)).OrderBy(x => x.Field<double>(2)).OrderBy(x => x.Field<double>(3))
                        .Select(x => new LocationTimeValue()
                        {
                            Value = new List<double>() { x.Field<double>(0) },
                            X = x.Field<double>(1),
                            Y = x.Field<double>(2),
                            Z = x.Field<double>(3)
                        }).ToList()));

                    AddDataSeries();

                    Ds[i].SeriesName = meas.Name;
                }
                catch
                {
                }

                i = Ds.Count();
            }

            if (SpatialPointSeries.Count() != 0)
                CreateChart();
        }

        /// <summary>
        /// Adding a series of points to the chart
        /// </summary>
        /// <param name="pointSeries"></param>
        public virtual void CreateChart()
        {

            try
            {
                if (Updating)
                    return;

                int i = 0;

                if (IsColorMap)
                {
                    SetBrush(ColorMap.CalculateColormapBrushes());
                    SetColorMapLabels();

                }

                //Adding values from the 3D chart collection
                foreach (ObservableCollection<LocationTimeValue> ser in SpatialPointSeries)
                {
                    Ds[i].LinePoints.Clear();

                    int counter = 1;
                    if (ser.Count() > 1000)
                        counter = 2;

                    List<LocationTimeValue> points = new List<LocationTimeValue>();

                    //Adding all points from the line series to the chart
                    for (int j = 0; j < ser.Count(); j++)
                    {
                        LocationTimeValue a = new LocationTimeValue();

                        switch (Direction)
                        {
                            case DirectionEnum.X:
                                a = new LocationTimeValue((double)ser[j].Value[0], (double)ser[j].X, 0, ser[j].Name);
                                break;
                            case DirectionEnum.Y:
                                a = new LocationTimeValue((double)ser[j].Value[0], (double)ser[j].Y, 0, ser[j].Name);
                                break;
                            case DirectionEnum.Z:
                                a = new LocationTimeValue((double)ser[j].Value[0], (double)ser[j].Z, 0, ser[j].Name);
                                break;
                            case DirectionEnum.XY:
                                a = new LocationTimeValue((double)ser[j].X, (double)ser[j].Y, (double)ser[j].Value[0], ser[j].Name);
                                break;
                            case DirectionEnum.XZ:
                                a = new LocationTimeValue((double)ser[j].X, (double)ser[j].Z, (double)ser[j].Value[0], ser[j].Name);
                                break;
                            case DirectionEnum.YZ:
                                a = new LocationTimeValue((double)ser[j].Y, (double)ser[j].Z, (double)ser[j].Value[0], ser[j].Name);
                                break;
                            default:
                                a = new LocationTimeValue((double)ser[j].X, (double)ser[j].Value[0], (double)ser[j].Z, ser[j].Name);
                                break;
                        }

                        if (a.X < Xmin || a.X > Xmax)
                            continue;

                        if (a.Y < Ymin || a.Y > Ymax)
                            continue;

                        if (IsColorMap)
                            a.Value = ser[j].Value;

                        points.Add(new LocationTimeValue()
                        {
                            X = NormalizePoint(a).X - 0.5 * Ds[i].Symbols.SymbolSize,
                            Y = NormalizePoint(a).Y - 0.5 * Ds[i].Symbols.SymbolSize,
                            Brush = !IsColorMap
                                ? Ds[i].Symbols.FillColor
                                : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)a.Value[0]) : (double)a.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)
                        });

                    }

                    points.OrderBy(x => x.Y);

                    Ds[i].LinePoints.AddRange(points);

                    if (Ds[i].LinePoints.Count > 0)
                    {
                        Maxx = Ds[i].LinePoints.Max(x => DeNormalizePoint(x).X);
                        Minx = Ds[i].LinePoints.Min(x => DeNormalizePoint(x).X);
                        Maxy = Ds[i].LinePoints.Max(x => DeNormalizePoint(x).Y);
                        Miny = Ds[i].LinePoints.Min(x => DeNormalizePoint(x).Y);
                    }

                    if(ShowConvexHull)
                    {
                        Ds[i].ComputeConvexHull();
                    }

                    //Computing the regression
                    if (ShowRegression)
                    {
                        Ds[i].ComputeRegression();
                        Ds[i].RegressionHelper.FitPolynomials[0] = DeNormalizePoint(new LocationTimeValue(0, Ds[i].RegressionHelper.FitPolynomials[0], 0)).Y;
                        Ds[i].RegressionHelper.CreateFunction();
                    }
                    else
                        Ds[i].RegressionLinePoints = new PointCollection();

                    i += 1;
                }

                AddGridlines();
                AddTicksAndLabels();
            }
            catch
            {

            }

            DataCollection = Ds;

            try
            {
                for(int i = 0;i<DataCollection.Count();i++)
                {
                    DataCollection[i].LinePoints = new BindableCollection<LocationTimeValue>(DataCollection[i].LinePoints);
                }
            }
            catch
            {

            }
            AddLegend();

        }


        /// <summary>
        /// Sets the solidcolorbrush based on the calculated cmap values and an opacity value
        /// </summary>
        /// <param name="cmap"></param>
        /// <param name="opacity"></param>
        public void SetBrush(byte[,] cmap, double opacity = 1)
        {
            List<SolidColorBrush> brushes = new List<SolidColorBrush>();

            double dy = (ColorMap.Ymax - ColorMap.Ymin) / (ColorMap.Ydivisions - 1);

            for (int i = 0; i < ColorMap.Ydivisions; i++)
            {
                int colorIndex = (int)((ColorMap.ColormapLength - 1) * i * dy / (ColorMap.Ymax - ColorMap.Ymin));
                if (colorIndex < 0)
                    colorIndex = 0;
                if (colorIndex >= ColorMap.ColormapLength)
                    colorIndex = ColorMap.ColormapLength - 1;
                brushes.Add(new SolidColorBrush(Color.FromArgb(cmap[colorIndex, 0],
                                                                cmap[colorIndex, 1],
                                                                cmap[colorIndex, 2],
                                                                cmap[colorIndex, 3])));

                brushes[i].Opacity = opacity;
                brushes[i].Freeze();
            }

            double stepHeight = (Ymax - Ymin) / brushes.Count;
            List<Rectangle2D> rects = new List<Rectangle2D>();

            for (int i = 0; i < brushes.Count; i++)
            {
                var a = new LocationTimeValue(Xmax, stepHeight * i);

                Rectangle2D rect = new Rectangle2D()
                {
                    Brush = brushes[i],
                    X = NormalizePoint(a).X + 2,
                    Y = Math.Abs(NormalizePoint(a).Y),
                    Height = NormalizePoint(new LocationTimeValue(0, Ymax - stepHeight, 0)).Y + 1
                };

                rects.Add(rect);
            }

            ColorMap.ColormapBrushes = new ObservableCollection<Rectangle2D>(rects);

        }

        public void SetColorMapLabels()
        {
            try
            {
                ColorMap.Labels = new ObservableCollection<Label>();

                double step = (ColorMap.Ymax - ColorMap.Ymin) / ColorMap.LabelSubdivisions;
                double stepHeight = (Ymax - Ymin) / ColorMap.LabelSubdivisions;

                for (int i = 0; i < ColorMap.LabelSubdivisions + 1; i++)
                {
                    var a = new LocationTimeValue(Xmax, stepHeight * i);

                    Label tb = new Label()
                    {
                        Text = (ColorMap.Ymin + step * i).ToString(),
                        X = NormalizePoint(a).X + 24,
                        Y = Math.Abs(NormalizePoint(a).Y) - MeasureString(Math.Round(ColorMap.Ymin + step * i, 2).ToString()).Height
                    };

                    ColorMap.Labels.Add(tb);
                }
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Adding a legend to the chart
        /// </summary>
        public override void AddLegend()
        {
            base.AddLegend();

            try
            {
                for (int i = 0; i < DataCollection.Count(); i++)
                {
                    Legend.LegendObjects[i].Rectangle.Brush = (SolidColorBrush)DataCollection[i].Symbols.FillColor;
                    Legend.LegendObjects[i].Symbol = DataCollection[i].Symbols.SymbolType;

                    if (ShowRegression)
                        Legend.LegendObjects[i].Label.Text = Legend.LegendObjects[i].Label.Text + " (" + DataCollection[i].RegressionHelper.Function + ") R^2 = " + DataCollection[i].RegressionHelper.RSquare.ToString().Substring(0,5);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Subdivides the axes based on the min an max values of the data set
        /// </summary>
        public void SubdivideAxes()
        {

            try
            {
                double minX = SpatialPointSeries.SelectMany(x => x.Select(y => y.X)).Min();
                double minY = SpatialPointSeries.SelectMany(x => x.Select(y => y.Y)).Min();
                double maxX = SpatialPointSeries.SelectMany(x => x.Select(y => y.X)).Max();
                double maxY = SpatialPointSeries.SelectMany(x => x.Select(y => y.X)).Min();

                LocationTimeValue Mins = new LocationTimeValue(
                    minX,
                    minY);

                LocationTimeValue Maxs = new LocationTimeValue(
        maxX,
        maxY);

                Xmin = Mins.X;
                Xmax = Maxs.X;
                Ymin = Mins.Y;
                Ymax = Maxs.Y;

            }
            catch
            {

            }


            //local = Math.Round(DataCollection.Min(x => x.LinePoints.Min(y => y.X)), 1) == 0 ?
            //    (Math.Round(Minx, 2) == 0 ?
            //    (Math.Round(Minx, 3) == 0 ?
            //    (Math.Round(Minx, 4) == 0 ?
            //      Math.Round(Minx, 4)
            //    : Math.Round(Minx, 4))
            //    : Math.Round(Minx, 3))
            //    : Math.Round(Minx, 2))
            //    : Math.Round(Minx, 1);

            ////Math.Round(minx, 1);

            //if (local > 0 && local < Xmin) { Xmin = local - local * 0.1; }
            //else if (local < Xmin) { Xmin = local + local * 0.1; }

            //local = Math.Round(Maxx, 1) == 0 ?
            //    (Math.Round(Maxx, 2) == 0 ?
            //    (Math.Round(Maxx, 3) == 0 ?
            //    (Math.Round(Maxx, 4) == 0 ?
            //      Math.Round(Maxx, 4)
            //    : Math.Round(Maxx, 4))
            //    : Math.Round(Maxx, 3))
            //    : Math.Round(Maxx, 2))
            //    : Math.Round(Maxx, 1);

            ////Math.Round(Ds.LinePoints.Max(x => x.X), 1);

            //if (local > 0 && local > Xmax) { Xmax = local + local * 0.1; }
            //else if (local > Xmax) { Xmax = local - local * 0.1; }

            //local = Math.Round(Miny, 1) == 0 ?
            //    (Math.Round(Miny, 2) == 0 ?
            //    (Math.Round(Miny, 3) == 0 ?
            //    (Math.Round(Miny, 4) == 0 ?
            //      Math.Round(Miny, 4)
            //    : Math.Round(Miny, 4))
            //    : Math.Round(Miny, 3))
            //    : Math.Round(Miny, 2))
            //    : Math.Round(Miny, 1);

            ////Math.Round(Ds.LinePoints.Min(y => y.Y), 1);

            //if (local > 0 && local < Ymin) { Ymin = local - local * 0.1; }
            //else if (local < Ymin) { Ymin = local + local * 0.1; }

            //local = Math.Round(Maxy, 1) == 0 ?
            //    (Math.Round(Maxy, 2) == 0 ?
            //    (Math.Round(Maxy, 3) == 0 ?
            //    (Math.Round(Maxy, 4) == 0 ?
            //      Math.Round(Maxy, 4)
            //    : Math.Round(Maxy, 4))
            //    : Math.Round(Maxy, 3))
            //    : Math.Round(Maxy, 2))
            //    : Math.Round(Maxy, 1);

            //if (local > 0 && local > Ymax) { Ymax = local + local * 0.1; }
            //else if (local == 0 && local > Ymax)
            //    Ymax = 1;
            //else if (local > Ymax) { Ymax = local - local * 0.1; }

            //if (XTick == 0)
            //    XTick = Math.Round((Xmax - Xmin) / 10, 1) == 0 ?
            //        (Math.Round((Xmax - Xmin) / 10, 2) == 0 ?
            //        (Math.Round((Xmax - Xmin) / 10, 3) == 0 ?
            //        (Math.Round((Xmax - Xmin) / 10, 4) == 0 ?
            //          Math.Round((Xmax - Xmin) / 10, 4)
            //        : Math.Round((Xmax - Xmin) / 10, 4))
            //        : Math.Round((Xmax - Xmin) / 10, 3))
            //        : Math.Round((Xmax - Xmin) / 10, 2))
            //        : Math.Round((Xmax - Xmin) / 10, 1);

            //if (YTick == 0)
            //    YTick = Math.Round((Ymax - Ymin) / 10, 1) == 0 ?
            //        (Math.Round((Ymax - Ymin) / 10, 2) == 0 ?
            //        (Math.Round((Ymax - Ymin) / 10, 3) == 0 ?
            //        (Math.Round((Ymax - Ymin) / 10, 4) == 0 ?
            //          Math.Round((Ymax - Ymin) / 10, 4)
            //        : Math.Round((Ymax - Ymin) / 10, 4))
            //        : Math.Round((Ymax - Ymin) / 10, 3))
            //        : Math.Round((Ymax - Ymin) / 10, 2))
            //        : Math.Round((Ymax - Ymin) / 10, 1);
        }


        #endregion
    }
}
