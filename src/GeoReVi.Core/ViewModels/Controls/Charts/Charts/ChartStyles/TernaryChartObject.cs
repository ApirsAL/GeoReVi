using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    [Serializable]
    public class TernaryChartObject : LineChartObject
    {

        private string zLabel = "Z axis";
        public string ZLabel
        {
            get => this.zLabel;
            set
            {
                this.zLabel = value;
                NotifyOfPropertyChange(() => ZLabel);
            }
        }
        #region Constructor

        public TernaryChartObject(TernaryChartObject _tco)
        {
            DataSet = _tco.DataSet;
            Title = _tco.Title;
            GridlineColor = _tco.GridlineColor;
            GridlinePattern = _tco.GridlinePattern;
            HasLegend = _tco.HasLegend;
            LegendPosition = _tco.LegendPosition;
            ShallRender = _tco.ShallRender;
            DataTableColumnNames = _tco.DataTableColumnNames;

            ChartHeight = _tco.ChartHeight;
            ChartWidth = _tco.ChartWidth;
            ColumnList = _tco.ColumnList;

            Y2max = _tco.Y2max;
            Y2min = _tco.Y2min;
            Y2Tick = _tco.Y2Tick;

            Ymax = _tco.Ymax;
            Ymin = _tco.Ymin;
            YTick = _tco.YTick;
            YLabel = _tco.YLabel;
            YLabels = _tco.YLabels;
            IsYLog = _tco.IsYLog;
            IsYGrid = _tco.IsYGrid;

            XLabel = _tco.XLabel;
            XLabels = _tco.XLabels;
            Xmax = _tco.Xmax;
            Xmin = _tco.Xmin;
            XTick = _tco.XTick;
            IsXLog = _tco.IsXLog;
            IsXGrid = _tco.IsXGrid;

            Direction = _tco.Direction;
            Ds = _tco.Ds;
            FillColor = _tco.FillColor;

            Maxx = _tco.Maxx;
            Maxy = _tco.Maxy;
            Minx = _tco.Minx;
            Miny = _tco.Miny;
            SpatialPointSeries = _tco.SpatialPointSeries;
            ZLabel = _tco.ZLabel;

            BarType = _tco.BarType;

            DataCollection = _tco.DataCollection;
        }
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public TernaryChartObject()
        {
            DataCollection = new BindableCollection<LineSeries>();
        }

        #endregion

        //Initialize
        public override void Initialize()
        {
            Xmax = 1;
            Xmin = 0;
            Ymax = 1;
            Ymin = 0;
            Ds.Clear();
            SpatialPointSeries.Clear();
        }

        /// <summary>
        /// Adding a data series to the histogram object
        /// </summary>
        public override void AddDataSeries()
        {
            LineSeries i = new LineSeries(DataCollection);

            var a = Ds.Count -1 ;

            if (a >= 1)
                i.Symbols.FillColor = IntToColorConverter.Convert(IntToColorConverter.ConvertBack(Ds[a].Symbols.FillColor) + 1);
            else
                i.Symbols.FillColor = IntToColorConverter.Convert(1);

            i.Symbols.SymbolSize = 6;
            i.Symbols.SymbolType = SymbolTypeEnum.Dot;
            i.Symbols.BorderThickness = 0;
            i.LineColor = System.Windows.Media.Brushes.Transparent;

            i.LineThickness = 1.5;

            Ds.Add(i);
        }

        /// <summary>
        /// Creating a ternary chart
        /// </summary>
        public void CreateTernaryChart()
        {
            if (!ShallRender)
                return;

            Initialize();

            foreach (var d in DataSet)
            {
                ObservableCollection<LocationTimeValue> tup = new ObservableCollection<LocationTimeValue>();

                tup.AddRange(new List<LocationTimeValue>(d.Data.AsEnumerable()
                    .Select(x => new LocationTimeValue()
                    {
                       X = (x.Field<double>(ColumnList[0]) == -9999 || x.Field<double>(ColumnList[0]) == -999999) ? 0 : x.Field<double>(ColumnList[0]),
                       Y = (x.Field<double>(ColumnList[1]) == -9999 || x.Field<double>(ColumnList[1]) == -999999) ? 0 : x.Field<double>(ColumnList[1]),
                       Z = (x.Field<double>(ColumnList[2]) == -9999 || x.Field<double>(ColumnList[2]) == -999999) ? 0 : x.Field<double>(ColumnList[2])
                    }).Where(x=> x.X != 0 && x.Y != 0 && x.Z != 0)));

                AddDataSeries();

                Ds[Ds.Count - 1].SeriesName = d.Name;
                SpatialPointSeries.Add(tup);
            }

            XLabel = ColumnList[0];
            YLabel = ColumnList[1];
            ZLabel = ColumnList[2];

            CreateChart();
        }


        /// <summary>
        /// Creating the ternary chart
        /// </summary>
        public override void CreateChart()
        {
            try
            {
                if (Updating)
                    return;

                DataCollection.Clear();
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
                    var a = TernaryHelper.ConvertToTwoDimensionalCoordinates(ser.ToList());

                    foreach (var loc in a)
                    {

                        LocationTimeValue b = NormalizePoint(loc);

                        try
                        {
                            Ds[i].LinePoints.Add(new LocationTimeValue()
                            {
                                X = NormalizePoint(loc).X - 0.5 * Ds[i].Symbols.SymbolSize,
                                Y = NormalizePoint(loc).Y - 0.5 * Ds[i].Symbols.SymbolSize,
                                Brush = !IsColorMap
        ? Ds[i].Symbols.FillColor
        : ColorMapHelper.GetBrush(ColorMap.IsLog ? Math.Log10((double)loc.Value[0]) : (double)loc.Value[0], ColorMap.IsLog && ColorMap.Ymin != 0 ? Math.Log10(ColorMap.Ymin) : ColorMap.Ymin, ColorMap.IsLog ? Math.Log10(ColorMap.Ymax) : ColorMap.Ymax, ColorMap)
                            });
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                AddGridlines();
            }
            catch
            {

            }

            DataCollection.AddRange(Ds);
        }

        /// <summary>
        /// Adds labels to the chart
        /// </summary>
        public override void AddGridlines()
        {
            //New gridline object
            Gridline gridline = new Gridline();

            //Clearing gridline collection
            XGridlines.Clear();

            try
            {
                // Create vertical gridlines: 
                if (IsXGrid == true)
                {
                    for (double i = 0; i < 1; i = i + 0.3333333)
                    {
                        i = Math.Round(i, 5);

                        gridline = new Gridline();

                        gridline.X1 = NormalizePoint(new LocationTimeValue(i / 2, i)).X;

                        gridline.Y1 = NormalizePoint(new LocationTimeValue(i / 2, i)).Y;

                        gridline.X2 = NormalizePoint(new LocationTimeValue(i, 0)).X;

                        gridline.Y2 = NormalizePoint(new LocationTimeValue(i, 0)).Y;

                        XGridlines.Add(gridline);

                        gridline = new Gridline();

                        gridline.X1 = NormalizePoint(new LocationTimeValue(i / 2, i)).X;

                        gridline.Y1 = NormalizePoint(new LocationTimeValue(i / 2, i)).Y;

                        gridline.X2 = NormalizePoint(new LocationTimeValue(1 - i / 2, i)).X;

                        gridline.Y2 = NormalizePoint(new LocationTimeValue(1 - i / 2, i)).Y;

                        XGridlines.Add(gridline);

                        gridline = new Gridline();

                        gridline.X1 = NormalizePoint(new LocationTimeValue(1 - i / 2, i)).X;

                        gridline.Y1 = NormalizePoint(new LocationTimeValue(1 - i / 2, i)).Y;

                        gridline.X2 = NormalizePoint(new LocationTimeValue(1 - i, 0)).X;

                        gridline.Y2 = NormalizePoint(new LocationTimeValue(1 - i, 0)).Y;


                        XGridlines.Add(gridline);
                    }
                }
            }
            catch
            {

            }
        }
    }
}
