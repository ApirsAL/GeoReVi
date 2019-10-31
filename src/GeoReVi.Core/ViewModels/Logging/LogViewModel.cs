using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;

namespace GeoReVi
{
    /// <summary>
    /// Displays the application log
    /// </summary>
    public class LogViewModel : Screen
    {
        #region Public properties

        /// <summary>
        /// The file path
        /// </summary>
        private string filePath = "";
        public string FilePath
        {
            get => this.filePath;
            set
            {
                this.filePath = value;
            }
        }

        /// <summary>
        /// The file Name
        /// </summary>
        private string fileName = "";
        public string FileName
        {
            get => this.fileName;
            set
            {
                this.fileName = value;
            }
        }


        //The text standing in the file
        private string fileText;
        public string FileText
        {
            get { return fileText; }
            set
            {
                if (fileText == value) return;
                fileText = value;
                NotifyOfPropertyChange(()=>FileText);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LogViewModel(string fileName = "Log.txt", string filePath = "")
        {
            FilePath = filePath;
            FileName = fileName;
            RunWatch(FileName);
        }
        
        #endregion

        #region Methods

        public void RunWatch(string filePath = "")
        {
            var watcher = new FileSystemWatcher();

            FilePath = System.AppDomain.CurrentDomain.BaseDirectory;

            // Watch for changes in LastAccess and LastWrite times, and the renaming of files or directories. 
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            // FILE TO WATCH PATH AND NAME. 
            watcher.Path = FilePath;
            watcher.Filter = FileName;
            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        // Define the event handlers. 
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                //Read file update the Graphical User Interface 
                FileText = File.ReadAllText(FilePath + "\\" + FileName);
            }
            catch (FileNotFoundException)
            {
                FileText = "File not found.";
            }
            catch (FileLoadException)
            {
                FileText = "File Failed to load";
            }
            catch (IOException)
            {
                FileText = "File I/O Error";
            }
            catch (Exception err)
            {
                FileText = err.Message;
            }
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // There will be code here to re-create file if it is renamed
        }

        #endregion
    }
}
