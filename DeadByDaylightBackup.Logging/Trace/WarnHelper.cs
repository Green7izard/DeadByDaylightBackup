using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class WarnHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Warn
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Warn(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Warn, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Warn
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Warn(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Warn, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Warn
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Warn(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Warn, ex, message);
        }

        /// <summary>
        /// Log a simple message as Warn
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Warn(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Warn, message);
        }
    }
}
