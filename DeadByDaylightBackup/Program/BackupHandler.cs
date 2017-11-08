﻿using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.Utility;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadByDaylightBackup.Program
{
    public class BackupHandler : IBackupHandler, IDisposable
    {
        private readonly IDictionary<long, Backup> BackupStore = new Dictionary<long, Backup>();
        private readonly IList<IBackupFileTrigger> Triggerlist = new List<IBackupFileTrigger>(1);

        private readonly FileManager _filemanager;
        private readonly BackupSettingsManager _settingManager;
        private readonly Logger _logger;

        public BackupHandler(FileManager filemanager, BackupSettingsManager settingManager, Logger logger)
        {
            _settingManager = settingManager;
            _filemanager = filemanager;
            _logger = logger;
            long id = 0;
            foreach (var setting in _settingManager.GetSettings())
            {
                setting.Id = id;
                BackupStore.Add(id, setting);
                id++;
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

        public long CreateBackup(FilePath filepath)
        {
            try
            {
                string fileName = FileManager.GetFileName(filepath.Path);
                string DateFolder = FileManager.MergePaths(_settingManager.GetBackupFileLocation(), filepath.LastEdited.SimpleShortFormat());
                string Playerfolder = FileManager.MergePaths(DateFolder, filepath.UserCode);
                string targetFile = FileManager.MergePaths(Playerfolder, FileManager.GetFileName(filepath.Path));
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
                                FileManager.CreateDirectory(DateFolder);
                                FileManager.CreateDirectory(Playerfolder);
                                FileManager.Copy(filepath.Path, targetFile);
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
                _logger.Fatal(ex, "Failed to backup '{0}' Because of {1}", filepath.Path, ex.Message);
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
                        _filemanager.DeleteFile(backup.FullFileName);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Unkown Backup id");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to remove backup '{0}' Because of {1}", id, ex.Message);
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
                _logger.Warn(ex, "Failed to retrieve backups because of {0}", ex.Message);
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
                _logger.Warn(ex, "Failed to register trigger because of {0}", ex.Message);
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
                        _logger.Warn(ex, "{1} has caused an errror!", trigger.GetType());
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
                        _logger.Warn(ex, "{1} has caused an errror!", trigger.GetType());
                    }
                }
        }
    }
}
