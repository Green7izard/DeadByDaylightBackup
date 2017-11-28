using System;
using System.Diagnostics;

namespace DeadByDaylightBackup.Logging
{
    /// <summary>
    /// Logger for cases where no logger was found
    /// </summary>
    internal sealed class VoidLogger : ILogger
    {
        internal VoidLogger(string name)
        {
            Name = name;
        }

        public string Name
        {
            get;
        }

        public void Log(LogLevel level, string message)
        {
            Debug.WriteLine($"{Name}|{level.ToString()}: {message}");
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            Debug.WriteLine($"{Name}|{level.ToString()}: {message}\n{ex.ToString()}");
        }

        public void Log(LogLevel level, string message, params object[] stringParamters)
        {
            Debug.WriteLine(string.Format($"{Name}|{level.ToString()}: {message}", stringParamters));
        }

        public void Log(LogLevel level, Exception ex, string message, params object[] stringParamters)
        {
            Debug.WriteLine(string.Format($"{Name}|{level.ToString()}: {message}\n{ex.ToString()}", stringParamters));
        }
    }
}
