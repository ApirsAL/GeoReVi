using Accord.Math;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    public class SammonProjectionHelper : MultiVariateAnalysis
    {
        #region Members

        private double _lambda = 1;
        private int[] _indicesI;
        private int[] _indicesJ;

        #endregion

        #region Public properties

        /// <summary>
        /// The number of iterations.
        /// </summary>
        private int iteration = 0;
        public int Iteration
        {
            get => this.iteration;
            set
            {
                this.iteration = value;
                NotifyOfPropertyChange(() => Iteration);
            }
        }

        /// <summary>
        /// The maximum number of iterations.
        /// </summary>
        private int maxIteration = 0;
        public int MaxIteration
        {
            get => this.maxIteration;
            set
            {
                this.maxIteration = value;
                NotifyOfPropertyChange(() => MaxIteration);
            }
        }

        /// <summary>
        /// The maximum number of iterations.
        /// </summary>
        private int outputDimension = 2;
        public int OutputDimension
        {
            get => this.outputDimension;
            set
            {
                this.outputDimension = value;
                NotifyOfPropertyChange(() => OutputDimension);
            }

        }

        /// <summary>
        /// The projection data set of this class
        /// </summary>
        private double[][] projectedValues;
        public double[][] ProjectedValues
        {
            get => this.projectedValues;
            set
            {
                this.projectedValues = value;
                NotifyOfPropertyChange(() => ProjectedValues);
                NotifyOfPropertyChange(() => ProjectedValuesView);
            }
        }

        /// <summary>
        /// The view of the projected values
        /// </summary>
        public DataTable ProjectedValuesView
        {
            get
            {
                return this.ProjectedValues.ToTable();
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel lineChartViewModel = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel LineChartViewModel
        {
            get => lineChartViewModel;
            set
            {
                this.lineChartViewModel = value;
                NotifyOfPropertyChange(() => LineChartViewModel);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dt"></param>
        public SammonProjectionHelper()
        {

        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="dt"></param>
        public SammonProjectionHelper(IEnumerable<Mesh> _dataSet)
        {
            DataSet = new BindableCollection<Mesh>(_dataSet);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Computing the clustering
        /// </summary>
        public async override Task Compute()
        {
            if (DataSet.Count() == 0)
                return;

            CommandHelper ch = new CommandHelper();

            DataTable dat = new DataTable();

            foreach (Mesh dt in DataSet)
                dat.Merge(dt.Data);

            dat.RemoveNonNumericColumns();
            dat.RemoveNanRowsAndColumns();
            CollectionHelper.ProcessNumericDataTable(dat);
            dat.TreatMissingValues(MissingData);

            Merge = dat;
            CalculationDataSet = dat.ToMatrix();

            NormalizeDataSet();

            InitializeSammon();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {
                try
                {
                    for(int z = 0; z<MaxIteration;z++)
                    {
                        int[] indicesI = _indicesI;
                        int[] indicesJ = _indicesJ;
                        double[,] distanceMatrix = CalculateDistanceMatrix();
                        double[][] projection = ProjectedValues;

                        // Shuffle the indices-array for random pick of the points:
                        indicesI.FisherYatesShuffle();
                        indicesJ.FisherYatesShuffle();

                        for (int i = 0; i < indicesI.Length; i++)
                        {
                            double[] distancesI = distanceMatrix.GetRow(indicesI[i]);
                            double[] projectionI = projection[indicesI[i]];

                            for (int j = 0; j < indicesJ.Length; j++)
                            {
                                if (indicesI[i] == indicesJ[j])
                                    continue;

                                double[] projectionJ = ProjectedValues[indicesJ[j]];

                                double dij = distancesI[indicesJ[j]];
                                double Dij = ManhattenDistance(
                                        projectionI,
                                        projectionJ);

                                // Avoid division by zero:
                                if (Dij == 0)
                                    Dij = 1e-10;

                                double delta = _lambda * (dij - Dij) / Dij;

                                for (int k = 0; k < projectionJ.Length; k++)
                                {
                                    double correction =
                                        delta * (projectionI[k] - projectionJ[k]);

                                    projectionI[k] += correction;
                                    projectionJ[k] -= correction;
                                }
                            }
                        }

                        ProjectedValues = projectedValues;
                        // Reduce lambda monotonically:
                        ReduceLambda();
                    }


                    try
                    {
                        List<Mesh> projectedData = new List<Mesh>();

                        int j = 0;
                        for (int i = 0; i < DataSet.Count(); i++)
                        {
                            projectedData.Add(new Mesh() { Name = DataSet[i].Name, Data = ProjectedValues.ToTable().AsEnumerable().Skip(j).Take(DataSet[i].Data.Rows.Count).CopyToDataTable() });

                            j += DataSet[i].Data.Rows.Count;
                        }

                        LineChartViewModel.Lco.ShallRender = true;
                        LineChartViewModel.Lco.DataSet = new BindableCollection<Mesh>(projectedData);
                        LineChartViewModel.Lco.CreateScatterChart();
                    }
                    catch
                    {
                        return;
                    }
                }
                catch
                {

                }
            });
        }

        /// <summary>
        /// Intializing the sammon projection
        /// </summary>
        public void InitializeSammon()
        {
            Random rnd = new Random();
            ProjectedValues = CalculationDataSet.ToJagged();

            for (int i = 0; i < CalculationDataSet.GetRow(0).Length; i++)
            {
                for (int j = 0; j < CalculationDataSet.GetRow(0).Length; j++)
                    ProjectedValues[i][j] = Convert.ToDouble(rnd.Next(0, CalculationDataSet.GetRow(0).Length));
            }

            // Create the indices-arrays:
            _indicesI = Enumerable.Range(0, CalculationDataSet.GetRow(0).Length).ToArray();
            _indicesJ = new int[CalculationDataSet.GetRow(0).Length];
            _indicesI.CopyTo(_indicesJ, 0);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the distance matrix of a double matrix
        /// </summary>
        /// <returns></returns>
        private double[,] CalculateDistanceMatrix()
        {
            //Semivariance matrix for the ordinary kriging system
            double[,] distanceMatrix = new double[CalculationDataSet.GetRow(0).Length, CalculationDataSet.GetRow(0).Length];

            for (int i = 0; i < CalculationDataSet.GetRow(0).Length; i++)
            {
                for (int j = 0; j < CalculationDataSet.GetRow(0).Length; j++)
                {
                    if (j == i)
                    {
                        distanceMatrix[i, j] = 0;
                        continue;
                    }

                    distanceMatrix[i, j] = ManhattenDistance(CalculationDataSet.GetColumn(i), CalculationDataSet.GetColumn(j));
                }
            }

            return distanceMatrix;
        }
        

        /// <summary>
        /// Calculates the Manhatten distance of two vectors
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        private double ManhattenDistance(double[] vec1, double[] vec2)
        {
            double distance = 0;

            for (int i = 0; i < vec1.Length; i++)
                distance += Math.Abs(vec1[i] - vec2[i]);

            return distance;
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Reduziert Lambda entsprechend den Iterationen.
        /// </summary>
        private void ReduceLambda()
        {
            this.Iteration++;

            // Herleitung über den Ansatz y(t) = k.exp(-l.t).
            double ratio = (double)this.Iteration / MaxIteration;

            // Start := 1, Ende := 0.01
            _lambda = Math.Pow(0.01, ratio);
        }

        #endregion
    }
}
