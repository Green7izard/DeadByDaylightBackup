using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility;
using System.Windows.Controls;

namespace DeadByDaylightBackup.View
{
    public class FilePathRow : IdentifyableRowDefinition<FilePath>
    {
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
                MaxHeight = IMaxHeight
            };
            DeleteRowButton.SetValue(Grid.ColumnProperty, 3);
            PathLabel = new Label
            {
                Content = Identity.FileName
                ,
                MaxHeight = IMaxHeight
            };
            PathLabel.SetValue(Grid.ColumnProperty, 0);
            UserCodeLabel = new Label
            {
                Content = Identity.UserCode,
                MaxHeight = IMaxHeight
            };
            UserCodeLabel.SetValue(Grid.ColumnProperty, 1);
            SizeLabel = new Label
            {
                Content = FileUtility.GetReadableFileSize(Identity.Path),
                MaxHeight = IMaxHeight
            };
            SizeLabel.SetValue(Grid.ColumnProperty, 2);
            MaxHeight = IMaxHeight;
        }

        protected override void Dispose(bool final)
        {
        }

        protected override void SetRow(int value)
        {
            DeleteRowButton.SetValue(Grid.RowProperty, value);
            PathLabel.SetValue(Grid.RowProperty, value);
            UserCodeLabel.SetValue(Grid.RowProperty, value);
            SizeLabel.SetValue(Grid.RowProperty, value);
        }
    }
}
