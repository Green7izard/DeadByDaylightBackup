using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DeadByDaylightBackup.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IFilePathTrigger, IBackupFileTrigger
    {
        #region privates

        private readonly IFilePathHandler filepathHandler;
        private readonly IBackupHandler backupHandler;

        private ICollection<FilePathRow> fileRows;
        private ICollection<BackUpRow> backupRows;

        private readonly ILogger _logger;

        public MainWindow(IFilePathHandler fileHandler, IBackupHandler backupHand, ILogger logger) : base()
        {
            _logger = logger;
            fileRows = new List<FilePathRow>(2);
            backupRows = new List<BackUpRow>(2);
            InitializeComponent();
            filepathHandler = fileHandler;
            backupHandler = backupHand;
            FoldersGrid.ShowGridLines = true;
            BackUpGrid.ShowGridLines = true;
            AddPathButton.Click += (o, i) => AddPathClick(o, i);
            BackupNowButton.Click += (o, i) => BackUpAll(o, i);
            SearchPathsButton.Click += (o, i) => SearchPaths(o, i);
            CleanupBackupsButton.Click += (o, i) => CleanupOldBackups(o, i);
        }

        #endregion privates

        #region startUp

        /// <summary>
        /// Activates the window
        /// </summary>
        public new void ShowDialog()
        {
            foreach (var x in filepathHandler.GetAllFilePaths())
            {
                CreationTrigger(x);
            }
            foreach (var x in backupHandler.GetBackups())
            {
                CreationTrigger(x);
            }
            base.ShowDialog();          
        }

        #endregion startUp

        #region buttonHandlers

        private void CleanupOldBackups(object o, RoutedEventArgs i)
        {
            CleanupBackupsButton.IsEnabled = false;
            try
            {
                backupHandler.CleanupOldBackups();
            }
            catch (Exception ex)
            {
                ShowPopup("Cleanup failed!", ex);
            }
            CleanupBackupsButton.IsEnabled = true;
        }

        private void SearchPaths(object o, RoutedEventArgs i)
        {
            SearchPathsButton.IsEnabled = false;
            try
            {
                filepathHandler.SearchFilePaths();
            }
            catch (Exception ex)
            {
                ShowPopup("File search failed!", ex);
            }
            SearchPathsButton.IsEnabled = true;
        }

        private void AddPathClick(object o, RoutedEventArgs i)
        {
            string txt = PathInput.Text.Trim();
            try
            {
                filepathHandler.CreateFilePath(FileUtility.GetFileWithExtension(txt, ".profjce"));
            }
            catch (Exception ex)
            {
                ShowPopup("Failed to add file!", ex);
            }
        }

        private void BackUpAll(object o, RoutedEventArgs i)
        {
            foreach (var filepath in filepathHandler.GetAllFilePaths())
            {
                try
                {
                    backupHandler.CreateBackup(filepath);
                }
                catch (Exception ex)
                {
                    ShowPopup($"Backup failed for {filepath.FileName}", ex);
                }
            }
        }

        #endregion buttonHandlers

        #region IBackupFileTrigger

        public void CreationTrigger(Backup backup)
        {
            try
            {
                if (!backupRows.Any(x => x.Identity.Id == backup.Id))
                {
                    var rowDefinition = new BackUpRow(backup);
                    rowDefinition.DeleteRowButton.Click += (o, i) =>
                    {
                        try
                        {
                            backupHandler.DeleteBackup(backup.Id);
                        }
                        catch (Exception ex)
                        {
                            ShowPopup($"Failed to remove backup {backup.FullFileName}!", ex);
                        }
                    };
                    rowDefinition.RestoreButton.Click += (o, i) =>
                    {
                        try { filepathHandler.RestoreBackup(backup); }
                        catch (Exception ex)
                        {
                            ShowPopup($"Failed to Restore backup {backup.FullFileName}!", ex);
                        }
                    };
                    lock (fileRows)
                    {
                        backupRows.Add(rowDefinition);
                        BackUpGrid.Children.Add(rowDefinition.PathLabel);
                        BackUpGrid.Children.Add(rowDefinition.DeleteRowButton);
                        BackUpGrid.Children.Add(rowDefinition.DateLabel);
                        BackUpGrid.Children.Add(rowDefinition.RestoreButton);
                        BackUpGrid.Children.Add(rowDefinition.UserCodeLabel);
                        BackUpGrid.Children.Add(rowDefinition.SizeLabel);
                        UpdateBackups();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup($"Failed to show Backup '{backup.FullFileName}'!", ex);
            }
        }

        public void DeletionTrigger(Backup deleted)
        {
            try
            {
                lock (backupRows)
                {
                    if (backupRows.Any(x => x.Identity.Equals(deleted)))
                    {
                        var row = backupRows.First(x => x.Identity.Equals(deleted));
                        int rowNumber = row.RowNumber;
                        BackUpGrid.Children.Remove(row.DeleteRowButton);
                        BackUpGrid.Children.Remove(row.PathLabel);
                        BackUpGrid.Children.Remove(row.DateLabel);
                        BackUpGrid.Children.Remove(row.RestoreButton);
                        BackUpGrid.Children.Remove(row.UserCodeLabel);
                        BackUpGrid.Children.Remove(row.SizeLabel);
                        BackUpGrid.RowDefinitions.RemoveAt(rowNumber);
                        backupRows.Remove(row);
                        UpdateBackups();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup($"Failed to remove Backup from UI'{deleted.FullFileName ?? deleted.Id.ToString()}'!", ex);
            }
        }

        #endregion IBackupFileTrigger

        #region IFilePathTrigger

        public void CreationTrigger(FilePath path)
        {
            try
            {
                if (!fileRows.Any(x => x.Identity.Id == path.Id))
                {
                    var rowDefinition = new FilePathRow(path);
                    rowDefinition.DeleteRowButton.Click += (o, i) =>
                    {
                        try
                        {
                            filepathHandler.DeleteFilePath(path.Id);
                        }
                        catch (Exception ex)
                        {
                            ShowPopup($"Failed to remove file {path.FileName}!", ex);
                        }
                    };
                    lock (fileRows)
                    {
                        fileRows.Add(rowDefinition);
                        FoldersGrid.Children.Add(rowDefinition.PathLabel);
                        FoldersGrid.Children.Add(rowDefinition.UserCodeLabel);
                        FoldersGrid.Children.Add(rowDefinition.DeleteRowButton);
                        FoldersGrid.Children.Add(rowDefinition.SizeLabel);
                        FoldersGrid.Children.Add(rowDefinition.DateLabel);
                        UpdateFilePaths();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup($"Failed to add Filepath '{path.FileName}'!", ex);
                _logger.Log(LogLevel.Warn, ex, "Failed to add FilePath '{0}'! Reason: {1}", path.Path, ex.Message);
            }
        }

        public void UpdateTrigger(FilePath input)
        {
            lock (fileRows)
            {
                if (fileRows.Any(x => x.Identity.Equals(input)))
                {
                    var row = fileRows.First(x => x.Identity.Equals(input));
                    row.Refresh();
                }
            }
        }

        public void DeletionTrigger(FilePath id)
        {
            try
            {
                lock (fileRows)
                {
                    if (fileRows.Any(x => x.Identity.Equals(id)))
                    {
                        var row = fileRows.First(x => x.Identity.Equals(id));
                        int rowNumber = row.RowNumber;
                        FoldersGrid.Children.Remove(row.DeleteRowButton);
                        FoldersGrid.Children.Remove(row.PathLabel);
                        FoldersGrid.Children.Remove(row.UserCodeLabel);
                        FoldersGrid.Children.Remove(row.SizeLabel);
                        FoldersGrid.Children.Remove(row.DateLabel);
                        FoldersGrid.RowDefinitions.RemoveAt(rowNumber);
                        fileRows.Remove(row);
                        UpdateFilePaths();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup($"Failed to remove Filepath from UI'{fileRows.Where(x => x.Identity.Equals(id)).Select(x => x.Identity.Path).FirstOrDefault() ?? id.ToString()}'!", ex);
            }
        }

        #endregion IFilePathTrigger

        #region UIHelper

        private void UpdateBackups()
        {
            try
            {
                lock (backupRows)
                {
                    int rowsInGrid = BackUpGrid.RowDefinitions.Count;
                    var first = BackUpGrid.RowDefinitions.First();
                    BackUpGrid.RowDefinitions.Clear();
                    BackUpGrid.RowDefinitions.Add(first);
                    int i = 1;
                    foreach (var row in backupRows.OrderBy(x => x.Identity.Date).ThenBy(x => x.Identity.UserCode))
                    {
                        BackUpGrid.RowDefinitions.Add(row);
                        row.RowNumber = i;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup("Update Backups Failed", ex);
            }
        }

        private void UpdateFilePaths()
        {
            try
            {
                lock (fileRows)
                {
                    int rowsInGrid = FoldersGrid.RowDefinitions.Count;
                    var first = FoldersGrid.RowDefinitions.First();
                    FoldersGrid.RowDefinitions.Clear();
                    FoldersGrid.RowDefinitions.Add(first);
                    int i = 1;
                    foreach (var row in fileRows.OrderBy(x => x.Identity.Path))
                    {
                        FoldersGrid.RowDefinitions.Add(row);
                        row.RowNumber = i;
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowPopup("Update FilePaths Failed", ex);
            }
        }

        /// <summary>
        /// Show a popup with a message
        /// </summary>
        /// <param name="message">the message to show</param>
        /// <param name="ex"> exception infomration</param>
        internal void ShowPopup(string message, Exception ex = null)
        {
            try
            {
                if (ex == null)
                    MessageBox.Show(this, message, message, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                else
                    MessageBox.Show(this, ex.ToString(), message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Fatal, exception, "Failed to show popup for exception! Message was {0}", message);
            }
        }

        #endregion UIHelper
    }
}
