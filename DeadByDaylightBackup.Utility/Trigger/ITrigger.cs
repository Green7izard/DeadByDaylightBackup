namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Interface for classes that can get triggered
    /// </summary>
    /// <typeparam name="T">Trigger value</typeparam>
    public interface ITrigger<T>
    {
        /// <summary>
        /// Trigger on the creation
        /// </summary>
        /// <param name="input">Created object</param>
        void CreationTrigger(T input);

        /// <summary>
        /// Trigger on the deletion
        /// </summary>
        /// <param name="deleted">Deleted object</param>
        void DeletionTrigger(T deleted);
    }
}
