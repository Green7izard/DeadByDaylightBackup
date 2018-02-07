using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Logging.Helper;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.Utility.Trigger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeadByDaylightBackup.Program
{
    public class FilePathHandler : ADisposable, IFilePathHandler
    {
        private readonly IDictionary<long, FilePath> FilePathStore;
        private readonly IDictionary<long, FileSystemWatcher> FileWatchers;

        private readonly FilePathSettingsManager _settingManager;

        private readonly ILogger _logger;
        private readonly ITriggerLauncher<FilePath> _triggerLauncher;

        public FilePathHandler(ITriggerLauncher<FilePath> TriggerLauncher, FilePathSettingsManager settingManager, ILogger logger) : base()
        {
            FileWatchers = new Dictionary<long, FileSystemWatcher>();
            FilePathStore = new Dictionary<long, FilePath>();
            _triggerLauncher = TriggerLauncher;
            _settingManager = settingManager;
            _logger = logger;
            foreach (var setting in _settingManager.GetSettings())
            {
                CreateFilePath(setting.Path);
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
                                    CreateFileWachter(filePath);
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

        private void CreateFileWachter(FilePath filePath)
        {
            lock (FileWatchers)
            {
                try
                {
                    string path = Path.GetDirectoryName(filePath.Path);
                    var watcher = new FileSystemWatcher(path, "*")
                    {
                        EnableRaisingEvents = true,
                        NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName |
                        NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security
                    };
                    FileWatchers.Add(filePath.Id, watcher);
                    watcher.Changed += (o, i) =>
                    {
                        if (FileUtility.FileExists(filePath.Path))
                        {
                            _triggerLauncher.TriggerUpdateEvent(filePath);
                        }
                        else
                        {
                            DeleteWatcher(filePath.Id);
                        }
                    };
                    watcher.Deleted += (o, i) =>
                    {
                        DeleteFilePath(filePath.Id);
                    };
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Could not monitor file: ", filePath.Path);
                }
            }
        }

        private void DeleteWatcher(long id)
        {
            lock (FileWatchers)
            {
                if (FileWatchers.ContainsKey(id))
                    try
                    {
                        FileWatchers[id].Dispose();
                    }
                    finally
                    {
                        FileWatchers.Remove(id);
                    }
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
                        DeleteWatcher(id);
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
                foreach (var x in FileWatchers.Keys)
                {
                    DeleteWatcher(x);
                }
                FileWatchers.Clear();
            }
        }
    }
}
