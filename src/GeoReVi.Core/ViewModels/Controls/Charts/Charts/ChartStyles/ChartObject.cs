using Caliburn.Micro;
using System.Windows.Media;
using System.Windows;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Linq;

namespace GeoReVi
{
    public abstract class ChartObject<T> : PropertyChangedBase where T : class, new()
    {
        #region Private members

        //Title of the plot
        private string title = "Scatterplot";
        //X-axis label
        private Label xLabel = new Label() { Text = "X Axis" };
        private BindableCollection<Label> xLabels = new BindableCollection<Label>();
        //Y-axis label
        private Label yLabel = new Label() { Text = "Y Axis" };
        private BindableCollection<Label> yLabels = new BindableCollection<Label>();
        //is x grid bool
        private bool isXGrid = true;
        private bool isXLog = false;
        //is y axis grid bool
        private bool isYGrid = true;
        private bool isYLog = false;


        private double chartWidth = 300;
        private double chartHeight = 300;

        private double xTick = 1;
        private double yTick = 0.5;

        private BarTypeEnum barType;
        private double xmin = 0;
        private double xmax = 10;
        private double ymin = 0;
        private double y2min = -1;
        private double y2tick = 0.5;
        private double ymax = 10;
        private double y2max = 1;
        private bool updating = false;
        private bool shallRender = true;

        #endregion

        #region Public properties

        /// <summary>
        /// Legend of the chart
        /// </summary>
        private Legend legend = new Legend();
        public Legend Legend
        {
            get => this.legend;
            set
            {
                this.legend = value;
                NotifyOfPropertyChange(() => Legend);
            }
        }

        /// <summary>
        /// X Gridlines of the chart object
        /// </summary>
        private BindableCollection<Gridline> xgridlines = new BindableCollection<Gridline>();
        public BindableCollection<Gridline> XGridlines
        {
            get => this.xgridlines;
            set
            {
                this.xgridlines = value;
                NotifyOfPropertyChange(() => XGridlines);
            }
        }

        /// <summary>
        /// Y Gridlines of the chart object
        /// </summary>
        private BindableCollection<Gridline> ygridlines = new BindableCollection<Gridline>();
        public BindableCollection<Gridline> YGridlines
        {
            get => this.ygridlines;
            set
            {
                this.ygridlines = value;
                NotifyOfPropertyChange(() => YGridlines);
            }
        }

        /// <summary>
        /// X ticks of the chart object
        /// </summary>
        private BindableCollection<Gridline> xticks = new BindableCollection<Gridline>();
        public BindableCollection<Gridline> XTicks
        {
            get => this.xticks;
            set
            {
                this.xticks = value;
                NotifyOfPropertyChange(() => XTicks);
            }
        }

        /// <summary>
        /// Y ticks of the chart object
        /// </summary>
        private BindableCollection<Gridline> yticks = new BindableCollection<Gridline>();
        public BindableCollection<Gridline> YTicks
        {
            get => this.yticks;
            set
            {
                this.yticks = value;
                NotifyOfPropertyChange(() => YTicks);
            }
        }

        //Title of the chart
        public string Title
        {
            get => this.title;
            set
            {
                title = value;

                NotifyOfPropertyChange(() => Title);
            }
        }

        /// <summary>
        /// X label of the chart
        /// </summary>
        public Label XLabel
        {
            get =>  this.xLabel;
            set
            {
                xLabel = value;

                NotifyOfPropertyChange(() => XLabel);
            }
        }

        /// <summary>
        /// X labels
        /// </summary>
        public BindableCollection<Label> XLabels
        {
            get => this.xLabels;
            set
            {
                xLabels = value;

                NotifyOfPropertyChange(() => XLabels);
            }
        }

        /// <summary>
        /// Y label text
        /// </summary>
        public Label YLabel
        {
            get
            {
                return yLabel;
            }
            set
            {
                yLabel = value;

                NotifyOfPropertyChange(() => YLabel);
            }
        }

