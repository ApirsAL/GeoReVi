using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// The standard log factory for GeoReVi
    /// Logs detauils to the console by default
    /// </summary>
    public class BaseLogFactory : ILogFactory
    {
        #region Protected methods

        /// <summary>
        /// The list of loggers in this factory
        /// </summary>
        protected List<ILogger> mLoggers = new List<ILogger>();

        /// <summary>
        /// A lock for the logger list to keep it thread safe
        /// </summary>
        protected object mLoggersLock = new object(); 


        #endregion

        #region Properties

        /// <summary>
        /// The level of logging to output
        /// </summary>
        public LogOutputLevel LogOutputLevel { get; set; }

        /// <summary>
        /// If true includes the origin of where the log message was logged from
        /// such as the class name, line number and file name
        /// </summary>
        public bool IncludeLogOrigindetails { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Fires whenever a new log arrives
        /// The syntax is C#7 specific and replaces a tuple
        /// </summary>
        public event Action<(string Message, LogLevel Level)> NewLog = (details) => { };

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseLogFactory(ILogger[] loggers = null)
        {
            // Adds each logger
            if(loggers != null)
             foreach (ILogger log in loggers)
                 AddLogger(log);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specific logger to this factory
        /// </summary>
        /// <param name="logger">The logger</param>
        public void AddLogger(ILogger logger)
        {
            // Log the list so it is thread-safe
            lock(mLoggersLock)
            {
                //If the logger is not already in the list
                if (!mLoggers.Contains(logger));
                    //Add the logger to the list
                    mLoggers.Add(logger);
            }
        }

        /// <summary>
        /// Removes the specified logger from the factory
        /// </summary>
        /// <param name="logger"></param>
        public void RemoveLogger(ILogger logger)
        {
            // Log the list so it is thread-safe
            lock (mLoggersLock)
            {
                //If the logger is not already in the list
                if (mLoggers.Contains(logger)) ;
                    //Add the logger to the list
                    mLoggers.Remove(logger);
            }
        }

        /// <summary>
        /// Logs the specific message to all loggers in this factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">Message level</param>
        /// <param name="origin">The method this message was logged</param>
        /// <param name="filePath">The line of code in the filename this messasge was logged from</param>
        public async void Log(string message,
            LogLevel level = LogLevel.Informative, 
            [CallerMemberName] string origin = "", 
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            if ((int)level < (int)LogOutputLevel)
                return;

            await Task.Run(() =>
            {
                //If the user wants to know where the log originated from...
                if (IncludeLogOrigindetails)
                    //The message logged to the log path
                    message = $"[{Path.GetFileName(filePath)} > {origin}() > Line {lineNumber}] {System.Environment.NewLine}{message} ";

                //Log message to the loggers
                mLoggers.ForEach(logger => logger.Log(message, level));

                // Inform listeners
                NewLog.Invoke((message, level));
            });
        }

        #endregion

    }
}
