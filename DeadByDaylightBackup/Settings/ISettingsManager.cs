using System.Collections.Generic;

namespace DeadByDaylightBackup.Settings
{
    public interface ISettingsManager<T> where T : class
    {
        /// <summary>
        /// Get the settings
        /// </summary>
        /// <returns>List of Settings</returns>
        ICollection<T> GetSettings();

        /// <summary>
        /// Save the settings
        /// </summary>
        /// <param name="input">settings to save</param>
        void SaveSettings(ICollection<T> input);
    }
}