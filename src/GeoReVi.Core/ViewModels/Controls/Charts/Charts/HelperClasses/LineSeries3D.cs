using Caliburn.Micro;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace GeoReVi
{
    public class Series3D : LineSeries
    {

        private BindableCollection<LineSeries3D> _a;

        private List<Tuple<Point3D, double, string>> linePoints = new List<Tuple<Point3D, double, string>>();

        private Model3DGroup model = new Model3DGroup();

        /// <summary>
        /// The line points
        /// </summary>
        public List<Tuple<Point3D,double,string>> LinePoints
        {
            get => this.linePoints;
            set
            {
                linePoints = value;
            }
        }

        /// <summary>
        /// The model
        /// </summary>
        public Model3DGroup Model
        {
            get => this.model;
            set
            {
                model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

        /// <summary>
        /// Display type of the line 3d series
        /// </summary>
        private Chart3DDisplayType chart3DDisplayType = Chart3DDisplayType.Scatter;
        public Chart3DDisplayType Chart3DDisplayType
        {
            get => this.chart3DDisplayType;
            set
            {
                this.chart3DDisplayType = value;

                if (_a != null)
                    if (_a.Count != 0)
                        _a.UpdateChart();

                NotifyOfPropertyChange(() => Chart3DDisplayType);
            }
        }

        private Symbols<LineSeries3D> symbols;
        //Symbols used for the chart control
        public Symbols<LineSeries3D> Symbols
        {
            get => this.symbols;
            set
            {
                this.symbols = value;
            }
        }

        /// <summary>
        /// The type of mesh visualization
        /// </summary>
        private MeshDisplayType meshDisplayType = MeshDisplayType.Faces;
        public MeshDisplayType MeshDisplayType
        {
            get => this.meshDisplayType;
            set
            {
                this.meshDisplayType = value;
                NotifyOfPropertyChange(() => MeshDisplayType);
            }
        }

        /// <summary>
        /// The maximum visible value
        /// </summary>
        private double wireframeThickness = 0.02;
        public double WireframeThickness
        {
            get => this.wireframeThickness;
            set
            {
                wireframeThickness = value;


                if (_a != null)
                    if (_a.Count != 0)
                        _a.UpdateChart();

                NotifyOfPropertyChange(() => WireframeThickness);
            }
        }

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        ///         /// <summary>
        /// Default constructor
        /// </summary>
        public LineSeries3D()
        {
            LinePoints = new List<Tuple<Point3D, double, string>>();
            LineThickness = 1;
            LineColor = Brushes.Black;
            LinePattern = LinePatternEnum.Solid;
            Symbols = new Symbols<LineSeries3D>();
            Symbols.FillColor = LineColor;
            Symbols.SymbolSize = 0.3;
        }

        public LineSeries3D(BindableCollection<LineSeries3D> _dataCollection)
        {
            LinePoints = new List<Tuple<Point3D, double, string>>();
            LineThickness = 1;
            LineColor = Brushes.Black;
            LinePattern = LinePatternEnum.Solid;
            Symbols = new Symbols<LineSeries3D>(_dataCollection);
            Symbols.FillColor = LineColor;
            Symbols.SymbolSize = 0.3;
            _a = _dataCollection;
        }

        #endregion

        #region Public methods


        /// <summary>
        /// Removing this line series from the chart
        /// </summary>
        public void Remove()
        {
            try
            {
                _a.Remove(this);
            }
            catch
            {

            }
        }


        /// <summary>
        /// Translating the model
        /// </summary>
        public void TransformModel()
        {
            try
            {
                Vector3D axis = new Vector3D();

                switch(RotationAxis)
                {
                    case DirectionEnum.X:
                        axis = new Vector3D(1, 0, 0);
                        break;
                    case DirectionEnum.Y:
                        axis = new Vector3D(0, 1, 0);
                        break;
                    case DirectionEnum.Z:
                        axis = new Vector3D(0, 0, 1);
                        break;
                    case DirectionEnum.XY:
                        axis = new Vector3D(1, 1, 0);
                        break;
                    case DirectionEnum.XZ:
                        axis = new Vector3D(1, 0, 1);
                        break;
                    case DirectionEnum.YZ:
                        axis = new Vector3D(0, 1, 1);
                        break;
                    case DirectionEnum.XYZ:
                        axis = new Vector3D(1, 1, 1);
                        break;
                }

                var matrix = new Matrix3D();

                matrix.Rotate(new Quaternion(axis, RotationAngle));
                matrix.Translate(new Vector3D(TranslateX, TranslateY, TranslateZ));

                Model.Transform = new MatrixTransform3D(matrix);
            }
            catch
            {

            }
        }

        /// <summary>
        /// Brings a model back to it's original position
        /// </summary>
        public void Origin()
        {
            TranslateX = 0;
            TranslateY = 0;
            TranslateZ = 0;
            RotationAngle = 0;
            TransformModel();
        }

        /// <summary>
        /// Exporting the current model
        /// </summary>
        public void ExportModel()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "OBJ (*.obj)|*.obj|X3D (*.x3d)|*.x3d|XAML (*.xaml)|*.xaml|XML (*.xml)|*.xml";
            saveFileDialog1.RestoreDirectory = true;

            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                //Getting the extension
                var ext = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                try
                {
                    //Getting the extension
                    var ext1 = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                    switch (ext1.ToString())
                    {
                        case ".obj":
                            using (FileStream stream = File.Create(saveFileDialog1.FileName))
                            {
                                var k = new ObjExporter
                                { TextureExtension = ".png",
                                   MaterialsFile = Path.ChangeExtension(saveFileDialog1.FileName, ".mtl"),
                                   TextureQualityLevel = 90,
                                   TextureSize = 90,
                                TextureFolder = Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName),
                                FileCreator = (f) => File.Create(System.IO.Path.Combine(Path.GetDirectoryName(saveFileDialog1.FileName) + "\\" + Path.GetFileNameWithoutExtension(saveFileDialog1.FileName), f)),
                                SwitchYZ = false,
                                ExportNormals = true,
                                };

                                if (!Directory.Exists(k.TextureFolder))
                                {
                                    Directory.CreateDirectory(k.TextureFolder);
                                }
                                k.Export((Model3D)this.Model, stream);
                            }
                            break;
                        case ".xml":
                            {
                                using (var stream = File.Create(saveFileDialog1.FileName))
                                {
                                    var k = new KerkytheaExporter();
                                    k.Export((Model3D)this.Model, stream);
                                }
                                break;
                            }
                        case ".xaml":
                            {
                                using (var stream = File.Create(saveFileDialog1.FileName))
                                {
                                    var k = new XamlExporter();
                                    k.Export((Model3D)this.Model, stream);
                                }
                                break;
                            }
                        case ".x3d":
                            {
                                using (var stream = File.Create(saveFileDialog1.FileName))
                                {
                                    var k = new X3DExporter();
                                    k.Export((Model3D)this.Model, stream);
                                }
                                break;
                            }
                    }
                }
                catch
                {

                }
            }
        }
        #endregion
    }
}