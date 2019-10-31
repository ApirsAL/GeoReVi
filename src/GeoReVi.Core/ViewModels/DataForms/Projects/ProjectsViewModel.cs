using Caliburn.Micro;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// View model for project management
    /// </summary>
    public class ProjectsViewModel : Screen
    {
        #region Private members

        /// <summary>
        /// Selected Project
        /// </summary>
        private tblProject selectedProject;

        /// <summary>
        /// Project collection
        /// </summary>
        private BindableCollection<tblProject> projects;

        /// <summary>
        /// Participating project collection
        /// </summary>
        private BindableCollection<tblProject> participatingProjects;

        /// <summary>
        /// All participants of a certain project
        /// </summary>
        private BindableCollection<tblPerson> participants;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        #endregion

        #region Public properties

        /// <summary>
        /// Selected Project object
        /// </summary>
        public tblProject SelectedProject
        {
            get { return this.selectedProject; }
            set
            {
                this.selectedProject = value;
                OnSelectedProjectChanged();
                NotifyOfPropertyChange(() => SelectedProject);
                NotifyOfPropertyChange(() => SelectedProjectIndex);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedProjectIndex
        {
            get
            {
                if (SelectedProject != null)
                    return (Projects.IndexOf(SelectedProject) + 1).ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => SelectedProjectIndex);
            }
        }

        /// <summary>
        /// Project collection
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get { return this.projects; }
            set { this.projects = value; NotifyOfPropertyChange(() => Projects); NotifyOfPropertyChange(() => CountProjects); }
        }

        /// <summary>
        /// Participating project collection
        /// </summary>
        public BindableCollection<tblProject> ParticipatingProjects
        {
            get { return this.participatingProjects; }
            set { this.participatingProjects = value; NotifyOfPropertyChange(() => ParticipatingProjects); }
        }

        /// <summary>
        /// Readonly to count the selected projects
        /// </summary>
        public int CountProjects
        {
            get
            {
                if (Projects != null)
                    return Projects.Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// All participants of a project
        /// </summary>
        public BindableCollection<tblPerson> Participants
        {
            get
            {
                return this.participants;
            }
            set
            {
                this.participants = value;
                NotifyOfPropertyChange(() => Participants);
            }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ProjectsViewModel(IEventAggregator events)
        {
            _events = events;
            LoadData((int)((ShellViewModel)IoC.Get<IShell>()).UserId);
        }

        #endregion

        #region Methods

        //Loading filtered data
        private void LoadData(int id)
        {

            using (var db = new ApirsRepository<tblProject>())
            {
                try
                {
                    Projects = new BindableCollection<tblProject>(db.GetModelByExpression(x => x.prjCreatorIdFk == id));

                    if (Projects.Count == 0)
                    {
                        SelectedProject = new tblProject() { prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                    }
                    else if (Projects.Count > 1)
                    {
                        SelectedProject = Projects.First();
                    }
                    else
                    { SelectedProject = Projects.First(); }
                }
                catch(Exception e)
                {
                    Projects = new BindableCollection<tblProject>();
                    SelectedProject = new tblProject() { prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }

                try
                {
                    ParticipatingProjects = new BindableCollection<tblProject>(db.GetProjectByParticipation((int)((ShellViewModel)IoC.Get<IShell>()).UserId));
                }
                catch
                {
                    ParticipatingProjects = new BindableCollection<tblProject>();
                }
            }
        }

        // Commit changes from the new rock sample form
        // or edits made to the existing rock sample form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedProject, (int)SelectedProject.prjCreatorIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblProject>())
            {
                try
                {
                    SelectedProject.prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;

                    if (SelectedProject.prjIdPk == 0)
                    {
                        db.InsertModel(SelectedProject);
                        Projects.Add(SelectedProject);
                        ((ShellViewModel)IoC.Get<IShell>()).Projects.Add(SelectedProject);
                    }
                    else
                        db.UpdateModel(SelectedProject, SelectedProject.prjIdPk);

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("EntityValidation"))
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("Please provide a project name.");
                    else
                        ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpected error occurred.");
                }
                finally
                {
                }

            }
        }


        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
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

            tblProject current = SelectedProject;
            int id = 0;

            try
            {
                if (SelectedProject != null)
                    id = SelectedProject.prjIdPk;

                LoadData((int)((ShellViewModel)IoC.Get<IShell>()).UserId);
                SelectedProject = Projects.Where(p => p.prjIdPk == id).First();
            }
            catch
            {
                try
                {
                    LoadData((int)((ShellViewModel)IoC.Get<IShell>()).UserId);
                    SelectedProject = new tblProject() { prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
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
            }
            catch
            {
                return;
            }

            SelectedProject = new tblProject() { prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                SelectedProject.prjCreatorIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId;
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
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedProject, (int)SelectedProject.prjCreatorIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblProject>())
            {
                try
                {
                    tblProject result = db.GetModelByExpression(p => p.prjIdPk == SelectedProject.prjIdPk).First();

                    if (result != null)
                    {
                        db.DeleteModelById(result.prjIdPk);
                    }

                    _events.PublishOnUIThreadAsync(new ChangeUserMessage((int)((ShellViewModel)IoC.Get<IShell>()).UserId, ""));
                    LoadData((int)((ShellViewModel)IoC.Get<IShell>()).UserId);

                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
                }
                finally
                {
                }

            }
        }

        /// <summary>
        /// Event that is fired, when the selected project changes
        /// </summary>
        private void OnSelectedProjectChanged()
        {
            using (var db = new ApirsRepository<tblPerson>())
            {
                try
                {
                    Participants = new BindableCollection<tblPerson>(db.GetPersonByProject(SelectedProject.prjIdPk));
                }
                catch
                {
                    Participants = new BindableCollection<tblPerson>();
                }
            }
        }

        //Adding a participant to a project
        public void AddParticipant()
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Users can't be added in local mode.");
                return;
            }

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedProject, (int)SelectedProject.prjCreatorIdFk, SelectedProject.prjIdPk))
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
                PersonsViewModel personsViewModel = new PersonsViewModel(SelectedProject.prjIdPk);
                WindowManager windowManager = new WindowManager();
                windowManager.ShowDialog(personsViewModel);

                if (personsViewModel.SelectedPerson != null)
                {
                    var selectedPerson = personsViewModel.SelectedPerson;
                    using (var db = new ApirsDatabase())
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO tblPersonsProjects(persIdFk, prjIdFk) VALUES ("
                            + selectedPerson.persIdPk.ToString()
                            + ", "
                            + SelectedProject.prjIdPk.ToString()
                            + ");");
                    }

                    _events.PublishOnUIThreadAsync(new SendUserMessageMessage
                        (
                            26,
                            selectedPerson.persIdPk,
                            "New project", "You were added to the project " + SelectedProject.prjName + ".",
                            DateTime.Now
                        ));


                    Refresh();
                }
                else
                {
                    return;
                }

            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
            }
        }

        /// <summary>
        /// Removing a participant based on his id and the actually selected project
        /// </summary>
        /// <param name="id"></param>
        public void RemoveParticipant(int id)
        {
            if((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Users can't be removed in local mode.");
                return;
            }

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedProject, (int)SelectedProject.prjCreatorIdFk, SelectedProject.prjIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (SelectedProject != null)
            {
                try
                {
                    using (var db = new ApirsDatabase())
                    {
                        db.Database.ExecuteSqlCommand("DELETE FROM tblPersonsProjects WHERE persIdFk="
                            + id.ToString()
                            + " AND prjIdFk ="
                            + SelectedProject.prjIdPk.ToString()
                            + ";");
                    }
                    _events.PublishOnUIThreadAsync(new SendUserMessageMessage
                        (
                            26,
                            id,
                            "Removed from project " + SelectedProject.prjName,
                            "You were removed from the project " + SelectedProject.prjName,
                            DateTime.Now
                        ));

                    Refresh();
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
                }
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// Unsubscribing the actual user based on the project id 
        /// </summary>
        /// <param name="id"></param>
        public void Unsubscribe(int id)
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("You can't unsubscribe in local mode.");
                return;
            }

            try
            {
                using (var db = new ApirsDatabase())
                {
                    db.Database.ExecuteSqlCommand("DELETE FROM tblPersonsProjects WHERE persIdFk="
                        + ((int)((ShellViewModel)IoC.Get<IShell>()).UserId).ToString()
                        + " AND prjIdFk ="
                        + id
                        + ";");
                }

                _events.PublishOnUIThreadAsync(new ChangeUserMessage((int)((ShellViewModel)IoC.Get<IShell>()).UserId, ""));
                Refresh();
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
            }

        }

        /// <summary>
        /// Go to the last dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Last()
        {
            if (Projects.Count != 0)
                SelectedProject = Projects.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (Projects.Count != 0)
                SelectedProject = Navigation.GetPrevious(Projects, SelectedProject);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (Projects.Count != 0)
                SelectedProject = Navigation.GetNext(Projects, SelectedProject);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (Projects.Count != 0)
                SelectedProject = Projects.First();
        }


        /// <summary>
        /// Checking the uniqueness of the name in the database
        /// </summary>
        public void CheckUniqueness()
        {
            int count;

            try
            {
                using (var db = new ApirsRepository<tblProject>())
                {
                    count = db.GetModelByExpression(x => x.prjName == SelectedProject.prjName && x.prjIdPk != SelectedProject.prjIdPk).Count();

                    if (count == 0)
                        return;

                    char a = 'A';

                    while (count > 0)
                    {
                        SelectedProject.prjName = SelectedProject.prjName + a.ToString();

                        count = db.GetModelByExpression(x => x.prjName == SelectedProject.prjName && x.prjIdPk != SelectedProject.prjIdPk).Count();
                        a++;
                    }

                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. We provided a valid alternative for it:" + Environment.NewLine
                        + SelectedProject.prjName);
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError("An unexpeced error occured.");
            }
        }
        #endregion
    }
}
