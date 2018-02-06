using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class DebugHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Debug
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Debug(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Debug, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Debug
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Debug(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Debug, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Debug
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Debug(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Debug, ex, message);
        }

        /// <summary>
        /// Log a simple message as Debug
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }
    }
}
