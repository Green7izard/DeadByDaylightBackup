using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Logging
{
    /// <summary>
    /// Helper class to get Loggers
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Get all available logger types
        /// </summary>
        /// <returns>a Collection of Types</returns>
        internal static ICollection<Type> GetLoggerTypes()
        {
            var type = typeof(ILogger);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && type.IsClass && type != typeof(VoidLogger));
            return types.ToArray();
        }

        /// <summary>
        /// Get a specific Logger
        /// </summary>
        /// <param name="name">Name for the logger, null for the name of the class</param>
        /// <returns>ILogger instance</returns>
        public static ILogger GetLogger(string name=null)
        {
            var types = GetLoggerTypes();
            if(types == null && !types.Any())
            {
                return new VoidLogger(name ?? "VoidLogger");
            }
            var type = types.First();
            var constructor = type.GetConstructor(new[] { typeof(string) });
            if(constructor== null)
            {
                return (ILogger)Activator.CreateInstance(type);
            }
            else
            {
                return (ILogger)constructor.Invoke(new[] { name ?? type.FullName });
            }
        }



    }
}
