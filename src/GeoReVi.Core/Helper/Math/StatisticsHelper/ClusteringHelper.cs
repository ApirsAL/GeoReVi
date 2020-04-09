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
    /// A class providing algorithms for clustering methods
    /// Some algorithms are taken from: https://visualstudiomagazine.com/Articles/2013/12/01/K-Means-Data-Clustering-Using-C.aspx?Page=1
    /// </summary>
    public class ClusteringHelper : MultiVariateAnalysis
    {
        #region Properties

        /// <summary>
        /// The selected clustering method
        /// </summary>
        private ClusteringMethod method = ClusteringMethod.KMeans;
        public ClusteringMethod Method
        {
            get => this.method;
            set
            {
                this.method = value;
                NotifyOfPropertyChange(() => Method);
            }
        }

        /// <summary>
        /// The resulted clusterings
        /// </summary>
        private int[] clustering;
        public int[] Clustering
        {
            get => this.clustering;
            set
            {
                this.clustering = value;
                NotifyOfPropertyChange(() => ClusteredDatasetView);
            }
        }

        /// <summary>
        /// The clustered data set of this class
        /// </summary>
        private double[][] clusteredDataSet;
        public double[][] ClusteredDataSet
        {
            get => this.clusteredDataSet;
            set
            {
                this.clusteredDataSet = value;
                NotifyOfPropertyChange(() => ClusteredDataSet);
                NotifyOfPropertyChange(() => ClusteredDatasetView);
            }
        }

        /// <summary>
        /// The resulting data set view
        /// </summary>
        private BindableCollection<KeyValuePair<string, DataTable>> clusteredDatasetView = new BindableCollection<KeyValuePair<string, DataTable>>();
        public BindableCollection<KeyValuePair<string, DataTable>> ClusteredDatasetView
        {
            get => this.clusteredDatasetView;
            set
            {
                this.clusteredDatasetView = value;
                NotifyOfPropertyChange(() => ClusteredDatasetView);
            }
        }

        /// <summary>
        /// The means of the clustered data sets
        /// </summary>
        private double[][] means;
        public double[][] Means
        {
            get => this.means;
            set
            {
                this.means = value;
                NotifyOfPropertyChange(() => Means);
            }
        }

        /// <summary>
        /// The number of clusters for supervised clustering
        /// </summary>
        private int numberOfClusters = 2;
        public int NumberOfClusters
        {
            get => this.numberOfClusters;
            set
            {
                this.numberOfClusters = value;
                NotifyOfPropertyChange(() => NumberOfClusters);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dt"></param>
        public ClusteringHelper()
        {

        }

        /// <summary>
        /// Constructor with data set
        /// </summary>
        /// <param name="dt"></param>
        public ClusteringHelper(BindableCollection<Mesh> _dataSet)
        {
            DataSet = _dataSet;
        }

        #endregion

        #region Compute

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

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsComputing, async () =>
            {

                try
                {
                    switch (Method)
                    {
                        case ClusteringMethod.KMeans:
                            bool changed = true; bool success = true;

                            NormalizeDataSet();

                            InitializeClustering();

                            int maxCount = ClusteredDataSet.Length * 10;
                            int ct = 0;

                            while (changed == true && success == true && ct < maxCount)
                            {
                                ++ct;
                                success = UpdateMeans();
                                changed = UpdateClustering();
                            }

                            var a = new BindableCollection<KeyValuePair<string, DataTable>>();
                            dat = DataSet.First().Data.Clone();

                            foreach (Mesh dt in DataSet)
                                dat.Merge(dt.Data);

                            //Adding a collection to the list for each cluster
                            for (int i = 0; i < NumberOfClusters; i++)
                                a.Add(new KeyValuePair<string, DataTable>("Cluster " + (i + 1).ToString(), DataSet.First().Data.Clone()));

                            try
                            {
                                ///Adding each data set to 
                                for (int i = 0; i < dat.Rows.Count; i++)
                                {
                                    a[Clustering[i]].Value.Rows.Add(dat.Rows[i].ItemArray);
                                }
                            }
                            catch
                            {

                            }

                            ClusteredDatasetView = a;

                            break;

                    }
                }
                catch
                {
                    return;
                }
            });

        }

        /// <summary>
        /// Initializing everything for the cluster analysis
        /// </summary>
        public void InitializeClustering()
        {
            Random random = new Random();

            //Clearing the result data sets
            if (ClusteredDataSet != null)
                ClusteredDataSet.Clear();
            if (Means != null)
                Means.Clear();

            ClusteredDataSet = CalculationDataSet.ToJagged();

            //Allocating the array of means to the required dimensions
            Means = Allocate(NumberOfClusters, CalculationDataSet.GetRow(0).Length);

            //Adding a cluster result to the result data set
            Clustering = new int[CalculationDataSet.GetLength(0)];

            for (int i = 0; i < CalculationDataSet.GetLength(0); ++i)
                Clustering[i] = i;

            for (int i = NumberOfClusters; i < Clustering.Length; ++i)
                Clustering[i] = random.Next(0, NumberOfClusters);
        }

        /// <summary>
        /// Updating the means and checking whether the number of points in the clusters changed
        /// </summary>
        /// <returns></returns>
        public bool UpdateMeans()
        {
            int numClusters = NumberOfClusters;
            int[] clusterCounts = new int[NumberOfClusters];
            for (int i = 0; i < Clustering.Length; ++i)
            {
                int cluster = Clustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < NumberOfClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            for (int k = 0; k < Means.Length; ++k)
                for (int j = 0; j < Means[k].Length; ++j)
                    Means[k][j] = 0.0;

            for (int i = 0; i < Clustering.Length; ++i)
            {
                int cluster = Clustering[i];
                for (int j = 0; j < ClusteredDataSet[i].Length; ++j)
                    Means[cluster][j] += ClusteredDataSet[i][j]; // accumulate sum
            }

            for (int k = 0; k < Means.Length; ++k)
                for (int j = 0; j < Means[k].Length; ++j)
                    Means[k][j] /= clusterCounts[k]; // danger of div by 0

            return true;
        }

        /// <summary>
        /// Updating the clusters
        /// </summary>
        /// <returns></returns>
        private bool UpdateClustering()
        {
            int numClusters = Means.Length;
            bool changed = false;

            int[] newClustering = new int[clustering.Length];
            Array.Copy(Clustering, newClustering, clustering.Length);

            double[] distances = new double[numClusters];

            for (int i = 0; i < Clustering.Length; ++i)
            {
                for (int k = 0; k < numClusters; ++k)
                    distances[k] = Distance(ClusteredDataSet[i], means[k]);

                int newClusterID = MinIndex(distances);
                if (newClusterID != newClustering[i])
                {
                    changed = true;
                    newClustering[i] = newClusterID;
                }
            }

            if (changed == false)
                return false;

            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < Clustering.Length; ++i)
            {
                int cluster = newClustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            Array.Copy(newClustering, Clustering, newClustering.Length);

            Clustering = newClustering;

            NotifyOfPropertyChange(() => ClusteredDatasetView);

            return true; // no zero-counts and at least one change
        }

        /// <summary>
        /// Allocating an array
        /// </summary>
        /// <param name="numClusters"></param>
        /// <param name="numColumns"></param>
        /// <returns></returns>
        private static double[][] Allocate(int numClusters, int numColumns)
        {
            double[][] result = new double[numClusters][];
            for (int k = 0; k < numClusters; ++k)
                result[k] = new double[numColumns];
            return result;
        }

        /// <summary>
        /// Calculating the euclidean distance
        /// </summary>
        /// <param name="tuple"></param>
        /// <param name="mean"></param>
        /// <returns></returns>
        private static double Distance(double[] tuple, double[] mean)
        {
            double sumSquaredDiffs = 0.0;
            for (int j = 0; j < tuple.Length; ++j)
                sumSquaredDiffs += Math.Pow((tuple[j] - mean[j]), 2);
            return Math.Sqrt(sumSquaredDiffs);
        }

        /// <summary>
        /// Getting the minimum index of a set of distances
        /// </summary>
        /// <param name="distances"></param>
        /// <returns></returns>
        private static int MinIndex(double[] distances)
        {
            int indexOfMin = 0;
            double smallDist = distances[0];
            for (int k = 0; k < distances.Length; ++k)
            {
                if (distances[k] < smallDist)
                {
                    smallDist = distances[k];
                    indexOfMin = k;
                }
            }
            return indexOfMin;
        }

        #endregion
    }

    /// <summary>
    /// A set of clustering methods
    /// </summary>
    public enum ClusteringMethod
    {
        KMeans = 1,
        KMedioid = 2
    }
}
