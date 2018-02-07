using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Program;
using DeadByDaylightBackup.Settings;
using DeadByDaylightBackup.Utility.Trigger;
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

        public App()
        {
            //Config.AddDllHelper();
            Config.SetUpLogger();
            _logger = LoggerFactory.GetLogger("Main App");
            _FilePathTriggerManager = Config.GetFilePathTrigger();
            _BackupTriggerManager = Config.GetBackupTrigger();
        }

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Trigger manager for the FIlepath
        /// </summary>
        private readonly TriggerManager<FilePath> _FilePathTriggerManager;

        /// <summary>
        /// Trigger manager for backups
        /// </summary>
        private readonly TriggerManager<Backup> _BackupTriggerManager;

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
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overidable dispose function
        /// </summary>
        /// <param name="final"></param>
        protected virtual void Dispose(bool final)
        {
            if (final)
            {
                _window.Close();
                _window = null;
                _BackupTriggerManager.Dispose();
                _FilePathTriggerManager.Dispose();
            }
        }

        #endregion IDisposable

        /// <summary>
        /// Startup functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            _logger.Log(LogLevel.Debug, $"Starting DeadByDaylightBackup version {VersionInformation.Business}");
            try
            {
                int savesToKeep = int.Parse(ConfigurationManager.AppSettings["SavesToKeep"]);

                //Filehandler setup
                FilePathHandler filehandle = new FilePathHandler(_FilePathTriggerManager.GetTriggerLauncher(),
                    new FilePathSettingsManager(), LoggerFactory.GetLogger("FileHandler"));
                BackupHandler backuphandle = new BackupHandler(_BackupTriggerManager.GetTriggerLauncher(),
                    savesToKeep, new BackupSettingsManager(), LoggerFactory.GetLogger("BackupHandler"));

                _window = new MainWindow(filehandle, backuphandle, LoggerFactory.GetLogger("UserInterface"));
                _FilePathTriggerManager.RegisterTrigger(_window);
                _BackupTriggerManager.RegisterTrigger(_window);
                _window.ShowInTaskbar = true;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Fatal, ex, "Fatal error in set up occured! {0}", ex.Message);
                throw;
            }
            base.OnStartup(e);
            ExectuteApplication(_window);
            _logger.Log(LogLevel.Debug, $"Stopping DeadByDaylightBackup version {VersionInformation.Business}");
        }

        /// <summary>
        /// Execute the application
        /// </summary>
        /// <param name="window">The maind window to execute</param>
        private void ExectuteApplication(MainWindow window)
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
