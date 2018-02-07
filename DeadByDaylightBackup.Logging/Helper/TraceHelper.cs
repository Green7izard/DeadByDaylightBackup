using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class TraceHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Trace
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Trace(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Trace, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Trace
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Trace(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Trace, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Trace
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Trace(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Trace, ex, message);
        }

        /// <summary>
        /// Log a simple message as Trace
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Trace(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Trace, message);
        }
    }
}
