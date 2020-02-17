using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using MoreLinq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Point3D = System.Windows.Media.Media3D.Point3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using System.Diagnostics;

namespace GeoReVi
{
    /// <summary>
    /// A 3D chart object
    /// </summary>
    public class Chart3DObject : ChartObject<Series3D>
    {
        #region Properties

        /// <summary>
        /// The min value of the z coordinate
        /// </summary>
        private bool isZGrid = true;
        public bool IsZGrid
        {
            get => this.isZGrid;
            set
            {
                isZGrid = value;

                NotifyOfPropertyChange(() => IsZGrid);
            }
        }

        /// <summary>
        /// The min value of the z coordinate
        /// </summary>
        private double zmin;
        public double Zmin
        {
            get => this.zmin;
            set
            {
                zmin = value;

                NotifyOfPropertyChange(() => Zmin);
            }
        }

        /// <summary>
        /// Max value of the z coordinate
        /// </summary>
        private double zmax;
        public double Zmax
        {
            get => this.zmax;
            set
            {
                zmax = value;

                NotifyOfPropertyChange(() => Zmax);
            }

        }

        /// <summary>
        /// The maximum visible value
        /// </summary>
        private double maximumVisibility;
        public double MaximumVisibility
        {
            get => this.maximumVisibility;
            set
            {
                maximumVisibility = value;

                NotifyOfPropertyChange(() => MaximumVisibility);
            }
        }

        /// <summary>
        /// The minimum visible value
        /// </summary>
        private double minimumVisibility;
        public double MinimumVisibility
        {
            get => this.minimumVisibility;
            set
            {
                minimumVisibility = value;

                NotifyOfPropertyChange(() => MinimumVisibility);
            }
        }

        /// <summary>
        /// Tick step width in z direction
        /// </summary>
        private double zTick = 0.5;
        public double ZTick
        {
            get => zTick;
            set
            {
                zTick = value;

                NotifyOfPropertyChange(() => ZTick);
            }
        }

        /// <summary>
        /// Z label
        /// </summary>
        private Label zLabel = new Label() { Text = "z axis" };
        public Label ZLabel
        {
            get => zLabel;
            set
            {
                this.zLabel = value;

                NotifyOfPropertyChange(() => ZLabel);
            }
        }

        /// <summary>
        /// Elevation of the view
        /// </summary>
        private double elevation = 1;
        public double Elevation
        {
            get => elevation;
            set
            {
                elevation = value;

                NotifyOfPropertyChange(() => Elevation);
            }
        }

        /// <summary>
        /// Azimuth of the view
        /// </summary>
        private double azimuth = 1;
        public double Azimuth
        {
            get => azimuth;
            set
            {
                azimuth = value;

                NotifyOfPropertyChange(() => Azimuth);
            }
        }

        #region Runtime parameter
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

        private double maxz;
        public double Maxz
        {
            get => this.maxz;
            set => maxz = value;
        }

        private double minz;
        public double Minz
        {
            get => this.minz;
            set => minz = value;
        }
        #endregion

        /// <summary>
        /// The maximum visible value
        /// </summary>
        private double gridlineThickness = 0.1;
        public double GridlineThickness
        {
            get => this.gridlineThickness;
            set
            {
                gridlineThickness = value;

                NotifyOfPropertyChange(() => GridlineThickness);
            }
        }

        /// <summary>
        /// The label size
        /// </summary>
        private double labelSize = 1;
        public double LabelSize
        {
            get => this.labelSize;
            set
            {
                labelSize = value;

                if (DataCollection != null && DataCollection.Count() > 0)
                    UpdateChart();

                NotifyOfPropertyChange(() => LabelSize);
            }
        }

        /// <summary>
        /// The chart series
        /// </summary>
        private List<Series3D> ds = new List<Series3D>();
        public List<Series3D> Ds
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
        /// Check if color map
        /// </summary>
        private bool isColorMap = false;
        public bool IsColorMap
        {
            get => DataCollection.Any(x => x.IsColorMap);
        }

        /// <summary>
        /// Checks if values should be filtered according to a colorbar range
        /// </summary>
        private bool colorMapFilterActive = false;
        public bool ColorMapFilterActive
        {
            get => this.colorMapFilterActive;
            set
            {
                colorMapFilterActive = value;

                NotifyOfPropertyChange(() => ColorMapFilterActive);
            }
        }

        /// <summary>
        /// A plane that is used for slicing
        /// </summary>
        private Plane3D plane3D = new Plane3D();
        public Plane3D Plane3D
        {
            get => this.plane3D;
            set
            {
                this.plane3D = value;
                NotifyOfPropertyChange(() => Plane3D);
            }
        }

        /// <summary>
        /// ColorMap
        /// </summary>
        private ColormapBrush colorMap;
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
        /// Excluding the origin from the analysis
        /// </summary>
        private bool excludeOrigin = true;
        public bool ExcludeOrigin
        {
            get => this.excludeOrigin;
            set
            {
                this.excludeOrigin = value;
                NotifyOfPropertyChange(() => ExcludeOrigin);
            }
        }

        #region HelixToolkit Properties


        /// <summary>
        /// Camera of the view
        /// </summary>
        private System.Windows.Media.Media3D.PerspectiveCamera camera = new System.Windows.Media.Media3D.PerspectiveCamera();
        [XmlIgnore]
        public System.Windows.Media.Media3D.PerspectiveCamera Camera
        {
            get => this.camera;
            set
            {
                this.camera = value;
                NotifyOfPropertyChange(() => Camera);
            }
        }

        /// <summary>
        /// XZ plane gridlines
        /// </summary>
        private GridLinesVisual3D xZgridlines = new GridLinesVisual3D();
        [XmlIgnore]
        public GridLinesVisual3D XZGridlines
        {
            get => this.xZgridlines;
            set
            {
                this.xZgridlines = value;
                NotifyOfPropertyChange(() => XZGridlines);
            }
        }

        /// <summary>
        /// XY plane gridlines
        /// </summary>
        private GridLinesVisual3D xYgridlines = new GridLinesVisual3D();
        [XmlIgnore]
        public GridLinesVisual3D XYGridlines
        {
            get => this.xYgridlines;
            set
            {
                this.xYgridlines = value;
                NotifyOfPropertyChange(() => XYGridlines);
            }
        }

