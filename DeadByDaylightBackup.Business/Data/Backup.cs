using DeadByDaylightBackup.Utility;
using System;
using System.IO;
using System.Linq;

namespace DeadByDaylightBackup.Data
{
    [Serializable]
    public class Backup : Identifyable
    {
        public static string Numbers = "0123456789";

        public Backup()
        {
        }

        public Backup(string fullFilePath)
        {
            FullFileName = fullFilePath;
            var filePath = FullFileName.Replace(FileName, "").Split('\\').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            UserCode = filePath.Last();
            if (UserCode.Any(x => !Numbers.Any(y => x == y))) throw new InvalidOperationException("Usercode not found or not correct: " + UserCode);
            var datestring = filePath[filePath.Length - 2];
            if (datestring.Length != 12 || datestring.Any(x => !Numbers.Any(y => x == y))) throw new InvalidOperationException("Datestring not found or not correct: " + datestring);
            Date = datestring.ToSimpleShortDate();
        }

        public string FullFileName { get; set; }

        public virtual string FileName
        {
            get
            {
                return Path.GetFileName(FullFileName);
            }
        }

        public DateTime? Date
        { get; set; }

        public string UserCode
        {
            get;
            set;
        }
    }
}
