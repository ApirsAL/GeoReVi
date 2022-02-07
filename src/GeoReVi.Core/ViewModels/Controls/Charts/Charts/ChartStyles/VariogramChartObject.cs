using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A variogram chart object
    /// </summary>
    public class VariogramChartObject : LineChartObject
    {

        /// <summary>
        /// A variogram helper object
        /// </summary>
        private VariogramHelper vh = new VariogramHelper();
        public VariogramHelper Vh
        {
            get => this.vh;
            set
            {
                this.vh = value;
                NotifyOfPropertyChange(() => Vh);
            }
        }


        #region Constructor

        public VariogramChartObject(VariogramChartObject _vco)
        {
            DataCollection = new BindableCollection<LineSeries>();

            DataSet = _vco.DataSet;
            Title = _vco.Title;
            GridlineColor = _vco.GridlineColor;
            GridlinePattern = _vco.GridlinePattern;
            Legend.IsLegend = _vco.Legend.IsLegend;
            Legend.LegendPosition = _vco.Legend.LegendPosition;
            ShallRender = _vco.ShallRender;

            ChartHeight = _vco.ChartHeight;
            ChartWidth = _vco.ChartWidth;

            Y2max = _vco.Y2max;
            Y2min = _vco.Y2min;
            Y2Tick = _vco.Y2Tick;

            Ymax = _vco.Ymax;
            Ymin = _vco.Ymin;
            YTick = _vco.YTick;
            YLabel = _vco.YLabel;
            YLabels = _vco.YLabels;
            IsYLog = _vco.IsYLog;
            IsYGrid = _vco.IsYGrid;

            XLabel = _vco.XLabel;
            XLabels = _vco.XLabels;
            Xmax = _vco.Xmax;
            Xmin = _vco.Xmin;
            XTick = _vco.XTick;
            IsXLog = _vco.IsXLog;
            IsXGrid = _vco.IsXGrid;

            Ds = _vco.Ds;
            FillColor = _vco.FillColor;

            Maxx = _vco.Maxx;
            Maxy = _vco.Maxy;
            Minx = _vco.Minx;
            Miny = _vco.Miny;

            BarType = _vco.BarType;

            DataCollection = _vco.DataCollection;
            Vh = new VariogramHelper(DataSet);
            Initialize();
            CreateChart();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public VariogramChartObject()
        {
            DataCollection = new BindableCollection<LineSeries>();
            Vh = new VariogramHelper(DataSet);
            Initialize();
            CreateChart();
        }

        #endregion

        /// <summary>
        /// Adding a data series to the variogram
        /// </summary>
        public void AddDataSeries()
        {
            LineSeries i = new LineSeries();
            //Ds.Symbols.FillColor = IntToColorConverter.Convert(0);
            i.Symbols.FillColor = IntToColorConverter.Convert(0);
            i.Symbols.SymbolSize = 6;
            i.Symbols.SymbolType = SymbolTypeEnum.Dot;
            i.Symbols.BorderThickness = 0;

            Ds.Add(i);
        }

        //Initializing a standard style for variograms
        public void InitializeStandardVariogram()
        {
            Title = "";
            YLabel.Text = "γ(Lag)";
            XLabel.Text = "Lag distance";
            Ds = null;
            Ds = new BindableCollection<LineSeries>();
        }

        /// <summary>
        /// Updating the chart object
        /// </summary>
        public void UpdateChart()
        {
            var a = new BindableCollection<LineSeries>(DataCollection);
            DataCollection.Clear();

            foreach (var i in a)
            {
                DataCollection.Add(i);
            }
        }

        /// <summary>
        /// Adding a model series to the variogram
        /// </summary>
        public void AddModelSeries()
        {
            LineSeries i = new LineSeries();
            i.Symbols.FillColor = System.Windows.Media.Brushes.Transparent;
            i.Symbols.SymbolSize = 0;
            i.Symbols.SymbolType = SymbolTypeEnum.None;
            i.Symbols.BorderThickness = 0;

            Ds.Add(i);
        }

        /// <summary>
        /// Optimizing the variogram model
        /// </summary>
        public void OptimizeModel()
        {
            CreateChart();
        }

        /// <summary>
        /// Creating the variogram chart object
        /// </summary>
        public override async Task CreateChart()
        {
            if (!ShallRender)
                return;

            List<LocationTimeValue> locVal = new List<LocationTimeValue>();

            //If we have an indicator variogram
            List<double> indicatorValues = new List<double>();
            string[] classes = new string[] { };

            //Performing the interpolation
            CommandHelper ch = new CommandHelper();
            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => Updating, async () =>
            {
                try
                {
                    if (Vh.IsIndicator)
                    {
                        classes = DataSet.Select(x => x.Name).GroupBy(g => g).Select(f => f.Key).ToArray();

                        for (int i = 0; i < classes.Length; i++)
                        {
                            indicatorValues.Add(Convert.ToDouble(i));
                        }

                    }

                    InitializeStandardVariogram();

                    for (int i = 0; i < DataSet.Count(); i++)
                    {
                        await Task.Delay(0);

                        AddDataSeries();

                        Ds[i].SeriesName = DataSet[i].Name;
                        Ds[i].LinePoints = null;
                        Ds[i].LinePoints = new BindableCollection<LocationTimeValue>();
                    }

                    //Computes the experimental semivariogram
                    await Task.WhenAll(Vh.ComputeExperimentalVariogram());


                    for (int i = 0; i < vh.Variogram.Count(); i++)
                    {
                        foreach (var var in vh.Variogram[i])
                        {

                            if (!double.IsNaN(var.Y))
                                try
                                {
                                    var a = new LocationTimeValue((double)var.X, (double)var.Y);

                                    if (a.X < Xmin || a.X > Xmax)
                                        continue;

                                    if (a.Y < Ymin || a.Y > Ymax)
                                        continue;

                                    Ds[i].LinePoints.Add(new LocationTimeValue()
                                    {
                                        X = NormalizePoint(a).X - 0.5 * Ds[i].Symbols.SymbolSize,
                                        Y = NormalizePoint(a).Y - 0.5 * Ds[i].Symbols.SymbolSize,
                                        Brush = !IsColorMap
                                            ? Ds[i].Symbols.FillColor
                                            : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)a.Value[0]) : (double)a.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)

                                    });
                                }
                                catch
                                {
                                    continue;
                                }
                        }

                        Ds[i].LinePoints.OrderBy(x => x.X);

                        Ds[i].LinePoints = new BindableCollection<LocationTimeValue>(Ds[i].LinePoints);

                        Maxx = Ds[i].LinePoints.Max(x => DeNormalizePoint(x).X);
                        Minx = Ds[i].LinePoints.Min(x => DeNormalizePoint(x).X);
                        Maxy = Ds[i].LinePoints.Max(x => DeNormalizePoint(x).Y);
                        Miny = Ds[i].LinePoints.Min(x => DeNormalizePoint(x).Y);
                    }

                }
                catch
                {

                }


            }));

            if (Xmin == 0 && Xmax == 0 && Ymin == 0 && Ymax == 0)
                SubdivideAxes();

            AddModelSeries();

            //Calculates the variogram model
            vh.CalculateVariogramModel();

            foreach (var b in Vh.VariogramModelPoints)
            {
                if (!double.IsNaN(b.Y))
                    try
                    {
                        var a = new LocationTimeValue((double)b.X, (double)b.Y);

                        if (a.X < Xmin || a.X > Xmax)
                            continue;

                        if (a.Y < Ymin || a.Y > Ymax)
                            continue;

                        Ds[Ds.Count -1].LinePoints.Add(new LocationTimeValue()
                        {
                            X = NormalizePoint(a).X - 0.5 * Ds[Ds.Count - 1].Symbols.SymbolSize,
                            Y = NormalizePoint(a).Y - 0.5 * Ds[Ds.Count - 1].Symbols.SymbolSize,
                            Brush = !IsColorMap
                                                        ? Ds[Ds.Count - 1].Symbols.FillColor
                                                        : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)a.Value[0]) : (double)a.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)

                        });
                    }
                    catch
                    {
                        continue;
                    }
            }

            Ds[Ds.Count - 1].LinePoints = new BindableCollection<LocationTimeValue>(Ds[Ds.Count - 1].LinePoints);

            Ds[Ds.Count - 1].SeriesName = "Theoretical";

            AddGridlines();
            AddTicksAndLabels();

            DataCollection = Ds;

        }

        /// <summary>
        /// Subdivides the axes based on the min an max values of the data set
        /// </summary>
        public override void SubdivideAxes()
        {
            try
            {

                Xmax = Vh.Variogram.SelectMany(x => x.Select(y => y.X)).Max();
                Ymax = Vh.Variogram.SelectMany(x => x.Select(y => y.Y)).Max();

                CreateChart();

            }
            catch
            {
                Xmax = 10;
                Ymax = 10;
                throw new Exception("Axes couldn't be scaled.");
            }
        }
    }
}
