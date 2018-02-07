using System.Collections.Generic;

namespace DeadByDaylightBackup.Utility.Settings
{
    /// <summary>
    /// Interface for classes that can get and set Settings
    /// </summary>
    /// <typeparam name="T">Type to set</typeparam>
    public interface ISettingsManager<T> : ISettingsGetter<T> where T : class
    {
        /// <summary>
        /// Save the settings
        /// </summary>
        /// <param name="input">settings to save</param>
        void SaveSettings(ICollection<T> input);
    }
}
