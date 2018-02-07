using System.Collections.Generic;

namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Interface for classes that handle triggers
    /// </summary>
    /// <typeparam name="T">Type to handle</typeparam>
    public interface ITriggerHandler<T>
    {
        /// <summary>
        /// Gets a list of classes that should be triggered
        /// </summary>
        /// <returns>List of objects to trigger</returns>
        ICollection<ITrigger<T>> GetTriggerList();

        /// <summary>
        /// Register A trigger
        /// </summary>
        /// <param name="trigger">Trigger to register</param>
        /// <returns>a registration for the trigger</returns>
        TriggerRegistration<T> RegisterTrigger(ITrigger<T> trigger);

        /// <summary>
        /// Remove a trigger
        /// </summary>
        /// <param name="triggerRegistration">trigger to remove</param>
        void RemoveTrigger(TriggerRegistration<T> triggerRegistration);
    }
}
