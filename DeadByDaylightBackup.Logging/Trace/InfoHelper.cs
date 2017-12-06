using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class InfoHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Info
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Info(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Info, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Info
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Info(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Info, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Info
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Info(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Info, ex, message);
        }

        /// <summary>
        /// Log a simple message as Info
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Info(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Info, message);
        }
    }
}
