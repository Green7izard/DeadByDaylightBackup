using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DeadByDaylightBackup.Settings
{
    public class BackupSettingsManager : ISettingsManager<Backup>
    {
        public BackupSettingsManager()
        {
            System.IO.Directory.CreateDirectory(GetBackupFileLocation());
        }

        public string GetBackupFileLocation()
        {
            var appSettings = ConfigurationManager.AppSettings;
            string result = appSettings["BackupLocation"] ?? "D:\\DeadByDaylightBackup\\";
            return result;
        }

        public ICollection<Backup> GetSettings()
        {
            var files = Directory.GetFiles(GetBackupFileLocation(), "*.profjce", SearchOption.AllDirectories);
            List<Backup> results = new List<Backup>(files.Length);
            foreach (var file in files)
            {
                try {
                    Backup result = new Backup()
                    {
                        FullFileName = file
                    };
                    var pathString = result.FullFileName.Replace(result.FileName, "").Trim(' ', '\\', '/', '.').Split('\\');
                    var dateString = pathString[pathString.Length - 2];
                    var playerstring = pathString[pathString.Length - 1];

                    result.Date = dateString.ToSimpleShortDate();
                    result.UserCode = playerstring;
                    results.Add(result);
                }
                catch
                {
                    //Ignore those we cant place... could be manual
                }
            }
            return results.ToArray();
        }

        public void SaveSettings(ICollection<Backup> input)
        {
            //IGNORE dynamic
        }
    }
}