using Caliburn.Micro;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class SpatialInterpolationHelper<T> : SpatialDiscretizationHelper<T> where T : struct
    {
        #region Public properties

        /// <summary>
        /// The wanted interpolation method
        /// </summary>
        private GeostatisticalInterpolationMethod interpolationMethod;
        public GeostatisticalInterpolationMethod InterpolationMethod
        {
            get => this.interpolationMethod;
            set
            {
                this.interpolationMethod = value;
                NotifyOfPropertyChange(() => this.InterpolationMethod);
            }
        }

        /// <summary>
        /// The wanted interpolation method
        /// </summary>
        private GeostatisticalInterpolationMethod estimationVariance;
        public GeostatisticalInterpolationMethod EstimationVariance
        {
            get => this.estimationVariance;
            set
            {
                this.estimationVariance = value;
                NotifyOfPropertyChange(() => this.EstimationVariance);
            }
        }

        /// <summary>
        /// The power parameter needed in the IDW interpolation
        /// </summary>
        private double power = 2;
        public double Power
        {
            get => this.power;
            set
            {
                if (value >= -10 && value <= 10)
                    this.power = value;
                else
                    this.power = 2;

                NotifyOfPropertyChange(() => Power);
            }
        }

        /// <summary>
        /// A variogram helper object
        /// </summary>
        private VariogramHelper vh = new VariogramHelper();
        public VariogramHelper Vh
        {
            get => this.vh;
            set
            {
                this.vh = value;
                NotifyOfPropertyChange(() => Vh);
            }
        }

        /// <summary>
        /// The Root-Mean-Square-Error of the algorithm
        /// </summary>
        private double rmse = 0;
        public double RMSE
        {
            get => this.rmse;
            set
            {
                this.rmse = value;
                NotifyOfPropertyChange(() => RMSE);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialInterpolationHelper(BindableCollection<LocationTimeValue<double>> locationValues,
            VariogramHelper _vh,
            int binsX = 20,
            int binsY = 20,
            int binsZ = 20,
            DiscretizationMethod _discretizationMethod = DiscretizationMethod.RegularGrid,
            DirectionEnum _direction = DirectionEnum.XYZ)
        {
            DiscretizedLocationValues = new List<LocationTimeValue<double>>();
            OriginalLocationValues = locationValues;

            BinsX = binsX;
            BinsY = binsY;
            BinsZ = binsZ;
            DiscretizationMethod = _discretizationMethod;
            Direction = _direction;

            Vh = _vh;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="locationValues"></param>
        public SpatialInterpolationHelper() : base()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Computing the interpolation
        /// </summary>
        public void ComputeInterpolation()
        {
            switch (InterpolationMethod)
            {
                case GeostatisticalInterpolationMethod.IDW:
                    InverseDistanceWeightingHelper idw = new InverseDistanceWeightingHelper(OriginalLocationValues, BinsX, BinsY, BinsZ, DiscretizationMethod, Direction, Power);
                    idw.ComputeIDW();
                    DiscretizedLocationValues = idw.DiscretizedLocationValues;
                    RMSE = idw.RMSE;
                    break;
                case GeostatisticalInterpolationMethod.OrdinaryKriging:
                    KrigingHelper kh = new KrigingHelper(OriginalLocationValues, Vh, BinsX, BinsY, BinsZ, DiscretizationMethod, Direction);
                    kh.ComputeOrdinaryKriging();
                    DiscretizedLocationValues = kh.DiscretizedLocationValues;
                    DiscretizedLocationVariance = kh.DiscretizedLocationVariance;
                    RMSE = kh.RMSE;
                    break;
                case GeostatisticalInterpolationMethod.SimpleKriging:
                    KrigingHelper skh = new KrigingHelper(OriginalLocationValues, Vh, BinsX, BinsY, BinsZ, DiscretizationMethod, Direction);
                    skh.ComputeSimpleKriging();
                    DiscretizedLocationValues = skh.DiscretizedLocationValues;
                    RMSE = skh.RMSE;
                    break;
            }
        }

        #endregion
    }
}