        /// <summary>
        /// Y labels
        /// </summary>
        public BindableCollection<Label> YLabels
        {
            get
            {
                return yLabels;
            }
            set
            {
                yLabels = value;

                NotifyOfPropertyChange(() => YLabels);
            }
        }

        /// <summary>
        /// Font size of the labels
        /// </summary>
        private double labelFontSize = (double)new FontSizeConverter().ConvertFrom("5pt") ;
        public double LabelFontSize
        {
            get => this.labelFontSize;
            set
            {
                this.labelFontSize = value;

                NotifyOfPropertyChange(() => LabelFontSize);

            }
        }

        private FontFamily labelFont = new FontFamily("Arial Narrow");
        [XmlIgnore()]
        public FontFamily LabelFont
        {
            get => labelFont;
            set
            {
                this.labelFont = value;

                NotifyOfPropertyChange(() => LabelFont);
            }
        }

        /// <summary>
        /// Label brush
        /// </summary>
        private Brush labelColor = Brushes.Black;
        [XmlIgnore()]
        public Brush LabelColor
        {
            get => labelColor;
            set
            {
                this.labelColor = value;
                NotifyOfPropertyChange(() => LabelColor);

            }
        }


        #region Gridline

        /// <summary>
        /// Gridline pattern
        /// </summary>
        private LinePatternEnum gridlinePattern = LinePatternEnum.Solid;
        [XmlIgnore]
        public LinePatternEnum GridlinePattern
        {
            get => this.gridlinePattern;
            set
            {
                gridlinePattern = value;

                NotifyOfPropertyChange(() => GridlinePattern);
            }
        }

        /// <summary>
        /// Gridline color
        /// </summary>
        private Brush gridlineColor = Brushes.LightGray;
        [XmlIgnore]
        public Brush GridlineColor
        {
            get => gridlineColor;
            set
            {
                gridlineColor = value;

                NotifyOfPropertyChange(() => GridlineColor);
            }
        }

        /// <summary>
        /// Font type of the ticks
        /// </summary>
        private FontFamily tickFont = new FontFamily("Arial Narrow");
        [XmlIgnore()]
        public FontFamily TickFont
        {
            get { return tickFont; }
            set
            {
                tickFont = value;

                NotifyOfPropertyChange(() => TickFont);

            }
        }

        /// <summary>
        /// Color of the chart ticks
        /// </summary>
        private Brush tickColor = Brushes.Black;
        [XmlIgnore()]
        public Brush TickColor
        {
            get { return tickColor; }
            set
            {
                tickColor = value;

                NotifyOfPropertyChange(() => TickColor);
            }
        }


        /// <summary>
        /// Font size of the ticks
        /// </summary>
        private double tickFontSize = 10;
        public double TickFontSize
        {
            get { return tickFontSize; }
            set
            {
                tickFontSize = value;

                NotifyOfPropertyChange(() => TickFontSize);
            }
        }

        #endregion

        /// <summary>
        /// Bar type
        /// </summary>
        [XmlIgnore]
        public BarTypeEnum BarType
        {
            get => this.barType;
            set
            {
                this.barType = value;


                NotifyOfPropertyChange(() => BarType);

            }
        }

        /// <summary>
        /// Tick step width x direction
        /// </summary>
        public double XTick
        {
            get => this.xTick;
            set
            {
                xTick = value;

                NotifyOfPropertyChange(() => XTick);
            }
        }

        /// <summary>
        /// Tick step width x direction
        /// </summary>
        public double YTick
        {
            get
            {
                return yTick;
            }
            set
            {
                yTick = value;

                NotifyOfPropertyChange(() => YTick);
            }
        }



        /// <summary>
        /// Check if x is grid
        /// </summary>
        public bool IsXGrid
        {
            get => isXGrid;
            set
            {
                isXGrid = value;

                NotifyOfPropertyChange(() => IsXGrid);
            }
        }

