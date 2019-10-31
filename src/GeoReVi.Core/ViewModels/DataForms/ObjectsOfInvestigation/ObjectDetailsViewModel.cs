using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Data;
using System.Drawing;
using Microsoft.Maps.MapControl.WPF;

namespace GeoReVi
{
    /// <summary>
    /// View model for the geological objects input forms
    /// </summary>
    public class ObjectDetailsViewModel : Screen, IHandle<ShortCutMessage>
    {
        #region Private members

        //Chronostrat collection
        private BindableCollection<tblUnionChronostratigraphy> chronostratigraphy;

        //Lithostrat collection
        private BindableCollection<LithostratigraphyUnion> lithostratigraphy;

        //Lithostrat collection
        private BindableCollection<tblBasin> basins;

        //Object lithostrat collection
        private BindableCollection<tblObjectLithostratigraphy> ooiLithostrat;


        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> objectOfInvestigation;

        //Objects of investigation collection
        private BindableCollection<tblObjectOfInvestigation> allObjectOfInvestigation;

        //Location of the object of investigation
        private Location ooiLocation;

        //Pictures
        private BindableCollection<v_PictureStore> pictureStore;
        private v_PictureStore selectedPictureStore;
        private BitmapImage selectedImage;

        //Textfilter variable
        private string textFilter;

        //Cancellation token for async methods
        CancellationTokenSource cts;

        /// <summary>
        /// Visibility members
        /// </summary>
        private bool isImageLoading = false;

        /// <summary>
        /// Selected objects
        /// </summary>
        private tblObjectOfInvestigation selectedObject;

        //Drill cores collection
        private BindableCollection<tblDrillCore> drillCores;

        /// <summary>
        /// Event aggregator for event subscription to communicate with other viewmodels
        /// </summary>
        private IEventAggregator _events;

        private string type;

        #endregion

        #region Public properties

        /// <summary>
        /// Bool if the object is a drilling or not
        /// </summary>
        public bool IsDrilling { get { try { return SelectedObject.ooiType == "Drilling"; } catch { return false; }; } set { NotifyOfPropertyChange(() => IsDrilling); } }

        //Object Type
        public string Type { get => this.type; set { this.type = value; if (value != null) LoadData(value); NotifyOfPropertyChange(() => Type); } }

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
                try
                {
                    if (SelectedObject.ooiIdPk != 0)
                        Update();
                }
                catch
                { }

                this.selectedObject = value;
                try
                {
                    Center = new Location((double)SelectedObject.ooiLatitude, (double)SelectedObject.ooiLongitude);
                }
                catch
                {
                    Center = new Location(0, 0);
                }

                if (value != null && SelectedObject.ooiIdPk != 0)
                    Initialization = OnSelectedObjectChanged();

                NotifyOfPropertyChange(() => SelectedObject);
                NotifyOfPropertyChange(() => IsDrilling);
                NotifyOfPropertyChange(() => SelectedObjectIndex);
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
        /// checks if image is loading
        /// </summary>
        public bool IsImageLoading
        {
            get { return this.isImageLoading; }
            set { this.isImageLoading = value; NotifyOfPropertyChange(() => IsImageLoading); }
        }


