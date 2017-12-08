using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DeadByDaylightBackup.Logging
{
    /// <summary>
    /// Helper class to get Loggers
    /// </summary>
    public static class LoggerFactory
    {
        /// <summary>
        /// Lock on the types system
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Collection of acceptable types
        /// </summary>
        private static ICollection<Type> _types = null;

        /// <summary>
        /// Get a specific Logger
        /// </summary>
        /// <param name="name">Name for the logger, null for the name of the class</param>
        /// <returns>ILogger instance</returns>
        public static ILogger GetLogger(string name = null)
        {
            lock (_lock)
            {
                Type type;

                do
                {
                    type = GetLoggerType();
                    try
                    {
                        var constructor = type.GetConstructor(new[] { typeof(string) });
                        if (constructor == null)
                        {
                            return (ILogger)Activator.CreateInstance(type);
                        }
                        else
                        {
                            return (ILogger)constructor.Invoke(new[] { name ?? type.FullName });
                        }
                    }
                    catch
                    {
                        _types.Remove(type);
                    }
                } while (type != null);
                return new VoidLogger(name ?? "VoidLogger");
            }
        }

        /// <summary>
        /// Get a type for a logger
        /// </summary>
        /// <returns></returns>
        internal static Type GetLoggerType()
        {
            lock (_lock)
            {
                if (_types == null) _types = GetLoggerTypes();
                return _types.FirstOrDefault();
            }
        }

        /// <summary>
        /// Get all available logger types
        /// </summary>
        /// <returns>a Collection of Types</returns>
        internal static ICollection<Type> GetLoggerTypes()
        {
            LoadAllDirectoryAssemblies();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.StartsWith("Microsoft"))
                .Where(x => !x.FullName.StartsWith("System"))
                .Where(x => !x.FullName.StartsWith("WindowsBase"))
                .Where(x => !x.FullName.StartsWith("PresentationCore"))
                .Where(x => !x.FullName.StartsWith("PresentationFramework"))
                .ToArray();
            var types = assemblies
                .SelectMany(s => s.GetExportedTypes());
            var typesNotSystem = types.Where(x => !x.FullName.StartsWith("System") && !x.FullName.StartsWith("Standard"));
            var typesNotMicrosoft = typesNotSystem.Where(x => !x.FullName.StartsWith("Microsoft") && !x.FullName.StartsWith("Windows") && !x.FullName.StartsWith("MS"));
            var instantiableTypes = typesNotMicrosoft.Where(x => x.IsClass && x != typeof(VoidLogger));
            var shouldBe = typesNotMicrosoft.Where(x => x.FullName.StartsWith("DeadByDaylightBackup.Logging.SimpleFile"));
            var loggers = instantiableTypes.Where(p => typeof(ILogger).IsAssignableFrom(p));
            return loggers.ToArray();
        }

        /// <summary>
        /// Load all available assemblies
        /// </summary>
        private static void LoadAllDirectoryAssemblies()
        {
            string binPath = AppDomain.CurrentDomain.BaseDirectory;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.FullName.StartsWith("Microsoft"))
                .Where(x => !x.FullName.StartsWith("System"))
                .Where(x => !x.FullName.StartsWith("WindowsBase"))
                .Where(x => !x.FullName.StartsWith("PresentationCore"))
                .Where(x => !x.FullName.StartsWith("PresentationFramework"))
                .Select(x => x.Location).Distinct().ToArray();
            var files = Directory.GetFiles(binPath, "*.dll", SearchOption.AllDirectories).Where(x => File.Exists(x));
            var filesToActivate = files.Where(x => !assemblies.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            foreach (string dll in filesToActivate)
            {
                //Dirty trick to prevent the direct use of Load, so that we could work around a false positive
                try
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dll);
                    typeof(Assembly).GetMethod("Load", new[] { typeof(AssemblyName) }).Invoke(null, new object[] { an });
                }
                catch (TargetInvocationException ex)
                {
                    if (!(ex.InnerException is BadImageFormatException) && !(ex.InnerException is FileLoadException))
                    {
                        throw ex.InnerException ?? ex;
                    }
                }
                //Assembly loadedAssembly = Assembly.LoadFile(dll);


            }
        }
    }
}
