using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace GeoReVi
{
    public class BarChartStyle 
    {
        //#region Private members

        ////the bar chart type
        //private BarTypeEnum barType = BarTypeEnum.Vertical;

        ////The bar polygon collection
        //private BindableCollection<BarPolygon> polygonCollection = new BindableCollection<BarPolygon>();

        //#endregion

        //#region Public properties

        ////A list of bar polygons
        //public BindableCollection<BarPolygon> PolygonCollection
        //{
        //    get
        //    {
        //        return this.polygonCollection;
        //    }
        //    set
        //    {
        //        this.polygonCollection = value;
        //    }
        //}

        //private BindableCollection<PointCollection> histogramPoints = new BindableCollection<PointCollection>();
        //public BindableCollection<PointCollection> HistogramPoints
        //{
        //    get => this.histogramPoints;
        //    set
        //    {
        //        this.histogramPoints = value;
        //    }
        //}

        ////the bar chart type
        //public BarTypeEnum BarType
        //{
        //    get { return barType; }
        //    set { barType = value; }
        //}
        //#endregion

        //#region Constructor

        //#endregion

        //#region Public methods

        ////Setting the bars of the chart
        //public void SetBars(BindableCollection<BarSeries> BarCollection)
        //{

        //    int nSeries = BarCollection.Count;
        //    PolygonCollection.Clear();

        //    double width;

        //    switch (BarType)
        //    {
        //        case BarTypeEnum.Vertical:
        //            if (nSeries == 1)
        //            {
        //                foreach (var ds in BarCollection)
        //                {
        //                    width = ds.BarPoints[1].X - ds.BarPoints[0].X * ds.BarWidth;

        //                    try
        //                    {
        //                        int a = 0;

        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        ///It is important thant the BarPoints are frozen before access here
        //                        /// => otherwise a 
        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        foreach (var f in ds.BarPoints)
        //                        {
        //                            SetVerticalBar(f, ds, width, 0);
        //                            a++;
        //                        }

        //                        try
        //                        {
        //                            if (ds.EDH.CalculateDistribution)
        //                            {
        //                                Y2min = 0;
        //                                Y2max = 1;
        //                                Y2Tick = 0.1;

        //                                double mean = ds.Values.Average();
        //                                double standardDeviation = ds.Values.StdDev();

        //                                ds.LineSeriesBar.SeriesName = ds.SeriesName;
        //                                ds.LineSeriesBar.Symbols.SymbolType = SymbolTypeEnum.None;

        //                                ds.LineSeriesBar.LineColor = ds.LineSeriesBar.LineColor == null ? ds.FillColor : ds.LineSeriesBar.LineColor;

        //                                for (double b = ds.Values.Min() - 0.5 * (ds.Values.Max() - ds.Values.Min()); b <= ds.Values.Max() + 0.5 * (ds.Values.Max() - ds.Values.Min()); b += (ds.Values.Max() - ds.Values.Min()) / 400)
        //                                {
        //                                    double value = ds.EDH.GetNormalDistributionValue(mean, standardDeviation, b);
        //                                    ds.LineSeriesBar.LinePoints2.Add(new LocationTimeValue(a, value));
        //                                }
        //                            }

        //                            SetLinesControl(new BindableCollection<LineSeries>() { ds.LineSeriesBar });
        //                        }
        //                        catch
        //                        {

        //                        }
        //                    }
        //                    catch
        //                    {

        //                    }

        //                }
        //            }
        //            else
        //            {
        //                int j = 0;
        //                foreach (var ds in BarCollection)
        //                {
        //                    foreach (var f in ds.BarPoints)
        //                    {
        //                        SetVerticalBar1(f, ds, nSeries, j);
        //                    }

        //                    try
        //                    {
        //                        if (ds.EDH.CalculateDistribution)
        //                        {

        //                            double mean = ds.Values.Average();
        //                            double standardDeviation = ds.Values.StdDev();

        //                            ds.LineSeriesBar.SeriesName = ds.SeriesName;
        //                            ds.LineSeriesBar.Symbols.SymbolType = SymbolTypeEnum.None;

        //                            ds.LineSeriesBar.LineColor = ds.LineSeriesBar.LineColor == null ? ds.FillColor : ds.LineSeriesBar.LineColor;

        //                            for (double b = ds.Values.Min() - 0.5 * (ds.Values.Max() - ds.Values.Min()); b <= ds.Values.Max() + 0.5 * (ds.Values.Max() - ds.Values.Min()); b += (ds.Values.Max() - ds.Values.Min()) / 400)
        //                            {
        //                                double value = ds.EDH.GetNormalDistributionValue(mean, standardDeviation, b);
        //                                ds.LineSeriesBar.LinePoints2.Add(new LocationTimeValue(b, value));
        //                            }
        //                        }

        //                        SetLinesControl(new BindableCollection<LineSeries>() { ds.LineSeriesBar });
        //                    }
        //                    catch
        //                    {

        //                    }
        //                    j++;
        //                }
        //            }
        //            break;

        //        case BarTypeEnum.VerticalOverlay:
        //            if (nSeries > 1)
        //            {
        //                List<Point> temp = new List<Point>();

        //                double[] tempy = new double[BarCollection[0].BarPoints.Count];

        //                width = ((BarCollection[0].BarPoints[1].X - BarCollection[0].BarPoints[0].X) * BarCollection[0].BarWidth) / nSeries;

        //                int j = 0;
        //                foreach (var ds in BarCollection)
        //                {
        //                    if (ds.SeriesName == "Default")
        //                        ds.SeriesName = "BarSeries" + j.ToString();

        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    ///It is important thant the BarPoints are frozen before access here
        //                    /// => otherwise a 
        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        SetVerticalBarSideBySide(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, j, nSeries);
        //                    }

        //                    try
        //                    {
        //                        if (ds.EDH.CalculateDistribution)
        //                        {

        //                            double mean = ds.Values.Average();
        //                            double standardDeviation = ds.Values.StdDev();

        //                            ds.LineSeriesBar.SeriesName = ds.SeriesName;
        //                            ds.LineSeriesBar.Symbols.SymbolType = SymbolTypeEnum.None;

        //                            ds.LineSeriesBar.LineColor = ds.LineSeriesBar.LineColor == null ? ds.FillColor : ds.LineSeriesBar.LineColor;

        //                            for (double b = ds.Values.Min() - 0.5 * (ds.Values.Max() - ds.Values.Min()); b <= ds.Values.Max() + 0.5 * (ds.Values.Max() - ds.Values.Min()); b += (ds.Values.Max() - ds.Values.Min()) / 400)
        //                            {
        //                                if (ds.Values.Max() - ds.Values.Min() <= 0)
        //                                    break;

        //                                switch (ds.EDH.DistributionType)
        //                                {
        //                                    case DistributionType.Normal:
        //                                            double value = ds.EDH.GetNormalDistributionValue(mean, standardDeviation, b);

        //                                        if(!Double.IsNaN(b))
        //                                            ds.LineSeriesBar.LinePoints2.Add(new LocationTimeValue(b, value));
        //                                        break;
        //                                    case DistributionType.Gaussian:

        //                                        break;
        //                                }
        //                            }
        //                        }

        //                        SetLinesControl(new BindableCollection<LineSeries>() { ds.LineSeriesBar });
        //                    }
        //                    catch
        //                    {

        //                    }

        //                    j++;
        //                }
        //            }
        //            else if (nSeries == 1)
        //            {
        //                foreach (var ds in BarCollection)
        //                {
        //                    try
        //                    {
        //                        width = ds.BarPoints[1].X - ds.BarPoints[0].X * ds.BarWidth;
        //                    }
        //                    catch
        //                    {
        //                        return;
        //                    }

        //                    try
        //                    {
        //                        int a = 0;

        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        ///It is important thant the BarPoints are frozen before access here
        //                        /// => otherwise a 
        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        foreach (var f in ds.BarPoints)
        //                        {
        //                            SetVerticalBar(f, ds, width, 0);
        //                            a++;
        //                        }

        //                    }
        //                    catch
        //                    {

        //                    }

        //                    try
        //                    {
        //                        if (ds.EDH.CalculateDistribution)
        //                        {

        //                            double mean = ds.Values.Average();
        //                            double standardDeviation = ds.Values.StdDev();

        //                            ds.LineSeriesBar.SeriesName = ds.SeriesName;
        //                            ds.LineSeriesBar.Symbols.SymbolType = SymbolTypeEnum.None;

        //                            ds.LineSeriesBar.LineColor = ds.LineSeriesBar.LineColor == null ? ds.FillColor : ds.LineSeriesBar.LineColor;

        //                            for (double b = ds.Values.Min() - 0.5 * (ds.Values.Max() - ds.Values.Min()); b <= ds.Values.Max() + 0.5 * (ds.Values.Max() - ds.Values.Min()); b += (ds.Values.Max() - ds.Values.Min()) / 400)
        //                            {
        //                                double value = ds.EDH.GetNormalDistributionValue(mean, standardDeviation, b);
        //                                ds.LineSeriesBar.LinePoints2.Add(new LocationTimeValue(b, value));
        //                            }
        //                        }

        //                        SetLinesControl(new BindableCollection<LineSeries>() { ds.LineSeriesBar });
        //                    }
        //                    catch
        //                    {

        //                    }
        //                }
        //            }
        //            break;


        //        case BarTypeEnum.VerticalStack:
        //            if (nSeries > 1)
        //            {
        //                List<Point> temp = new List<Point>();

        //                double[] tempy = new double[BarCollection[0].BarPoints.Count];

        //                foreach (var ds in BarCollection)
        //                {
        //                    width = (ds.BarPoints[1].X - ds.BarPoints[0].X) * ds.BarWidth;

        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    ///It is important that the BarPoints are frozen before access here
        //                    /// => otherwise a 
        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        if (temp.Count > 0)
        //                        {
        //                            tempy[i] += temp[i].Y;
        //                        }

        //                        SetVerticalBar(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, tempy[i]);
        //                    }

        //                    temp.Clear();
        //                    temp.AddRange(ds.BarPoints);
        //                }
        //            }
        //            else
        //            {
        //                List<Point> temp = new List<Point>();

        //                double[] tempy = new double[BarCollection[0].BarPoints.Count];

        //                foreach (var ds in BarCollection)
        //                {
        //                    try
        //                    {
        //                        width = (ds.BarPoints[1].X - ds.BarPoints[0].X) * ds.BarWidth;

        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        ///It is important that the BarPoints are frozen before access here
        //                        /// => otherwise a 
        //                        ///!!!!!!!!!!!!!!!!!!!!!!
        //                        for (int i = 0; i < ds.BarPoints.Count; i++)
        //                        {
        //                            if (temp.Count > 0)
        //                            {
        //                                tempy[i] += temp[i].Y;
        //                            }

        //                            SetVerticalBar(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, tempy[i]);
        //                        }

        //                        temp.Clear();
        //                        temp.AddRange(ds.BarPoints);
        //                    }
        //                    catch
        //                    {
        //                        continue;
        //                    }
        //                }
        //            }
        //            break;
        //        //Setting up horizontal bars
        //        case BarTypeEnum.Horizontal:
        //            if (nSeries == 1)
        //            {
        //                foreach (var ds in BarCollection)
        //                {
        //                    width = YTick * ds.BarWidth;
        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        SetHorizontalBar(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, 0);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                int j = 0;

        //                foreach (var ds in BarCollection)
        //                {
        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    ///It is important thant the BarPoints are frozen before access here
        //                    /// => otherwise a 
        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        SetHorizontalBar1(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, nSeries, j);
        //                    }

        //                    j++;
        //                }
        //            }
        //            break;

        //        case BarTypeEnum.HorizontalOverlay:
        //            if (nSeries > 1)
        //            {
        //                int j = 0;
        //                foreach (var ds in BarCollection)
        //                {
        //                    width = YTick * ds.BarWidth;
        //                    width = width / Math.Pow(2, j);

        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        SetHorizontalBar(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, 0);
        //                    }
        //                    j++;
        //                }
        //            }
        //            break;

        //        case BarTypeEnum.HorizontalStack:
        //            if (nSeries > 1)
        //            {
        //                List<Point> temp = new List<Point>();
        //                double[] tempy = new double[BarCollection[0].BarPoints.Count];

        //                foreach (var ds in BarCollection)
        //                {
        //                    width = YTick * ds.BarWidth;

        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    ///It is important thant the BarPoints are frozen before access here
        //                    /// => otherwise a 
        //                    ///!!!!!!!!!!!!!!!!!!!!!!
        //                    for (int i = 0; i < ds.BarPoints.Count; i++)
        //                    {
        //                        if (temp.Count > 0)
        //                        {
        //                            tempy[i] += temp[i].X;
        //                        }

        //                        SetHorizontalBar(new Point(ds.BarPoints[i].X, ds.BarPoints[i].Y), ds, width, tempy[i]);
        //                    }

        //                    temp.Clear();
        //                    temp.AddRange(ds.BarPoints);
        //                }
        //            }
        //            break;
        //    }
        //}

        ////Setting the bars of the chart
        //public void SetBars(BindableCollection<SectionLayerSeries> LayerSeriesCollection)
        //{
        //    int nSeries = LayerSeriesCollection.Count;
        //    PolygonCollection.Clear();

        //    switch (BarType)
        //    {
        //        //Setting the lithological layers
        //        case BarTypeEnum.LithologicalSection:

        //            int j = 0;


        //            foreach (var sds in LayerSeriesCollection)
        //            {
        //                for (int i = 0; i < sds.LayerCollection.Count; i++)
        //                {
        //                    if (sds.LayerCollection[i].litsecBase != null && sds.LayerCollection[i].litsecTop != null)
        //                    {
        //                        SetHorizontalLayer(sds.LayerCollection[i], sds, j);
        //                    }
        //                }
        //                j++;

        //            }
        //            break;

        //    }
        //}

        ///// <summary>
        ///// Setting the vertical bars for certain bar chart types
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds">The bar series</param>
        ///// <param name="width">width of the bars</param>
        ///// <param name="y">Height of the bar</param>
        //private void SetVerticalBar(Point pt, BarSeries ds, double width, double y, string name = "")
        //{
        //    //Layouting the bar
        //    BarPolygon plg = new BarPolygon();
        //    plg.FillColor = ds.FillColor;
        //    plg.BorderColor = ds.BorderColor;
        //    plg.BorderThickness = ds.BorderThickness;

        //    //Setting the foot point to the middle of the bar 
        //    double x = pt.X + 0.5 * (ds.BarPoints[1].X - ds.BarPoints[0].X);

        //    //Defining the edges of the bar
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2, y)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + width / 2, y)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + width / 2, y + pt.Y)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2, y + pt.Y)));

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);

        //}

        ///// <summary>
        ///// Setting the vertical bars for certain bar chart types
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds">The bar series</param>
        ///// <param name="width">width of the bars</param>
        ///// <param name="y">Height of the bar</param>
        //private void SetVerticalBarSideBySide(Point pt,
        //    BarSeries ds,
        //    double width,
        //    int count,
        //    int totalCounts,
        //    string name = "")
        //{
        //    //Layouting the bar
        //    BarPolygon plg = new BarPolygon();
        //    plg.FillColor = ds.FillColor;
        //    plg.BorderColor = ds.BorderColor;
        //    plg.BorderThickness = ds.BorderThickness;
        //    count += 1;
        //    //Setting the foot point to the middle of the bar 
        //    double x = pt.X + (width * (totalCounts - count)) + 0.5 * width;

        //    //Defining the edges of the bar
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - (width / 2), 0)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + width / 2, 0)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + width / 2, pt.Y)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2, pt.Y)));

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);

        //}

        ///// <summary>
        ///// Setting the vertical bars for multiple bar chart types
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds">The bar series</param>
        ///// <param name="nSeries">Count of the series displayed in one bar column</param>
        ///// <param name="y">Number of the series (e.g. 1 of 3)</param>
        //private void SetVerticalBar1(Point pt, BarSeries ds, int nSeries, int n)
        //{
        //    //Layouting the bar
        //    BarPolygon plg = new BarPolygon();
        //    plg.FillColor = ds.FillColor;
        //    plg.BorderColor = ds.BorderColor;
        //    plg.BorderThickness = ds.BorderThickness;

        //    //Defining the width and height of the single bars in the bar series
        //    double width = 0.7 * XTick;
        //    double w1 = width / nSeries;
        //    double w = ds.BarWidth * w1;
        //    double space = (w1 - w) / 2;
        //    //Setting the foot point to the middle of the bar 
        //    double x = pt.X - 0.5 * XTick;

        //    //Defining the edges of the bar
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2 + space + n * w1, 0)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2 + space + n * w1 + 1, 0)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2 + space + n * w1 + w, pt.Y)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x - width / 2 + space + n * w1, pt.Y)));

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);
        //}

        ///// <summary>
        ///// Setting the horizontal bars for certain bar chart types
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds">The bar series</param>
        ///// <param name="width">width of the bars</param>
        ///// <param name="y">Height of the bar</param>
        //private void SetHorizontalBar(Point pt, BarSeries ds, double width, double x)
        //{
        //    //Layouting the bar
        //    BarPolygon plg = new BarPolygon();
        //    plg.FillColor = ds.FillColor;
        //    plg.BorderColor = ds.BorderColor;
        //    plg.BorderThickness = ds.BorderThickness;

        //    //Defining the central bottom point of the bar
        //    double y = pt.Y - 0.5 * YTick;

        //    //Defining the edges of the bar
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x, y - width / 2)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x, y + width / 2)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + pt.X, y + width / 2)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(x + pt.X, y - width / 2)));

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);
        //}

        ///// <summary>
        ///// Setting the horizontal bars
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds"></param>
        ///// <param name="nSeries"></param>
        ///// <param name="n"></param>
        //private void SetHorizontalBar1(Point pt, BarSeries ds, int nSeries, int n)
        //{
        //    //layouting the polygon
        //    BarPolygon plg = new BarPolygon();
        //    plg.FillColor = ds.FillColor;
        //    plg.BorderColor = ds.BorderColor;
        //    plg.BorderThickness = ds.BorderThickness;

        //    //Defining the width and height of the single bars in the bar series
        //    double width = 0.7 * YTick;
        //    double w1 = width / nSeries;
        //    double w = ds.BarWidth * w1;
        //    double space = (w1 - w) / 2;
        //    double y = pt.Y - 0.5 * YTick;

        //    //Defining the edges of the bar
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(0, y - width / 2 + space + n * w1)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(0, y - width / 2 + space + n * w1 + w)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(pt.X, y - width / 2 + space + n * w1 + w)));
        //    plg.PolygonPoints.Add(NormalizePoint(new Point(pt.X, y - width / 2 + space + n * w1)));

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);
        //}

        ///// <summary>
        ///// Setting the horizontal bars
        ///// </summary>
        ///// <param name="pt"></param>
        ///// <param name="ds"></param>
        ///// <param name="nSeries"></param>
        ///// <param name="n"></param>
        //private void SetHorizontalLayer(tblSectionLithofacy sec, SectionLayerSeries sls, double x)
        //{
        //    var converter = new System.Windows.Media.BrushConverter();

        //    //layouting the polygon
        //    BarPolygon plg = new BarPolygon();

        //    sec.litseclftCode = new ApirsRepository<tblFacy>().GetModelByExpression(z => z.facIdPk == sec.litseclftId).Select(z => z.facCode).FirstOrDefault();

        //    try
        //    {
        //        plg.FillColor = StringToFaciesTileConverter.Convert(sec.litseclftCode) ?? Brushes.LightGray;
        //    }
        //    catch
        //    {
        //        plg.FillColor = Brushes.LightGray;
        //    }

        //    plg.Name = sec.litseclftCode;
        //    plg.BorderColor = sls.BorderColor;
        //    plg.BorderThickness = sls.BorderThickness;
        //    GrainSizeToIntConverter gsi = new GrainSizeToIntConverter();

        //    double y = 0;

        //    //Defining the width and height of the single bars in the bar series
        //    if (sec.litsecThickness > 0)
        //    {
        //        y = ((double)sec.litsecBase + (double)sec.litsecTop) / 2;

        //        if (sec.litsecBaseType != "Erosive")
        //        {
        //            //Defining the edges of the bar
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y + (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y - (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeBase), y - (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeTop), y + (double)sec.litsecThickness / 2)));
        //        }
        //        else
        //        {
        //            //Defining the edges of the bar
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y + (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y - (double)sec.litsecThickness / 2)));

        //            int count1 = 60;
        //            double grainSize1 = (double)gsi.Convert(sec.litsecGrainSizeBase);

        //            for (double j = 0; j <= grainSize1 * 2 * Math.PI; j += grainSize1 * 2 * Math.PI / count1)
        //            {
        //                if (j != 0)
        //                {
        //                    plg.PolygonPoints.Add(NormalizePoint(new Point(x + grainSize1 * (j / (grainSize1 * 2 * Math.PI)), y - ((double)sec.litsecThickness / 2 + Math.Sin(j) / 60) - Math.Sin(1) / 60)));
        //                }
        //            }
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeTop), y + (double)sec.litsecThickness / 2)));
        //        }
        //    }
        //    else
        //    {
        //        y = -((double)sec.litsecBase + (double)sec.litsecTop) / 2;

        //        if (sec.litsecBaseType != "Erosive")
        //        {
        //            //Defining the edges of the bar
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y - (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y + (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeBase), y + (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeTop), y - (double)sec.litsecThickness / 2)));
        //        }
        //        else
        //        {
        //            //Defining the edges of the bar
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y - (double)sec.litsecThickness / 2)));
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x, y + (double)sec.litsecThickness / 2)));

        //            int count = 60;
        //            double grainSize = (double)gsi.Convert(sec.litsecGrainSizeBase);

        //            for (double j = 0; j <= grainSize * 2 * Math.PI; j += grainSize * 2 * Math.PI / count)
        //            {
        //                if (j != 0)
        //                {
        //                    plg.PolygonPoints.Add(NormalizePoint(new Point(x + grainSize * (j / (grainSize * 2 * Math.PI)), y + ((double)sec.litsecThickness / 2 + Math.Sin(j) / 60) - Math.Sin(1) / 60)));
        //                }
        //            }
        //            plg.PolygonPoints.Add(NormalizePoint(new Point(x + gsi.Convert(sec.litsecGrainSizeTop), y - (double)sec.litsecThickness / 2)));
        //        }
        //    }

        //    //Adding the bar to the PolygonCollection
        //    PolygonCollection.Add(plg);
        //}

        ////Adding style to the bars control
        //public void SetBarsControl(BindableCollection<BarSeries> BarSeries = null)
        //{

        //    GrainSizeToIntConverter gsi = new GrainSizeToIntConverter();

        //    // determine right offset:
        //    TextBlock tb1 = new TextBlock();
        //    tb1.Text = gsi.ConvertBack(Convert.ToInt32(Xmax));
        //    tb1.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

        //    Size size = tb1.DesiredSize;

        //    double offset = 20;
        //    double rightOffset = size.Width / 2 + 15;

        //    // Determine left offset through iterating through the whole y axis labels: 
        //    for (double dy = Ymin; dy <= Ymax; dy += YTick)
        //    {
        //        Point pt = NormalizePoint(new Point(Xmin, dy));
        //        tb1.Text = Math.Round(dy, 2).ToString();
        //        tb1.TextAlignment = TextAlignment.Right;
        //        tb1.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        //        size = tb1.DesiredSize;

        //        if (offset < size.Width)
        //            offset = size.Width;
        //    }

        //    double leftOffset = offset + 15;

        //    foreach (var bar in PolygonCollection)
        //    {
        //        int count = 0;

        //        Polygon rect = new Polygon();
        //        rect.Stroke = bar.BorderColor;
        //        rect.Fill = bar.FillColor;
        //        rect.StrokeThickness = bar.BorderThickness;
        //        rect.Points = bar.PolygonPoints;

        //        foreach (Point p in rect.Points)
        //        {
        //            if (rect.Points.Where(x => x.X == p.X && x.Y == p.Y).Count() > 1)
        //                count = 2;
        //        }

        //        if (count < 2)
        //            this.ChartCanvas.Children.Add(rect);

        //        count = 0;
        //    }
        //}

        //#endregion

        //#region Helpers

        ////Normalizes a point information based on the relative width and height of the canvas object
        //public Point NormalizePoint(Point pt)
        //{
        //    if (Double.IsNaN(ChartCanvas.Width) || ChartCanvas.Width <= 0)
        //        ChartCanvas.Width = 270;

        //    if (Double.IsNaN(ChartCanvas.Height) || ChartCanvas.Height <= 0)
        //        ChartCanvas.Height = 250;

        //    Point result = new Point();

        //    result.X = (pt.X - Xmin) * ChartCanvas.Width / (Xmax - Xmin);

        //    result.Y = ChartCanvas.Height - (pt.Y - Ymin) * ChartCanvas.Height / (Ymax - Ymin);

        //    if (double.IsInfinity(result.X) || double.IsInfinity(result.Y))
        //        return new Point(0, 0);

        //    return result;
        //}

        //#endregion
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
