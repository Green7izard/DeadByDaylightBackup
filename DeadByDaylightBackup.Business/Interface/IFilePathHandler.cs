using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IFilePathHandler
    {
        long CreateFilePath(string path);

        void DeleteFilePath(long id);

        FilePath[] GetAllFilePaths();

        long[] SearchFilePaths();

        void RestoreBackup(Backup backup);
    }

}
