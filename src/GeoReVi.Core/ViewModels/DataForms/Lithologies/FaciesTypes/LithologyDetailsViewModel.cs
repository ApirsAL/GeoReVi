using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GeoReVi
{
    public class LithologyDetailsViewModel : Screen, IHandle<ShortCutMessage>
    {
        #region Private members

        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;

        //Petrography colleciton
        private BindableCollection<tblUnionPetrography> unionPetrography;

        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;

        //Facies codes collection
        private BindableCollection<tblFaciesCode> allFaciesCodes;
        private BindableCollection<tblFaciesCode> faciesCodes;

        //Facy lithostrat collection
        private BindableCollection<tblFaciesLithostrat> faciesLithostrat;

        //Facy occurences collection
        private BindableCollection<tblFaciesObservation> faciesObservations;

        //FaciesTypes of investigation collection
        private BindableCollection<tblFacy> faciesTypes;

        //Pictures
        private BindableCollection<v_PictureStore> pictureStore;
        private v_PictureStore selectedPictureStore;
        private BitmapImage selectedImage;

        //Cancellation token for async methods
        CancellationTokenSource cts;

        /// <summary>
        /// Visibility members
        /// </summary>
        private bool isImageLoading = false;

        /// <summary>
        /// Selected FaciesTypes
        /// </summary>
        private tblFacy selectedFacy;


        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

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
        /// The selected Facy
        /// </summary>
        public tblFacy SelectedFacy
        {
            get
            {
                return this.selectedFacy;
            }
            set
            {
                try
                {
                    if (SelectedFacy.facIdPk != 0)
                        Update();
                }
                catch { }

                this.selectedFacy = value;

                if (value != null && SelectedFacy.facIdPk != 0)
                {
                    Initialization = OnSelectedFacyChanged();
                }

                NotifyOfPropertyChange(() => SelectedFacy);
                NotifyOfPropertyChange(() => FaciesLithostrat);
                NotifyOfPropertyChange(() => FaciesObservations);
                NotifyOfPropertyChange(() => SelectedFacyIndex);
            }
        }

        /// <summary>
        /// checks if image is loading
        /// </summary>
        public bool IsImageLoading
        {
            get { return this.isImageLoading; }
            set { this.isImageLoading = value; NotifyOfPropertyChange(() => IsImageLoading); }
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
        public BindableCollection<tblFaciesLithostrat> FaciesLithostrat
        {
            get
            {
                try
                {
                    return new BindableCollection<tblFaciesLithostrat>((from ooilith in this.faciesLithostrat
                                                                        where ooilith.facIdFk == SelectedFacy.facIdPk
                                                                        select ooilith));
                }
                catch
                {
                    return new BindableCollection<tblFaciesLithostrat>();
                }
            }
            set
            {
                this.faciesLithostrat = value;
                NotifyOfPropertyChange(() => FaciesLithostrat);
            }
        }

        /// <summary>
        /// facies observations based on the selected facies type
        /// </summary>
        public BindableCollection<tblFaciesObservation> FaciesObservations
        {
            get
            {
                try
                {
                    return new BindableCollection<tblFaciesObservation>((from obs in this.faciesObservations
                                                                         where obs.fofacIdFk == SelectedFacy.facIdPk
                                                                         select obs).ToList());
                }
                catch
                {
                    return new BindableCollection<tblFaciesObservation>();
                }
            }
            set
            {
                this.faciesObservations = value;
                NotifyOfPropertyChange(() => FaciesObservations);
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

        //All petrographic terms
        public BindableCollection<tblUnionPetrography> UnionPetrography
        {
            get { return this.unionPetrography; }
            set { this.unionPetrography = value; NotifyOfPropertyChange(() => UnionPetrography); }
        }

        //All facies codes
        public BindableCollection<tblFaciesCode> FaciesCodes
        {
            get { return this.faciesCodes; }
            set
            {
                this.faciesCodes = value;
                NotifyOfPropertyChange(() => FaciesCodes);
            }
        }

        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<tblFacy> FaciesTypes
        {
            get { return this.faciesTypes; }
            set
            {
                this.faciesTypes = value;
                NotifyOfPropertyChange(() => FaciesTypes);
                NotifyOfPropertyChange(() => CountFaciesTypes);
            }
        }

        /// <summary>
        /// Readonly count of the FaciesTypes
        /// </summary>
        public string CountFaciesTypes
        {
            get
            {
                try
                {
                    if (FaciesTypes != null)
                        return FaciesTypes.Count.ToString();
                    if (FaciesTypes.Count > 0)
                        return FaciesTypes.Count.ToString();
                }
                catch
                {
                    return "0";
                }

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountFaciesTypes);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedFacyIndex
        {
            get
            {
                if (SelectedFacy != null)
                    return (FaciesTypes.IndexOf(SelectedFacy) + 1).ToString();

                return "0";
            }
            set
            {
                OnSelectedFacyIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedFacyIndex);
            }
        }

        public BindableCollection<v_PictureStore> PictureStore
        {
            get
            {
                return this.pictureStore;
            }
            set
            {
                this.pictureStore = value;
                NotifyOfPropertyChange(() => PictureStore);
                NotifyOfPropertyChange(() => CountImagesFacy);
            }
        }

        //The selected image file
        public v_PictureStore SelectedPictureStore
        {
            get
            {
                return this.selectedPictureStore;
            }
            set
            {
                this.selectedPictureStore = value;
                OnSelectedImageFacyChanged();
                NotifyOfPropertyChange(() => SelectedPictureStore);
                NotifyOfPropertyChange(() => SelectedImageFacyIndex);
            }
        }

        /// <summary>
        /// Creating an image source out of unc paths of the images
        /// </summary>
        public BitmapImage SelectedImage
        {
            get
            {
                return this.selectedImage;
            }
            set
            {
                this.selectedImage = value;
                NotifyOfPropertyChange(() => SelectedImage);
            }
        }

        /// <summary>
        /// Readonly count of the FaciesTypes
        /// </summary>
        public string CountImagesFacy
        {
            get
            {
                if (PictureStore != null)
                    return PictureStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountImagesFacy);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedImageFacyIndex
        {
            get
            {
                if (SelectedPictureStore != null)
                    return (PictureStore.IndexOf(SelectedPictureStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedImageFacyIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedImageFacyIndex);
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

            Type = "Siliciclastic";

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public LithologyDetailsViewModel()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public LithologyDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            Initialization = InitializeAsync();

        }

        #endregion

        #region Methods

        //Loading the static data
        private async Task LoadData()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {

                try
                {
                    try
                    {
                        Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(new ApirsRepository<tblUnionChronostratigraphy>().GetModel());
                    }
                    catch
                    {
                        Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                    }
                    try
                    {
                        UnionPetrography = new BindableCollection<tblUnionPetrography>(new ApirsRepository<tblUnionPetrography>().GetModel());
                    }
                    catch
                    {
                        UnionPetrography = new BindableCollection<tblUnionPetrography>();
                    }

                    try
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(new ApirsRepository<tblUnionLithostratigraphy>().GetCompleteLithostratigraphy());
                    }
                    catch
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();

                    }
                    try
                    {
                        FaciesCodes = new BindableCollection<tblFaciesCode>(new ApirsRepository<tblFaciesCode>().GetModelByExpression(fc => fc.fcHierarchy == "Facies type"
                                                                             || fc.fcFaciesType == "All"
                                                                             && fc.fcHierarchy == "Facies type"
                                                                             || fc.fcCode.Contains("Unknown")));
                        this.allFaciesCodes = FaciesCodes;
                    }
                    catch
                    {
                        FaciesCodes = new BindableCollection<tblFaciesCode>();
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
        /// <param name="parameter">Facy type as string</param>
        public async void LoadData(string parameter)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                using (var db = new ApirsRepository<tblFaciesCode>())
                {
                    try
                    {
                        FaciesCodes = new BindableCollection<tblFaciesCode>(allFaciesCodes.Where(fc => fc.fcFaciesType == parameter));
                    }
                    catch
                    {
                        FaciesCodes = new BindableCollection<tblFaciesCode>();
                    }
                }

                try
                {
                    using (var db1 = new ApirsRepository<tblFacy>())
                    {
                        FaciesTypes = new BindableCollection<tblFacy>(db1.GetFaciesTypeByProject(Projects, parameter));
                    }
                }
                catch
                {
                    FaciesTypes = new BindableCollection<tblFacy>();
                }

                if (FaciesTypes.Count != 0) SelectedFacy = FaciesTypes.First();
                else SelectedFacy = new tblFacy() { facInterpreterId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
            });
        }


        /// <summary>
        /// Method that fires a task which downloads data from the server and converts it to bitmaps
        /// </summary>
        private async Task OnSelectedFacyChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                try
                {
                    using (var db = new ApirsRepository<tblFaciesLithostrat>())
                    {

                        //Assigning the newly generated types to the facies types to update the count directly
                        FaciesLithostrat = new BindableCollection<tblFaciesLithostrat>(db.GetModelByExpression(fl => fl.facIdFk == SelectedFacy.facIdPk));

                    }
                }
                catch
                {
                    FaciesLithostrat = new BindableCollection<tblFaciesLithostrat>();
                }

                try
                {
                    using (var db = new ApirsRepository<tblFaciesObservation>())
                    {
                        FaciesObservations = new BindableCollection<tblFaciesObservation>(db.GetModelByExpression(fo => fo.fofacIdFk == SelectedFacy.facIdPk));
                    }
                }
                catch
                {
                    FaciesObservations = new BindableCollection<tblFaciesObservation>();
                }
            });

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsImageLoading, async () =>
             {
                 PictureStore = await FileHelper.LoadImagesAsync(SelectedFacy.facIdPk, "Facies");
                 SelectedPictureStore = PictureStore.Count > 0 ? PictureStore.First() : new v_PictureStore();
             });

        }

        //Event that is fired when the index changed
        private void OnSelectedFacyIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != FaciesTypes.IndexOf(SelectedFacy))
                {
                    SelectedFacy = FaciesTypes.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }

        /// <summary>
        /// Downloading the selected image async
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private async void OnSelectedImageFacyChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {

                SelectedImage = FileHelper.LoadImageAsync(SelectedPictureStore.file_stream);
            });
        }


        /// <summary>
        /// If index is changed go to picture
        /// </summary>
        /// <param name="parameter"></param>
        private void OnSelectedImageFacyIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != PictureStore.IndexOf(SelectedPictureStore))
                {
                    SelectedPictureStore = PictureStore.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }

        /// <summary>
        /// Creates a png file of a selected bitmap image
        /// </summary>
        public void DownloadImage()
        {
            ///Configuring the savefiledialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG (*.png)|*.png |BMP (*.bmp)|*.bmp";
            saveFileDialog.RestoreDirectory = true;

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                //Downloading to the specific folder
                ImageCapturer.SaveToPng(SelectedImage, saveFileDialog.FileName);
            }
        }

        /// <summary>
        /// Uploading an image file to the database
        /// </summary>
        public void UploadImage()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesFacy) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per facies type");
                return;
            }

            try
            {
                //File dialog for opening a jpeg, png or bmp file
                OpenFileDialog openFileDlg = new OpenFileDialog();
                openFileDlg.Filter = @"JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";
                openFileDlg.RestoreDirectory = true;
                openFileDlg.ShowDialog();

                if (openFileDlg.FileName != "")
                {
                    using (var db = new ApirsRepository<tblPictureLithofacy>())
                    {
                        Tuple<Guid, string> result = db.UploadImage(openFileDlg.FileName);

                        if ((Guid)result.Item1 != Guid.Empty)
                        {
                            db.InsertModel(new tblPictureLithofacy() { lftIdFk = SelectedFacy.facIdPk, picStreamIdFk = (Guid)result.Item1, picName = (string)result.Item2 });
                            OnSelectedFacyChanged();
                        }
                    }
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
            }

        }

        //If an image file gets dropped on the image control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesFacy) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per Facies type");
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

            if (extension != ".PNG" && extension != ".JPG" && extension != ".BMP" && extension != ".jpg" && extension != ".png" && extension != ".bmp")
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please comply to the necessary file formats .png, .bmp or .jpg");
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblPictureLithofacy>())
                {
                    Tuple<Guid, string> result = db.UploadImage(fileName);

                    if ((Guid)result.Item1 != Guid.Empty)
                    {
                        db.InsertModel(new tblPictureLithofacy() { lftIdFk = SelectedFacy.facIdPk, picStreamIdFk = (Guid)result.Item1, picName = (string)result.Item2 });
                        OnSelectedFacyChanged();
                    }
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Deleting the currently selected image
        /// </summary>
        public void DeleteImage()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (CountImagesFacy == "0")
                return;

            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                try
                {
                    Guid guid = SelectedPictureStore.stream_id;

                    using (var db = new ApirsRepository<v_PictureStore>())
                    {
                        db.DeleteModelById(SelectedPictureStore.stream_id);
                    }
                    using (var db = new ApirsRepository<tblPictureRockSample>())
                    {
                        db.DeleteModelById(guid);
                    }
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }

                OnSelectedFacyChanged();
                return;
            }

            try
            {
                //Instantiate database
                using (ApirsDatabase db = new ApirsDatabase())
                {
                    //Establishing a sql connection
                    using (SqlConnection SqlConn = new SqlConnection(db.Database.Connection.ConnectionString.ToString()))
                    {
                        SqlCommand spDeleteImage = new SqlCommand("dbo.spDeleteImage", SqlConn);

                        //Preparing the stored procedure
                        spDeleteImage.CommandType = System.Data.CommandType.StoredProcedure;

                        //Adding the parameters
                        spDeleteImage.Parameters.Add("@file_name", SqlDbType.NVarChar, 255).Value = SelectedPictureStore.name;

                        //Opening the connection
                        SqlConn.Open();

                        //Deleting the image in the database
                        spDeleteImage.ExecuteNonQuery();

                        SqlConn.Close();

                        Refresh();
                    }
                }
            }
            catch
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
            if (FaciesTypes.Count != 0)
                SelectedFacy = FaciesTypes.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (FaciesTypes.Count != 0)
                SelectedFacy = Navigation.GetPrevious(FaciesTypes, SelectedFacy);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (FaciesTypes.Count != 0)
                SelectedFacy = Navigation.GetNext(FaciesTypes, SelectedFacy);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (FaciesTypes.Count != 0)
                SelectedFacy = FaciesTypes.First();
        }

        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviousImage()
        {
            if (PictureStore.Count != 0)
                SelectedPictureStore = Navigation.GetPrevious(PictureStore, SelectedPictureStore);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextImage()
        {
            if (PictureStore.Count != 0)
                SelectedPictureStore = Navigation.GetNext(PictureStore, SelectedPictureStore);
        }

        public void RefreshView()
        {
            Refresh();
        }

        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            tblFacy current = SelectedFacy;
            int id = 0;

            try
            {
                if (SelectedFacy != null)
                    id = SelectedFacy.facIdPk;

                LoadData(SelectedFacy.facType.ToString());

                if (id == 0) SelectedFacy = FaciesTypes.First();
                else SelectedFacy = FaciesTypes.Where(x => x.facIdPk == id).First();
            }
            catch (Exception e)
            {
                try
                {
                    LoadData("Siliciclastic");
                    if (FaciesTypes.Count != 0) SelectedFacy = FaciesTypes.First();
                    else SelectedFacy = FaciesTypes.Where(x => x.facIdPk == id).First();
                }
                catch (Exception ex)
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
            }
            catch
            {
                return;
            }

            SelectedFacy = new tblFacy() { facInterpreterId = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
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
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedFacy, (int)SelectedFacy.facInterpreterId))
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

            if (((ShellViewModel)IoC.Get<IShell>()).ShowQuestion("Are you sure to delete the record?") == MessageBoxViewResult.No)
            {
                return;
            }

            using (var db = new ApirsRepository<tblFacy>())
            {
                try
                {
                    tblFacy result = db.GetModelByExpression(b => b.facIdPk == SelectedFacy.facIdPk).First();

                    if (result != null)
                    {
                        db.DeleteModelById(result.facIdPk);
                    }
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Facies deleted.");

                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                }
                finally
                {
                    Refresh();
                }

            }
        }

        // Commit changes from the new Facy form
        // or edits made to the existing Facy form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedFacy, (int)SelectedFacy.facInterpreterId))
                {
                    return;
                }
            }
            catch
            {
                return;
            }
            using (var db = new ApirsRepository<tblFacy>())
            {
                try
                {
                    if (SelectedFacy.facIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedFacy);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Facies can't be added. Please check every field again.");
                            return;
                        }

                        OnSelectedFacyChanged();

                    }
                    else if (SelectedFacy.facCode != null)
                    {
                        //Only accessible if current user uploaded the Facy
                        try
                        {
                            if (SelectedFacy == null || SelectedFacy.facInterpreterId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                            {
                                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Facies. Please contact him via message service.");
                                return;
                            }
                        }
                        catch
                        {
                            return;
                        }

                        try
                        {
                            //Updating Facy information
                            db.UpdateModel(SelectedFacy, SelectedFacy.facIdPk);
                        }
                        catch (Exception ex)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                        }

                        using (var db1 = new ApirsRepository<tblFaciesLithostrat>())
                        {
                            ///Updating lithostratigraphy
                            foreach (tblFaciesLithostrat faclith in FaciesLithostrat)
                            {
                                try
                                {
                                    tblFaciesLithostrat result1 = db1.GetModelByExpression(a => a.facIdFk == faclith.facIdFk).First();

                                    if (!result1.Equals(faclith))
                                    {
                                        db1.DeleteModelById(result1.faclithIdPk);
                                        db1.InsertModel(new tblFaciesLithostrat() { facIdFk = faclith.facIdFk, litIdFk = faclith.litIdFk });
                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                        using (var db1 = new ApirsRepository<tblFaciesObservation>())
                        {
                            ///Updating observations
                            foreach (tblFaciesObservation facObs in FaciesObservations)
                            {
                                try
                                {
                                    db1.UpdateModel(facObs, facObs.foIdPk);
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }
                    }

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                }

            }

        }

        /// <summary>
        /// Adding a default lithostratigraphic unit
        /// </summary>
        public void AddLithostratigraphicUnit()
        {

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the Facy
            try
            {
                if (SelectedFacy == null || SelectedFacy.facInterpreterId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Facy. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblFaciesLithostrat>())
                {
                    db.InsertModel(new tblFaciesLithostrat() { facIdFk = SelectedFacy.facIdPk, litIdFk = 55 });
                    this.faciesLithostrat = new BindableCollection<tblFaciesLithostrat>(db.GetModel());
                }
            }
            catch
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
                OnSelectedFacyChanged();
            }
        }

        /// <summary>
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemoveLithostratigraphicUnit(int id)
        {

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the Facy
            try
            {
                if (SelectedFacy == null || SelectedFacy.facInterpreterId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Facy. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblFaciesLithostrat>())
                {
                    tblFaciesLithostrat result = db.GetModelByExpression(o => o.faclithIdPk == id).First();
                    db.DeleteModelById(result.faclithIdPk);
                    this.faciesLithostrat = new BindableCollection<tblFaciesLithostrat>(db.GetModel());
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
                Refresh();
            }
        }

        /// <summary>
        /// Adding a default lithostratigraphic unit
        /// </summary>
        public void AddObservation()
        {

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Add, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the Facy
            try
            {
                if (SelectedFacy == null || SelectedFacy.facInterpreterId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Facy. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblFaciesObservation>())
                {
                    db.InsertModel(new tblFaciesObservation() { fofacIdFk = SelectedFacy.facIdPk, foPersonIdFk = SelectedFacy.facInterpreterId });
                }
            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
            }
            finally
            {
                OnSelectedFacyChanged();
            }
        }

        /// <summary>
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemoveObservation(int id)
        {

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the Facy
            try
            {
                if (SelectedFacy == null || SelectedFacy.facInterpreterId != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Facy. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblFaciesObservation>())
                {
                    tblFaciesObservation result = db.GetModelByExpression(o => o.foIdPk == id).First();
                    db.DeleteModelById(result.foIdPk);
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
                Refresh();
            }
        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedFacy, (int)SelectedFacy.facInterpreterId, SelectedFacy.facIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (SelectedFacy != null)
                switch (SelectedFacy.facType)
                {
                    case "Siliciclastic":
                        var type1 = new tblLithofacy();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, SelectedFacy.facIdPk.ToString()));
                        break;
                    case "Biochemical":
                        var type2 = new tblBiochemicalFacy();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type2, SelectedFacy.facIdPk.ToString()));
                        break;
                    case "Volcanic":
                        var type3 = new tblVolcanicFacy();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type3, SelectedFacy.facIdPk.ToString()));
                        break;
                    case "Igneous":
                        var type4 = new tblIgneousFacy();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type4, SelectedFacy.facIdPk.ToString()));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

        //Adding an item
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

            if (SelectedFacy != null)
                switch (parameter)
                {
                    case "Lithostratigraphy":
                        var type4 = new LithostratigraphyUnion();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type4, "Lithostratigraphy"));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

        //Exporting a control
        public void ExportControl(object parameter, bool report = false)
        {
            if (report)
            {
                _events.PublishOnUIThreadAsync(new ExportControlMessage((UIElement)parameter, "pdf"));
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
                case "Project changed":
                    if (SelectedFacy.facIdPk != 0)
                        LoadData(SelectedFacy.facType);
                    else
                        LoadData("Siliciclastic");
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

        #endregion
    }
}
