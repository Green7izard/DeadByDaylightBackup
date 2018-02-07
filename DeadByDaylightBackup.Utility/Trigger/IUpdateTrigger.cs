namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Interface for classes that can get triggered
    /// Also By Update
    /// </summary>
    /// <typeparam name="T">Trigger value</typeparam>
    public interface IUpdateTrigger<T> : ITrigger<T>
    {
        /// <summary>
        /// Trigger on the Update of a object
        /// </summary>
        /// <param name="input">Created object</param>
        void UpdateTrigger(T input);
    }
}
