using Caliburn.Micro;

namespace GeoReVi
{
    public class SectionLayerSeries : BarSeries
    {
        #region Public properties

        /// <summary>
        /// Collection of lithological layers
        /// </summary>
        private BindableCollection<Layer> lithologicalLayers = new BindableCollection<Layer>();
        public BindableCollection<Layer> LithologicalLayers
        {
            get => this.lithologicalLayers;
            set
            {
                this.lithologicalLayers = value;
                NotifyOfPropertyChange(() => LithologicalLayers);
            }
        }

        /// <summary>
        /// Collection of lithological layers
        /// </summary>
        private BindableCollection<Layer> stratigraphicLayers = new BindableCollection<Layer>();
        public BindableCollection<Layer> StratigraphicLayers
        {
            get => this.stratigraphicLayers;
            set
            {
                this.stratigraphicLayers = value;
                NotifyOfPropertyChange(() => StratigraphicLayers);
            }
        }

        public BindableCollection<LineSeries> VerticalLogs { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SectionLayerSeries()
        {

        }

        #endregion
    }
}
