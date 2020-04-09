using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media;

namespace GeoReVi
{
    public class BarChartObject : ChartObject<BarSeries>
    {
        /// <summary>
        /// Number of bins, the histogram is subdivided into
        /// </summary>
        private int numberBins = 10;
        public int NumberBins
        {
            get => numberBins;
            set
            {
                if (value > 0 && value < 50)
                {
                    numberBins = value;

                    if (DataCollection != null)
                        if (DataCollection.Count != 0)
                            CreateChart();
                }

                NotifyOfPropertyChange(() => NumberBins);
            }
        }

        private int maxCount;

        /// <summary>
        /// The value set for the histogram
        /// </summary>
        private List<double> allValues;
        public List<double> AllValues
        {
            get => this.allValues;
            set
            {
                this.allValues = value;
                NotifyOfPropertyChange(() => AllValues);
            }
        }

        /// <summary>
        /// A bar series object
        /// </summary>
        private List<BarSeries> bs;
        public List<BarSeries> Bs
        {
            get => this.bs;
            set
            {
                this.bs = value;
                NotifyOfPropertyChange(() => Bs);
            }
        }

        #region Constructor

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="_bco"></param>
        public BarChartObject(BarChartObject _bco)
        {
            DataSet = _bco.DataSet;
            Title = _bco.Title;
            GridlineColor = _bco.GridlineColor;
            GridlinePattern = _bco.GridlinePattern;
            Legend.IsLegend = _bco.Legend.IsLegend;
            Legend.LegendPosition = _bco.Legend.LegendPosition;
            ShallRender = _bco.ShallRender;

            ChartHeight = _bco.ChartHeight;
            ChartWidth = _bco.ChartWidth;

            Y2max = _bco.Y2max;
            Y2min = _bco.Y2min;
            Y2Tick = _bco.Y2Tick;

            Ymax = _bco.Ymax;
            Ymin = _bco.Ymin;
            YTick = _bco.YTick;
            YLabel = _bco.YLabel;
            YLabels = _bco.YLabels;
            IsYLog = _bco.IsYLog;
            IsYGrid = _bco.IsYGrid;

            XLabel = _bco.XLabel;
            XLabels = _bco.XLabels;
            Xmax = _bco.Xmax;
            Xmin = _bco.Xmin;
            XTick = _bco.XTick;
            IsXLog = _bco.IsXLog;
            IsXGrid = _bco.IsXGrid;

            Bs = _bco.Bs;

            AllValues = _bco.AllValues;
            NumberBins = _bco.NumberBins;
            BarType = _bco.BarType;

            DataCollection = _bco.DataCollection;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BarChartObject()
        {
            Bs = new List<BarSeries>();
            DataCollection = new BindableCollection<BarSeries>();
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            Xmax = 0;
            Xmin = 0;
            Ymax = 0;
            Ymin = 0;
            Bs.Clear();
            AllValues = new List<double>();

        }

        /// <summary>
        /// Adding a data series to the histogram object
        /// </summary>
        public void AddDataSeries()
        {
            BarSeries i = new BarSeries(DataCollection);
            i.BorderThickness = 1;
            i.BorderColor = System.Windows.Media.Brushes.Black;

            if (Bs.Count >= 1)
                i.FillColor = IntToColorConverter.Convert(IntToColorConverter.ConvertBack(Bs[Bs.Count - 1].FillColor) + 1);
            else
                i.FillColor = IntToColorConverter.Convert(1);

            i.BarWidth = 1;
            Bs.Add(i);
        }

        /// <summary>
        /// Initialization
        /// </summary>
        public void InitializeStandardHistogram()
        {
            YLabel.Text = "Frequency [-]";
        }

        /// <summary>
        /// Creating a histogram chart
        /// </summary>
        public void CreateHistogram()
        {

            Initialize();
            InitializeStandardHistogram();

            if (!ShallRender)
                return;

            for (int i = 0; i < DataSet.Count(); i++)
            {
                try
                {
                    AddDataSeries();

                    Bs[i].SeriesName = DataSet[i].Name;

                    //Sorting and filtering
                    if (!IsXLog)
                        Bs[i].Values = DataSet[i].Vertices.Where(x => x.Value[0] != 0).OrderBy(x => x.Value[0]).Select(x => x.Value[0]).ToList();
                    else
                    {
                        Bs[i].Values = DataSet[i].Vertices.Select(x => Math.Log10(x.Value[0])).Where(x => !double.IsNegativeInfinity(x)).OrderBy(x => x).ToList();
                    }

                    AllValues.AddRange(Bs[i].Values);
                }
                catch
                {
                    continue;
                }

                CreateChart();
            }
        }

        /// <summary>
        /// Creates a histogram based on categories
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="values"></param>
        public void CreateCategoricHistogram(string[] labels, double[] values)
        {
            XLabels.Clear();

            try
            {
                AddDataSeries();
                foreach (string lab in labels)
                {
                    XLabels.Add(new Label() { Text = lab });
                }

                Bs.Clear();
                Bs.Add(new BarSeries());

                for (int j = 0; j < Bs.Count(); j++)
                {
                    //Calculating empirical distributions
                    for (int i = 0; i < labels.Count(); i++)
                    {
                        var a = NormalizePoint(new LocationTimeValue(i + 1, values[i] > Ymax ? Ymax : values[i]));
                        var b = NormalizePoint(new LocationTimeValue(Xmin + 1, values[i] > Ymax ? Ymax - Ymin : values[i]));

                        Bs[j].BarPoints.Add(new Rectangle2D()
                        {
                            X = a.X,
                            Y = a.Y,
                            Width = b.X,
                            Height = ChartHeight - Math.Abs(b.Y)
                        });

                        if (Xmin == 0 && Xmax == 0 && Ymin == 0 && Ymax == 0)
                            SubdivideAxes();

                    }

                    AddGridlines();
                    AddTicksAndLabels();
                }
            }
            catch
            {

            }

            DataCollection.AddRange(Bs);
        }

        /// <summary>
        /// Creating the chart object
        /// </summary>
        public virtual void CreateChart()
        {
            try
            {
                if (Updating)
                    return;

                DataCollection.Clear();

                if (AllValues != null)
                {
                    //Subdividing into bins
                    double[] bins = DistributionHelper.Subdivide(AllValues.ToArray(), NumberBins);
                    double width = bins[1] - bins[0];


                    for (int i = 0; i < Bs.Count(); i++)
                    {
                        Bs[i].BarPoints.Clear();
                        Bs[i].LinePoints.Clear();

                        Bs[i].Counts = DistributionHelper.Counts(Bs[i].Values.ToArray(), bins).Select(x => (double)x).ToArray();

                        var count = Bs[i].Counts.Max();

                        if (count > maxCount)
                        {
                            maxCount = Convert.ToInt32(count);
                        }

                        for (int j = 0; j < Bs[i].Counts.Count(); j++)
                        {
                            if (bins[j] + (i * (width / Bs.Count)) > Xmax || bins[j] < Xmin)
                                continue;

                            var a = NormalizePoint(new LocationTimeValue(bins[j] + (i * (width / Bs.Count)), Bs[i].Counts[j] > Ymax ? Ymax : Bs[i].Counts[j]));
                            var b = NormalizePoint(new LocationTimeValue(Xmin + width / Bs.Count(), Bs[i].Counts[j] > Ymax ? Ymax - Ymin : Bs[i].Counts[j]));

                            Bs[i].BarPoints.Add(new Rectangle2D()
                            {
                                X = a.X,
                                Y = a.Y,
                                Width = b.X,
                                Height = ChartHeight - Math.Abs(b.Y)
                            });
                        }

                        //Calculating the theoretical distributions
                        if (Bs[i].EDH.CalculateDistribution)
                        {
                            double av = Bs[i].Values.Average();
                            double std = Bs[i].Values.StdDev();
                            bins = DistributionHelper.Subdivide(AllValues.ToArray(), 200);
                            double[] theoreticalDistribution = new double[200];

                            for (int j = 0; j < 200; j++)
                            {
                                theoreticalDistribution[j] = Bs[i].EDH.GetDistributionValue(av, std, bins[j]);
                            }

                            //Scale the distribution to fit in the chart
                            theoreticalDistribution = DistributionHelper.ScaleToArea(theoreticalDistribution, Ymax, Ymin);

                            for (int j = 0; j < 200; j++)
                            {
                                if (bins[j] >= Xmin && bins[j] <= Xmax)
                                    Bs[i].LinePoints.Add(NormalizePoint(new LocationTimeValue(bins[j], theoreticalDistribution[j])));
                            }
                        }
                    }
                }

                if (Xmin == 0 && Xmax == 0 && Ymin == 0 && Ymax == 0)
                    SubdivideAxes();

                DataCollection.AddRange(Bs);
                AddLegend();
                AddGridlines();
                AddTicksAndLabels();
            }
            catch
            {

            }

        }

        /// <summary>
        /// Removes the selected series
        /// </summary>
        public override void RemoveSelectedSeries()
        {
            try
            {
                base.RemoveSelectedSeries();
                CreateHistogram();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Adding the legend to the chart
        /// </summary>
        public override void AddLegend()
        {
            base.AddLegend();

            try
            {
                for(int i = 0; i<DataCollection.Count();i++)
                {
                    Legend.LegendObjects[i].Rectangle.Brush = DataCollection[i].FillColor;
                }
            }
            catch
            {

            }
        }

        public void SubdivideAxes()
        {
            Ymin = 0;
            Ymax = AllValues.Count();
            Xmax = AllValues.Max();

            YTick = Math.Round((Ymax - Ymin) / 10, 0);

            if (YTick == 0)
                YTick += 1;
            if (XTick == 0)
                XTick += 1;
        }
        #endregion
    }

    //Enumerator for the bar types
    public enum BarTypeEnum
    {
        Vertical = 0,
        Horizontal = 1,
        VerticalStack = 2,
        HorizontalStack = 3,
        VerticalOverlay = 4,
        HorizontalOverlay = 5,
        LithologicalSection = 6,
        TwoDArray = 7,
        HorizontalBoxPlot = 8,
        VerticalBoxPlot = 9,
        VerticalSideBySide = 10
    }
}
