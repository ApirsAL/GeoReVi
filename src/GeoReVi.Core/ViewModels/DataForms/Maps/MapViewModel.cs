using System;
using Caliburn.Micro;
using System.Linq;
using System.ComponentModel;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Threading.Tasks;
using DotSpatial.Data;

namespace GeoReVi
{
    public class MapViewModel : Screen,
        IHandle<FilterByLocationMessage>
    {
        #region Private members

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private MapStyle ms;

        //Filter text
        private string filterText;
        //Coordinates string
        private string coordinates;

        /// <summary>
        /// The data collections
        /// </summary>
        private BindableCollection<tblRockSample> rockSamples;
        private BindableCollection<tblMeasurement> laboratoryMeasurements;
        private BindableCollection<tblObjectOfInvestigation> objectsOfInvestigation;
        private BindableCollection<DrillingJoin> drillings;
        private BindableCollection<OutcropJoin> outcrops;
        private BindableCollection<tblTransect> transects;
        private BindableCollection<tblMeasurement> palaeoFlow;
        private BindableCollection<tblCountry> countries;

        /// <summary>
        /// The all data collections
        /// </summary>
        private BindableCollection<tblRockSample> allRockSamples;
        private BindableCollection<tblMeasurement> allLaboratoryMeasurements;
        private BindableCollection<tblObjectOfInvestigation> allObjectsOfInvestigation;
        private BindableCollection<DrillingJoin> allDrillings;
        private BindableCollection<OutcropJoin> allOutcrops;

        /// <summary>
        /// Visibility booleans
        /// </summary>
        private bool rockSamplesVisible = false;
        private bool laboratoryMeasurementsVisible = false;
        private bool drillingsVisible = false;
        private bool outcropsVisible = false;
        private bool transectsVisible = false;
        private bool palaeoFlowVisible = false;

        //Tokensource
        CancellationTokenSource cts;

        //Layer treeview class for the local shapes
        private BindableCollection<LayerModel> localShapes = new BindableCollection<LayerModel>();
        #endregion

        #region Public properties

        ///<summary>
        ///The associated bing key 
        /// </summary>
        private string bingKey = "";
        public string BingKey
        {
            get
            {
                return this.bingKey;
            }
            set
            {
                this.bingKey = value;
                NotifyOfPropertyChange(() => BingKey);
            }
        }

        /// <summary>
        /// Mapstyle singleton
        /// </summary>
        public MapStyle Ms
        {
            get => this.ms;
            private set
            {
                NotifyOfPropertyChange(() => Ms);
            }
        }

        /// <summary>
        ///Object collections
        /// </summary>
        public BindableCollection<tblRockSample> RockSamples
        {
            get
            {
                return this.rockSamples;
            }
            set
            {
                this.rockSamples = value;
                NotifyOfPropertyChange(() => RockSamples);
                OnCollectionOrVisibilityChanged("RockSample");
            }
        }
        public BindableCollection<tblMeasurement> LaboratoryMeasurements
        {
            get
            {
                return this.laboratoryMeasurements;
            }
            set
            {
                this.laboratoryMeasurements = value;
                NotifyOfPropertyChange(() => RockSamples);
            }
        }
        public BindableCollection<DrillingJoin> Drillings
        {
            get
            {
                return this.drillings;
            }
            set
            {
                this.drillings = value;
                NotifyOfPropertyChange(() => Drillings);
                OnCollectionOrVisibilityChanged("Drillings");
            }
        }
        public BindableCollection<OutcropJoin> Outcrops
        {
            get
            {
                return this.outcrops;
            }
            set
            {
                this.outcrops = value;
                NotifyOfPropertyChange(() => Outcrops);
                OnCollectionOrVisibilityChanged("Outcrops");
            }
        }
        public BindableCollection<tblTransect> Transects
        {
            get
            {
                return this.transects;
            }
            set
            {
                this.transects = value;
                NotifyOfPropertyChange(() => Transects);
                OnCollectionOrVisibilityChanged("Transects");
            }
        }
        public BindableCollection<tblObjectOfInvestigation> ObjectsOfInvestigation
        {
            get
            {
                return this.objectsOfInvestigation;
            }
            set
            {
                this.objectsOfInvestigation = value;
                NotifyOfPropertyChange(() => ObjectsOfInvestigation);
            }
        }
        public BindableCollection<tblCountry> Countries
        {
            get
            {
                return this.countries;
            }
        }
        public BindableCollection<tblMeasurement> PalaeoFlow
        {
            get
            {
                return this.palaeoFlow;
            }
            set
            {
                this.palaeoFlow = value;
                NotifyOfPropertyChange(() => PalaeoFlow);
                OnCollectionOrVisibilityChanged("PalaeoFlow");
            }
        }
        
        /// <summary>
        /// All objects collection
        /// </summary>
        public BindableCollection<tblRockSample> AllRockSamples
        {
            get
            {
                return this.allRockSamples;
            }
            set
            {
                this.allRockSamples = value;
                NotifyOfPropertyChange(() => AllRockSamples);
            }
        }
        public BindableCollection<tblMeasurement> AllLaboratoryMeasurements
        {
            get
            {
                return this.allLaboratoryMeasurements;
            }
            set
            {
                this.allLaboratoryMeasurements = value;
                NotifyOfPropertyChange(() => AllLaboratoryMeasurements);
            }
        }
        public BindableCollection<DrillingJoin> AllDrillings
        {
            get
            {
                return this.allDrillings;
            }
            set
            {
                this.allDrillings = value;
                NotifyOfPropertyChange(() => AllDrillings);
            }
        }
        public BindableCollection<OutcropJoin> AllOutcrops
        {
            get
            {
                return this.allOutcrops;
            }
            set
            {
                this.allOutcrops = value;
                NotifyOfPropertyChange(() => AllOutcrops);
            }
        }
        public BindableCollection<tblObjectOfInvestigation> AllObjectsOfInvestigation
        {
            get
            {
                return this.allObjectsOfInvestigation;
            }
            set
            {
                this.allObjectsOfInvestigation = value;
                NotifyOfPropertyChange(() => AllObjectsOfInvestigation);
            }
        }
        public BindableCollection<tblMeasurement> AllPalaeoFlows
        {
            get;
            set;
        }

        //Check if the objects are already displayed in the map
        public bool RockSamplesVisible
        {
            get
            {
                return this.rockSamplesVisible;
            }
            set
            {
                this.rockSamplesVisible = value;
                NotifyOfPropertyChange(() => RockSamplesVisible);
                OnCollectionOrVisibilityChanged("RockSample");
            }
        }
        public bool LaboratoryMeasurementsVisible
        {
            get => this.laboratoryMeasurementsVisible;
            set
            {
                this.laboratoryMeasurementsVisible = value;
                NotifyOfPropertyChange(() => LaboratoryMeasurementsVisible);
            }
        }
        public bool OutcropsVisible
        {
            get
            {
                return this.outcropsVisible;
            }
            set
            {
                this.outcropsVisible = value;
                NotifyOfPropertyChange(() => OutcropsVisible);
                OnCollectionOrVisibilityChanged("Outcrops");
            }
        }
        public bool TransectsVisible
        {
            get
            {
                return this.transectsVisible;
            }
            set
            {
                this.transectsVisible = value;
                NotifyOfPropertyChange(() => TransectsVisible);
                OnCollectionOrVisibilityChanged("Transects");
            }
        }
        public bool PalaeoFlowVisible
        {
            get
            {
                return this.palaeoFlowVisible;
            }
            set
            {
                this.palaeoFlowVisible = value;
                NotifyOfPropertyChange(() => PalaeoFlowVisible);
                OnCollectionOrVisibilityChanged("PalaeoFlow");
            }
        }
        public bool DrillingsVisible
        {
            get { return this.drillingsVisible; }
            set
            {
                this.drillingsVisible = value;
                NotifyOfPropertyChange(() => DrillingsVisible);
                OnCollectionOrVisibilityChanged("Drillings");
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
        /// All projects, the user acutally participates
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get
            {
                if ((BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).Projects != null)
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).Projects;
                return new BindableCollection<tblProject>();
            }
        }

        //Layer treeview class for the local shapes
        public BindableCollection<LayerModel> LocalShapes
        {
            get => this.localShapes;
            set
            {
                this.localShapes = value;
                NotifyOfPropertyChange(() => LocalShapes);
            }
        }

        /// <summary>
        /// Filter text
        /// </summary>
        public string FilterText
        {
            get
            {
                return this.filterText;
            }
            set
            {
                this.filterText = value;
                OnFilterTextChanged();
                NotifyOfPropertyChange(() => FilterText);
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="map">The actual Bing map object</param>
        public MapViewModel()
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="map">The actual Bing map object</param>
        public MapViewModel(MapStyle _ms, IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            ms = _ms;
            LoadData();
        }

        /// <summary>
        /// Loading the relevant data
        /// </summary>
        private async void LoadData()
        {
            CommandHelper ch = new CommandHelper();
            object key = new object();
            _events.PublishOnUIThreadAsync(new IsLoadingMessage(key));

            await ch.RunCommand(() => Ms.Updating, async () =>
            {
                await Task.Delay(1000);
                //IEnumerable<tblCountry> countryQuery;


                try
                {
                    LaboratoryMeasurements = new BindableCollection<tblMeasurement>();

                    switch (SelectedProject.prjName)
                    {
                        case "All projects":
                            try
                            {
                                using (var db = new ApirsRepository<tblRockSample>())
                                {
                                    RockSamples = new BindableCollection<tblRockSample>(db.GetAllRockSamplesByProject(Projects).ToList());
                                    AllRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                                }
                            }
                            catch
                            {
                                RockSamples = new BindableCollection<tblRockSample>();
                                AllRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                            }
                            try
                            {
                                using (var db = new ApirsRepository<tblMeasurement>())
                                {
                                    //PalaeoFlow = new BindableCollection<tblFieldMeasurement>(db.GetStructureOrientationByProjectsAndType(Projects, "Palaeo flow axis").ToList());
                                    AllPalaeoFlows = new BindableCollection<tblMeasurement>(PalaeoFlow);
                                }
                            }
                            catch(Exception palex)
                            {
                                PalaeoFlow = new BindableCollection<tblMeasurement>();
                                AllPalaeoFlows = new BindableCollection<tblMeasurement>(PalaeoFlow);
                            }
                            break;
                        default:
                            try
                            {
                                using (var db = new ApirsRepository<tblRockSample>())
                                {
                                    //Selecting all rock samples related to the selected project
                                    RockSamples = new BindableCollection<tblRockSample>(db.GetModelByExpression(x => x.sampprjIdFk == SelectedProject.prjIdPk).ToList());
                                    AllRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                                }
                            }
                            catch
                            {
                                RockSamples = new BindableCollection<tblRockSample>();
                                AllRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                            }
                            try
                            {
                                using (var db = new ApirsRepository<tblMeasurement>())
                                {
                                    //Selecting all rock samples related to the selected project
                                    //PalaeoFlow = new BindableCollection<tblFieldMeasurement>(db.GetStructureOrientationByProjectsAndType(new List<tblProject>() { SelectedProject }, "Palaeo flow axis").ToList());
                                    AllPalaeoFlows = new BindableCollection<tblMeasurement>(PalaeoFlow);
                                }
                            }
                            catch
                            {
                                PalaeoFlow = new BindableCollection<tblMeasurement>();
                                AllPalaeoFlows = new BindableCollection<tblMeasurement>(PalaeoFlow);
                            }
                            break;
                    }

                    try
                    {
                        using (var db = new ApirsRepository<tblObjectOfInvestigation>())
                        {
                            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db.GetModel().ToList());
                            AllObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(ObjectsOfInvestigation);
                        }
                    }
                    catch
                    {
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                        AllObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(ObjectsOfInvestigation);
                    }

                    try
                    {
                        using (var db = new ApirsRepository<tblDrilling>())
                        {
                            Drillings = new BindableCollection<DrillingJoin>(db.GetAllDrillings());
                            AllDrillings = new BindableCollection<DrillingJoin>(Drillings);
                        }
                    }
                    catch(Exception e)
                    {
                        Drillings = new BindableCollection<DrillingJoin>();
                        AllDrillings = new BindableCollection<DrillingJoin>(Drillings);
                    }
                    try
                    {
                        using (var db = new ApirsRepository<tblOutcrop>())
                        {
                            Outcrops = new BindableCollection<OutcropJoin>(db.GetAllOutcrops());
                            AllOutcrops = new BindableCollection<OutcropJoin>(Outcrops);
                        }
                    }
                    catch(Exception e)
                    {
                        Outcrops = new BindableCollection<OutcropJoin>();
                    }
                    try
                    {
                        using (var db = new ApirsRepository<tblTransect>())
                        {
                            Transects = new BindableCollection<tblTransect>(db.GetModel());
                        }
                    }
                    catch (Exception e)
                    {
                        Transects = new BindableCollection<tblTransect>();
                    }
                    //this.countries = new BindableCollection<tblCountry>(countryQuery);
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("No network connection.");
                    RockSamples = new BindableCollection<tblRockSample>();
                    LaboratoryMeasurements = new BindableCollection<tblMeasurement>();
                    ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                    Drillings = new BindableCollection<DrillingJoin>();
                    Outcrops = new BindableCollection<OutcropJoin>();
                }
                finally
                {
                }

                LocalShapes.Add(new LayerModel() { Name = "Test", Objects = new object() });
                LocalShapes.First().Children.Add(new LayerModel() { Name = "Test2", Objects = new object() });
            });

            _events.PublishOnUIThreadAsync(new IsLoadingMessage(key));
        }
        #endregion

        #region Public methods

        /// <summary>
        /// Adding the pushpins in the pushpinlist to the map
        /// </summary>
        public async void AddElementToMap(string element, bool value = true)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunCommand(() => Ms.Updating, async () =>
            {
                await Task.Delay(1000);

                switch (element)
                {
                    case "RockSample":
                        ms.AddElementToMap(RockSamples);
                        break;
                    case "Drillings":
                        ms.AddElementToMap(Drillings);
                        break;
                    case "Outcrops":
                        ms.AddElementToMap(Outcrops);
                        break;
                    case "PalaeoFlow":
                        ms.AddElementToMap(PalaeoFlow, "PalaeoFlow");
                        break;
                }
            });
        }

        /// <summary>
        /// Opens the file where the individual bing map key can be inserted
        /// </summary>
        /// <returns></returns>
        public async Task OpenBingMapFile()
        {
            try
            {
                System.Diagnostics.Process.Start(@"Media\Data\K.csv");
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
            }
        }

        #endregion

        #region Helper
        private bool HasValue(double value)
        {
            return !Double.IsNaN(value) && !Double.IsInfinity(value);
        }

        /// <summary>
        /// Fired if visibility or collection changed
        /// </summary>
        /// <param name="parameter"></param>
        private void OnCollectionOrVisibilityChanged(string parameter)
        {
            switch (parameter)
            {
                case "RockSample":

                    ms.RemoveMapPushpinByName("RockSample");
                    if (RockSamplesVisible)
                    {
                        ms.AddElementToMap(RockSamples);
                    }
                    break;
                case "Drillings":

                    ms.RemoveMapPushpinByName("Drillings");
                    if (DrillingsVisible)
                    {
                        ms.AddElementToMap(Drillings);
                    }
                    break;
                case "Outcrops":

                    ms.RemoveMapPushpinByName("Outcrops");

                    if (OutcropsVisible)
                    {
                        ms.AddElementToMap(Outcrops);
                    }
                    break;
                case "Transects":

                    ms.RemoveMapPolylineByName("Transects");

                    if (TransectsVisible)
                    {
                        ms.AddElementToMap(Transects);
                    }
                    break;
                case "PalaeoFlow":
                    ms.RemoveMapPushpinByName("PalaeoFlow");

                    if (PalaeoFlowVisible)
                    {
                        ms.AddElementToMap(PalaeoFlow, "PalaeoFlow");
                    }
                    break;
            }
        }

        public void Handle(FilterByLocationMessage message)
        {
            //Filtering data based on the selection
            try
            {
                ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(CollectionHelper.FilterByCoordinates<tblObjectOfInvestigation>(ObjectsOfInvestigation,
                                                                                                                    message.SelectionStartLocation.Latitude,
                                                                                                                    message.SelectionStartLocation.Longitude,
                                                                                                                    message.SelectionEndLocation.Latitude,
                                                                                                                    message.SelectionEndLocation.Longitude).ToList());
                //Filtering drillings
                Drillings = new BindableCollection<DrillingJoin>(CollectionHelper.FilterByCoordinates<DrillingJoin>(Drillings,
                                                                                                                    message.SelectionStartLocation.Latitude,
                                                                                                                    message.SelectionStartLocation.Longitude,
                                                                                                                    message.SelectionEndLocation.Latitude,
                                                                                                                    message.SelectionEndLocation.Longitude).ToList());


                //Filtering outcrops
                Outcrops = new BindableCollection<OutcropJoin>(CollectionHelper.FilterByCoordinates<OutcropJoin>(Outcrops,
                                                                                                                    message.SelectionStartLocation.Latitude,
                                                                                                                    message.SelectionStartLocation.Longitude,
                                                                                                                    message.SelectionEndLocation.Latitude,
                                                                                                                    message.SelectionEndLocation.Longitude).ToList());
                //Filtering objects of investigation

                //Filtering rock samples
                RockSamples = new BindableCollection<tblRockSample>(CollectionHelper.FilterByCoordinates<tblRockSample>(RockSamples,
                                                                                                                    message.SelectionStartLocation.Latitude,
                                                                                                                    message.SelectionStartLocation.Longitude,
                                                                                                                    message.SelectionEndLocation.Latitude,
                                                                                                                    message.SelectionEndLocation.Longitude).ToList());
            }
            catch(Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
            }
        }

        public void AddShapefile()
        {
            Shapefile shape = new Shapefile();


        }

        /// <summary>
        /// Refreshing data and the map
        /// </summary>
        public void RefreshData()
        {
            RockSamples = new BindableCollection<tblRockSample>(AllRockSamples);
            Drillings = new BindableCollection<DrillingJoin>(AllDrillings);
            Outcrops = new BindableCollection<OutcropJoin>(AllOutcrops);
            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(AllObjectsOfInvestigation);

            RockSamplesVisible = false;
            DrillingsVisible = false;
            OutcropsVisible = false;
            PalaeoFlowVisible = false;
        }

        /// <summary>
        /// Creates a png file of the map control
        /// </summary>
        public void DownloadMap()
        {
            ///Configuring the savefiledialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG (*.png)|*.png |BMP (*.bmp)|*.bmp";
            saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Downloading to the specific folder
                ImageCapturer.SaveToPng(ms.BingMap, saveFileDialog.FileName);
            }
        }

