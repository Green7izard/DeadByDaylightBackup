using System;

namespace DeadByDaylightBackup.Utility.Trigger
{
    /// <summary>
    /// Class that shows that
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TriggerRegistration<T> : ADisposable
    {
        /// <summary>
        /// Create a registration of a trigger
        /// </summary>
        /// <param name="creator">The creator of the trigger</param>
        /// <param name="target">The target of the trigger</param>
        public TriggerRegistration(ITriggerHandler<T> creator, ITrigger<T> target, bool disposeTrigger = false)
        {
            if (creator == null)
            {
                throw new ArgumentException(nameof(creator));
            }
            else
            {
                Creator = creator;
            }
            if (target == null)
            {
                throw new ArgumentException(nameof(target));
            }
            else
            {
                Target = target;
            }
            DisposeTrigger = disposeTrigger;
        }

        /// <summary>
        /// Whether the trigger will be disposed on unregister (If possible)
        /// </summary>
        private bool DisposeTrigger
        {
            get; set;
        }

        /// <summary>
        /// Creator of this object
        /// </summary>
        public ITriggerHandler<T> Creator
        {
            get;
        }

        /// <summary>
        /// The target of the trigger
        /// </summary>
        public ITrigger<T> Target
        {
            get;
        }

        /// <summary>
        /// Remove the trigger if it is disposed
        /// </summary>
        /// <param name="disposing">true if diposing</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Creator.RemoveTrigger(this);
                if (DisposeTrigger && Target is IDisposable)
                {
                    ((IDisposable)Target).Dispose();
                }
            }
        }
    }
}
