using Caliburn.Micro;
using System.Windows;
using System.Linq;
using System.Linq.Dynamic;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.IO;
using System.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MoreLinq;

namespace GeoReVi
{
    /// <summary>
    /// View model for a single data entry form for rock samples
    /// </summary>
    public class LabMeasurementDetailsViewModel : PropertyChangedBase
    {
        #region Private members

        //Rock sample collection
        private BindableCollection<tblRockSample> rockSamples;
        //Rock sample collection
        private BindableCollection<tblRockSample> allRockSamples;

        //Rock sample collection
        private BindableCollection<tblMeasurement> laboratoryMeasurements;
        //Petrography colleciton
        private BindableCollection<tblUnionPetrography> unionPetrography;
        //Facies collection
        private BindableCollection<tblFacy> facies;
        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;
        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;
        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> objectOfInvestigation;
        //Analytical instruments collection
        private BindableCollection<tblMeasuringDevice> analyticalInstrument;

        /// <summary>
        /// Selected objects
        /// </summary>
        private tblRockSample selectedRockSample;
        private tblMeasurement selectedLaboratoryMeasurement;

        //Subproperties
        private tblAxialCompression axialCompression;
        private tblGrainDensity grainDensity;
        private tblGrainSize grainSize;
        private tblBulkDensity bulkDensity;
        private tblEffectivePorosity porosity;
        private tblHydraulicHead hydraulicHead;
        private tblApparentPermeability apparentPermeability;
        private tblIntrinsicPermeability intrinsicPermeability;
        private tblIsotopes isotope;
        private tblThermalConductivity thermalConductivity;
        private tblThermalDiffusivity thermalDiffusivity;
        private tblResistivity resistivity;
        private tblSonicWave sonicWaveVelocity;
        private tblXRayFluorescenceSpectroscopy xrf;

        /// <summary>
        /// Selected laboratory measurements
        /// </summary>
        //Pictures
        private BindableCollection<v_FileStore> fileStore;
        private v_FileStore selectedFileStore;

        //checks if File is loading
        private bool isFileLoading = false;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Cancellation token source for cancelling File downloads
        /// </summary>
        private CancellationTokenSource cts;

        //Location of the rock sample
        private Location center;

        private string type;

        #endregion

        #region Public properties

        public string Type
        {
            get => this.type;
            set
            {
                this.type = value; if (value != null) LoadData(value);
                NotifyOfPropertyChange(() => Type);
            }
        }

        /// <summary>
        /// Property which checks if File is loading
        /// </summary>
        public bool IsFileLoading
        {
            get { return this.isFileLoading; }
            set { this.isFileLoading = value; NotifyOfPropertyChange(() => IsFileLoading); }
        }

        /// <summary>
        /// The selected rock sample
        /// </summary>
        public tblRockSample SelectedRockSample
        {
            get { return this.selectedRockSample; }
            set
            {
                this.selectedRockSample = value;

                try
                {
                    Center = new Location((double)SelectedRockSample.sampLatitude, (double)SelectedRockSample.sampLongitude);
                }
                catch
                {
                    Center = new Location(0, 0);
                }

                if (value != null)
                {
                    Initialization = OnSelectedRockSampleChanged();
                }


                NotifyOfPropertyChange(() => SelectedRockSample);
                NotifyOfPropertyChange(() => SelectedRockSampleIndex);
            }
        }

