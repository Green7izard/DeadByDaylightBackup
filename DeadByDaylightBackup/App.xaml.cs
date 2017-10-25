using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.Program;

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private MainWindow window;

        public void Dispose()
        {
            window.Close();
            window = null;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            FileManager manager = new FileManager();
            FilePathHandler filehandle = new FilePathHandler(manager, new FilePathSettingsManager());
            BackupHandler backuphandle = new BackupHandler(manager, new BackupSettingsManager());
            window = new MainWindow(filehandle, backuphandle);
            window.ShowInTaskbar = true;
            base.OnStartup(e);
            // here you take control

            window.ShowDialog();

        }


    }
}