        public BindableCollection<tblDrillCore> DrillCores
        {
            get { return this.drillCores; }
            set { this.drillCores = value; NotifyOfPropertyChange(() => DrillCores); }
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
        /// Collection of all chronostratigraphic units
        /// </summary>
        public BindableCollection<tblBasin> Basins
        {
            get { return this.basins; }
            set { this.basins = value; NotifyOfPropertyChange(() => Basins); }
        }

        /// <summary>
        /// lithostratigraphic units based on the selected 
        /// </summary>
        public BindableCollection<tblObjectLithostratigraphy> OoiLithostrat
        {
            get
            {
                return this.ooiLithostrat;
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
        /// Collection of all objects
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
                NotifyOfPropertyChange(() => CountImagesObject);
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
                OnSelectedImageObjectChanged();
                NotifyOfPropertyChange(() => SelectedPictureStore);
                NotifyOfPropertyChange(() => SelectedImageObjectIndex);
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
        public string CountImagesObject
        {
            get
            {
                if (PictureStore != null)
                    return PictureStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountImagesObject);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedImageObjectIndex
        {
            get
            {
                if (SelectedPictureStore != null)
                    return (PictureStore.IndexOf(SelectedPictureStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedImageObjectIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedImageObjectIndex);
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
            await LoadData().ConfigureAwait(true);

            Type = "Outcrop";

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="events"></param>
        public ObjectDetailsViewModel(IEventAggregator events)
        {
            this._events = events;
            _events.Subscribe(this);
            Initialization = InitializeAsync();
        }

        #endregion

        #region Methods

        //Loading the relevant data
        private async Task LoadData()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
            {
                try
                {
                    using (var db1 = new ApirsRepository<tblUnionChronostratigraphy>())
                    {
                        Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>(db1.GetModel().ToList());
                    }
                }
                catch
                {
                    Chronostratigraphy = new BindableCollection<tblUnionChronostratigraphy>();
                }
                try
                {
                    using (var db1 = new ApirsRepository<tblBasin>())
                    {
                        Basins = new BindableCollection<tblBasin>(db1.GetModel().ToList());
                    }
                }
                catch
                {
                    Basins = new BindableCollection<tblBasin>();
                }

                try
                {
                    using (var db1 = new ApirsRepository<tblUnionLithostratigraphy>())
                    {
                        Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>(db1.GetCompleteLithostratigraphy().ToList());
                    }
                }
                catch
                {
                    Lithostratigraphy = new BindableCollection<LithostratigraphyUnion>();

                }
            });
        }

        /// <summary>
        /// Loading data based on an input parameter
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
                       ObjectsOfInvestigation = new BindableCollection<tblObjectOfInvestigation>(db.GetModelByExpression(x => x.ooiType == parameter).ToList());

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
                       SelectedObject = new tblObjectOfInvestigation() { ooiDateUpload = DateTime.Now, ooiUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                   }
               }
           });

        }

        /// <summary>
        /// Method that fires a task which downloads data from the server and converts it to bitmaps
        /// </summary>
        private async Task OnSelectedObjectChanged()
        {
            CommandHelper ch = new CommandHelper();

            await ch.RunBackgroundWorkerHelperAsync(async () =>
           {
               try
               {
                   using (var db = new ApirsRepository<tblDrillCore>())
                   {
                       DrillCores = new BindableCollection<tblDrillCore>(db.GetModelByExpression(dc => dc.dcdrillNameFk == SelectedObject.ooiName).ToList());
                   }
               }
               catch
               {
                   DrillCores = new BindableCollection<tblDrillCore>();
               }
               try
               {
                   using (var db = new ApirsRepository<tblObjectLithostratigraphy>())
                   {
                       OoiLithostrat = new BindableCollection<tblObjectLithostratigraphy>(db.GetModelByExpression(ooilith => ooilith.ooiIdFk == SelectedObject.ooiIdPk).ToList());
                   }
               }
               catch
               {
                   OoiLithostrat = new BindableCollection<tblObjectLithostratigraphy>();
               }
           });

            ch = new CommandHelper();

            await ch.RunBackgroundWorkerWithFlagHelperAsync(() => IsImageLoading, async () =>
             {
                 PictureStore = await FileHelper.LoadImagesAsync(SelectedObject.ooiIdPk, "Object");
                 SelectedPictureStore = PictureStore.Count > 0 ? PictureStore.First() : new v_PictureStore();
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
        /// Downloading the selected image async
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private async void OnSelectedImageObjectChanged()
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
        private void OnSelectedImageObjectIndexChanged(string parameter)
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
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (!IsDrilling && Convert.ToInt32(CountImagesObject) > 2)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of images is limited to three per object");
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
                    using (var db = new ApirsRepository<tblPictureObjectOfInvestigation>())
                    {
                        Tuple<Guid, string> result = db.UploadImage(openFileDlg.FileName);

                        if ((Guid)result.Item1 != Guid.Empty)
                        {
                            db.InsertModel(new tblPictureObjectOfInvestigation() { ooiIdFk = SelectedObject.ooiIdPk, picStreamIdFk = (Guid)result.Item1, picName = (string)result.Item2 });
                            OnSelectedObjectChanged();
                        }
                    }
                }
            }
            catch
            {

            }
        }

        //If an image file gets dropped on the image control, the file gets added to the database
        public void FileDropped(DragEventArgs e)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            //Limit the count of images to three
            if (!IsDrilling && Convert.ToInt32(CountImagesObject) > 2)
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
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please comply with the file formats .png, .bmp or .jpg");
                return;
            }

