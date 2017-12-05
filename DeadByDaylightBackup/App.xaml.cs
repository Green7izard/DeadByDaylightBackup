using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Program;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.View;
using System;
using System.Configuration;
using System.Windows;

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
        private ILogger _logger = LoggerFactory.GetLogger("Main App");

        /// <summary>
        /// The window to keep track off. Disposed when the app is disposed
        /// </summary>
        private MainWindow _window;

        #endregion privates

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
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        /// <summary>
        /// Startup functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                int savesToKeep = int.Parse(ConfigurationManager.AppSettings["SavesToKeep"]);

                //Manage dependencies
                // FileUtility manager = new FileManager();
                FilePathHandler filehandle = new FilePathHandler(//manager,
                    new FilePathSettingsManager(), LoggerFactory.GetLogger("FileHandler"));
                BackupHandler backuphandle = new BackupHandler(savesToKeep,//manager,
                    new BackupSettingsManager(), LoggerFactory.GetLogger("BackupHandler"));
                _window = new MainWindow(filehandle, backuphandle, LoggerFactory.GetLogger("UserInterface"));
                _window.ShowInTaskbar = true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal,ex, "Fatal error in set up occured! {0}", ex.Message);
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
                _logger.Log(LogLevel.Fatal, ex, "Fatal error in application occured! {0}", ex.Message);
                throw;
            }
        }
    }
}
