using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Models.Regression.Linear;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// A class providing algorithms for principal component analysis
    /// </summary>
    public class PrincipalComponentHelper : MultiVariateAnalysis
    {
        #region Public properties

        /// <summary>
        /// The PCA method
        /// </summary>
        private PrincipalComponentMethod method = PrincipalComponentMethod.Eigendecomposition;
        public PrincipalComponentMethod Method
        {
            get => this.method;
            set
            {
                this.method = value;
                NotifyOfPropertyChange(() => Method);
            }
        }

        /// <summary>
        /// Singular values of the data set
        /// </summary>
        private double[][] eigenVectors;
        public double[][] EigenVectors
        {
            get => this.eigenVectors;
            set
            {
                this.eigenVectors = value;
                NotifyOfPropertyChange(() => EigenVectorsView);
            }
        }

        /// <summary>
        /// The view of the eigen vectors
        /// </summary>
        public DataTable EigenVectorsView
        {
            get
            {
                //Preparing the view
                string[] columnNames = new string[Merge.Columns.Count];
                string[] eigenVectorNames = new string[Merge.Columns.Count];

                for (int i = 0; i < Merge.Columns.Count; i++)
                {
                    columnNames[i] = Merge.Columns[i].ColumnName.ToString();
                    eigenVectorNames[i] = "Eigenvector " + i.ToString();
                }

                DataTable data = EigenVectors.ToTable(eigenVectorNames);

                DataColumn rowNames = new DataColumn("RowNames", typeof(string));

                data.Columns.Add(rowNames.ColumnName, typeof(string));

                try
                { 
                    for (int i = 0; i < columnNames.Length; i++)
                    {
                        data.Rows[i][rowNames.ColumnName] = columnNames[i].ToString();
                    }

                }
                catch
                {

                }

                return data;
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
                NotifyOfPropertyChange(() => EigenValuesView);
                NotifyOfPropertyChange(() => EigenValuesVarianceView);
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[] eigenValuesVariance;
        public double[] EigenValuesVariance
        {
            get => this.eigenValuesVariance;
            set
            {
                this.eigenValuesVariance = value;
                NotifyOfPropertyChange(() => EigenValuesVarianceView);
            }
        }

        /// <summary>
        /// Bar chart for the eigenvalues
        /// </summary>
        private BarChartViewModel eigenValueBarChart = new BarChartViewModel()
        {
            Barco = new BarChartObject()
            {
                BarType = BarTypeEnum.Vertical,
                ChartHeight = 200,
                ChartWidth = 200
            }
        };
        public BarChartViewModel EigenValueBarChart
        {
            get => this.eigenValueBarChart;
            set
            {
                this.eigenValueBarChart = value;
                NotifyOfPropertyChange(() => EigenValueBarChart);
            }
        }

        /// <summary>
        /// The view of the eigenvalues
        /// </summary>
        public BindableCollection<double> EigenValuesView
        {
            get
            {
                return new BindableCollection<double>(this.EigenValues.ToList());
            }
        }

        /// <summary>
        /// The view of the eigen vectors
        /// </summary>
        public BindableCollection<double> EigenValuesVarianceView
        {
            get
            {
                return new BindableCollection<double>(this.EigenValuesVariance.ToList());
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[,] featureVector;
        public double[,] FeatureVector
        {
            get => this.featureVector;
            set
            {
                this.featureVector = value;
                NotifyOfPropertyChange(() => FeatureVector);
            }
        }

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
        /// The view of the projected values
        /// </summary>
        public DataTable ProjectedValuesView
        {
            get
            {
                return ProjectedValues.ToTable();
            }
        }

        /// <summary>
        /// Eigenvalues of the singular values
        /// </summary>
        private double[,] covarianceMatrix;
        public double[,] CovarianceMatrix
        {
            get => this.covarianceMatrix;
            set
            {
                this.covarianceMatrix = value;
                NotifyOfPropertyChange(() => CovarianceMatrix);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc12 = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC12
        {
            get => pc12;
            set
            {
                this.pc12 = value;
                NotifyOfPropertyChange(() => PC12);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc12BiPlot = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC12BiPlot
        {
            get => pc12BiPlot;
            set
            {
                this.pc12BiPlot = value;
                NotifyOfPropertyChange(() => PC12BiPlot);
            }
        }

        /// <summary>
        /// Objects for the scatter chart
        /// </summary>
        private LineAndScatterChartViewModel pc13BiPlot = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel PC13BiPlot
        {
            get => pc13BiPlot;
            set
            {
                this.pc13BiPlot = value;
                NotifyOfPropertyChange(() => PC13BiPlot);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dt"></param>
        public PrincipalComponentHelper()
        {

        }

        #endregion

        /// <summary>
        /// Computing the PCA
        /// </summary>
        public async override Task Compute()
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

                //Normalizing the dataset
                NormalizeDataSet();

                switch (Method)
                {
                    case PrincipalComponentMethod.Eigendecomposition:

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
                        EigenValuesVariance = EigenValues.Select(x => x / EigenValues.Sum()).ToArray();
                        EigenVectors = pca.ComponentVectors.Transpose();
                        ProjectedValues = pca.Transform(CalculationDataSet.ToJagged());

                        try
                        {
                            List<Mesh> projectedData = new List<Mesh>();

                            int j = 0;

                            for (int i = 0; i < DataSet.Count(); i++)
                            {
                                Mesh mesh = new Mesh();
                                ObservableCollection<LocationTimeValue> locs = new ObservableCollection<LocationTimeValue>();
                                for (int k = 0; k < DataSet[i].Vertices.Count(); k++)
                                {
                                    LocationTimeValue loc = new LocationTimeValue(DataSet[i].Vertices[k]);
                                    for (int l = 0; l < DataSet[i].Properties.Count(); l++)
                                    {
                                        loc.Value[l] = ProjectedValues[k + j][l];
                                    }

                                    locs.Add(loc);
                                }

                                projectedData.Add(new Mesh() { Name = DataSet[i].Name, Vertices = locs, Properties = DataSet[i].Properties });
                                j += DataSet[i].Vertices.Count();
                            }

                            EigenValueBarChart.Barco.ShallRender = true;

                            List<string> eigenvalues = new List<string>();
                            List<string> eigenvectors = new List<string>();

                            for (int i = 0; i < EigenValues.Count(); i++)
                                eigenvalues.Add("Eigenvalue " + i);

                            EigenValueBarChart.Barco.XLabels.Clear();

                            foreach (string eig in eigenvalues)
                                EigenValueBarChart.Barco.XLabels.Add(new Label() { Text = eig });

                            ///Creating eigen value bar chart
                            EigenValueBarChart.Barco.YLabel.Text = "Loading";
                            EigenValueBarChart.Barco.Ymax = EigenValues.Max() + 2;
                            EigenValueBarChart.Barco.Xmax = EigenValues.Count() + 1;
                            EigenValueBarChart.Barco.CreateCategoricHistogram(eigenvalues.ToArray(), EigenValues);
                            EigenValueBarChart.Barco.XLabel.Text = "Eigenvalue#";
                            EigenValueBarChart.Barco.YLabel.Text = "Eigenvalue";

                            //Creating visualization of the projected values
                            PC12.Lco.ShallRender = true;
                            PC12.Lco.XLabel.Text = "PC1";
                            PC12.Lco.YLabel.Text = "PC2";
                            PC12.Lco.Xmin = -1;
                            PC12.Lco.Xmax = 1;
                            PC12.Lco.Ymin = -1;
                            PC12.Lco.Ymax = 1;
                            PC12.Lco.XProperty = SelectedPropertyEnum.Property1;
                            PC12.Lco.YProperty = SelectedPropertyEnum.Property2;
                            PC12.Lco.DataSet = new BindableCollection<Mesh>(projectedData);
                            PC12.Lco.CreateLineChart();

                            foreach (DataColumn column in dat.Columns)
                            {
                                eigenvectors.Add(column.ColumnName);
                            }

                            //Creating visualization of the eigenvectors
                            PC12BiPlot.Lco.ShallRender = true;
                            PC12BiPlot.Lco.XLabel.Text = "PC1";
                            PC12BiPlot.Lco.YLabel.Text = "PC2";
                            PC12BiPlot.Lco.DataSet.Clear();

                            ////Adding a data series for each biplot member
                            //for (int i = 1; i < EigenVectorsView.Rows.Count; i++)
                            //{
                            //    Mesh biplot = new Mesh() { Name = columnNames[i] };
                            //    DataTable dt = EigenVectorsView.Clone();
                            //    dt.Rows.Add(EigenVectorsView.Rows[i].ItemArray);

                            //    for (int f = 0; f < dt.Columns.Count; f++)
                            //        dt.Rows[0][f] = 0;

                            //    dt.Rows.Add(EigenVectorsView.Rows[i].ItemArray);

                            //    biplot.Data = dt;

                            //    PC12BiPlot.Lco.DataSet.Add(biplot);
                            //    PC13BiPlot.Lco.DataSet.Add(biplot);
                            //}

                            //PC12BiPlot.Lco.CreateLineChart();
                            //PC13BiPlot.Lco.CreateLineChart();

                            //for(int i = 0; i< PC12BiPlot.Lco.DataCollection.Count(); i++)
                            //{
                            //    PC12BiPlot.Lco.DataCollection[i].Symbols.FillColor = ColorHelper.PickBrush();
                            //    PC13BiPlot.Lco.DataCollection[i].Symbols.FillColor = ColorHelper.PickBrush();
                            //}
                        }
                        catch
                        {
                            return;
                        }
                        break;
                }

            }
            catch
            {
                return;
            }
        }

    }
}
