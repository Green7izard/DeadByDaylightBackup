using System.Configuration;

namespace DeadByDaylightBackup.Logging.SimpleFile
{
    /// <summary>
    /// Helper class for the configurationfile
    /// </summary>
    internal static class ConfigHelper
    {
        /// <summary>
        /// Get or Set a config key
        /// </summary>
        /// <param name="key">the key to get or set</param>
        /// <param name="defaultValue">the value to set it to if it does not exists</param>
        /// <returns>The value</returns>
        internal static string GetOrCreateSetting(string key, string defaultValue)
        {
            Configuration configuration =
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (configuration.AppSettings.Settings[key] == null || string.IsNullOrWhiteSpace(configuration.AppSettings.Settings[key].Value))
            {
                configuration.AppSettings.Settings[key].Value = defaultValue;
                configuration.Save(ConfigurationSaveMode.Modified, true);
                ConfigurationManager.RefreshSection("appSettings");
                return defaultValue;
            }
            else
            {
                ConfigurationManager.RefreshSection("appSettings");
                return configuration.AppSettings.Settings[key].Value;
            }
        }


    }
}
