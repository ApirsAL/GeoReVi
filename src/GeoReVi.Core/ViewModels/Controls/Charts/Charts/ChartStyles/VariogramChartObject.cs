using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

       
        /// <summary>
        /// Measured points of the variogram
        /// </summary>
        private BindableCollection<LocationTimeValue> measuringPoints;
        public BindableCollection<LocationTimeValue> MeasuringPoints
        {
            get => this.measuringPoints;
            set
            {
                this.measuringPoints = value;
                NotifyOfPropertyChange(() => MeasuringPoints);
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

            Direction = _vco.Direction;
            Ds = _vco.Ds;
            FillColor = _vco.FillColor;

            Maxx = _vco.Maxx;
            Maxy = _vco.Maxy;
            Minx = _vco.Minx;
            Miny = _vco.Miny;
            SpatialPointSeries = _vco.SpatialPointSeries;

            MeasuringPoints = _vco.MeasuringPoints;

            BarType = _vco.BarType;

            DataCollection = _vco.DataCollection;
            Vh = new VariogramHelper(MeasuringPoints);
            Initialize();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public VariogramChartObject()
        {
            DataCollection = new BindableCollection<LineSeries>();
            Vh = new VariogramHelper(MeasuringPoints);
            Initialize();
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
        public override void CreateChart()
        {
            if (!ShallRender)
                return;

            List<LocationTimeValue> locVal = new List<LocationTimeValue>();

            MeasuringPoints = new BindableCollection<LocationTimeValue>();

            //If we have an indicator variogram
            List<double> indicatorValues = new List<double>();
            string[] classes = new string[] { };

            if (Vh.IsIndicator)
            {
                classes = DataSet.Select(x => x.Name).GroupBy(g => g).Select(f => f.Key).ToArray();

                for (int i = 0; i < classes.Length; i++)
                {
                    indicatorValues.Add(Convert.ToDouble(i));
                }

            }

            for (int i = 0; i < DataSet.Count();i++)
            {
                try
                {
                    MeasuringPoints.AddRange(DataSet[i].Data.AsEnumerable().OrderBy(x => (double)x[1]).OrderBy(x => (double)x[2]).OrderBy(x => (double)x[3]).Select(x => new LocationTimeValue()
                    {
                        Value = new List<double>() { !Vh.IsIndicator ? x.Field<double>(0) : indicatorValues[i] },
                        X = x.Field<double>(1),
                        Y = x.Field<double>(2),
                        Z = x.Field<double>(3),
                        Name = x.Field<string>(5)
                    }));
                }
                catch
                {
                    continue;
                }
            }


            InitializeStandardVariogram();
            AddDataSeries();
            Ds[0].SeriesName = "Experimental";


            switch (Direction)
            {
                case DirectionEnum.X:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, X = a.X, Y = 0, Z = 0 }));
                    break;
                case DirectionEnum.Y:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, X = 0, Y = a.Y, Z = 0 }));
                    break;
                case DirectionEnum.Z:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, X = 0, Y = 0, Z = a.Z }));
                    break;
                case DirectionEnum.XY:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, Y = a.Y, X = a.X, Z = 0 }));
                    break;
                case DirectionEnum.XZ:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, Z = a.Z, X = a.X, Y = 0 }));
                    break;
                case DirectionEnum.YZ:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, Y = a.Y, Z = a.Z, X = 0 }));
                    break;
                case DirectionEnum.XYZ:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, X = a.X, Y = a.Y , Z = a.Z}));
                    break;
                default:
                    MeasuringPoints = new BindableCollection<LocationTimeValue>(MeasuringPoints.Select(a => new LocationTimeValue() { Value = a.Value, X = a.X, Y = a.Y, Z = a.Z }));
                    break;
            }

            Vh.DataSet = MeasuringPoints;

            //Computes the experimental semivariogram
            Vh.ComputeExperimentalVariogram();

            Ds[0].LinePoints = null;
            Ds[0].LinePoints = new BindableCollection<LocationTimeValue>();

            foreach (var var in vh.Variogram)
            {
                if (!double.IsNaN(var.Y))
                    try
                    {
                        var a = new LocationTimeValue((double)var.X, (double)var.Y);

                        if (a.X < Xmin || a.X > Xmax)
                            continue;

                        if (a.Y < Ymin || a.Y > Ymax)
                            continue;

                        Ds[0].LinePoints.Add(new LocationTimeValue()
                        {
                            X = NormalizePoint(a).X - 0.5 * Ds[0].Symbols.SymbolSize,
                            Y = NormalizePoint(a).Y - 0.5 * Ds[0].Symbols.SymbolSize,
                            Brush = !IsColorMap
                                ? Ds[0].Symbols.FillColor
                                : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)a.Value[0]) : (double)a.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)

                        });
                    }
                    catch
                    {
                        continue;
                    }
            }

            Ds[0].LinePoints.OrderBy(x => x.X);

            Ds[0].LinePoints = new BindableCollection<LocationTimeValue>(Ds[0].LinePoints);

            Maxx = Ds[0].LinePoints.Max(x => DeNormalizePoint(x).X);
            Minx = Ds[0].LinePoints.Min(x => DeNormalizePoint(x).X);
            Maxy = Ds[0].LinePoints.Max(x => DeNormalizePoint(x).Y);
            Miny = Ds[0].LinePoints.Min(x => DeNormalizePoint(x).Y);


            if (Xmin == 0 && Xmax ==0 && Ymin ==0 && Ymax ==0)
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

                        Ds[1].LinePoints.Add(new LocationTimeValue()
                        {
                            X = NormalizePoint(a).X - 0.5 * Ds[1].Symbols.SymbolSize,
                            Y = NormalizePoint(a).Y - 0.5 * Ds[1].Symbols.SymbolSize,
                            Brush = !IsColorMap
                                                        ? Ds[1].Symbols.FillColor
                                                        : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)a.Value[0]) : (double)a.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)

                        });
                    }
                    catch
                    {
                        continue;
                    }
            }

            Ds[1].LinePoints = new BindableCollection<LocationTimeValue>(Ds[1].LinePoints);

            Ds[1].SeriesName = "Theoretical";

            AddGridlines();
            AddTicksAndLabels();

            DataCollection = Ds;

        }
    }
}
