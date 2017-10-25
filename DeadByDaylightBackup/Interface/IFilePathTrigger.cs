using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadByDaylightBackup.Data;


namespace DeadByDaylightBackup.Interface
{
    public interface IFilePathTrigger
    {
        void AddFilePath(FilePath path);
        void RemoveFilePath(long id);
    }
}
