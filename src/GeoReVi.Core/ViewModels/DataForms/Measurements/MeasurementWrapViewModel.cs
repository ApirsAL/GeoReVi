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
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblRockSample>().GetLaboratoryMeasurementPoints(
                            LabMeasurementDetailsViewModel.RockSamples,
                            LabMeasurementDetailsViewModel.SelectedRockSample,
                            LabMeasurementDetailsViewModel.SelectedLaboratoryMeasurement,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.GroupBy,
                            LabMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.All,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.Global,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.From : null,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.To : null,
                            LabMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName != null ? LabMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName : "",
                            LabMeasurementDetailsViewModel.FilterText));

                    }
                    else
                    {
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblMeasurement>()
                            .GetFieldMeasurementPoints(
                            FieldMeasurementDetailsViewModel.FieldMeasurements,
                            FieldMeasurementDetailsViewModel.SelectedFieldMeasurement,
                            FieldMeasurementDetailsViewModel.ObjectsOfInvestigation,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.GroupBy,
                             FieldMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                             ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.All,
                             ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.Global,
                             ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.From : null,
                             ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.FilterByDate ? ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.SingleParameterViewModel.To : null,
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
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblRockSample>().GetLaboratoryPetrophysics(
                            LabMeasurementDetailsViewModel.RockSamples,
                            LabMeasurementDetailsViewModel.SelectedRockSample,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.GroupBy,
                            ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.All));
                    else
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.AddRange(new ApirsRepository<tblMeasurement>().GetFieldPetrophysics(
                            FieldMeasurementDetailsViewModel.FieldMeasurements, 
                            FieldMeasurementDetailsViewModel.SelectedFieldMeasurement, 
                            FieldMeasurementDetailsViewModel.GroupBy,
                            FieldMeasurementDetailsViewModel.All));



                    if (((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.MeasPoints.Count > 0)
                    {
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.SetDataTableNames().AsResult();


                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.SelectedColumn.Add(((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.DataTableColumnNames[0]);
                    }
                    else
                    {
                        ((ShellViewModel)IoC.Get<IShell>(null)).DatasetManagementAndVisualizationViewModel.MultiParameterViewModel.DataTableColumnNames = new BindableCollection<string>();
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
