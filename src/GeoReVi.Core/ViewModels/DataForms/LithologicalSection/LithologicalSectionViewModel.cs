using Caliburn.Micro;
using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    /// <summary>
    /// A view model that displays the available lithological sections
    /// </summary>
    public class LithologicalSectionViewModel : Screen
    {
        #region Private members

        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;

        //Lithostrat collection
        private BindableCollection<tblUnionLithostratigraphy> lithostratigraphy;

        //Object lithostrat collection
        private BindableCollection<tblObjectLithostratigraphy> ooiLithostrat;

        //Facies collection
        private BindableCollection<tblFacy> facies;

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

        //Cancellation token for async methods
        CancellationTokenSource cts;

        /// <summary>
        /// Selected objects
        /// </summary>
        private tblObjectOfInvestigation selectedObject;

        /// <summary>
        /// Field measurement collections and objects
        /// </summary>
        private BindableCollection<tblSection> lithologicalSections;
        private tblSection selectedLithologicalSection;

        //Subproperties
        private BindableCollection<tblSectionLithofacy> faciesBeds;
        private tblSectionLithofacy selectedFaciesBed;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Checks if data collection is updating
        /// </summary>
        private bool updating = false;

        //Text filter variable
        private string textFilter;
        private string type;

        /// <summary>
        /// The view model to display the lithological section
        /// </summary>
        private SectionChartViewModel sectionChartViewModel = new SectionChartViewModel();
        public SectionChartViewModel SectionChartViewModel
        {
            get => this.sectionChartViewModel;
            set
            {
                this.sectionChartViewModel = value;
                NotifyOfPropertyChange(() => SectionChartViewModel);
            }
        }

        #endregion

        #region Public properties

        //Getting the object type
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

                try
                {
                    Center = new Location((double)SelectedObject.ooiLatitude, (double)SelectedObject.ooiLongitude);
                }
                catch
                {
                    Center = new Location(0, 0);
                }

                NotifyOfPropertyChange(() => SelectedObject);
                NotifyOfPropertyChange(() => SelectedObjectIndex);
            }
        }

        public bool Updating
        {
            get => this.updating;
            set
            {
                this.updating = value;
                NotifyOfPropertyChange(() => Updating);
            }
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

        /// <summary>
        /// Collection of all chronostratigraphic units
        /// </summary>
        public BindableCollection<tblUnionChronostratigraphy> Chronostratigraphy
        {
            get { return this.chronostratigraphy; }
            private set
            {
                this.chronostratigraphy = value;
                NotifyOfPropertyChange(() => Chronostratigraphy);
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
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<tblUnionLithostratigraphy> Lithostratigraphy
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
        public BindableCollection<tblSection> LithologicalSections
        {
            get { return this.lithologicalSections; }
            set
            {
                this.lithologicalSections = value;
                NotifyOfPropertyChange(() => LithologicalSections);
            }
        }

        /// <summary>
        /// The selected laboratory measurement
        /// </summary>
        public tblSection SelectedLithologicalSection
        {
            get
            {
                return this.selectedLithologicalSection;
            }
            set
            {
                try
                {
                    if (SelectedLithologicalSection.secIdPk != 0)
                        Update();
                }
                catch { }
                this.selectedLithologicalSection = value;

                if (value != null)
                {
                    OnSelectedLithologicalSectionChanged();
                }

                NotifyOfPropertyChange(() => IsLithologicalSectionSelected);
                NotifyOfPropertyChange(() => SelectedLithologicalSection);
            }
        }

        //Check if lithological section is selected
        public bool IsLithologicalSectionSelected
        {
            get
            {
                try
                {
                    return SelectedLithologicalSection.secIdPk != 0;
                }
                catch
                {
                    return false;
                };
            }
            set
            {
                NotifyOfPropertyChange(() => IsLithologicalSectionSelected);
            }
        }


        //The section beds
        public BindableCollection<tblSectionLithofacy> FaciesBeds
        {
            get
            {
                return this.faciesBeds;
            }
            set
            {
                this.faciesBeds = value;

                NotifyOfPropertyChange(() => FaciesBeds);
            }
        }

        //The section units
        private BindableCollection<tblLithoStratigraphySection> lithostratigraphicUnits;
        public BindableCollection<tblLithoStratigraphySection> LithostratigraphicUnits
        {
            get
            {
                return this.lithostratigraphicUnits;
            }
            set
            {
                this.lithostratigraphicUnits = value;

                NotifyOfPropertyChange(() => LithostratigraphicUnits);
            }
        }
        //The selected facies bed
        public tblSectionLithofacy SelectedFaciesBed
        {
            get
            {
                return this.selectedFaciesBed;
            }
            set
            {
                if (SelectedFaciesBed != null && SelectedFaciesBed.litsecIdFk != 0)
                    this.selectedFaciesBed = value;

                NotifyOfPropertyChange(() => SelectedFaciesBed);
            }
        }

        //The selected facies bed
        private BindableCollection<tblSectionLithofacy> selectedFaciesBeds;
        public BindableCollection<tblSectionLithofacy> SelectedFaciesBeds
        {
            get
            {
                return this.selectedFaciesBeds;
            }
            set
            {
                this.selectedFaciesBeds = value;

                NotifyOfPropertyChange(() => SelectedFaciesBeds);
            }
        }

        //The selected facies bed
        private tblLithoStratigraphySection selectedLithostratigraphicUnit;
        public tblLithoStratigraphySection SelectedLithostratigraphicUnit
        {
            get
            {
                return this.selectedLithostratigraphicUnit;
            }
            set
            {
                if (value != null && value.lithosecIdPk != 0)
                    this.selectedLithostratigraphicUnit = value;

                NotifyOfPropertyChange(() => SelectedLithostratigraphicUnit);
            }
        }

        private FilesListViewModel<tblSection> filesListViewModel;
        public FilesListViewModel<tblSection> FilesListViewModel
        {
            get => this.filesListViewModel;
            set
            {
                this.filesListViewModel = value;
                NotifyOfPropertyChange(() => FilesListViewModel);
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
                if ((BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SelectedProjects != null)
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SelectedProjects;
                return new BindableCollection<tblProject>();
            }
        }

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

        //Petrophysical measurement values
        private BindableCollection<KeyValuePair<string, DataTable>> petrophysics;
        public BindableCollection<KeyValuePair<string, DataTable>> Petrophysics
        {
            get => this.petrophysics;
            set
            {
                this.petrophysics = value;
                NotifyOfPropertyChange(() => Petrophysics);
            }
        }

        private BindableCollection<string> dataTableColumnNames;
        public BindableCollection<string> DataTableColumnNames
        {
            get => this.dataTableColumnNames;
            set
            {
                this.dataTableColumnNames = value;
                NotifyOfPropertyChange(() => DataTableColumnNames);
            }
        }

        private bool drawWhileInput = true;
        public bool DrawWhileInput
        {
            get => drawWhileInput;
            set
            {
                drawWhileInput = value;
                NotifyOfPropertyChange(() => DrawWhileInput);
            }
        }

        //The import type for the sections
        public string ImportType { get; set; }

        //Section id where beds should be moved to
        public int MoveToSectionId { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public LithologicalSectionViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            LoadData();
            FilesListViewModel = new FilesListViewModel<tblSection>(SelectedLithologicalSection);
            SelectedFaciesBeds = new BindableCollection<tblSectionLithofacy>();
            Type = "Outcrop";
        }

        #endregion

        #region Methods

        //Loading the relevant data
        private void LoadData()
        {

            try
            {

                Lithostratigraphy = new BindableCollection<tblUnionLithostratigraphy>();

                try
                {
                    Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(new ApirsRepository<tblUnionChronostratigraphy>().GetModel().ToList());
                }
                catch (Exception e)
                {
                    Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                }
                try
                {
                     Facies = new BindableCollection<tblFacy>(new ApirsRepository<tblFacy>().GetFaciesByProject(Projects));
                }
                catch
                {
                    Facies = new BindableCollection<tblFacy>();
                }
            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(ex.Message + Environment.NewLine + ex.InnerException.Message);
            }

        }

        /// <summary>
        /// Loading data based on an inpunt parameter
        /// </summary>
        /// <param name="parameter">Object type as string</param>
        public async Task LoadData(string parameter, int id = 0)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                using (var db = new ApirsRepository<tblFacy>())
                {
                    try
                    {

                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(new ApirsRepository<tblObjectOfInvestigation>().GetModelByExpression(ooi => ooi.ooiType == parameter).ToList());
                        this.allObjectOfInvestigation = ObjectsOfInvestigation;

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
        private void OnSelectedObjectChanged()
        {

            using (var db = new ApirsRepository<tblSection>())
            {
                try
                {
                    LithologicalSections = new BindableCollection<tblSection>(db.GetSectionByProject(Projects, "Lithological").Where(sec => sec.secOoiName == SelectedObject.ooiName).OrderBy(x => x.secIdPk));

                    if (LithologicalSections.Count() > 0)
                        SelectedLithologicalSection = LithologicalSections.First();
                    else
                        SelectedLithologicalSection = new tblSection() { secInterpreterIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }
                catch
                {
                    LithologicalSections = new BindableCollection<tblSection>();
                    SelectedLithologicalSection = new tblSection() { secInterpreterIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }
            }
        }

        //Activating a background worker to select and download Files asynchronously
        private async void OnSelectedLithologicalSectionChanged()
        {

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {

                try
                {
                    int ooi = new ApirsRepository<tblObjectOfInvestigation>().GetModelByExpression(x => x.ooiName == SelectedLithologicalSection.secOoiName).Select(x => x.ooiIdPk).FirstOrDefault();
                    List<int> lithostrat = new ApirsRepository<tblOOILithostrat>().GetModelByExpression(y => y.ooiIdFk == ooi).Select(x=>x.lithID).ToList();
                    Lithostratigraphy = new BindableCollection<tblUnionLithostratigraphy>(new ApirsRepository<tblUnionLithostratigraphy>().GetModelByExpression(x => lithostrat.Contains(x.ID)).ToList());
                }
                catch
                {
                    Lithostratigraphy = new BindableCollection<tblUnionLithostratigraphy>();
                }

                using (var db = new ApirsRepository<tblSectionLithofacy>())
                {
                    try
                    {
                        FaciesBeds = new BindableCollection<tblSectionLithofacy>();

                        switch (SelectedLithologicalSection.secType)
                        {
                            case "Lithological":
                                FaciesBeds = new BindableCollection<tblSectionLithofacy>(db.GetModelByExpression(a => a.litsecIdFk == SelectedLithologicalSection.secIdPk).OrderBy(x => x.litsecBase).ToList());

                                try
                                {
                                    SelectedFaciesBed = FaciesBeds.First();
                                }
                                catch
                                {
                                    SelectedFaciesBed = new tblSectionLithofacy();
                                }
                                break;

                        }

                        try
                        {
                            Center = new Location((double)SelectedLithologicalSection.secLatitude, (double)SelectedLithologicalSection.secLongitude);
                        }
                        catch
                        {
                            Center = new Location((double)SelectedObject.ooiLatitude, (double)SelectedObject.ooiLongitude);
                        }
                    }
                    catch (Exception e)
                    {
                        FaciesBeds = new BindableCollection<tblSectionLithofacy>();
                        SelectedFaciesBed = new tblSectionLithofacy();
                    }
                }

                using (var db = new ApirsRepository<tblLithoStratigraphySection>())

                    try
                    {
                        LithostratigraphicUnits = new BindableCollection<tblLithoStratigraphySection>();

                        switch (SelectedLithologicalSection.secType)
                        {
                            case "Lithological":
                                LithostratigraphicUnits = new BindableCollection<tblLithoStratigraphySection>(db.GetModelByExpression(a => a.lithosecIdFk == SelectedLithologicalSection.secIdPk).OrderBy(x => x.lithosecBase).ToList());

                                try
                                {
                                   
                                }
                                catch
                                {
                                    
                                }
                                break;

                        }
                    }
                    catch (Exception e)
                    {
                        LithostratigraphicUnits = new BindableCollection<tblLithoStratigraphySection>();
                    }
            });

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => FilesListViewModel.IsFileLoading, async () =>
             {
                 FilesListViewModel.ReferedObject = SelectedLithologicalSection;
                 FilesListViewModel.FileStore = await FileHelper.LoadFilesAsync(SelectedLithologicalSection.secIdPk, "Section");
                 FilesListViewModel.SelectedFileStore = FilesListViewModel.FileStore.Count > 0 ? FilesListViewModel.FileStore.First() : new v_FileStore();
             });
        }

        /// <summary>
        /// Creating the chart
        /// </summary>
        public async void CreateCharts()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => SectionChartViewModel.LiLo.Updating, async () =>
            {
                SectionChart();
            });
        }

        /// <summary>
        /// Creating a line chart
        /// </summary>
        public void SectionChart()
        {
            try
            {
                SectionChartViewModel.LiLo.Title = SelectedLithologicalSection.secName.ToString();
                SectionChartViewModel.LiLo.LithofaciesLayers = new BindableCollection<tblSectionLithofacy>(FaciesBeds);
                SectionChartViewModel.LiLo.CreateLithologicalLog();
            }
            catch
            {
                return;
            }
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please select a valid value");
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
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            tblObjectOfInvestigation current = SelectedObject;
            tblSection currentSection = SelectedLithologicalSection;

            try
            {
                OnSelectedObjectChanged();

                try
                {
                    SelectedLithologicalSection = LithologicalSections.Where(x => x.secIdPk == currentSection.secIdPk).First();
                }
                catch
                {
                    try
                    {
                        SelectedLithologicalSection = LithologicalSections.First();
                    }
                    catch
                    {
                        SelectedLithologicalSection = new tblSection() { secInterpreterIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                    }
                }
            }
            catch
            {
                try
                {
                    LoadData(current.ooiType.ToString());
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }
            }
        }

        // Sets up the form so that user can enter data. Data is later  
        // saved when user clicks Commit.  
        public void Add()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedObject, (int)SelectedObject.ooiUploaderIdFk, SelectedObject.ooiIdPk))
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

            using (var db = new ApirsRepository<tblSection>())
            {
                try
                {
                    db.InsertModel(new tblSection() { secprjIdFk = SelectedProject.prjIdPk, secOoiName = SelectedObject.ooiName, secInterpreterIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId, secType = "Lithological", secName = SelectedObject.ooiName + "_Section1" });
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("New section added.");
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Measurement can't be added. Please check every field again.");
                    return;
                }
            }

            OnSelectedObjectChanged();
        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool CanDelete { get => !Updating; set { NotifyOfPropertyChange(() => CanDelete); } }
        public void Delete()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            // If existing window is visible, delete the customer and all their orders.  
            // In a real application, you should add warnings and allow the user to cancel the operation.  

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?",
                "You won't be able to retrieve the related measurement data after deleting this measurement.") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblSection>())
            {
                try
                {
                    tblSection result = db.GetModelByExpression(b => b.secIdPk == SelectedLithologicalSection.secIdPk).First();

                    if (result != null)
                    {
                        db.DeleteModelById(result.secIdPk);
                    }

                }
                catch (Exception ex)
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                }
                finally
                {
                }

            }

            OnSelectedObjectChanged();
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk))
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
                new ApirsRepository<tblSection>().UpdateModel(SelectedLithologicalSection, SelectedLithologicalSection.secIdPk);

                switch (SelectedLithologicalSection.secType)
                {
                    case "Lithological":
                        using (var db1 = new ApirsRepository<tblLithoStratigraphySection>())
                        {
                            try
                            {
                                var fc = LithostratigraphicUnits;

                                foreach (tblLithoStratigraphySection seclit in fc)
                                {
                                    if (seclit.lithosecIdPk == 0)
                                    {
                                        db1.InsertModel(seclit);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            seclit.lithosecThickness = seclit.lithosecBase - seclit.lithosecTop;
                                            db1.UpdateModelWithoutSave(seclit, seclit.lithosecIdPk);
                                        }
                                        catch (Exception ex) { }
                                    }
                                }

                                db1.Save();
                            }
                            catch
                            {
                            }
                        }

                        using (var db1 = new ApirsRepository<tblSectionLithofacy>())
                        {
                            try
                            {
                                var fc = FaciesBeds;

                                foreach (tblSectionLithofacy seclit in fc)
                                {
                                    if (seclit.litsecIdPk == 0)
                                    {
                                        db1.InsertModel(seclit);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            seclit.litsecThickness = seclit.litsecTop - seclit.litsecBase;
                                            db1.UpdateModelWithoutSave(seclit, seclit.litsecIdPk);
                                            FaciesBeds.Where(x => x.litsecIdPk == seclit.litsecIdPk).FirstOrDefault().litsecThickness = seclit.litsecThickness;
                                        }
                                        catch (Exception ex) { }
                                    }
                                }

                                db1.Save();
                            }
                            catch
                            {
                            }
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
                    if (e.Message.Contains("Sequence")) return;
                    if (e.InnerException.Message.Contains("CHECK"))
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("Your provided values exceed the valid data ranges. Please review and try again.");
                    else
                        _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                }
                catch
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                }
            }
        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
        {
            if (!DataValidation.CheckPrerequisites(CRUD.Add))
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


        ///Event that gets fired if the filter text was changed
        private void OnTextFilterChanged()
        {
            if (TextFilter == "")
            {
                ObjectsOfInvestigation = this.allObjectOfInvestigation;
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
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(CollectionHelper.Filter<tblObjectOfInvestigation>(this.allObjectOfInvestigation, TextFilter));
                    }
                    catch (Exception e)
                    {
                        try
                        {
                            //Filtering outcrops
                            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>((from rs in this.allObjectOfInvestigation
                                                                                                       where CollectionHelper.NullToString(rs.ooiName).ToLower().Contains(TextFilter)
                                                                                                       select rs).ToList());
                        }
                        catch (Exception ex)
                        {
                            _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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

        //Adding a bed to the collection
        public void AddBed()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk, SelectedLithologicalSection.secIdPk))
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
                using (var db = new ApirsRepository<tblSectionLithofacy>())
                {

                    db.InsertModel(new tblSectionLithofacy()
                    {
                        litsecIdFk = SelectedLithologicalSection.secIdPk,
                        litseclftId = 38,
                        litseclftCode = "Unknown",
                        litsecBase = SelectedObject.ooiType == "Outcrop" ? FaciesBeds.Max(x => x.litsecTop) : FaciesBeds.Min(x => x.litsecTop)
                    });

                    OnSelectedLithologicalSectionChanged();
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        //Adding a lithostratigraphic layer to the collection
        public void AddLithostratigraphicLayer()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk, SelectedLithologicalSection.secIdPk))
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
                using (var db = new ApirsRepository<tblLithoStratigraphySection>())
                {

                    db.InsertModel(new tblLithoStratigraphySection()
                    {
                        lithosecIdFk = SelectedLithologicalSection.secIdPk,
                        lithosecLithostratigraphy = "Unknown",
                        lithosecBase = SelectedObject.ooiType == "Outcrop" ? LithostratigraphicUnits.Max(x => x.lithosecTop) : LithostratigraphicUnits.Min(x => x.lithosecTop)
                    });

                    OnSelectedLithologicalSectionChanged();
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        //Adding a bed to the collection
        public void MoveBeds(object parameter)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk))
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
                ListView selectedBeds = (ListView)parameter;
                List<int> bedIndex = new List<int>();

                if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(selectedBeds.SelectedItems.Count.ToString() + " beds will be deleted. Are you sure to delete the records?",
                            "You won't be able to retrieve the related data after deleting these beds.") == MessageBoxViewResult.No)
                {
                    return;
                }

                if (selectedBeds.SelectedItems.Count <= 0)
                    return;

                for (int i = 0; i < selectedBeds.SelectedItems.Count; i++)
                {
                    bedIndex.Add(selectedBeds.Items.IndexOf(selectedBeds.SelectedItems[i]));
                }

                using (var db = new ApirsRepository<tblSectionLithofacy>())
                {
                    for (int j = 0; j < bedIndex.Count; j++)
                    {
                        var id = FaciesBeds.ElementAt(bedIndex[j]).litsecIdPk;
                        tblSectionLithofacy result = db.GetModelByExpression(x => x.litsecIdPk == id).First();
                        result.litsecIdFk = MoveToSectionId;
                        db.UpdateModelWithoutSave(result, result.litsecIdPk);
                    }

                    db.Save();

                    OnSelectedLithologicalSectionChanged();
                }

            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }

        }

        //Adding a bed to the collection
        public void RemoveBeds()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedLithologicalSection, (int)SelectedLithologicalSection.secInterpreterIdFk))
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
                if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion(SelectedFaciesBeds.Count.ToString() + " beds will be deleted. Are you sure to delete the records?",
                            "You won't be able to retrieve the related data after deleting these beds.") == MessageBoxViewResult.No)
                {
                    return;
                }

                foreach (tblSectionLithofacy faciesBed in SelectedFaciesBeds)
                {
                    new ApirsRepository<tblSectionLithofacy>().DeleteModelById(faciesBed.litsecIdPk);
                }

                    FaciesBeds.RemoveRange(SelectedFaciesBeds);
            //    ListView selectedBeds = (ListView)parameter;
            //    List<int> bedIndex = new List<int>();



            //    if (selectedBeds.SelectedItems.Count <= 0)
            //        return;

            //    for (int i = 0; i < selectedBeds.SelectedItems.Count; i++)
            //    {
            //        bedIndex.Add(selectedBeds.Items.IndexOf(selectedBeds.SelectedItems[i]));
            //    }

            //    using (var db = new ApirsRepository<tblSectionLithofacy>())
            //    {
            //        for (int j = 0; j < bedIndex.Count; j++)
            //        {
            //            var id = FaciesBeds.ElementAt(bedIndex[j]).litsecIdPk;
            //            tblSectionLithofacy result = db.GetModelByExpression(x => x.litsecIdPk == id).First();
            //            db.DeleteModelById(result.litsecIdPk);
            //        }

            //        OnSelectedLithologicalSectionChanged();
            //    }

            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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

        //Exporting a control
        public void ExportControl(object parameter, bool report = false)
        {
            if (report)
            {
                _events.PublishOnUIThreadAsync(new ExportControlMessage((UIElement)parameter, "pdf"));
            }
        }


        //Exporting a control
        public async void ExportControl(object parameter)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|PDF (*.pdf)|*.pdf|CSV (*.csv)|*.csv";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Getting the extension
                var ext = saveFileDialog.FileName.Substring(saveFileDialog.FileName.LastIndexOf(".")).ToLower();

                switch (ext.ToString())
                {
                    case ".csv":
                        await Task.Run(() =>
                        {
                            try
                            {
                                //Converting data to data tables
                                DataTable sectionTable = CollectionHelper.ConvertTo<tblSection>(LithologicalSections);
                                DataTable bedTable = CollectionHelper.ConvertTo<tblSectionLithofacy>(FaciesBeds);

                                //DataTable beds = CollectionHelper.ConvertTo<tblSectionLithofacy>(FaciesBeds.ToList());

                                //Exporting data to a csv
                                ExportHelper.ExportDataTableToCsv(sectionTable, saveFileDialog.FileName);
                                ExportHelper.ExportDataTableToCsv(bedTable, saveFileDialog.FileName, true);
                            }
                            catch
                            {
                                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                            }
                        });
                        break;
                    case ".png":
                        //Downloading to the specific folder
                        ImageCapturer.SaveToPng((FrameworkElement)parameter, saveFileDialog.FileName);
                        break;
                    case ".bmp":
                        ImageCapturer.SaveToBmp((FrameworkElement)parameter, saveFileDialog.FileName);
                        break;
                    case ".pdf":
                        List<Bitmap> bm = new List<Bitmap>();
                        var encoder = new PngBitmapEncoder();
                        bm.Add(ImageCapturer.UIElementToBitmap((FrameworkElement)parameter, encoder, 300));
                        ExportHelper.ExportImageToPdf(bm, saveFileDialog.FileName);
                        break;
                }
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
                    case "Lithological":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblSectionLithofacy>(
                                new ImportProcedureViewModel<tblSectionLithofacy>(_events, table));
                        break;
                    case "Lithostratigraphic":
                        break;
                }

            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        #endregion
    }

}
