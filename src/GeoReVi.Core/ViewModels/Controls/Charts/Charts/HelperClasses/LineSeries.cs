using System.Windows.Media;
using Caliburn.Micro;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class LineSeries : PropertyChangedBase 
    {
        #region Private members

        private Symbols<LineSeries> symbols;

        //Name of the line series
        private string seriesName = "Default";

        private Brush lineColor = Brushes.Black;

        //Double collection of line patterns
        private DoubleCollection lineDashPattern;

        private double lineThickness = 1;

        private BindableCollection<LineSeries> _a { get; set; }

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

        /// <summary>
        /// Convex hull of the points
        /// </summary>
        private PointCollection hull = new PointCollection();
        public PointCollection Hull
        {
            get => this.hull;
            set
            {
                this.hull = value;
                NotifyOfPropertyChange(() => Hull);
            }
        }

        /// <summary>
        /// RegressionHelper
        /// </summary>
        private RegressionHelper regressionHelper = new RegressionHelper();
        public RegressionHelper RegressionHelper
        {
            get => this.regressionHelper;
            set
            {
                this.regressionHelper = value;
                NotifyOfPropertyChange(() => RegressionHelper);
            }
        }

        //Point collection that build up the line series
        private BindableCollection<LocationTimeValue> linePoints = new BindableCollection<LocationTimeValue>();
        public BindableCollection<LocationTimeValue> LinePoints
        {
            get => this.linePoints;
            set
            {
                this.linePoints = value;
                NotifyOfPropertyChange(() => LinePoints);
            }
        }

        ///Point collection that build up the regression of the series
        private PointCollection regressionLinePoints = new PointCollection();
        public PointCollection RegressionLinePoints
        {
            get => this.regressionLinePoints;
            set
            {
                this.regressionLinePoints = value;
                NotifyOfPropertyChange(() => RegressionLinePoints);
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

        //Direction of the rotation
        private DirectionEnum rotationAxis = DirectionEnum.Z;
        [XmlIgnore]
        public DirectionEnum RotationAxis
        {
            get => this.rotationAxis;
            set
            {
                this.rotationAxis = value;
                NotifyOfPropertyChange(() => RotationAxis);
            }
        }


        //Rotation angle in °
        private double rotationAngle = 0;
        public double RotationAngle
        {
            get => this.rotationAngle;
            set
            {
                this.rotationAngle = value;
                NotifyOfPropertyChange(() => RotationAngle);
            }
        }

        #endregion

        //Color of the line
        [XmlIgnore()]
        public Brush LineColor
        {
            get => this.lineColor;
            set
            {
                this.lineColor = value;

                if (_a != null)
                    if(_a.Count != 0)
                        _a.UpdateChart();

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
        [XmlIgnore]
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

        //Symbols used for the chart control
        public Symbols<LineSeries> Symbols
        {
            get =>this.symbols;
            set
            {
                this.symbols = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineSeries()
        {
            LinePoints = new BindableCollection<LocationTimeValue>();
            Symbols = new Symbols<LineSeries>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LineSeries(BindableCollection<LineSeries> a)
        {
            _a = a;
            LinePoints = new BindableCollection<LocationTimeValue>();
            Symbols = new Symbols<LineSeries>(a);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Remove line series from collection
        /// </summary>
        public void Remove(int i)
        {
            if (_a != null && _a.Count > 0)
            {
                _a.RemoveAt(i);
                _a.UpdateChart();
            }
        }

        /// <summary>
        /// Moves line series one index up
        /// </summary>
        public void IndexUp(int i)
        {
            if (_a != null && _a.Count > 0)
            {
                if (i > 0)
                {
                    _a.Move(i, i - 1);
                    _a.UpdateChart();
                }
            }
        }

        /// <summary>
        /// Moves line series one index lower
        /// </summary>
        public void IndexDown(int i)
        {
            if (_a != null && _a.Count > 0)
            {
                if (i < _a.Count)
                {
                    _a.Move(i, i + 1);
                    _a.UpdateChart();
                }
            }
        }

        //Setter method for the line pattern (value converter)
        public void SetLinePattern()
        {
            switch (LinePattern)
            {
                case LinePatternEnum.Dash:
                    LineDashPattern = new DoubleCollection() { 4, 3 };
                    break;
                case LinePatternEnum.Dot:
                    LineDashPattern = new DoubleCollection() { 1, 2 };
                    break;
                case LinePatternEnum.DashDot:
                    LineDashPattern = new DoubleCollection() { 4, 2, 1, 2 };
                    break;
            }
        }

        /// <summary>
        /// Computes the convex hull of the Line points
        /// </summary>
        public void ComputeConvexHull()
        {
            try
            {
                Hull = ConvexHull.ComputeConvexHull2D(LinePoints.ToList()).ToList().ToPointCollection();
            }
            catch
            {
                Hull = new PointCollection();
            }
        }

        /// <summary>
        /// Computing the regression
        /// </summary>
        public void ComputeRegression()
        {
            try
            {
               RegressionLinePoints = RegressionHelper.Compute(LinePoints.ToList());
            }
            catch
            {

            }
        }

        #endregion

    }
    /// <summary>
    /// Enum of the line patterns
    /// </summary>
    public enum LinePatternEnum
    {
        [Description("–")]
        Solid = 1,
        [Description("--")]
        Dash = 2,
        [Description(". .")]
        Dot = 3,
        [Description("-.")]
        DashDot = 4,
        [Description("None")]
        None =5
    }
}
