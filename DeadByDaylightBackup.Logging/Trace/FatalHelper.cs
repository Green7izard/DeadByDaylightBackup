using System;

namespace DeadByDaylightBackup.Logging.Helper
{
    public static class FatalHelper
    {
        /// <summary>
        /// Log a formatted string with an exception as Fatal
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="ex">The exception to log</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Fatal(this ILogger logger, Exception ex, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Fatal, ex, message, stringParamters);
        }

        /// <summary>
        /// Log a formatted string as Fatal
        /// </summary>
        /// <param name="message">String to format</param>
        /// <param name="stringParamters">Paramters for formatting</param>
        public static void Fatal(this ILogger logger, string message, params object[] stringParamters)
        {
            logger.Log(LogLevel.Fatal, message, stringParamters);
        }

        /// <summary>
        /// Log an Exception as Fatal
        /// </summary>
        /// <param name="ex">The exception to log</param>
        /// <param name="message">message to log</param>
        public static void Fatal(this ILogger logger, Exception ex, string message)
        {
            logger.Log(LogLevel.Fatal, ex, message);
        }

        /// <summary>
        /// Log a simple message as Fatal
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Fatal(this ILogger logger, string message)
        {
            logger.Log(LogLevel.Fatal, message);
        }
    }
}
