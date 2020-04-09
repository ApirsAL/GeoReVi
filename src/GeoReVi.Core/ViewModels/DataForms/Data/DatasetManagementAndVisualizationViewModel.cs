using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    public class DatasetManagementAndVisualizationViewModel : PropertyChangedBase
    {
        #region Public properties

        #region Chart view models

        /// <summary>
        /// Line and scatter chart object
        /// </summary>
        private LineAndScatterChartViewModel lineAndScatterChartViewModel = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel LineAndScatterChartViewModel
        {
            get => this.lineAndScatterChartViewModel;
            set
            {
                this.lineAndScatterChartViewModel = value;
                NotifyOfPropertyChange(() => LineAndScatterChartViewModel);
            }
        }


        /// <summary>
        /// Bar chart object
        /// </summary>
        private BarChartViewModel barChartViewModel = new BarChartViewModel();
        public BarChartViewModel BarChartViewModel
        {
            get => this.barChartViewModel;
            set
            {
                this.barChartViewModel = value;
                NotifyOfPropertyChange(() => BarChartViewModel);
            }
        }

        /// <summary>
        /// Box whisker chart object
        /// </summary>
        private BoxWhiskerChartViewModel boxWhiskerChartViewModel = new BoxWhiskerChartViewModel();
        public BoxWhiskerChartViewModel BoxWhiskerChartViewModel
        {
            get => this.boxWhiskerChartViewModel;
            set
            {
                this.boxWhiskerChartViewModel = value;
                NotifyOfPropertyChange(() => BoxWhiskerChartViewModel);
            }
        }

        /// <summary>
        /// Variogram chart object
        /// </summary>
        private VariogramChartViewModel variogramChartViewModel = new VariogramChartViewModel();
        public VariogramChartViewModel VariogramChartViewModel
        {
            get => this.variogramChartViewModel;
            set
            {
                this.variogramChartViewModel = value;
                NotifyOfPropertyChange(() => VariogramChartViewModel);
            }
        }

        /// <summary>
        /// Ternary chart object
        /// </summary>
        private TernaryChartViewModel ternaryChartViewModel = new TernaryChartViewModel();
        public TernaryChartViewModel TernaryChartViewModel
        {
            get => this.ternaryChartViewModel;
            set
            {
                this.ternaryChartViewModel = value;
                NotifyOfPropertyChange(() => TernaryChartViewModel);
            }
        }


        /// <summary>
        /// 3d chart object
        /// </summary>
        private HelixChart3DViewModel lineChart3DUnivariateViewModel = new HelixChart3DViewModel();
        public HelixChart3DViewModel LineChart3DUnivariateViewModel
        {
            get => this.lineChart3DUnivariateViewModel;
            set
            {
                this.lineChart3DUnivariateViewModel = value;
                NotifyOfPropertyChange(() => LineChart3DUnivariateViewModel);
            }
        }

        /// <summary>
        /// Line chart view model
        /// </summary>
        private LineAndScatterChartViewModel selectedLineChartViewModel = new LineAndScatterChartViewModel();
        public LineAndScatterChartViewModel SelectedLineChartViewModel
        {
            get => selectedLineChartViewModel;
            set
            {
                this.selectedLineChartViewModel = value;
                NotifyOfPropertyChange(() => SelectedLineChartViewModel);
            }
        }

        /// <summary>
        /// A view model that holds an uni-parametric data set of a laboratory parameter
        /// </summary>
        private LoadParameterDataViewModel singleParameterViewModel = new LoadParameterDataViewModel();
        public LoadParameterDataViewModel SingleParameterViewModel
        {
            get => this.singleParameterViewModel;
            set
            {
                this.singleParameterViewModel = value;
                NotifyOfPropertyChange(() => SingleParameterViewModel);
            }
        }

        /// <summary>
        /// A view model that holds a multi parametric data set of the laboratory MultiParameterViewModel.MeasPoints
        /// </summary>
        private MultivariateDataViewModel multiParameterViewModel = new MultivariateDataViewModel();
        public MultivariateDataViewModel MultiParameterViewModel
        {
            get => this.multiParameterViewModel;
            set
            {
                this.multiParameterViewModel = value;
                NotifyOfPropertyChange(() => MultiParameterViewModel);
            }
        }

        /// <summary>
        /// A correlation analysis view model
        /// </summary>
        private CorrelationHelperViewModel correlationHelperViewModel = new CorrelationHelperViewModel();
        public CorrelationHelperViewModel CorrelationHelperViewModel
        {
            get => this.correlationHelperViewModel;
            set
            {
                this.correlationHelperViewModel = value;
                NotifyOfPropertyChange(() => CorrelationHelperViewModel);
            }
        }

        /// <summary>
        /// A PCA view model
        /// </summary>
        private PrincipalComponentAnalysisViewModel principalComponentAnalysisViewModel = new PrincipalComponentAnalysisViewModel();
        public PrincipalComponentAnalysisViewModel PrincipalComponentAnalysisViewModel
        {
            get => this.principalComponentAnalysisViewModel;
            set
            {
                this.principalComponentAnalysisViewModel = value;
                NotifyOfPropertyChange(() => PrincipalComponentAnalysisViewModel);
            }
        }

        /// <summary>
        /// A Sammon projection view model
        /// </summary>
        private SammonProjectionViewModel sammonProjectionViewModel = new SammonProjectionViewModel();
        public SammonProjectionViewModel SammonProjectionViewModel
        {
            get => this.sammonProjectionViewModel;
            set
            {
                this.sammonProjectionViewModel = value;
                NotifyOfPropertyChange(() => SammonProjectionViewModel);
            }
        }

        /// <summary>
        /// A cluster analysis object
        /// </summary>
        private ClusteringViewModel clusteringViewModel = new ClusteringViewModel();
        public ClusteringViewModel ClusteringViewModel
        {
            get => this.clusteringViewModel;
            set
            {
                this.clusteringViewModel = value;
                NotifyOfPropertyChange(() => ClusteringViewModel);
            }
        }

        /// <summary>
        /// An analytical hierarchy process object
        /// </summary>
        private AnalyticalHierarchyProcessViewModel analyticalHierarchyProcessViewModel = new AnalyticalHierarchyProcessViewModel();
        public AnalyticalHierarchyProcessViewModel AnalyticalHierarchyProcessViewModel
        {
            get => this.analyticalHierarchyProcessViewModel;
            set
            {
                this.analyticalHierarchyProcessViewModel = value;
                NotifyOfPropertyChange(() => AnalyticalHierarchyProcessViewModel);
            }
        }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DatasetManagementAndVisualizationViewModel()
        {
            SingleParameterViewModel.SpatialInterpolationHelper.Vh = VariogramChartViewModel.Vco.Vh;
        }


        #endregion

        #region Methods


        /// <summary>
        /// Adds the 
        /// </summary>
        public async void AddClustersToDataSet()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    foreach (var list in ClusteringViewModel.ClusteringHelper.ClusteredDatasetView)
                    {
                        MultiParameterViewModel.MeasPoints.Add(new Mesh() { Name = list.Key, Data = list.Value });
                    }

                    if (MultiParameterViewModel.MeasPoints.Count > 0)
                    {
                        MultiParameterViewModel.SetDataTableNames();
                    }
                    else
                    {
                        MultiParameterViewModel.DataTableColumnNames = new BindableCollection<string>();
                    }

                }
                catch (Exception e)
                {
                    return;
                }
            });
        }

        /// <summary>
        /// Adds data to the cluster window
        /// </summary>
        public void AddToCluster()
        {
            try
            {
                MultiParameterViewModel.SelectedMeasPoint.Data.AcceptChanges();
            }
            catch
            {

            }

            try
            {
                ClusteringViewModel.ClusteringHelper.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Adding a 3D chart to the 3D chart control
        /// </summary>
        public void AddTo3DChart()
        {
            try
            {
                LineChart3DUnivariateViewModel.Ch3d.DataSet.Add(SingleParameterViewModel.SelectedMeasPoint);
                LineChart3DUnivariateViewModel.Ch3d.AddDataToSeries();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating the viewmodel for principal component analysis
        /// </summary>
        public void AddToPca()
        {
            try
            {
                MultiParameterViewModel.SelectedMeasPoint.Data.AcceptChanges();
            }
            catch
            {

            }

            try
            {
                PrincipalComponentAnalysisViewModel.PrincipalComponentHelper.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating the viewmodel for principal component analysis
        /// </summary>
        public void AddToAHP()
        {
            try
            {
                MultiParameterViewModel.SelectedMeasPoint.Data.AcceptChanges();
            }
            catch
            {

            }

            try
            {
                PrincipalComponentAnalysisViewModel.PrincipalComponentHelper.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating the viewmodel
        /// </summary>
        public void AddToSammon()
        {
            try
            {
                MultiParameterViewModel.SelectedMeasPoint.Data.AcceptChanges();
            }
            catch
            {

            }

            try
            {
                SammonProjectionViewModel.SammonProjectionHelper.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }



        /// <summary>
        /// Creating a varioram chart
        /// </summary>
        public void AddToVariogramChart()
        {
            try
            {
                VariogramChartViewModel.Vco.DataSet.Add(SingleParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating a Box-Whisker-Plot
        /// </summary>
        public void AddToLineChart()
        {
            try
            {
                LineAndScatterChartViewModel.Lco.DataSet.Add(SingleParameterViewModel.SelectedMeasPoint);
                LineAndScatterChartViewModel.Lco.CreateLineChart();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating a ternary chart
        /// </summary>
        public void CreateTernaryChart()
        {
            try
            {
                //Clearing the data collection
                TernaryChartViewModel.Tco.DataSet.Clear();

                for (int i = 0; i < MultiParameterViewModel.MeasPoints.Count(); i++)
                {
                    DataTable dt = new DataTable();
                    DataColumn dc0 = MultiParameterViewModel.MeasPoints[i].Data.Columns[MultiParameterViewModel.SelectedColumn[0]];
                    DataColumn dc1 = MultiParameterViewModel.MeasPoints[i].Data.Columns[MultiParameterViewModel.SelectedColumn[1]];
                    DataColumn dc2 = MultiParameterViewModel.MeasPoints[i].Data.Columns[MultiParameterViewModel.SelectedColumn[2]];

                    dt.Columns.Add("dummy", dc0.DataType);
                    dt.Columns.Add(dc0.ColumnName, dc0.DataType);
                    dt.Columns.Add(dc1.ColumnName, dc1.DataType);
                    dt.Columns.Add(dc2.ColumnName, dc2.DataType);

                    for (int j = 0; j < MultiParameterViewModel.MeasPoints[i].Data.Rows.Count; j++)
                    {
                        dt.Rows.Add(
                            MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc0.ColumnName],
                            MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc0.ColumnName],
                            MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc1.ColumnName],
                            MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc2.ColumnName]);
                    }

                    MultiParameterViewModel.MeasPoints[i].Data.Clone();

                    Mesh mesh = new Mesh()
                    {
                        Data = dt
                    };

                    TernaryChartViewModel.Tco.DataSet.Add(mesh);
                }

                TernaryChartViewModel.Tco.CreateTernaryChart();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating a histogram chart
        /// </summary>
        public void AddToBarChart()
        {
            try
            {
                BarChartViewModel.Barco.DataSet.Add(SingleParameterViewModel.SelectedMeasPoint);
                BarChartViewModel.Barco.CreateHistogram();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating a Box-Whisker-Plot
        /// </summary>
        public void AddToBoxWhiskerChart()
        {
            try
            {
                BoxWhiskerChartViewModel.Bco.DataSet.Add(SingleParameterViewModel.SelectedMeasPoint);
                BoxWhiskerChartViewModel.Bco.CreateChart();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Creating a scatter chart
        /// </summary>
        public void CreateScatterCharts()
        {
            try
            {
                //Clearing the data collection
                SelectedLineChartViewModel.Lco.Direction = DirectionEnum.Directionless;

                SelectedLineChartViewModel.Lco.DataSet.Clear();

                for (int i = 0; i<MultiParameterViewModel.MeasPoints.Count();i++)
                {
                    DataTable dt = new DataTable();
                    DataColumn dc0 = MultiParameterViewModel.MeasPoints[i].Data.Columns[MultiParameterViewModel.SelectedColumn[0]];
                    DataColumn dc1 = MultiParameterViewModel.MeasPoints[i].Data.Columns[MultiParameterViewModel.SelectedColumn[1]];

                    dt.Columns.Add(dc0.ColumnName, dc0.DataType);
                    dt.Columns.Add(dc1.ColumnName, dc1.DataType);
                    dt.Columns.Add("dummy1", dc1.DataType);
                    dt.Columns.Add("dummy2", dc1.DataType);

                    SelectedLineChartViewModel.Lco.YLabel.Text = dc0.ColumnName;
                    SelectedLineChartViewModel.Lco.XLabel.Text = dc1.ColumnName;

                    for ( int j = 0; j< MultiParameterViewModel.MeasPoints[i].Data.Rows.Count; j++)
                    {
                        try
                        {
                            dt.Rows.Add(
                                MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc0.ColumnName],
                                MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc1.ColumnName],
                                MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc1.ColumnName],
                                MultiParameterViewModel.MeasPoints[i].Data.Rows[j][dc1.ColumnName]);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    MultiParameterViewModel.MeasPoints[i].Data.Clone();

                    Mesh mesh = new Mesh()
                    {
                        Data = dt
                    };

                    SelectedLineChartViewModel.Lco.DataSet.Add(mesh);
                }

                SelectedLineChartViewModel.Lco.CreateLineChart();
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Assigning and transforming the data to a data table and instantiating a CorrelationHelper for each table
        /// </summary>
        public void AddToCorrelation()
        {
            try
            {
                try
                {
                    MultiParameterViewModel.SelectedMeasPoint.Data.AcceptChanges();
                }
                catch
                {

                }

                CorrelationHelperViewModel.CorrelationHelper.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
            }
            catch
            {
                return;
            }
        }

        #endregion

    }
}
