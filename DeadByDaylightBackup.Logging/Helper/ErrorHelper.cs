using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class ErrorHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Error
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Error(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Error, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Error
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Error(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Error, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Error
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Error(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Error, ex, message);
        }

        /// <summary>
        /// Log a simple message as Error
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Error(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Error, message);
        }
    }
}
