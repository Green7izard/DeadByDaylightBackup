using DeadByDaylightBackup.Data;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace DeadByDaylightBackup.Settings
{
    public class BackupSettingsManager : ISettingsManager<Backup>
    {
        public BackupSettingsManager()
        {
            Directory.CreateDirectory(GetBackupFileLocation());
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
                try
                {
                    results.Add(new Backup(file));
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