        /// <summary>
        /// YZ plane gridlines
        /// </summary>
        private GridLinesVisual3D yZgridlines = new GridLinesVisual3D();
        [XmlIgnore]
        public GridLinesVisual3D YZGridlines
        {
            get => this.yZgridlines;
            set
            {
                this.yZgridlines = value;
                NotifyOfPropertyChange(() => YZGridlines);
            }
        }

        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        private Model3DGroup model;
        [XmlIgnore]
        public Model3DGroup Model
        {
            get => this.model;
            set
            {
                this.model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        /// <value>The model.</value>
        private BillboardTextGroupVisual3D labels;
        [XmlIgnore]
        public BillboardTextGroupVisual3D Labels
        {
            get => this.labels;
            set
            {
                this.labels = value;
                NotifyOfPropertyChange(() => Labels);
            }
        }

        /// <summary>
        /// Exaggeration factor
        /// </summary>
        private double exaggerationFactor = 1;
        public double ExaggerationFactor
        {
            get => this.exaggerationFactor;
            set
            {
                this.exaggerationFactor = value;
                NotifyOfPropertyChange(() => ExaggerationFactor);
            }
        }

        /// <summary>
        /// The minimum longitude value
        /// </summary>
        private double minEast = 999999999999999999;
        public double MinEast
        {
            get => this.minEast;
            set
            {
                this.minEast = value;
                NotifyOfPropertyChange(() => MinEast);
            }
        }

        /// <summary>
        /// Minimum latitude in data set
        /// </summary>
        private double minNorth = 999999999999999999;
        public double MinNorth
        {
            get => this.minNorth;
            set
            {
                this.minNorth = value;
                NotifyOfPropertyChange(() => MinNorth);
            }
        }

        /// <summary>
        /// Minimum altitude in data set
        /// </summary>
        double minAltitude = 999999999999999999;
        public double MinAltitude
        {
            get => this.minAltitude;
            set
            {
                this.minAltitude = value;
                NotifyOfPropertyChange(() => MinAltitude);
            }
        }


        /// <summary>
        /// A 3D Editor
        /// </summary>
        private ThreeDEditor threeDEditor = new ThreeDEditor();
        [XmlIgnore]
        public ThreeDEditor ThreeDEditor
        {
            get => threeDEditor;
            set
            {
                this.threeDEditor = value;
                NotifyOfPropertyChange(() => ThreeDEditor);
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Specific constructor
        /// </summary>
        /// <param name="_ch"></param>
        public Chart3DObject(Chart3DObject _ch)
        {
            DataCollection = new BindableCollection<Series3D>();
            ColorMap = new ColormapBrush();

            DataSet = _ch.DataSet;
            Title = _ch.Title;
            GridlineColor = _ch.GridlineColor;
            GridlinePattern = _ch.GridlinePattern;
            Legend.IsLegend = _ch.Legend.IsLegend;
            Legend.LegendPosition = _ch.Legend.LegendPosition;
            ShallRender = _ch.ShallRender;
            Elevation = _ch.Elevation;
            Azimuth = _ch.Azimuth;

            ChartHeight = _ch.ChartHeight;
            ChartWidth = _ch.ChartWidth;

            Y2max = _ch.Y2max;
            Y2min = _ch.Y2min;
            Y2Tick = _ch.Y2Tick;

            Ymax = _ch.Ymax;
            Ymin = _ch.Ymin;
            YTick = _ch.YTick;
            YLabel = _ch.YLabel;
            YLabels = _ch.YLabels;
            IsYLog = _ch.IsYLog;
            IsYGrid = _ch.IsYGrid;

            XLabel = _ch.XLabel;
            XLabels = _ch.XLabels;
            Xmax = _ch.Xmax;
            Xmin = _ch.Xmin;
            XTick = _ch.XTick;
            IsXLog = _ch.IsXLog;
            IsXGrid = _ch.IsXGrid;

            ZLabel = _ch.ZLabel;
            Zmax = _ch.Zmax;
            Zmin = _ch.Zmin;
            ZTick = _ch.ZTick;

            ColorMap = _ch.ColorMap;
            Ds = _ch.Ds;

            Maxx = _ch.Maxx;
            Maxy = _ch.Maxy;
            Maxz = _ch.Maxz;
            Minx = _ch.Minx;
            Miny = _ch.Miny;
            Minz = _ch.Minz;

            BarType = _ch.BarType;

            DataCollection = _ch.DataCollection;
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public Chart3DObject()
        {
            DataCollection = new BindableCollection<Series3D>();
            ColorMap = new ColormapBrush();
        }

        #endregion

        #region Methods

        public override void Initialize()
        {
            Maxx = 0;
            Minx = 0;
            Maxy = 0;
            Miny = 0;
        }

        /// <summary>
        /// Adding a data series to the chart object
        /// </summary>
        public virtual void AddDataSeries()
        {
            Series3D i = new Series3D();
            i.Symbols.FillColor = System.Windows.Media.Brushes.DarkBlue;
            i.Symbols.SymbolSize = 6;
            i.Symbols.SymbolType = SymbolTypeEnum.Dot;
            i.Symbols.BorderThickness = 0;
            i.LineColor = IntToColorConverter.Convert(0);

            i.LinePattern = LinePatternEnum.None;
            i.LineThickness = 1.5;

            Ds.Add(i);
        }

        /// <summary>
        /// Performs one editing operation
        /// </summary>
        public async Task PerformEditing()
        {

            //return if not editing
            if (!ThreeDEditor.Editing)
                return;

            //return if no point is selected
            if (ThreeDEditor.CursorOnElementPosition.HasValue == false)
                return;

            //Performing the interpolation
            CommandHelper ch = new CommandHelper();
            await Task.WhenAll(ch.RunBackgroundWorkerHelperAsync(async () =>
            {

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                try
                {
                    switch (ThreeDEditor.EditingType)
                    {
                        case EditingTypeEnum.SelectPoints:

                            break;
                        //Adding a point to a selected data sets
                        case EditingTypeEnum.AddPoints:
                            try
                            {
                                LocationTimeValue loc = new LocationTimeValue(ThreeDEditor.CursorOnElementPosition.Value.X + MinEast,
                                                                              ThreeDEditor.CursorOnElementPosition.Value.Y + MinNorth,
                                                                              ThreeDEditor.CursorOnElementPosition.Value.Z + MinAltitude);

                                ThreeDEditor.AddedPoints.Add(loc);

                                // Create a mesh builder and add a sphere to it
                                var meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                                meshBuilder.AddSphere(ThreeDEditor.CursorOnElementPosition.Value, SelectedSeries.Symbols.SymbolSize, 10, 10);

                                var mesh = meshBuilder.ToMesh(true);

                                var material = MaterialHelper.CreateMaterial(Colors.Black);

                                Model.Dispatcher.Invoke(()=> Model.Children.Add(new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material }));
                            }
                            catch
                            {

                            }
                            break;
                        //Extracting a vertical section from the selected model
                        case EditingTypeEnum.ExtractVerticalSection:
                            try
                            {

                                LocationTimeValue loc = new LocationTimeValue(ThreeDEditor.CursorOnElementPosition.Value.X + MinEast,
                                                                              ThreeDEditor.CursorOnElementPosition.Value.Y + MinNorth,
                                                                              ThreeDEditor.CursorOnElementPosition.Value.Z + MinAltitude);

                                ThreeDEditor.AddedPoints.Add(loc);

                                // Create a mesh builder and add a sphere to it
                                var meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                                meshBuilder.AddSphere(ThreeDEditor.CursorOnElementPosition.Value, SelectedSeries.Symbols.SymbolSize, 10, 10);

                                var mesh = meshBuilder.ToMesh(true);

                                var material = MaterialHelper.CreateMaterial(Colors.Black);

                                new DispatchService().Invoke(() =>
                                {
                                    Model.Dispatcher.InvokeAsync(() => Model.Children.Add(new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material }));
                                });

                                if (ThreeDEditor.AddedPoints.Count == 2)
                                {
                                    Task.WaitAll(FinishEditing());
                                    return;
                                }
                            }
                            catch
                            {

                            }
                            break;
                        case EditingTypeEnum.ExtractHorizontalSection:
                            break;
                        case EditingTypeEnum.ExtractVerticalProfile:
                            break;
                        case EditingTypeEnum.ExtractHorizontalProfile:
                            break;

                    }
                }
                catch
                {
                    throw new Exception("Editing operation ended with exception.");
                }
            }));
        }

        /// <summary>
        /// Finishes the editing mode
        /// </summary>
        public async Task FinishEditing()
        {
            if (!ThreeDEditor.Editing)
                return;

            try
            {
                switch (ThreeDEditor.EditingType)
                {
                    case EditingTypeEnum.SelectPoints:

                        break;
                    //Adding a point to a selected data sets
                    case EditingTypeEnum.AddPoints:
                        SelectedSeries.Mesh.Vertices.AddRange(ThreeDEditor.AddedPoints);
                        for (int i = 0; i < ThreeDEditor.AddedPoints.Count(); i++)
                        {
                            await Task.Delay(0);
                            try
                            {
                                DataRow dr = SelectedSeries.Mesh.Data.NewRow();
                                dr[0] = ThreeDEditor.AddedPoints[i].Value[0];
                                dr[1] = ThreeDEditor.AddedPoints[i].X;
                                dr[2] = ThreeDEditor.AddedPoints[i].Y;
                                dr[3] = ThreeDEditor.AddedPoints[i].Z;
                                dr[4] = ThreeDEditor.AddedPoints[i].Date;
                                dr[5] = ThreeDEditor.AddedPoints[i].Name;

                                SelectedSeries.Mesh.Data.Rows.Add(dr);
                            }
                            catch
                            {

                            }
                        }
                        break;
                    //Extracting a vertical section from the selected model
                    case EditingTypeEnum.ExtractVerticalSection:
                        DataSet.Add(await SelectedSeries.Mesh.ExtractSection(ThreeDEditor.AddedPoints[0].ToPoint3D(), ThreeDEditor.AddedPoints[1].ToPoint3D(), ThreeDEditor.VerticalCellCount, ThreeDEditor.HorizontalCellCount, true));
                        AddDataToSeries();
                        break;
                    case EditingTypeEnum.ExtractHorizontalSection:
                        break;
                    case EditingTypeEnum.ExtractVerticalProfile:
                        break;
                    case EditingTypeEnum.ExtractHorizontalProfile:
                        break;

                }
            }
            catch
            {

            }

            ThreeDEditor.Editing = false;
        }

        /// <summary>
        /// Removes the selected data set and series
        /// </summary>
        public override void RemoveSelectedSeries()
        {
            try
            {
                if (SelectedSeries != null)
                {
                    SelectedDataSet = DataSet.Where(x => x.Data.Equals(SelectedSeries.Mesh.Data)).FirstOrDefault();
                    if (SelectedDataSet == null)
                    {
                        DataCollection.Remove(SelectedSeries);
                        return;
                    }

                    int index = DataSet.IndexOf(SelectedDataSet);
                    DataSet.RemoveAt(index);
                    index = DataCollection.IndexOf(SelectedSeries);
                    DataCollection.RemoveAt(index);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Adds the data series based on the data set to the chart object
        /// </summary>
        public void AddDataToSeries()
        {
            try
            {
                if (!ShallRender)
                    return;

                DataCollection = new BindableCollection<Series3D>(DataCollection.Where(x => x.Chart3DDisplayType == Chart3DDisplayType.Model).ToList());

                Ds.Clear();

                for (int i = 0; i < DataSet.Count; i++)
                    try
                    {
                        if (DataSet[i].Data.Rows.Count != DataSet[i].Vertices.Count())
                        {
                            DataSet[i].Vertices.Clear();
                            DataSet[i].Vertices.AddRange(DataSet[i].Data.AsEnumerable()
                                .Select(x => new LocationTimeValue()
                                {
                                    Value = new List<double>() { (x.Field<double?>(0) == -9999 || x.Field<double?>(0) == -999999 || x.Field<double?>(0) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(0)) },
                                    X = (x.Field<double?>(1) == -9999 || x.Field<double?>(1) == -999999 || x.Field<double?>(1) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(1)),
                                    Y = (x.Field<double?>(2) == -9999 || x.Field<double?>(2) == -999999 || x.Field<double?>(2) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(2)),
                                    Z = (x.Field<double?>(3) == -9999 || x.Field<double?>(3) == -999999 || x.Field<double?>(3) == 9999999) ? 0 : Convert.ToDouble(x.Field<double?>(3))
                                }));
                        }

                        ///Only add the mesh to the chart series if it is not yet included
                        if (!Ds.Any(x => x.Mesh.Equals(DataSet[i])))
                        {
                            var ds = new Series3D();

                            ds.Mesh = DataSet[i];

                            Ds.Add(ds);

                            Ds[i].SeriesName = DataSet[i].Name;
                        }
                    }
                    catch
                    {
                        i++;
                        continue;
                    }


                XLabel.Text = "Local x [m]";
                YLabel.Text = "Local y [m]";
                ZLabel.Text = "Local z [m]";
                Title = "Parameter";
            }
            catch
            {

            }

            CreateChart();
        }


        /// <summary>
        /// Creates the chart
        /// </summary>
        public virtual void CreateChart()
        {
            try
            {
                if (ColorMap.Ymax == 10)
                    ColorMap.Ymax = Ds.Max(x => x.Mesh.Vertices.Max(y => y.Value[0]));

                if (ColorMap.Ymin == 0)
                    ColorMap.Ymin = Ds.Min(x => x.Mesh.Vertices.Min(y => y.Value[0]));

                if (MaximumVisibility == 0)
                    MaximumVisibility = ColorMap.Ymax;

                if (MinimumVisibility == 0)
                    MinimumVisibility = ColorMap.Ymin;

                if (Xmin == 0 && Xmax == 0 && Zmin == 0 && Zmax == 0 && Ymin == 0 && Ymax == 0)
                    SubdivideAxes();

            }
            catch
            {

            }

            DataCollection.AddRange(Ds);
        }

        /// <summary>
        /// Adding a data collection
        /// </summary>
        public async Task AddDataCollection()
        {
            CommandHelper ch = new CommandHelper();

            if (ThreeDEditor.Editing)
                return;

            var modelGroup = new Model3DGroup();

            Model = new Model3DGroup();

            await Task.WhenAll(await Application.Current.Dispatcher.InvokeAsync(async () =>
            {
                // Create a mesh builder and add a box to it
                var meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                var mesh = meshBuilder.ToMesh(true);

                var material = MaterialHelper.CreateMaterial(Colors.Black);

                //Determining the minimum x, y and z value for model translation
                MinEast = 999999999999999999;
                MinNorth = 999999999999999999;
                MinAltitude = 999999999999999999;

                //Define minimum and maximum values for scaling
                DataCollection.ForEach(x =>
                {
                    if (x.Display != false)
                    {
                        if (x.Mesh.Vertices.Count() > 0)
                        {
                            MinEast = x.Mesh.Vertices.Select(a => a.X).Min() <= minEast ? x.Mesh.Vertices.Select(a => a.X).Min() : minEast;
                            MinNorth = x.Mesh.Vertices.Select(a => a.Y).Min() <= minNorth ? x.Mesh.Vertices.Select(a => a.Y).Min() : minNorth;
                            MinAltitude = x.Mesh.Vertices.Select(a => a.Z).Min() <= minNorth ? x.Mesh.Vertices.Select(a => a.Z).Min() : minNorth;
                        }
                    }
                });

                //Check if it should be visible
                if (IsColorMap)
                {
                    SetBrush(ColorMap.CalculateColormapBrushes());
                    SetColorMapLabels();
                }

                //Adding each data collection item to the model group
                foreach (Series3D ls3D in DataCollection)
                {
                    if (ls3D.Display == false)
                        continue;

                    await Task.Delay(0);

                    //Instantiating objects that hold the visual objects that should be added to the chart
                    List<Tuple<LocationTimeValue, SolidColorBrush>> pointMaterials = new List<Tuple<LocationTimeValue, SolidColorBrush>>();
                    List<Tuple<Face, SolidColorBrush>> faceMaterials = new List<Tuple<Face, SolidColorBrush>>();
                    List<SolidColorBrush> materials = new List<SolidColorBrush>();

                    try
                    {

                        if (ls3D.Chart3DDisplayType != Chart3DDisplayType.Model)
                            ls3D.Model = new Model3DGroup();

                        //Adding point clould of the series
                        if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Scatter)
                        {
                            //Adding all points to the model group
                            foreach (var point in ls3D.Mesh.Vertices)
                            {
                                await Task.Delay(0);

                                try
                                {
                                    //Check if it should be visible
                                    if (ls3D.IsColorMap && ColorMapFilterActive)
                                    {
                                        if (point.Value[0] < MinimumVisibility || point.Value[0] > MaximumVisibility)
                                            continue;
                                    }

                                    //Filter values
                                    if (Plane3D.IsActive == true)
                                    {
                                        switch (Plane3D.RelativeOrientation)
                                        {
                                            case RelativeOrientation.Above:
                                                if (Plane3D.PointRelativeToPlane(point.ToVector3D()) > 0)
                                                    continue;
                                                break;
                                            case RelativeOrientation.Below:
                                                if (Plane3D.PointRelativeToPlane(point.ToVector3D()) < 0)
                                                    continue;
                                                break;
                                            default:
                                                if (Plane3D.PointRelativeToPlane(point.ToVector3D()) == 0)
                                                    continue;
                                                break;
                                        }
                                    }

                                    SolidColorBrush m = ls3D.IsColorMap ? ColorMapHelper.GetBrush(point.Value[0], ColorMap.Ymin, ColorMap.Ymax, ColorMap) : (SolidColorBrush)ls3D.Symbols.FillColor;

                                    pointMaterials.Add(new Tuple<LocationTimeValue, SolidColorBrush>(point, m));

                                    materials.Add(m);
                                }
                                catch
                                {

                                }
                            }
                        }
                        else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Gradient)
                        {
                            Material mat = MaterialHelper.CreateMaterial(Brushes.Black);
                            List<Tuple<Point3D, double[]>> gradients = new List<Tuple<Point3D, double[]>>();

                            // Create a mesh builder and add a box to it
                            meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                            Task.WaitAll(ls3D.Mesh.CreateVerticeArray());

                            //Adding all points to the model group
                            for (int i = 0; i < ls3D.Mesh.Vertices.Count(); i += 4)
                            {
                                await Task.Delay(0);

                                try
                                {
                                    switch (ls3D.Mesh.Dimensionality)
                                    {
                                        case Dimensionality.ThreeD:
                                            if (ls3D.Mesh.Vertices[i].IsExterior)
                                                continue;

                                            Point3D central = ls3D.Mesh.Vertices[i].ToPoint3D();

                                            gradients.Add(new Tuple<Point3D, double[]>(central, ls3D.Mesh.CalculateGradientFunction(ls3D.Mesh.Vertices[i])));
                                            break;
                                        case Dimensionality.TwoD:
                                            Point3D central1 = ls3D.Mesh.Vertices[i].ToPoint3D();
                                            Vector3D vec = ls3D.Mesh.CalculateGradient2D(ls3D.Mesh.Vertices[i]);
                                            gradients.Add(new Tuple<Point3D, double[]>(central1, new double[3] { vec.X, vec.Y, vec.Z }));
                                            break;

                                    }

                                }
                                catch
                                {
                                    continue;
                                }

                            }

                            //Maximum magnitudes of the gradients
                            List<double> maxGradients = new List<double>()
                            {
                                gradients.Max(x => Math.Abs(x.Item2[0])),
                                gradients.Max(x => Math.Abs(x.Item2[1])),
                                gradients.Max(x => Math.Abs(x.Item2[2]))
                            };

                            double max = maxGradients.Max();

                            gradients.ForEach(x =>
                            {
                                //Normalizing each vector based on the symbol size
                                x.Item2[0] = 0.5 * (x.Item2[0] / max) * ls3D.Symbols.SymbolSize;
                                x.Item2[1] = 0.5 * (x.Item2[1] / max) * ls3D.Symbols.SymbolSize;
                                x.Item2[2] = 0.5 * (x.Item2[2] / max) * ls3D.Symbols.SymbolSize;

                                //Adding arrow to the mesh builder
                                meshBuilder.AddArrow(
                                      new Point3D(x.Item1.X - 0.5 * x.Item2[0],
                                                  x.Item1.Y - 0.5 * x.Item2[1],
                                                  x.Item1.Z - 0.5 * x.Item2[2]),
                                      new Point3D(x.Item1.X + 0.5 * x.Item2[0],
                                                  x.Item1.Y + 0.5 * x.Item2[1],
                                                  x.Item1.Z + 0.5 * x.Item2[2]),
                                      ls3D.WireframeThickness,
                                      ls3D.WireframeThickness,
                                      18);
                            });

                            mesh = meshBuilder.ToMesh(true);
                            ls3D.Model.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D { Geometry = mesh, Material = mat, BackMaterial = mat });

                        }
                        else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Surface)
                        {

                            ls3D.Mesh.FacesFromPointCloud();

                            ///Adding all points to the model group
                            foreach (var face in ls3D.Mesh.Faces)
                            {
                                try
                                {
                                    await Task.Delay(0);

                                    double average = face.Vertices.Select(x => x.Value[0]).Average();

                                    //Filtering values based on minimum and maximum visibility
                                    if (ColorMapFilterActive)
                                        if (average < MinimumVisibility || average > MaximumVisibility)
                                            continue;

                                    //Filter values
                                    if (Plane3D.IsActive == true)
                                    {
                                        foreach (var vertice in face.Vertices)
                                            switch (Plane3D.RelativeOrientation)
                                            {
                                                case RelativeOrientation.Above:
                                                    if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) > 0)
                                                        continue;
                                                    break;
                                                case RelativeOrientation.Below:
                                                    if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) < 0)
                                                        continue;
                                                    break;
                                                default:
                                                    if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) == 0)
                                                        continue;
                                                    break;
                                            }
                                    }

                                    SolidColorBrush m = ls3D.IsColorMap ? ColorMapHelper.GetBrush(average, ColorMap.Ymin, ColorMap.Ymax, ColorMap) : (SolidColorBrush)ls3D.Symbols.FillColor;

                                    faceMaterials.Add(new Tuple<Face, SolidColorBrush>(face, m));

                                    if (materials.Where(x => x.Color == m.Color).Count() == 0)
                                        materials.Add(m);
                                }
                                catch
                                {

                                }

                            }
                        }
                        else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Volumetric)
                        {
                            await Task.Delay(0);

                            //ls3D.Mesh.CellsFromPointCloud();

                            foreach (var cell in ls3D.Mesh.Cells)
                            {

                                double average = cell.Vertices.Select(x => x.Value[0]).Average();

                                //Filtering values based on minimum and maximum visibility
                                if (ColorMapFilterActive)
                                    if (average < MinimumVisibility || average > MaximumVisibility)
                                        continue;

                                if (cell.Faces.Count < 1)
                                    cell.CreateFaces();


                                ///Adding all points to the model group
                                if (cell.Vertices.Where(x => x.IsExterior).Count() > 0)
                                    foreach (var face in cell.Faces)
                                    {
                                        try
                                        {
                                            await Task.Delay(0);

                                            bool isFiltered = false;

                                            if (!face.Vertices.Any(x => x.IsExterior))
                                                continue;

                                            //Filter values
                                            if (Plane3D.IsActive == true)
                                            {
                                                foreach (var vertice in face.Vertices)
                                                    switch (Plane3D.RelativeOrientation)
                                                    {
                                                        case RelativeOrientation.Above:
                                                            if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) > 0)
                                                                isFiltered = true; ;
                                                            break;
                                                        case RelativeOrientation.Below:
                                                            if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) < 0)
                                                                isFiltered = true;
                                                            break;
                                                        default:
                                                            if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) == 0)
                                                                isFiltered = true;
                                                            break;
                                                    }
                                            }

