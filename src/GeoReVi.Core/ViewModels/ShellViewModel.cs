using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    [Export(typeof(IShell))]
    public class ShellViewModel : Conductor<object>,
        IShell,
        IHandle<ChangeViewModelMessage>,
        IHandle<ChangeUserMessage>,
        IHandle<OpenDataWindowMessage>,
        IHandle<ExportControlMessage>,
        IHandle<SendUserMessageMessage>,
        IHandle<IsLoadingMessage>,
        IHandle<MessageBoxMessage>
    {
        #region Private members

        //User id of the logged in user
        private int userId = 0;

        private string userFullName = "Logged out";

        /// <summary>
        /// Status text members
        /// </summary>
        private string statusText = "Status: Connected";
        private System.Windows.Media.Brush statusTextColor = System.Windows.Media.Brushes.Green;

        //The currently active item
        private string currentActiveItem = "LoginView";

        //EventAggregator
        public readonly IEventAggregator _events;

        //Application main window manager
        private IWindowManager _windowManager;

        //Check if local or server mode
        private bool localMode = false;

        //Checks if data is loading
        private bool isLoading;

        //Running processes
        private Dictionary<object, bool> processes = new Dictionary<object, bool>();

        //A project bindable collection
        private BindableCollection<tblProject> projects = new BindableCollection<tblProject>();

        //The selected project
        private tblProject selectedProject = new tblProject();

        #endregion

        #region Public properties

        /// <summary>
        /// Side menu where all logging-functionality and project navigation functionality is located
        /// </summary>
        private SideMenuViewModel sideMenuViewModel;
        public SideMenuViewModel SideMenuViewModel
        {
            get => this.sideMenuViewModel;
            set
            {
                this.sideMenuViewModel = value;
                NotifyOfPropertyChange(() => SideMenuViewModel);
            }
        }

        /// <summary>
        /// View model for data management and visualization
        /// </summary>
        private DatasetManagementAndVisualizationViewModel datasetManagementAndVisualizationViewModel = new DatasetManagementAndVisualizationViewModel();
        public DatasetManagementAndVisualizationViewModel DatasetManagementAndVisualizationViewModel
        {
            get => this.datasetManagementAndVisualizationViewModel;
            set
            {
                this.datasetManagementAndVisualizationViewModel = value;
                NotifyOfPropertyChange(() => DatasetManagementAndVisualizationViewModel);
            }
        }

        //Status text
        public string StatusText
        {
            get { return this.statusText; }
            set { this.statusText = value; NotifyOfPropertyChange(() => StatusText); }
        }
        public System.Windows.Media.Brush StatusTextColor
        {
            get { return this.statusTextColor; }
            set { this.statusTextColor = value; NotifyOfPropertyChange(() => StatusTextColor); }
        }

        /// <summary>
        /// The selected project
        /// </summary>
        public tblProject SelectedProject
        {
            get
            {
                if (SideMenuViewModel.SelectedProject != null)
                    return SideMenuViewModel.SelectedProject;
                else
                    return new tblProject();
            }
        }

        /// <summary>
        /// The selected project
        /// </summary>
        private BindableCollection<tblProject> selectedProjects;
        public BindableCollection<tblProject> SelectedProjects
        {
            get
            {
                if (SideMenuViewModel.SelectedProjects != null)
                    return SideMenuViewModel.SelectedProjects;
                else
                    return new BindableCollection<tblProject>();
            }
        }

        /// <summary>
        /// The projects, the logged in user participates
        /// </summary>
        public BindableCollection<tblProject> Projects
        {
            get
            {
                if (SideMenuViewModel.Projects != null)
                    return SideMenuViewModel.Projects;
                else
                    return new BindableCollection<tblProject>();
            }
        }

        /// <summary>
        /// Cancellation tokens that are injected in background threads
        /// </summary>
        private List<CancellationTokenSource> cts = new List<CancellationTokenSource>();
        public List<CancellationTokenSource> Cts
        {
            get => this.cts;
            set
            {
                this.cts = value;
            }
        }

        /// <summary>
        /// The current active item
        /// </summary>
        public string CurrentActiveItem
        {
            get
            {
                return this.currentActiveItem;
            }
            set
            {
                this.currentActiveItem = value;
                NotifyOfPropertyChange(() => CurrentActiveItem);
            }
        }

        //The logged in user ID
        public int UserId
        {
            get
            {
                return this.userId;
            }
            private set
            {
                this.userId = value;
                NotifyOfPropertyChange(() => UserId);
                NotifyOfPropertyChange(() => IsLoggedInOrLocalMode);
            }
        }

        //Users full name
        public string UserFullName
        {
            get
            {
                return this.userFullName;
            }
            private set
            {
                this.userFullName = value;
                NotifyOfPropertyChange(() => UserFullName);
            }
        }

        /// <summary>
        /// Check if application runs in local mode
        /// </summary>
        public bool LocalMode
        {
            get
            {
                return this.localMode;
            }
            set
            {
                this.localMode = value;

                if (value != null)
                {
                    LoadView("LoginView");
                    ChangeUser(0, "Logged out");
                }


                NotifyOfPropertyChange(() => LocalMode);
                NotifyOfPropertyChange(() => IsLoggedInOrLocalMode);
            }
        }

        /// <summary>
        /// Property to determine if data is loading
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }
            set
            {
                this.isLoading = value;
                NotifyOfPropertyChange(() => IsLoading);
            }
        }

        private bool isBooting = false;
        /// <summary>
        /// Property to determine if shell view model is actually booting
        /// </summary>
        public bool IsBooting
        {
            get
            {
                return this.isBooting;
            }
            set
            {
                this.isBooting = value;
                NotifyOfPropertyChange(() => IsBooting);
            }
        }

        /// <summary>
        /// Checks if a user is logged in or in local mode
        /// </summary>
        public bool IsLoggedInOrLocalMode
        {
            get { return LocalMode || UserId != 0; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Instantiating the Shellviewmodel and subscribe to the eventaggregator
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(bool _isBooting = false)
        {
            IsBooting = _isBooting;

            this._windowManager = new WindowManager();
            this._events = new EventAggregator();

            _events.Subscribe(this);

            NetworkChange.NetworkAddressChanged += new
            NetworkAddressChangedEventHandler(AddressChangedCallback);

            CheckNetwork();

            SideMenuViewModel = new SideMenuViewModel();

            ChangeUser(0, "Logged out");
            //LoadView("LoginView");


            LocalMode = false;
            IsLoading = false;
            IsBooting = false;

            UseLocalMode();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Changing the user ID
        /// </summary>
        /// <param name="userId"></param>
        public void ChangeUser(int userId, string fullName)
        {
            UserId = userId;

            if (fullName != "")
                UserFullName = fullName;

            SideMenuViewModel.LoadProjects(userId);
        }

        public void LoadView()
        {
            LoadView(CurrentActiveItem);
        }

        /// <summary>
        /// Loading a viewmodel based on an input parameter
        /// </summary>
        /// <param name="activeItem"></param>
        public void LoadView(string activeItem)
        {
            this.CurrentActiveItem = activeItem;
            DeactivateItem(ActiveItem, true);
            //CheckNetwork();

            switch (activeItem)
            {
                case "HomeView":
                    ActivateItem(new HomeViewModel());
                    break;
                case "LoginView":
                    ActivateItem(new LoginViewModel(_events));
                    break;
                case "MapViewWrapView":
                    ActivateItem(new MapViewWrapViewModel());
                    break;
                case "RegisterView":
                    ActivateItem(new RegisterViewModel(_events));
                    break;
                case "RockSampleDetailsView":
                    ActivateItem(new RockSampleDetailsViewModel(_events));
                    break;
                case "ObjectDetailsView":
                    ActivateItem(new ObjectDetailsViewModel(_events));
                    break;
                case "LithologyDetailsView":
                    ActivateItem(new LithologiesWrapViewModel(_events));
                    break;
                case "MeasurementWrapView":
                    ActivateItem(new MeasurementWrapViewModel(_events));
                    break;
                case "ProjectsView":
                    ActivateItem(new ProjectsViewModel(_events));
                    break;
                case "UserView":
                    if (UserId != 0)
                    {
                        ActivateItem(new UserViewModel(_events));
                        break;
                    }
                    Handle(new MessageBoxMessage("Please login first", "", MessageBoxViewType.Information, MessageBoxViewButton.Ok));
                    break;
                case "LithologicalSectionView":
                    ActivateItem(new LithologicalSectionViewModel(_events));
                    break;
                case "AboutView":
                    ActivateItem(new AboutViewModel());
                    break;
                default:
                    ActivateItem(new LoginViewModel(_events));
                    break;
            }

        }

        /// <summary>
        /// Logging the user out
        /// </summary>
        public void Logout()
        {
            try
            {
                if (UserId != 0)
                    UserId = 0;

                if (SideMenuViewModel.SelectedProject != null)
                    SideMenuViewModel.SelectedProject = new tblProject();

                if (SideMenuViewModel.Projects != new BindableCollection<tblProject>())
                    SideMenuViewModel.Projects = new BindableCollection<tblProject>();
            }
            catch
            {

            }
            finally
            {
                LoadView("LoginView");

                ChangeUser(0, "Logged out");

                if (LocalMode)
                    LocalMode = !LocalMode;

            }


        }

        private async void AddressChangedCallback(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                Task.Delay(3000);
                CheckNetwork();
            });
        }

        /// <summary>
        /// Opening the charts window
        /// </summary>
        public void OpenChartWindow()
        {
            AdditionalShellViewModel additionalShellViewModel = new AdditionalShellViewModel(_events, "ChartWrapView");
            this._windowManager.ShowWindow(additionalShellViewModel);
        }

        //Method to open a details windows dependend on the current item type
        public void ForwardShortCut(string shortcutCharacters)
        {
            if (shortcutCharacters != "")
                if (CurrentActiveItem != "LoginView" && CurrentActiveItem != "HomeView")
                    _events.PublishOnUIThreadAsync(new ShortCutMessage(shortcutCharacters));
        }

        /// <summary>
        /// Opening the documentation
        /// </summary>
        public void OpenDocumentation()
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/ApirsAL/GeoReVi/blob/master/docs/GeoReViUserManual.pdf");
            }
            catch (Exception e)
            {
                Handle(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok));
            }
        }

        /// <summary>
        /// Go into local mode
        /// </summary>
        public void UseLocalMode()
        {
            LocalMode = true;
            _events.PublishOnUIThreadAsync(new ChangeUserMessage(Convert.ToInt32(0), "Local mode"));
            //Changing the viewmodel to the homeview
            _events.PublishOnUIThreadAsync(new ChangeViewModelMessage("HomeView"));
        }

        public async Task UpdateLocalDB()
        {
            try
            {
                await Task.WhenAll(new ApirsRepository<tblUnionChronostratigraphy>().UpdateLocalDatabase());
            }
            catch
            {

            }
        }

        /// <summary>
        /// Showing a question dialog view
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public MessageBoxViewResult ShowQuestion(string question, string details = "")
        {
            MessageBoxViewModel messageBoxViewModel = new MessageBoxViewModel(_events,
                                                                                question,
                                                                                details,
                                                                                MessageBoxViewType.Question,
                                                                                MessageBoxViewButton.YesNo);
            this._windowManager.ShowDialog(messageBoxViewModel);

            try
            {
                return messageBoxViewModel.Result;
            }
            catch
            {
                return MessageBoxViewResult.No;
            }
        }

        /// <summary>
        /// Showing a question dialog view
        /// </summary>
        /// <param name="question"></param>
        /// <returns></returns>
        public void ShowLocationValueImport(ref DataTable dt)
        {
            AdditionalShellViewModel imp = new AdditionalShellViewModel(_events, new ImportLocationValuesViewModel(this._events, dt));
            this._windowManager.ShowDialog(imp);
        }

        /// <summary>
        /// Showing an error message
        /// </summary>
        /// <param name="error"></param>
        public void ShowError(string errorText)
        {
            try
            {
                new DispatchService().Invoke(
                () =>
                {
                    MessageBoxViewModel messageBoxViewModel = new MessageBoxViewModel(_events,
                                                                                      errorText,
                                                                                      "",
                                                                                      MessageBoxViewType.Error,
                                                                                      MessageBoxViewButton.Ok);
                    this._windowManager.ShowDialog(messageBoxViewModel);
                });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Showing an error message
        /// </summary>
        /// <param name="error"></param>
        public async void LogError(Exception error)
        {
            if (!IsBooting)
                try
                {
                    CommandHelper ch = new CommandHelper();

                    await ch.RunBackgroundWorkerHelperAsync(async () =>
                    {
                        ((BaseLogFactory)IoC.Get<ILogFactory>()).Log(error.Message, LogLevel.Error, error.StackTrace, error.Source);

                    });
                }
                catch
                {
                }
        }

        /// <summary>
        /// Showing an error message
        /// </summary>
        /// <param name="error"></param>
        public void ShowInformation(string informationText)
        {
            try
            {
                new DispatchService().Invoke(
                () =>
                {
                    MessageBoxViewModel messageBoxViewModel = new MessageBoxViewModel(_events,
                                                                                      informationText,
                                                                                      "",
                                                                                      MessageBoxViewType.Information,
                                                                                      MessageBoxViewButton.Ok);
                    this._windowManager.ShowDialog(messageBoxViewModel);
                });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Showing an error messageD:\OneDrive\Promotion\Promotionsdurchfuehrung\FrontendDev\WPF\APIRS_Visual\APIRS_Visual\APIRS.Core\ViewModels\ShellViewModel.cs
        /// </summary>
        /// <param name="error"></param>
        public void OpenGenericWindow<T>(T vm) where T : class, new()
        {
            try
            {
                new DispatchService().Invoke(
                () =>
                {
                    this._windowManager.ShowDialog(new GenericViewModel<T>(vm));
                });
            }
            catch
            {
            }
        }
        #endregion

        #region Handler from other viewmodels

        /// <summary>
        /// Loading the next view based on the message sent from another view model
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ChangeViewModelMessage message)
        {
            LoadView(message.View);
        }

        /// <summary>
        /// Loading the next view based on the message sent from another view model
        /// </summary>
        /// <param name="message"></param>
        public void Handle(MessageBoxMessage message)
        {
            try
            {
                new DispatchService().Invoke(
                () =>
                {
                    MessageBoxViewModel messageBoxViewModel = new MessageBoxViewModel(_events,
                                                                                                    message.MessageText,
                                                                                                    message.DetailsText,
                                                                                                    message.MessageBoxType,
                                                                                                    message.MessageBoxButton);
                    this._windowManager.ShowDialog(messageBoxViewModel);
                });
            }
            catch
            {
            }
        }

        /// <summary>
        /// Changes the user
        /// </summary>
        /// <param name="message"></param>
        public void Handle(ChangeUserMessage message)
        {
            ChangeUser(message.UserId, message.FullName);
        }

        /// <summary>
        /// Changes the user
        /// </summary>
        /// <param name="message"></param>
        public void Handle(SendUserMessageMessage message)
        {
            try
            {
                using (var db2 = new ApirsRepository<tblMessage>())
                {
                    db2.InsertModel(new tblMessage()
                    {
                        messFromPersonIdFk = message.FromUser,
                        messToPersonIdFk = message.ToUser,
                        messHeader = message.HeaderText,
                        messPlainText = StringCipher.Encrypt(message.PlainText, "helloPhrase"),
                        messDate = message.MessageDate
                    });

                    db2.Save();
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        public void Handle(OpenDataWindowMessage message)
        {
            string type = "";

            switch (message.DataObject.GetType().ToString())
            {
                //Objects of investigation forms
                case "GeoReVi.tblOutcrop":
                    type = "Outcrop";
                    break;
                case "GeoReVi.tblDrilling":
                    type = "Drilling";
                    break;
                case "GeoReVi.tblTransect":
                    type = "Transect";
                    break;

                //Lithology forms
                case "GeoReVi.tblLithofacy":
                    type = "Siliciclastic";
                    break;
                case "GeoReVi.tblBiochemicalFacy":
                    type = "Biochemical";
                    break;
                case "GeoReVi.tblVolcanicFacy":
                    type = "Volcanic";
                    break;
                case "GeoReVi.tblIgneousFacy":
                    type = "Igneous";
                    break;

                //Rock sample forms
                case "GeoReVi.tblPlug":
                    type = "Plug";
                    break;
                case "GeoReVi.tblCuboid":
                    type = "Cuboid";
                    break;
                case "GeoReVi.tblHandpiece":
                    type = "Handpiece";
                    break;
                case "GeoReVi.tblThinSection":
                    type = "ThinSection";
                    break;
                case "GeoReVi.tblPowder":
                    type = "Powder";
                    break;

                //Stratigraphy forms
                case "GeoReVi.tblBasin":
                    type = "Basin";
                    break;
                case "GeoReVi.LithostratigraphyUnion":
                    type = "Lithostratigraphy";
                    break;

                //DataSets
                case "GeoReVi.tblRockSample":
                    if (message.Message.Substring(Math.Max(0, message.Message.Length - 4)) == ".csv" ||
                        message.Message.Substring(Math.Max(0, message.Message.Length - 5)) == ".xlsx")
                    {
                        type = "RockSamples";
                    }

                    break;
                case "GeoReVi.tblLaboratoryMeasurement":
                    if (message.Message.Substring(Math.Max(0, message.Message.Length - 4)) == ".csv" ||
                        message.Message.Substring(Math.Max(0, message.Message.Length - 5)) == ".xlsx")
                    {
                        type = "LaboratoryMeasurements";
                    }

                    break;
            }

            if (type != "")
            {
                AdditionalShellViewModel additionalShellViewModel = new AdditionalShellViewModel(_events, message.Message, type);
                this._windowManager.ShowDialog(additionalShellViewModel);
            }
        }

        //Showing the import window dialog
        public void ShowImportWindow<T>(ImportProcedureViewModel<T> importProcedureViewModel)
            where T : class, new()
        {
            AdditionalShellViewModel<T> imp = new AdditionalShellViewModel<T>(_events, importProcedureViewModel);
            this._windowManager.ShowDialog(imp);
        }
        //Showing the import window dialog
        public void ShowImportWindow<T, U>(ImportProcedureViewModel<T, U> importProcedureViewModel)
            where T : class, new()
            where U : class, new()
        {
            AdditionalShellViewModel<T, U> imp = new AdditionalShellViewModel<T, U>(_events, importProcedureViewModel);
            this._windowManager.ShowDialog(imp);
        }

        /// <summary>
        /// Exiting the actual application instance
        /// </summary>
        public void Exit()
        {
            TryClose();
        }

        /// <summary>
        /// Checks network access
        /// </summary>
        private void CheckNetwork()
        {

            //Loading the project data
            if (!ServerInteractionHelper.IsNetworkAvailable(50) || !ServerInteractionHelper.TryAccessDatabase())
            {
                StatusText = "Status: Not connected";
                StatusTextColor = System.Windows.Media.Brushes.Red;
            }
            else
            {
                StatusText = "Status: Connected";
                StatusTextColor = System.Windows.Media.Brushes.Green;
            }
        }

        public void Handle(ExportControlMessage message)
        {
            switch (message.Format)
            {
                case "pdf":
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "PDF (*.pdf)|*.pdf";
                    saveFileDialog.RestoreDirectory = true;

                    saveFileDialog.ShowDialog();

                    if (saveFileDialog.FileName != "")
                    {
                        try
                        {
                            //Getting the extension
                            var ext = saveFileDialog.FileName.Substring(saveFileDialog.FileName.LastIndexOf(".")).ToLower();

                            switch (ext.ToString())
                            {
                                case ".pdf":
                                    List<Bitmap> bm = new List<Bitmap>();
                                    var encoder = new PngBitmapEncoder();
                                    bm.Add(ImageCapturer.UIElementToBitmap((UIElement)message.Element, encoder, 250));
                                    ExportHelper.ExportImageToPdf(bm, saveFileDialog.FileName);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                        }
                    }
                    break;
                case "image":

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "PNG (*.png)|*.png|BMP (*.bmp)|*.bmp|EMF (*.emf)|*.emf|PDF (*.pdf)|*.pdf|CSV (*.csv)|*.csv";
                    saveFileDialog1.RestoreDirectory = true;

                    saveFileDialog1.ShowDialog();

                    if (saveFileDialog1.FileName != "")
                    {
                        //Getting the extension
                        var ext = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                        try
                        {
                            //Getting the extension
                            var ext1 = saveFileDialog1.FileName.Substring(saveFileDialog1.FileName.LastIndexOf(".")).ToLower();

                            switch (ext1.ToString())
                            {
                                case ".png":
                                    //Downloading to the specific folder
                                    ImageCapturer.SaveToPng((FrameworkElement)message.Element, saveFileDialog1.FileName);
                                    break;
                                case ".bmp":
                                    ImageCapturer.SaveToBmp((FrameworkElement)message.Element, saveFileDialog1.FileName);
                                    break;
                                case ".emf":
                                    ImageCapturer.SaveToEmf((UIElement)message.Element, saveFileDialog1.FileName);
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).LogError(e);
                        }
                    }
                    break;
            }

        }

        public void Handle(IsLoadingMessage message)
        {
            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += ((sender1, args) =>
            {

                new DispatchService().Invoke(
                async () =>
                {
                    if (processes.ContainsKey(message.ProcessId))
                    {
                        processes.Remove(message.ProcessId);
                    }
                    else
                    {
                        processes.Add(message.ProcessId, true);
                    }

                    IsLoading = processes.Count > 0 ? true : false;
                });

            });

            bw.RunWorkerCompleted += ((sender1, args) =>
            {
                if (args.Error != null)  // if an exception occurred during DoWork,
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

            });

            bw.RunWorkerAsync(); // start the background worker

        }

        public void SynchronizeLocalData()
        {
            try
            {
                using (var db = new ApirsRepository<tblUnionChronostratigraphy>())
                {
                    db.SynchronizeLocalDatabase();
                }
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Stops the loading event
        /// </summary>
        public void StopLoading()
        {
            foreach (CancellationTokenSource cts in Cts)
            {
                cts.Cancel();
            }

            Cts = new List<CancellationTokenSource>();

            this.IsLoading = false;
        }

        #endregion
    }

    public interface IShell
    {

    }
}
