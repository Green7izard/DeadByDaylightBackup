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
namespace DeadByDaylightBackup.View
{
    public class FilePathRow: IdentifyableRowDefinition<FilePath>
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
        public Label UserCodeLabel
        {
            get
            ; set;
        }

        public FilePathRow(FilePath input): base(input)
        {
            DeleteRowButton = new Button
            {
                Content = "Delete",
                MaxHeight = IMaxHeight
            };
            DeleteRowButton.SetValue(Grid.ColumnProperty, 2);
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
        }
    }
}
