using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Logging
{
    /// <summary>
    /// Helper for logging information
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// The name of this logger
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Log a formatted string with an exception
        /// </summary>
        /// <param name="level">Loglevel</param>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        void Log(LogLevel level, Exception ex, string message, params object[] stringParamters);

        /// <summary>
        /// Log a formatted string
        /// </summary>
        /// <param name="level">Loglevel</param>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        void Log(LogLevel level, string message, params object[] stringParamters);

        /// <summary>
        /// Log an Exception
        /// </summary>
        /// <param name="level">Loglevel</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        void Log(LogLevel level, Exception ex, string message);
        /// <summary>
        /// Log a simple message
        /// </summary>
        /// <param name="level">Loglevel</param>
        /// <param name="message">message to log</param>
        void Log(LogLevel level, string message);
    }
}
