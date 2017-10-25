using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IFilePathHandler
    {
        void Register(IFilePathTrigger trigger);

        long CreateFilePath(string path);
        void DeleteFilePath(long id);
        FilePath[] GetAllFilePaths();
        long[] SearchFilePaths();
        void RestoreBackup(Backup backup);
    }
}
