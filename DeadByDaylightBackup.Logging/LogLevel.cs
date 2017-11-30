namespace DeadByDaylightBackup.Logging
{
    /// <summary>
    /// Defines loglevels
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Lowest level
        /// </summary>
        Trace = 5,

        /// <summary>
        /// Debug for testing
        /// </summary>
        Debug = 4,

        /// <summary>
        /// Information message
        /// </summary>
        Info = 3,

        /// <summary>
        /// Small error
        /// </summary>
        Warn = 2,

        /// <summary>
        /// Large error
        /// </summary>
        Error = 1,

        /// <summary>
        /// Critical error system cannot continue
        /// </summary>
        Fatal = 0
    }
}
