using Accord.Math;
using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Main part of the logic is taken from
    /// https://www.codeproject.com/Articles/43123/Sammon-Projection
    /// </summary>
    public class SammonProjectionHelper : MultiVariateAnalysis
    {
        #region Members

        private double _lambda = 1;
        private int[] _indicesI;
        private int[] _indicesJ;

        /// <summary>
        /// The precalculated distance-matrix.
        /// </summary>
        protected double[][] _distanceMatrix;

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
		/// The number of input-vectors.
		/// </summary>
		public int Count
        {
            get { return this.CalculationJaggedDataSet.Length; }
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
        private int outputDimension = 4;
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

            //Iterating
            for (int z = 0; z < MaxIteration; z++)
            {
                Iterate();
            }

            try
            {
                List<Mesh> projectedData = new List<Mesh>();

                int j = 0;
                for (int i = 0; i < DataSet.Count(); i++)
                {
                    projectedData.Add(new Mesh() { Name = DataSet[i].Name, Data = ProjectedValues.ToTable().AsEnumerable().Skip(j).Take(DataSet[i].Data.Rows.Count).CopyToDataTable() });
                    projectedData[i].Data.Columns[projectedData[i].Data.Columns.Count - 1].SetOrdinal(0);
                    j += DataSet[i].Data.Rows.Count;
                }

                LineChartViewModel.Lco.ShallRender = true;
                LineChartViewModel.Lco.Direction = DirectionEnum.XY;
                LineChartViewModel.Lco.DataSet = new BindableCollection<Mesh>(projectedData);
                LineChartViewModel.Lco.CreateLineChart();
                LineChartViewModel.Lco.XLabel.Text = "Dimension 1";
                LineChartViewModel.Lco.YLabel.Text = "Dimension 2";
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Making one iteration
        /// </summary>
        public void Iterate()
        {
            try
            {
                int[] indicesI = _indicesI;
                int[] indicesJ = _indicesJ;
                double[][] distanceMatrix = _distanceMatrix;
                double[][] projection = ProjectedValues;

                // Shuffle the indices-array for random pick of the points:
                indicesI.FisherYatesShuffle();
                indicesJ.FisherYatesShuffle();

                for (int i = 0; i < indicesI.Length; i++)
                {
                    double[] distancesI = distanceMatrix[indicesI[i]];
                    double[] projectionI = projection[indicesI[i]];

                    for (int j = 0; j < indicesJ.Length; j++)
                    {
                        if (indicesI[i] == indicesJ[j])
                            continue;

                        double[] projectionJ = projection[indicesJ[j]];

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

                // Reduce lambda monotonically:
                ReduceLambda();

                ProjectedValues = projection;
            }
            catch
            {

            }
        }

        /// <summary>
        /// Intializing the sammon projection
        /// </summary>
        public void InitializeSammon()
        {
            _distanceMatrix = CalculateDistanceMatrix();

            // Initialize random points for the projection:
            Random rnd = new Random();
            double[][] projection = new double[this.Count][];
            this.ProjectedValues = projection;
            for (int i = 0; i < projection.Length; i++)
            {
                double[] projectionI = new double[this.OutputDimension];
                projection[i] = projectionI;
                for (int j = 0; j < projectionI.Length; j++)
                    projectionI[j] = rnd.Next(0, this.Count);
            }

            // Create the indices-arrays:
            _indicesI = Enumerable.Range(0, this.Count).ToArray();
            _indicesJ = new int[this.Count];
            _indicesI.CopyTo(_indicesJ, 0);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Calculates the distance matrix of a double matrix
        /// </summary>
        /// <returns></returns>
        private double[][] CalculateDistanceMatrix()
        {
            double[][] distanceMatrix = new double[this.Count][];
            double[][] inputData = this.CalculationJaggedDataSet;

            for (int i = 0; i < distanceMatrix.Length; i++)
            {
                double[] distances = new double[this.Count];
                distanceMatrix[i] = distances;

                double[] inputI = inputData[i];

                for (int j = 0; j < distances.Length; j++)
                {
                    if (j == i)
                    {
                        distances[j] = 0;
                        continue;
                    }

                    distances[j] = ManhattenDistance(
                        inputI,
                        inputData[j]);
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
