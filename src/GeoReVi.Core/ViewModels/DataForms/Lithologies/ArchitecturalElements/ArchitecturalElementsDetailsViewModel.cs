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
    /// <summary>
    /// Viewmodel for architectural element management
    /// </summary>
    public class ArchitecturalElementsDetailsViewModel : Screen, IHandle<ShortCutMessage>
    {
        #region Private members

        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;

        //Facies codes collection
        private BindableCollection<tblFaciesCode> faciesCodes;

        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> objectOfInvestigation;

        //ArchitecturalElement lithostrat collection
        private BindableCollection<tblArchitecturalElementLithostrat> architecturalElementLithostrat;

        private BindableCollection<tblLithofaciesArchitecturalElement> lithofaciesArchitecturalElement;

        //ArchitecturalElement occurences collection
        private BindableCollection<tblArchEleOccurence> archEleOccurence;

        //architecturalElements of investigation collection
        private BindableCollection<tblArchitecturalElement> architecturalElements;

        //Facies types
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
        /// Selected architecturalElements
        /// </summary>
        private tblArchitecturalElement selectedArchitecturalElement;


        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;


        private string type;

        #endregion

        #region Public properties

        public string Type { get => this.type; set { this.type = value; if (value != null) LoadData(value); NotifyOfPropertyChange(() => Type); } }

        /// <summary>
        /// The selected ArchitecturalElement
        /// </summary>
        public tblArchitecturalElement SelectedArchitecturalElement
        {
            get
            {
                return this.selectedArchitecturalElement;
            }
            set
            {
                try
                {
                    if (SelectedArchitecturalElement.aeIdPk != 0)
                        Update();
                }
                catch { }

                this.selectedArchitecturalElement = value;

                if (value != null)
                    OnSelectedArchitecturalElementChanged();

                NotifyOfPropertyChange(() => SelectedArchitecturalElement);
                NotifyOfPropertyChange(() => ArchitecturalElementLithostrat);
                NotifyOfPropertyChange(() => ArchEleOccurence);
                NotifyOfPropertyChange(() => SelectedArchitecturalElementIndex);
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
        /// facies observations based on the selected facies type
        /// </summary>
        public BindableCollection<tblArchEleOccurence> ArchEleOccurence
        {
            get
            {
                try
                {
                    return new BindableCollection<tblArchEleOccurence>((from obs in this.archEleOccurence
                                                                        where obs.aeIdFk == SelectedArchitecturalElement.aeIdPk
                                                                        select obs).ToList());
                }
                catch
                {
                    return new BindableCollection<tblArchEleOccurence>();
                }
            }
            set
            {
                this.archEleOccurence = value;
                NotifyOfPropertyChange(() => ArchEleOccurence);
            }
        }

        //ArchitecturalElement lithostrat collection
        public BindableCollection<tblArchitecturalElementLithostrat> ArchitecturalElementLithostrat
        {
            get
            {
                try
                {
                    return this.architecturalElementLithostrat;
                }
                catch
                {
                    return new BindableCollection<tblArchitecturalElementLithostrat>();
                }
            }
            set
            {
                this.architecturalElementLithostrat = value;
                NotifyOfPropertyChange(() => ArchitecturalElementLithostrat);
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

        //All facies codes
        public BindableCollection<tblFaciesCode> FaciesCodes
        {
            get { return this.faciesCodes; }
            set { this.faciesCodes = value; NotifyOfPropertyChange(() => FaciesCodes); }
        }

        /// <summary>
        /// Collection of all lithostratigraphic units
        /// </summary>
        public BindableCollection<tblArchitecturalElement> ArchitecturalElements
        {
            get { return this.architecturalElements; }
            set
            {
                this.architecturalElements = value;

                NotifyOfPropertyChange(() => ArchitecturalElements);
                NotifyOfPropertyChange(() => CountArchitecturalElements);
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
            }
        }

        public BindableCollection<tblLithofaciesArchitecturalElement> LithofaciesArchitecturalElement
        {
            get
            {
                return this.lithofaciesArchitecturalElement;
            }
            set
            {
                this.lithofaciesArchitecturalElement = value;
                NotifyOfPropertyChange(() => LithofaciesArchitecturalElement);
            }

        }
        /// <summary>
        /// Readonly count of the ArchitecturalElements
        /// </summary>
        public string CountArchitecturalElements
        {
            get
            {
                if (ArchitecturalElements != null)
                    return ArchitecturalElements.Count.ToString();
                if (ArchitecturalElements.Count > 0)
                    return ArchitecturalElements.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountArchitecturalElements);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedArchitecturalElementIndex
        {
            get
            {
                if (SelectedArchitecturalElement != null)
                    return (ArchitecturalElements.IndexOf(SelectedArchitecturalElement) + 1).ToString();

                return "0";
            }
            set
            {
                OnSelectedArchitecturalElementIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedArchitecturalElementIndex);
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
                NotifyOfPropertyChange(() => CountImagesArchitecturalElement);
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

                if (value != null)
                    OnSelectedImageArchitecturalElementChanged();
                else
                    SelectedImage = new BitmapImage();

                NotifyOfPropertyChange(() => SelectedPictureStore);
                NotifyOfPropertyChange(() => SelectedImageArchitecturalElementIndex);
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
        /// Readonly count of the ArchitecturalElements
        /// </summary>
        public string CountImagesArchitecturalElement
        {
            get
            {
                if (PictureStore != null)
                    return PictureStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountImagesArchitecturalElement);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedImageArchitecturalElementIndex
        {
            get
            {
                if (SelectedPictureStore != null)
                    return (PictureStore.IndexOf(SelectedPictureStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedImageArchitecturalElementIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedImageArchitecturalElementIndex);
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

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public ArchitecturalElementsDetailsViewModel()
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public ArchitecturalElementsDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            LoadData();
            Type = "Siliciclastic";
        }

        #endregion

        #region Methods

        //Loading the static data
        private void LoadData()
        {
            if (ServerInteractionHelper.IsNetworkAvailable() && ServerInteractionHelper.TryAccessDatabase())
            {
                try
                {
                    try
                    {
                        using (var db1 = new ApirsRepository<tblUnionChronostratigraphy>())
                            Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(db1.GetCompleteLithostratigraphy().ToList());
                    }
                    catch
                    {
                        this.lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();

                    }
                    try
                    {
                        using (var db1 = new ApirsRepository<tblObjectOfInvestigation>())
                        {
                            ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db1.GetModel().ToList());
                        }
                    }
                    catch
                    {
                        ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
                    }
                }
                catch (Exception ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(ex.Message + Environment.NewLine + ex.InnerException.Message);
                }
            }
            else
            {
                Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();
                ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>();
            }

        }

        /// <summary>
        /// Loading data based on an inpunt parameter
        /// </summary>
        /// <param name="parameter">ArchitecturalElement type as string</param>
        public void LoadData(string parameter)
        {

            try
            {
                //Adding the facies codes dependend on the selected facies type
                try
                {
                    using (var db = new ApirsRepository<tblFaciesCode>())
                    {
                        FaciesCodes = new BindableCollection<tblFaciesCode>(db.GetModelByExpression(fc => fc.fcFaciesType == parameter
                        && fc.fcHierarchy == "Architectural element"
                        || fc.fcCode.Contains("Unknown")));
                    }
                }
                catch
                {
                    FaciesCodes = new BindableCollection<tblFaciesCode>();
                }
                try
                {
                    using (var db = new ApirsRepository<tblArchitecturalElement>())
                    {
                        ArchitecturalElements = new BindableCollection<tblArchitecturalElement>(db.GetArchitecturalElementsByProject(Projects, parameter));
                    }
                }
                catch
                {
                    ArchitecturalElements = new BindableCollection<tblArchitecturalElement>();
                }
            }
            catch (Exception e)
            {
                ArchitecturalElements = new BindableCollection<tblArchitecturalElement>();
                SelectedArchitecturalElement = new tblArchitecturalElement() { aeUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
            }

            if (ArchitecturalElements.Count != 0) SelectedArchitecturalElement = ArchitecturalElements.First();
            else SelectedArchitecturalElement = new tblArchitecturalElement() { aeUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
        }

        /// <summary>
        /// Method that fires a task which downloads data from the server and converts it to bitmaps
        /// </summary>
        private void OnSelectedArchitecturalElementChanged()
        {

            try
            {
                using (var db = new ApirsRepository<tblFacy>())
                {
                    FaciesTypes = new BindableCollection<tblFacy>(db.GetFaciesTypeByProject(Projects, SelectedArchitecturalElement.aeType));
                }
                using (var db = new ApirsRepository<tblLithofaciesArchitecturalElement>())
                {
                    try
                    {
                        LithofaciesArchitecturalElement = new BindableCollection<tblLithofaciesArchitecturalElement>(db.GetModelByExpression(x => x.archIdFk == SelectedArchitecturalElement.aeIdPk));
                    }
                    catch
                    {
                        LithofaciesArchitecturalElement = new BindableCollection<tblLithofaciesArchitecturalElement>();
                    }
                }
                try
                {
                    using (var db = new ApirsRepository<tblArchEleOccurence>())
                    {
                        ArchEleOccurence = new BindableCollection<tblArchEleOccurence>(db.GetOccurenceByArchitecturalElement(ArchitecturalElements));
                    }
                }
                catch
                {
                    ArchEleOccurence = new BindableCollection<tblArchEleOccurence>();
                }
                try
                {

                    using (var db = new ApirsRepository<tblArchitecturalElementLithostrat>())
                    {
                        ArchitecturalElementLithostrat = new BindableCollection<tblArchitecturalElementLithostrat>(db.GetModelByExpression(aelit => aelit.aeIdFk == SelectedArchitecturalElement.aeIdPk));
                    }
                }
                catch
                {
                    ArchitecturalElementLithostrat = new BindableCollection<tblArchitecturalElementLithostrat>();
                }
            }
            catch
            {
                FaciesTypes = new BindableCollection<tblFacy>();
                LithofaciesArchitecturalElement = new BindableCollection<tblLithofaciesArchitecturalElement>();
            }

            BackgroundWorker bw = new BackgroundWorker();

            bw.DoWork += ((sender1, args) =>
            {

                new DispatchService().Invoke(
                async () =>
                {
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts = null;
                    }
                    cts = new CancellationTokenSource();

                    IsImageLoading = true;
                    PictureStore = await FileHelper.LoadImagesAsync(SelectedArchitecturalElement.aeIdPk, "ArchitecturalElement");
                    SelectedPictureStore = (PictureStore.Count > 0) ? PictureStore.First() : new v_PictureStore();
                    IsImageLoading = false;
                });

            });

            bw.RunWorkerCompleted += ((sender1, args) =>
            {
                if (args.Error != null)  // if an exception occurred during DoWork,
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(args.Error.ToString());

            });

            bw.RunWorkerAsync(); // start the background worker

        }

        //Event that is fired when the index changed
        private void OnSelectedArchitecturalElementIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != ArchitecturalElements.IndexOf(SelectedArchitecturalElement))
                {
                    SelectedArchitecturalElement = ArchitecturalElements.ElementAt(Convert.ToInt32(parameter) - 1);
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
        private async void OnSelectedImageArchitecturalElementChanged()
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
        private void OnSelectedImageArchitecturalElementIndexChanged(string parameter)
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
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesArchitecturalElement) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per ArchitecturalElement");
                return;
            }

            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"JPEG (*.jpg)|*.jpg|PNG (*.png)|*.png|BMP (*.bmp)|*.bmp";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName != "")
            {
                //Getting file information
                FileInfo fi = new FileInfo(openFileDlg.FileName);

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

                    //Retrieving file meta data
                    string fileName = fi.Name;
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    string extension = fi.Extension;
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

                        //Testing if a connection is established
                        if (ServerInteractionHelper.IsNetworkAvailable())
                        {
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
                                db.tblPictureArchitecturalElements.Add(new tblPictureArchitecturalElement() { aeIdFk = SelectedArchitecturalElement.aeIdPk, picStreamIdFk = result, picName = fileName });
                                db.SaveChanges();
                                Refresh();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                }

            }
        }

        //If an image file gets dropped on the image control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (Convert.ToInt32(CountImagesArchitecturalElement) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per ArchitecturalElement");
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

                    //Testing if a connection is established
                    if (ServerInteractionHelper.IsNetworkAvailable())
                    {
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
                            db.tblPictureArchitecturalElements.Add(new tblPictureArchitecturalElement() { aeIdFk = SelectedArchitecturalElement.aeIdPk, picStreamIdFk = result, picName = fileName });
                            db.SaveChanges();
                            Refresh();
                        }
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
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
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
            if (ArchitecturalElements.Count != 0)
                SelectedArchitecturalElement = ArchitecturalElements.Last();
        }


        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Previous()
        {
            if (ArchitecturalElements.Count != 0)
                SelectedArchitecturalElement = Navigation.GetPrevious(ArchitecturalElements, SelectedArchitecturalElement);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Next()
        {

            if (ArchitecturalElements.Count != 0)
                SelectedArchitecturalElement = Navigation.GetNext(ArchitecturalElements, SelectedArchitecturalElement);
        }

        /// <summary>
        /// Go to the first dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void First()
        {
            if (ArchitecturalElements.Count != 0)
                SelectedArchitecturalElement = ArchitecturalElements.First();
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
            tblArchitecturalElement current = SelectedArchitecturalElement;
            int id = 0;

            try
            {
                if (SelectedArchitecturalElement != null)
                    id = SelectedArchitecturalElement.aeIdPk;

                LoadData(SelectedArchitecturalElement.aeType.ToString());

                if (id == 0) SelectedArchitecturalElement = ArchitecturalElements.First();
                else SelectedArchitecturalElement = ArchitecturalElements.Where(x => x.aeIdPk == id).First();
            }
            catch (Exception e)
            {
                try
                {
                    LoadData("Siliciclastic");
                    if (ArchitecturalElements.Count != 0) SelectedArchitecturalElement = ArchitecturalElements.First();
                    else SelectedArchitecturalElement = new tblArchitecturalElement() { aeUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
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
            //Only accessible if current user uploaded the ArchitecturalElement
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

            SelectedArchitecturalElement = new tblArchitecturalElement() { aeUserIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
        }

        /// <summary>
        /// Deleting the currently viewed rock sample
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Delete()
        {

            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the architectural element. Please contact him via message service.");
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

            using (var db = new ApirsDatabase())
            {
                try
                {
                    tblArchitecturalElement result = db.tblArchitecturalElements.Where(b => b.aeIdPk == SelectedArchitecturalElement.aeIdPk).First();

                    if (result != null)
                    {
                        db.tblArchitecturalElements.Remove(result);
                    }

                    db.SaveChanges();
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Architectural element deleted.");
                    LoadData(result.ToString());

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

        // Commit changes from the new ArchitecturalElement form
        // or edits made to the existing ArchitecturalElement form.  
        public void Update()
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblArchitecturalElement>())
            {
                try
                {
                    if (SelectedArchitecturalElement.aeIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedArchitecturalElement);
                        }
                        catch
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Element can't be added. Please check every field again.");
                            return;
                        }

                        Refresh();

                    }
                    else if (SelectedArchitecturalElement.aeCode != null)
                    {
                        //Only accessible if current user uploaded the ArchitecturalElement
                        try
                        {
                            if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                            {
                                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the Architectural Element. Please contact him via message service.");
                                return;
                            }
                        }
                        catch
                        {
                            return;
                        }

                        using (var db1 = new ApirsRepository<tblArchitecturalElementLithostrat>())
                        {
                            ///Updating lithostratigraphy
                            foreach (tblArchitecturalElementLithostrat faclith in ArchitecturalElementLithostrat)
                            {
                                try
                                {
                                    tblArchitecturalElementLithostrat result1 = db1.GetModelByExpression(a => a.aelithIdPk == faclith.aelithIdPk).First();

                                    if (!result1.Equals(faclith))
                                    {
                                        db1.UpdateModel(faclith, faclith.aelithIdPk);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                        using (var db1 = new ApirsRepository<tblArchEleOccurence>())
                        {
                            ///Updating observations
                            foreach (tblArchEleOccurence facObs in ArchEleOccurence)
                            {
                                try
                                {
                                    tblArchEleOccurence result2 = db1.GetModelById(facObs.aoIdPk);

                                    if (result2 != null)
                                    {
                                        db1.UpdateModel(facObs, facObs.aoIdPk);
                                        db1.Save();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                        using (var db1 = new ApirsRepository<tblLithofaciesArchitecturalElement>())
                        {
                            ///Updating observations
                            foreach (tblLithofaciesArchitecturalElement facel in LithofaciesArchitecturalElement)
                            {
                                try
                                {
                                    tblLithofaciesArchitecturalElement result2 = db1.GetModelById(facel.litarIdPk);

                                    if (result2 != null)
                                    {
                                        db1.UpdateModel(facel, facel.litarIdPk);
                                        db1.Save();
                                    }

                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        }

                        try
                        {
                            //Updating ArchitecturalElement information
                            tblArchitecturalElement result = db.GetModelByExpression(b => b.aeIdPk == SelectedArchitecturalElement.aeIdPk).First();

                            if (result != null)
                            {
                                db.UpdateModel(SelectedArchitecturalElement, SelectedArchitecturalElement.aeIdPk);
                                db.Save();
                            }
                        }
                        catch (Exception ex)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                        }
                    }

                }
                catch (SqlException ex)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please provide valid input parameters");
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("Object reference")) return;
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("An unexpected error occured.");
                }

            }
        }

        /// <summary>
        /// Adding a default lithostratigraphic unit
        /// </summary>
        public void AddLithostratigraphicUnit()
        {

            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
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
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
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
                using (var db = new ApirsRepository<tblArchitecturalElementLithostrat>())
                {
                    db.InsertModel(new tblArchitecturalElementLithostrat() { aeIdFk = SelectedArchitecturalElement.aeIdPk, litIdFk = 55 });
                }
            }
            catch
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
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the ArchitecturalElement. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblArchEleOccurence>())
                {
                    db.InsertModel(new tblArchEleOccurence() { aeIdFk = SelectedArchitecturalElement.aeIdPk, aoOoiIdFk = 48, aoInterpreter = SelectedArchitecturalElement.aeUser });
                    db.Save();
                }
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

        public void AddFaciesType()
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
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
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
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
                using (var db = new ApirsDatabase())
                {
                    db.tblLithofaciesArchitecturalElements.Add(new tblLithofaciesArchitecturalElement() { archIdFk = SelectedArchitecturalElement.aeIdPk, lftIdFk = 154 });
                    db.SaveChanges();
                }
            }
            catch
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
                Refresh();
            }
        }

        /// <summary>
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemLithostratigraphicUnit(int id)
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
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
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
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
                using (var db = new ApirsRepository<tblArchitecturalElementLithostrat>())
                {
                    tblArchitecturalElementLithostrat result = db.GetModelById(id);
                    db.DeleteModelById(result.aelithIdPk);
                    db.Save();
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
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemFacies(int id)
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
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
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the architectural element. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblLithofaciesArchitecturalElement>())
                {
                    tblLithofaciesArchitecturalElement result = db.GetModelById(id);
                    db.DeleteModelById(result.litarIdPk);
                    db.Save();
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
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemObservation(int id)
        {
            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedArchitecturalElement, (int)SelectedArchitecturalElement.aeUserIdFk, SelectedArchitecturalElement.aeIdPk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Only accessible if current user uploaded the ArchitecturalElement
            try
            {
                if (SelectedArchitecturalElement == null || SelectedArchitecturalElement.aeUserIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the ArchitecturalElement. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblArchEleOccurence>())
                {
                    tblArchEleOccurence result = db.GetModelById(id);
                    db.DeleteModelById(result.aoIdPk);
                    db.Save();
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
                case "Right":
                    Next();
                    break;
                case "Left":
                    Previous();
                    break;
                case "Project changed":
                    if (SelectedArchitecturalElement.aeIdPk != 0)
                        LoadData(SelectedArchitecturalElement.aeType.ToString());
                    else
                        LoadData("Siliciclastic");
                    break;
            }
        }

        public void AddItem(string parameter)
        {
            //Only accessible if current user uploaded the ArchitecturalElement
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

            if (SelectedArchitecturalElement != null)
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
