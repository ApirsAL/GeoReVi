using Accord.Math;
using Accord.Statistics.Analysis;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// An abstract class that provides the basic properties and methods for multivariate statistical analysis
    /// </summary>
    public abstract class MultiVariateAnalysis : PropertyChangedBase, IMultidimensionalDataSetHolder, IStatisticalAnalysis
    {
        //An intermediate list object for buffering the processed meshes
        protected List<DataTable> mergedDataSets = new List<DataTable>();

        /// <summary>
        /// The calculation dataSet
        /// </summary>
        private double[,] calculationDataSet = new double[,] { };
        public double[,] CalculationDataSet
        {
            get => this.calculationDataSet;
            set
            {
                this.calculationDataSet = value;
                NotifyOfPropertyChange(() => CalculationDataSetView);
            }
        }

        //Data set in jagged format
        public double[][] CalculationJaggedDataSet
        {
            get => CalculationDataSet.ToJagged();
        }

        /// <summary>
        /// The merges table used for analysis
        /// </summary>
        private DataTable merge = new DataTable();
        public DataTable Merge
        {
            get => this.merge;
            set
            {
                this.merge = value;
                NotifyOfPropertyChange(() => Merge);
            }
        }

        /// <summary>
        /// The view of the calculation data set
        /// </summary>
        public DataTable CalculationDataSetView
        {
            get
            {
                return this.CalculationDataSet.ToTable(DataSet.First().Properties.Select(x => x.Value).ToArray());
            }
        }

        /// <summary>
        /// The data set of this class
        /// </summary>
        private BindableCollection<Mesh> dataSet = new BindableCollection<Mesh>();
        public BindableCollection<Mesh> DataSet
        {
            get => this.dataSet;
            set
            {
                this.dataSet = value;
                NotifyOfPropertyChange(() => DataSet);
            }
        }


        /// <summary>
        /// Checks if the class holds a data set
        /// </summary>
        public bool HoldsData
        {
            get
            {
                if (this.DataSet != null)
                    if (this.DataSet.Count > 1)
                        return true;

                return false;
            }
        }

        /// <summary>
        /// Analysis method for a multivariate analysis
        /// </summary>
        private AnalysisMethod analysisMethod = AnalysisMethod.Center;
        public AnalysisMethod AnalysisMethod
        {
            get => this.analysisMethod;
            set
            {
                this.analysisMethod = value;
                NotifyOfPropertyChange(() => AnalysisMethod);
            }
        }

        /// <summary>
        /// The method how missing data should be treated
        /// </summary>
        private MissingDataTreatment missingData = MissingDataTreatment.ArithmeticAverage;
        public MissingDataTreatment MissingData
        {
            get => this.missingData;
            set
            {
                this.missingData = value;
                NotifyOfPropertyChange(() => MissingData);
            }
        }

        /// <summary>
        /// Defines, how a mesh should be joined
        /// </summary>
        private JoinMethod joinMethod = JoinMethod.Exact;
        public JoinMethod JoinMethod
        {
            get => this.joinMethod;
            set
            {
                this.joinMethod = value;
                NotifyOfPropertyChange(() => JoinMethod);
            }
        }

        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        private bool isComputing = false;
        public bool IsComputing
        {
            get => this.isComputing;
            set
            {
                this.isComputing = value;
                NotifyOfPropertyChange(() => IsComputing);
            }
        }

        /// <summary>
        /// Computing the analysis
        /// </summary>
        public virtual async Task Compute()
        {

        }
        
        /// <summary>
        /// Normalizing the data set based on an analysis method
        /// </summary>
        public virtual void NormalizeDataSet()
        {
            // Prepare the data, storing it in the new matrix.
            //AnalysisMethod.Standardize operates on the correlation matrix
            if (AnalysisMethod == Accord.Statistics.Analysis.AnalysisMethod.Standardize)
            {
                CalculationDataSet = CalculationDataSet.ZScore();
            }
            else if (AnalysisMethod == Accord.Statistics.Analysis.AnalysisMethod.Center)
            {
                CalculationDataSet = CalculationDataSet.Adjust();
            }
        }

        /// <summary>
        /// Converting a Accord.Statistics.Analysis.AnalysisMethod to a Accord.Statistics.Analysis.PrincipalComponentMethod
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public Accord.Statistics.Analysis.PrincipalComponentMethod ConvertMethod(Accord.Statistics.Analysis.AnalysisMethod method)
        {
            switch (method)
            {
                case AnalysisMethod.Center:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Center;
                case AnalysisMethod.Standardize:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Standardize;
                default:
                    return Accord.Statistics.Analysis.PrincipalComponentMethod.Center;
            }
        }

        /// <summary>
        /// Clears the data 
        /// </summary>
        /// <returns></returns>
        public virtual async Task ClearDataSets()
        {
            try
            {
                DataSet.Clear();
            }
            catch
            {

            }
        }

        /// <summary>
        /// Merges the data sets according to their group assignment
        /// </summary>
        /// <returns></returns>
        public virtual List<DataTable> MergeDataSets()
        {
            List<DataTable> ret = new List<DataTable>();

            try
            {
                for(int i = 0; i< DataSet.Count();i++)
                {
                    DataTable dat = MeshJoiner.ExtractDataTable(DataSet[i]);
                    ret.Add(dat);
                }
            }
            catch
            {
                throw new System.Exception("Data sets cannot be merged");
            }

            return ret;
        }
    }
}
