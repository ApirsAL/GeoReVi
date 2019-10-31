using Caliburn.Micro;
using System;

namespace GeoReVi
{
    /// <summary>
    /// Logs to a file
    /// </summary>
    public class FileLogger : ILogger
    {
        #region Public properties

        // File path to log to
        public string FilePath { get; set; }

        // Checks if the time should be logged
        public bool LogTime { get; set; } = true;

        #endregion

        #region Constructor
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="filePath"></param>
        public FileLogger(string filePath)
        {
            FilePath = filePath;
        }


        #endregion

        #region Public methods

        /// <summary>
        /// Logs a message to a file
        /// </summary>
        /// <param name="message"></param>
        /// <param name="level"></param>
        public void Log(string message, LogLevel level)
        {
            var currentTime = DateTimeOffset.Now.ToString("yyyy-MM-dd hh:mm:ss");
            
            //Write the message to the log file. If the application is not booting, the message will be written in an empty file
            ((FileManager)IoC.Get<IFileManager>()).WriteTextToFileAsync(LogTime ? $"[{currentTime}] [{level}] {message}" + Environment.NewLine + Environment.NewLine : $"{ message }" + Environment.NewLine + Environment.NewLine, FilePath, true);
        }

        #endregion

    }
}
