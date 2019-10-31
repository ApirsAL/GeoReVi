using Caliburn.Micro;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;

namespace GeoReVi
{
    public class FilesListViewModel<T> : Screen where T : class
    {
        #region Public Properties

        /// <summary>
        /// The refered object
        /// </summary>
        private T referedObject;
        public T ReferedObject
        {
            get => this.referedObject;
            set
            {
                this.referedObject = value;
                NotifyOfPropertyChange(() => ReferedObject);
            }
        }

        /// <summary>
        /// Readonly count of the objects
        /// </summary>
        private string countFiles;
        public string CountFiles
        {
            get
            {
                if (FileStore != null)
                    return FileStore.Count.ToString();

                return "0";
            }
            set
            {
                NotifyOfPropertyChange(() => CountFiles);
            }
        }

        //All pictures related to a certain rock sample
        private BindableCollection<v_FileStore> fileStore = new BindableCollection<v_FileStore>();
        public BindableCollection<v_FileStore> FileStore
        {
            get
            {
                return this.fileStore;
            }
            set
            {
                this.fileStore = value;
                NotifyOfPropertyChange(() => FileStore);
                NotifyOfPropertyChange(() => CountFiles);
            }
        }

        //The selected File file
        private v_FileStore selectedFileStore = new v_FileStore();
        public v_FileStore SelectedFileStore
        {
            get
            {
                return this.selectedFileStore;
            }
            set
            {
                this.selectedFileStore = value;
                NotifyOfPropertyChange(() => SelectedFileIndex);
                NotifyOfPropertyChange(() => SelectedFileStore);
            }
        }

        /// <summary>
        /// Readonly index of the selected item
        /// </summary>
        public string SelectedFileIndex
        {
            get
            {
                if (SelectedFileStore != null)
                    return (FileStore.IndexOf(SelectedFileStore) + 1).ToString();

                return "0";
            }

            set
            {
                OnSelectedFileIndexChanged(value);
                NotifyOfPropertyChange(() => SelectedFileIndex);
            }
        }

        /// <summary>
        /// checks if image is loading
        /// </summary>
        private bool isFileLoading = false;
        public bool IsFileLoading
        {
            get { return this.isFileLoading; }
            set { this.isFileLoading = value; NotifyOfPropertyChange(() => IsFileLoading); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="_object"></param>
        [ImportingConstructor]
        public FilesListViewModel(T _object)
        {
            ReferedObject = _object;
        }
        #endregion

        #region Methods

        /// <summary>
        /// If index is changed go to picture
        /// </summary>
        /// <param name="parameter"></param>
        private void OnSelectedFileIndexChanged(string parameter)
        {
            try
            {
                if (Convert.ToInt32(parameter) != FileStore.IndexOf(SelectedFileStore))
                {
                    SelectedFileStore = FileStore.ElementAt(Convert.ToInt32(parameter) - 1);
                }
            }
            catch
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Please selecte a valid value");
            }
        }

        /// <summary>
        /// Uploading file to the database
        /// </summary>
        public void UploadFile()
        {
            try
            {
                if (ReferedObject.GetType() == typeof(tblSection))
                    if (!DataValidation.CheckPrerequisites(CRUD.Delete, ((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))), (int)((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))).secInterpreterIdFk))
                    {
                        return;
                    }
                    else if (ReferedObject.GetType() == typeof(tblMeasurement))
                        return;
            }
            catch
            {
                return;
            }

            //Limit the count of Files to three
            if (Convert.ToInt32(CountFiles) > 1)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("The number of files is limited to three per measurement");
                return;
            }

            //File dialog for opening a jpeg, png or bmp file
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = @"Pdf (*.pdf)|*.pdf|Image (*.PNG;*.JPG)|*.PNG;*.JPG|Excel (*.xlsx;*.xls)|*.xlsx;*.xls|Word (*.docx)|*.docx|CSV (*.csv)|*.csv|Text (*.txt)|*.txt";
            openFileDlg.RestoreDirectory = true;
            openFileDlg.ShowDialog();

            if (openFileDlg.FileName == "")
            {
                return;
            }

            //Getting file information
            FileInfo fI = new FileInfo(openFileDlg.FileName);

            //Uploading file
            Guid id = FileHelper.UploadFile(openFileDlg.FileName);

            if (id == Guid.Empty)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                return;
            }

            if (ReferedObject.GetType() == typeof(tblSection))
            {
                try
                {
                    new ApirsRepository<tblFileSection>().InsertModel(new tblFileSection() { filName = fI.Name, secIdFk = ((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))).secIdPk, filStreamIdFk = id });
                }
                catch (Exception e)
                {
                    ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
                }
            }

        }

        /// <summary>
        /// Creates a png file of a selected bitmap File
        /// </summary>
        public void DownloadFile()
        {
            try
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.InitialDirectory = "C:\\Users";
                dialog.IsFolderPicker = true;
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    FileHelper.LoadFile(SelectedFileStore.file_stream, dialog.FileName + "\\" + SelectedFileStore.name);
                }


            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Deleting the currently selected File
        /// </summary>
        public void DeleteFile()
        {
            try
            {
                if (!DataValidation.CheckPrerequisites(CRUD.Delete, ((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))), (int)((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))).secInterpreterIdFk))
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
                if (ReferedObject.GetType() == typeof(tblSection))
                    if (((tblSection)Convert.ChangeType(ReferedObject, typeof(tblSection))).secInterpreterIdFk != (int)((ShellViewModel)IoC.Get<IShell>()).UserId)
                    {
                        ((ShellViewModel)IoC.Get<IShell>()).ShowInformation("Only the uploader can make changes to the object. Please contact him via message service.");
                        return;
                    }
            }
            catch
            {
                return;
            }

            if (CountFiles == "0")
                return;

            string name = SelectedFileStore.name;

            try
            {
                //Instantiate database
                new ApirsRepository<v_FileStore>().DeleteFile(name);
            }
            catch (Exception e)
            {
                ((ShellViewModel)IoC.Get<IShell>()).ShowInformation(UserMessageValueConverter.ConvertBack(1));
            }
        }

        /// <summary>
        /// Go to the previous dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviousFile()
        {
            if (FileStore.Count != 0)
                SelectedFileStore = Navigation.GetPrevious(FileStore, SelectedFileStore);
        }

        /// <summary>
        /// Go to the next dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NextFile()
        {
            if (FileStore.Count != 0)
                SelectedFileStore = Navigation.GetNext(FileStore, SelectedFileStore);
        }

        #endregion
    }
}
