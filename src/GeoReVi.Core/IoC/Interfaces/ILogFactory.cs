using System;

/// <summary>
/// Holds a bunch of loggers to log messages for the user
/// </summary>
namespace GeoReVi
{
    public interface ILogFactory
    {

        #region  Properties

        /// <summary>
        /// The level of logging to output
        /// </summary>
        LogOutputLevel LogOutputLevel { get; set; }

        /// <summary>
        /// If true includes the origin of where the log message was logged from
        /// such as the class name, line number and file name
        /// </summary>
        bool IncludeLogOrigindetails { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Fires whenever a new log arrives
        /// </summary>
        event Action<(string Message, LogLevel Level)> NewLog;

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specific logger to this factory
        /// </summary>
        /// <param name="logger">The logger</param>
        void AddLogger(ILogger logger);

        /// <summary>
        /// Removes the specified logger from the factory
        /// </summary>
        /// <param name="logger"></param>
        void RemoveLogger(ILogger logger);

        /// <summary>
        /// Logs the specific message to all loggers in this factory
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">Message level</param>
        /// <param name="origin">The method this message was logged</param>
        /// <param name="filePath">The line of code in the filename this mesasge was logged from</param>
        void Log(string message,
            LogLevel level = LogLevel.Informative, 
            string origin = "", 
            string filePath = "",
            int lineNumber = 0);
       
        #endregion
    }
}
