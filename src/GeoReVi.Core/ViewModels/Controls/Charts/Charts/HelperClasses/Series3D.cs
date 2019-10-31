using Caliburn.Micro;
using HelixToolkit.Wpf;
using Microsoft.Win32;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class Series3D : PropertyChangedBase
    {
        #region Private members

        //Name of the line series
        private string seriesName = "Default";

        //Double collection of line patterns
        private DoubleCollection lineDashPattern;

        private double lineThickness;

        #endregion

        #region Public properties

        /// <summary>
        /// Shows or hides the labels of the line points
        /// </summary>
        private bool showPointLabels = false;
        public bool ShowPointLabels
        {
            get => this.showPointLabels;
            set
            {
                this.showPointLabels = value;
                NotifyOfPropertyChange(() => ShowPointLabels);
            }
        }

        #region Transformation properties

        //Magnitude of translation in X direction
        private double translateX = 0;
        public double TranslateX
        {
            get => this.translateX;
            set
            {
                this.translateX = value;
                NotifyOfPropertyChange(() => TranslateX);
            }
        }

        //Magnitude of translation in X direction
        private double translateY = 0;
        public double TranslateY
        {
            get => this.translateY;
            set
            {
                this.translateY = value;
                NotifyOfPropertyChange(() => TranslateY);
            }
        }

        //Magnitude of translation in Z direction
        private double translateZ = 0;
        public double TranslateZ
        {
            get => this.translateZ;
            set
            {
                this.translateZ = value;
                NotifyOfPropertyChange(() => TranslateZ);
            }
        }

        //Magnitude of rotation in X direction
        private double rotateX = 0;
        public double RotateX
        {
            get => this.rotateX;
            set
            {
                this.rotateX = value;
                NotifyOfPropertyChange(() => RotateX);
            }
        }

        //Magnitude of rotation in X direction
        private double rotateY = 0;
        public double RotateY
        {
            get => this.rotateY;
            set
            {
                this.rotateY = value;
                NotifyOfPropertyChange(() => RotateY);
            }
        }

        //Magnitude of rotation in Z direction
        private double rotateZ = 0;
        public double RotateZ
        {
            get => this.rotateZ;
            set
            {
                this.rotateZ = value;
                NotifyOfPropertyChange(() => RotateZ);
            }
        }

        //Scale of the object
        private double scale = 1;
        public double Scale
        {
            get => this.scale;
            set
            {
                this.scale = value;
                NotifyOfPropertyChange(() => Scale);
            }
        }

        #endregion

        /// <summary>
        /// Line Color
        /// </summary>
        private Brush lineColor;
        [XmlIgnore]
        public Brush LineColor
        {
            get => this.lineColor;
            set
            {
                this.lineColor = value;

                NotifyOfPropertyChange(() => LineColor);
            }
        }

        //Check if series should be displayed
        private bool display = true;
        public bool Display
        {
            get => this.display;
            set
            {
                this.display = value;

                NotifyOfPropertyChange(() => Display);
            }
        }

        //Thickness of the line
        public double LineThickness
        {
            get => this.lineThickness;
            set
            {
                this.lineThickness = value;

                NotifyOfPropertyChange(() => LineThickness);
            }
        }

        //Line pattern
        private LinePatternEnum linePattern = LinePatternEnum.Dash;
        public LinePatternEnum LinePattern
        {
            get => this.linePattern;
            set
            {
                this.linePattern = value;
                NotifyOfPropertyChange(() => LinePattern);
            }
        }

        //name of the line series
        public string SeriesName
        {
            get
            {
                return seriesName;
            }
            set
            {
                seriesName = value;
                NotifyOfPropertyChange(() => SeriesName);
            }
        }

        //Double collection of line patterns
        public DoubleCollection LineDashPattern
        {
            get => lineDashPattern;
            set
            {
                lineDashPattern = value;

                NotifyOfPropertyChange(() => LineDashPattern);
            }
        }

        /// <summary>
        /// The model
        /// </summary>
        private Model3DGroup model = new Model3DGroup();
        [XmlIgnore]
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
        /// Checks if the series is a colormap or not
        /// </summary>
        private bool isColorMap = false;
        public bool IsColorMap
        {
            get => this.isColorMap;
            set
            {
                this.isColorMap = value;
                NotifyOfPropertyChange(() => IsColorMap);
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

                NotifyOfPropertyChange(() => Chart3DDisplayType);
            }
        }

        private Symbols<Series3D> symbols = new Symbols<Series3D>();
        //Symbols used for the chart control
        public Symbols<Series3D> Symbols
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

                NotifyOfPropertyChange(() => WireframeThickness);
            }
        }

        /// <summary>
        /// Path to the image
        /// </summary>
        private Image image = new Image();
        public Image Image
        {
            get => this.image;
            set
            {
                this.image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        /// <summary>
        /// Mesh of the line series
        /// </summary>
        private Mesh mesh = new Mesh();
        public Mesh Mesh
        {
            get => this.mesh;
            set
            {
                this.mesh = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        ///         /// <summary>
        /// Default constructor
        /// </summary>
        public Series3D()
        {
            LineThickness = 1;
            LineColor = Brushes.Black;
            LinePattern = LinePatternEnum.Solid;
            Symbols.FillColor = LineColor;
            Symbols.SymbolSize = 0.3;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Brings a model back to it's original position
        /// </summary>
        public void Origin()
        {
            TranslateX = 0;
            TranslateY = 0;
            TranslateZ = 0;
            RotateX = 0;
            RotateY = 0;
            RotateZ = 0;
            Scale = 0;
        }

        /// <summary>
        /// Exporting the current model
        /// </summary>
        public void ExportModel()
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "OBJ (*.obj)|*.obj|STL (*.stl)|*.stl|X3D (*.x3d)|*.x3d|XAML (*.xaml)|*.xaml|XML (*.xml)|*.xml";
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
                                var k = new ObjExporter()
                                {
                                    TextureFolder = Path.GetDirectoryName(saveFileDialog1.FileName),
                                    TextureExtension=".jpg",
                                    TextureQualityLevel=100,
                                    TextureSize = 1024,
                                FileCreator = (f) => File.Create(System.IO.Path.Combine(Path.GetDirectoryName(saveFileDialog1.FileName), f))
                                };

                                 k.SwitchYZ = false;
                                 k.MaterialsFile = Path.ChangeExtension(Path.GetFileNameWithoutExtension(saveFileDialog1.FileName), ".mtl");
                                k.Export((Model3D)this.Model, stream);
                            }
                            break;
                        case ".stl":
                            using (FileStream stream = File.Create(saveFileDialog1.FileName))
                            {
                                var exp = new StlExporter()
                                {
                                    
                                };
                                exp.Export(this.Model, stream);

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