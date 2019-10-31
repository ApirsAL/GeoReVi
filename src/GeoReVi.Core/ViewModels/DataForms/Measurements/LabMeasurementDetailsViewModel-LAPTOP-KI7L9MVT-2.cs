using Caliburn.Micro;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Linq.Dynamic;
using System;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Data;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// View model for a single data entry form for rock samples
    /// </summary>
    public class LabMeasurementDetailsViewModel : Screen, IHandle<ShortCutMessage>
    {
        #region Private members

        //Rock sample collection
        private BindableCollection<tblRockSample> rockSamples;
        //Rock sample collection
        private BindableCollection<tblRockSample> allRockSamples;

        //Rock sample collection
        private BindableCollection<tblLaboratoryMeasurement> laboratoryMeasurements;
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
        private tblLaboratoryMeasurement selectedLaboratoryMeasurement;

        //Subproperties
        private tblGrainDensity grainDensity;
        private tblBulkDensity bulkDensity;
        private tblEffectivePorosity porosity;
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

        //Text filter variable
        private string textFilter;

        private string type;

        #endregion

        #region Public properties

        public string Type
        {
            get => this.type;
            set { this.type = value; if (value != null) LoadData(value);
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
        public tblLaboratoryMeasurement SelectedLaboratoryMeasurement
        {
            get { return this.selectedLaboratoryMeasurement; }
            set
            {
                try
                {
                    if (SelectedLaboratoryMeasurement.labmeIdPk != 0)
                        Update();
                }
                catch
                {
                }

                this.selectedLaboratoryMeasurement = value;

                if (value != null && value.labmeIdPk != 0)
                {
                    Initialization = OnSelectedLaboratoryMeasurementChanged();
                }

                NotifyOfPropertyChange(() => IsGrainDensity);
                NotifyOfPropertyChange(() => IsBulkDensity);
                NotifyOfPropertyChange(() => IsPorosity);
                NotifyOfPropertyChange(() => IsApparentPermeability);
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

        //Subproperties
        public tblGrainDensity GrainDensity { get { return this.grainDensity; } set { this.grainDensity = value; NotifyOfPropertyChange(() => GrainDensity); } }
        public tblBulkDensity BulkDensity { get { return this.bulkDensity; } set { this.bulkDensity = value; NotifyOfPropertyChange(() => BulkDensity); } }
        public tblEffectivePorosity Porosity { get { return this.porosity; } set { this.porosity = value; NotifyOfPropertyChange(() => Porosity); } }
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
        public BindableCollection<tblLaboratoryMeasurement> LaboratoryMeasurements
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
        /// All projects, the user acutally participates
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get
            {
                if ((BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).Projects != null)
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SideMenuViewModel.Projects;
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

        //Visibility members dependent on the selected lab measurement
        public bool IsGrainDensity { get { try { return SelectedLaboratoryMeasurement.labmeParameter == "Grain density"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsGrainDensity); } }
        public bool IsBulkDensity { get { try { return SelectedLaboratoryMeasurement.labmeParameter == "Bulk density"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsBulkDensity); } }
        public bool IsPorosity { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Porosity"); } catch { return false; }; } set { NotifyOfPropertyChange(() => IsPorosity); } }
        public bool IsIntrinsicPermeability { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Intrinsic permeability"); } catch { return false; }; } }
        public bool IsIsotope { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Isotope"); } catch { return false; }; } }
        public bool IsApparentPermeability { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Apparent permeability"); } catch { return false; }; } }
        public bool IsThermalConductivity { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Thermal conductivity"); } catch { return false; }; } }
        public bool IsThermalDiffusivity { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Thermal diffusivity"); } catch { return false; }; } }
        public bool IsResistivity { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Resistivity"); } catch { return false; }; } }
        public bool IsSonicWaveVelocity { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("Sonic wave velocity"); } catch { return false; }; } }
        public bool IsXRF { get { try { return SelectedLaboratoryMeasurement.labmeParameter.Contains("X-Ray Fluorescence"); } catch { return false; }; } }

        /// <summary>
        /// Text filter variable
        /// </summary>
        public string TextFilter
        {
            get
            {
                return this.textFilter;
            }
            set
            {
                this.textFilter = value;
                OnTextFilterChanged();
                NotifyOfPropertyChange(() => this.textFilter);
            }
        }

        /// <summary>
        /// Objects for the depth line chart
        /// </summary>
        public LineChartObject Lco { get; set; }
        public BoxPlotChartObject Bco { get; set; }
        public BubbleChartObject Bubco { get; set; }
        public VariogramChartObject Vco { get; set; }
        public TernaryChartObject Tco { get; set; }
        public BarChartObject Barco { get; set; }
        public BindableCollection<LineChartObject> ScatterPlots { get; set; }

        //variable to check to show all objects
        private bool all;
        public bool All
        {
            get => this.all;
            set
            {
                this.all = value;
                NotifyOfPropertyChange(() => All);
            }
        }

        /// <summary>
        /// Variable the data set will be grouped by
        /// </summary>
        private string groupBy;
        public string GroupBy
        {
            get => this.groupBy;
            set
            {
                this.groupBy = value;
                NotifyOfPropertyChange(() => GroupBy);
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

            Lco = new LineChartObject();
            Vco = new VariogramChartObject();
            Tco = new TernaryChartObject();
            Bco = new BoxPlotChartObject();
            Bubco = new BubbleChartObject();
            Barco = new BarChartObject();
            ScatterPlots = new BindableCollection<LineChartObject>();

            Initialization = InitializeAsync();
        }
        #endregion

        #region Methods

        //Initial data load method
        public async Task LoadData()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
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
                        switch (SelectedProject.prjName)
                        {
                            case "All projects":
                                IEnumerable<tblRockSample> query;
                                RockSamples = new BindableCollection<tblRockSample>(db.GetRockSamplesByProject(Projects, parameter));
                                break;

                            default:
                                //Selecting all rock samples related to the selected project
                                RockSamples = new BindableCollection<tblRockSample>(query = db.GetRockSamplesByProject(new List<tblProject>() { SelectedProject }, parameter));
                                break;
                        }
                    }
                    catch
                    {
                        RockSamples = new BindableCollection<tblRockSample>();
                    }
                }

                this.allRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                if (RockSamples.Count != 0)
                    SelectedRockSample = RockSamples.First();

            });

        }

        //Activating a background worker to select and download Files asynchronously
        private async Task OnSelectedRockSampleChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () => {
                using (var db = new ApirsRepository<tblLaboratoryMeasurement>())
                {
                    try
                    {
                        LaboratoryMeasurements = new BindableCollection<tblLaboratoryMeasurement>(db.GetModelByExpression(labme => labme.labmeSampleName == SelectedRockSample.sampLabel).OrderBy(labme => labme.labmeParameter));
                    }
                    catch
                    {
                        LaboratoryMeasurements = new BindableCollection<tblLaboratoryMeasurement>();
                    }
                }

                if (LaboratoryMeasurements.Count() > 0)
                    SelectedLaboratoryMeasurement = LaboratoryMeasurements.First();
                else
                    SelectedLaboratoryMeasurement = new tblLaboratoryMeasurement();

            });
        }

        //Activating a background worker to select and download Files asynchronously
        private async Task OnSelectedLaboratoryMeasurementChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                try
                {
                    GrainDensity = new tblGrainDensity();
                    BulkDensity = new tblBulkDensity();
                    Porosity = new tblEffectivePorosity();
                    ApparentPermeability = new tblApparentPermeability();
                    IntrinsicPermeability = new tblIntrinsicPermeability();
                    Isotope = new tblIsotopes();
                    ThermalConductivity = new tblThermalConductivity();
                    ThermalDiffusivity = new tblThermalDiffusivity();
                    Resistivity = new tblResistivity();
                    SonicWaveVelocity = new tblSonicWave();
                    Xrf = new tblXRayFluorescenceSpectroscopy();

                    switch (SelectedLaboratoryMeasurement.labmeParameter)
                    {
                        case "Grain density":
                            using (var db1 = new ApirsRepository<tblGrainDensity>()) { GrainDensity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Bulk density":
                            using (var db1 = new ApirsRepository<tblBulkDensity>()) { BulkDensity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Porosity":
                            using (var db1 = new ApirsRepository<tblEffectivePorosity>()) { Porosity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Apparent permeability":
                            using (var db1 = new ApirsRepository<tblApparentPermeability>()) { ApparentPermeability = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Intrinsic permeability":
                            using (var db1 = new ApirsRepository<tblIntrinsicPermeability>()) { IntrinsicPermeability = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Isotope":
                            using (var db1 = new ApirsRepository<tblIsotopes>()) { Isotope = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Thermal conductivity":
                            using (var db1 = new ApirsRepository<tblThermalConductivity>()) { ThermalConductivity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Thermal diffusivity":
                            using (var db1 = new ApirsRepository<tblThermalDiffusivity>()) { ThermalDiffusivity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Resistivity":
                            using (var db1 = new ApirsRepository<tblResistivity>()) { Resistivity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "Sonic wave velocity":
                            using (var db1 = new ApirsRepository<tblSonicWave>()) { SonicWaveVelocity = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                        case "X-Ray Fluorescence":
                            using (var db1 = new ApirsRepository<tblXRayFluorescenceSpectroscopy>()) { Xrf = db1.GetModelById(SelectedLaboratoryMeasurement.labmeIdPk); }
                            break;
                    }
                }
                catch (Exception e)
                {
                    GrainDensity = new tblGrainDensity();
                    BulkDensity = new tblBulkDensity();
                    Porosity = new tblEffectivePorosity();
                    ApparentPermeability = new tblApparentPermeability();
                    IntrinsicPermeability = new tblIntrinsicPermeability();
                    Isotope = new tblIsotopes();
                    ThermalConductivity = new tblThermalConductivity();
                    ThermalDiffusivity = new tblThermalDiffusivity();
                    Resistivity = new tblResistivity();
                    SonicWaveVelocity = new tblSonicWave();
                    Xrf = new tblXRayFluorescenceSpectroscopy();
                }
            });

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                #region Variable declaration

                List<string> c = RockSamples.Where(samp => samp.sampooiName == SelectedRockSample.sampooiName)
                .Select(samp => samp.sampLabel).ToList();

                    List<tblRockSample> samples = new List<tblRockSample>();

                    if (All)
                        samples = new List<tblRockSample>(RockSamples.ToList());
                    else
                        samples = new List<tblRockSample>(RockSamples.Where(samp => samp.sampooiName == SelectedRockSample.sampooiName));

                    IEnumerable<IGrouping<string, tblRockSample>> groups = new List<IGrouping<string, tblRockSample>>();

                    List<tblLaboratoryMeasurement> allMeasurements = new ApirsRepository<tblLaboratoryMeasurement>().GetModelByExpression(labme => c.Contains(labme.labmeSampleName)).ToList();

                    int i = 0;

                    var z = System.Linq.Expressions.Expression.Parameter(typeof(tblRockSample), "z");

                    List<Tuple<double, double, double, double, string>> measPoints = new List<Tuple<double, double, double, double, string>>();
                    List<Tuple<double, double, double, double, string>> values = new List<Tuple<double, double, double, double, string>>();

                    List<XY> variogramPoints = new List<XY>();

                    List<XY> query = new List<XY>();
                    #endregion

                    #region Initialization

                    Bco.Initialize();

                    Lco.Initialize();
                    Lco.InitializeStandardSpatialLog();

                    Tco.Initialize();

                    Barco.Initialize();
                    Barco.InitializeStandardHistogram();

                    Bubco.Initialize();
                    Bubco.InitializeStandardBubbleChart();

                    #endregion


                    try
                    {
                        Lco.Title = "Object: " + SelectedRockSample.sampooiName.ToString();
                        Bubco.Title = "Object: " + SelectedRockSample.sampooiName.ToString();

                        if (!All)
                        {
                            Bco.Title = "Object: " + SelectedRockSample.sampooiName.ToString();
                            Barco.Title = "Object: " + SelectedRockSample.sampooiName.ToString();
                        }
                        else
                        {
                            Bco.Title = "All objects";
                            Barco.Title = "All objects";
                        }


                        switch (GroupBy)
                        {
                            case "Object of investigation":
                                groups = from x in samples
                                         group x by x.sampooiName into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Petrography":
                                groups = from x in samples
                                         group x by x.sampPetrographicTerm into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Lithostratigraphy":
                                groups = from x in samples
                                         group x by x.sampLithostratigraphyName into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Chronostratigraphy":
                                groups = from x in samples
                                         group x by x.sampChronStratName into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Facies":
                                groups = from x in samples
                                         group x by x.sampFaciesFk into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Architectural element":
                                groups = from x in samples
                                         group x by x.sampArchitecturalElement into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            case "Depositional environment":
                                groups = from x in samples
                                         group x by x.sampDepositionalEnvironment into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;
                            default:
                                groups = from x in samples
                                         group x by x.sampType into newGroup
                                         orderby newGroup.Key
                                         select newGroup;
                                break;

                        }

                        foreach (var group in groups)
                        {
                            try
                            {
                                Bco.AddDataSeries();

                                switch (GroupBy)
                                {
                                    case "Object of investigation":
                                        c = samples.Where(samp => samp.sampooiName == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Petrography":
                                        c = samples.Where(samp => samp.sampPetrographicTerm == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Lithostratigraphy":
                                        c = samples.Where(samp => samp.sampLithostratigraphyName == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Chronostratigraphy":
                                        c = samples.Where(samp => samp.sampChronStratName == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Facies":
                                        c = samples.Where(samp => samp.sampFaciesFk == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Architectural element":
                                        c = samples.Where(samp => samp.sampArchitecturalElement == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    case "Depositional environment":
                                        c = samples.Where(samp => samp.sampDepositionalEnvironment == group.Key)
                                                               .Select(samp => samp.sampLabel)
                                                               .ToList();
                                        break;
                                    default:
                                        c = samples.Where(samp => samp.sampType == group.Key)
                                                       .Select(samp => samp.sampLabel)
                                                       .ToList();
                                        break;

                                }

                                List<tblLaboratoryMeasurement> measurements = new ApirsRepository<tblLaboratoryMeasurement>()
                                                                                    .GetModelByExpression(labme => c.Contains(labme.labmeSampleName))
                                                                                    .ToList();

                                //measurements related to the selected rock samples with the selected parameter
                                List<int> b = measurements.Where(labme => (string)labme.labmeParameter == SelectedLaboratoryMeasurement.labmeParameter
                                                                && labme.labmeIdPk != 0 && c.Contains(labme.labmeSampleName))
                                                                .Select(labme => (int)labme.labmeIdPk)
                                                                .ToList();

                                List<Tuple<double, double, double>> tup = new List<Tuple<double, double, double>>();
                                List<string> prop = new List<string>();

                                prop.Add("sampLocalXCoordinates");
                                prop.Add("sampLocalYCoordinates");
                                prop.Add("sampLocalZCoordinates");
                                prop.Add("sampLabel");

                                var body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[0]);
                                var lambdaX = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                                body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[1]);
                                var lambdaY = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                                body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[2]);
                                var lambdaZ = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, double?>>(body, z);

                                body = System.Linq.Expressions.Expression.PropertyOrField(z, prop[3]);
                                var lambdaName = System.Linq.Expressions.Expression.Lambda<Func<tblRockSample, string>>(body, z);

                                //temporal storage for measurement values
                                double[] a = new double[] { };

                                if (SelectedLaboratoryMeasurement.labmeParameter == "Apparent permeability")
                                {
                                    values = new ApirsRepository<tblApparentPermeability>().GetModelByExpression(aperm => b.Contains(aperm.apermIdFk) && aperm.apermMeasurementDirection == ApparentPermeability.apermMeasurementDirection)
                                .Select(aperm => new Tuple<double, double, double, double, string>(
                                    Convert.ToDouble(aperm.apermValueMD),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == aperm.apermIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == aperm.apermIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == aperm.apermIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == aperm.apermIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                )).ToList();

                                    Lco.XLabel = "Apparent Permeability [mD] (" + ApparentPermeability.apermMeasurementDirection + "-direction)";
                                    Bco.XLabel = "Apparent Permeability [mD] (" + ApparentPermeability.apermMeasurementDirection + "-direction)";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Intrinsic permeability")
                                {
                                    values = new ApirsRepository<tblIntrinsicPermeability>()
                                    .GetModelByExpression(inpe => b.Contains(inpe.inpeIdFk))
                                    .Select(inpe => new Tuple<double, double, double, double, string>(
                                        Convert.ToDouble(inpe.inpeValuemD),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == inpe.inpeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == inpe.inpeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == inpe.inpeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                        RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == inpe.inpeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Intrinsic Permeability [mD]";
                                    Bco.XLabel = "Intrinsic Permeability [mD]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Isotope")
                                {
                                    values = new ApirsRepository<tblIsotopes>()
                                    .GetModelByExpression(iso => b.Contains(iso.islabmeIdFk))
                                    .Select(iso => new Tuple<double, double, double, double, string>(
                                        Convert.ToDouble(iso.isValue),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == iso.islabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == iso.islabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                        Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == iso.islabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                        RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == iso.islabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Isotope";
                                    Bco.XLabel = "Intrinsic Permeability [mD]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Bulk density")
                                {
                                    values = new ApirsRepository<tblBulkDensity>().GetModelByExpression(bd => b.Contains(bd.bdIdFk))
                                .Select(bd => new Tuple<double, double, double, double, string>(
                                    Convert.ToDouble(bd.bdValue),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == bd.bdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == bd.bdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == bd.bdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == bd.bdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Bulk density [g cm\x207B\xB3]";
                                    Bco.XLabel = "Bulk density [g cm\x207B\xB3]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Porosity")
                                {
                                    values = new ApirsRepository<tblEffectivePorosity>().GetModelByExpression(por => b.Contains(por.porIdFk))
                            .Select(por => new Tuple<double, double, double, double, string>(
                                Convert.ToDouble(por.porValuePerc),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == por.porIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == por.porIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == por.porIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == por.porIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Porosity [%]";
                                    Bco.XLabel = "Porosity [%]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Grain density")
                                {
                                    values = new ApirsRepository<tblGrainDensity>().GetModelByExpression(gd => b.Contains(gd.gdIdFk))
                                .Select(gd => new Tuple<double, double, double, double, string>(
                                    Convert.ToDouble(gd.gdMeanDensity),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == gd.gdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == gd.gdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == gd.gdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == gd.gdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Grain density [g cm\x207B\xB3]";
                                    Bco.XLabel = "Grain density [g cm\x207B\xB3]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Thermal conductivity")
                                {
                                    values = new ApirsRepository<tblThermalConductivity>().GetModelByExpression(tc => b.Contains(tc.tcIdFk))
                                .Select(tc => new Tuple<double, double, double, double, string>(
                                    Convert.ToDouble(tc.tcAvValue),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == tc.tcIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == tc.tcIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == tc.tcIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == tc.tcIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Thermal conducticity [W m\x207B\xB9 K\x207B\xB9]";
                                    Bco.XLabel = "Thermal conducticity [W m\x207B\xB9 K\x207B\xB9]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Thermal diffusivity")
                                {
                                    values = new ApirsRepository<tblThermalDiffusivity>().GetModelByExpression(td => b.Contains(td.tdIdFk))
                                .Select(td => new Tuple<double, double, double, double, string>(
                                    Convert.ToDouble(td.tdAvValue),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == td.tdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == td.tdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                    Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == td.tdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                    RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == td.tdIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                    )).ToList();

                                    Lco.XLabel = "Thermal diffusivity [m\xB2 s\x207B\xB9]";
                                    Bco.XLabel = "Thermal diffusivity [m\xB s\x207B\xB9]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Resistivity")
                                {
                                    values = new ApirsRepository<tblResistivity>().GetModelByExpression(res => b.Contains(res.reslabmeIdFk))
                            .Select(res => new Tuple<double, double, double, double, string>(
                                Convert.ToDouble(res.resValue),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == res.reslabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == res.reslabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == res.reslabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == res.reslabmeIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                )).ToList();

                                    Lco.XLabel = "Resistivity [Ωm]";
                                    Bco.XLabel = "Resistivity [Ωm]";
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "Sonic wave velocity")
                                {
                                    values = new ApirsRepository<tblSonicWave>().GetModelByExpression(sw => b.Contains(sw.swIdFk) && sw.swWavetype == SonicWaveVelocity.swWavetype)
                            .Select(sw => new Tuple<double, double, double, double, string>(
                                Convert.ToDouble(sw.swVelocity),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == sw.swIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == sw.swIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == sw.swIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == sw.swIdFk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                )).ToList();

                                    Lco.XLabel = "Sonic wave velocity [m s\x207B\xB9] " + SonicWaveVelocity.swWavetype;
                                    Bco.XLabel = "Sonic wave velocity [m s\x207B\xB9] " + SonicWaveVelocity.swWavetype;
                                }
                                else if (SelectedLaboratoryMeasurement.labmeParameter == "X-Ray Fluorescence")
                                {
                                    values = new ApirsRepository<tblXRayFluorescenceSpectroscopy>().GetModelByExpression(sw => b.Contains(xrf.xrfIdPk))
                            .Select(sw => new Tuple<double, double, double, double, string>(
                                Convert.ToDouble(xrf.xrfSiO2),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == xrf.xrfIdPk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaX.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == xrf.xrfIdPk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaY.Compile()).First()),
                                Convert.ToDouble(RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == xrf.xrfIdPk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaZ.Compile()).First()),
                                RockSamples.Where(rs => rs.sampLabel == measurements.Where(labme => labme.labmeIdPk == xrf.xrfIdPk).Select(labme => labme.labmeSampleName).FirstOrDefault()).Select(lambdaName.Compile()).First()
                                )).ToList();

                                    Bco.XLabel = "SiO2 [%]";
                                    Bco.XLabel = "SiO2 [%]";
                                }

                                if (Bco.IsXLog)
                                {
                                    a = values.Select(x => Math.Log10(x.Item1)).Where(x => !double.IsNegativeInfinity(x)).ToArray();
                                    Bco.XLabel = "log(" + Bco.XLabel + ")";
                                }
                                else
                                    a = values.Select(x=>x.Item1).Where(x => x != 0).ToArray();

                                Bco.Bps.BoxPlotStatisticsCollection.Add(new BoxPlotStatistics(a, Bco.OutliersRemoved, Bco.OutlierRange));

                                Bco.Bps.SeriesName = group.Key;

                                Bco.Xmin = 0;
                                Bco.YTick = 1;
                                Bco.SubdivideAxes();
                                Bco.DataCollection.Add(Bco.Bps);
                                Bco.Ymax = Bco.DataCollection.Count();

                                //
                                //Adding histogram data
                                //

                                Barco.AddDataSeries();
                                Barco.Bs[i].SeriesName = group.Key;
                                Barco.XLabel = Bco.XLabel.ToString();

                                //Sorting and filtering
                                if (!Barco.IsXLog)
                                    Barco.Bs[i].Values = values.Where(x => x.Item1 != 0).OrderBy(x => x.Item1).Select(x=> x.Item1).ToList();
                                else
                                {
                                    Barco.Bs[i].Values = values.Select(x => Math.Log10(x.Item1)).Where(x => !double.IsNegativeInfinity(x)).OrderBy(x => x).ToList();
                                    Barco.XLabel = "log(" + Barco.XLabel.ToString() + ")";
                                }

                                Barco.AllValues.AddRange(Barco.Bs[i].Values);

                                i = Barco.Bs.Count;

                                List<v_PetrophysicsRockSamples> pet = new ApirsRepository<v_PetrophysicsRockSamples>()
                                        .GetModelByExpression(pets => c.Contains(pets.labmeSampleName)).ToList();

                            //DataTable dt = CollectionHelper.ConvertNumbersTo<v_PetrophysicsRockSamples>(pet);
                            //CollectionHelper.ProcessNumericDataTable(dt);
                            //PrincipalComponentHelper pca = new PrincipalComponentHelper(dt);
                            //pca.CalculatePCA();

                                measPoints.AddRange(values);

                                tup.AddRange(new List<Tuple<double, double, double>>(pet
                                    .Where(x => x.P_sonic_wave_velocity != -9999 && x.Grain_density != -9999 && x.Porosity != -9999)
                                    .Select(x => new Tuple<double, double, double>(((double)x.P_sonic_wave_velocity != -9999) ? (double)x.P_sonic_wave_velocity : 0, (double)x.Grain_density, (double)x.Porosity))
                                    ));

                            //for (int j = 0; j < 9; j++)
                            //{
                            //    if (ScatterPlots.Count < 1)
                            //    {
                            //        LineChartObject lco = new LineChartObject();
                            //        lco.Initialize();
                            //        lco.InitializeStandardSpatialLog();
                            //    }

                            //    for (int k = 0; k < groups.Count(); k++)
                            //        if (j == 0)
                            //        {
                            //            Lco.AddDataSeries();
                            //            Lco.PointSeries = new List<List<Tuple<double, double, double, double, string>>>() { measPoints };
                            //            Lco.CreateChart();

                            //            ScatterPlots[i].AddDataSeries();
                            //        }

                            //}


                            Tco.AddDataSeries();

                                Tco.XLabel = "Bulk density";
                                Tco.YLabel = "Porosity";
                                Tco.ZLabel = "Grain density";

                                Tco.PointSeries.Add(tup);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        Lco.XLabel = Lco.IsXLog ? "log(" + Lco.XLabel + ")" : Lco.XLabel;


                        List<LocationValue<double>> locVal = new List<LocationValue<double>>();
                        measPoints = new List<Tuple<double, double, double, double, string>>(measPoints.OrderBy(x => x.Item2).OrderBy(x => x.Item3).OrderBy(x => x.Item4));

                        Vco.InitializeStandardVariogram();
                        Vco.MeasuringPoints = new List<Tuple<double, double, double, double, string>>(measPoints.OrderBy(x => x.Item2).OrderBy(x => x.Item3).OrderBy(x => x.Item4));
                        Vco.Title = "Object: " + SelectedRockSample.sampooiName.ToString();
                        Vco.CreateChart(Lco.XLabel);

                        Lco.AddDataSeries();
                        Lco.PointSeries = new List<List<Tuple<double, double, double, double, string>>>() { measPoints };
                        Lco.CreateChart();

                        Bubco.Points.AddRange(new List<Tuple<double, double, double, double, string>>(measPoints));
                        Bubco.Unit = Lco.XLabel.ToString();

                        Bubco.CreateChart();

                        Tco.CreateChart();
                        Barco.CreateChart();
                    }
                    catch (Exception e)
                    {
                    }
                });

            ch = new CommandHelper();

                await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsFileLoading, async () =>
            {
                    try
                    {
                        FileStore = await FileHelper.LoadFilesAsync(SelectedLaboratoryMeasurement.labmeIdPk, "LabMeasurement");
                        SelectedFileStore = FileStore.Count > 0 ? FileStore.First() : new v_FileStore();
                    }
                    catch
                    {
                        FileStore = new BindableCollection<v_FileStore>();
                        SelectedFileStore = new v_FileStore();
                    }
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Uploading an File file to the database
        /// </summary>
        public void UploadFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.labmeUploaderId, SelectedLaboratoryMeasurement.labmeIdPk))
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
                    db.InsertModel(new tblFileLabMeasurement() { picName = fI.Name, labmeIdFk = SelectedLaboratoryMeasurement.labmeIdPk, picStreamIdFk = id });
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }
                db.Save();
            }

            Refresh();
        }

        //If an File file gets dropped on the File control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.labmeUploaderId, SelectedLaboratoryMeasurement.labmeIdPk))
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
                _events.PublishOnUIThread(new OpenDataWindowMessage(type1, FileList[0].ToString()));
            }
            catch (Exception ex)
            {
                _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }


        /// <summary>
        /// Deleting the currently selected File
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.labmeUploaderId, SelectedLaboratoryMeasurement.labmeIdPk))
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

                        //Testing if a connection is established
                        //Normally: if (sv.IsNetworkAvailable() && sv.IsServerConnected("130.83.96.87"))
                        if (ServerInteractionHelper.IsNetworkAvailable())
                        {
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
            tblLaboratoryMeasurement currentLabMeas = SelectedLaboratoryMeasurement;

            try
            {
                int selected = SelectedRockSample.sampIdPk;
                int selectedMeas = SelectedLaboratoryMeasurement.labmeIdPk;
                LoadData(SelectedRockSample.sampType.ToString());
                SelectedRockSample = RockSamples.Where(x => x.sampIdPk == selected).First();
                try
                {
                    SelectedLaboratoryMeasurement = LaboratoryMeasurements.Where(x => x.labmeIdPk == selectedMeas).First();
                }
                catch
                {
                    try
                    {
                        SelectedLaboratoryMeasurement = LaboratoryMeasurements.First();
                    }
                    catch
                    { }
                }
            }
            catch
            {
                try
                {
                    LoadData(current.sampType.ToString());
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
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

            if ((int)((ShellViewModel)IoC.Get<IShell>()).UserId == 0)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please login first.");
                return;
            }
            else if (SelectedLaboratoryMeasurement == null)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No measurement selected");
                return;
            }
            else if (SelectedLaboratoryMeasurement.labmeUploaderId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object.");
                return;
            }
            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?",
                "You won't be able to retrieve the related measurement data after deleting this measurement.") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblLaboratoryMeasurement>())
            {
                try
                {
                    db.DeleteModelById(SelectedLaboratoryMeasurement.labmeIdPk);

                    OnSelectedRockSampleChanged();

                }
                catch (Exception ex)
                {
                    _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedLaboratoryMeasurement, (int)SelectedLaboratoryMeasurement.labmeUploaderId, SelectedLaboratoryMeasurement.labmeIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblLaboratoryMeasurement>())
            {
                try
                {

                    db.UpdateModel(SelectedLaboratoryMeasurement, SelectedLaboratoryMeasurement.labmeIdPk);

                    switch (SelectedLaboratoryMeasurement.labmeParameter)
                    {
                        case "Grain density":
                            if (GrainDensity.gdIdFk == 0)
                            {
                                GrainDensity.gdIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblGrainDensity>().InsertModel(GrainDensity);
                            }
                            else
                            {
                                new ApirsRepository<tblGrainDensity>().UpdateModel(GrainDensity, GrainDensity.gdIdFk);
                            }
                            break;
                        case "Bulk density":
                            if (BulkDensity.bdIdFk == 0)
                            {
                                BulkDensity.bdIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblBulkDensity>().InsertModel(BulkDensity);
                            }
                            else
                            {
                                new ApirsRepository<tblBulkDensity>().UpdateModel(BulkDensity, BulkDensity.bdIdFk);
                            }
                            break;
                        case "Porosity":
                            if (Porosity.porIdFk == 0)
                            {
                                Porosity.porIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblEffectivePorosity>().InsertModel(Porosity);
                            }
                            else
                            {
                                new ApirsRepository<tblEffectivePorosity>().UpdateModel(Porosity, Porosity.porIdFk);
                            }
                            break;
                        case "Apparent permeability":
                            if (ApparentPermeability == null)
                            {
                                ApparentPermeability.apermIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblApparentPermeability>().InsertModel(ApparentPermeability);
                            }

                            if (ApparentPermeability.apermIdFk == 0)
                            {
                                ApparentPermeability.apermIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
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
                                IntrinsicPermeability.inpeIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblIntrinsicPermeability>().InsertModel(IntrinsicPermeability);
                            }
                            else
                            {
                                new ApirsRepository<tblIntrinsicPermeability>().UpdateModel(IntrinsicPermeability, IntrinsicPermeability.inpeIdFk);
                            }
                            break;
                        case "Isotope":
                            if (Isotope.islabmeIdFk== 0)
                            {
                                Isotope.islabmeIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblIsotopes>().InsertModel(Isotope);
                            }
                            else
                            {
                                new ApirsRepository<tblIsotopes>().UpdateModel(Isotope, Isotope.islabmeIdFk);
                            }
                            break;
                        case "Thermal conductivity":
                            if (ThermalConductivity.tcIdFk == 0)
                            {
                                ThermalConductivity.tcIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblThermalConductivity>().InsertModel(ThermalConductivity);
                            }
                            else
                            {
                                new ApirsRepository<tblThermalConductivity>().UpdateModel(ThermalConductivity, ThermalConductivity.tcIdFk);
                            }
                            break;
                        case "Thermal diffusivity":
                            if (ThermalDiffusivity.tdIdFk == 0)
                            {
                                ThermalDiffusivity.tdIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblThermalDiffusivity>().InsertModel(ThermalDiffusivity);
                            }
                            else
                            {
                                new ApirsRepository<tblThermalDiffusivity>().UpdateModel(ThermalDiffusivity, ThermalDiffusivity.tdIdFk);
                            }
                            break;
                        case "Resistivity":
                            if (Resistivity.reslabmeIdFk == 0)
                            {
                                Resistivity.reslabmeIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblResistivity>().InsertModel(Resistivity);
                            }
                            else
                            {
                                new ApirsRepository<tblResistivity>().UpdateModel(Resistivity, Resistivity.reslabmeIdFk);
                            }
                            break;
                        case "Sonic wave velocity":
                            if (SonicWaveVelocity.swIdFk == 0)
                            {
                                SonicWaveVelocity.swIdFk = SelectedLaboratoryMeasurement.labmeIdPk;
                                new ApirsRepository<tblSonicWave>().InsertModel(SonicWaveVelocity);
                            }
                            else
                            {
                                new ApirsRepository<tblSonicWave>().UpdateModel(SonicWaveVelocity, SonicWaveVelocity.swIdFk);
                            }
                            break;
                        case "X-Ray Fluorescence":
                            if (Xrf.xrfIdPk == 0)
                            {
                                Xrf.xrfIdPk = SelectedLaboratoryMeasurement.labmeIdPk;
                                Xrf.xrfType = "Handheld";
                                new ApirsRepository<tblXRayFluorescenceSpectroscopy>().InsertModel(Xrf);
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
                            ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured");
                    }
                    catch (Exception ex)
                    {
                        _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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
                if (((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk == 0)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a project first.");
                    return;
                }
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }
            using (var db = new ApirsRepository<tblLaboratoryMeasurement>())
            {
                try
                {
                    db.InsertModel(new tblLaboratoryMeasurement() { labmeprjIdFk = SelectedProject.prjIdPk, labmeSampleName = SelectedRockSample.sampLabel, labmeUploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId });
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("New measurement added.");
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Measurement can't be added. Please check every field again.");
                    return;
                }
            }
            Refresh();
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
                        _events.PublishOnUIThread(new OpenDataWindowMessage(type1, SelectedRockSample.sampLabel));
                        break;
                    case "Cuboid":
                        var type2 = new tblCuboid();
                        _events.PublishOnUIThread(new OpenDataWindowMessage(type2, SelectedRockSample.sampLabel));
                        break;
                    case "Handpiece":
                        var type3 = new tblHandpiece();
                        _events.PublishOnUIThread(new OpenDataWindowMessage(type3, SelectedRockSample.sampLabel));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

        //Method to open a details windows dependend on the current item type
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
                        _events.PublishOnUIThread(new OpenDataWindowMessage(type1, SelectedRockSample.sampLabel));
                        break;
                    case "Powder":
                        var type2 = new tblPowder();
                        _events.PublishOnUIThread(new OpenDataWindowMessage(type2, SelectedRockSample.sampLabel));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

        //Initiate refresh because the view is not firing the event
        public void InRefresh() => Refresh();

        //Delete a range of measurements
        public void RemoveMeasurements(object parameter)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, (int)SelectedRockSample.sampIdPk))
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
                ListView selectedMeasurements = (ListView)parameter;
                List<int> measIndex = new List<int>();

                if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(selectedMeasurements.SelectedItems.Count.ToString() + " measurements will be deleted. Are you sure to delete the records?",
            "You won't be able to retrieve the related data after deleting these measurements.") == MessageBoxViewResult.No)
                {
                    return;
                }

                if (selectedMeasurements.SelectedItems.Count <= 0)
                    return;

                for (int i = 0; i < selectedMeasurements.SelectedItems.Count; i++)
                {
                    measIndex.Add(selectedMeasurements.Items.IndexOf(selectedMeasurements.SelectedItems[i]));
                }

                using (var db = new ApirsRepository<tblLaboratoryMeasurement>())
                {
                    for (int j = 0; j < measIndex.Count; j++)
                    {
                        var id = LaboratoryMeasurements.ElementAt(measIndex[j]).labmeIdPk;
                        //tblLaboratoryMeasurement result = db.GetModelByExpression(x => x.labmeIdPk == id).First();
                        db.DeleteModelById(id);
                    }

                    OnSelectedRockSampleChanged();
                }

            }
            catch (Exception e)
            {
                _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }

        }

        ///Event that gets fired if the filter text was changed
        private async void OnTextFilterChanged()
        {
            if (TextFilter == "")
            {
                RockSamples = this.allRockSamples;
            }

            CommandHelper ch = new CommandHelper();


            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                //Filtering data based on the selection
                try
                  {
                      //Filtering outcrops
                      RockSamples = new BindableCollection<tblRockSample>(CollectionHelper.Filter<tblRockSample>(allRockSamples, TextFilter).ToList());
                  }
                  catch (Exception e)
                  {
                      _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
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
            _events.PublishOnUIThread(new ExportControlMessage((UIElement)parameter, "image"));
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
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement>(_events, table));
                        break;
                    case "Grain density":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblGrainDensity>(
                            new ImportProcedureViewModel<tblLaboratoryMeasurement, tblGrainDensity>(_events, table));
                        break;
                    case "Bulk density":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblBulkDensity>(
                            new ImportProcedureViewModel<tblLaboratoryMeasurement, tblBulkDensity>(_events, table));
                        break;
                    case "Apparent permeability":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblApparentPermeability>(
                                 new ImportProcedureViewModel<tblLaboratoryMeasurement, tblApparentPermeability>(_events, table));
                        break;
                    case "Intrinsic permeability":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblIntrinsicPermeability>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblIntrinsicPermeability>(_events, table));
                        break;
                    case "Isotope":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblIsotopes>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblIsotopes>(_events, table));
                        break;
                    case "Porosity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblEffectivePorosity>(
                                 new ImportProcedureViewModel<tblLaboratoryMeasurement, tblEffectivePorosity>(_events, table));
                        break;
                    case "Thermal conductivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblThermalConductivity>(
                                 new ImportProcedureViewModel<tblLaboratoryMeasurement, tblThermalConductivity>(_events, table));
                        break;
                    case "Thermal diffusivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblThermalDiffusivity>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblThermalDiffusivity>(_events, table));
                        break;
                    case "Resistivity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblResistivity>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblResistivity>(_events, table));
                        break;
                    case "Sonic wave velocity":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblSonicWave>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblSonicWave>(_events, table));
                        break;
                    case "XRF":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblLaboratoryMeasurement, tblXRayFluorescenceSpectroscopy>(
                                new ImportProcedureViewModel<tblLaboratoryMeasurement, tblXRayFluorescenceSpectroscopy>(_events, table));
                        break;
                }

            }
            catch (Exception ex)
            {
                _events.BeginPublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        /// <summary>
        /// Updating chart event
        /// </summary>
        /// <param name="type"></param>
        public async void UpdateChart(string type)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                if (type == "SpatialPlot")
                {
                    Lco.DataCollection.UpdateChart();
                }
                else if (type == "VariogramPlot")
                {
                    Vco.CreateChart(Vco.Ds[0].SeriesName);
                }
                else if (type == "OptimizeVariogram")
                {
                    Vco.OptimizeModel();
                }
                else if (type == "BoxPlot")
                {
                    Bco.DataCollection.UpdateChart();
                }
                else if (type == "HistoPlot")
                {
                    Barco.HistogramDataCollection.UpdateChart();
                }
                else if (type == "BubblePlot")
                {
                    Bubco.CreateChart();
                    //Bubco.DataCollection.UpdateChart();
                }
                else if (type == "TernaryPlot")
                {
                    Tco.DataCollection.UpdateChart();
                    //Bubco.DataCollection.UpdateChart();
                }
            });
        }

        /// <summary>
        /// Handles a short cut message from the shellviewmodel
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ShortCutMessage message)
        {
            if (message.Characters == "")
                return;

            switch (message.Characters)
            {
                case "S":
                    Update();
                    break;
                case "N":
                    Add();
                    break;
                case "D":
                    AddDetails();
                    break;
                case "Right":
                    Next();
                    break;
                case "Left":
                    Previous();
                    break;
                case "Project changed":
                    if (SelectedRockSample != null)
                        LoadData(SelectedRockSample.sampType);
                    else
                        LoadData("Plug");
                    break;
            }
        }
        #endregion

        #region Helper
        //Converts a null value to string
        private string NullToString(object Value)
        {

            // Value.ToString() allows for Value being DBNull, but will also convert int, double, etc.
            return Value == null ? "" : Value.ToString();

            // If this is not what you want then this form may suit you better, handles 'Null' and DBNull otherwise tries a straight cast
            // which will throw if Value isn't actually a string object.
            //return Value == null || Value == DBNull.Value ? "" : (string)Value;
        }
        #endregion
    }
}