            using (var db = new ApirsRepository<tblPictureObjectOfInvestigation>())
            {
                Tuple<Guid, string> result = db.UploadImage(fileName);

                if ((Guid)result.Item1 != Guid.Empty)
                {
                    db.InsertModel(new tblPictureObjectOfInvestigation() { ooiIdFk = SelectedObject.ooiIdPk, picStreamIdFk = (Guid)result.Item1, picName = (string)result.Item2 });
                    Refresh();
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
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblObjectOfInvestigation>(
                                new ImportProcedureViewModel<tblObjectOfInvestigation>(_events, table));
                        break;
                    case "Outcrop":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblObjectOfInvestigation, tblOutcrop>(
                                     new ImportProcedureViewModel<tblObjectOfInvestigation, tblOutcrop>(_events, table));
                        break;
                    case "Drilling":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblObjectOfInvestigation, tblDrilling>(
                                     new ImportProcedureViewModel<tblObjectOfInvestigation, tblDrilling>(_events, table));
                        break;
                    case "Transect":
                        ((ShellViewModel)IoC.Get<IShell>()).ShowImportWindow<tblObjectOfInvestigation, tblTransect>(
                                     new ImportProcedureViewModel<tblObjectOfInvestigation, tblTransect>(_events, table));
                        break;
                }

            }
            catch (Exception ex)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
        }

        /// <summary>
        /// Deleting the currently selected image
        /// </summary>
        public void DeleteImage()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            if (CountImagesObject == "0")
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

                OnSelectedObjectChanged();
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

            tblObjectOfInvestigation current = SelectedObject;
            int id = 0;