        #endregion

        #region Helper

        ///Event that gets fired if the filter text was changed
        private void OnFilterTextChanged()
        {
            if (FilterText == "")
            {
                RockSamples = new BindableCollection<tblRockSample>(AllRockSamples);
                Drillings = new BindableCollection<DrillingJoin>(AllDrillings);
                Outcrops = new BindableCollection<OutcropJoin>(AllOutcrops);
                ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(AllObjectsOfInvestigation);
            }

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += ((sender1, args) =>
            {

                new DispatchService().Invoke(
                () =>
                {
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts = null;
                    }
                    cts = new CancellationTokenSource();

                    //Filtering data based on the selection
                    try
                    {
                        //Filtering outcrops

                        Outcrops = new BindableCollection<OutcropJoin>(CollectionHelper.Filter<OutcropJoin>(AllOutcrops, FilterText));

                        //Filtering drillings
                        Drillings = new BindableCollection<DrillingJoin>(CollectionHelper.Filter<DrillingJoin>(AllDrillings, FilterText));

                        //Filtering rock samples
                        RockSamples = new BindableCollection<tblRockSample>(CollectionHelper.Filter<tblRockSample>(AllRockSamples, FilterText));

                        //Filtering all objects
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(CollectionHelper.Filter<tblObjectOfInvestigation>(AllObjectsOfInvestigation, FilterText));
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            //Filtering outcrops
                            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>((from rs in AllObjectsOfInvestigation
                                                                                                       where CollectionHelper.NullToString(rs.ooiName).ToLower().Contains(FilterText)
                                                                                                       select rs).ToList());
                        }
                        catch
                        {
                            _events.PublishOnUIThread(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));;
                        }
                    }
                });

            });

            bw.RunWorkerCompleted += ((sender1, args) =>
            {
                if (args.Error != null)  // if an exception occurred during DoWork,
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

            });

            bw.RunWorkerAsync(); // start the background worker

        }

        //Exporting the actually selected list of objects to a csv file
        public void ExportList(string parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                switch (parameter)
                {
                    case "Objects":
                        ExportHelper.ExportList<tblObjectOfInvestigation>(ObjectsOfInvestigation, saveFileDialog.FileName, "All");
                        break;
                    case "RockSamples":
                        ExportHelper.ExportList<tblObjectOfInvestigation>(ObjectsOfInvestigation, saveFileDialog.FileName, "All");
                        break;
                }
            }
        }

        #endregion
    }
}

