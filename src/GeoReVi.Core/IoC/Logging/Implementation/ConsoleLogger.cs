using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Logs the messages to the console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs the given message to the system console
        /// </summary>
        /// <param name="message">Message to log</param>
        /// <param name="level">The level of the message</param>
        public void Log(string message, LogLevel level)
        {
            //Old color
            var consoleOldColor = Console.ForegroundColor;

            //Default log color value
            var consoleColor = ConsoleColor.White;

            switch(level)
            {
                //Debug is blue
                case LogLevel.Debug:
                    consoleColor = ConsoleColor.Blue;
                    break;
                //Verbose is gray
                case LogLevel.Verbose:
                    consoleColor = ConsoleColor.Gray;
                    break;
                // Warning is yellow
                case LogLevel.Warning:
                    consoleColor = ConsoleColor.DarkYellow;
                    break;

                // Error is red
                case LogLevel.Error:
                    consoleColor = ConsoleColor.Red;
                    break;

                // Success is green
                case LogLevel.Success:
                    consoleColor = ConsoleColor.Green;
                    break;
            }

            Console.ForegroundColor = consoleColor;

            // Writing the message to the console in a given format
            Console.WriteLine($"[{level}]".PadRight(13, ' ') + message);
            Console.ForegroundColor = consoleOldColor;
        }
    }
}
