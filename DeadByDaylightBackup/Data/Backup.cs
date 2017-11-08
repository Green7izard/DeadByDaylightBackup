using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DeadByDaylightBackup.Utility;

namespace DeadByDaylightBackup.Data
{
    [Serializable]
    public class Backup : Identifyable
    {

        public  Backup()
        {
        }
        public Backup(string fileName)
        {
            FullFileName = fileName;
            var filePath = FullFileName.Replace(FileName, "").Split('\\');
            UserCode = filePath.Last();
            var datestring = filePath[filePath.Length - 2];
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
