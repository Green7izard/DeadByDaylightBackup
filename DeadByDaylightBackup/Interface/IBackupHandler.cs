using DeadByDaylightBackup.Data;
using System.Collections.Generic;

namespace DeadByDaylightBackup.Interface
{
    public interface IBackupHandler
    {
        void Register(IBackupFileTrigger trigger);

        long CreateBackup(FilePath fullFilePath);

        void DeleteBackup(long id);

        ICollection<Backup> GetBackups();
        void CleanupOldBackups();
    }
}
