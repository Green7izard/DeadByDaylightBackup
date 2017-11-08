using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility;
using NLog;
using DeadByDaylightBackup.Settings;

namespace DeadByDaylightBackup.Program
{
    class FilePathHandler : IFilePathHandler
    {
        private readonly IDictionary<long, FilePath> BackupStore = new Dictionary<long, FilePath>();
        private readonly IList<IFilePathTrigger> Triggerlist = new List<IFilePathTrigger>(1);

        private readonly FileManager _filemanager;
        private readonly FilePathSettingsManager _settingManager;
        private readonly Logger _logger;

        public FilePathHandler(FileManager filemanager, FilePathSettingsManager settingManager, Logger logger)
        {
            _settingManager = settingManager;
            _filemanager = filemanager;
            _logger = logger;
            long id = 1;
            foreach (var setting in _settingManager.GetSettings())
            {
                if (FileManager.FileExists(setting.Path))
                {
                    setting.Id = id;
                    BackupStore.Add(id, setting);
                    id++;
                }
            }
        }
        public void Dispose()
        {
            lock (Triggerlist)
            {
                _settingManager.SaveSettings(BackupStore.Values.ToArray());
                Triggerlist.Clear();
                BackupStore.Clear();
            }
        }

        public long CreateFilePath(string path)
        {
            try
            {
                if (FileManager.FileExists(path))
                {
                    var filePath = new FilePath
                    {
                        Path = path
                    };
                    lock (BackupStore)
                    {
                        if (BackupStore.Any(x => x.Value.Path.Equals(path, StringComparison.OrdinalIgnoreCase)))
                        {
                            return BackupStore.First(x => x.Value.Path.Equals(path, StringComparison.OrdinalIgnoreCase)).Key;
                        }
                        else
                            for (long i = 0; i < long.MaxValue; i++)
                            {
                                if (!BackupStore.ContainsKey(i))
                                {
                                    filePath.Id = i;
                                    BackupStore.Add(i, filePath);
                                    TriggerCreate(filePath);
                                    _settingManager.SaveSettings(BackupStore.Values.ToArray());
                                    return i;
                                }
                            }

                    }
                    throw new ArgumentOutOfRangeException("No More room in long!");
                }
                else throw new KeyNotFoundException("Unkown File " + path);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to add filepath '{0}' Because of {1}", path, ex.Message);
                throw;
            }
        }

        public void DeleteFilePath(long id)
        {
            try
            {
                lock (BackupStore)
                {
                    if (BackupStore.ContainsKey(id))
                    {
                        var backup = BackupStore[id];
                        BackupStore.Remove(id);
                        TriggerDelete(id);
                        _settingManager.SaveSettings(BackupStore.Values.ToArray());
                    }
                    else
                    {
                        throw new KeyNotFoundException("Unkown Backup id");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to remove filepath '{0}' Because of {1}", id, ex.Message);
                throw;
            }
        }

        public void Register(IFilePathTrigger trigger)
        {
            try
            {
                lock (Triggerlist)
                {
                    if (Triggerlist.Contains(trigger))
                    {
                        throw new InvalidOperationException("Trigger already Registered");
                    }
                    else
                    {
                        Triggerlist.Add(trigger);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to register trigger  Because of {0}", ex.Message);
                throw;
            }
            foreach (var val in this.BackupStore.Values)
            {
                trigger.AddFilePath(val);
            }
        }


        private void TriggerCreate(FilePath backup)
        {
            lock (Triggerlist)
                foreach (IFilePathTrigger trigger in Triggerlist)
                {
                    try
                    {
                        trigger.AddFilePath(backup);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn(ex, "{1} has caused an errror!", trigger.GetType());
                    }
                }
        }
        private void TriggerDelete(long id)
        {
            lock (Triggerlist)
                foreach (IFilePathTrigger trigger in Triggerlist)
                {
                    try
                    {
                        trigger.RemoveFilePath(id);
                    }
                    catch (Exception ex)
                    {
                        _logger.Warn(ex, "{1} has caused an errror!", trigger.GetType());
                    }
                }
        }

        public FilePath[] GetAllFilePaths()
        {
            try
            {
                return BackupStore.Values.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to get filepaths because of {0}", ex.Message);
                throw;
            }
        }

        public long[] SearchFilePaths()
        {
            try
            {
                var result = FileManager.FullFileSearch("381210", "*.profjce");
                lock (BackupStore)
                {
                    return result.Select(x => CreateFilePath(x)).ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to search for filepaths because of {0}", ex.Message);
                throw;
            }
        }

        public void RestoreBackup(Backup backup)
        {
            try { 
            lock (BackupStore)
            {
                FilePath path = BackupStore.Values.Single(x => x.UserCode == backup.UserCode);
                FileManager.Copy(backup.FullFileName, path.Path);
            }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Failed to get Restore backup '{0}' because of {1}",backup.FullFileName, ex.Message);
                throw;
            }
        }
    }
}
