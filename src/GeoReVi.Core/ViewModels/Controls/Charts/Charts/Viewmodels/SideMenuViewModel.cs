using Caliburn.Micro;
using System.Linq;

namespace GeoReVi
{
    /// <summary>
    /// Viewmodel for the side menu
    /// </summary>
    public class SideMenuViewModel : PropertyChangedBase
    {

        #region Properties

        /// <summary>
        /// A single instance of the view model
        /// </summary>
        public static SideMenuViewModel Instance => new SideMenuViewModel();

        /// <summary>
        /// The selected project
        /// </summary>
        private tblProject selectedProject = new tblProject();
        public tblProject SelectedProject
        {
            get
            {
                return selectedProject;
            }
            set
            {
                selectedProject = value;
                if (selectedProject != null)
                    ((ShellViewModel)IoC.Get<IShell>()).LoadView();
                NotifyOfPropertyChange(() => SelectedProject);
            }
        }

        /// <summary>
        /// All projects
        /// </summary>
        private BindableCollection<tblProject> projects = new BindableCollection<tblProject>();
        public BindableCollection<tblProject> Projects
        {
            get => projects;
            set
            {
                projects = value;
                NotifyOfPropertyChange(() => Projects);
            }
        }

        /// <summary>
        /// All projects
        /// </summary>
        private BindableCollection<tblProject> selectedProjects = new BindableCollection<tblProject>();
        public BindableCollection<tblProject> SelectedProjects
        {
            get => selectedProjects;
            set
            {
                selectedProjects = value;

                if (selectedProjects != null)
                    ((ShellViewModel)IoC.Get<IShell>()).LoadView();

                NotifyOfPropertyChange(() => SelectedProjects);
            }
        }

        /// <summary>
        /// Messages for the user
        /// </summary>
        private BindableCollection<tblMessage> messages = new BindableCollection<tblMessage>();
        public BindableCollection<tblMessage> Messages
        {
            get
            {
                return this.messages;
            }
            set
            {
                this.messages = value;
                NotifyOfPropertyChange(() => Messages);
            }
        }

        private LogViewModel logViewModel = new LogViewModel();
        public LogViewModel LogViewModel
        {
            get => this.logViewModel;
            set
            {
                this.logViewModel = value;
                NotifyOfPropertyChange(() => LogViewModel);
            }
        }

        #endregion

        #region Constructor

        public SideMenuViewModel()
        {

        }

        #endregion

        #region Methods

        //Load the projects datasets
        public void LoadProjects(int userId = 0)
        {
            try
            {
                if(userId == 0)
                {
                    Projects = new BindableCollection<tblProject>(new ApirsRepository<tblProject>().GetUserProjects(userId));
                    SelectedProject = Projects.First();
                }
                else
                {
                    Projects = new BindableCollection<tblProject>(new ApirsRepository<tblProject>().GetUserProjects(userId));
                    Projects.AddRange(new ApirsRepository<tblProject>().GetProjectByParticipation(userId).ToList());
                    SelectedProject = Projects.First();
                }
            }
            catch
            {
            }

        }

        #endregion

    }
}
