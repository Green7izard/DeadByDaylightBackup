using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DeadByDaylightBackup.Utility;
using DeadByDaylightBackup.Program;
using DeadByDaylightBackup.View;
using NLog;
using DeadByDaylightBackup.Settings;

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        #region privates
        /// <summary>
        /// Logger
        /// </summary>
        Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The window to keep track off. Disposed when the app is disposed
        /// </summary>
        private MainWindow _window;
        #endregion
        #region IDisposable

        /// <summary>
        /// Dispose the application
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
          
        }

        /// <summary>
        /// Overidable dispose function
        /// </summary>
        /// <param name="final"></param>
        protected virtual void Dispose(bool final)
        {
            _window.Close();
            _window = null;
        }
        #endregion

        /// <summary>
        /// Startup functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //Manage dependencies
                FileManager manager = new FileManager();
                FilePathHandler filehandle = new FilePathHandler(manager, new FilePathSettingsManager(), LogManager.GetLogger("FileHandler"));
                BackupHandler backuphandle = new BackupHandler(manager, new BackupSettingsManager(), LogManager.GetLogger("BackupHandler"));
                _window = new MainWindow(filehandle, backuphandle, LogManager.GetLogger("UserInterface"));
                _window.ShowInTaskbar = true;
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Fatal error in set up occured! {0}", ex.Message);
                throw;
            }
            base.OnStartup(e);
            ExectuteApplication(_window);
        }

        /// <summary>
        /// Execute the application
        /// </summary>
        /// <param name="window">The maind window to execute</param>
        private void ExectuteApplication(Window window)
        {
            try
            {
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Fatal error in application occured! {0}", ex.Message);
                throw;
            }
        }

    }
}
