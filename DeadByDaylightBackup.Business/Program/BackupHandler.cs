using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Program
{
    /// <summary>
    /// Basic handler for backups!
    /// </summary>
    public class BackupHandler : IBackupHandler, IDisposable
    {
        private readonly IDictionary<long, Backup> BackupStore = new Dictionary<long, Backup>();
        private readonly IList<IBackupFileTrigger> Triggerlist = new List<IBackupFileTrigger>(1);
        private readonly int _numberOfSavesTokeep;

        // private readonly FileUtility _filemanager;
        private readonly BackupSettingsManager _settingManager;

        private readonly ILogger _logger;

        public BackupHandler(int numberOfSaves, //FileUtility filemanager,
            BackupSettingsManager settingManager, ILogger logger)
        {
            _numberOfSavesTokeep = numberOfSaves;
            _settingManager = settingManager;
            //_filemanager = filemanager;
            _logger = logger;
            long id = 0;
            foreach (var setting in _settingManager.GetSettings())
            {
                setting.Id = id;
                BackupStore.Add(id, setting);
                id++;
            }
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overidable dispose function
        /// </summary>
        /// <param name="final"></param>
        protected virtual void Dispose(bool final)
        {
            lock (Triggerlist)
            {
                _settingManager.SaveSettings(BackupStore.Values.ToArray());
                Triggerlist.Clear();
                BackupStore.Clear();
            }
        }

        public long CreateBackup(FilePath filepath)
        {
            try
            {
                string fileName = FileUtility.GetFileName(filepath.Path);
                string DateFolder = FileUtility.MergePaths(_settingManager.GetBackupFileLocation(), filepath.LastEdited.SimpleShortFormat());
                string Playerfolder = FileUtility.MergePaths(DateFolder, filepath.UserCode);
                string targetFile = FileUtility.MergePaths(Playerfolder, FileUtility.GetFileName(filepath.Path));
                Backup backup = new Backup
                {
                    FullFileName = targetFile,
                    Date = filepath.LastEdited,
                    UserCode = filepath.UserCode
                };
                lock (BackupStore)
                {
                    if (BackupStore.Any(x => x.Value.FullFileName == backup.FullFileName))
                    {
                        return BackupStore.First(x => x.Value.FullFileName == backup.FullFileName).Key;
                    }
                    else
                    {
                        for (long i = 0; i < long.MaxValue; i++)
                        {
                            if (!BackupStore.ContainsKey(i))
                            {
                                backup.Id = i;
                                FileUtility.CreateDirectory(DateFolder);
                                FileUtility.CreateDirectory(Playerfolder);
                                FileUtility.Copy(filepath.Path, targetFile);
                                BackupStore.Add(i, backup);
                                TriggerCreate(backup);
                                _settingManager.SaveSettings(BackupStore.Values.ToArray());
                                return i;
                            }
                        }
                        throw new ArgumentOutOfRangeException("No More room in long!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal, ex, "Failed to backup '{0}' Because of {1}", filepath.Path, ex.Message);
                throw;
            }
        }

        public void DeleteBackup(long id)
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
                        FileUtility.DeleteFile(backup.FullFileName);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Unkown Backup id");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Failed to remove backup '{0}' Because of {1}", id, ex.Message);
                throw;
            }
        }

        public ICollection<Backup> GetBackups()
        {
            try
            {
                lock (BackupStore)
                    return BackupStore.Select(x => x.Value).ToArray();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex, "Failed to retrieve backups because of {0}", ex.Message);
                throw;
            }
        }

        public void Register(IBackupFileTrigger trigger)
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
                _logger.Log(LogLevel.Warn, ex, "Failed to register trigger because of {0}", ex.Message);
                throw;
            }

            foreach (var val in this.BackupStore.Values)
            {
                trigger.AddBackupFile(val);
            }
        }

        private void TriggerCreate(Backup backup)
        {
            lock (Triggerlist)
                foreach (IBackupFileTrigger trigger in Triggerlist)
                {
                    try
                    {
                        trigger.AddBackupFile(backup);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Warn, ex, "{1} has caused an errror!", trigger.GetType());
                    }
                }
        }

        private void TriggerDelete(long id)
        {
            lock (Triggerlist)
                foreach (IBackupFileTrigger trigger in Triggerlist)
                {
                    try
                    {
                        trigger.RemoveBackupFile(id);
                    }
                    catch (Exception ex)
                    {
                        _logger.Log(LogLevel.Warn, ex, "{1} has caused an errror!", trigger.GetType());
                    }
                }
        }

        public void CleanupOldBackups()
        {
            try
            {
                lock (BackupStore)
                {
                    var backupGroups = BackupStore.Values.GroupBy(x => x.UserCode);
                    foreach (var group in backupGroups)
                    {
                        Backup[] latestsDateIds = new Backup[0];
                        Backup[] largestFiles = new Backup[0];
                        Parallel.Invoke(
                            //Get the latests files of a date!
                            () => latestsDateIds = group.GroupBy(x => x.Date.GetValueOrDefault().Date)
                                    .Select(y => y.OrderByDescending(x => x.Date).LastOrDefault())
                                    .ToArray(),
                            //Get the largest file of a date!
                            () => largestFiles = group.GroupBy(x => x.Date.GetValueOrDefault().Date)
                                    .Select(y => y.OrderByDescending(x => FileUtility.GetFileSize(x.FullFileName)).LastOrDefault())
                                    .ToArray());
                        long[] safeIds = (latestsDateIds.Concat(largestFiles)).OrderByDescending(x => x.Date)
                                    .Select(x => x.Id)
                                    .Where(x => x != 0).Take(Math.Min(_numberOfSavesTokeep, 1)).ToArray();
                        long[] idsForDeletion = BackupStore.Keys.Where(x => safeIds.Contains(x)).ToArray();
                        foreach (var id in idsForDeletion)
                        {
                            DeleteBackup(id);
                        }
                    }
                }
            }
            catch (AggregateException ag)
            {
                ag.Handle(ex => { _logger.Log(LogLevel.Warn, ex, "Failed to select old backups for deletion!"); return true; });
                throw ag.InnerException ?? ag.InnerExceptions.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex, "Failed to delete old backups!");
                throw;
            }
        }
    }
}
