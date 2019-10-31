using Caliburn.Micro;
using System.Windows;
using System.Linq;
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Data;
using Microsoft.Maps.MapControl.WPF;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// View model for a single data entry form for rock samples
    /// </summary>
    public class RockSampleDetailsViewModel : Screen
    {
        #region Private members

        //Rock sample collection
        private BindableCollection<tblRockSample> rockSamples;
        //Rock sample collection
        private BindableCollection<tblRockSample> allRockSamples;

        //Petrography colleciton
        private BindableCollection<tblUnionPetrography> unionPetrography;
        //Facies collection
        private BindableCollection<tblFacy> facies;
        //Architectural element colleciton
        private BindableCollection<tblArchitecturalElement> architecturalElements;
        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;
        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;
        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> objectOfInvestigation;

        /// <summary>
        /// Selected objects
        /// </summary>
        private tblRockSample selectedRockSample;

        //Pictures
        private BindableCollection<v_PictureStore> pictureStore;
        private v_PictureStore selectedPictureStore;
        private BitmapImage selectedImage;

        //checks if image is loading
        private bool isImageLoading = false;

        /// <summary>
        /// Event agregator for event subscription to communicate with other view models
        /// </summary>
        private IEventAggregator _events;

        /// <summary>
        /// Cancellation token source for canceling image downloads
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
                this.type = value;

                if (value != null)
                {
                    LoadData(value);
                }

                NotifyOfPropertyChange(() => Type);
            }
        }

        /// <summary>
        /// Property which checks if image is loading
        /// </summary>
        public bool IsImageLoading
        {
            get { return this.isImageLoading; }
            set { this.isImageLoading = value; NotifyOfPropertyChange(() => IsImageLoading); }
        }

        /// <summary>
        /// The selected rock sample
        /// </summary>
        public tblRockSample SelectedRockSample
        {
            get { return this.selectedRockSample; }
            set
            {
                if (value != null)
                    Update();

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
                    Initialization = OnSelectedRockSampleChanged();

                NotifyOfPropertyChange(() => SelectedRockSample);
                NotifyOfPropertyChange(() => SelectedRockSampleIndex);
            }
        }

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
                if (RockSamples != null && SelectedRockSample != null)
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
                NotifyOfPropertyChange(() => CountImagesRockSample);
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
                NotifyOfPropertyChange(() => SelectedImageRockSampleIndex);
                NotifyOfPropertyChange(() => SelectedPictureStore);
                OnSelectedImageRockSampleChanged(value);
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
        /// Readonly count of the objects
        /// </summary>
        public string CountImagesRockSample
        {
            get
            {
                if (PictureStore != null)
                    return PictureStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountImagesRockSample);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedImageRockSampleIndex
        {
            get
            {
                if (SelectedPictureStore != null)
                    return (PictureStore.IndexOf(SelectedPictureStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedImageRockSampleIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedImageRockSampleIndex);
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
                NotifyOfPropertyChange(() => RockSamples);
                NotifyOfPropertyChange(() => CountRockSamples);
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
                    return (tblProject)((ShellViewModel)IoC.Get<IShell>()).SelectedProject;

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
                    return (BindableCollection<tblProject>)((ShellViewModel)IoC.Get<IShell>()).SelectedProjects;
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

        //Type of the import objects
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
            await LoadData().ContinueWith((a) =>
            {
                Type = "Plug";
            });
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RockSampleDetailsViewModel(IEventAggregator events)
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
                    using (var db1 = new ApirsRepository<tblUnionPetrography>())
                    {
                        UnionPetrography = new BindableCollection<tblUnionPetrography>(db1.GetModel().ToList());
                    }
                    using (var db1 = new ApirsRepository<tblUnionChronostratigraphy>())
                    {
                        Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(db1.GetModel().ToList());
                    }
                    using (var db1 = new ApirsRepository<tblObjectOfInvestigation>())
                    {
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db1.GetModel().ToList());
                    }
                    using (var db1 = new ApirsRepository<tblUnionLithostratigraphy>())
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(db1.GetCompleteLithostratigraphy().ToList());
                    }
                    using (var db1 = new ApirsRepository<tblFacy>())
                    {
                        Facies = new BindableCollection<tblFacy>(db1.GetFaciesByProject(Projects).ToList());
                    }
                    using (var db1 = new ApirsRepository<tblArchitecturalElement>())
                    {
                        ArchitecturalElements = new BindableCollection<tblArchitecturalElement>(db1.GetArchitecturalElementsByProject(Projects).ToList());
                    }
                }
                catch (Exception ex)
                {
                    UnionPetrography = new BindableCollection<tblUnionPetrography>();
                    Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                    Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();
                    ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                    ArchitecturalElements = new BindableCollection<tblArchitecturalElement>();
                    Facies = new BindableCollection<tblFacy>();
                }
            });
        }

        //Loading filtered data
        public async void LoadData(string parameter)
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                using (var db = new ApirsRepository<tblRockSample>())
                {
                    try
                    {
                        RockSamples = new BindableCollection<tblRockSample>(db.GetRockSamplesByProject(Projects, parameter).ToList());

                        this.allRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                    }
                    catch (Exception e)
                    {
                        RockSamples = new BindableCollection<tblRockSample>();
                        this.allRockSamples = new BindableCollection<tblRockSample>(RockSamples);
                    }
                    finally
                    {
                        if (RockSamples.Count != 0)
                            SelectedRockSample = RockSamples.First();
                        else
                            SelectedRockSample = new tblRockSample() { sampUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                    }

                }
            });
        }

        //Activating a background worker to select and download images asynchronously
        private async Task OnSelectedRockSampleChanged()
        {

            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                PictureStore = await FileHelper.LoadImagesAsync(SelectedRockSample.sampIdPk, "RockSample");
                SelectedPictureStore = (PictureStore.Count > 0) ? PictureStore.First() : new v_PictureStore();
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }

        /// <summary>
        /// Downloading images asynchronously
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private async void OnSelectedImageRockSampleChanged(v_PictureStore pic)
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
        private void OnSelectedImageRockSampleIndexChanged(string parameter)
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
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesRockSample) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per object");
                return;
            }

            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName != "")
            {
                using (var db = new ApirsRepository<tblPictureRockSample>())
                {
                    Tuple<Guid, string> result = db.UploadImage(openFileDlg.FileName);

                    if ((Guid)result.Item1 != Guid.Empty)
                    {
                        db.InsertModel(new tblPictureRockSample() { sampIdFk = SelectedRockSample.sampIdPk, picStreamIdFk = (Guid)result.Item1, picName = (string)result.Item2 });
                        OnSelectedRockSampleChanged();
                    }
                }
            }
        }

        //If an image file gets dropped on the image control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            if ((bool)((ShellViewModel)IoC.Get<IShell>()).LocalMode)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Not available in local mode.");
                return;
            }

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesRockSample) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per object");
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

            //Implementing a new filestream
            FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);

            //Transfering filestream into binary format
            BinaryReader rdr = new BinaryReader(fs);
            byte[] fileData = rdr.ReadBytes((int)fs.Length);

            //Scaling factor
            double scale = 0.6;

            //Resizing bitmap to reduce image size
            Bitmap startBitmap;
            //Converting fileStream data into bitmap
            using (var ms = new MemoryStream(fileData))
            {
                startBitmap = new Bitmap(ms);
            }

            //new size
            int newHeight = (int)Math.Round(startBitmap.Height * scale, 0);
            int newWidth = (int)Math.Round(startBitmap.Width * scale, 0);

            // write CreateBitmapFromBytes  
            Bitmap newBitmap = new Bitmap(newWidth, newHeight);

            //Resizing the bitmap
            using (Graphics graphics = Graphics.FromImage(newBitmap))
            {
                graphics.DrawImage(startBitmap, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(0, 0, startBitmap.Width, startBitmap.Height), GraphicsUnit.Pixel);
            }

            //Back conversion
            fileData = BitmapToByte(newBitmap); // write CreateBytesFromBitmap 

            //Closing filestream
            rdr.Close();
            fs.Close();

            try
            {
                ApirsDatabase ap = new ApirsDatabase();
                char charac = 'a';

                //Changing image name based on the count of occurences
                while (ap.v_PictureStore.Where(x => x.name == fileName).Count() > 0)
                {
                    fileName = fileNameWithoutExtension + charac + extension;
                    charac++;
                }

                ap = new ApirsDatabase();
                //Establishing a sql connection
                using (SqlConnection SqlConn = new SqlConnection(ap.Database.Connection.ConnectionString.ToString()))
                {
                    SqlCommand spAddImage = new SqlCommand("dbo.spAddImage", SqlConn);

                    //Preparing the stored procedure
                    spAddImage.CommandType = System.Data.CommandType.StoredProcedure;

                    //Adding the parameters
                    spAddImage.Parameters.Add("@pName", SqlDbType.NVarChar, 255).Value = fileName;
                    spAddImage.Parameters.Add("@pFile_Stream", SqlDbType.Image, fileData.Length).Value = fileData;

                    //Opening the connection
                    SqlConn.Open();

                    Guid result = (Guid)spAddImage.ExecuteScalar();

                    SqlConn.Close();

                    //Instantiate database
                    using (ApirsDatabase db = new ApirsDatabase())
                    {
                        db.tblPictureRockSamples.Add(new tblPictureRockSample() { sampIdFk = SelectedRockSample.sampIdPk, picStreamIdFk = result, picName = fileName });
                        db.SaveChanges();
                        Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
            }
        }

        /// <summary>
        /// Deleting the currently selected image
        /// </summary>
        public void DeleteImage()
        {

            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (CountImagesRockSample == "0")
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

                OnSelectedRockSampleChanged();

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

        /// <summary>
        /// Refreshing the dataset
        /// </summary>
        public override void Refresh()
        {
            tblRockSample current = SelectedRockSample;

            try
            {
                int selected = SelectedRockSample.sampIdPk;
                OnSelectedRockSampleChanged();
                SelectedRockSample = RockSamples.Where(x => x.sampIdPk == selected).First();
            }
            catch
            {
                try
                {
                    LoadData("Plug");
                    SelectedRockSample = new tblRockSample() { sampUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                }
                catch
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
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
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk))
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

            using (var db = new ApirsRepository<tblRockSample>())
            {
                try
                {
                    db.DeleteModelById(SelectedRockSample.sampIdPk);
                    db.Save();

                    LoadData(SelectedRockSample.sampType.ToString());

                }
                catch (Exception ex)
                {
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
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
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblRockSample>())
            {
                try
                {
                    if (SelectedRockSample.sampIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedRockSample);
                            RockSamples.Add(SelectedRockSample);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Rock sample can't be added. Please check every field again.");
                            return;
                        }

                        NotifyOfPropertyChange(() => SelectedRockSample);

                    }
                    else
                    {
                        db.UpdateModel(SelectedRockSample, SelectedRockSample.sampIdPk);
                    }

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Something went wrong");
                }

            }

            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.

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

            SelectedRockSample = new tblRockSample() { sampUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId, sampprjIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).SelectedProject.prjIdPk };
        }

        /// <summary>
        /// Checking the uniqueness of the name in the database
        /// </summary>
        public void CheckUniqueness()
        {
            int count;

            try
            {
                using (var db = new ApirsRepository<tblRockSample>())
                {
                    if (SelectedRockSample.sampIdPk == 0)
                    {
                        count = db.GetModelByExpression(x => x.sampLabel == SelectedRockSample.sampLabel && x.sampIdPk != SelectedRockSample.sampIdPk).Count();
                    }
                    else
                    {
                        count = db.GetModelByExpression(x => x.sampLabel == SelectedRockSample.sampLabel && x.sampIdPk != SelectedRockSample.sampIdPk).Count();
                    }

                    if (count == 0)
                        return;

                    char a = 'A';

                    while (count > 0)
                    {

                        if (SelectedRockSample.sampIdPk == 0)
                        {
                            SelectedRockSample.sampLabel = SelectedRockSample.sampLabel + a.ToString();

                            count = db.GetModelByExpression(x => x.sampLabel == SelectedRockSample.sampLabel && x.sampIdPk != SelectedRockSample.sampIdPk).Count();
                        }
                        else
                        {
                            SelectedRockSample.sampLabel = SelectedRockSample.sampLabel + a.ToString();

                            count = db.GetModelByExpression(x => x.sampLabel == SelectedRockSample.sampLabel && x.sampIdPk != SelectedRockSample.sampIdPk).Count();
                        }
                        a++;
                    }

                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. We provided a valid alternative for it.");
                    NotifyOfPropertyChange(() => SelectedRockSample);
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(e.Message);
            }
        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
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
                    case "Lithostratigraphy":
                        var type4 = new LithostratigraphyUnion();
                        _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type4, "Lithostragigraphy"));
                        break;
                }
            else
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

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

            if (SelectedRockSample != null)
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

        //Method to open a details windows dependend on the current item type
        public void SubSample(string type)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedRockSample, (int)SelectedRockSample.sampUploaderIdFk, SelectedRockSample.sampIdPk))
                {
                    return;
                }
            }
            catch
            {
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("No sample selected.");
        }

        /// <summary>
        /// Apply text filter
        /// </summary>
        /// <returns></returns>
        public async Task Filter()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => ((ShellViewModel)IoC.Get<IShell>(null)).IsLoading, async () =>
            {
                try
                {
                    FilterDataSetViewModel.Filter().AsResult();
                    RockSamples = new BindableCollection<tblRockSample>(FilterDataSetViewModel.FilterDataSet);
                }
                finally
                {
                    if (!RockSamples.Contains(SelectedRockSample))
                        SelectedRockSample = RockSamples.First();
                }
            });
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
                    case "Soil/Sediment":
                    case "No Specific":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblRockSample>(
                                new ImportProcedureViewModel<tblRockSample>(_events, table));
                        break;
                    case "Plug":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblRockSample, tblPlug>(
                                new ImportProcedureViewModel<tblRockSample, tblPlug>(_events, table));
                        break;
                    case "Cuboid":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblRockSample, tblCuboid>(
                                new ImportProcedureViewModel<tblRockSample, tblCuboid>(_events, table));
                        break;
                    case "Handpiece":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblRockSample, tblHandpiece>(
                             new ImportProcedureViewModel<tblRockSample, tblHandpiece>(_events, table));
                        break;
                }

            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        //Exporting the actually selected list of rock samples to a csv file
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
                ExportHelper.ExportList<tblRockSample>(RockSamples, saveFileDialog.FileName, SelectedRockSample.sampType);
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
