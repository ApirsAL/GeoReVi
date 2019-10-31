using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace GeoReVi
{
    public class Chart3DChartStyle : ChartStyle
    {
        #region Methods

        /// <summary>
        /// Add a 3D line to a chart canvas
        /// </summary>
        /// <param name="cs"></param>
        public async Task<List<UIElement>> AddLine3D(LineSeries3D ls, ColormapBrush cm, CancellationToken token)
        {
            List<UIElement> symbols = new List<UIElement>();
            
            await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                var pLine = new Polyline();

                Matrix3D m = Chart3DHelper.AzimuthElevation(Elevation, Azimuth);

                Point3D[] pts = new Point3D[ls.LinePoints.Count];

                int stepWidth = 1;

                if (ls.LinePoints.Count > 20000)
                    stepWidth = (ls.LinePoints.Count / 20000);

                for (int i = 0; i < ls.LinePoints.Count; i += stepWidth)
                {
                    if(token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (i == 0)
                        Console.WriteLine("1:" + DateTime.Now);

                    await Task.Delay(1);

                   if (ls.LinePoints[i].Item1.X < Xmin || ls.LinePoints[i].Item1.X > Xmax 
                    || ls.LinePoints[i].Item1.Y < Ymin || ls.LinePoints[i].Item1.Y > Ymax
                    || ls.LinePoints[i].Item1.Z < Zmin || ls.LinePoints[i].Item1.Z > Zmax)
                                continue;


                    try
                    {
                        if (i == 0)
                            Console.WriteLine("2:" + DateTime.Now);

                        pts[i] = Normalize3D(m, ls.LinePoints[i].Item1);

                        pLine.Points.Add(new Point(pts[i].X, pts[i].Y));

                        if (i == 0)
                            Console.WriteLine("3:" + DateTime.Now);

                        if (!IsBubbleChart)
                        {
                            if (ls.Symbols.SymbolType != SymbolTypeEnum.None)
                                ls.Symbols.AddSymbol(ChartCanvas, new Point(pts[i].X, pts[i].Y));
                        }
                        else if (ls.Symbols.SymbolType != SymbolTypeEnum.None)
                        {
                            ls.Symbols.FillColor = ColorMapHelper.GetBrush(ls.LinePoints[i].Item2, cm.Ymin, cm.Ymax, cm);
                            symbols.AddRange(ls.Symbols.GetSymbol(new Point(pts[i].X, pts[i].Y)));
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    if (i == 0)
                        Console.WriteLine("4:" + DateTime.Now);
                }

                if (!IsBubbleChart)
                {
                    SetLinePattern(ls, pLine);
                    symbols.Add(pLine);
                }

            }).Result;

            return symbols;
        }

        public void SetLinePattern(LineSeries3D ls, Polyline pLine)
        {
            pLine.Stroke = ls.LineColor;
            pLine.StrokeThickness = ls.LineThickness;

            switch (ls.LinePattern)
            {
                case LinePatternEnum.Dash:
                    pLine.StrokeDashArray = new DoubleCollection(new double[2] { 4, 3 });
                    break;
                case LinePatternEnum.Dot:
                    pLine.StrokeDashArray = new DoubleCollection(new double[2] { 1, 2 });
                    break;
                case LinePatternEnum.DashDot:
                    pLine.StrokeDashArray = new DoubleCollection(new double[4] { 4, 2, 1, 2 });
                    break;
            }
        }

        #endregion
    }
}
