using Caliburn.Micro;
using System;
using System.ComponentModel;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Class to represent a lithological chart object
    /// </summary>
    [Serializable]
    public class LithoLogChartObject : ChartObject<SectionLayerSeries>
    {
        #region Public properties
        
        //Title of the chart
        private string metaInformation;
        public string MetaInformation
        {
            get => this.metaInformation;
            set
            {
                this.metaInformation = value;
                NotifyOfPropertyChange(() => MetaInformation);
            }

        }

        private int xmax;
        public int Xmax
        {
            get => this.xmax;
            set
            {
                if (value <= 0)
                    xmax = 1;
                else if (value > 11)
                    xmax = 11;
                else
                    xmax = value;

                NotifyOfPropertyChange(() => Xmax);
            }
        }

        private int xmin;
        public int Xmin
        {
            get => this.xmin;
            set
            {
                if (value < 0)
                    xmin = 0;
                else if (value > 10)
                    xmin = 10;
                else
                    xmin = value;

                NotifyOfPropertyChange(() => Xmin);
            }
        }

        /// <summary>
        /// Lithofacies
        /// </summary>
        private BindableCollection<tblSectionLithofacy> lithofaciesLayers = new BindableCollection<tblSectionLithofacy>();
        public BindableCollection<tblSectionLithofacy> LithofaciesLayers
        {
            get => this.lithofaciesLayers;
            set
            {
                this.lithofaciesLayers = value;
                NotifyOfPropertyChange(() => LithofaciesLayers);
            }
        }

        /// <summary>
        /// Lithological layer series
        /// </summary>
        private BindableCollection<SectionLayerSeries> lithologicalLayerSeries = new BindableCollection<SectionLayerSeries>();
        public BindableCollection<SectionLayerSeries> LithologicalLayerSeries
        {
            get => this.lithologicalLayerSeries;
            set
            {
                this.lithologicalLayerSeries = value;
                NotifyOfPropertyChange(() => LithologicalLayerSeries);
            }
        }

        /// <summary>
        /// Lithological layer series
        /// </summary>
        private BindableCollection<SectionLayerSeries> stratigraphicLayerSeries = new BindableCollection<SectionLayerSeries>();
        public BindableCollection<SectionLayerSeries> StratigraphicLayerSeries
        {
            get => this.stratigraphicLayerSeries;
            set
            {
                this.stratigraphicLayerSeries = value;
                NotifyOfPropertyChange(() => StratigraphicLayerSeries);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LithoLogChartObject()
        {
            DataCollection = new BindableCollection<SectionLayerSeries>();
        }

        #endregion

        #region Public methods

        public void AddLithologicalSeries()
        {
            try
            {
                LithologicalLayerSeries.Add(new SectionLayerSeries()
                {
                    BorderColor = System.Windows.Media.Brushes.Black,
                    FillColor = System.Windows.Media.Brushes.DarkSlateGray
                });
            }
            catch
            {

            }
        }

        public void InitializeStandardLog()
        {
            LithologicalLayerSeries.Clear();
            StratigraphicLayerSeries.Clear();

            Xmax = 11;

            XLabel.Text = "Grain size";
        }

        /// <summary>
        /// Creating the chart
        /// </summary>
        public void CreateLithologicalLog()
        {
            try
            {
                DataCollection.Clear();

                InitializeStandardLog();

                //Grouping by lithofacies type
                string[] groups = LithofaciesLayers.GroupBy(x => x.litseclftCode).Select(x => x.Key).ToArray();

                //Adding lithofacies groups to the section
                for(int i = 0; i < groups.Length; i++)
                {
                    AddLithologicalSeries();
                    LithologicalLayerSeries[i]
                        .LithologicalLayers
                        .AddRange(LithofaciesLayers.Where(x => x.litseclftCode == groups[i])
                        .OrderByDescending(x => x.litsecBase).Select(x => new LithologicalLayer()
                    {
                        Top = (double)x.litsecTop,
                        Base = (double)x.litsecBase,
                        GrainSizeBase = new GrainSizeToIntConverter().Convert(x.litsecGrainSizeBase),
                        GrainSizeTop = new GrainSizeToIntConverter().Convert(x.litsecGrainSizeTop)
                        }));
                }

                YLabel.Text = "Depth [m]";
                double diff = Math.Abs(Ymax - Ymin);
                DataCollection.AddRange(LithologicalLayerSeries);
            }
            catch
            {
                return;
            }
           
        }

        #endregion
    }
}
