using System;
using System.Configuration;
using System.IO;
using System.Text;

namespace DeadByDaylightBackup.Logging.SimpleFile
{
    /// <summary>
    /// Logger that logs to a subfolder names logs in a TXT file
    /// </summary>
    public class FileLogger : ILogger
    {
        /// <summary>
        /// Register this class as the logger
        /// </summary>
        public static void Install()
        {
            LoggerFactory.SetLoggerType(typeof(FileLogger));
        }

        /// <summary>
        /// The folder it will log to
        /// </summary>
        public const string Folder = "logs\\";

        /// <summary>
        /// default filename
        /// </summary>
        public const string FileName = "SimpleFile";

        /// <summary>
        /// Extension to use
        /// </summary>
        public const string Extension = "txt";

        /// <summary>
        /// Whether it should use the name of the logger, or the default filename
        /// </summary>
        private readonly bool UseLoggerName;

        /// <summary>
        /// Create the fileLogger
        /// </summary>
        /// <param name="name">Name that could be used</param>
        public FileLogger(string name)
        {
            Name = name;
            try
            {
                UseLoggerName = "True".Equals(ConfigurationManager.AppSettings["SeperateFiles"], StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                UseLoggerName = false;
            }
        }

        public string Name
        {
            get;
        }

        public void Log(LogLevel level, string message)
        {
            WriteLine($"{Name}|{level.ToString()}: {message}");
        }

        public void Log(LogLevel level, Exception ex, string message)
        {
            WriteLine($"{Name}|{level.ToString()}: {message}\n{ex.ToString()}");
        }

        public void Log(LogLevel level, string message, params object[] stringParamters)
        {
            WriteLine(string.Format($"{Name}|{level.ToString()}: {message}", stringParamters));
        }

        public void Log(LogLevel level, Exception ex, string message, params object[] stringParamters)
        {
            WriteLine(string.Format($"{Name}|{level.ToString()}: {message}\n{ex.ToString()}", stringParamters));
        }

        /// <summary>
        /// Write a line to a file
        /// </summary>
        /// <param name="input">string to put on a new line</param>
        private void WriteLine(string input)
        {
            string folder = Path.GetFullPath(Folder);
            Directory.CreateDirectory(folder);
            string fileName = UseLoggerName ? Name : FileName;
            string filePath = Path.Combine(folder, $"{fileName}.{Extension}");
            File.AppendAllText(filePath, $"{input}{Environment.NewLine}", Encoding.UTF8);
        }
    }
}
