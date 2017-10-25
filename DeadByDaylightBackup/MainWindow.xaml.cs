using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Interface;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.View;
namespace DeadByDaylightBackup
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IFilePathTrigger, IBackupFileTrigger
    {

        private readonly IFilePathHandler filepathHandler;
        private readonly IBackupHandler backupHandler;

        private ICollection<FilePathRow> fileRows;
        private ICollection<BackUpRow> backupRows;

        public MainWindow(IFilePathHandler fileHandler, IBackupHandler backupHand) : base()
        {
            fileRows = new List<FilePathRow>(2);
            backupRows = new List<BackUpRow>(2);
            InitializeComponent();
            filepathHandler = fileHandler;
            backupHandler = backupHand;
            filepathHandler.Register(this);
            backupHandler.Register(this);
            FoldersGrid.ShowGridLines = true;
            BackUpGrid.ShowGridLines = true;
            AddPathButton.Click += (o, i) => AddPathClick(o, i);
            BackupNowButton.Click += (o, i) => BackUpAll(o, i);
            SearchPathsButton.Click += (o, i) => SearchPaths(o, i);
        }

        private void  SearchPaths(object o, RoutedEventArgs i)
        {

            SearchPathsButton.IsEnabled = false;
            filepathHandler.SearchFilePaths();
            SearchPathsButton.IsEnabled = true;
        }
        private void AddPathClick(object o, RoutedEventArgs i)
        {
            string txt = PathInput.Text.Trim();
            filepathHandler.CreateFilePath(FileManager.GetFileWithExtension(txt, ".profjce"));
        }
        private void BackUpAll(object o, RoutedEventArgs i)
        {
            foreach (var filepath in filepathHandler.GetAllFilePaths())
            { backupHandler.CreateBackup(filepath); }
        }


        public void AddBackupFile(Backup backup)
        {
            if (!backupRows.Any(x => x.Identity.Id == backup.Id))
            {
                var rowDefinition = new BackUpRow(backup);
                rowDefinition.DeleteRowButton.Click += (o, i) => backupHandler.DeleteBackup(backup.Id);
                rowDefinition.RestoreButton.Click += (o, i) => filepathHandler.RestoreBackup(backup);
                lock (fileRows)
                {
                    backupRows.Add(rowDefinition);
                    BackUpGrid.Children.Add(rowDefinition.PathLabel);
                    BackUpGrid.Children.Add(rowDefinition.DeleteRowButton);
                    BackUpGrid.Children.Add(rowDefinition.DateLabel);
                    BackUpGrid.Children.Add(rowDefinition.RestoreButton);
                    BackUpGrid.Children.Add(rowDefinition.UserCodeLabel);
                    UpdateBackups();
                }
            }
        }

        public void AddFilePath(FilePath path)
        {
            if (!fileRows.Any(x => x.Identity.Id == path.Id))
            {
                var rowDefinition = new FilePathRow(path);
                rowDefinition.DeleteRowButton.Click += (o, i) => filepathHandler.DeleteFilePath(path.Id);
                lock (fileRows)
                {
                    fileRows.Add(rowDefinition);
                    FoldersGrid.Children.Add(rowDefinition.PathLabel);
                    FoldersGrid.Children.Add(rowDefinition.UserCodeLabel);
                    FoldersGrid.Children.Add(rowDefinition.DeleteRowButton);
                    UpdateFilePaths();
                }
            }
        }
        private void UpdateBackups()
        {
            lock (backupRows)
            {
                int rowsInGrid = BackUpGrid.RowDefinitions.Count;
                var first = BackUpGrid.RowDefinitions.First();
                BackUpGrid.RowDefinitions.Clear();
                BackUpGrid.RowDefinitions.Add(first);
                int i = 1;
                foreach (var row in backupRows)
                {
                    BackUpGrid.RowDefinitions.Add(row);
                    row.RowNumber = i;
                    i++;
                }
            }
        }
        private void UpdateFilePaths()
        {
            lock (fileRows)
            {
                int rowsInGrid = FoldersGrid.RowDefinitions.Count;
                var first = FoldersGrid.RowDefinitions.First();
                FoldersGrid.RowDefinitions.Clear();
                FoldersGrid.RowDefinitions.Add(first);  
                int i = 1;
                foreach (var row in fileRows)
                {
                    FoldersGrid.RowDefinitions.Add(row);
                    row.RowNumber = i;
                    i++;
                }
            }
        }

        public void RemoveBackupFile(long id)
        {
            lock (backupRows)
            {
                if (backupRows.Any(x => x.Identity.Id == id))
                {
                    var row = backupRows.First(x => x.Identity.Id == id);
                    int rowNumber = row.RowNumber;
                    BackUpGrid.Children.Remove(row.DeleteRowButton);
                    BackUpGrid.Children.Remove(row.PathLabel);
                    BackUpGrid.Children.Remove(row.DateLabel);
                    BackUpGrid.Children.Remove(row.RestoreButton);
                    BackUpGrid.Children.Remove(row.UserCodeLabel);
                    BackUpGrid.RowDefinitions.RemoveAt(rowNumber);
                    backupRows.Remove(row);
                    UpdateBackups();
                }
            }
        
    }

        public void RemoveFilePath(long id)
        {
            lock (fileRows)
            {
                if (fileRows.Any(x => x.Identity.Id == id))
                {
                    var row = fileRows.First(x => x.Identity.Id == id);
                    int rowNumber = row.RowNumber;
                    FoldersGrid.Children.Remove(row.DeleteRowButton);
                    FoldersGrid.Children.Remove(row.PathLabel);
                    FoldersGrid.Children.Remove(row.UserCodeLabel);
                    FoldersGrid.RowDefinitions.RemoveAt(rowNumber);
                    fileRows.Remove(row);
                    UpdateFilePaths();
                }
            }
        }
    }
}