            try
            {
                if (SelectedObject != null)
                    id = SelectedObject.ooiIdPk;

                LoadData(SelectedObject.ooiType.ToString(), id);
            }
            catch
            {
                try
                {
                    LoadData("Outcrop");
                    SelectedObject = new tblObjectOfInvestigation() { ooiDateUpload = DateTime.Now, ooiUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
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
            }
            catch
            {
                return;
            }

            SelectedObject = new tblObjectOfInvestigation() { ooiDateUpload = DateTime.Now, ooiUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
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
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
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

            using (var db = new ApirsRepository<tblObjectOfInvestigation>())
            {
                try
                {
                    db.DeleteModelById(SelectedObject.ooiIdPk);
                    db.Save();

                    try
                    {
                        SelectedObject = ObjectsOfInvestigation.Where(x => x.ooiIdPk != SelectedObject.ooiIdPk).First();
                    }
                    catch
                    {
                        SelectedObject = new tblObjectOfInvestigation() { ooiDateUpload = DateTime.Now, ooiUploaderIdFk = (int)((ShellViewModel)IoC.Get<IShell>()).UserId };
                    }

                    Refresh();

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

        // Commit changes from the new object form
        // or edits made to the existing object form.  
        public void Update()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedObject, (int)selectedObject.ooiUploaderIdFk))
                {
                    return;
                }
            }
            catch
            {
                return;
            }

            using (var db = new ApirsRepository<tblObjectOfInvestigation>())
            {
                try
                {
                    if (SelectedObject.ooiIdPk == 0)
                    {
                        try
                        {
                            db.InsertModel(SelectedObject);
                        }
                        catch (Exception e)
                        {
                            ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Object can't be added. Please check every field again.");
                            return;
                        }

                        Refresh();

                    }
                    else
                    {
                        try
                        {
                            //Updating object information
                            db.UpdateModel(SelectedObject, selectedObject.ooiIdPk);
                            db.Save();
                        }
                        catch (Exception e)
                        {
                            _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                        }

                        using (var db1 = new ApirsRepository<tblObjectLithostratigraphy>())
                            ///Updating lithostratigraphy
                            foreach (tblObjectLithostratigraphy ooilith in OoiLithostrat)
                            {
                                try
                                {
                                    db1.UpdateModel(ooilith, ooilith.ooilitIdPk);
                                }
                                catch (Exception ex)
                                {
                                    continue;
                                }
                            }
                        //Updating the selected drill cores
                        if (DrillCores.Count != 0)

                            using (var db1 = new ApirsRepository<tblDrillCore>())
                                foreach (tblDrillCore dc in DrillCores)
                                {
                                    try
                                    {
                                        tblDrillCore result2 = db1.GetModelByExpression(a => a.dcIdPk == dc.dcIdPk).First();

                                        if (result2 != null)
                                            db1.UpdateModel(dc, dc.dcIdPk);

                                        db1.Save();
                                    }
                                    catch (Exception exc)
                                    {
                                        continue;
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
                    _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
                }

            }

        }

        //Adding a default lithostratigraphic unit
        public void AddLithostratigraphicUnit()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Update, SelectedObject, (int)SelectedObject.ooiUploaderIdFk, SelectedObject.ooiIdPk))
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
                using (var db = new ApirsRepository<tblObjectLithostratigraphy>())
                {
                    db.InsertModel(new tblObjectLithostratigraphy() { ooiIdFk = SelectedObject.ooiIdPk, lithID = 55 });
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
        public void RemoveLithostratigraphicUnit(int id)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
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
                using (var db = new ApirsRepository<tblObjectLithostratigraphy>())
                {
                    tblObjectLithostratigraphy result = db.GetModelByExpression(o => o.ooilitIdPk == id).First();
                    db.DeleteModelById(result.ooilitIdPk);
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

        //Adding a default lithostratigraphic unit
        public void AddDrillCore()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.AddToObject, SelectedObject, (int)SelectedObject.ooiUploaderIdFk, selectedObject.ooiIdPk))
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
                using (var db = new ApirsRepository<tblDrillCore>())
                {
                    db.InsertModel(new tblDrillCore() { dcdrillNameFk = SelectedObject.ooiName });
                }
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));
            }
            finally
            {
                OnSelectedObjectChanged();
            }
        }

        /// <summary>
        /// Removes a certain lithounit of ID == id
        /// </summary>
        /// <param name="id"></param>
        public void RemoveDrillCore(int id)
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, SelectedObject, (int)SelectedObject.ooiUploaderIdFk))
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
                if (SelectedObject == null || SelectedObject.ooiUploaderIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object. Please contact him via message service.");
                    return;
                }
            }
            catch
            {
                return;
            }

            try
            {
                using (var db = new ApirsRepository<tblDrillCore>())
                {
                    tblDrillCore result = db.GetModelByExpression(o => o.dcIdPk == id).First();
                    db.DeleteModelById(result.dcIdPk);
                    db.Save();
                }
            }
            catch (Exception e)
            {
                _events.PublishOnUIThreadAsync(new MessageBoxMessage(UserMessageValueConverter.ConvertBack(1), "", MessageBoxViewType.Error, MessageBoxViewButton.Ok)); ;
            }
            finally
            {
                OnSelectedObjectChanged();
            }
        }
        /// <summary>
        /// Checking the uniqueness of the name in the database
        /// </summary>
        public void CheckUniqueness()
        {
            int count;

            try
            {
                using (var db = new ApirsRepository<tblObjectOfInvestigation>())
                {
                    if (SelectedObject.ooiIdPk == 0)
                    {
                        count = db.GetModelByExpression(ooi => ooi.ooiName == SelectedObject.ooiName).Count();
                    }
                    else
                    {
                        count = db.GetModelByExpression(ooi => ooi.ooiName == SelectedObject.ooiName
                                 && ooi.ooiIdPk != SelectedObject.ooiIdPk).Count();
                    }
                    if (count == 0)
                        return;

                    char a = 'A';

                    while (count > 0)
                    {
                        if (SelectedObject.ooiIdPk == 0)
                        {
                            SelectedObject.ooiName = SelectedObject.ooiName + a.ToString();

                            count = db.GetModelByExpression(ooi => ooi.ooiName == SelectedObject.ooiName).Count(); ;
                        }
                        else
                        {
                            SelectedObject.ooiName = SelectedObject.ooiName + a.ToString();

                            count = db.GetModelByExpression(ooi => ooi.ooiName == SelectedObject.ooiName
                                 && ooi.ooiIdPk != SelectedObject.ooiIdPk).Count();
                        }

                        a++;
                    }

                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The name was already in use. We provided a valid alternative for it.");
                    SelectedObject = SelectedObject;
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
                    ((ShellViewModel)IoC.Get<IShell>()).ShowError(UserMessageValueConverter.ConvertBack(1));

            });

            bw.RunWorkerAsync(); // start the background worker

        }

        //Method to open a details windows dependend on the current item type
        public void AddDetails()
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

            if (parameter == "Basin" || parameter == "Basins")
            {
                var type1 = new tblBasin();
                _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type1, SelectedObject.ooiName));
            }
            if (parameter == "Lithostratigraphy")
            {
                var type2 = new LithostratigraphyUnion();
                _events.PublishOnUIThreadAsync(new OpenDataWindowMessage(type2, "Lithostratigraphy"));
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
                ExportHelper.ExportList<tblObjectOfInvestigation>(ObjectsOfInvestigation, saveFileDialog.FileName, SelectedObject.ooiType);
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

        #endregion
    }
}
