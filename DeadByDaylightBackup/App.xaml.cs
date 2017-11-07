using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.Program;
using NLog;

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
        Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            try {
                FileManager manager = new FileManager();
                FilePathHandler filehandle = new FilePathHandler(manager, new FilePathSettingsManager(), LogManager.GetLogger("FileHandler"));
                BackupHandler backuphandle = new BackupHandler(manager, new BackupSettingsManager(), LogManager.GetLogger("BackupHandler"));
                window = new MainWindow(filehandle, backuphandle, LogManager.GetLogger("UI Logger"));
                window.ShowInTaskbar = true;
                base.OnStartup(e);


                window.ShowDialog();
            }
            catch(Exception ex)
            {
                _logger.Fatal(ex, "Fatal error in application occured! {0}", ex.Message);
            }
        }


    }
}
