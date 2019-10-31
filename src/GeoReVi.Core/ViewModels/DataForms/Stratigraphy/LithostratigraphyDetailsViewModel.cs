using Caliburn.Micro;
using System.Windows;
using System.Linq;
using System;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace GeoReVi
{
    /// <summary>
    /// View model for a single data entry form for rock samples
    /// </summary>
    public class LithostratigraphyDetailsViewModel : Screen
    {
        #region Private members

        //Lithostratigraphy collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphies;
        //Lithostratigraphy collection
        private BindableCollection<LithostratigraphyUnion> allLithostratigraphies;
        //Parent lithostratigraphy collection
        private BindableCollection<LithostratigraphyUnion> parentLithostratigraphies;

        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;

        /// <summary>
        /// Visibility members
        /// </summary>
        private bool isNotGroup;

        /// <summary>
        /// Selected objects
        /// </summary>
        private LithostratigraphyUnion selectedLithostratigraphy;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Cancellation token source for cancelling image downloads
        /// </summary>
        private CancellationTokenSource cts;

        //Text filter variable
        private string textFilter;

        private string hierarchy;
        #endregion

        #region Public properties

        /// <summary>
        /// The selected rock sample
        /// </summary>
        public LithostratigraphyUnion SelectedLithostratigraphy
        {
            get { return this.selectedLithostratigraphy; }
            set
            {
                this.selectedLithostratigraphy = value;
                NotifyOfPropertyChange(() => SelectedLithostratigraphy);
            }
        }

        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<LithostratigraphyUnion> Lithostratigraphies
        {
            get { return this.lithostratigraphies; }
            set
            {
                this.lithostratigraphies = value;
                NotifyOfPropertyChange(() => Lithostratigraphies);
            }
        }

        /// <summary>
        /// Rock sample collection for the form
        /// </summary>
        public BindableCollection<LithostratigraphyUnion> ParentLithostratigraphies
        {
            get { return this.parentLithostratigraphies; }
            set
            {
                this.parentLithostratigraphies = value;
                NotifyOfPropertyChange(() => IsNotGroup);
                NotifyOfPropertyChange(() => ParentLithostratigraphies);
            }
        }

        /// <summary>
        /// Collection of all chronostratigraphic units
        /// </summary>
        public BindableCollection<tblUnionChronostratigraphy> Chronostratigraphy
        {
            get { return this.chronostratigraphy; }
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
        /// Hierarchy of the lithostratigraphic unit
        /// </summary>
        public string Hierarchy
        {
            get { return this.hierarchy; }
            set
            {
                this.hierarchy = value;
                if (value != null)
                    LoadData(value);
                NotifyOfPropertyChange(() => Hierarchy);
            }
        }

        /// <summary>
        /// Checking if actual selected item is not of group hierarchy
        /// </summary>
        public bool IsNotGroup
        {
            get
            {
                try
                {
                    return ParentLithostratigraphies.Count > 0;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                NotifyOfPropertyChange(() => IsNotGroup);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LithostratigraphyDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            LoadData();
            Hierarchy = "Group";
        }
        #endregion

        #region Methods

        /// <summary>
        /// Loads all meta data
        /// </summary>
        private void LoadData()
        {
            try
            {
                try
                {
                    using (var db = new ApirsRepository<tblUnionChronostratigraphy>())
                    {
                        this.chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(db.GetModel().ToList());

                        this.allLithostratigraphies = new BindableCollection<LithostratigraphyUnion>(db.GetCompleteLithostratigraphy().ToList());
                    }
                }
                catch (Exception ex)
                {
                    this.chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                    this.allLithostratigraphies = new BindableCollection<LithostratigraphyUnion>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //Initial data load method
        public async void LoadData(string parameter = "Group")
        {
            try
            {
                try
                {
                    Lithostratigraphies = new BindableCollection<LithostratigraphyUnion>(this.allLithostratigraphies.Where(x => x.Hierarchy.Equals(parameter)).OrderBy(x => x.grName).ToList());

                    switch (parameter)
                    {
                        case "Group":
                            ParentLithostratigraphies = new BindableCollection<LithostratigraphyUnion>();
                            break;
                        case "Subgroup":
                            ParentLithostratigraphies = new BindableCollection<LithostratigraphyUnion>(this.allLithostratigraphies.Where(x => x.Hierarchy.Equals("Group")).OrderBy(x => x.grName).ToList());
                            break;
                        case "Formation":
                            ParentLithostratigraphies = new BindableCollection<LithostratigraphyUnion>(this.allLithostratigraphies.Where(x => x.Hierarchy.Equals("Subgroup")).OrderBy(x => x.grName).ToList());
                            break;
                        case "Subformation":
                            ParentLithostratigraphies = new BindableCollection<LithostratigraphyUnion>(this.allLithostratigraphies.Where(x => x.Hierarchy.Equals("Formation")).OrderBy(x => x.grName).ToList());
                            break;
                    }

                    if (Lithostratigraphies.Count > 0)
                        SelectedLithostratigraphy = Lithostratigraphies.First();
                    else
                        SelectedLithostratigraphy = new LithostratigraphyUnion() { Hierarchy = this.Hierarchy };
                }
                catch (Exception ex)
                {
                    Lithostratigraphies = new BindableCollection<LithostratigraphyUnion>();
                    SelectedLithostratigraphy = new LithostratigraphyUnion() { Hierarchy = this.Hierarchy };
                    ParentLithostratigraphies = new BindableCollection<LithostratigraphyUnion>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Go to the last dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Last()
        {
            if (Lithostratigraphies.Count != 0)
                SelectedLithostratigraphy = Lithostratigraphies.Last();
        }

        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (Lithostratigraphies.Count != 0)
                SelectedLithostratigraphy = Navigation.GetPrevious(Lithostratigraphies, SelectedLithostratigraphy);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {
            if (Lithostratigraphies.Count != 0)
                SelectedLithostratigraphy = Navigation.GetNext(Lithostratigraphies, SelectedLithostratigraphy);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (Lithostratigraphies.Count != 0)
                SelectedLithostratigraphy = Lithostratigraphies.First();
        }

        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            LithostratigraphyUnion current = SelectedLithostratigraphy;

            try
            {
                string selected = SelectedLithostratigraphy.grName;
                LoadData();
                LoadData(SelectedLithostratigraphy.Hierarchy.ToString());
                SelectedLithostratigraphy = Lithostratigraphies.Where(x => x.grName == selected).First();
            }
            catch
            {
                try
                {
                    LoadData();
                    LoadData("Group");
                    SelectedLithostratigraphy = new LithostratigraphyUnion();
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occured.");
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
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedLithostratigraphy, SelectedLithostratigraphy.UploaderId))
                {
                    return;
                }
            }
            catch
            {
                return;
            }


            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?",
                "You won't be able to retrieve the related measurement data after deleting this sample.") == MessageBoxViewResult.No)
            {
                return;
            }
            try
            {
                switch (SelectedLithostratigraphy.Hierarchy)
                {
                    case "Group":
                        tblGroup result = new ApirsRepository<tblGroup>().GetModelByExpression(b => b.grName == SelectedLithostratigraphy.grName).First();

                        if (result != null)
                        {
                            new ApirsRepository<tblGroup>().DeleteModelById(result.grIdPk);
                        }
                        break;
                    case "Subgroup":
                        tblSubgroup result1 = new ApirsRepository<tblSubgroup>().GetModelByExpression(b => b.sgName == SelectedLithostratigraphy.grName).First();

                        if (result1 != null)
                        {
                            new ApirsRepository<tblSubgroup>().DeleteModelById(result1.sgIdPk);
                        }
                        break;
                    case "Formation":
                        tblFormation result2 = new ApirsRepository<tblFormation>().GetModelByExpression(b => b.fmName == SelectedLithostratigraphy.grName).First();

                        if (result2 != null)
                        {
                            new ApirsRepository<tblFormation>().DeleteModelById(result2.fmIdPk);
                        }
                        break;
                    case "Subformation":
                        tblSubformation result3 = new ApirsRepository<tblSubformation>().GetModelByExpression(b => b.sfName == SelectedLithostratigraphy.grName).First();

                        if (result3 != null)
                        {
                            new ApirsRepository<tblSubformation>().DeleteModelById(result3.sfIdPk);
                        }
                        break;
                }

                tblUnionLithostratigraphy result4 = new ApirsRepository<tblUnionLithostratigraphy>().GetModelByExpression(b => b.grName == SelectedLithostratigraphy.grName).First();

                if (result4 != null)
                {
                    new ApirsRepository<tblUnionLithostratigraphy>().DeleteModelById(result4.ID);
                }

                Lithostratigraphies.Remove(SelectedLithostratigraphy);
                allLithostratigraphies.Remove(SelectedLithostratigraphy);
                SelectedLithostratigraphy = Lithostratigraphies.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
            }
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, Lithostratigraphies, SelectedLithostratigraphy.UploaderId))
                {
                    return;
                }
                else if (SelectedLithostratigraphy.grName == null)
                    return;
            }
            catch
            {
                return;
            }

            try
            {
                if (SelectedLithostratigraphy.Id == 0)
                {
                    try
                    {
                        SelectedLithostratigraphy.UploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;
                        new ApirsRepository<tblUnionLithostratigraphy>().InsertModel(new tblUnionLithostratigraphy()
                        {
                            grName = SelectedLithostratigraphy.grName,
                            unionLithUploaderIdFk = SelectedLithostratigraphy.UploaderId,
                            chronostratNameFk = SelectedLithostratigraphy.Chronostratigraphy
                        });

                        switch (SelectedLithostratigraphy.Hierarchy)
                        {
                            case "Group":
                                new ApirsRepository<tblGroup>().InsertModel(new tblGroup()
                                {
                                    grName = SelectedLithostratigraphy.grName,
                                    grBaseBoundary = SelectedLithostratigraphy.BaseBoundary,
                                    grTopBoundary = SelectedLithostratigraphy.TopBoundary,
                                    grCountries = SelectedLithostratigraphy.Countries,
                                    grDateDocumentation = SelectedLithostratigraphy.DateDocumentation,
                                    grLiterature = SelectedLithostratigraphy.Literature,
                                    grLithologicDescriptionShort = SelectedLithostratigraphy.LithologicDescriptionShort,
                                    grMaxThickness = SelectedLithostratigraphy.MaxThickness,
                                    grMeanThickness = SelectedLithostratigraphy.MeanThickness,
                                    grNotes = SelectedLithostratigraphy.Notes,
                                    grStates = SelectedLithostratigraphy.States,
                                    grTypeLocality = SelectedLithostratigraphy.TypeLocality
                                });
                                break;
                            case "Subgroup":
                                new ApirsRepository<tblSubgroup>().InsertModel(new tblSubgroup()
                                {
                                    sgName = SelectedLithostratigraphy.grName,
                                    sggrIdFk = new ApirsRepository<tblGroup>().GetModelByExpression(strat => strat.grName == SelectedLithostratigraphy.ParentName).Select(strat => strat.grIdPk).FirstOrDefault(),
                                    sgBaseBoundary = SelectedLithostratigraphy.BaseBoundary,
                                    sgTypeLocality = SelectedLithostratigraphy.TypeLocality,
                                    sgTopBoundary = SelectedLithostratigraphy.TopBoundary,
                                    sgCountries = SelectedLithostratigraphy.Countries,
                                    sgDateOfDocumentation = SelectedLithostratigraphy.DateDocumentation,
                                    sgLiterature = SelectedLithostratigraphy.Literature,
                                    sgLithologicDescriptionShort = SelectedLithostratigraphy.LithologicDescriptionShort,
                                    sgMaxThickness = SelectedLithostratigraphy.MaxThickness,
                                    sgMeanThickness = SelectedLithostratigraphy.MeanThickness,
                                    sgNotes = SelectedLithostratigraphy.Notes,
                                    sgStates = SelectedLithostratigraphy.States,
                                    sgDescription = SelectedLithostratigraphy.TypeLocality
                                });
                                break;
                            case "Formation":
                                new ApirsRepository<tblFormation>().InsertModel(new tblFormation()
                                {
                                    fmName = SelectedLithostratigraphy.grName,
                                    fmsgIdFk = new ApirsRepository<tblSubgroup>().GetModelByExpression(strat => strat.sgName == SelectedLithostratigraphy.ParentName).Select(strat => strat.sgIdPk).FirstOrDefault(),
                                    fmBaseBoundary = SelectedLithostratigraphy.BaseBoundary,
                                    fmTopBoundary = SelectedLithostratigraphy.TopBoundary,
                                    fmCountries = SelectedLithostratigraphy.Countries,
                                    fmLiterature = SelectedLithostratigraphy.Literature,
                                    fmDescription = SelectedLithostratigraphy.LithologicDescriptionShort,
                                    fmMaxThickness = SelectedLithostratigraphy.MaxThickness,
                                    fmMeanThickness = SelectedLithostratigraphy.MeanThickness,
                                    fmNotes = SelectedLithostratigraphy.Notes,
                                    fmStates = SelectedLithostratigraphy.States,
                                    fmDateOfDocumentation = DateTime.Now,
                                    fmTypeLocality = SelectedLithostratigraphy.TypeLocality
                                });
                                break;
                            case "Subformation":
                                new ApirsRepository<tblSubformation>().InsertModel(new tblSubformation()
                                {
                                    sfName = SelectedLithostratigraphy.grName,
                                    sffmId = new ApirsRepository<tblFormation>().GetModelByExpression(strat => strat.fmName == SelectedLithostratigraphy.ParentName).Select(strat => strat.fmIdPk).FirstOrDefault(),
                                    sfBaseBoundary = SelectedLithostratigraphy.BaseBoundary,
                                    sfTopBoundary = SelectedLithostratigraphy.TopBoundary,
                                    sfLiterature = SelectedLithostratigraphy.Literature,
                                    sfDescription = SelectedLithostratigraphy.LithologicDescriptionShort,
                                    sfMaxThickness = SelectedLithostratigraphy.MaxThickness,
                                    sfMeanThickness = SelectedLithostratigraphy.MeanThickness,
                                    sfNotes = SelectedLithostratigraphy.Notes,
                                    sfCountries = SelectedLithostratigraphy.Countries,
                                    sfDateOfDocumentation = DateTime.Now,
                                    sfTypeLocality = SelectedLithostratigraphy.TypeLocality
                                });
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (!e.InnerException.ToString().Contains("entries"))
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Lithostratigraphic unit can't be added. Please check every field again.");
                    }

                }
                else
                {
                    string tempName = "";
                    tblUnionLithostratigraphy result0 = new ApirsRepository<tblUnionLithostratigraphy>().GetModelByExpression(b => b.ID == SelectedLithostratigraphy.Id).First();
                    if (result0 != null)
                    {
                        tempName = result0.grName;
                        var a = new ApirsRepository<tblUnionLithostratigraphy>().GetModelById(SelectedLithostratigraphy.Id);
                        a.ID = SelectedLithostratigraphy.Id;
                        a.grName = SelectedLithostratigraphy.grName;
                        a.unionLithUploaderIdFk = SelectedLithostratigraphy.UploaderId;
                        a.chronostratNameFk = SelectedLithostratigraphy.Chronostratigraphy;

                        new ApirsRepository<tblUnionLithostratigraphy>().UpdateModel(a, a.ID);
                    }

                    switch (SelectedLithostratigraphy.Hierarchy)
                    {
                        case "Group":
                            using (var db1 = new ApirsRepository<tblGroup>())
                            {
                                tblGroup result = db1.GetModelByExpression(b => b.grName == tempName).First();
                                if (result != null)
                                {
                                    result.grName = SelectedLithostratigraphy.grName;
                                    result.grBaseBoundary = SelectedLithostratigraphy.BaseBoundary;
                                    result.grTopBoundary = SelectedLithostratigraphy.TopBoundary;
                                    result.grCountries = SelectedLithostratigraphy.Countries;
                                    result.grDateDocumentation = SelectedLithostratigraphy.DateDocumentation;
                                    result.grLiterature = SelectedLithostratigraphy.Literature;
                                    result.grLithologicDescriptionShort = SelectedLithostratigraphy.LithologicDescriptionShort;
                                    result.grMaxThickness = SelectedLithostratigraphy.MaxThickness;
                                    result.grMeanThickness = SelectedLithostratigraphy.MeanThickness;
                                    result.grNotes = SelectedLithostratigraphy.Notes;
                                    result.grStates = SelectedLithostratigraphy.States;
                                    result.grTypeLocality = SelectedLithostratigraphy.TypeLocality;
                                };

                                db1.UpdateModel(result, result.grIdPk);
                            }

                            break;
                        case "Subgroup":
                            using (var db1 = new ApirsRepository<tblSubgroup>())
                            {
                                tblSubgroup result1 = db1.GetModelByExpression(b => b.sgName == tempName).First();
                                if (result1 != null)
                                {
                                    result1.sgName = SelectedLithostratigraphy.grName;
                                    result1.sgBaseBoundary = SelectedLithostratigraphy.BaseBoundary;
                                    result1.sgTopBoundary = SelectedLithostratigraphy.TopBoundary;
                                    result1.sgTypeLocality = SelectedLithostratigraphy.TypeLocality;
                                    result1.sgCountries = SelectedLithostratigraphy.Countries;
                                    result1.sgDateOfDocumentation = SelectedLithostratigraphy.DateDocumentation;
                                    result1.sgLiterature = SelectedLithostratigraphy.Literature;
                                    result1.sgLithologicDescriptionShort = SelectedLithostratigraphy.LithologicDescriptionShort;
                                    result1.sgMaxThickness = SelectedLithostratigraphy.MaxThickness;
                                    result1.sgMeanThickness = SelectedLithostratigraphy.MeanThickness;
                                    result1.sgNotes = SelectedLithostratigraphy.Notes;
                                    result1.sgStates = SelectedLithostratigraphy.States;
                                    result1.sgDescription = SelectedLithostratigraphy.TypeLocality;

                                    db1.UpdateModel(result1, result1.sgIdPk);
                                }
                            }
                            break;
                        case "Formation":
                            using (var db1 = new ApirsRepository<tblFormation>())
                            {
                                tblFormation result2 = db1.GetModelByExpression(b => b.fmName == tempName).First();
                                if (result2 != null)
                                {

                                    result2.fmName = SelectedLithostratigraphy.grName;
                                    result2.fmBaseBoundary = SelectedLithostratigraphy.BaseBoundary;
                                    result2.fmTopBoundary = SelectedLithostratigraphy.TopBoundary;
                                    result2.fmCountries = SelectedLithostratigraphy.Countries;
                                    result2.fmDateOfDocumentation = SelectedLithostratigraphy.DateDocumentation;
                                    result2.fmLiterature = SelectedLithostratigraphy.Literature;
                                    result2.fmDescription = SelectedLithostratigraphy.LithologicDescriptionShort;
                                    result2.fmMaxThickness = SelectedLithostratigraphy.MaxThickness;
                                    result2.fmMeanThickness = SelectedLithostratigraphy.MeanThickness;
                                    result2.fmNotes = SelectedLithostratigraphy.Notes;
                                    result2.fmStates = SelectedLithostratigraphy.States;
                                    result2.fmTypeLocality = SelectedLithostratigraphy.TypeLocality;

                                    db1.UpdateModel(result2, result2.fmIdPk);
                                }

                            }

                            break;
                        case "Subformation":
                            using (var db1 = new ApirsRepository<tblSubformation>())
                            {
                                tblSubformation result3 = db1.GetModelByExpression(b => b.sfName == tempName).First();
                                if (result3 != null)
                                {
                                    result3.sfName = SelectedLithostratigraphy.grName;
                                    result3.sfBaseBoundary = SelectedLithostratigraphy.BaseBoundary;
                                    result3.sfTopBoundary = SelectedLithostratigraphy.TopBoundary;
                                    result3.sfLiterature = SelectedLithostratigraphy.Literature;
                                    result3.sfDescription = SelectedLithostratigraphy.LithologicDescriptionShort;
                                    result3.sfMaxThickness = SelectedLithostratigraphy.MaxThickness;
                                    result3.sfMeanThickness = SelectedLithostratigraphy.MeanThickness;
                                    result3.sfNotes = SelectedLithostratigraphy.Notes;
                                    result3.sfCountries = SelectedLithostratigraphy.Countries;
                                    result3.sfDateOfDocumentation = DateTime.Now;
                                    result3.sfTypeLocality = SelectedLithostratigraphy.TypeLocality;

                                    db1.UpdateModel(result3, result3.sfIdPk);
                                }
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
                if (e.Message.Contains("Sequence"))
                    return;

                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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
        }
        catch
        {
            return;
        }

        SelectedLithostratigraphy = new LithostratigraphyUnion() { Hierarchy = this.Hierarchy, UploaderId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
    }

    /// <summary>
    /// Checking the uniqueness of the name in the database
    /// </summary>
    public void CheckUniqueness()
    {
        int count;

        try
        {
            using (var db = new ApirsRepository<tblUnionLithostratigraphy>())
            {
                if (SelectedLithostratigraphy.Id == 0)
                {
                    count = db.GetModelByExpression(x => x.grName == SelectedLithostratigraphy.grName).Count();
                }
                else
                {
                    count = db.GetModelByExpression(x => x.grName == SelectedLithostratigraphy.grName && x.ID != SelectedLithostratigraphy.Id).Count();
                }

                if (count == 0)
                    return;

                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. Please provide another name.");
            }
        }
        catch (Exception e)
        {
            _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
        }
    }

    ///Event that gets fired if the filter text was changed
    private void OnTextFilterChanged()
    {

        if (TextFilter == "")
        {
            Lithostratigraphies = new BindableCollection<LithostratigraphyUnion>(this.allLithostratigraphies.Where(x => x.Hierarchy == Hierarchy.ToString()).ToList());
        }

        if (Lithostratigraphies.Count == 0)
            return;

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
                    Lithostratigraphies = new BindableCollection<LithostratigraphyUnion>(CollectionHelper.Filter<LithostratigraphyUnion>(allLithostratigraphies, TextFilter).Where(x => x.Hierarchy == hierarchy));
                }
                catch (Exception e)
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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

    //Exporting a control
    public void ExportControl(object parameter, bool report = false)
    {
        if (report)
        {
            _events.PublishOnUIThreadAsync(new ExportControlMessage((UIElement)parameter, "pdf"));
        }
    }

    //Exporting the actually selected list of objects to a csv file
    public void ExportList()
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

        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "CSV (*.csv)|*.csv";
        saveFileDialog.RestoreDirectory = true;

        saveFileDialog.ShowDialog();

        if (saveFileDialog.FileName != "")
        {
            //Exporting the list dependent on the sample type and the actual selection
            ExportHelper.ExportList<LithostratigraphyUnion>(Lithostratigraphies, saveFileDialog.FileName);
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

    /// <summary>
    /// Checking if the user is logged in, a field measurement is selected and the user is also the uploader 
    /// </summary>

    #endregion
}
}