        /// <summary>
        /// Check if x axis is logarithmic
        /// </summary>
        public bool IsXLog { get { return isXLog; } set { isXLog = value; NotifyOfPropertyChange(() => IsXLog); } }

        /// <summary>
        /// Check if y is grid
        /// </summary>
        public bool IsYGrid
        {
            get
            {
                return isYGrid;
            }
            set
            {
                isYGrid = value;

                NotifyOfPropertyChange(() => IsYGrid);
            }
        }

        /// <summary>
        /// Check if y is logarithmic
        /// </summary>
        public bool IsYLog { get { return isYLog; } set { isYLog = value; NotifyOfPropertyChange(() => IsYLog); } }


        /// <summary>
        /// Minimum x axis value
        /// </summary>
        public double Xmin
        {
            get
            {
                return this.xmin;
            }
            set
            {
                this.xmin = value;

                NotifyOfPropertyChange(() => Xmin);

            }
        }


        /// <summary>
        /// Maximum x axis value
        /// </summary>
        public double Xmax
        {
            get
            {
                return this.xmax;
            }
            set
            {
                this.xmax = value;
                NotifyOfPropertyChange(() => Xmax);
            }
        }


        /// <summary>
        /// Minimum y axis value
        /// </summary>
        public double Ymin
        {
            get
            {
                return this.ymin;
            }
            set
            {
                this.ymin = value;

                NotifyOfPropertyChange(() => Ymin);
            }
        }


        /// <summary>
        /// Minimum y2 axis value
        /// </summary>
        public double Y2min
        {
            get
            {
                return this.y2min;
            }
            set
            {
                this.y2min = value;

                NotifyOfPropertyChange(() => Y2min);
            }
        }


        /// <summary>
        /// Y2 axis tick value
        /// </summary>
        public double Y2Tick
        {
            get
            {
                return this.y2tick;
            }
            set
            {
                this.y2tick = value;

                NotifyOfPropertyChange(() => Y2Tick);
            }
        }

        /// <summary>
        /// Maximum y value
        /// </summary>
        public double Ymax
        {
            get
            {
                return this.ymax;
            }
            set
            {
                this.ymax = value;

                NotifyOfPropertyChange(() => Ymax);
            }
        }

        /// <summary>
        /// Maximum y2 value
        /// </summary>
        public double Y2max
        {
            get
            {
                return this.y2max;
            }
            set
            {
                this.y2max = value;

                NotifyOfPropertyChange(() => Y2max);
            }
        }

        /// <summary>
        /// Checks if the chart is updating
        /// </summary>
        public bool Updating
        {
            get => this.updating;
            set
            {
                this.updating = value;
                NotifyOfPropertyChange(() => Updating);
            }
        }

        /// <summary>
        /// The chart height
        /// </summary>
        public double ChartHeight
        {
            get
            {
                return this.chartHeight;
            }
            set
            {
                this.chartHeight = value;

                NotifyOfPropertyChange(() => ChartHeight);
            }
        }

        /// <summary>
        /// Width of the chart object
        /// </summary>
        public double ChartWidth
        {
            get
            {
                return this.chartWidth;
            }
            set
            {
                this.chartWidth = value;

                if (DataCollection != null)
                    if(DataCollection.Count != 0)
                        UpdateChart();
                    

                NotifyOfPropertyChange(() => ChartWidth);
            }
        }

        /// <summary>
        /// Checks if the chart should be rendered
        /// </summary>
        public bool ShallRender
        {
            get => this.shallRender;
            set
            {
                this.shallRender = value;

                NotifyOfPropertyChange(() => ChartWidth);
            }
        }

        /// <summary>
        /// Chart data collection
        /// </summary>
        private BindableCollection<T> dataCollection;
        public BindableCollection<T> DataCollection
        {
            get => this.dataCollection;
            set
            {
                this.dataCollection = value;
                if(DataCollection != null)
                    NotifyOfPropertyChange(() => DataCollection);
            }
        }

