using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// A view model for lithological section visualization
    /// </summary>
    public class SectionChartViewModel : Screen
    {
        #region Public properties

        /// <summary>
        /// chart object for the litho log
        /// </summary>
        private LithoLogChartObject liLo = new LithoLogChartObject();
        public LithoLogChartObject LiLo
        {
            get => this.liLo;
            set
            {
                this.liLo = value;
                NotifyOfPropertyChange(() => LiLo);
            }
        }

        /// <summary>
        /// The line chart logs that will be plotted next to the lithological sections
        /// </summary>
        private BindableCollection<LineChartViewModel> logs = new BindableCollection<LineChartViewModel>();
        public BindableCollection<LineChartViewModel> Logs
        {
            get => this.logs;
            set
            {
                this.logs = value;
                NotifyOfPropertyChange(() => Logs);
            }
        }

        public string Scale { get => "1 m = " + Environment.NewLine + Math.Round(ChartHeight / (LiLo.Ymax - LiLo.Ymin), 2).ToString() + " px"; set { NotifyOfPropertyChange(() => Scale); } }

        /// <summary>
        /// A view model that holds a multi parametric data set of the laboratory MultiParameterViewModel.MeasPoints
        /// </summary>
        private LoadParameterDataViewModel laboratoryParameterViewModel = new LoadParameterDataViewModel(ParameterClass.LaboratoryMeasurements);
        public LoadParameterDataViewModel LaboratoryParameterViewModel
        {
            get => this.laboratoryParameterViewModel;
            set
            {
                this.laboratoryParameterViewModel = value;
                NotifyOfPropertyChange(() => LaboratoryParameterViewModel);
            }
        }

        /// <summary>
        /// Height of the chart
        /// </summary>
        private double chartHeight = 500;

        public double ChartHeight
        {
            get
            {
                return this.chartHeight;
            }
            set { this.chartHeight = value;
                NotifyOfPropertyChange(() => ChartHeight);
                NotifyOfPropertyChange(() => Scale); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        [ImportingConstructor]
        public SectionChartViewModel()
        {

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Loading the Laboratory data table from the database
        /// </summary>
        public async void LoadLaboratoryParameterDataMatrix(tblSection section)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    List<tblRockSample> rockSamples = new ApirsRepository<tblRockSample>().GetModelByExpression(x => x.sampooiName == section.secOoiName
                                                                                                                    && x.sampLatitude == section.secLatitude
                                                                                                                    && x.sampLongitude == section.secLongitude
                                                                                                                    && x.sampprjIdFk == section.secprjIdFk
                                                                                                                    ).ToList();

                    LaboratoryParameterViewModel.MeasPoints = new BindableCollection<KeyValuePair<string, DataTable>>(new ApirsRepository<tblRockSample>().GetLaboratoryPetrophysics(rockSamples, rockSamples.FirstOrDefault()).ToList());

                    DataTable dt = LaboratoryParameterViewModel.MeasPoints[0].Value.Clone();
                    dt.RemoveNonNumericColumns();

                    if (LaboratoryParameterViewModel.MeasPoints.Count > 0)
                    {
                        LaboratoryParameterViewModel.DataTableColumnNames = new BindableCollection<string>(dt.Columns.Cast<DataColumn>().Where(x => x.DataType == typeof(double))
                                 .Select(x => x.ColumnName).ToList());
                    }
                    else
                    {
                        LaboratoryParameterViewModel.DataTableColumnNames = new BindableCollection<string>();
                    }
                }
                catch
                {

                }
            });
        }

        #endregion
    }
}
