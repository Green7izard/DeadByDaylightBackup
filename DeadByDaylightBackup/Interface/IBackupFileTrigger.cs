using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IBackupFileTrigger
    {
        void AddBackupFile(Backup backup);

        void RemoveBackupFile(long id);
    }
}
