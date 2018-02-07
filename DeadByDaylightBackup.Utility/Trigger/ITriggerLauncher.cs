namespace DeadByDaylightBackup.Utility.Trigger
{
    public interface ITriggerLauncher<T>
    {
        /// <summary>
        /// Trigger on the creation
        /// </summary>
        /// <param name="input">Created object</param>
        void TriggerCreationEvent(T input);

        /// <summary>
        /// Trigger on the deletion
        /// </summary>
        /// <param name="deleted">Deleted object</param>
        void TriggerDeletionEvent(T deleted);

        /// <summary>
        /// Trigger on the Update of a object
        /// </summary>
        /// <param name="input">Created object</param>
        void TriggerUpdateEvent(T input);
    }
}
