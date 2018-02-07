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
    public class FilePathHandler : ADisposable, IFilePathHandler
    {
        private readonly IDictionary<long, FilePath> FilePathStore = new Dictionary<long, FilePath>();

        // private readonly FileUtility _filemanager;
        private readonly FilePathSettingsManager _settingManager;

        private readonly ILogger _logger;
        private readonly ITriggerLauncher<FilePath> _triggerLauncher;

        public FilePathHandler(ITriggerLauncher<FilePath> TriggerLauncher, FilePathSettingsManager settingManager, ILogger logger) : base()
        {
            _triggerLauncher = TriggerLauncher;
            _settingManager = settingManager;
            //_filemanager = filemanager;
            _logger = logger;
            long id = 1;
            foreach (var setting in _settingManager.GetSettings())
            {
                if (FileUtility.FileExists(setting.Path))
                {
                    setting.Id = id;
                    FilePathStore.Add(id, setting);
                    id++;
                }
            }
        }

        public long CreateFilePath(string path)
        {
            try
            {
                if (FileUtility.FileExists(path))
                {
                    var filePath = new FilePath
                    {
                        Path = path
                    };
                    lock (FilePathStore)
                    {
                        if (FilePathStore.Any(x => x.Value.Path.Equals(path, StringComparison.OrdinalIgnoreCase)))
                        {
                            return FilePathStore.First(x => x.Value.Path.Equals(path, StringComparison.OrdinalIgnoreCase)).Key;
                        }
                        else
                            for (long i = 0; i < long.MaxValue; i++)
                            {
                                if (!FilePathStore.ContainsKey(i))
                                {
                                    filePath.Id = i;
                                    FilePathStore.Add(i, filePath);
                                    _triggerLauncher.TriggerCreationEvent(filePath);
                                    _settingManager.SaveSettings(FilePathStore.Values.ToArray());
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
                _logger.Log(LogLevel.Fatal, ex, "Failed to add filepath '{0}' Because of {1}", path, ex.Message);
                throw;
            }
        }

        public void DeleteFilePath(long id)
        {
            try
            {
                lock (FilePathStore)
                {
                    if (FilePathStore.ContainsKey(id))
                    {
                        var backup = FilePathStore[id];
                        FilePathStore.Remove(id);
                        _triggerLauncher.TriggerDeletionEvent(backup);
                        _settingManager.SaveSettings(FilePathStore.Values.ToArray());
                    }
                    else
                    {
                        throw new KeyNotFoundException("Unkown Backup id");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex, "Failed to remove filepath '{0}' Because of {1}", id, ex.Message);
                throw;
            }
        }


        public FilePath[] GetAllFilePaths()
        {
            try
            {
                return FilePathStore.Values.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex, "Failed to get filepaths because of {0}", ex.Message);
                throw;
            }
        }

        public long[] SearchFilePaths()
        {
            try
            {
                var result = FileUtility.FullFileSearch("381210", "*.profjce");
                lock (FilePathStore)
                {
                    return result.Select(x => CreateFilePath(x)).ToArray();
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Warn, ex, "Failed to search for filepaths because of {0}", ex.Message);
                throw;
            }
        }

        public void RestoreBackup(Backup backup)
        {
            try
            {
                lock (FilePathStore)
                {
                    FilePath path = FilePathStore.Values.Single(x => x.UserCode == backup.UserCode);
                    FileUtility.Copy(backup.FullFileName, path.Path);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal, ex, "Failed to get Restore backup '{0}' because of {1}", backup.FullFileName, ex.Message);
                throw;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _settingManager.SaveSettings(FilePathStore.Values.ToArray());
                FilePathStore.Clear();
            }
        }
    }
}
