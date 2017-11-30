using System;
using System.IO;
using System.Text;

namespace DeadByDaylightBackup.Logging.SimpleFile
{
    public class FileLogger : ILogger
    {
        public const string LoggerLocationKey = "SimpleFileLoggerFile";
        public const string DefaultLoggerLocation = "'\\log.txt";

        public FileLogger(string name)
        {
            Name = name;
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

        private void WriteLine(string input)
        {
            string path = ConfigHelper.GetOrCreateSetting(LoggerLocationKey, DefaultLoggerLocation);
            File.AppendAllText(path, $"{input}{Environment.NewLine}", Encoding.UTF8);
        }
    }
}
