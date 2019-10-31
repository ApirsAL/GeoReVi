using Caliburn.Micro;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Point3D = System.Windows.Media.Media3D.Point3D;
using TranslateTransform3D = System.Windows.Media.Media3D.TranslateTransform3D;

namespace GeoReVi
{
    /// <summary>
    /// A viewmodel for the helix 3D chart
    /// </summary>
    public class HelixChart3DViewModel : Screen
    {
        //A chart 3D object
        private Chart3DObject ch3d;
        public Chart3DObject Ch3d
        {
            get => this.ch3d;
            set
            {
                this.ch3d = value;
                NotifyOfPropertyChange(() => Ch3d);
            }
        }

        /// <summary>
        /// Camera of the view
        /// </summary>
        private PerspectiveCamera camera = new PerspectiveCamera();
        public PerspectiveCamera Camera
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
        private Model3D model;
        public Model3D Model
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
        public BillboardTextGroupVisual3D Labels
        {
            get => this.labels;
            set
            {
                this.labels = value;
                NotifyOfPropertyChange(() => Labels);
            }
        }

        #region Constructor

        public HelixChart3DViewModel()
        {
            //Instantiating new chart 3D object
            Ch3d = new Chart3DObject();

            Ch3d.DataCollection.CollectionChanged += this.OnCollectionChanged;

        }

        public HelixChart3DViewModel(VariogramHelper _vh)
        {
            //Instantiating new chart 3D object
            Ch3d = new Chart3DObject(_vh);

            Ch3d.DataCollection.CollectionChanged += this.OnCollectionChanged;

        }

        #endregion

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddDataCollection();

            if (e.NewItems != null)
            {
                foreach (LineSeries3D newItem in e.NewItems)
                {
                    //Add listener for each item on PropertyChanged event
                    newItem.PropertyChanged += this.OnItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LineSeries3D oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= this.OnItemPropertyChanged;
                }
            }
        }

