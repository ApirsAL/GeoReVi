using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class RegressionHelper : MultiVariateAnalysis
    {

        #region Public properties

        /// <summary>
        /// Gradient of the regression function
        /// </summary>
        private double gradient = 0;
        public double Gradient
        {
            get => this.gradient;
            set
            {
                this.gradient = value;
                NotifyOfPropertyChange(() => Gradient);
            }
        }

        /// <summary>
        /// Interception point of the regression function
        /// </summary>
        private double axisInterceptionPoint = 0;
        public double AxisInterceptionPoint
        {
            get => this.axisInterceptionPoint;
            set
            {
                this.axisInterceptionPoint = value;
                NotifyOfPropertyChange(() => AxisInterceptionPoint);
            }
        }

        #endregion


        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public RegressionHelper()
        {

        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="_dataSet"></param>
        public RegressionHelper(IEnumerable<Mesh> _dataSet)
        {
            this.DataSet = new BindableCollection<Mesh>(_dataSet);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Computing a regression analysis
        /// </summary>
        public async override Task Compute()
        {
            CommandHelper ch = new CommandHelper();

            double[][] correlationMatrix = new double[][] { };

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {

                

            });

        }

        #endregion
    }
}
