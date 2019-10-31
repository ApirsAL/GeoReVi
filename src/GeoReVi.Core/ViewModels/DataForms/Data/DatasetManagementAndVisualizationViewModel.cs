using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
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

                        SelectedLineChartViewModel.Lco.DataTableColumnNames = MultiParameterViewModel.DataTableColumnNames;
                        TernaryChartViewModel.Tco.DataTableColumnNames = MultiParameterViewModel.DataTableColumnNames;

                        TernaryChartViewModel.Tco.ColumnList.Clear();
                        SelectedLineChartViewModel.Lco.ColumnList.Clear();

                        MultiParameterViewModel.SelectedColumn.Add(MultiParameterViewModel.DataTableColumnNames[0]);
                        TernaryChartViewModel.Tco.ColumnList.AddRange(TernaryChartViewModel.Tco.DataTableColumnNames.Take(3));
                        SelectedLineChartViewModel.Lco.ColumnList.AddRange(SelectedLineChartViewModel.Lco.DataTableColumnNames.Take(2));
                    }
                    else
                    {
                        MultiParameterViewModel.DataTableColumnNames = new BindableCollection<string>();
                        SelectedLineChartViewModel.Lco.DataTableColumnNames = new ObservableCollection<string>();
                        TernaryChartViewModel.Tco.DataTableColumnNames = new ObservableCollection<string>();
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
                LineAndScatterChartViewModel.Lco.CreateLineChart("","");
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
                TernaryChartViewModel.Tco.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
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
        /// Creating a set of scatter charts
        /// </summary>
        public void CreateScatterCharts()
        {
            try
            {
                SelectedLineChartViewModel.Lco.DataSet.Add(MultiParameterViewModel.SelectedMeasPoint);
                SelectedLineChartViewModel.Lco.CreateScatterChart();
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
