using System;

namespace DeadByDaylightBackup.Utility
{
    /// <summary>
    /// Abstract implementation of IDispoable
    /// </summary>
    public abstract class ADisposable : IDisposable
    {
        /// <summary>
        /// Create the Disposable
        /// </summary>
        protected ADisposable()
        {
            disposedValue = false;
        }

        /// <summary>
        /// Protetection against double disposing
        /// </summary>
        private bool disposedValue;

        /// <summary>
        /// Overrideable dispose
        /// </summary>
        /// <param name="disposing">true if it is the final</param>
        protected abstract void Dispose(bool disposing);

        /// <summary>
        /// Dispose the object
        /// </summary>
        public void Dispose()
        {
            Dispose(!disposedValue);
            disposedValue = true;
            GC.SuppressFinalize(this);
        }
    }
}