        /// <summary>
        /// The selected laboratory measurement
        /// </summary>
        public tblMeasurement SelectedLaboratoryMeasurement
        {
            get { return this.selectedLaboratoryMeasurement; }
            set
            {
                try
                {
                    if (SelectedLaboratoryMeasurement.measIdPk != 0)
                        if (DataValidation.CheckPrerequisites(CRUD.Update, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                        {
                            Update();
                        }
                }
                catch
                {
                }

                this.selectedLaboratoryMeasurement = value;

                if (value != null && value.measIdPk != 0)
                {
                    OnSelectedLaboratoryMeasurementChanged();
                }

                NotifyOfPropertyChange(() => IsAxialCompression);
                NotifyOfPropertyChange(() => IsGrainSize);
                NotifyOfPropertyChange(() => IsGrainDensity);
                NotifyOfPropertyChange(() => IsBulkDensity);
                NotifyOfPropertyChange(() => IsPorosity);
                NotifyOfPropertyChange(() => IsApparentPermeability);
                NotifyOfPropertyChange(() => IsHydraulicHead);
                NotifyOfPropertyChange(() => IsIntrinsicPermeability);
                NotifyOfPropertyChange(() => IsIsotope);
                NotifyOfPropertyChange(() => IsThermalConductivity);
                NotifyOfPropertyChange(() => IsThermalDiffusivity);
                NotifyOfPropertyChange(() => IsResistivity);
                NotifyOfPropertyChange(() => IsSonicWaveVelocity);
                NotifyOfPropertyChange(() => IsXRF);
                NotifyOfPropertyChange(() => SelectedLaboratoryMeasurement);
            }
        }

        /// <summary>
        /// All selected laboratory measurements
        /// </summary>
        private BindableCollection<tblMeasurement> selectedLaboratoryMeasurements = new BindableCollection<tblMeasurement>();
        public BindableCollection<tblMeasurement> SelectedLaboratoryMeasurements
        {
            get => this.selectedLaboratoryMeasurements;
            set
            {
                this.selectedLaboratoryMeasurements = value;
                NotifyOfPropertyChange(() => SelectedLaboratoryMeasurements);
            }
        }


        //Entities
        public tblAxialCompression AxialCompression { get { return this.axialCompression; } set { this.axialCompression = value; NotifyOfPropertyChange(() => AxialCompression); } }
        public tblGrainDensity GrainDensity { get { return this.grainDensity; } set { this.grainDensity = value; NotifyOfPropertyChange(() => GrainDensity); } }
        public tblGrainSize GrainSize { get { return this.grainSize; } set { this.grainSize = value; NotifyOfPropertyChange(() => GrainSize); } }
        public tblBulkDensity BulkDensity { get { return this.bulkDensity; } set { this.bulkDensity = value; NotifyOfPropertyChange(() => BulkDensity); } }
        public tblEffectivePorosity Porosity { get { return this.porosity; } set { this.porosity = value; NotifyOfPropertyChange(() => Porosity); } }
        public tblHydraulicHead HydraulicHead { get { return this.hydraulicHead; } set { this.hydraulicHead = value; NotifyOfPropertyChange(() => HydraulicHead); } }
        public tblApparentPermeability ApparentPermeability { get { return this.apparentPermeability; } set { this.apparentPermeability = value; NotifyOfPropertyChange(() => ApparentPermeability); } }
        public tblIntrinsicPermeability IntrinsicPermeability { get { return this.intrinsicPermeability; } set { this.intrinsicPermeability = value; NotifyOfPropertyChange(() => IntrinsicPermeability); } }
        public tblIsotopes Isotope { get { return this.isotope; } set { this.isotope = value; NotifyOfPropertyChange(() => Isotope); } }
        public tblThermalConductivity ThermalConductivity { get { return this.thermalConductivity; } set { this.thermalConductivity = value; NotifyOfPropertyChange(() => ThermalConductivity); } }
        public tblThermalDiffusivity ThermalDiffusivity { get { return this.thermalDiffusivity; } set { this.thermalDiffusivity = value; NotifyOfPropertyChange(() => ThermalDiffusivity); } }
        public tblResistivity Resistivity { get { return this.resistivity; } set { this.resistivity = value; NotifyOfPropertyChange(() => Resistivity); } }
        public tblSonicWave SonicWaveVelocity { get { return this.sonicWaveVelocity; } set { this.sonicWaveVelocity = value; NotifyOfPropertyChange(() => SonicWaveVelocity); } }
        public tblXRayFluorescenceSpectroscopy Xrf { get { return this.xrf; } set { this.xrf = value; NotifyOfPropertyChange(() => Xrf); } }

        /// <summary>
        /// count of the objects
        /// </summary>
        public string CountRockSamples
        {
            get
            {
                if (RockSamples != null)
                    return RockSamples.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountRockSamples);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedRockSampleIndex
        {
            get
            {
                if (SelectedRockSample != null)
                    return (RockSamples.IndexOf(SelectedRockSample) + 1).ToString();

                return "0";
            }
            set
            {
                OnSelectedRockSampleIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedRockSampleIndex);
            }
        }

        //All pictures related to a certain rock sample
        public BindableCollection<v_FileStore> FileStore
        {
            get
            {
                return this.fileStore;
            }
            set
            {
                this.fileStore = value;
                NotifyOfPropertyChange(() => FileStore);
                NotifyOfPropertyChange(() => CountFilesLaboratoryMeasurement);
            }
        }

        //The selected File file
        public v_FileStore SelectedFileStore
        {
            get
            {
                return this.selectedFileStore;
            }
            set
            {
                this.selectedFileStore = value;
                NotifyOfPropertyChange(() => SelectedFileRockSampleIndex);
                NotifyOfPropertyChange(() => SelectedFileStore);
            }
        }

        /// <summary>
        /// Readonly count of the objects
        /// </summary>
        public string CountFilesLaboratoryMeasurement
        {
            get
            {
                if (FileStore != null)
                    return FileStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountFilesLaboratoryMeasurement);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedFileRockSampleIndex
        {
            get
            {
                if (SelectedFileStore != null)
                    return (FileStore.IndexOf(SelectedFileStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedFileRockSampleIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedFileRockSampleIndex);
            }
        }

        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<tblRockSample> RockSamples
        {
            get { return this.rockSamples; }
            set
            {
                this.rockSamples = value;
                NotifyOfPropertyChange(() => CountRockSamples);
                NotifyOfPropertyChange(() => RockSamples);
            }
        }

        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<tblMeasurement> LaboratoryMeasurements
        {
            get { return this.laboratoryMeasurements; }
            set
            {
                this.laboratoryMeasurements = value;
                NotifyOfPropertyChange(() => LaboratoryMeasurements);
            }
        }

        //All petrographic terms
        public BindableCollection<tblUnionPetrography> UnionPetrography
        {
            get { return this.unionPetrography; }
            set
            {
                this.unionPetrography = value;
                NotifyOfPropertyChange(() => UnionPetrography);
            }
        }

        //All Analytical devices
        public BindableCollection<tblMeasuringDevice> AnalyticalInstrument
        {
            get { return this.analyticalInstrument; }
            set
            {
                this.analyticalInstrument = value;
                NotifyOfPropertyChange(() => AnalyticalInstrument);
            }
        }

        /// <summary>
        /// Collection of all documented facies types
        /// </summary>
        public BindableCollection<tblFacy> Facies
        {
            get { return this.facies; }
            set
            {
                this.facies = value;
                NotifyOfPropertyChange(() => Facies);
            }
        }

        /// <summary>
        /// Collection of all chronostratigraphic units
        /// </summary>
        public BindableCollection<tblUnionChronostratigraphy> Chronostratigraphy
        {
            get { return this.chronostratigraphy; }
            set
            {
                this.chronostratigraphy = value;
                NotifyOfPropertyChange(() => Chronostratigraphy);
            }
        }

        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<LithostratigraphyUnion> Lithostratigraphy
        {
            get { return this.lithostratigraphy; }
            set
            {
                this.lithostratigraphy = value;
                NotifyOfPropertyChange(() => Lithostratigraphy);
            }
        }

        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<tblObjectOfInvestigation> ObjectsOfInvestigation
        {
            get { return this.objectOfInvestigation; }
            set
            {
                this.objectOfInvestigation = value;
                NotifyOfPropertyChange(() => ObjectsOfInvestigation);
            }
        }

        //The selected project
        public tblProject SelectedProject
        {
            get
            {
                if ((tblProject)((ShellViewModel)IoC.Get<IShell>()).SelectedProject != null)
                    return (tblProject)(tblProject)((ShellViewModel)IoC.Get<IShell>()).SideMenuViewModel.SelectedProject;

                return new tblProject();
            }
        }

        /// <summary>
        /// All projects, the user actually participates
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get
            {
                if ((BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).Projects != null)
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SideMenuViewModel.SelectedProjects;
                return new BindableCollection<tblProject>();
            }
        }

        //Center property for a map
        public Location Center
        {
            get
            {
                return this.center;
            }
            set
            {
                this.center = value;
                NotifyOfPropertyChange(() => Center);
            }
        }

        /// <summary>
        /// The selected property that should be downloaded
        /// </summary>
        private tblAlia selectedProperty = new tblAlia();
        public tblAlia SelectedProperty
        {
            get { return this.selectedProperty; }
            set
            {
                this.selectedProperty = value;
                NotifyOfPropertyChange(() => SelectedProperty);
            }
        }

        /// <summary>
        /// Collection of all alias of the class properties
        /// </summary>
        private BindableCollection<tblAlia> properties = new BindableCollection<tblAlia>();
        public BindableCollection<tblAlia> Properties
        {
            get { return this.properties; }
            set
            {
                this.properties = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }

        /// <summary>
        /// The selected property that should be applied as filter when datasets are downloaded
        /// </summary>
        private tblAlia selectedFilterProperty = new tblAlia();
        public tblAlia SelectedFilterProperty
        {
            get { return this.selectedFilterProperty; }
            set
            {
                this.selectedFilterProperty = value;
                NotifyOfPropertyChange(() => SelectedFilterProperty);
            }
        }

        /// <summary>
        /// Collection of all alias of the class properties for filter purposes
        /// </summary>
        private BindableCollection<tblAlia> filterProperties = new BindableCollection<tblAlia>();
        public BindableCollection<tblAlia> FilterProperties
        {
            get { return this.filterProperties; }
            set
            {
                this.filterProperties = value;
                NotifyOfPropertyChange(() => FilterProperties);
            }
        }

        /// <summary>
        /// Filter text
        /// </summary>
        private string filterText = "";
        public string FilterText
        {
            get { return this.filterText; }
            set
            {
                this.filterText = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }

        //Visibility members dependent on the selected lab measurement
        public bool IsAxialCompression { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Axial compression"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsAxialCompression); } }
        public bool IsGrainSize { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Grain size"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsGrainSize); } }
        public bool IsGrainDensity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Grain density"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsGrainDensity); } }
        public bool IsBulkDensity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Bulk density"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsBulkDensity); } }
        public bool IsPorosity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Porosity"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsPorosity); } }
        public bool IsHydraulicHead { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Hydraulic head"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsPorosity); } }
        public bool IsIntrinsicPermeability { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Intrinsic permeability"); } catch { return false; }; } }
        public bool IsIsotope { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Isotope"); } catch { return false; }; } }
        public bool IsApparentPermeability { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Apparent permeability"); } catch { return false; }; } }
        public bool IsThermalConductivity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Thermal conductivity"); } catch { return false; }; } }
        public bool IsThermalDiffusivity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Thermal diffusivity"); } catch { return false; }; } }
        public bool IsResistivity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Resistivity"); } catch { return false; }; } }
        public bool IsSonicWaveVelocity { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("Sonic wave velocity"); } catch { return false; }; } }
        public bool IsXRF { get { try { return SelectedLaboratoryMeasurement.measParameter.Contains("X-Ray Fluorescence"); } catch { return false; }; } }

        /// <summary>
        /// A view model to filter the main data set
        /// </summary>
        private FilterDataSetViewModel<tblRockSample> filterDataSetViewModel = new FilterDataSetViewModel<tblRockSample>();
        public FilterDataSetViewModel<tblRockSample> FilterDataSetViewModel
        {
            get => this.filterDataSetViewModel;
            set
            {
                this.filterDataSetViewModel = value;
                NotifyOfPropertyChange(() => FilterDataSetViewModel);
            }
        }

        //The import type for the sections
        public string ImportType { get; set; }

        #endregion

        #region Constructor

        //Initialization
        public Task Initialization { get; private set; }

        /// <summary>
        /// Initialize the Viewmodel asynchronously
        /// </summary>
        /// <returns></returns>
        private async Task InitializeAsync()
        {
            await LoadData().ConfigureAwait(false);

            Type = "All";

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public LabMeasurementDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            Initialization = InitializeAsync();
        }

        #endregion

        #region Methods

        //Initial data load method
        public async Task LoadData()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                try
                {
                    try
                    {
                        using (var db1 = new ApirsRepository<tblUnionPetrography>())
                        {
                            UnionPetrography = new BindableCollection<tblUnionPetrography>(db1.GetModel().ToList());
                        }
                    }
                    catch
                    {
                        UnionPetrography = new BindableCollection<tblUnionPetrography>();
                    }

                    try
                    {
                        using (var db = new ApirsRepository<tblUnionChronostratigraphy>())
                        {
                            Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(db.GetModel());
                        }
                    }
                    catch
                    {
                        Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                    }

                    try
                    {

                        using (var db = new ApirsRepository<tblObjectLithostratigraphy>())
                        {
                            Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(db.GetCompleteLithostratigraphy().ToList());
                        }

                    }
                    catch
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();

                    }
                    try
                    {
                        using (var db = new ApirsRepository<tblMeasuringDevice>())
                        {
                            AnalyticalInstrument = new BindableCollection<tblMeasuringDevice>(db.GetModel().OrderBy(a => a.mdName));
                        }
                    }
                    catch
                    {
                        AnalyticalInstrument = new BindableCollection<tblMeasuringDevice>();
                    }

                    try
                    {
                        using (var db = new ApirsRepository<tblObjectOfInvestigation>())
                        {
                            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db.GetModel());
                        }
                    }
                    catch
                    {
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            });
        }

        //Loading filtered data
        public async void LoadData(string parameter)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                using (var db = new ApirsRepository<tblRockSample>())
                {
                    try
                    {
                        RockSamples = new BindableCollection<tblRockSample>(db.GetRockSamplesByProject(Projects, parameter));
                    }
                    catch
                    {
                        RockSamples = new BindableCollection<tblRockSample>();
                    }
                }

                this.allRockSamples = new BindableCollection<tblRockSample>(RockSamples);

                FilterDataSetViewModel = new FilterDataSetViewModel<tblRockSample>(allRockSamples, RockSamples, new ObservableCollection<string>(), false);


                if (RockSamples.Count != 0)
                    SelectedRockSample = RockSamples.First();

            });

        }

        //Activating a background worker to select and download files and data asynchronously
        private async Task OnSelectedRockSampleChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                using (var db = new ApirsRepository<tblMeasurement>())
                {
                    try
                    {
                        LaboratoryMeasurements = new BindableCollection<tblMeasurement>(
                            db.GetModelByExpression(labme => labme.measRockSampleIdFk == SelectedRockSample.sampLabel).OrderBy(labme => labme.measParameter));
                    }
                    catch
                    {
                        LaboratoryMeasurements = new BindableCollection<tblMeasurement>();
                    }
                }

            }).ContinueWith((a) =>
            {
                if (LaboratoryMeasurements.Count() > 0)
                    SelectedLaboratoryMeasurement = LaboratoryMeasurements.First();
                else
                    SelectedLaboratoryMeasurement = new tblMeasurement();
            });
        }

