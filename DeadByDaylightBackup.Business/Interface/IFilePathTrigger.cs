using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IFilePathTrigger
    {
        void AddFilePath(FilePath path);

        void RemoveFilePath(long id);
    }
}
