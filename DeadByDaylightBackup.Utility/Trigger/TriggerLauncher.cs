using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Logging.Helper;
using System;
using System.Linq;

namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Basic implementation of a trigger launcher
    /// </summary>
    /// <typeparam name="T">type to trigger</typeparam>
    public class TriggerLauncher<T> : ITriggerLauncher<T>
    {
        protected readonly ILogger _logger;
        protected readonly ITriggerHandler<T> _triggerHanlder;

        public TriggerLauncher(ITriggerHandler<T> triggerHandler, ILogger logger)
        {
            _triggerHanlder = triggerHandler;
            _logger = logger;
        }

        /// <summary>
        /// Trigger on the creation
        /// </summary>
        /// <param name="input">Created object</param>
        public void TriggerCreationEvent(T input)
        {
            var list = _triggerHanlder.GetTriggerList();
            foreach (var trigger in list)
            {
                try
                {
                    trigger.CreationTrigger(input);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to trigger creation for: {0}", trigger.ToString());
                }
            }
        }

        /// <summary>
        /// Trigger on the deletion
        /// </summary>
        /// <param name="deleted">Deleted object</param>
        public void TriggerDeletionEvent(T deleted)
        {
            var list = _triggerHanlder.GetTriggerList();
            foreach (var trigger in list)
            {
                try
                {
                    trigger.DeletionTrigger(deleted);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to trigger deletion for: {0}", trigger.ToString());
                }
            }
        }

        /// <summary>
        /// Trigger on the Update of a object
        /// </summary>
        /// <param name="input">Created object</param>
        public void TriggerUpdateEVent(T input)
        {
            var list = _triggerHanlder.GetTriggerList().Where(x => x is IUpdateTrigger<T>)
                .Select(x => x as IUpdateTrigger<T>).ToArray();
            foreach (var trigger in list)
            {
                try
                {
                    trigger.UpdateTrigger(input);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to trigger update for: {0}", trigger.ToString());
                }
            }
        }
    }
}