        void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            LineSeries3D item = sender as LineSeries3D;
            if (item != null)
                AddDataCollection();
        }

        /// <summary>
        /// A method to add some test objects to the 3D view port
        /// </summary>
        public void AddTestGeometries()
        {
            // Create a mesh builder and add a box to it
            var meshBuilder = new MeshBuilder(false, false);
            // Create a model group
            var modelGroup = new Model3DGroup();

            meshBuilder.AddBox(new Point3D(0, 0, 1), 1, 2, 0.5);
            meshBuilder.AddBox(new Rect3D(0, 0, 1.2, 0.5, 1, 0.4));
            meshBuilder.AddSphere(new Point3D(0, 0, 0));
            meshBuilder.AddNode(new Point3D(1, 1, 1), new Vector3D(1, 1, 1), new System.Windows.Point(1, 1));

            // Create a mesh from the builder (and freeze it)
            var mesh = meshBuilder.ToMesh(true);

            // Create some materials
            var greenMaterial = MaterialHelper.CreateMaterial(Colors.Green);
            var redMaterial = MaterialHelper.CreateMaterial(Colors.Red);
            var blueMaterial = MaterialHelper.CreateMaterial(Colors.Blue);
            var insideMaterial = MaterialHelper.CreateMaterial(Colors.Yellow);

            // Add 3 models to the group (using the same mesh, that's why we had to freeze it)
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = greenMaterial, BackMaterial = insideMaterial });
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(-2, 0, 0), Material = redMaterial, BackMaterial = insideMaterial });
            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Transform = new TranslateTransform3D(2, 0, 0), Material = blueMaterial, BackMaterial = insideMaterial });

            // Set the property, which will be bound to the Content property of the ModelVisual3D (see MainWindow.xaml)
            Model = modelGroup;
        }

        /// <summary>
        /// Adding a data collection
        /// </summary>
        public void AddDataCollection()
        {
            // Create a model group
            var modelGroup = new Model3DGroup();
            // Create a mesh builder and add a box to it
            var meshBuilder = new MeshBuilder(false, false);

            AddGrid();
            AddLabels(ref modelGroup);

            var mesh = meshBuilder.ToMesh(true);

            var material = MaterialHelper.CreateMaterial(Colors.Black);
            
            foreach (LineSeries3D ls3D in Ch3d.DataCollection)
            {
                try
                {
                    double width = 0;
                    double length = 0;
                    double height = 0;

                    if(ls3D.Chart3DDisplayType == Chart3DDisplayType.RegularGrid)
                    {
                        var linePoints = ls3D.LinePoints.FindAll(x => x.Item1.Y == ls3D.LinePoints.Min(y => y.Item1.Y) && x.Item1.Z == ls3D.LinePoints.Min(y => y.Item1.Z));

                        width = Convert.ToDouble((linePoints.Max(x => x.Item1.X) - linePoints.Min(x => x.Item1.X))/ Convert.ToDouble(linePoints.Count-1));

                        linePoints = ls3D.LinePoints.FindAll(x => x.Item1.Z == ls3D.LinePoints.Min(y => y.Item1.Z) && x.Item1.X == ls3D.LinePoints.Min(y => y.Item1.X));

                        length = Convert.ToDouble((linePoints.Max(x => x.Item1.Y) - linePoints.Min(x => x.Item1.Y))/ Convert.ToDouble(linePoints.Count-1));


                        linePoints = ls3D.LinePoints.FindAll(x => x.Item1.Y == ls3D.LinePoints.Min(y => y.Item1.Y) && x.Item1.X == ls3D.LinePoints.Min(y => y.Item1.X));

                        height = Convert.ToDouble((linePoints.Max(x => x.Item1.Z) - linePoints.Min(x => x.Item1.Z)) / Convert.ToDouble(linePoints.Count-1));

                    }
                    else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.Model)
                    {
                        modelGroup.Children.Add(ls3D.Model);
                    }

                    foreach (var point in ls3D.LinePoints)
                    {
                        try
                        {
                            if (Ch3d.IsColorMap)
                            {
                                if (point.Item2 < Ch3d.MinimumVisibility || point.Item2 > Ch3d.MaximumVisibility)
                                    continue;
                            }

                            // Create a mesh builder and add a box to it
                            meshBuilder = new MeshBuilder(false, false);
                            mesh = new MeshGeometry3D();

                            switch (ls3D.Symbols.SymbolType)
                            {
                                case SymbolTypeEnum.Box:
                                    if(ls3D.Chart3DDisplayType == Chart3DDisplayType.Scatter)
                                        meshBuilder.AddBox(point.Item1, ls3D.Symbols.SymbolSize, ls3D.Symbols.SymbolSize, ls3D.Symbols.SymbolSize, BoxFaces.All);
                                    else if (ls3D.Chart3DDisplayType == Chart3DDisplayType.RegularGrid)
                                    {
                                        meshBuilder.AddBox(point.Item1, width, length, height, BoxFaces.All);
                                    }

                                    break;
                                case SymbolTypeEnum.Circle:
                                    meshBuilder.AddSphere(point.Item1, ls3D.Symbols.SymbolSize, 10, 10);
                                    break;
                                case SymbolTypeEnum.Dot:
                                    meshBuilder.AddSphere(point.Item1, ls3D.Symbols.SymbolSize, 10, 10);
                                    break;
                            }

                            mesh = meshBuilder.ToMesh(true);

                            material = MaterialHelper.CreateMaterial(ColorMapHelper.GetBrush(point.Item2, Ch3d.ColorMap.Ymin, Ch3d.ColorMap.Ymax, Ch3d.ColorMap, ls3D.Symbols.Opacity));

                            modelGroup.Children.Add(new GeometryModel3D { Geometry = mesh, Material = material, BackMaterial = material });
                        }
                        catch
                        {

                        }

                    }
                }
                catch
                {
                    continue;
                }

            }

            Model = modelGroup;
        }

        /// <summary>
        /// Adds the grid to the model group
        /// </summary>
        private void AddGrid()
        {
            for (int i = 0; i < 3; i++)
            {
                GridLinesVisual3D gridlines = new GridLinesVisual3D();
                if (i == 0)
                {
                    //Check if the grid should be visible
                    if (!Ch3d.IsXGrid)
                    {
                        XZGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        XZGridlines.Visible = true;
                    }

                    XZGridlines.Center = new Point3D((Ch3d.Xmax + Ch3d.Xmin) / 2, Ch3d.Ymax, (Ch3d.Zmax + Ch3d.Zmin) / 2);
                    XZGridlines.Normal = new Vector3D(0, 1, 0);
                    XZGridlines.MinorDistance = Ch3d.YTick;
                    XZGridlines.Length = (Ch3d.Xmax - Ch3d.Xmin);
                    XZGridlines.Width = (Ch3d.Zmax - Ch3d.Zmin);
                    XZGridlines.Material = MaterialHelper.CreateMaterial(Ch3d.GridlineColor);
                }
                else if (i == 1)
                {
                    if (!Ch3d.IsYGrid)
                    {
                        XYGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        XYGridlines.Visible = true;
                    }

                    XYGridlines.Center = new Point3D((Ch3d.Xmax + Ch3d.Xmin) / 2, (Ch3d.Ymax + Ch3d.Ymin) / 2, Ch3d.Zmin);
                    XYGridlines.Normal = new Vector3D(0, 0, 1);
                    XYGridlines.MinorDistance = Ch3d.XTick;
                    XYGridlines.Length = (Ch3d.Xmax - Ch3d.Xmin);
                    XYGridlines.Width = (Ch3d.Ymax - Ch3d.Ymin);
                    XZGridlines.Material = MaterialHelper.CreateMaterial(Ch3d.GridlineColor);
                }
                else if (i == 2)
                {
                    if (!Ch3d.IsZGrid)
                    {
                        YZGridlines.Visible = false;
                        continue;
                    }
                    else
                    {
                        YZGridlines.Visible = true;
                    }

                    YZGridlines.Center = new Point3D(Ch3d.Xmin, (Ch3d.Ymax + Ch3d.Ymin) / 2, (Ch3d.Zmax + Ch3d.Zmin) / 2);
                    YZGridlines.Normal = new Vector3D(1, 0, 0);
                    YZGridlines.MinorDistance = Ch3d.ZTick;
                    YZGridlines.Length = (Ch3d.Zmax - Ch3d.Zmin);
                    YZGridlines.Width = (Ch3d.Ymax - Ch3d.Ymin);
                    XZGridlines.Material = MaterialHelper.CreateMaterial(Ch3d.GridlineColor);
                }

            }
        }

        /// <summary>
        /// Adds the labels to the model group
        /// </summary>
        public void AddLabels(ref Model3DGroup model)
        {
            double dx = 0;
            for (dx = 0; dx <= 2; dx += 1)
            {
                TextVisual3D txt = new TextVisual3D();
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Padding = new Thickness(2);
                txt.FontWeight = FontWeights.Bold;
                txt.Height = 1.5;

                if (dx ==0)
                {
                    txt.Position = new Point3D((Ch3d.Xmax - Ch3d.Xmin)/2-txt.Height, Ch3d.Ymin-(2*txt.Height), 0);
                    txt.Text = Ch3d.XLabel;
                    txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                }
                if(dx==1)
                {
                    txt.Position = new Point3D(Ch3d.Xmin - 2 * txt.Height, (Ch3d.Ymax + Ch3d.Ymin) / 2 - txt.Height, 0);
                    txt.Text = Ch3d.YLabel;
                    txt.TextDirection = new Vector3D(0, 1, 0);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(1, 0, 0);                             // Set text to Point Up on Y axis
                }
                if(dx == 2)
                {
                    txt.Position = new Point3D(Ch3d.Xmin - 2 * txt.Height, Ch3d.Ymax, (Ch3d.Zmax + Ch3d.Zmin) / 2);
                    txt.Text = Ch3d.ZLabel;
                    txt.TextDirection = new Vector3D(0, 0, 1);                      // Set text to run in line with X axis
                    txt.UpDirection = new Vector3D(1, 0, 0);
                }

                model.Children.Add(txt.Content);
            }

            for (dx = Ch3d.Xmin; dx <= Ch3d.Xmax; dx += Ch3d.XTick)
            {
                TextVisual3D txt = new TextVisual3D();
                txt.Height = 1;
                txt.Position = new Point3D(dx, Ch3d.Ymin - txt.Height, 0);
                if (txt.Text == "0" && dx != Ch3d.Xmin)
                    txt.Text = dx.ToString("E0");
                txt.Text = Math.Round(dx, 2).ToString();
                txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Padding = new Thickness(2);

                if (Ch3d.XTick == 0)
                    Ch3d.XTick += 1;

                model.Children.Add(txt.Content);
            }
            for (dx = Ch3d.Ymin; dx <= Ch3d.Ymax; dx += Ch3d.YTick)
            {
                TextVisual3D txt = new TextVisual3D();
                txt.Height = 1;
                txt.Position = new Point3D(Ch3d.Xmin - txt.Height, dx, 0);
                if (txt.Text == "0" && dx != Ch3d.Ymin)
                    txt.Text = dx.ToString("E0");
                txt.Text = Math.Round(dx, 2).ToString();
                txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                txt.UpDirection = new Vector3D(0, 1, 0);                             // Set text to Point Up on Y axis
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Padding = new Thickness(2);

                if (Ch3d.YTick == 0)
                    Ch3d.YTick += 1;

                model.Children.Add(txt.Content);
            }
            for (dx = Ch3d.Zmin; dx <= Ch3d.Zmax; dx += Ch3d.ZTick)
            {
                TextVisual3D txt = new TextVisual3D();
                txt.Height = 1;
                txt.Position = new Point3D(Ch3d.Xmin - txt.Height, Ch3d.Ymax, dx);
                if (txt.Text == "0" && dx != Ch3d.Zmin)
                    txt.Text = dx.ToString("E0");
                txt.Text = Math.Round(dx, 2).ToString();
                txt.TextDirection = new Vector3D(1, 0, 0);                      // Set text to run in line with X axis
                txt.UpDirection = new Vector3D(0, 0, 1);                             // Set text to Point Up on Y axis
                txt.Foreground = new SolidColorBrush(Colors.Black);
                txt.Padding = new Thickness(2);

                if (Ch3d.ZTick == 0)
                    Ch3d.ZTick += 1;

                model.Children.Add(txt.Content);
            }

        }

        public void Load3dModel()
        {
            ObjReader CurrentHelixObjReader = new ObjReader();

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

            if(fI.Extension == ".obj")
            {
                LineSeries3D model = new LineSeries3D();
                model.Model = CurrentHelixObjReader.Read(fI.FullName);
                model.Chart3DDisplayType = Chart3DDisplayType.Model;
                Ch3d.DataCollection.Add(model);
            }

            AddDataCollection();
        }
    }
}
