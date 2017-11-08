﻿using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility;
using System.Windows.Controls;

namespace DeadByDaylightBackup.View
{
    public class BackUpRow : IdentifyableRowDefinition<Backup>
    {
        public Button DeleteRowButton
        {
            get;
            private set;
        }

        public Button RestoreButton
        {
            get;
            private set;
        }

        public Label DateLabel
        {
            get
            ; set;
        }

        public Label SizeLabel
        {
            get
            ; set;
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

        public BackUpRow(Backup input) : base(input)
        {
            UserCodeLabel = new Label
            {
                Content = Identity.UserCode,
                MaxHeight = IMaxHeight
            };
            UserCodeLabel.SetValue(Grid.ColumnProperty, 1);
            DeleteRowButton = new Button
            {
                Content = "Delete",
                MaxHeight = IMaxHeight
            };
            DeleteRowButton.SetValue(Grid.ColumnProperty, 4);
            RestoreButton = new Button
            {
                Content = "Restore",
                MaxHeight = IMaxHeight
            };
            RestoreButton.SetValue(Grid.ColumnProperty, 5);
            PathLabel = new Label
            {
                Content = Identity.FileName
                ,
                MaxHeight = IMaxHeight
            };
            PathLabel.SetValue(Grid.ColumnProperty, 0);
            DateLabel = new Label
            {
                Content = Identity.Date.GetValueOrDefault().SimpleLongFormat()
                ,
                MaxHeight = IMaxHeight
            };
            DateLabel.SetValue(Grid.ColumnProperty, 2);
            SizeLabel = new Label
            {
                Content = FileUtility.GetReadableFileSize(Identity.FullFileName)
               ,
                MaxHeight = IMaxHeight
            };
            SizeLabel.SetValue(Grid.ColumnProperty, 3);
            MaxHeight = IMaxHeight;
        }

        protected override void Dispose(bool final)
        {
        }

        protected override void SetRow(int value)
        {
            DateLabel.SetValue(Grid.RowProperty, value);
            DeleteRowButton.SetValue(Grid.RowProperty, value);
            RestoreButton.SetValue(Grid.RowProperty, value);
            PathLabel.SetValue(Grid.RowProperty, value);
            UserCodeLabel.SetValue(Grid.RowProperty, value);
            SizeLabel.SetValue(Grid.RowProperty, value);
        }
    }
}
