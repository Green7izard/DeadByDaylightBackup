using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeadByDaylightBackup.Utility.Settings
{
    public abstract class SettingsManager<T> : ISettingsManager<T> where T : class
    {
        private const string Extension = ".Properties";
        private const string SettingsFolder = "Settings";
        private static readonly string ExecutingPath = AppDomain.CurrentDomain.BaseDirectory;
        private readonly string SettingsFilePath;

        protected SettingsManager(string filename)
        {
            string fullPath = Path.Combine(ExecutingPath, SettingsFolder);
            System.IO.Directory.CreateDirectory(fullPath);
            SettingsFilePath = Path.Combine(fullPath, filename.Trim(' ', '.') + Extension);
        }

        public ICollection<T> GetSettings()
        {
            string input = File.Exists(SettingsFilePath) ? File.ReadAllText(SettingsFilePath) : null;
            return ConvertFromText(input);
        }

        protected abstract ICollection<T> ConvertFromText(string input);

        public void SaveSettings(ICollection<T> input)
        {
            string result = ConvertToText(input);
            FileUtility.WriteToFile(SettingsFilePath, result);
        }

        protected abstract string ConvertToText(ICollection<T> input);

        public static ICollection<string> SplitPerLine(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new string[] { };
            return input.Split('\n', '\r').Select(x => x.Trim(' ', ';', '\t')).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }
}