                                            if (isFiltered)
                                                continue;

                                            //Defining the color for the face
                                            SolidColorBrush m = ls3D.IsColorMap ? ColorMapHelper.GetBrush(average, ColorMap.Ymin, ColorMap.Ymax, ColorMap) : (SolidColorBrush)ls3D.Symbols.FillColor;

                                            //Adding the face to the face objects
                                            faceMaterials.Add(new Tuple<Face, SolidColorBrush>(face, m));

                                            //Adding color to new materials if it is not included yet
                                            if (materials.Where(x => x.Color == m.Color).Count() == 0)
                                                materials.Add(m);
                                        }
                                        catch
                                        {

                                        }

                                    }
                                else if (Plane3D.IsActive == true)
                                {
                                    int verticesCut = 0;

                                    foreach (var vertice in cell.Vertices)
                                        switch (Plane3D.RelativeOrientation)
                                        {
                                            case RelativeOrientation.Above:
                                                if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) > 0)
                                                    verticesCut += 1;
                                                break;
                                            case RelativeOrientation.Below:
                                                if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) < 0)
                                                    verticesCut += 1;
                                                break;
                                            default:
                                                if (Plane3D.PointRelativeToPlane(vertice.ToVector3D()) == 0)
                                                    verticesCut += 1;
                                                break;
                                        }

                                    if ((verticesCut > 0 && verticesCut < cell.Vertices.Count()) || (cell.Vertices.Any(x => x.IsExterior) && verticesCut < cell.Vertices.Count()))
                                    {
                                        foreach (var face in cell.Faces)
                                        {
                                            SolidColorBrush m = ls3D.IsColorMap ? ColorMapHelper.GetBrush(average, ColorMap.Ymin, ColorMap.Ymax, ColorMap) : (SolidColorBrush)ls3D.Symbols.FillColor;

                                            faceMaterials.Add(new Tuple<Face, SolidColorBrush>(face, m));

                                            if (materials.Where(x => x.Color == m.Color).Count() == 0)
                                                materials.Add(m);
                                        }
                                    }

                                }
                            }
                        }
                        //Add image
                        else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Image)
                        {
                            meshBuilder = new MeshBuilder(false, true);

                            IList<Point3D> pnts = new List<Point3D>();

                            //Creating the projection plane
                            pnts.Add(ls3D.Image.LowerLeft.ToPoint3D());
                            pnts.Add(ls3D.Image.LowerRight.ToPoint3D());
                            pnts.Add(ls3D.Image.UpperRight.ToPoint3D());
                            pnts.Add(ls3D.Image.UpperLeft.ToPoint3D());

                            meshBuilder.AddPolygon(pnts);

                            mesh = meshBuilder.ToMesh(false);

                            ImageBrush brush = new ImageBrush();
                            brush.ImageSource = new BitmapImage(new Uri(@ls3D.Image.ImagePath, UriKind.Relative));

                            // Create a 45 rotate transform about the brush's center
                            // and apply it to the brush's RelativeTransform property.
                            RotateTransform aRotateTransform = new RotateTransform();
                            aRotateTransform.CenterX = 0.5;
                            aRotateTransform.CenterY = 0.5;
                            aRotateTransform.Angle = 90;
                            brush.RelativeTransform = aRotateTransform;

                            //Adding the texture coordinates of the image to the mesh
                            PointCollection pntCol = new PointCollection();
                            pntCol.Add(new System.Windows.Point(0, 0));
                            pntCol.Add(new System.Windows.Point(0, brush.ImageSource.Height));    //Tile image height/width is 204
                            pntCol.Add(new System.Windows.Point(brush.ImageSource.Width, brush.ImageSource.Height));
                            pntCol.Add(new System.Windows.Point(brush.ImageSource.Width, 0));
                            mesh.TextureCoordinates = pntCol;

                            //Defining viewport and tile type for the projection
                            brush.Viewport = new Rect(0, 0, brush.ImageSource.Width, brush.ImageSource.Height);
                            brush.TileMode = TileMode.Tile;
                            brush.ViewportUnits = BrushMappingMode.Absolute;
                            brush.ViewboxUnits = BrushMappingMode.Absolute;
                            brush.Stretch = Stretch.None;
                            brush.AlignmentX = AlignmentX.Left;
                            brush.AlignmentY = AlignmentY.Top;
                            DiffuseMaterial mat = new DiffuseMaterial(brush);

                            ls3D.Model.Children.Add(new GeometryModel3D { Geometry = mesh, Material = mat, BackMaterial = mat });

                        }

                        materials = materials.DistinctBy(x => x.Color).ToList();

                        foreach (SolidColorBrush s in materials)
                        {

                            Material mat = MaterialHelper.CreateMaterial(s);

                            // Create a mesh builder and add a box to it
                            meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                            mesh = new System.Windows.Media.Media3D.MeshGeometry3D();

                            List<Tuple<LocationTimeValue, SolidColorBrush>> a = pointMaterials.Where(x => x.Item2 == s).ToList();
                            List<Tuple<Face, SolidColorBrush>> b = faceMaterials.Where(x => x.Item2 == s).ToList();

                            for (int i = 0; i < a.Count(); i++)
                            {
                                await Task.Delay(0);

                                switch (ls3D.Symbols.SymbolType)
                                {
                                    case SymbolTypeEnum.Box:
                                        meshBuilder.AddBox(a[i].Item1.ToPoint3D(), ls3D.Symbols.SymbolSize, ls3D.Symbols.SymbolSize, ls3D.Symbols.SymbolSize, HelixToolkit.Wpf.BoxFaces.All);
                                        break;
                                    case SymbolTypeEnum.Dot:
                                        meshBuilder.AddSphere(a[i].Item1.ToPoint3D(), ls3D.Symbols.SymbolSize, 10, 5);
                                        break;
                                }
                            }

                            for (int i = 0; i < b.Count(); i++)
                            {
                                await Task.Delay(0);

                                switch (b[i].Item1.FaceType)
                                {
                                    case FaceType.Triangular:
                                        meshBuilder.AddTriangle(b[i].Item1.Vertices[0].ToPoint3D(), b[i].Item1.Vertices[1].ToPoint3D(), b[i].Item1.Vertices[2].ToPoint3D());
                                        break;
                                    case FaceType.Quadrilateral:
                                        ((Quadrilateral)b[i].Item1).SortVertices();
                                        meshBuilder.AddQuad(b[i].Item1.Vertices[0].ToPoint3D(), b[i].Item1.Vertices[1].ToPoint3D(), b[i].Item1.Vertices[2].ToPoint3D(), b[i].Item1.Vertices[3].ToPoint3D());
                                        break;
                                }
                            }

                            mesh = meshBuilder.ToMesh(true);

                            ///Adding the mesh to the model group
                            if (ls3D.MeshDisplayType == MeshDisplayType.Faces || ls3D.MeshDisplayType == MeshDisplayType.WireFrameAndFaces)
                            {
                                material = mat;
                                ls3D.Model.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material });
                            }
                            if (ls3D.MeshDisplayType == MeshDisplayType.WireFrame || ls3D.MeshDisplayType == MeshDisplayType.WireFrameAndFaces)
                            {
                                ///Adding a wireframe
                                ls3D.Model.Children.Add(CreateWireframe(mesh, ls3D.WireframeThickness));
                            }
                            if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Line)
                            {
                                pointMaterials.OrderBy(x => x.Item1.X).OrderBy(x => x.Item1.Y).OrderBy(x => x.Item1.Z);

                                // Create a mesh builder and add a box to it
                                meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

                                for (int i = 0; i < pointMaterials.Count() - 1; i++)
                                    meshBuilder.AddCylinder(pointMaterials[i].Item1.ToPoint3D(), pointMaterials[i + 1].Item1.ToPoint3D(), ls3D.WireframeThickness, 5, false, false);

                                mesh = meshBuilder.ToMesh(true);

                                material = MaterialHelper.CreateMaterial(ls3D.LineColor);

                                ls3D.Model.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D { Geometry = mesh, Material = material });
                            }

                        }

                        //Applying a transformation on the dataset
                        try
                        {
                            Transform3DGroup transformations = new Transform3DGroup();

                            ls3D.Model.Transform = transformations;


                            //Scaling
                            ScaleTransform3D scaling = new ScaleTransform3D(new Vector3D(1, 1, ls3D.Scale));

                            //Adding transformations to the transformation group
                            transformations.Children.Add(scaling);


                            //Translation
                            double trueTranslateX = 0;
                            double trueTranslateY = 0;
                            double trueTranslateZ = 0;

                            if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Model)
                            {
                                trueTranslateX = ls3D.Model.Bounds.X;
                                trueTranslateY = ls3D.Model.Bounds.Y;
                                trueTranslateZ = -ls3D.Model.Bounds.Z;
                            }

                            TranslateTransform3D translation =
                            new TranslateTransform3D(new Vector3D(ls3D.TranslateX + trueTranslateX - MinEast, ls3D.TranslateY + trueTranslateY - MinNorth, ls3D.TranslateZ + trueTranslateZ - MinAltitude));


                            transformations.Children.Add(translation);

                            //Rotation
                            Vector3D axisX = new Vector3D(1, 0, 0);
                            Vector3D axisY = new Vector3D(0, 1, 0);
                            Vector3D axisZ = new Vector3D(0, 0, 1);

                            Point3D center = new Point3D(ls3D.Model.Bounds.X + 0.5 * ls3D.Model.Bounds.SizeX, ls3D.Model.Bounds.Y + 0.5 * ls3D.Model.Bounds.SizeY, ls3D.Model.Bounds.Z + 0.5 * ls3D.Model.Bounds.SizeZ);

                            RotateTransform3D xRotation = new RotateTransform3D();
                            xRotation.Rotation = new AxisAngleRotation3D(axisX, ls3D.RotateX);
                            xRotation.CenterX = center.X;
                            xRotation.CenterY = center.Y;
                            xRotation.CenterZ = center.Z;
                            RotateTransform3D yRotation = new RotateTransform3D();
                            yRotation.Rotation = new AxisAngleRotation3D(axisY, ls3D.RotateY);
                            yRotation.CenterX = center.X;
                            yRotation.CenterY = center.Y;
                            yRotation.CenterZ = center.Z;
                            RotateTransform3D zRotation = new RotateTransform3D();
                            zRotation.Rotation = new AxisAngleRotation3D(axisZ, ls3D.RotateZ);
                            zRotation.CenterX = center.X;
                            zRotation.CenterY = center.Y;
                            zRotation.CenterZ = center.Z;

                            transformations.Children.Add(xRotation);
                            transformations.Children.Add(yRotation);
                            transformations.Children.Add(zRotation);
                        }
                        catch
                        {

                        }
                        if (ls3D.Chart3DDisplayType != Chart3DDisplayType.Model)
                        {
                            ls3D.Model.Freeze();
                        }

                        modelGroup.Children.Add(ls3D.Model);
                    }
                    catch
                    {
                        continue;
                    }

                }

                AddSlice(ref modelGroup);
                AddLabels(ref modelGroup, MinEast, MinNorth, MinAltitude);

                ///Applying a scale to the model
                ScaleTransform3D scaleTransform = new ScaleTransform3D(1, 1, ExaggerationFactor, 0, 0, 0);
                modelGroup.Transform = scaleTransform;
            }));

            AddGrid(MinEast, MinNorth, MinAltitude);
            Model = modelGroup;
        }

        //Adding the slicing polygon
        private void AddSlice(ref Model3DGroup modelGroup)
        {
            //Return if not active or visible
            if (Plane3D.IsActive == false || Plane3D.IsVisible == false)
                return;

            // Create a mesh builder and add a box to it
            var meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

            List<Point3D> points = Plane3D.GetBoundingPoints(new Point3D(Xmax, Ymax, Zmax), new Point3D(Xmin, Ymin, Zmin)).ToList();

            for (int i = 0; i < points.Count() - 1; i++)
                meshBuilder.AddCylinder(points[i], points[i + 1], 0.05, 3, false, false);

            var mesh = meshBuilder.ToMesh(true);

            var material = MaterialHelper.CreateMaterial(Colors.Black, 1);

            modelGroup.Children.Add(new System.Windows.Media.Media3D.GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material });
        }

        /// <summary>
        /// Adds the grid to the model group
        /// </summary>
        private void AddGrid(double minEast = 0, double minNorth = 0, double minAltitude = 0)
        {
            ///Applying a scale to the model
            ScaleTransform3D scaleTransform = new ScaleTransform3D(new Vector3D(1, 1, ExaggerationFactor));

            for (int i = 0; i < 3; i++)
            {
                GridLinesVisual3D gridlines = new GridLinesVisual3D();
                if (i == 0)
                {
                    //Check if the grid should be visible
                    if (!IsXGrid)
                    {
                        XZGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        XZGridlines.Visible = true;
                    }

                    XZGridlines.Center = new Point3D((Xmax + Xmin) / 2 - MinEast, Ymax - MinNorth, ExaggerationFactor * ((Zmax + Zmin)) / 2 - MinAltitude);
                    XZGridlines.Normal = new Vector3D(0, 1, 0);
                    XZGridlines.Thickness = GridlineThickness;
                    XZGridlines.MinorDistance = YTick;
                    XZGridlines.Length = (Xmax - Xmin);
                    XZGridlines.Width = ExaggerationFactor * (Zmax - Zmin);
                    XZGridlines.Material = MaterialHelper.CreateMaterial(GridlineColor);
                    XZGridlines.Transform = scaleTransform;
                }
                else if (i == 1)
                {
                    if (!IsYGrid)
                    {
                        XYGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        XYGridlines.Visible = true;
                    }

                    XYGridlines.Center = new Point3D((Xmax + Xmin) / 2 - minEast, ((Ymax + Ymin)) / 2 - minNorth, Zmin - minAltitude);
                    XYGridlines.Normal = new Vector3D(0, 0, 1);
                    XYGridlines.MinorDistance = XTick;
                    XYGridlines.Thickness = GridlineThickness;
                    XYGridlines.Length = (Xmax - Xmin);
                    XYGridlines.Width = (Ymax - Ymin);
                    XYGridlines.Material = MaterialHelper.CreateMaterial(GridlineColor);
                    XYGridlines.Transform = scaleTransform;
                }
                else if (i == 2)
                {
                    if (!IsZGrid)
                    {
                        YZGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        YZGridlines.Visible = true;
                    }

                    YZGridlines.Center = new Point3D(Xmin - minEast, ((Ymax + Ymin)) / 2 - minNorth, ExaggerationFactor * (Zmax + Zmin) / 2 - minAltitude);
                    YZGridlines.Normal = new Vector3D(1, 0, 0);
                    YZGridlines.MinorDistance = double.IsInfinity(ZTick) ? Zmax : ZTick;
                    YZGridlines.Thickness = GridlineThickness;
                    YZGridlines.Length = ExaggerationFactor * ((Zmax - Zmin));
                    YZGridlines.Width = ((Ymax - Ymin));
                    YZGridlines.Material = MaterialHelper.CreateMaterial(GridlineColor);
                    YZGridlines.Transform = scaleTransform;
                }

            }
        }

        /// <summary>
        /// Adds the labels to the model group
        /// </summary>
        public void AddLabels(ref Model3DGroup model, double minEast = 0, double minNorth = 0, double minAltitude = 0)
        {
            double dx = 0;
            for (dx = 0; dx <= 2; dx += 1)
            {
                TextVisual3D txt = new TextVisual3D();
                txt.Background = new SolidColorBrush(Colors.Transparent);
                txt.Padding = new Thickness(2);
                txt.FontWeight = FontWeights.Bold;
                txt.Height = LabelFontSize;

                if (dx == 0)
                {
                    txt.Foreground = XLabel.LabelColor;
                    txt.Position = new Point3D(((Xmax + Xmin)) / 2 - minEast - txt.Height, (Ymin - minNorth) - (2 * txt.Height), Zmin - minAltitude);
                    txt.Text = XLabel.Text;
                    txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                }
                if (dx == 1)
                {
                    txt.Foreground = YLabel.LabelColor;
                    txt.Position = new Point3D((Xmin - minEast) - 2 * txt.Height, ((Ymax + Ymin)) / 2 - minNorth - txt.Height, Zmin - minAltitude);
                    txt.Text = YLabel.Text;
                    txt.TextDirection = new Vector3D(0, 1, 0);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(1, 0, 0);                             // Set text to Point Up on Y axis
                }
                if (dx == 2)
                {
                    txt.Foreground = ZLabel.LabelColor;
                    txt.Position = new Point3D((Xmin - minEast) - 2 * txt.Height, Ymax - minNorth, ((Zmax + Zmin)) / 2 - minAltitude);
                    txt.Text = IsZGrid ? ZLabel.Text : " ";
                    txt.TextDirection = new Vector3D(0, 0, 1);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(1, 0, 0);
                }

                model.Children.Add(txt.Content);
            }

            for (dx = Xmin; dx <= Xmax; dx += XTick)
            {
                if (double.IsInfinity(Xmax))
                    Xmax = Xmin + 1;

                if ((Xmax - Xmin) / XTick > 10000)
                    XTick = (Xmax - Xmin) / 100;

                TextVisual3D txt = new TextVisual3D();
                txt.Background = new SolidColorBrush(Colors.Transparent);
                txt.Height = LabelFontSize * 0.8;
                txt.Position = new Point3D(dx - minEast, (Ymin - minNorth) - txt.Height, Zmin - minAltitude);
                if (txt.Text == "0" && dx != Xmin)
                    txt.Text = (dx).ToString("E0");
                txt.Text = Math.Round(dx, 2).ToString();
                txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                txt.Foreground = XLabel.LabelColor;
                txt.Padding = new Thickness(2);

                if (XTick == 0)
                    XTick += 1;

                model.Children.Add(txt.Content);
            }
            for (dx = Ymin; dx <= Ymax; dx += YTick)
            {
                if (double.IsInfinity(Ymax))
                    Ymax = Ymin + 1;

                if ((Ymax - Ymin) / YTick > 10000)
                    YTick = (Ymax - Ymin) / 100;

                TextVisual3D txt = new TextVisual3D();
                txt.Background = new SolidColorBrush(Colors.Transparent);
                txt.Height = LabelFontSize * 0.8;
                txt.Position = new Point3D((Xmin - minEast) - txt.Height, dx - minNorth, Zmin - minAltitude);
                if (txt.Text == "0" && dx != Ymin)
                    txt.Text = dx.ToString("E0");
                txt.Text = Math.Round(dx, 2).ToString();
                txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                txt.Foreground = YLabel.LabelColor;
                txt.Padding = new Thickness(2);

                if (YTick == 0)
                    YTick += 1;

                model.Children.Add(txt.Content);
            }

            if (IsZGrid)
                for (dx = Zmin; dx <= Zmax; dx += ZTick)
                {
                    if (double.IsInfinity(Zmax) || Zmax == Zmin)
                        Zmax = Zmin + 1;

                    if ((Zmax - Zmin) / ZTick > 10000 || Double.IsInfinity(ZTick))
                        ZTick = (Zmax - Zmin) / 100;

                    TextVisual3D txt = new TextVisual3D();
                    txt.Background = new SolidColorBrush(Colors.Transparent);
                    txt.Height = LabelFontSize * 0.8;
                    txt.Position = new Point3D((Xmin - minEast) - txt.Height, Ymax - minNorth, dx - minAltitude);
                    if (txt.Text == "0" && dx != Zmin)
                        txt.Text = dx.ToString("E0");
                    txt.Text = Math.Round(dx, 2).ToString();
                    txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(0, 0, 1);                             // Set text to Point Up on Y axis
                    txt.Foreground = ZLabel.LabelColor;
                    txt.Padding = new Thickness(2);

                    if (ZTick == 0)
                        ZTick += 1;

                    model.Children.Add(txt.Content);
                }

        }

        /// <summary>
        /// Loading a 3D model
        /// </summary>
        public void Load3dModel()
        {
            HelixToolkit.Wpf.ObjReader CurrentHelixObjReader = new HelixToolkit.Wpf.ObjReader();

            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"OBJ (*.obj)|*.obj;";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName == "")
            {
                return;
            }

            //Getting file information
            FileInfo fI = new FileInfo(openFileDlg.FileName);

            if (fI.Extension == ".obj")
            {
                Series3D model = new Series3D();
                model.Model = CurrentHelixObjReader.Read(fI.FullName);
                model.Chart3DDisplayType = Chart3DDisplayType.Model;
                DataCollection.Add(model);
            }
        }

        /// <summary>
        /// Transforming a mesh to a wireframe
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        public static System.Windows.Media.Media3D.GeometryModel3D CreateWireframe(System.Windows.Media.Media3D.MeshGeometry3D mesh, double gridlineThickness = 0.02)
        {
            // Create a mesh builder and add a box to it
            var meshBuilder = new HelixToolkit.Wpf.MeshBuilder(false, false);

            Model3DGroup group = new Model3DGroup();
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                int a = mesh.TriangleIndices[i];
                int b = mesh.TriangleIndices[i + 1];
                int c = mesh.TriangleIndices[i + 2];

                Point3D pa = mesh.Positions[a];
                Point3D pb = mesh.Positions[b];
                Point3D pc = mesh.Positions[c];

                meshBuilder.AddCylinder(pa, pb, gridlineThickness, 4, false, false);
                meshBuilder.AddCylinder(pa, pc, gridlineThickness, 4, false, false);
                meshBuilder.AddCylinder(pb, pc, gridlineThickness, 4, false, false);


            }

            var material = MaterialHelper.CreateMaterial(Colors.Black);
            var mesh1 = meshBuilder.ToMesh(true);

            return new System.Windows.Media.Media3D.GeometryModel3D { Geometry = mesh1, Material = material };

        }

        /// <summary>
        /// Updates the chart
        /// </summary>
        public override async Task UpdateChart()
        {
            // Do any rectangle-specific processing here.
            await AddDataCollection().ConfigureAwait(false);

            // Call the base class event invocation method.
            await base.UpdateChart();
        }

        #endregion

        #region Helper methods

        public void SubdivideAxes()
        {
            try
            {
                for (int i = 0; i < Ds.Count; i++)
                {
                    Maxx = Ds[i].Mesh.Vertices.Max(x => x.X);
                    Minx = Ds[i].Mesh.Vertices.Min(x => x.X);
                    Maxy = Ds[i].Mesh.Vertices.Max(x => x.Y);
                    Miny = Ds[i].Mesh.Vertices.Min(x => x.Y);
                    Maxz = Ds[i].Mesh.Vertices.Max(x => x.Z);
                    Minz = Ds[i].Mesh.Vertices.Min(x => x.Z);
                }

                double local;

                local = Math.Round(Minx, 1) == 0 ?
                    (Math.Round(Minx, 2) == 0 ?
                    (Math.Round(Minx, 3) == 0 ?
                    (Math.Round(Minx, 4) == 0 ?
                      Math.Round(Minx, 4)
                    : Math.Round(Minx, 4))
                    : Math.Round(Minx, 3))
                    : Math.Round(Minx, 2))
                    : Math.Round(Minx, 1);

                //Math.Round(minx, 1);

                if (local > 0 && (local < Xmin || local > Xmin)) { Xmin = local; }
                else if (local < Xmin) { Xmin = local; }

                local = Math.Round(Maxx, 1) == 0 ?
                    (Math.Round(Maxx, 2) == 0 ?
                    (Math.Round(Maxx, 3) == 0 ?
                    (Math.Round(Maxx, 4) == 0 ?
                      Math.Round(Maxx, 4)
                    : Math.Round(Maxx, 4))
                    : Math.Round(Maxx, 3))
                    : Math.Round(Maxx, 2))
                    : Math.Round(Maxx, 1);

                //Math.Round(Ds.LinePoints.Max(x => x.X), 1);

                if (local > 0 && local > Xmax) { Xmax = local; }
                else if (local > Xmax) { Xmax = local - local; }

                local = Math.Round(Miny, 1) == 0 ?
                    (Math.Round(Miny, 2) == 0 ?
                    (Math.Round(Miny, 3) == 0 ?
                    (Math.Round(Miny, 4) == 0 ?
                      Math.Round(Miny, 4)
                    : Math.Round(Miny, 4))
                    : Math.Round(Miny, 3))
                    : Math.Round(Miny, 2))
                    : Math.Round(Miny, 1);

                //Math.Round(Ds.LinePoints.Min(y => y.Y), 1);

                if (local > 0 && (local < Ymin || local > Ymin)) { Ymin = local; }
                else if (local < Ymin) { Ymin = local; }

                local = Math.Round(Maxy, 1) == 0 ?
                    (Math.Round(Maxy, 2) == 0 ?
                    (Math.Round(Maxy, 3) == 0 ?
                    (Math.Round(Maxy, 4) == 0 ?
                      Math.Round(Maxy, 4)
                    : Math.Round(Maxy, 4))
                    : Math.Round(Maxy, 3))
                    : Math.Round(Maxy, 2))
                    : Math.Round(Maxy, 1);

                if (local > 0 && local > Ymax) { Ymax = local; }
                else if (local == 0 && local > Ymax)
                    Ymax = 1;
                else if (local > Ymax) { Ymax = local; }

                local = Math.Round(Minz, 1) == 0 ?
                        (Math.Round(Minz, 2) == 0 ?
                        (Math.Round(Minz, 3) == 0 ?
                        (Math.Round(Minz, 4) == 0 ?
                          Math.Round(Minz, 4)
                        : Math.Round(Minz, 4))
                        : Math.Round(Minz, 3))
                        : Math.Round(Minz, 2))
                        : Math.Round(Minz, 1);

                //Math.Round(Ds.LinePoints.Min(y => y.Y), 1);

                if (local > 0 && (local < Zmin || local > Zmin)) { Zmin = local; }
                else if (local < Zmin) { Zmin = local; }

                local = Math.Round(Maxz, 1) == 0 ?
                    (Math.Round(Maxz, 2) == 0 ?
                    (Math.Round(Maxz, 3) == 0 ?
                    (Math.Round(Maxz, 4) == 0 ?
                      Math.Round(Maxz, 4)
                    : Math.Round(Maxz, 4))
                    : Math.Round(Maxz, 3))
                    : Math.Round(Maxz, 2))
                    : Math.Round(Maxz, 1);

                if (local > 0 && local > Zmax) { Zmax = local; }
                else if (local == 0 && local > Zmax)
                    Ymax = 1;

                else if (local > Ymax) { Ymax = local; }
                if (XTick == 0)
                    XTick = Math.Round((Xmax - Xmin) / 10, 1) == 0 ?
                        (Math.Round((Xmax - Xmin) / 10, 2) == 0 ?
                        (Math.Round((Xmax - Xmin) / 10, 3) == 0 ?
                        (Math.Round((Xmax - Xmin) / 10, 4) == 0 ?
                          Math.Round((Xmax - Xmin) / 10, 4)
                        : Math.Round((Xmax - Xmin) / 10, 4))
                        : Math.Round((Xmax - Xmin) / 10, 3))
                        : Math.Round((Xmax - Xmin) / 10, 2))
                        : Math.Round((Xmax - Xmin) / 10, 1);

                if (YTick == 0)
                    YTick = Math.Round((Ymax - Ymin) / 10, 1) == 0 ?
                        (Math.Round((Ymax - Ymin) / 10, 2) == 0 ?
                        (Math.Round((Ymax - Ymin) / 10, 3) == 0 ?
                        (Math.Round((Ymax - Ymin) / 10, 4) == 0 ?
                          Math.Round((Ymax - Ymin) / 10, 4)
                        : Math.Round((Ymax - Ymin) / 10, 4))
                        : Math.Round((Ymax - Ymin) / 10, 3))
                        : Math.Round((Ymax - Ymin) / 10, 2))
                        : Math.Round((Ymax - Ymin) / 10, 1);

                if (ZTick == 0)
                    ZTick = Math.Round((Zmax - Zmin) / 10, 1) == 0 ?
                        (Math.Round((Zmax - Zmin) / 10, 2) == 0 ?
                        (Math.Round((Zmax - Zmin) / 10, 3) == 0 ?
                        (Math.Round((Zmax - Zmin) / 10, 4) == 0 ?
                          Math.Round((Zmax - Zmin) / 10, 4)
                        : Math.Round((Zmax - Zmin) / 10, 4))
                        : Math.Round((Zmax - Zmin) / 10, 3))
                        : Math.Round((Zmax - Zmin) / 10, 2))
                        : Math.Round((Zmax - Zmin) / 10, 1);

            }
            catch
            {

            }
        }

        /// <summary>
        /// Sets the solidcolorbrush based on the calculated cmap values and an opacity value
        /// </summary>
        /// <param name="cmap"></param>
        /// <param name="opacity"></param>
        public void SetBrush(byte[,] cmap, double opacity = 1)
        {
            List<SolidColorBrush> brushes = new List<SolidColorBrush>();

            double dy = (ColorMap.Ymax - ColorMap.Ymin) / (ColorMap.Ydivisions - 1);

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

            ColorMap.ColormapBrushes = new BindableCollection<Rectangle2D>(rects);

        }

        public void SetColorMapLabels()
        {
            try
            {
                ColorMap.Labels = new BindableCollection<Label>();

                double step = (ColorMap.Ymax - ColorMap.Ymin) / ColorMap.LabelSubdivisions;
                double stepHeight = (Ymax - Ymin) / ColorMap.LabelSubdivisions;

                for (int i = 0; i < ColorMap.LabelSubdivisions + 1; i++)
                {
                    var a = new LocationTimeValue(Xmax, ColorMap.Ymax - stepHeight * i);

                    Label tb = new Label()
                    {
                        Text = (ColorMap.Ymax - step * i).ToString(),
                        X = NormalizePoint(a).X + 24,
                        Y = Math.Abs(NormalizePoint(a).Y) - MeasureString(Math.Round(ColorMap.Ymax - step * i, 2).ToString()).Height
                    };

                    ColorMap.Labels.Add(tb);
                }
            }
            catch
            {
                return;
            }
        }

        #endregion
    }
}