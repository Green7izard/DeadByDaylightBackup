using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility;
using System.IO;
using System.Windows.Controls;

namespace DeadByDaylightBackup.View
{
    /// <summary>
    /// Row to show current file paths
    /// </summary>
    public class FilePathRow : IdentifyableRowDefinition<FilePath>
    {
        private FileSystemWatcher watcher;

        public Button DeleteRowButton
        {
            get;
            private set;
        }

        public Label PathLabel
        {
            get
            ; set;
        }

        public Label SizeLabel
        {
            get
            ; set;
        }

        public Label DateLabel
        {
            get
            ; set;
        }

        public Label UserCodeLabel
        {
            get
            ; set;
        }

        public FilePathRow(FilePath input) : base(input)
        {
            DeleteRowButton = new Button
            {
                Content = "Delete",
                MaxHeight = MaxRowHeight
            };
            DeleteRowButton.SetValue(Grid.ColumnProperty, 4);
            PathLabel = new Label
            {
                Content = Identity.FileName
                ,
                MaxHeight = MaxRowHeight
            };
            PathLabel.SetValue(Grid.ColumnProperty, 0);
            UserCodeLabel = new Label
            {
                Content = Identity.UserCode,
                MaxHeight = MaxRowHeight
            };
            UserCodeLabel.SetValue(Grid.ColumnProperty, 1);
            SizeLabel = new Label
            {
                Content = FileUtility.GetReadableFileSize(Identity.Path),
                MaxHeight = MaxRowHeight
            };
            SizeLabel.SetValue(Grid.ColumnProperty, 3);
            DateLabel = new Label
            {
                Content = FileUtility.GetLastEditDate(Identity.Path).SimpleLongFormat(),
                MaxHeight = MaxRowHeight
            };
            DateLabel.SetValue(Grid.ColumnProperty, 2);
            MaxHeight = MaxRowHeight;
            string path = Path.GetDirectoryName(Identity.Path);
            watcher = new FileSystemWatcher(path, Identity.FileName);
            watcher.Changed += (o, i) =>
            {
                if (FileUtility.FileExists(Identity.Path))
                    Refresh();
            };
        }

        /// <summary>
        /// Refresh information
        /// </summary>
        public void Refresh()
        {
            SizeLabel.Content = FileUtility.GetReadableFileSize(Identity.Path);
            DateLabel.Content = FileUtility.GetLastEditDate(Identity.Path).SimpleLongFormat();
        }

        protected override void Dispose(bool final)
        {
            watcher.Dispose();
        }

        protected override void SetRow(int value)
        {
            DeleteRowButton.SetValue(Grid.RowProperty, value);
            PathLabel.SetValue(Grid.RowProperty, value);
            UserCodeLabel.SetValue(Grid.RowProperty, value);
            SizeLabel.SetValue(Grid.RowProperty, value);
            DateLabel.SetValue(Grid.RowProperty, value);
        }
    }
}
