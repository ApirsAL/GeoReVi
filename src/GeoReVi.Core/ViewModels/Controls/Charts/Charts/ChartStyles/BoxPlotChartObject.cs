﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    [Serializable]
    public class BoxPlotChartObject : ChartObject<BoxPlotSeries>
    {

        //Check if outliers should be removed
        private bool outliersRemoved;
        //Should outliers be removed
        public bool OutliersRemoved
        {
            get => this.outliersRemoved;
            set
            {
                this.outliersRemoved = value;

                if (DataCollection != null)
                    if (DataCollection.Count != 0)
                    {
                        string xlabel = this.XLabel.ToString();
                        CreateChart();
                        this.XLabel = xlabel;
                    }

                NotifyOfPropertyChange(() => this.OutliersRemoved);
            }
        }

        //provide range of the outliers
        private double outlierRange = 1;
        //Range of outliers
        public double OutlierRange
        {
            get => this.outlierRange;
            set
            {
                if (value > 0 && value <= 2)
                    this.outlierRange = value;
                else
                    this.outlierRange = 1;

                if (DataCollection != null)
                    if (DataCollection.Count != 0)
                    {
                        string xlabel = this.XLabel.ToString();
                        CreateChart();
                        this.XLabel = xlabel;
                    }



                NotifyOfPropertyChange(() => this.OutlierRange);
            }
        }

        /// <summary>
        /// Data series
        /// </summary>
        private List<BoxPlotSeries> bp;
        public List<BoxPlotSeries> Bp
        {
            get => this.bp;
            set
            {
                this.bp = value;
                NotifyOfPropertyChange(() => Bp);
            }
        }


        #region Constructor

        public BoxPlotChartObject(BoxPlotChartObject _bco)
        {
            DataSet = _bco.DataSet;
            Title = _bco.Title;
            GridlineColor = _bco.GridlineColor;
            GridlinePattern = _bco.GridlinePattern;
            HasLegend = _bco.HasLegend;
            LegendPosition = _bco.LegendPosition;
            ShallRender = _bco.ShallRender;
            DataTableColumnNames = _bco.DataTableColumnNames;

            ChartHeight = _bco.ChartHeight;
            ChartWidth = _bco.ChartWidth;
            ColumnList = _bco.ColumnList;

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

            Bp = _bco.Bp;
            OutlierRange = _bco.OutlierRange;
            OutliersRemoved = _bco.OutliersRemoved;

            BarType = _bco.BarType;

            DataCollection = _bco.DataCollection;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoxPlotChartObject()
        {
            this.Bp = new List<BoxPlotSeries>();
            DataCollection = new BindableCollection<BoxPlotSeries>();
        }

        #endregion

        #region Methods
        //Initializing
        public override void Initialize()
        {
            DataCollection.Clear();
            Bp.Clear();
        }

        /// <summary>
        /// Creates the data series and corresponding chart objects
        /// </summary>
        /// <param name="title"></param>
        /// <param name="parameter"></param>
        /// <param name="par"></param>
        public void CreateChart()
        {

            if (!ShallRender)
                return;

            Initialize();

            for(int i = 0; i< DataSet.Count();i++)
            {
                try
                {
                    //temporal storage for measurement values
                    double[] a = new double[] { };

                    AddDataSeries();

                    a = DataSet[i].Data.AsEnumerable().Select(x => (double)x[0]).Where(x => x != 0).ToArray();

                    Bp[i].BoxPlotStatisticsCollection  = new BoxPlotStatistics(a, OutliersRemoved, OutlierRange);

                    Bp[i].SeriesName = DataSet[i].Name;

                    YTick = 1;

                    AddGridlines();
                    AddTicksAndLabels();
                    AddWhiskers(Bp[i], i);
                    AddBox(Bp[i], i);

                }
                catch
                {
                    continue;
                }
            }

            DataCollection.AddRange(Bp);
        }

        /// <summary>
        /// Adding a default data series
        /// </summary>
        public void AddDataSeries()
        {
            BoxPlotSeries Bps = new BoxPlotSeries();
            Bps.BorderColor = System.Windows.Media.Brushes.Black;
            Bps.BorderThickness = 1;
            Bps.BarWidth = 0.4;
            Bps.FillColor = IntToColorConverter.Convert(DataCollection.Count);

            Bp.Add(Bps);
        }

        /// <summary>
        /// Adds whiskers to a series
        /// </summary>
        /// <param name="bps"></param>
        public void AddBox(BoxPlotSeries bps, double y)
        {
            try
            {
                y += 0.5 * YTick;

                bps.BarPoints.Clear();

                var a = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.LowerQuartile, y+ 0.5*bps.BarWidth));
                var b = NormalizePoint(new LocationTimeValue(Xmin + (bps.BoxPlotStatisticsCollection.UpperQuartile - bps.BoxPlotStatisticsCollection.LowerQuartile),
                            Ymax - bps.BarWidth));

                Rectangle2D rect = new Rectangle2D()
                {
                    X = a.X,
                    Y = a.Y,
                    Width = IsXLog ? Math.Log10(b.X) : b.X,
                    Height = b.Y
                };

                bps.BarPoints.Add(rect);

            }
            catch
            {

            }
        }

        /// <summary>
        /// Adds whiskers to a series
        /// </summary>
        /// <param name="bps"></param>
        public void AddWhiskers(BoxPlotSeries bps, double y)
        {
            try
            {
                //Lower Wiskers
                bps.Wiskers.Clear();

                y += 0.5 * YTick;

                Gridline gl = new Gridline();

                LocationTimeValue lower = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.Min, y));
                LocationTimeValue upper = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.LowerQuartile, y));
                LocationTimeValue middle = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.Mean, y));
                LocationTimeValue yDifference = NormalizePoint(new LocationTimeValue(Ymax - bps.BarWidth * 0.5, Ymax - bps.BarWidth * 0.5));

                gl.X1 = lower.X;
                gl.X2 = upper.X;
                gl.Y1 = lower.Y;
                gl.Y2 = upper.Y;

                bps.Wiskers.Add(gl);

                gl = new Gridline();

                gl.X1 = lower.X;
                gl.X2 = lower.X;
                gl.Y1 = lower.Y + yDifference.Y;
                gl.Y2 = lower.Y - yDifference.Y; 

                bps.Wiskers.Add(gl);

                //Middle line
                gl = new Gridline();

                gl.X1 = middle.X;
                gl.X2 = middle.X;
                gl.Y1 = middle.Y + yDifference.Y;
                gl.Y2 = middle.Y - +yDifference.Y;

                bps.Wiskers.Add(gl);

                //Upper wiskers
                gl = new Gridline();

                lower = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.UpperQuartile, y));
                upper = NormalizePoint(new LocationTimeValue(bps.BoxPlotStatisticsCollection.Max, y));

                gl.X1 = lower.X;
                gl.X2 = upper.X;
                gl.Y1 = lower.Y;
                gl.Y2 = upper.Y;

                bps.Wiskers.Add(gl);

                gl = new Gridline();

                gl.X1 = upper.X;
                gl.X2 = upper.X;
                gl.Y1 = upper.Y + yDifference.Y;
                gl.Y2 = upper.Y - yDifference.Y;

                bps.Wiskers.Add(gl);
            }
            catch
            {

            }
        }

        #endregion
    }
}