        /// <summary>
        /// The selected series
        /// </summary>
        private T selectedSeries;
        public T SelectedSeries
        {
            get => this.selectedSeries;
            set
            {
                this.selectedSeries = value;
                NotifyOfPropertyChange(() => SelectedSeries);
            }
        }

        /// <summary>
        /// Data set containing the relevant data for the chart object
        /// </summary>
        private BindableCollection<Mesh> dataSet = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> DataSet
        {
            get => this.dataSet;
            set
            {
                this.dataSet = value;
                NotifyOfPropertyChange(() => DataSet);
            }
        }

        /// <summary>
        /// The selected chart series
        /// </summary>
        private Mesh selectedDataSet = new Mesh();
        public Mesh SelectedDataSet
        {
            get
            {
                return this.selectedDataSet;
            }
            set
            {
                this.selectedDataSet = value;
                NotifyOfPropertyChange(() => SelectedDataSet);
            }
        }

        #endregion

        #region Methods

        //Initializing
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// Updates the chart object
        /// </summary>
        public virtual async Task UpdateChart()
        {
            await Task.Delay(0);

            if (Updating)
                return;

            if (typeof(T) != typeof(Series3D) && typeof(T) != typeof(LineSeries))
                DataCollection.UpdateChart();
        }

        /// <summary>
        /// Adds a legend to the chart
        /// </summary>
        public virtual void AddLegend()
        {
            try
            {
                //Getting the longest in the legend
                double xMax = DataCollection.Select(x => MeasureString(x.GetType().GetProperty("SeriesName").GetValue(x, null).ToString())).Select(x => x.Width).Max();
                double height = DataCollection.Count() * 15.0;

                Legend.LegendObjects.Clear();

                //Placing the legend
                switch (Legend.LegendPosition)
                {
                    case LegendPositionEnum.East:
                        Legend.X = ChartWidth;
                        Legend.Y = ChartHeight / 2;
                        break;
                    case LegendPositionEnum.North:
                        Legend.X = ChartWidth / 2;
                        Legend.Y = 0;
                        break;
                    case LegendPositionEnum.South:
                        Legend.X = ChartWidth / 2;
                        Legend.Y = ChartHeight - height;
                        break;
                    case LegendPositionEnum.West:
                        Legend.X = 0;
                        Legend.Y = ChartHeight / 2 - xMax;
                        break;
                    case LegendPositionEnum.NorthEast:
                        Legend.X = ChartWidth;
                        Legend.Y = 0;
                        break;
                    case LegendPositionEnum.NorthWest:
                        Legend.X = 0;
                        Legend.Y = 0;
                        break;
                    case LegendPositionEnum.SouthEast:
                        Legend.X = ChartWidth;
                        Legend.Y = ChartHeight - height;
                        break;
                    case LegendPositionEnum.SouthWest:
                        Legend.X = 0;
                        Legend.Y = ChartHeight - height;
                        break;
                }

                //Adding labels
                for (int i = 0; i < DataCollection.Count; i++)
                {
                    Legend.LegendObjects.Add(new LegendObject()
                    {
                        Label = new Label()
                        {
                            Text = DataCollection[i].GetType().GetProperty("SeriesName").GetValue(DataCollection[i], null).ToString(),
                            X = Legend.X + 15,
                            Y = Legend.Y + 15 * i
                        },
                        Rectangle = new Rectangle2D()
                        {
                            X = Legend.X,
                            Y = Legend.Y + 15*i
                        }
                    });

                    xMax = MeasureString(Legend.LegendObjects[i].Label.Text).Width > xMax ? MeasureString(Legend.LegendObjects[i].Label.Text).Width : xMax;
                }

            }
            catch
            {

            }
        }

