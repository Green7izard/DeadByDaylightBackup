using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Logging.SimpleFile;
using DeadByDaylightBackup.Utility.Trigger;

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Config class to help setup some parts
    /// </summary>
    internal static class Config
    {
        /// <summary>
        /// Set up the logger!
        /// </summary>
        public static void SetUpLogger()
        {
            FileLogger.Install();
        }

        #region trigger

        #region FilePath

        /// <summary>
        /// Get a set of triggers for the filepath
        /// </summary>
        /// <returns>FilePathManager</returns>
        public static TriggerManager<FilePath> GetFilePathTrigger()
        {
            ITriggerHandler<FilePath> handler = GetFilePathHandler(LoggerFactory.GetLogger("FilePathHandler"));
            ITriggerLauncher<FilePath> launcher = GetFilePathHandler(LoggerFactory.GetLogger("FilePathLauncher"), handler);
            return new TriggerManager<FilePath>(handler, launcher);
        }

        private static ITriggerHandler<FilePath> GetFilePathHandler(ILogger logger)
        {
            return new TriggerHandler<FilePath>(logger, true);
        }

        private static ITriggerLauncher<FilePath> GetFilePathHandler(ILogger logger, ITriggerHandler<FilePath> handler)
        {
            return new TriggerLauncher<FilePath>(handler, logger);
        }

        #endregion FilePath

        #region backup

        public static TriggerManager<Backup> GetBackupTrigger()
        {
            ITriggerHandler<Backup> handler = GetBackupHandler(LoggerFactory.GetLogger("BackupHandler"));
            ITriggerLauncher<Backup> launcher = GetBackupHandler(LoggerFactory.GetLogger("BackupLauncher"), handler);
            return new TriggerManager<Backup>(handler, launcher);
        }

        private static ITriggerHandler<Backup> GetBackupHandler(ILogger logger)
        {
            return new TriggerHandler<Backup>(logger, true);
        }

        private static ITriggerLauncher<Backup> GetBackupHandler(ILogger logger, ITriggerHandler<Backup> handler)
        {
            return new TriggerLauncher<Backup>(handler, logger);
        }

        #endregion backup

        #endregion trigger
    }
}
