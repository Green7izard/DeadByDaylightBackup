using System;

namespace DeadByDaylightBackup.Logging.NLogger
{
    /// <summary>
    /// Uses NLog to log details
    /// </summary>
    public class NLogger : ILogger
    {
        /// <summary>
        /// The NLog to use
        /// </summary>
        private readonly NLog.ILogger _logger;

        /// <summary>
        /// The constuctor
        /// </summary>
        /// <param name="name">Name for the logger</param>
        public NLogger(string name)
        {
            _logger = NLog.LogManager.GetLogger(name);
        }

        /// <summary>
        /// Get the name
        /// </summary>
        public string Name
        {
            get
            {
                return _logger.Name;
            }
        }

        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Fatal: _logger.Fatal(message); return;
                case LogLevel.Error: _logger.Error(message); return;
                case LogLevel.Warn: _logger.Warn(message); return;
                case LogLevel.Info: _logger.Info(message); return;
                case LogLevel.Debug: _logger.Debug(message); return;
                default: _logger.Trace(message); return;
            }
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            switch (level)
            {
                case LogLevel.Fatal: _logger.Fatal(ex, message); return;
                case LogLevel.Error: _logger.Error(ex, message); return;
                case LogLevel.Warn: _logger.Warn(ex, message); return;
                case LogLevel.Info: _logger.Info(ex, message); return;
                case LogLevel.Debug: _logger.Debug(ex, message); return;
                default: _logger.Trace(ex, message); return;
            }
        }

        public void Log(LogLevel level, string message, params object[] stringParamters)
        {
            switch (level)
            {
                case LogLevel.Fatal: _logger.Fatal(message, stringParamters); return;
                case LogLevel.Error: _logger.Error(message, stringParamters); return;
                case LogLevel.Warn: _logger.Warn(message, stringParamters); return;
                case LogLevel.Info: _logger.Info(message, stringParamters); return;
                case LogLevel.Debug: _logger.Debug(message, stringParamters); return;
                default: _logger.Trace(message, stringParamters); return;
            }
        }

        public void Log(LogLevel level, Exception ex, string message, params object[] stringParamters)
        {
            switch (level)
            {
                case LogLevel.Fatal: _logger.Fatal(ex, message, stringParamters); return;
                case LogLevel.Error: _logger.Error(ex, message, stringParamters); return;
                case LogLevel.Warn: _logger.Warn(ex, message, stringParamters); return;
                case LogLevel.Info: _logger.Info(ex, message, stringParamters); return;
                case LogLevel.Debug: _logger.Debug(ex, message, stringParamters); return;
                default: _logger.Trace(ex, message, stringParamters); return;
            }
        }
    }
}