        /// <summary>
        /// Adds labels to the chart
        /// </summary>
        public virtual void AddGridlines()
        {
            //Step width
            double dx, dy;

            //New gridline object
            Gridline gridline = new Gridline();

            //Clearing gridline collection
            XGridlines.Clear();
            YGridlines.Clear();

            if ((Xmax - Xmin) / XTick > 100)
                XTick = (Xmax - Xmin) / 10;

            if ((Ymax - Ymin) / YTick > 100)
                YTick = (Ymax - Ymin) / 10;

            try
            {
                // Create vertical gridlines: 
                if (IsYGrid == true)
                {
                    if (!IsXLog)
                        for (dx = Xmin + XTick; dx < Xmax; dx += XTick)
                        {
                            gridline = new Gridline();
                            gridline.X1 = NormalizePoint(new LocationTimeValue(dx, Ymin)).X;
                            gridline.Y1 = NormalizePoint(new LocationTimeValue(dx, Ymin)).Y;
                            gridline.X2 = NormalizePoint(new LocationTimeValue(dx, Ymax)).X;
                            gridline.Y2 = NormalizePoint(new LocationTimeValue(dx, Ymax)).Y;
                            XGridlines.Add(gridline);
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

                            gridline = new Gridline();
                            gridline.X1 = NormalizePoint(new LocationTimeValue(dx, Ymin)).X;
                            gridline.Y1 = NormalizePoint(new LocationTimeValue(dx, Ymin)).Y;
                            gridline.X2 = NormalizePoint(new LocationTimeValue(dx, Ymax)).X;
                            gridline.Y2 = NormalizePoint(new LocationTimeValue(dx, Ymax)).Y;
                            XGridlines.Add(gridline);
                        }

                    }
                }

                // Create horizontal gridlines: 
                if (IsXGrid == true)
                {
                    if (!IsYLog)
                        for (dy = Ymin + YTick; dy < Ymax; dy += YTick)
                        {
                            gridline = new Gridline();
                            gridline.X1 = NormalizePoint(new LocationTimeValue(Xmin, dy)).X;
                            gridline.Y1 = NormalizePoint(new LocationTimeValue(Xmin, dy)).Y;
                            gridline.X2 = NormalizePoint(new LocationTimeValue(Xmax, dy)).X;
                            gridline.Y2 = NormalizePoint(new LocationTimeValue(Xmax, dy)).Y;
                            YGridlines.Add(gridline);
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

                            gridline = new Gridline();
                            gridline.X1 = NormalizePoint(new LocationTimeValue(Xmin, dy)).X;
                            gridline.Y1 = NormalizePoint(new LocationTimeValue(Xmin, dy)).Y;
                            gridline.X2 = NormalizePoint(new LocationTimeValue(Xmax, dy)).X;
                            gridline.Y2 = NormalizePoint(new LocationTimeValue(Xmax, dy)).Y;
                            YGridlines.Add(gridline);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Adds ticks to the chart object
        /// </summary>
        public virtual void AddTicksAndLabels()
        {

            //Step width
            double dx, dy;
            Gridline tick = new Gridline();

            XLabels.Clear();
            XTicks.Clear();
            YLabels.Clear();
            YTicks.Clear();

            if ((Xmax - Xmin) / XTick > 100)
                XTick = (Xmax - Xmin) / 10;

            if ((Ymax - Ymin) / YTick > 100)
                YTick = (Ymax - Ymin) / 10;

            Size xlabelSize = MeasureString(XLabel.Text);
            Size ylabelSize = MeasureString(YLabel.Text);

            try
            {
                //Meta data for the placement of the controls
                int i = 0;
                double width = 0;
                double maxWidth = 0;

                // Create y-axis tick marks: 
                if (!IsYLog)
                {
                    for (dy = Ymin; dy <= Ymax; dy += YTick)
                    {
                        if (YTick == 0)
                            YTick += 1;

                        //pt = NormalizePoint(new Point(Xmin, dy));
                        LocationTimeValue pt = NormalizePoint(new LocationTimeValue(Xmin, dy));
                        tick = new Gridline();
                        tick.X1 = pt.X;
                        tick.Y1 = pt.Y;
                        tick.X2 = pt.X + 5;
                        tick.Y2 = pt.Y;

                        YTicks.Add(tick);

                        width = 3 * MeasureString(dy.ToString()).Width;

                        if (width > maxWidth)
                        {
                            maxWidth = width;

                            foreach (Label l in YLabels)
                                l.X = -1 * maxWidth;

                            YLabel.X = -0.5 * maxWidth;
                        }

                        Label tb = new Label()
                        {
                            Text = dy.ToString(),
                            X = -1*maxWidth,
                            Y = pt.Y - MeasureString(Math.Round(dy, 2).ToString()).Height
                        };

                        if (dy == Ymin)
                        {
                            YLabel.X = -1 * maxWidth;
                            YLabel.Y = ChartHeight / 2 - MeasureString(YLabel.Text).Height / 2 - 3.5 * xlabelSize.Height;
                        }

                        YLabels.Add(tb);

                        i++;

                    }
                }
                else
                {
                    double a = Ymin == 0 ? 0.1 : Ymin;
                    for (dy = a; dy <= Ymax; dy += 9 * dy)
                    {
                        if (YTick == 0)
                            YTick += 1;

                        if (a == 0)
                            break;

                        LocationTimeValue pt = NormalizePoint(new LocationTimeValue(Xmin, dy));
                        tick = new Gridline();

                        tick.X1 = pt.X;
                        tick.Y1 = pt.Y;
                        tick.X2 = pt.X + 5;
                        tick.Y2 = pt.Y;

                       YTicks.Add(tick);

                        width = 3 * MeasureString(dy.ToString()).Width;

                        if (width > maxWidth)
                        {
                            maxWidth = width;

                            foreach (Label l in YLabels)
                                l.X = -3 * maxWidth;

                            YLabel.X = -1 * maxWidth;
                        }

                        Label tb = new Label()
                        {
                            Text = dy.ToString(),
                            X = - 2*maxWidth,
                            Y = pt.Y - MeasureString(dy.ToString()).Height
                        };

                        if(dy == Ymin)
                        {
                            YLabel.X = -1 * maxWidth;
                            YLabel.Y = ChartHeight / 2 - MeasureString(YLabel.Text).Height / 2;
                        }

                        YLabels.Add(tb);

                    }
                }

                i = 0;

                // Create x-axis tick marks: 
                if (!IsXLog)
                    for (dx = Xmin; dx <= Xmax; dx += xTick)
                    {
                        if (xTick == 0)
                            xTick += 1;

                        LocationTimeValue pt = NormalizePoint(new LocationTimeValue(dx, Ymin));
                        tick = new Gridline();
                        tick.X1 = pt.X;
                        tick.Y1 = pt.Y;
                        tick.X2 = pt.X;
                        tick.Y2 = pt.Y - 5;
                        XTicks.Add(tick);

                        Label tb = new Label()
                        {
                            Text = dx.ToString(),
                            X = pt.X - MeasureString(dx.ToString()).Width,
                            Y = pt.Y + MeasureString(dx.ToString()).Height
                        };

                        if (dx == Xmin)
                        {
                            XLabel.X = ChartWidth / 2 - xlabelSize.Width + maxWidth + 5;
                            XLabel.Y = ChartHeight + 3.5 * xlabelSize.Height;
                        }

                        if (tb.Text == "0" && dx != Xmin)
                            tb.Text = dx.ToString("E0");

                        XLabels.Add(tb);

                        i++;
                    }
                else
                {
                    double a = Xmin == 0 ? 0.1 : Xmin;
                    for (dx = a; dx <= Xmax; dx += 9 * dx)
                    {
                        if (a == 0)
                            break;

                        LocationTimeValue pt = NormalizePoint(new LocationTimeValue(dx, Ymin));
                        tick = new Gridline();
                        tick.X1 = pt.X;
                        tick.Y1 = pt.Y;
                        tick.X2 = pt.X;
                        tick.Y2 = pt.Y - 5;
                        XTicks.Add(tick);

                        Label tb = new Label()
                        {
                            Text = dx.ToString(),
                            X = pt.X - MeasureString(Math.Round(dx, 2).ToString()).Width - 5,
                            Y = pt.Y + MeasureString(Math.Round(dx, 2).ToString()).Height
                        };

                        if (dx == Xmin)
                        {
                            XLabel.X = ChartWidth / 2 - MeasureString(XLabel.Text).Width;
                            XLabel.Y = ChartHeight + 3.5 * MeasureString(XLabel.Text).Height;
                        }

                        if (tb.Text == "0" && dx != Xmin)
                            tb.Text = a.ToString("E0");

                        XLabels.Add(tb);

                        i++;
                    }
                }

            }
            catch
            {

            }

        }

        /// <summary>
        /// Removes the selected series from the data collection
        /// </summary>
        public virtual void RemoveSelectedSeries()
        {
            try
            {
                if (SelectedDataSet != null)
                {
                    int index = DataSet.IndexOf(SelectedDataSet);
                    DataSet.RemoveAt(index);
                    DataCollection.RemoveAt(index);
                }
            }
            catch
            {

            }
        }

        #endregion

        #region Helper methods


        //Normalizes a point information based on the relative width and height of the canvas object
        public LocationTimeValue NormalizePoint(LocationTimeValue pt)
        {
            if (Double.IsNaN(ChartWidth) || ChartHeight <= 0)
                ChartWidth = 270;

            if (Double.IsNaN(ChartHeight) || ChartHeight <= 0)
                ChartHeight = 250;

            LocationTimeValue result = new LocationTimeValue();

            if (IsXLog)
                result.X = (Math.Log10(pt.X) - Math.Log10(Xmin)) * ChartWidth / (Math.Log10(Xmax) - Math.Log10(Xmin));
            else
                result.X = (pt.X - Xmin) * ChartWidth / (Xmax - Xmin);
            if(IsYLog)
                result.Y = ChartHeight - (Math.Log10(pt.Y) - Math.Log10(Ymin)) * ChartHeight / (Math.Log10(Ymax) - Math.Log10(Ymin));
            else
                result.Y = ChartHeight - (pt.Y - Ymin) * ChartHeight / (Ymax - Ymin);

            if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
                return new LocationTimeValue() { X = 0, Y = 0 };

            return result;
        }

        //Normalizes a point information based on the relative width and height of the canvas object
        public LocationTimeValue DeNormalizePoint(LocationTimeValue pt)
        {
            LocationTimeValue result = new LocationTimeValue();

            if (IsXLog)
                result.X = (Math.Pow(10,pt.X) * (Math.Pow(10, Xmax) - Math.Pow(10, Xmin))) / ChartWidth + Math.Pow(10, Xmin);
            else
                result.X = (pt.X * (Xmax - Xmin)) / ChartWidth + Xmin;
            if (IsYLog)
                result.Y = -1 * (((Math.Pow(10, pt.Y) - ChartHeight) * (Math.Pow(10, Ymax) - Math.Pow(10, Ymin))) / ChartHeight + Math.Pow(10, Ymin));
            else
                result.Y = -1 * (((pt.Y - ChartHeight) * (Ymax - Ymin)) / ChartHeight + Ymin);

            if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
                return new LocationTimeValue() { X = 0, Y = 0 };

            return result;
        }

        /// <summary>
        /// Measure size of a string
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.LabelFont, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal),
                this.LabelFontSize,
                Brushes.Black,
                new NumberSubstitution());

            return new Size(formattedText.Width, formattedText.Height);
        }

        #endregion
    }
}
