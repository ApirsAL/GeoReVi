using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class AnalyticalHierarchyProcessHelper : MultiVariateAnalysis
    {
        #region Properties

        /// <summary>
        /// The result of the analysis
        /// </summary>
        private DataTable classificationResult = new DataTable();
        public DataTable ClassificationResult
        {
            get =>  this.classificationResult;
            set
            {
                this.classificationResult = value;
                NotifyOfPropertyChange(() => ClassificationResult);
            }
        }

        /// <summary>
        /// The priority matrix of the analysis
        /// </summary>
        private DataTable priorityMatrix = new DataTable();
        public DataTable PriorityMatrix
        {
            get => this.priorityMatrix;
            private set
            {
                this.priorityMatrix = value;
                NotifyOfPropertyChange(() => PriorityMatrix);
            }
        }

        /// <summary>
        /// Headers of the table
        /// </summary>
        private BindableCollection<string> headers = new BindableCollection<string>();
        public BindableCollection<string> Headers
        {
            get => this.headers;
            set
            {
                this.headers = value;
                NotifyOfPropertyChange(() => Headers);
            }
        }

        /// <summary>
        /// Priorities for each header
        /// </summary>
        private BindableCollection<Number> priorities = new BindableCollection<Number>();
        public BindableCollection<Number> Priorities
        {
            get => this.priorities;
            set
            {
                this.priorities = value;
                NotifyOfPropertyChange(() => Priorities);
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[] eigenValues;
        public double[] EigenValues
        {
            get => this.eigenValues;
            set
            {
                this.eigenValues = value;
                NotifyOfPropertyChange(() => MaximumEigenValue); 
            }
        }

        /// <summary>
        /// Normalized Eigenvalues of the singular values
        /// </summary>
        private double[] eigenValuesNormalized;
        public double[] EigenValuesNormalized
        {
            get => this.eigenValuesNormalized;
            set
            {
                this.eigenValuesNormalized = value;
                NotifyOfPropertyChange(() => EigenValuesNormalized);
            }
        }

        /// <summary>
        /// The weights
        /// </summary>
        private double[] weights;
        public double[] Weights
        {
            get => this.weights;
            set
            {
                this.weights = value;
                NotifyOfPropertyChange(() => Weights);
            }
        }

        /// <summary>
        /// Consistency index of the matrix
        /// </summary>
        private double consistencyIndex = 0.0;
        public double ConsistencyIndex
        {
            get => this.consistencyIndex;
            set
            {
                this.consistencyIndex = value;
                NotifyOfPropertyChange(() => ConsistencyIndex);
            }
        }

        /// <summary>
        /// Consistency ratio of the matrix
        /// </summary>
        private double consistencyRatio = 0.0;
        public double ConsistencyRatio
        {
            get => this.consistencyRatio;
            set
            {
                this.consistencyRatio = value;
                NotifyOfPropertyChange(() => ConsistencyRatio);
            }
        }

        private Dictionary<int, double> matrixRankToRandomIConsistencyndex = new Dictionary<int, double>();

        /// <summary>
        /// The projected values
        /// </summary>
        private double[][] projectedValues;
        public double[][] ProjectedValues
        {
            get => this.projectedValues;
            private set
            {
                this.projectedValues = value;
                NotifyOfPropertyChange(() => ProjectedValues);
                NotifyOfPropertyChange(() => ProjectedValuesView);
            }
        }

        /// <summary>
        /// The maximum eigenvalue as read-only property
        /// </summary>
        public double MaximumEigenValue
        {
            get => this.EigenValues.Max();
        }

        /// <summary>
        /// The view of the projected values
        /// </summary>
        public DataTable ProjectedValuesView
        {
            get
            {
                return ProjectedValues.ToTable();
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public AnalyticalHierarchyProcessHelper()
        {

        }

        #endregion

        #region Methods

        /// <summary>
        /// Preprocessing
        /// </summary>
        /// <returns></returns>
        public async Task Preprocess()
        {
            try
            {

                this.mergedDataSets = MergeDataSets();

                CommandHelper ch = new CommandHelper();

                DataTable dat = new DataTable();

                foreach (DataTable dt in this.mergedDataSets)
                    dat.Merge(dt, true, MissingSchemaAction.Add);

                dat.RemoveNonNumericColumns();
                CollectionHelper.ProcessNumericDataTable(dat);
                dat.RemoveNanRowsAndColumns();
                dat.TreatMissingValues(MissingData);

                Merge = dat;
                //Converting the data set to a matrix
                CalculationDataSet = dat.ToMatrix();

                //Clearing the headers
                Headers.Clear();
                Priorities.Clear();

                //Adding headers to list
                for (int i = 0; i < dat.Columns.Count; i++)
                {
                    Headers.Add(dat.Columns[i].Caption);
                    Priorities.Add(new Number(1.0));
                }
            }
            catch
            {
                throw new Exception("AHP preprocessing failed.");
            }

        }

        /// <summary>
        /// Updating the priority table
        /// </summary>
        /// <returns></returns>
        public async Task UpdatePriorityMatrix()
        {
            try
            {
                PriorityMatrix = new DataTable();
                DataColumn dc = new DataColumn("", typeof(string));
                PriorityMatrix.Columns.Add(dc);

                for (int i = 0; i < Headers.Count(); i++)
                {
                    DataColumn dc1 = new DataColumn(Headers[i], typeof(double));
                    PriorityMatrix.Columns.Add(dc1);
                }
                for (int i = 0; i < Headers.Count(); i++)
                {
                    DataRow dr = PriorityMatrix.NewRow();
                    PriorityMatrix.Rows.Add(dr);
                }
                for (int i = 1; i < Headers.Count() + 1; i++)
                {
                    for (int j = 0; j < Headers.Count(); j++)
                    {
                        PriorityMatrix.Rows[i][j] = Convert.ToDouble(Priorities[i-1].Num / Priorities[j].Num);
                    }
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Computing the AHP
        /// </summary>
        /// <returns></returns>
        public async override Task Compute()
        {
            try
            {
                double[] classificationResults = new double[CalculationDataSet.Length];

                // Let's create an analysis with centering (covariance method)
                // but no standardization (correlation method) and whitening:
                var pca = new PrincipalComponentAnalysis()
                {
                    Method = ConvertMethod(AnalysisMethod),
                    Whiten = true
                };

                // Now we can learn the linear projection from the data
                MultivariateLinearRegression transform = pca.Learn(CalculationDataSet.ToJagged());

                EigenValues = pca.Eigenvalues;

                double maxEigenValue = EigenValues.Max();
                double matrixRank = Matrix.Rank(PriorityMatrix.ToMatrix());
                CalculateWeights();

                //Calculating consistency matrix
                ConsistencyIndex = (maxEigenValue - matrixRank) / (matrixRank - 1);

                for (int i = 0; i < CalculationDataSet.Length; i++)
                {
                    classificationResults[i] = CalculationDataSet.GetRow(i).Dot(weights);
                }

                CalculationDataSet.InsertColumn(classificationResults);

                ProjectedValues = CalculationDataSet.ToJagged();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Calculating the weights for the single parameters
        /// </summary>
        private void CalculateWeights()
        {
            try
            {
                weights = new double[PriorityMatrix.Rows.Count];
                double[] sums = new double[PriorityMatrix.Rows.Count];

                for (int i = 0; i < weights.Length; i++)
                    for (int j = 1; j < PriorityMatrix.Columns.Count + 1; j++)
                        sums[i] += Convert.ToDouble(PriorityMatrix.Rows[i][j]);
                for (int i = 0; i < weights.Length; i++)
                    weights[i] = sums[i] / sums.Sum();
            }
            catch
            {
                throw new Exception("Weights of the AHP couldn't be calculated.");
            }
        }

        /// <summary>
        /// Getting the random consistency index
        /// </summary>
        /// <param name="rank"></param>
        /// <returns></returns>
        public double GetRandomConsistencyIndex(int rank)
        {
            try
            {
                switch(rank)
                {
                    case 0: return 0;
                    case 1: return  0;
                    case 2: return 0;
                    case 3: return 0.52;
                    case 4: return 0.89;
                    case 5: return 1.11;
                    case 6: return 1.25;
                    case 7: return 1.35;
                    case 8: return 1.4;
                    case 9: return 1.45;
                    case 10: return 1.49;
                    default: return 0;
                }

            }
            catch
            {
                return 0;
            }
        }

        #endregion
    }
}
