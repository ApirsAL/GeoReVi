
namespace GeoReVi
{
    /// <summary>
    /// A logger that will output log messages
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Handles the logged message being passed in
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">Level of the message</param>
        void Log(string message, LogLevel level);
    }
}