        //Activating a background worker to select and download Files asynchronously
        private async Task OnSelectedLaboratoryMeasurementChanged()
        {
            CommandHelper ch = new CommandHelper();

            try
            {
                AxialCompression = new tblAxialCompression();
                GrainDensity = new tblGrainDensity();
                GrainSize = new tblGrainSize();
                BulkDensity = new tblBulkDensity();
                Porosity = new tblEffectivePorosity();
                HydraulicHead = new tblHydraulicHead();
                ApparentPermeability = new tblApparentPermeability();
                IntrinsicPermeability = new tblIntrinsicPermeability();
                Isotope = new tblIsotopes();
                ThermalConductivity = new tblThermalConductivity();
                ThermalDiffusivity = new tblThermalDiffusivity();
                Resistivity = new tblResistivity();
                SonicWaveVelocity = new tblSonicWave();
                Xrf = new tblXRayFluorescenceSpectroscopy();

                string tableName = new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableAlias == SelectedLaboratoryMeasurement.measParameter).First().alTableName;
                Properties = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableName == tableName && x.alDataType == "Numerical"));
                FilterProperties = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableName == tableName && x.alDataType == "Categorical"));

                switch (SelectedLaboratoryMeasurement.measParameter)
                {
                    case "Axial compression":
                        AxialCompression = new ApirsRepository<tblAxialCompression>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "Grain density":
                        using (var db1 = new ApirsRepository<tblGrainDensity>()) { GrainDensity = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Bulk density":
                        using (var db1 = new ApirsRepository<tblBulkDensity>()) { BulkDensity = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Porosity":
                        using (var db1 = new ApirsRepository<tblEffectivePorosity>()) { Porosity = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Hydraulic head":
                        using (var db1 = new ApirsRepository<tblHydraulicHead>()) { HydraulicHead = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Grain size":
                        using (var db1 = new ApirsRepository<tblGrainSize>()) { GrainSize = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Apparent permeability":
                        using (var db1 = new ApirsRepository<tblApparentPermeability>()) { ApparentPermeability = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Intrinsic permeability":
                        using (var db1 = new ApirsRepository<tblIntrinsicPermeability>()) { IntrinsicPermeability = db1.GetModelById(SelectedLaboratoryMeasurement.measIdPk); }
                        break;
                    case "Isotope":
                        Isotope = new ApirsRepository<tblIsotopes>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "Thermal conductivity":
                        ThermalConductivity = new ApirsRepository<tblThermalConductivity>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "Thermal diffusivity":
                        ThermalDiffusivity = new ApirsRepository<tblThermalDiffusivity>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "Resistivity":
                        Resistivity = new ApirsRepository<tblResistivity>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "Sonic wave velocity":
                        SonicWaveVelocity = new ApirsRepository<tblSonicWave>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                    case "X-Ray Fluorescence":
                        Xrf = new ApirsRepository<tblXRayFluorescenceSpectroscopy>().GetModelById(SelectedLaboratoryMeasurement.measIdPk);
                        break;
                }

                if (Properties.Count() > 0)
                    SelectedProperty = Properties.First();
                else
                    SelectedProperty = new tblAlia();

                if (FilterProperties.Count() > 0)
                    SelectedFilterProperty = FilterProperties.First();
                else
                    SelectedFilterProperty = new tblAlia();
            }
            catch (Exception e)
            {
                AxialCompression = new tblAxialCompression();
                GrainSize = new tblGrainSize();
                GrainDensity = new tblGrainDensity();
                BulkDensity = new tblBulkDensity();
                Porosity = new tblEffectivePorosity();
                HydraulicHead = new tblHydraulicHead();
                ApparentPermeability = new tblApparentPermeability();
                IntrinsicPermeability = new tblIntrinsicPermeability();
                Isotope = new tblIsotopes();
                ThermalConductivity = new tblThermalConductivity();
                ThermalDiffusivity = new tblThermalDiffusivity();
                Resistivity = new tblResistivity();
                SonicWaveVelocity = new tblSonicWave();
                Xrf = new tblXRayFluorescenceSpectroscopy();
            }

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                ch = new CommandHelper();

                await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsFileLoading, async () =>
                {
                    try
                    {
                        FileStore = await FileHelper.LoadFilesAsync(SelectedLaboratoryMeasurement.measIdPk, "LabMeasurement");
                        SelectedFileStore = FileStore.Count > 0 ? FileStore.First() : new v_FileStore();
                    }
                    catch
                    {
                        FileStore = new BindableCollection<v_FileStore>();
                        SelectedFileStore = new v_FileStore();
                    }
                });
            });
        }

        //Event that is fired when the index changed
        private void OnSelectedRockSampleIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != RockSamples.IndexOf(SelectedRockSample))
                {
                    SelectedRockSample = RockSamples.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a valid value");
            }
        }

        /// <summary>
        /// If index is changed go to picture
        /// </summary>
        /// <param name="parameter"></param>
        private void OnSelectedFileRockSampleIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != FileStore.IndexOf(SelectedFileStore))
                {
                    SelectedFileStore = FileStore.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }

        /// <summary>
        /// Creates a png file of a selected bitmap File
        /// </summary>
        public void DownloadFile()
        {
            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    FileHelper.LoadFile(SelectedFileStore.file_stream, dialog.FileName + "\\" + SelectedFileStore.name);
                }


            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }
        }

        /// <summary>
        /// Uploading an File file to the database
        /// </summary>
        public void UploadFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of Files to three
            if (Convert.ToInt32(CountFilesLaboratoryMeasurement) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of files is limited to three per measuremet");
                return;
            }

            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Word (*.docx)|*.docx|CSV (*.csv)|*.csv|Text (*.txt)|*.txt";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName == "")
            {
                return;
            }

            //Getting file information
            FileInfo fI = new FileInfo(openFileDlg.FileName);

            //Uploading file
            Guid id = FileHelper.UploadFile(openFileDlg.FileName);

            if (id == Guid.Empty)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                return;
            }

            using (var db = new ApirsRepository<tblFileLabMeasurement>())
            {
                try
                {
                    db.InsertModel(new tblFileLabMeasurement() { picName = fI.Name, labmeIdFk = SelectedLaboratoryMeasurement.measIdPk, picStreamIdFk = id });
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }
            }

            Refresh();
        }

        //If an File file gets dropped on the File control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Reading the data out
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            //Getting file information
            FileInfo fi = new FileInfo(FileList[0]);

            //Retrieving file meta data
            string fileName = fi.Name;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string extension = fi.Extension;

            if (extension != ".xlsx" && extension != ".csv" && extension != ".XLSX" && extension != ".CSV")
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please comply to the necessary file formats .xlsx or .csv");
                return;
            }
            try
            {
                var type1 = new tblRockSample();
                _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, FileList[0].ToString()));
            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }


        /// <summary>
        /// Deleting the currently selected File
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (CountFilesLaboratoryMeasurement == "0")
                return;

            string name = SelectedFileStore.name;

            try
            {
                //Instantiate database
                using (ApirsDatabase db = new ApirsDatabase())
                {
                    //Establishing a sql connection
                    using (SqlConnection SqlConn = new SqlConnection(db.Database.Connection.ConnectionString.ToString()))
                    {
                        SqlCommand spDeleteFile = new SqlCommand("dbo.spDeleteFile", SqlConn);

                        //Preparing the stored procedure
                        spDeleteFile.CommandType = System.Data.CommandType.StoredProcedure;

                        //Adding the parameters
                        spDeleteFile.Parameters.Add("@file_name", SqlDbType.NVarChar, 255).Value = name;

                        //Opening the connection
                        SqlConn.Open();

                        //Deleting the File in the database
                        spDeleteFile.ExecuteNonQuery();

                        SqlConn.Close();

                        Refresh();
                    }
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured");
            }
        }

        /// <summary>
        /// Go to the last dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Last()
        {
            if (RockSamples.Count != 0)
                SelectedRockSample = RockSamples.Last();
        }

        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (RockSamples.Count != 0)
                SelectedRockSample = Navigation.GetPrevious(RockSamples, SelectedRockSample);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {
            if (RockSamples.Count != 0)
                SelectedRockSample = Navigation.GetNext(RockSamples, SelectedRockSample);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (RockSamples.Count != 0)
                SelectedRockSample = RockSamples.First();
        }

        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviousFile()
        {
            if (FileStore.Count != 0)
                SelectedFileStore = Navigation.GetPrevious(FileStore, SelectedFileStore);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextFile()
        {
            if (FileStore.Count != 0)
                SelectedFileStore = Navigation.GetNext(FileStore, SelectedFileStore);
        }

        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override async void Refresh()
        {
            tblRockSample current = SelectedRockSample;
            tblMeasurement currentLabMeas = SelectedLaboratoryMeasurement;

            try
            {
                Initialization = InitializeAsync();
            }
            catch
            {
                try
                {
                    LoadData(current.sampType.ToString());
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            }
        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void Delete()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(SelectedLaboratoryMeasurements.Count() + " values will be deleted. Are you sure to go on?",
                "You won't be able to retrieve the related measurement data after deleting this measurement.") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblMeasurement>())
            {
                try
                {
                    foreach (tblMeasurement lab in SelectedLaboratoryMeasurements)
                        db.DeleteModelById(lab.measIdPk);


                    OnSelectedRockSampleChanged();

                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
                finally
                {
                }

            }
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.measUploaderId, SelectedLaboratoryMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblMeasurement>())
            {
                try
                {
                    db.UpdateModel(SelectedLaboratoryMeasurement, SelectedLaboratoryMeasurement.measIdPk);

                    switch (SelectedLaboratoryMeasurement.measParameter)
                    {
                        case "Axial compression":
                            if (AxialCompression.aclabmeIdFk == 0)
                            {
                                AxialCompression.aclabmeIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblAxialCompression>().InsertModel(AxialCompression);
                                NotifyOfPropertyChange(() => IsAxialCompression);
                            }
                            else
                            {
                                new ApirsRepository<tblAxialCompression>().UpdateModel(AxialCompression, AxialCompression.aclabmeIdFk);
                            }
                            break;
                        case "Grain density":
                            if (GrainDensity.gdIdFk == 0)
                            {
                                GrainDensity.gdIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblGrainDensity>().InsertModel(GrainDensity);
                                NotifyOfPropertyChange(() => IsGrainDensity);
                            }
                            else
                            {
                                new ApirsRepository<tblGrainDensity>().UpdateModel(GrainDensity, GrainDensity.gdIdFk);
                            }
                            break;
                        case "Grain size":
                            if (GrainSize.gslabmeIdFk == 0)
                            {
                                GrainSize.gslabmeIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblGrainSize>().InsertModel(GrainSize);
                                NotifyOfPropertyChange(() => IsGrainSize);
                            }
                            else
                            {
                                new ApirsRepository<tblGrainSize>().UpdateModel(GrainSize, GrainSize.gslabmeIdFk);
                            }
                            break;
                        case "Bulk density":
                            if (BulkDensity.bdIdFk == 0)
                            {
                                BulkDensity.bdIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblBulkDensity>().InsertModel(BulkDensity);
                                NotifyOfPropertyChange(() => IsBulkDensity);
                            }
                            else
                            {
                                new ApirsRepository<tblBulkDensity>().UpdateModel(BulkDensity, BulkDensity.bdIdFk);
                            }
                            break;
                        case "Porosity":
                            if (Porosity.porIdFk == 0)
                            {
                                Porosity.porIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblEffectivePorosity>().InsertModel(Porosity);
                                NotifyOfPropertyChange(() => IsPorosity);
                            }
                            else
                            {
                                new ApirsRepository<tblEffectivePorosity>().UpdateModel(Porosity, Porosity.porIdFk);
                            }
                            break;
                        case "Hydraulic head":
                            if (HydraulicHead.hhmeasIdFk == 0)
                            {
                                HydraulicHead.hhmeasIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblHydraulicHead>().InsertModel(HydraulicHead);
                                NotifyOfPropertyChange(() => IsHydraulicHead);
                            }
                            else
                            {
                                new ApirsRepository<tblHydraulicHead>().UpdateModel(HydraulicHead, HydraulicHead.hhmeasIdFk);
                            }
                            break;
                        case "Apparent permeability":
                            if (ApparentPermeability == null)
                            {
                                ApparentPermeability.apermIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblApparentPermeability>().InsertModel(ApparentPermeability);
                                NotifyOfPropertyChange(() => IsApparentPermeability);
                            }

                            if (ApparentPermeability.apermIdFk == 0)
                            {
                                ApparentPermeability.apermIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblApparentPermeability>().InsertModel(ApparentPermeability);
                            }
                            else
                            {
                                new ApirsRepository<tblApparentPermeability>().UpdateModel(ApparentPermeability, ApparentPermeability.apermIdFk);
                            }
                            break;
                        case "Intrinsic permeability":
                            if (IntrinsicPermeability.inpeIdFk == 0)
                            {
                                IntrinsicPermeability.inpeIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblIntrinsicPermeability>().InsertModel(IntrinsicPermeability);
                                NotifyOfPropertyChange(() => IsIntrinsicPermeability);
                            }
                            else
                            {
                                new ApirsRepository<tblIntrinsicPermeability>().UpdateModel(IntrinsicPermeability, IntrinsicPermeability.inpeIdFk);
                            }
                            break;
                        case "Isotope":
                            if (Isotope.islabmeIdFk == 0)
                            {
                                Isotope.islabmeIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblIsotopes>().InsertModel(Isotope);
                                NotifyOfPropertyChange(() => IsIsotope);
                            }
                            else
                            {
                                new ApirsRepository<tblIsotopes>().UpdateModel(Isotope, Isotope.islabmeIdFk);
                            }
                            break;
                        case "Thermal conductivity":
                            if (ThermalConductivity.tcIdFk == 0)
                            {
                                ThermalConductivity.tcIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblThermalConductivity>().InsertModel(ThermalConductivity);
                                NotifyOfPropertyChange(() => IsThermalConductivity);
                            }
                            else
                            {
                                new ApirsRepository<tblThermalConductivity>().UpdateModel(ThermalConductivity, ThermalConductivity.tcIdFk);
                            }
                            break;
                        case "Thermal diffusivity":
                            if (ThermalDiffusivity.tdIdFk == 0)
                            {
                                ThermalDiffusivity.tdIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblThermalDiffusivity>().InsertModel(ThermalDiffusivity);
                                NotifyOfPropertyChange(() => IsThermalDiffusivity);
                            }
                            else
                            {
                                new ApirsRepository<tblThermalDiffusivity>().UpdateModel(ThermalDiffusivity, ThermalDiffusivity.tdIdFk);
                            }
                            break;
                        case "Resistivity":
                            if (Resistivity.reslabmeIdFk == 0)
                            {
                                Resistivity.reslabmeIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblResistivity>().InsertModel(Resistivity);
                                NotifyOfPropertyChange(() => IsResistivity);
                            }
                            else
                            {
                                new ApirsRepository<tblResistivity>().UpdateModel(Resistivity, Resistivity.reslabmeIdFk);
                            }
                            break;
                        case "Sonic wave velocity":
                            if (SonicWaveVelocity.swIdFk == 0)
                            {
                                SonicWaveVelocity.swIdFk = SelectedLaboratoryMeasurement.measIdPk;
                                new ApirsRepository<tblSonicWave>().InsertModel(SonicWaveVelocity);
                                NotifyOfPropertyChange(() => IsSonicWaveVelocity);
                            }
                            else
                            {
                                new ApirsRepository<tblSonicWave>().UpdateModel(SonicWaveVelocity, SonicWaveVelocity.swIdFk);
                            }
                            break;
                        case "X-Ray Fluorescence":
                            if (Xrf.xrfIdPk == 0)
                            {
                                Xrf.xrfIdPk = SelectedLaboratoryMeasurement.measIdPk;
                                Xrf.xrfType = "Handheld";
                                new ApirsRepository<tblXRayFluorescenceSpectroscopy>().InsertModel(Xrf);
                                NotifyOfPropertyChange(() => IsXRF);
                            }
                            else
                            {
                                new ApirsRepository<tblXRayFluorescenceSpectroscopy>().UpdateModel(Xrf, Xrf.xrfIdPk);
                            }
                            break;

                    }
                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    try
                    {
                        if (e.Message.Contains("Sequence"))
                            return;
                        else if (e.InnerException.InnerException.Message.ToString().Contains("CHECK"))
                            ((ShellViewModel)IoC.Get<IShell>()).ShowError("Your provided values exceed the valid data ranges. Please review and try again.");
                        else
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                        }
                    }
                    catch (Exception ex)
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                    }
                }

            }

        }

        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        public void Add()
        {
            try
            {
                using (var db = new ApirsRepository<tblMeasurement>())
                {
                    try
                    {
                        tblMeasurement meas = new tblMeasurement()
                        {
                            measprjIdFk = SelectedProject.prjIdPk,
                            measParameter = "Undefined",
                            measRockSampleIdFk = SelectedRockSample.sampLabel,
                            measUploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId,
                            measType = "Laboratory measurement"
                        };

                        db.InsertModel(meas);

                        LaboratoryMeasurements.Add(meas);
                    }
                    catch
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("Measurement can't be added. Please check every field again.");
                        return;
                    }
                }
            }
            catch
            {
                return;
            }

        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
        {
            if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                return;
            }

            if (SelectedRockSample != null)
                switch (SelectedRockSample.sampType)
                {
                    case "Plug":
                        var type1 = new tblPlug();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, SelectedRockSample.sampLabel));
                        break;
                    case "Cuboid":
                        var type2 = new tblCuboid();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type2, SelectedRockSample.sampLabel));
                        break;
                    case "Handpiece":
                        var type3 = new tblHandpiece();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type3, SelectedRockSample.sampLabel));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("No sample selected.");
        }

        //Method to open a details windows dependent on the current item type
        public void SubSample(string type)
        {
            if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                return;
            }

            if (SelectedRockSample.sampIdPk != 0)
                switch (type)
                {
                    case "ThinSection":
                        var type1 = new tblThinSection();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, SelectedRockSample.sampLabel));
                        break;
                    case "Powder":
                        var type2 = new tblPowder();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type2, SelectedRockSample.sampLabel));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("No sample selected.");
        }

        //Initiate refresh because the view is not firing the event
        public void InRefresh() => Refresh();

        ///Event that gets fired if the filter text was changed
        public async Task Filter()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    await Task.WhenAll(FilterDataSetViewModel.Filter());
                    RockSamples = new BindableCollection<tblRockSample>(FilterDataSetViewModel.FilterDataSet);
                }
                finally
                {
                    if (!RockSamples.Contains(SelectedRockSample))
                        SelectedRockSample = RockSamples.First();
                }
            });
        }

        //Adding a participant to a project
        public void AddItem(string parameter)
        {
            if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                return;
            }

            try
            {
                //Opening a dialog window with all available persons excluding those that already participate the project
                AnalyticalInstrumentDetailsViewModel analyticalInstrumentDetailsViewModel = new AnalyticalInstrumentDetailsViewModel(this._events);
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialog(analyticalInstrumentDetailsViewModel);

                using (var db = new ApirsRepository<tblMeasuringDevice>())
                {
                    try
                    {
                        AnalyticalInstrument = new BindableCollection<tblMeasuringDevice>(db.GetModel().OrderBy(x => x.mdName));
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
            }
        }


        //Exporting the actually selected list of objects to a csv file
        public void ExportList()
        {
            if (RockSamples == null)
                return;
            if (RockSamples.Count == 0)
                return;
            if (SelectedRockSample == null)
                SelectedRockSample = RockSamples.First();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Exporting the list dependent on the sample type and the actual selection
                ExportHelper.ExportList<tblRockSample>(RockSamples, saveFileDialog.FileName, "LabMeasurement");
            }
        }

        //Exporting a control
        public void ExportControl(object parameter)
        {
            _events.PublishOnUIThreadAsync(new ExportControlMessage((UIElement)parameter, "image"));
        }

        /// <summary>
        /// Importing a dropped object of investigation data file
        /// </summary>
        /// <param name="e"></param>
        public void ImportFile(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Import))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                FileInfo fi = FileHelper.GetFileInfo(e);
                if (!FileHelper.CheckFileFormat(fi, FileImportTypes.ExcelAndCSV))
                    return;

                DataTable table = new DataTable();

                if (fi.Extension == ".XLSX" || fi.Extension == ".xlsx")
                {
                    DataSet tables = FileHelper.LoadWorksheetsInDataSheets(fi.FullName, false, "", fi.Extension);
                    table = tables.Tables[0];
                }
                else if (fi.Extension == ".CSV" || fi.Extension == ".csv")
                {
                    table = FileHelper.CsvToDataTable(fi.FullName, true);
                }

                switch (ImportType)
                {
                    case "No specific":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement>(
                                new ImportProcedureViewModel<tblMeasurement>(_events, table));
                        break;
                    case "Axial compression":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblAxialCompression>(
                            new ImportProcedureViewModel<tblMeasurement, tblAxialCompression>(_events, table));
                        break;
                    case "Grain size":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblGrainSize>(
                            new ImportProcedureViewModel<tblMeasurement, tblGrainSize>(_events, table));
                        break;
                    case "Grain density":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblGrainDensity>(
                            new ImportProcedureViewModel<tblMeasurement, tblGrainDensity>(_events, table));
                        break;
                    case "Bulk density":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblBulkDensity>(
                            new ImportProcedureViewModel<tblMeasurement, tblBulkDensity>(_events, table));
                        break;
                    case "Apparent permeability":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblApparentPermeability>(
                                 new ImportProcedureViewModel<tblMeasurement, tblApparentPermeability>(_events, table));
                        break;
                    case "Intrinsic permeability":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblIntrinsicPermeability>(
                                new ImportProcedureViewModel<tblMeasurement, tblIntrinsicPermeability>(_events, table));
                        break;
                    case "Isotope":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblIsotopes>(
                                new ImportProcedureViewModel<tblMeasurement, tblIsotopes>(_events, table));
                        break;
                    case "Porosity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblEffectivePorosity>(
                                 new ImportProcedureViewModel<tblMeasurement, tblEffectivePorosity>(_events, table));
                        break;
                    case "Hydraulic head":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblHydraulicHead>(
                                 new ImportProcedureViewModel<tblMeasurement, tblHydraulicHead>(_events, table));
                        break;
                    case "Thermal conductivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblThermalConductivity>(
                                 new ImportProcedureViewModel<tblMeasurement, tblThermalConductivity>(_events, table));
                        break;
                    case "Thermal diffusivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblThermalDiffusivity>(
                                new ImportProcedureViewModel<tblMeasurement, tblThermalDiffusivity>(_events, table));
                        break;
                    case "Resistivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblResistivity>(
                                new ImportProcedureViewModel<tblMeasurement, tblResistivity>(_events, table));
                        break;
                    case "Sonic wave velocity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblSonicWave>(
                                new ImportProcedureViewModel<tblMeasurement, tblSonicWave>(_events, table));
                        break;
                    case "X-Ray Fluorescence":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblXRayFluorescenceSpectroscopy>(
                                new ImportProcedureViewModel<tblMeasurement, tblXRayFluorescenceSpectroscopy>(_events, table));
                        break;
                }

            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
            }
        }

        #endregion
    }
}
