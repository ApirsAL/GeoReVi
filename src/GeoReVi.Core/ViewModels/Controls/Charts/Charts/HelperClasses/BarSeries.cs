using System.Windows.Media;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeoReVi
{
    public class BarSeries : PropertyChangedBase
    {
        #region Private members

        //name of the series
        private string seriesName = "Default";

        //Fill color for the rectangles
        private Brush fillColor = Brushes.Blue;

        //Color of the border
        private Brush borderColor = Brushes.Black;

        //thickness of the border
        private double borderThickness = 2;

        //width of the border
        private double barWidth = 0.8;

        private List<string> barNames = new List<string>();

        //The bin subdivision
        private double[] bins;

        //Counts of the single bins
        private double[] counts;

        /// <summary>
        /// The value set for the histogram
        /// </summary>
        private List<double> values;

        //Data collection
        private BindableCollection<BarSeries> _a;


        #endregion

        #region Public properties

        public LineSeries LineSeriesBar { get; set; }

        /// Point collection for the bar series
        private BindableCollection<Rectangle2D> barPoints = new BindableCollection<Rectangle2D>();
        public BindableCollection<Rectangle2D> BarPoints
        {
            get => this.barPoints;
            set
            {
                this.barPoints = value;
                NotifyOfPropertyChange(() => BarPoints);
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

        /// <summary>
        ///  Bar names collection
        /// </summary>
        public List<string> BarNames
        {
            get => this.barNames;
            set
            {
                this.barNames = value;

                NotifyOfPropertyChange(() => BarNames);
            }
        }

        //fill color property
        [XmlIgnore()]
        public Brush FillColor
        {
            get { return this.fillColor; }
            set
            {
                this.fillColor = value;

                NotifyOfPropertyChange(() => FillColor);
            }
        }

        //Border color property
        [XmlIgnore()]
        public Brush BorderColor
        {
            get { return this.borderColor; }
            set
            {
                this.borderColor = value;

                NotifyOfPropertyChange(() => BorderColor);
            }
        }

        //Border thickness property
        public double BorderThickness
        {
            get { return this.borderThickness; }
            set
            {
                if (value < 0)
                    this.borderThickness = 0;
                else if (value > 20)
                    this.borderThickness = 20;
                else
                    this.borderThickness = value;

                NotifyOfPropertyChange(() => BorderThickness);
            }
        }

        //Bar width property
        public double BarWidth
        {
            get { return this.barWidth; }
            set
            {
                this.barWidth = value;
                NotifyOfPropertyChange(() => BarWidth);
            }
        }

        //Name of the series
        public string SeriesName
        {
            get { return this.seriesName; }
            set
            {
                this.seriesName = value;
                NotifyOfPropertyChange(() => SeriesName);
            }
        }

        /// <summary>
        /// Values of the bar chart
        /// </summary>
        public List<double> Values
        {
            get => this.values;
            set
            {
                this.values = value;
                NotifyOfPropertyChange(() => Values);
            }
        }

        /// <summary>
        /// The counts for the bins
        /// </summary>
        public double[] Counts
        {
            get => this.counts;
            set
            {
                counts = value;
                NotifyOfPropertyChange(() => Counts);
            }
        }

        /// <summary>
        /// A helper for empirical distribution analysis
        /// </summary>
        private EmpiricalDistributionHelper<BarSeries> edh;
        public EmpiricalDistributionHelper<BarSeries> EDH
        {
            get => this.edh;
            set
            {
                this.edh = value;
                NotifyOfPropertyChange(() => EDH);
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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BarSeries()
        {
            LineSeriesBar = new LineSeries();
            EDH = new EmpiricalDistributionHelper<BarSeries>();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BarSeries(BindableCollection<BarSeries> a)
        {
            _a = a;
            LineSeriesBar = new LineSeries();
            EDH = new EmpiricalDistributionHelper<BarSeries>();
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

        #endregion
    }


}
