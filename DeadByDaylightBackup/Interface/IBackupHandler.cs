using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IBackupHandler
    {
        void Register(IBackupFileTrigger trigger);

        long CreateBackup(FilePath fullFilePath);
        void DeleteBackup(long id);

        ICollection<Backup> GetBackups();
    }
}
