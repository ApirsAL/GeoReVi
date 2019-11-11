using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.Generic;
using System.Drawing;
using MoreLinq;
using System.Collections.ObjectModel;

namespace GeoReVi
{
    /// <summary>
    /// View model for the field measurements
    /// </summary>
    public class FieldMeasurementDetailsViewModel : Screen, IHandle<ShortCutMessage>
    {
        #region Private members

        //Object lithostrat collection
        private BindableCollection<tblObjectLithostratigraphy> ooiLithostrat;


        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> objectOfInvestigation;

        //All objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> allObjectOfInvestigation;

        //Analytical instruments collection
        private BindableCollection<tblMeasuringDevice> analyticalInstrument;

        //Location of the object of investigation
        private Location ooiLocation;

        /// <summary>
        /// Selected laboratory measurements
        /// </summary>
        //Pictures
        private BindableCollection<v_FileStore> fileStore;
        private v_FileStore selectedFileStore;

        //checks if File is loading
        private bool isFileLoading = false;

        //Cancellation token for async methods
        CancellationTokenSource cts;

        /// <summary>
        /// Selected objects
        /// </summary>
        private tblObjectOfInvestigation selectedObject;

        //Facies collection
        private BindableCollection<tblFacy> facies;
        //Architectural element colleciton
        private BindableCollection<tblArchitecturalElement> architecturalElements;
        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;
        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;


        /// <summary>
        /// Field measurement collections and objects
        /// </summary>
        private BindableCollection<tblMeasurement> fieldMeasurements;
        private BindableCollection<tblMeasurement> allFieldMeasurements;
        private tblMeasurement selectedFieldMeasurement;

        //Subproperties
        private tblTotalGammaRay totalGammaRay;
        private tblSpectralGammaRay spectralGammaRay;
        private tblSusceptibility magneticSusceptibility;
        private tblStructureOrientation structureOrientation;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        //Text filter variable
        private string textFilter;

        private string type;

        #endregion

        #region Public properties

        public string Type
        {
            get => this.type;
            set
            {
                this.type = value;
                if (value != null)
                    LoadData(value);

                NotifyOfPropertyChange(() => Type);
            }
        }

        /// <summary>
        /// The selected Object
        /// </summary>
        public tblObjectOfInvestigation SelectedObject
        {
            get
            {
                return this.selectedObject;
            }
            set
            {
                this.selectedObject = value;

                if (value != null)
                    OnSelectedObjectChanged();

                NotifyOfPropertyChange(() => SelectedObject);
                NotifyOfPropertyChange(() => OoiLithostrat);
                NotifyOfPropertyChange(() => SelectedObjectIndex);
            }
        }

        //All Analytical devices
        public BindableCollection<tblMeasuringDevice> AnalyticalInstrument
        {
            get { return this.analyticalInstrument; }
            set { this.analyticalInstrument = value; NotifyOfPropertyChange(() => AnalyticalInstrument); }
        }

        //Center property for a map
        public Location Center
        {
            get
            {
                return this.ooiLocation;
            }
            set
            {
                this.ooiLocation = value;
                NotifyOfPropertyChange(() => Center);
            }
        }

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

        /// <summary>
        /// checks if image is loading
        /// </summary>
        public bool IsFileLoading
        {
            get { return this.isFileLoading; }
            set { this.isFileLoading = value; NotifyOfPropertyChange(() => IsFileLoading); }
        }


        /// <summary>
        /// Collection of all chronostratigraphic units
        /// </summary>
        public BindableCollection<tblUnionChronostratigraphy> Chronostratigraphy
        {
            get { return this.chronostratigraphy; }
            set { this.chronostratigraphy = value; NotifyOfPropertyChange(() => Chronostratigraphy); }
        }

        /// <summary>
        /// lithostratigraphic units based on the selected 
        /// </summary>
        public BindableCollection<tblObjectLithostratigraphy> OoiLithostrat
        {
            get
            {
                try
                {
                    return this.OoiLithostrat;
                }
                catch
                {
                    return new BindableCollection<tblObjectLithostratigraphy>();
                }
            }
            set
            {
                this.ooiLithostrat = value;
                NotifyOfPropertyChange(() => OoiLithostrat);
            }
        }


        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<LithostratigraphyUnion> Lithostratigraphy
        {
            get
            {
                return this.lithostratigraphy;
            }
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
                NotifyOfPropertyChange(() => CountObjects);
            }
        }

