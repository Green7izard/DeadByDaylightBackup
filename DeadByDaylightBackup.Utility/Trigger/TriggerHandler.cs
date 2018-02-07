using DeadByDaylightBackup.Logging;
using DeadByDaylightBackup.Logging.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Basic implementation of the TriggerHandler
    /// </summary>
    /// <typeparam name="T">Type to trigger with</typeparam>
    public class TriggerHandler<T> : ADisposable, ITriggerHandler<T>
    {
        /// <summary>
        /// List of triggers
        /// </summary>
        protected readonly List<TriggerRegistration<T>> _triggerList;

        /// <summary>
        /// true if all triggers should be disposed when unregistered
        /// </summary>
        protected readonly bool _disposeOnUnregister;

        /// <summary>
        /// Logger
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Create the handler of triggers
        /// </summary>
        /// <param name="logger">Logger to log with</param>
        /// <param name="disposeOnUnregister">true if all triggers should be disposed when unregistered</param>
        public TriggerHandler(ILogger logger, bool disposeOnUnregister = true)
        {
            _triggerList = new List<TriggerRegistration<T>>();
            _disposeOnUnregister = disposeOnUnregister;
            _logger = logger;
        }

        /// <summary>
        /// Gets a list of classes that should be triggered
        /// </summary>
        /// <returns>List of objects to trigger</returns>
        public virtual ICollection<ITrigger<T>> GetTriggerList()
        {
            lock (_triggerList)
            {
                return _triggerList.Select(x => x.Target).Distinct().ToArray();
            }
        }

        /// <summary>
        /// Register A trigger
        /// </summary>
        /// <param name="trigger">Trigger to register</param>
        /// <returns>a registration for the trigger</returns>
        public virtual TriggerRegistration<T> RegisterTrigger(ITrigger<T> trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }
            else {
                lock (_triggerList)
                {
                    TriggerRegistration<T> regi = new TriggerRegistration<T>(this, trigger, _disposeOnUnregister);
                    _triggerList.Add(regi);
                    _logger.Trace("Added a trigger for type {0}. Implemented by: {1}", typeof(T), trigger.GetType());
                    return regi;
                }
            }
        }

        /// <summary>
        /// Remove a trigger
        /// </summary>
        /// <param name="triggerRegistration">trigger to remove</param>
        public virtual void RemoveTrigger(TriggerRegistration<T> triggerRegistration)
        {
            if (triggerRegistration == null)
            {
                throw new ArgumentNullException(nameof(triggerRegistration));
            }
            if (!Equals(triggerRegistration.Creator))
            {
                throw new ArgumentException("Cannot Remove triggers that are not created by this Instance");
            }
            else {
                lock (_triggerList)
                {
                    if (_triggerList.Any(x => x.Creator.Equals(this) && x.Target.Equals(triggerRegistration.Target) && x.Equals(triggerRegistration)))
                    {
                        _triggerList.Remove(triggerRegistration);
                        _logger.Trace("Removed a trigger for type {0}. Implemented by: {1}", typeof(T), triggerRegistration.Target.GetType());
                    }
                }
            }
        }

        /// <summary>
        /// Clearup all remaining triggers
        /// </summary>
        /// <param name="disposing">true if should dispose</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var trigger in _triggerList)
                {
                    try
                    {
                        trigger?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Could not dispose of {0}", trigger?.GetType()?.ToString() ?? "Null type");
                    }
                }
                _triggerList.Clear();
            }
        }
    }
}
