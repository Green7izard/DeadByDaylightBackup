using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadByDaylightBackup.Data;

namespace DeadByDaylightBackup.Interface
{
    public interface IBackupFileTrigger
    {
        void AddBackupFile(Backup backup);
        void RemoveBackupFile(long id);
    }
}