        /// <summary>
        /// Readonly count of the objects
        /// </summary>
        public string CountObjects
        {
            get
            {
                if (ObjectsOfInvestigation != null)
                    return ObjectsOfInvestigation.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountObjects);
            }
        }

        /// <summary>
        /// Collection of all documented facies types
        /// </summary>
        public BindableCollection<tblFacy> Facies
        {
            get { return this.facies; }
            set { this.facies = value; NotifyOfPropertyChange(() => Facies); }
        }

        /// <summary>
        /// Collection of all documented facies types
        /// </summary>
        public BindableCollection<tblArchitecturalElement> ArchitecturalElements
        {
            get { return this.architecturalElements; }
            set { this.architecturalElements = value; NotifyOfPropertyChange(() => ArchitecturalElements); }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedObjectIndex
        {
            get
            {
                if (SelectedObject != null)
                    return (ObjectsOfInvestigation.IndexOf(SelectedObject) + 1).ToString();

                return "0";
            }
            set
            {
                OnSelectedObjectIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedObjectIndex);
            }
        }


        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<tblMeasurement> FieldMeasurements
        {
            get { return this.fieldMeasurements; }
            set
            {
                this.fieldMeasurements = value;
                NotifyOfPropertyChange(() => FieldMeasurements);
            }
        }

        /// <summary>
        /// The selected laboratory measurement
        /// </summary>
        public tblMeasurement SelectedFieldMeasurement
        {
            get { return this.selectedFieldMeasurement; }
            set
            {
                try
                {
                    if (SelectedFieldMeasurement.measIdPk != 0)
                        Update();
                }
                catch { }

                this.selectedFieldMeasurement = value;

                if (value != null)
                    OnSelectedFieldMeasurementChanged();

                NotifyOfPropertyChange(() => IsTotalGammaRay);
                NotifyOfPropertyChange(() => IsSpectralGammaRay);
                NotifyOfPropertyChange(() => IsMagneticSusceptibility);
                NotifyOfPropertyChange(() => IsStructureOrientation);
                NotifyOfPropertyChange(() => IsTemperature);
                NotifyOfPropertyChange(() => IsRQD);
                NotifyOfPropertyChange(() => IsSonicLog);
                NotifyOfPropertyChange(() => SelectedFieldMeasurement);
            }
        }

        /// <summary>
        /// All selected Field measurements
        /// </summary>
        private BindableCollection<tblMeasurement> selectedFieldMeasurements = new BindableCollection<tblMeasurement>();
        public BindableCollection<tblMeasurement> SelectedFieldMeasurements
        {
            get => this.selectedFieldMeasurements;
            set
            {
                this.selectedFieldMeasurements = value;
                NotifyOfPropertyChange(() => SelectedFieldMeasurements);
            }
        }

        //Subproperties
        public tblTotalGammaRay TotalGammaRay { get { return this.totalGammaRay; } set { this.totalGammaRay = value; NotifyOfPropertyChange(() => TotalGammaRay); } }
        public tblSpectralGammaRay SpectralGammaRay { get { return this.spectralGammaRay; } set { this.spectralGammaRay = value; NotifyOfPropertyChange(() => SpectralGammaRay); } }
        public tblSusceptibility MagneticSusceptibility { get { return this.magneticSusceptibility; } set { this.magneticSusceptibility = value; NotifyOfPropertyChange(() => MagneticSusceptibility); } }
        public tblStructureOrientation StructureOrientation { get { return this.structureOrientation; } set { this.structureOrientation = value; NotifyOfPropertyChange(() => StructureOrientation); } }
        private tblRockQualityDesignationIndex rockQualityDesignationIndex;
        public tblRockQualityDesignationIndex RockQualityDesignationIndex { get => this.rockQualityDesignationIndex; set { this.rockQualityDesignationIndex = value; NotifyOfPropertyChange(() => RockQualityDesignationIndex); } }
        private tblBoreholeTemperature temperature;
        public tblBoreholeTemperature Temperature { get => this.temperature; set { this.temperature = value; NotifyOfPropertyChange(() => Temperature); } }
        private tblSonicLog sonicLog;
        public tblSonicLog SonicLog { get => this.sonicLog; set { this.sonicLog = value; NotifyOfPropertyChange(() => SonicLog); } }

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
                NotifyOfPropertyChange(() => CountFilesFieldMeasurement);
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
                NotifyOfPropertyChange(() => SelectedFileFieldMeasurementIndex);
                NotifyOfPropertyChange(() => SelectedFileStore);
            }
        }

        /// <summary>
        /// Readonly count of the objects
        /// </summary>
        public string CountFilesFieldMeasurement
        {
            get
            {
                if (FileStore != null)
                    return FileStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountFilesFieldMeasurement);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedFileFieldMeasurementIndex
        {
            get
            {
                if (SelectedFileStore != null)
                    return (FileStore.IndexOf(SelectedFileStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedFileFieldMeasurementIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedFileFieldMeasurementIndex);
            }
        }

        //The selected project
        public tblProject SelectedProject
        {
            get
            {
                if ((tblProject)((ShellViewModel)IoC.Get<IShell>()).SelectedProject != null)
                    return (tblProject)(tblProject)((ShellViewModel)IoC.Get<IShell>()).SelectedProject;

                return new tblProject();
            }
        }

        /// <summary>
        /// Loaded projects
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get
            {
                if ((BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).Projects != null)
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SelectedProjects;
                return new BindableCollection<tblProject>();
            }
        }

        //Visibility members dependent on the selected lab measurement
        public bool IsTotalGammaRay { get { try { return SelectedFieldMeasurement.measParameter == "Total Gamma Ray"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsTotalGammaRay); } }
        public bool IsSpectralGammaRay { get { try { return SelectedFieldMeasurement.measParameter == "Spectral Gamma Ray"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsSpectralGammaRay); } }
        public bool IsMagneticSusceptibility { get { try { return SelectedFieldMeasurement.measParameter == "Magnetic Susceptibility"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsMagneticSusceptibility); } }
        public bool IsStructureOrientation { get { try { return SelectedFieldMeasurement.measParameter == "Structure Orientation"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsStructureOrientation); } }
        public bool IsTemperature { get { try { return SelectedFieldMeasurement.measParameter == "Temperature"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsTemperature); } }
        public bool IsSonicLog { get { try { return SelectedFieldMeasurement.measParameter == "Sonic Log"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsSonicLog); } }
        public bool IsRQD { get { try { return SelectedFieldMeasurement.measParameter == "Rock Quality Designation Index"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsRQD); } }


        #region Chart view models

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

        #endregion

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

            Type = "Outcrop";
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public FieldMeasurementDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);

            Initialization = InitializeAsync();
        }

        /// <summary>
        /// A view model to filter the main data set
        /// </summary>
        private FilterDataSetViewModel<tblObjectOfInvestigation> filterDataSetViewModel = new FilterDataSetViewModel<tblObjectOfInvestigation>();
        public FilterDataSetViewModel<tblObjectOfInvestigation> FilterDataSetViewModel
        {
            get => this.filterDataSetViewModel;
            set
            {
                this.filterDataSetViewModel = value;
                NotifyOfPropertyChange(() => FilterDataSetViewModel);
            }
        }

        #endregion

        #region Methods

        //Loading the relevant data
        private async Task LoadData()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    try
                    {
                        using (var db1 = new ApirsRepository<tblFacy>())
                        {
                            Facies = new BindableCollection<tblFacy>(db1.GetFaciesByProject(Projects).ToList());
                        }
                        using (var db1 = new ApirsRepository<tblArchitecturalElement>())
                        {
                            ArchitecturalElements = new BindableCollection<tblArchitecturalElement>(db1.GetArchitecturalElementsByProject(Projects).ToList());
                        }
                    }
                    catch
                    {
                        ArchitecturalElements = new BindableCollection<tblArchitecturalElement>();
                        Facies = new BindableCollection<tblFacy>();

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
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(ex.Message + Environment.NewLine + ex.InnerException.Message);
                }
            });
        }

        /// <summary>
        /// Loading data based on an inpunt parameter
        /// </summary>
        /// <param name="parameter">Object type as string</param>
        public async void LoadData(string parameter, int id = 0)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                using (var db = new ApirsRepository<tblObjectOfInvestigation>())
                {
                    try
                    {
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db.GetModelByExpression(ooi => ooi.ooiType == parameter));

                        this.allObjectOfInvestigation = ObjectsOfInvestigation;

                        FilterDataSetViewModel = new FilterDataSetViewModel<tblObjectOfInvestigation>(allObjectOfInvestigation, ObjectsOfInvestigation, new ObservableCollection<string>(), false);

                        if (id == 0)
                            SelectedObject = ObjectsOfInvestigation.First();
                        else
                            SelectedObject = ObjectsOfInvestigation.Where(b => b.ooiIdPk == id).First();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                        this.allObjectOfInvestigation = ObjectsOfInvestigation;
                        SelectedObject = new tblObjectOfInvestigation();
                    }
                }
            });
        }

        //Loading related field measurement data
        private async void OnSelectedObjectChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
             {
                 try
                 {
                     using (var db = new ApirsRepository<tblObjectLithostratigraphy>())
                     {
                         OoiLithostrat = new BindableCollection<tblObjectLithostratigraphy>(db.GetModelByExpression(ooilith => ooilith.ooiIdFk == SelectedObject.ooiIdPk));
                     }

                     Center = new Location((double)SelectedObject.ooiLatitude, (double)SelectedObject.ooiLongitude);
                 }
                 catch
                 {
                     OoiLithostrat = new BindableCollection<tblObjectLithostratigraphy>();
                     Center = new Location(0, 0);
                 }
             });

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                using (var db = new ApirsRepository<tblMeasurement>())
                {
                    try
                    {
                        FieldMeasurements = new BindableCollection<tblMeasurement>(db.GetFieldMeasurementByProject(Projects, SelectedObject.ooiName));
                    }
                    catch
                    {
                        FieldMeasurements = new BindableCollection<tblMeasurement>();
                    }
                }


                if (FieldMeasurements.Count() > 0)
                    SelectedFieldMeasurement = FieldMeasurements.First();
                else
                    SelectedFieldMeasurement = new tblMeasurement();

            });
        }


        //Activating a background worker to select and download Files asynchronously
        private async void OnSelectedFieldMeasurementChanged()
        {
            try
            {
                TotalGammaRay = new tblTotalGammaRay();
                SpectralGammaRay = new tblSpectralGammaRay();
                MagneticSusceptibility = new tblSusceptibility();
                Temperature = new tblBoreholeTemperature();
                SonicLog = new tblSonicLog();
                rockQualityDesignationIndex = new tblRockQualityDesignationIndex();
                StructureOrientation = new tblStructureOrientation();

                switch (SelectedFieldMeasurement.measParameter)
                {
                    case "Total Gamma Ray":
                        TotalGammaRay = new ApirsRepository<tblTotalGammaRay>().GetModelByExpression(g => g.tgrfimeIdPk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Spectral Gamma Ray":
                        SpectralGammaRay = new ApirsRepository<tblSpectralGammaRay>().GetModelByExpression(sgr => sgr.sgrIdPk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Magnetic Susceptibility":
                        MagneticSusceptibility = new ApirsRepository<tblSusceptibility>().GetModelByExpression(ms => ms.susfimeIdPk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Temperature":
                        Temperature = new ApirsRepository<tblBoreholeTemperature>().GetModelByExpression(bt => bt.btfimeIdFk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Sonic Log":
                        SonicLog = new ApirsRepository<tblSonicLog>().GetModelByExpression(sl => sl.slfimeIdFk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Rock Quality Designation Index":
                        RockQualityDesignationIndex = new ApirsRepository<tblRockQualityDesignationIndex>().GetModelByExpression(rqd => rqd.rqdfimeIdFk == SelectedFieldMeasurement.measIdPk).First();
                        break;
                    case "Structure Orientation":
                        StructureOrientation = new ApirsRepository<tblStructureOrientation>().GetModelByExpression(so => so.sofimeIdFk == SelectedFieldMeasurement.measIdPk).First();
                        break;

                }

                try
                {
                    string tableName = new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableAlias.ToLower() == SelectedFieldMeasurement.measParameter.ToLower()).FirstOrDefault().alTableName;
                    Properties = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableName == tableName && x.alDataType == "Numerical"));
                    FilterProperties = new BindableCollection<tblAlia>(new ApirsRepository<tblAlia>().GetModelByExpression(x => x.alTableName == tableName && x.alDataType == "Categorical"));

                    if (Properties.Count() > 0)
                        SelectedProperty = Properties.First();
                    else
                        SelectedProperty = new tblAlia();

                    if (FilterProperties.Count() > 0)
                        SelectedFilterProperty = FilterProperties.First();
                    else
                        SelectedFilterProperty = new tblAlia();
                }

                catch
                {

                }

            }
            catch (Exception e)
            {
                TotalGammaRay = new tblTotalGammaRay();
                SpectralGammaRay = new tblSpectralGammaRay();
                MagneticSusceptibility = new tblSusceptibility();
                StructureOrientation = new tblStructureOrientation();
                Temperature = new tblBoreholeTemperature();
                RockQualityDesignationIndex = new tblRockQualityDesignationIndex();
                SonicLog = new tblSonicLog();
            }

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsFileLoading, async () =>
            {
                FileStore = await FileHelper.LoadFilesAsync(SelectedFieldMeasurement.measIdPk, "FieldMeasurement");
                SelectedFileStore = FileStore.Count > 0 ? FileStore.First() : new v_FileStore();
            });
        }

        //Event that is fired when the index changed
        private void OnSelectedObjectIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != ObjectsOfInvestigation.IndexOf(SelectedObject))
                {
                    SelectedObject = ObjectsOfInvestigation.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }


        /// <summary>
        /// Uploading a file to the database
        /// </summary>
        public void UploadFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId, SelectedFieldMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of Files to three
            if (Convert.ToInt32(CountFilesFieldMeasurement) > 1)
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

            using (var db = new ApirsRepository<tblFileFieldMeasurement>())
            {
                try
                {
                    db.InsertModel(new tblFileFieldMeasurement() { filName = fI.Name, fimeIdFk = SelectedFieldMeasurement.measIdPk, filStreamIdFk = id });
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }
            }

            OnSelectedFieldMeasurementChanged();
        }

        /// <summary>
        /// Deleting the currently selected File
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId, SelectedFieldMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the object
            try
            {
                if (SelectedFieldMeasurement.measUploaderId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            if (CountFilesFieldMeasurement == "0")
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occurred");
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Go to the last dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Last()
        {
            if (ObjectsOfInvestigation.Count != 0)
                SelectedObject = ObjectsOfInvestigation.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (ObjectsOfInvestigation.Count != 0)
                SelectedObject = Navigation.GetPrevious(ObjectsOfInvestigation, SelectedObject);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (ObjectsOfInvestigation.Count != 0)
                SelectedObject = Navigation.GetNext(ObjectsOfInvestigation, SelectedObject);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (ObjectsOfInvestigation.Count != 0)
                SelectedObject = ObjectsOfInvestigation.First();
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
        public override void Refresh()
        {

            tblObjectOfInvestigation current = SelectedObject;
            tblMeasurement currentFieldMeas = SelectedFieldMeasurement;

            try
            {
                Initialization = InitializeAsync();

            }
            catch
            {
                try
                {
                    LoadData(current.ooiType.ToString());
                }
                catch
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                }
            }
        }

        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        public void Add()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Add))
                {
                    return;
                }
                else if (SelectedProject.prjIdPk == 0)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a project first.");
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
                    tblMeasurement meas = new tblMeasurement()
                    {
                        measprjIdFk = SelectedProject.prjIdPk,
                        measObjectOfInvestigationIdFk = SelectedObject.ooiName,
                        measParameter = "Undefined",
                        measUploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId,
                        measType = "Field measurement"
                    };

                    db.InsertModel(meas);
                    FieldMeasurements.Add(meas);
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Measurement can't be added. Please check every field again.");
                    return;
                }
            }
        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Delete()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(SelectedFieldMeasurements.Count() + " values will be deleted. Are you sure to go on?",
                           "You won't be able to retrieve the related measurement data after deleting this measurement.") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblMeasurement>())
            {
                try
                {
                    foreach (tblMeasurement lab in SelectedFieldMeasurements)
                        db.DeleteModelById(lab.measIdPk);


                    OnSelectedObjectChanged();

                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(ex);
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
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId))
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
                using (var db = new ApirsRepository<tblMeasurement>())
                {
                    string resultType = new ApirsRepository<tblMeasurement>().GetModelByExpression(b => b.measIdPk == SelectedFieldMeasurement.measIdPk).Select(b => b.measParameter).First();

                    if (resultType != null)
                    {
                        if (resultType != SelectedFieldMeasurement.measParameter)
                        {
                            switch (resultType)
                            {
                                case "Total Gamma Ray":
                                    if (TotalGammaRay.tgrfimeIdPk != 0)
                                        new ApirsRepository<tblTotalGammaRay>().DeleteModelById(TotalGammaRay.tgrfimeIdPk);
                                    break;
                                case "Spectral Gamma Ray":
                                    if (SpectralGammaRay.sgrIdPk != 0)
                                        new ApirsRepository<tblSpectralGammaRay>().DeleteModelById(SpectralGammaRay.sgrIdPk);
                                    break;
                                case "Magnetic Susceptibility":
                                    if (MagneticSusceptibility.susfimeIdPk != 0)
                                        new ApirsRepository<tblSusceptibility>().DeleteModelById(MagneticSusceptibility.susfimeIdPk);
                                    break;
                                case "Sonic Log":
                                    if (SonicLog.slfimeIdFk != 0)
                                        new ApirsRepository<tblSonicLog>().DeleteModelById(SonicLog.slfimeIdFk);
                                    break;
                                case "Temperature":
                                    if (Temperature.btfimeIdFk != 0)
                                        new ApirsRepository<tblBoreholeTemperature>().DeleteModelById(Temperature.btfimeIdFk);
                                    break;
                                case "Rock Quality Designation Index":
                                    if (RockQualityDesignationIndex.rqdfimeIdFk != 0)
                                        new ApirsRepository<tblRockQualityDesignationIndex>().DeleteModelById(RockQualityDesignationIndex.rqdfimeIdFk);
                                    break;
                                case "Structure Orientation":
                                    if (StructureOrientation.sofimeIdFk != 0)
                                        new ApirsRepository<tblStructureOrientation>().DeleteModelById(StructureOrientation.sofimeIdFk);
                                    break;
                            }
                        }
                    }

                    db.UpdateModel(SelectedFieldMeasurement, SelectedFieldMeasurement.measIdPk);

                    switch (resultType)
                    {
                        case "Total Gamma Ray":
                            if (TotalGammaRay.tgrfimeIdPk == 0)
                            {
                                TotalGammaRay.tgrfimeIdPk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblTotalGammaRay>().InsertModel(TotalGammaRay);
                                NotifyOfPropertyChange(() => IsTotalGammaRay);
                            }
                            else
                            {
                                new ApirsRepository<tblTotalGammaRay>().UpdateModel(TotalGammaRay, TotalGammaRay.tgrfimeIdPk);
                            }
                            break;
                        case "Spectral Gamma Ray":
                            if (SpectralGammaRay.sgrIdPk == 0)
                            {
                                SpectralGammaRay.sgrIdPk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblSpectralGammaRay>().InsertModel(SpectralGammaRay);
                                NotifyOfPropertyChange(() => IsSpectralGammaRay);
                            }
                            else
                            {
                                new ApirsRepository<tblSpectralGammaRay>().UpdateModel(SpectralGammaRay, SpectralGammaRay.sgrIdPk);
                            }
                            break;
                        case "Magnetic Susceptibility":
                            if (MagneticSusceptibility.susfimeIdPk == 0)
                            {
                                MagneticSusceptibility.susfimeIdPk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblSusceptibility>().InsertModel(MagneticSusceptibility);
                                NotifyOfPropertyChange(() => IsMagneticSusceptibility);
                            }
                            else
                            {
                                new ApirsRepository<tblSusceptibility>().UpdateModel(MagneticSusceptibility, MagneticSusceptibility.susfimeIdPk);
                            }
                            break;
                        case "Sonic Log":
                            if (SonicLog.slfimeIdFk == 0)
                            {
                                SonicLog.slfimeIdFk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblSonicLog>().InsertModel(SonicLog);
                                NotifyOfPropertyChange(() => IsSonicLog);
                            }
                            else
                            {
                                new ApirsRepository<tblSonicLog>().UpdateModel(SonicLog, SonicLog.slfimeIdFk);
                            }
                            break;
                        case "Temperature":
                            if (Temperature.btfimeIdFk == 0)
                            {
                                Temperature.btfimeIdFk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblBoreholeTemperature>().InsertModel(Temperature);
                                NotifyOfPropertyChange(() => IsTemperature);
                            }
                            else
                            {
                                new ApirsRepository<tblBoreholeTemperature>().UpdateModel(Temperature, Temperature.btfimeIdFk);
                            }
                            break;
                        case "Rock Quality Designation Index":
                            if (RockQualityDesignationIndex.rqdfimeIdFk == 0)
                            {
                                RockQualityDesignationIndex.rqdfimeIdFk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblRockQualityDesignationIndex>().InsertModel(RockQualityDesignationIndex);
                                NotifyOfPropertyChange(() => IsRQD);
                            }
                            else
                            {
                                new ApirsRepository<tblRockQualityDesignationIndex>().UpdateModel(RockQualityDesignationIndex, RockQualityDesignationIndex.rqdfimeIdFk);
                            }
                            break;
                        case "Structure Orientation":
                            if (StructureOrientation.sofimeIdFk == 0)
                            {
                                StructureOrientation.sofimeIdFk = SelectedFieldMeasurement.measIdPk;
                                new ApirsRepository<tblStructureOrientation>().InsertModel(StructureOrientation);
                                NotifyOfPropertyChange(() => IsStructureOrientation);
                            }
                            else
                            {
                                new ApirsRepository<tblStructureOrientation>().UpdateModel(StructureOrientation, StructureOrientation.sofimeIdFk);
                            }
                            break;
                    }
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
                    else if (e.InnerException.Message.Contains("CHECK"))
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("Your provided values exceed the valid data ranges. Please review and try again.");
                    else
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured");
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                }
            }
        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedObject, (int)SelectedObject.ooiUploaderIdFk, SelectedFieldMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (SelectedObject != null)
                switch (SelectedObject.ooiType)
                {
                    case "Outcrop":
                        var type1 = new tblOutcrop();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, SelectedObject.ooiName));
                        break;
                    case "Drilling":
                        var type2 = new tblDrilling();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type2, SelectedObject.ooiName));
                        break;
                    case "Transect":
                        var type3 = new tblTransect();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type3, SelectedObject.ooiName));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a valid value");
            }
        }

        ///Event that gets fired if the filter text was changed
        public async Task Filter()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    FilterDataSetViewModel.Filter().AsResult();
                    ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(FilterDataSetViewModel.FilterDataSet);
                }
                finally
                {
                    if (!ObjectsOfInvestigation.Contains(SelectedObject))
                        SelectedObject = ObjectsOfInvestigation.First();
                }
            });
        }

        //Adding a participant to a project
        public void AddItem(string parameter)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Add))
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
                //Opening a dialog window with all available persons excluding those that already participate the project
                AnalyticalInstrumentDetailsViewModel analyticalInstrumentDetailsViewModel = new AnalyticalInstrumentDetailsViewModel(this._events);
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialog(analyticalInstrumentDetailsViewModel);

                Refresh();

            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
            }
        }
        //If an File file gets dropped on the File control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId, SelectedFieldMeasurement.measIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the object
            try
            {
                if (SelectedObject == null)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You must have registered object before uploading field measurement data.");
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
                var type1 = new tblMeasurement();
                _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, FileList[0]));
            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        //Exporting the actually selected list of objects to a csv file
        public void ExportList()
        {
            if (ObjectsOfInvestigation == null)
                return;
            if (ObjectsOfInvestigation.Count == 0)
                return;
            if (SelectedObject == null)
                SelectedObject = ObjectsOfInvestigation.First();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Exporting the list dependent on the sample type and the actual selection
                ExportHelper.ExportList<tblMeasurement>(FieldMeasurements, saveFileDialog.FileName, "All");
            }
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
                    case "Magnetic susceptibility":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblSusceptibility>(
                            new ImportProcedureViewModel<tblMeasurement, tblSusceptibility>(_events, table));
                        break;
                    case "Spectral gamma ray":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblSpectralGammaRay>(
                            new ImportProcedureViewModel<tblMeasurement, tblSpectralGammaRay>(_events, table));
                        break;
                    case "Structure orientation":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblStructureOrientation>(
                                 new ImportProcedureViewModel<tblMeasurement, tblStructureOrientation>(_events, table));
                        break;
                    case "Total gamma ray":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblTotalGammaRay>(
                                new ImportProcedureViewModel<tblMeasurement, tblTotalGammaRay>(_events, table));
                        break;
                    case "Sonic log":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblSonicLog>(
                                new ImportProcedureViewModel<tblMeasurement, tblSonicLog>(_events, table));
                        break;
                    case "Temperature":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblBoreholeTemperature>(
                                new ImportProcedureViewModel<tblMeasurement, tblBoreholeTemperature>(_events, table));
                        break;
                    case "Rock quality designation index":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblMeasurement, tblRockQualityDesignationIndex>(
                                new ImportProcedureViewModel<tblMeasurement, tblRockQualityDesignationIndex>(_events, table));
                        break;
                }

            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        /// <summary>
        /// If index is changed go to picture
        /// </summary>
        /// <param name="parameter"></param>
        private void OnSelectedFileFieldMeasurementIndexChanged(string parameter)
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a valid value");
            }
        }

        //Exporting a control
        public void ExportControl(object parameter)
        {
            _events.PublishOnUIThreadAsync(new ExportControlMessage((UIElement)parameter, "image"));
        }

        public void MapMeasurementToFaciesFromSection()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedFieldMeasurement, (int)SelectedFieldMeasurement.measUploaderId))
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
                List<tblMeasurement> fims = FieldMeasurements
                                                  .Where(fime => fime.measObjectOfInvestigationIdFk == SelectedFieldMeasurement.measObjectOfInvestigationIdFk
                                                                 && fime.measLatitude == SelectedFieldMeasurement.measLatitude
                                                                 && fime.measLongitude == SelectedFieldMeasurement.measLongitude)
                                                  .ToList();

                tblSection sec = new ApirsRepository<tblSection>().GetSectionByProject(Projects, "Lithological")
                           .Where(a => a.secOoiName == fims.First().measObjectOfInvestigationIdFk).MinBy(x => ((double)x.secLatitude - SelectedFieldMeasurement.measLatitude) + ((double)x.secLongitude - SelectedFieldMeasurement.measLongitude)).FirstOrDefault();


                Tuple<int, List<tblMeasurement>> tup = new ApirsRepository<tblSection>()
                        .MapFieldMeasurementToLithologicalSection(fims, sec, Projects);


                // If existing window is visible, delete the customer and all their orders.  
                // In a real application, you should add warnings and allow the user to cancel the operation.  
                if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(tup.Item1 + " values could be mapped. Do you want to proceed?",
                    "You won't be able to retrieve the original values of this set after confirmation.") == MessageBoxViewResult.Yes)
                {
                    try
                    {
                        using (var db = new ApirsRepository<tblMeasurement>())
                        {
                            foreach (tblMeasurement fime in fims)
                            {
                                db.UpdateModel(fime, fime.measIdPk);
                            };

                        }
                    }
                    catch
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
                    }
                }

            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
            }
            finally
            {
                OnSelectedObjectChanged();
            }

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
            }
        }

        #endregion

        #region Helper

        //Converts a bitmap to an image
        public static byte[] BitmapToByte(Bitmap btmp)
        {
            using (var stream = new MemoryStream())
            {
                btmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return stream.ToArray();
            }
        }


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
