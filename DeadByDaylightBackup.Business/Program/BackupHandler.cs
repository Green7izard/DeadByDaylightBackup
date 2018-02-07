using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.Utility.Trigger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadByDaylightBackup.Program
{
    /// <summary>
    /// Basic handler for backups!
    /// </summary>
    public class BackupHandler : IBackupHandler
    {
        private readonly IDictionary<long, Backup> BackupStore = new Dictionary<long, Backup>();
        private readonly int _numberOfSavesTokeep;

        private static readonly TimeSpan RoundingFactor = new TimeSpan(0, 15, 0);

        // private readonly FileUtility _filemanager;
        private readonly BackupSettingsManager _settingManager;

        private readonly ILogger _logger;
        private ITriggerLauncher<Backup> _triggerLauncher;

        public BackupHandler(ITriggerLauncher<Backup> triggerLauncher, int numberOfSaves, BackupSettingsManager settingManager, ILogger logger)
        {
            _triggerLauncher = triggerLauncher;
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

        public long CreateBackup(FilePath filepath)
        {
            try
            {
                string fileName = FileUtility.GetFileName(filepath.Path);
                string DateFolder = FileUtility.MergePaths(_settingManager.GetBackupFileLocation(), filepath.LastEdited.SimpleShortFormat());
                string Playerfolder = FileUtility.MergePaths(DateFolder, filepath.UserCode);
                string targetFile = FileUtility.MergePaths(Playerfolder, FileUtility.GetFileName(filepath.Path));

                DateTime lastEdit = filepath.LastEdited;
                string userCode = filepath.UserCode;

                lock (BackupStore)
                {
                    Backup curBackup = BackupStore.Select(x => x.Value).FirstOrDefault(x => x.Date.RoundUp(RoundingFactor) == x.Date.RoundUp(RoundingFactor)
                      && x.UserCode.Equals(userCode));
                    if (curBackup != null)
                    {
                        return curBackup.Id;
                    }
                    else
                    {
                        curBackup = new Backup
                        {
                            FullFileName = targetFile,
                            Date = lastEdit,
                            UserCode = userCode
                        };
                        for (long i = 0; i < long.MaxValue; i++)
                        {
                            if (!BackupStore.ContainsKey(i))
                            {
                                curBackup.Id = i;
                                FileUtility.CreateDirectory(DateFolder);
                                FileUtility.CreateDirectory(Playerfolder);
                                FileUtility.Copy(filepath.Path, targetFile);
                                BackupStore.Add(i, curBackup);
                                _triggerLauncher.TriggerCreationEvent(curBackup);
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

        /// <summary>
        /// Delete the backup with a specified id
        /// </summary>
        /// <param name="id">id to delete</param>
        public void DeleteBackup(long id)
        {
            try
            {
                lock (BackupStore)
                {
                    if (BackupStore.ContainsKey(id))
                    {
                        var backup = BackupStore[id];
                        while (BackupStore.ContainsKey(id))
                        {
                            BackupStore.Remove(id);
                        }
                        _triggerLauncher.TriggerDeletionEvent(backup);
                        //  _settingManager.SaveSettings(BackupStore.Values.ToArray());
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

        public void CleanupOldBackups()
        {
            try
            {
                lock (BackupStore)
                {
                    CleanUpBadBackups();
                    var backupGroups = BackupStore.Values.Where(x => x != null).GroupBy(x => x.UserCode);
                    foreach (var group in backupGroups.Where(x => x.Count() > Math.Max(_numberOfSavesTokeep, 2)))
                    {
                        Backup largestFile = group.OrderByDescending(x => FileUtility.GetFileSize(x.FullFileName)).First();
                        Backup[] dateGroups = group.GroupBy(x => x.Date.GetValueOrDefault().Date)
                            .Select(y => y.OrderByDescending(x => x.Date)
                            .LastOrDefault(x => x.Id != largestFile.Id)).ToArray();
                        Backup[] latestsDateIds = dateGroups.Where(x => x != null && x.Date != null)
                            .OrderByDescending(x => x.Date).Take(_numberOfSavesTokeep).ToArray();
                        var idsForDeletion = BackupStore.Values.Where(x => !latestsDateIds.Any(y => y.Id == x.Id) && x.Id != largestFile.Id
                         && x.UserCode == group.Key).Select(x => x.Id);
                        RemoveBackups(idsForDeletion);
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

        /// <summary>
        /// Remove a selection of backups!
        /// </summary>
        /// <param name="backups"></param>
        private void RemoveBackups(IEnumerable<long> backups)
        {
            var idsForDeletion = backups.Distinct().Where(x => BackupStore.ContainsKey(x)).ToArray();
            foreach (var id in idsForDeletion)
            {
                DeleteBackup(id);
            }
        }

        /// <summary>
        /// Cleanup Backups that are not filled!
        /// </summary>
        private void CleanUpBadBackups()
        {
            lock (BackupStore)
            {
                RemoveBackups(BackupStore.Where(x => x.Value == null).Select(x => x.Key));
                RemoveBackups(BackupStore.Where(x => x.Value.Date == null).Select(x => x.Key));
                RemoveBackups(BackupStore.Where(x => x.Value.FullFileName == null).Select(x => x.Key));
                RemoveBackups(BackupStore.Where(x => x.Value.FileName == null).Select(x => x.Key));
            }
        }

        /*    /// <summary>
            /// Disposes the object
            /// </summary>
            /// <param name="disposing">True if disposing</param>
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    lock (Triggerlist)
                    {
                        _settingManager.SaveSettings(BackupStore.Values.ToArray());
                        Triggerlist.Clear();
                        BackupStore.Clear();
                    }
                }
            }*/
    }
}
