using DeadByDaylightBackup.Logging;
using System;
using System.Collections.Generic;

namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Class that has both launcher and hanlder
    /// </summary>
    /// <typeparam name="T">type that could get triggered</typeparam>
    public class TriggerManager<T> : ADisposable, ITriggerHandler<T>, ITriggerLauncher<T>
    {
        private readonly ITriggerHandler<T> _handler;
        private readonly ITriggerLauncher<T> _launcher;

        /// <summary>
        /// Create a set of triggerManager
        /// </summary>
        /// <param name="handler">Triggerhandler to user</param>
        /// <param name="launcher">Trigger launcher to use</param>
        public TriggerManager(ITriggerHandler<T> handler, ITriggerLauncher<T> launcher)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            if (launcher == null) throw new ArgumentNullException(nameof(launcher));
            _handler = handler;
            _launcher = launcher;
        }

        /// <summary>
        /// Create a set of triggerManager
        /// </summary>
        /// <param name="handler">Triggerhandler to use</param>
        /// <param name="logger">Logger to use</param>
        public TriggerManager(TriggerHandler<T> triggerHandler, ILogger logger)
            : this(triggerHandler, new TriggerLauncher<T>(triggerHandler, logger))
        { }

        /// <summary>
        /// Create a set of triggerManager
        /// </summary>
        public TriggerManager(ILogger logger, bool shouldDispose = true)
            : this(new TriggerHandler<T>(logger, shouldDispose), logger)
        { }

        /// <summary>
        /// Get the trigger launcher
        /// </summary>
        /// <returns>Launcher</returns>
        public ITriggerLauncher<T> GetTriggerLauncher()
        {
            return _launcher;
        }

        /// <summary>
        /// Get the trigger handler
        /// </summary>
        /// <returns>handler </returns>
        public ITriggerHandler<T> GetTriggerHandler()
        {
            return _handler;
        }

        /// <summary>
        /// Dispose both launcher and handler
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_handler is IDisposable)
                {
                    ((IDisposable)_handler).Dispose();
                }
                if (_launcher is IDisposable)
                {
                    ((IDisposable)_launcher).Dispose();
                }
            }
        }

        #region ITriggerHandler<T>

        public ICollection<ITrigger<T>> GetTriggerList()
        {
            return _handler.GetTriggerList();
        }

        public TriggerRegistration<T> RegisterTrigger(ITrigger<T> trigger)
        {
            return _handler.RegisterTrigger(trigger);
        }

        public void RemoveTrigger(TriggerRegistration<T> triggerRegistration)
        {
            _handler.RemoveTrigger(triggerRegistration);
        }

        #endregion ITriggerHandler<T>

        #region ITriggerLauncher<T>

        public void TriggerCreationEvent(T input)
        {
            _launcher.TriggerCreationEvent(input);
        }

        public void TriggerDeletionEvent(T deleted)
        {
            _launcher.TriggerDeletionEvent(deleted);
        }

        public void TriggerUpdateEvent(T input)
        {
            _launcher.TriggerUpdateEvent(input);
        }

        #endregion ITriggerLauncher<T>
    }
}
