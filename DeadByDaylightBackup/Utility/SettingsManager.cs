using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using DeadByDaylightBackup.Data;
using System.Configuration;

namespace DeadByDaylightBackup.Utility
{
    public abstract class SettingsManager<T> where T :class
    {
        private const string Extension = ".Properties";
        private const string SettingsFolder = "Settings";
        private static readonly string ExecutingPath =AppDomain.CurrentDomain.BaseDirectory;
        private readonly string SettingsFilePath;
        protected  SettingsManager(string filename) 
        {
            string fullPath = Path.Combine(ExecutingPath, SettingsFolder);
            System.IO.Directory.CreateDirectory(fullPath);
            SettingsFilePath = Path.Combine(fullPath, filename.Trim(' ','.') + Extension);
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
            FileManager.WriteToFile(SettingsFilePath, result);
        }
        protected abstract string ConvertToText(ICollection<T> input);

        public static ICollection<string> SplitPerLine(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return new string[] { };
            return input.Split('\n', '\r').Select(x => x.Trim(' ', ';', '\t')).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }
    }

    public class BackupSettingsManager : SettingsManager<Backup>
    {
        public BackupSettingsManager(): base("Backups")
        {
            System.IO.Directory.CreateDirectory(GetBackupFileLocation());
        }

        protected override ICollection<Backup> ConvertFromText(string input)
        {
            var results =  SplitPerLine(input).Select(x => new Backup
            {
                FullFileName = x
            }).ToArray();
            foreach(var result in results)
            {
                var pathString = result.FullFileName.Replace(result.FileName, "").Trim(' ', '\\','/', '.').Split('\\');
                var dateString = pathString[pathString.Length-2];
                var playerstring = pathString[pathString.Length - 1];
                result.Date = dateString.ToSimpleDate();
                result.UserCode = playerstring;
            }
            return results;
        }

        protected override string ConvertToText(ICollection<Backup> input)
        {
            return string.Join(Environment.NewLine, input.Select(x => x.FullFileName));
        }

        public string GetBackupFileLocation()
        {
            var appSettings= ConfigurationManager.AppSettings;
            string result = appSettings["BackupLocation"] ?? "D:\\DeadByDaylightBackup\\";
            return result;
        }
    }


    public class FilePathSettingsManager : SettingsManager<FilePath>
    {
        public FilePathSettingsManager() : base("FilePaths")
        {
        }

        protected override ICollection<FilePath> ConvertFromText(string input)
        {
            var results = SplitPerLine(input).Select(x => new FilePath
            {
                Path = x
            }).ToArray();
           
            return results;
        }

        protected override string ConvertToText(ICollection<FilePath> input)
        {
            return string.Join(Environment.NewLine, input.Select(x => x.Path));
        }

    }
}
