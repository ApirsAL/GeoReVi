using System.Linq;

namespace GeoReVi
{
    public class UnivariateHeterogeneityMeasuresHelper : BasicUnivariateStatisticalMeasuresHelper, IStatisticalAnalysis
    {

        #region Properties

        /// <summary>
        /// Coefficient of variation
        /// </summary>
        private double coefficientOfVariation = 0;
        public double CoefficientOfVariation
        {
            get => this.coefficientOfVariation;
            private set
            {
                this.coefficientOfVariation = value;
                NotifyOfPropertyChange(() => CoefficientOfVariation);
            }
        }

        /// <summary>
        /// Dykstra Parsons coefficient
        /// </summary>
        private double dykstraParsonsCoefficient = 0;
        public double DykstraParsonsCoefficient
        {
            get => this.dykstraParsonsCoefficient;
            private set
            {
                this.dykstraParsonsCoefficient = value;
                NotifyOfPropertyChange(() => DykstraParsonsCoefficient);
            }
        }

        /// <summary>
        /// Coefficient of variation
        /// </summary>
        private double lorenzCoefficient = 0;
        public double LorenzCoefficient
        {
            get => this.lorenzCoefficient;
            private set
            {
                this.lorenzCoefficient = value;
                NotifyOfPropertyChange(() => LorenzCoefficient);
            }
        }

        #region Constructor

        public UnivariateHeterogeneityMeasuresHelper(double[] _dataSet, string dataSetName = "") : base(_dataSet, dataSetName)
        {
            Compute();
        }

        #endregion

        public void Compute()
        {
            try
            {
                CoefficientOfVariation = ComputeCoefficientOfVariation(DataSet);
                DykstraParsonsCoefficient = ComputeDykstraParsonsCoefficient(DataSet);
            }
            catch
            {
                return;
            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Calculating the COV
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private double ComputeCoefficientOfVariation(double[] dataSet)
        {
             return dataSet.StdDev() / dataSet.Average();
        }

        /// <summary>
        /// Calculating the DPC
        /// </summary>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        private double ComputeDykstraParsonsCoefficient(double[] dataSet)
        {
            double p50 = ComputePercentile(50);
            double p16 = ComputePercentile(16);
            return 1 - p16/p50;
        }



        #endregion
    }
}
