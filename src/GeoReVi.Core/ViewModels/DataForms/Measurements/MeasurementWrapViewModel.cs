using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// View model that contains lab and field measurement forms
    /// </summary>
    public class MeasurementWrapViewModel : Screen
    {
        #region Public properties
        /// <summary>
        /// Checks whether a computation takes place ATM or not
        /// </summary>
        private bool isLoading = false;
        public bool IsLoading
        {
            get => this.isLoading;
            set
            {
                this.isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

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

        /// <summary>
        /// Filter criteria
        /// </summary>
        private FilterCriteria filterCriteria = new FilterCriteria();
        public FilterCriteria FilterCriteria
        {
            get => this.filterCriteria;
            set
            {
                this.filterCriteria = value;
                NotifyOfPropertyChange(() => FilterCriteria);
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

            await Task.WhenAll(ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsLoading, async () =>
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
                            FilterCriteria.GroupBy,
                            LabMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                            FilterCriteria.All,
                            FilterCriteria.Global,
                            FilterCriteria.FilterByDate ? FilterCriteria.From : null,
                            FilterCriteria.FilterByDate ? FilterCriteria.To : null,
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
                            FilterCriteria.GroupBy,
                             FieldMeasurementDetailsViewModel.SelectedProperty.alColumnName,
                             FilterCriteria.All,
                             FilterCriteria.Global,
                             FilterCriteria.FilterByDate ? FilterCriteria.From : null,
                             FilterCriteria.FilterByDate ? FilterCriteria.To : null,
                             FieldMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName != null ? FieldMeasurementDetailsViewModel.SelectedFilterProperty.alColumnName : "",
                             FieldMeasurementDetailsViewModel.FilterText));

                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            }));
        }
        #endregion
    }
}
