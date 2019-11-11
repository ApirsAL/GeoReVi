using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// View model that contains lab and field measurement forms
    /// </summary>
    public class MeasurementWrapViewModel : Screen
    {
        #region Public properties

        /// <summary>
        /// View model for lab measurements
        /// </summary>
        private LabMeasurementDetailsViewModel labMeasurementDetailsViewModel;
        public LabMeasurementDetailsViewModel LabMeasurementDetailsViewModel
        {
            get => this.labMeasurementDetailsViewModel;
            set
            {
                this.labMeasurementDetailsViewModel = value;
                NotifyOfPropertyChange(() => LabMeasurementDetailsViewModel);
            }
        }

        /// <summary>
        /// View model for field measurements
        /// </summary>
        private FieldMeasurementDetailsViewModel fieldMeasurementDetailsViewModel;
        public FieldMeasurementDetailsViewModel FieldMeasurementDetailsViewModel
        {
            get => this.fieldMeasurementDetailsViewModel;
            set
            {
                this.fieldMeasurementDetailsViewModel = value;
                NotifyOfPropertyChange(() => FieldMeasurementDetailsViewModel);
            }
        }

        /// <summary>
        /// View model for data management and visualization
        /// </summary>
        private DatasetManagementAndVisualizationViewModel datasetManagementAndVisualizationViewModel = new DatasetManagementAndVisualizationViewModel();
        public DatasetManagementAndVisualizationViewModel DatasetManagementAndVisualizationViewModel
        {
            get => this.datasetManagementAndVisualizationViewModel;
            set
            {
                this.datasetManagementAndVisualizationViewModel = value;
                NotifyOfPropertyChange(() => DatasetManagementAndVisualizationViewModel);
            }
        }

        /// <summary>
        /// The selected index
        /// </summary>
        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                this.selectedIndex = value;
                NotifyOfPropertyChange(() => SelectedIndex);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MeasurementWrapViewModel(IEventAggregator events)
        {
            LabMeasurementDetailsViewModel = new LabMeasurementDetailsViewModel(events);
            FieldMeasurementDetailsViewModel = new FieldMeasurementDetailsViewModel(events);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loading the single parameter data matrix for the selected measurement
        /// </summary>
        public async void LoadSingleParameterDataMatrix()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    ///Loads a univariate data set from the actually selected laboratory measurement
                    if (SelectedIndex == 0)
                    {
                        DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblRockSample>().GetLaboratoryMeasurementPoints(
                            LabMeasurementDetailsViewModel.RockSamples,
                            LabMeasurementDetailsViewModel.SelectedRockSample,
                            LabMeasurementDetailsViewModel.SelectedLaboratoryMeasurement,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.GroupBy,
                            LabMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.All,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.Global,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.From : null,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.To : null,
                            LabMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName != null ? LabMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName : "",
                            LabMeasurementDetailsViewModel.FilterText));

                    }
                    else
                    {
                        DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblMeasurement>()
                            .GetFieldMeasurementPoints(
                            FieldMeasurementDetailsViewModel.FieldMeasurements,
                            FieldMeasurementDetailsViewModel.SelectedFieldMeasurement,
                            FieldMeasurementDetailsViewModel.ObjectsOfInvestigation,
                            DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.GroupBy,
                             FieldMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                             DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.All,
                             DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.Global,
                             DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.From : null,
                             DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.To : null,
                             FieldMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName != null ? FieldMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName : "",
                             FieldMeasurementDetailsViewModel.FilterText));

                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            });
        }

        /// <summary>
        /// Loading the multiparametric data table from the database
        /// </summary>
        public async void LoadMultiparametricDataMatrix()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {


                try
                {

                    if (SelectedIndex == 0)
                        DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblRockSample>().GetLaboratoryPetrophysics(
                            LabMeasurementDetailsViewModel.RockSamples,
                            LabMeasurementDetailsViewModel.SelectedRockSample,
                            DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.GroupBy,
                            DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.All));
                    else
                        DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblMeasurement>().GetFieldPetrophysics(
                            FieldMeasurementDetailsViewModel.FieldMeasurements, 
                            FieldMeasurementDetailsViewModel.SelectedFieldMeasurement, 
                            FieldMeasurementDetailsViewModel.GroupBy,
                            FieldMeasurementDetailsViewModel.All));



                    if (DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.Count > 0)
                    {
                        DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.SetDataTableNames().AsResult();


                        DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.SelectedColumn.Add(DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.DataTableColumnNames[0]);
                    }
                    else
                    {
                        DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.DataTableColumnNames = new BindableCollection<string>();
                    }
                }
                catch
                {
                    return;
                }
            });
        }

        #endregion
    }
}
