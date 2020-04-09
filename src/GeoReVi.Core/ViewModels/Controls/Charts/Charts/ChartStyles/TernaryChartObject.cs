using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    [Serializable]
    public class TernaryChartObject : LineChartObject
    {

        private Label zLabel = new Label() { Text = "Z axis" };
        public Label ZLabel
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
            Legend.IsLegend = _tco.Legend.IsLegend;
            Legend.LegendPosition = _tco.Legend.LegendPosition;
            ShallRender = _tco.ShallRender;

            ChartHeight = _tco.ChartHeight;
            ChartWidth = _tco.ChartWidth;

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

            Ds = _tco.Ds;
            FillColor = _tco.FillColor;

            Maxx = _tco.Maxx;
            Maxy = _tco.Maxy;
            Minx = _tco.Minx;
            Miny = _tco.Miny;
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
        }

        /// <summary>
        /// Adding a data series to the histogram object
        /// </summary>
        public override void AddDataSeries()
        {
            LineSeries i = new LineSeries(DataCollection);

            var a = Ds.Count - 1;

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

            XLabel.Text = "X";
            YLabel.Text = "Y";
            ZLabel.Text = "Z";

            CreateChart();
        }


        /// <summary>
        /// Creating the ternary chart
        /// </summary>
        public override async Task CreateChart()
        {
            try
            {
                if (Updating)
                    return;

                Ds.Clear();
                DataCollection.Clear();
                int i = 0;

                if (IsColorMap)
                {
                    SetBrush(ColorMap.CalculateColormapBrushes());
                    SetColorMapLabels();
                }

                //Adding values from the 3D chart collection
                foreach (Mesh mesh in DataSet)
                {
                    List<LocationTimeValue> locs = new List<LocationTimeValue>();
                    AddDataSeries();

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

                        LocationTimeValue b = new LocationTimeValue(x, y, z, mesh.Vertices[j].Name);
                        locs.Add(b);
                    }

                    var a = TernaryHelper.ConvertToTwoDimensionalCoordinates(locs);

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

                    i++;
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

        /// <summary>
        /// Adding ticks and labels to the chart
        /// </summary>
        public override void AddTicksAndLabels()
        {
            try
            {
                //Adding x label
                XLabel.X = ChartWidth - MeasureString(XLabel.Text).Width;
                XLabel.Y = ChartHeight + 3.5 * MeasureString(XLabel.Text).Height;

                //Adding y label
                YLabel.X = 0;
                YLabel.Y = ChartHeight + 3.5 * MeasureString(YLabel.Text).Height;

                //Adding z label
                ZLabel.X = ChartWidth / 2 - 0.5 * MeasureString(ZLabel.Text).Width;
                ZLabel.Y = 0 - 3.5 * MeasureString(XLabel.Text).Height;
            }
            catch
            {

            }

        }
    }
}
