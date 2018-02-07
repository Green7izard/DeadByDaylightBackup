using System.Collections.Generic;

namespace DeadByDaylightBackup.Utility.Settings
{
    /// <summary>
    /// Interface for classes that can get settings
    /// </summary>
    /// <typeparam name="T">Type to get</typeparam>
    public interface ISettingsGetter<T> where T : class
    {
        /// <summary>
        /// Get the settings
        /// </summary>
        /// <returns>List of Settings</returns>
        ICollection<T> GetSettings();
    }
}
