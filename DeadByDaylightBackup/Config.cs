using DeadByDaylightBackup.Logging.SimpleFile;
using System;

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Config class to help setup some parts
    /// </summary>
    internal static class Config
    {
        /// <summary>
        /// Set up the logger!
        /// </summary>
        public static void SetUpLogger()
        {
            FileLogger.Install();
        }

        /// <summary>
        /// Add the DLL Helper
        /// </summary>
        public static void AddDllHelper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += DLLHelper.OnResolveAssembly;
        }
    }
}
