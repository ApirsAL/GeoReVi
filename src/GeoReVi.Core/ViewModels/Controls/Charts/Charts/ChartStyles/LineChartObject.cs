using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace GeoReVi
{
    [Serializable]
    public class LineChartObject : ChartObject<LineSeries>, INotifyPropertyChanged
    {
        #region Public properties

        /// <summary>
        /// The selected property for the x axis
        /// </summary>
        private SelectedPropertyEnum xProperty = SelectedPropertyEnum.XAxis;
        public SelectedPropertyEnum XProperty
        {
            get => this.xProperty;
            set
            {
                this.xProperty = value;
                NotifyOfPropertyChange(() => XProperty);
            }
        }

        /// <summary>
        /// The selected property for the y axis
        /// </summary>
        private SelectedPropertyEnum yProperty = SelectedPropertyEnum.YAxis;
        public SelectedPropertyEnum YProperty
        {
            get => this.yProperty;
            set
            {
                this.yProperty = value;
                NotifyOfPropertyChange(() => YProperty);
            }
        }

        /// <summary>
        /// The selected property for the z axis
        /// </summary>
        private SelectedPropertyEnum zProperty = SelectedPropertyEnum.ZAxis;
        public SelectedPropertyEnum ZProperty
        {
            get => this.zProperty;
            set
            {
                this.zProperty = value;
                NotifyOfPropertyChange(() => ZProperty);
            }
        }

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

            Ds = _lco.Ds;
            FillColor = _lco.FillColor;

            Maxx = _lco.Maxx;
            Maxy = _lco.Maxy;
            Minx = _lco.Minx;
            Miny = _lco.Miny;

            BarType = _lco.BarType;

            DataCollection = _lco.DataCollection;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineChartObject()
        {
            DataCollection = new BindableCollection<LineSeries>();
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
            Ds.Add(i);
        }

        //Initializing a standard 2D scatter plot
        public void InitializeStandardScatterplot()
        {
            Title = "";
            YLabel.Text = "Y";
            XLabel.Text = "X";
            DataCollection = null;
            DataCollection = new BindableCollection<LineSeries>();
            Ds = null;
            Ds = new BindableCollection<LineSeries>();
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
        /// Creating a line chart
        /// </summary>
        public void CreateLineChart()
        {

            if (!ShallRender)
                return;

            Initialize();
            InitializeStandardScatterplot();

            int i = 0;

            foreach (var meas in DataSet)
            {
                try
                {
                    AddDataSeries();

                    Ds[i].SeriesName = meas.Name;
                }
                catch
                {
                }

                i = Ds.Count();
            }

            if (Ds.Count() != 0)
                CreateChart();
        }

        /// <summary>
        /// Adding a series of points to the chart
        /// </summary>
        /// <param name="pointSeries"></param>
        public virtual async Task CreateChart()
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
                foreach (Mesh mesh in DataSet)
                {
                    await Task.Delay(0);
                    Ds[i].LinePoints.Clear();

                    List<LocationTimeValue> points = new List<LocationTimeValue>();

                    //Adding all points from the line series to the chart
                    for (int j = 0; j < mesh.Vertices.Count(); j++)
                    {
                        await Task.Delay(0);
                        double x = 0;
                        double y = 0;
                        double z = 0;

                        //Assigning the property for the x axis
                        switch (XProperty)
                        {
                            case SelectedPropertyEnum.XAxis:
                                x = (double)mesh.Vertices[j].X;
                                break;
                            case SelectedPropertyEnum.YAxis:
                                x = (double)mesh.Vertices[j].Y;
                                break;
                            case SelectedPropertyEnum.ZAxis:
                                x = (double)mesh.Vertices[j].Z;
                                break;
                            default:
                                try
                                {
                                    x = (double)mesh.Vertices[j].Value[(int)Enum.Parse(typeof(SelectedPropertyEnum), Enum.GetName(typeof(SelectedPropertyEnum), XProperty))];
                                }
                                catch
                                {
                                    throw new Exception("Property not available.");
                                }
                                break;
                        }

                        //Assigning the property for the y axis
                        switch (YProperty)
                        {
                            case SelectedPropertyEnum.XAxis:
                                y = (double)mesh.Vertices[j].X;
                                break;
                            case SelectedPropertyEnum.YAxis:
                                y = (double)mesh.Vertices[j].Y;
                                break;
                            case SelectedPropertyEnum.ZAxis:
                                y = (double)mesh.Vertices[j].Z;
                                break;
                            default:
                                try
                                {
                                    y = (double)mesh.Vertices[j].Value[(int)Enum.Parse(typeof(SelectedPropertyEnum), Enum.GetName(typeof(SelectedPropertyEnum), YProperty))];
                                }
                                catch
                                {
                                    throw new Exception("Property not available.");
                                }
                                break;
                        }

                        //Assigning the property for the colormap
                        switch (ZProperty)
                        {
                            case SelectedPropertyEnum.XAxis:
                                z = (double)mesh.Vertices[j].X;
                                break;
                            case SelectedPropertyEnum.YAxis:
                                z = (double)mesh.Vertices[j].Y;
                                break;
                            case SelectedPropertyEnum.ZAxis:
                                z = (double)mesh.Vertices[j].Z;
                                break;
                            default:
                                try
                                {
                                    z = (double)mesh.Vertices[j].Value[(int)Enum.Parse(typeof(SelectedPropertyEnum), Enum.GetName(typeof(SelectedPropertyEnum), ZProperty))];
                                }
                                catch
                                {
                                    throw new Exception("Property not available.");
                                }
                                break;
                        }

                        LocationTimeValue a = new LocationTimeValue(x, y, z, mesh.Vertices[j].Name);

                        if (a.X < Xmin || a.X > Xmax)
                            continue;

                        if (a.Y < Ymin || a.Y > Ymax)
                            continue;

                        if (IsColorMap)
                            a.Value = new List<double>() { z };

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

                    if (ShowConvexHull)
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
                for (int i = 0; i < DataCollection.Count(); i++)
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

            double dy = (ColorMap.Ymax - ColorMap.Ymin) / (ColorMap.ColormapLength - 1);

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
                        Legend.LegendObjects[i].Label.Text = Legend.LegendObjects[i].Label.Text + " (" + DataCollection[i].RegressionHelper.Function + ") R^2 = " + DataCollection[i].RegressionHelper.RSquare.ToString().Substring(0, 5);
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
                double minX = DataCollection.SelectMany(x => x.LinePoints.Select(y => DeNormalizePoint(y).X)).Min();
                double minY = DataCollection.SelectMany(x => x.LinePoints.Select(y => DeNormalizePoint(y).Y)).Min();
                double maxX = DataCollection.SelectMany(x => x.LinePoints.Select(y => DeNormalizePoint(y).X)).Max();
                double maxY = DataCollection.SelectMany(x => x.LinePoints.Select(y => DeNormalizePoint(y).Y)).Max();

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
        }


        #endregion
    }
}
