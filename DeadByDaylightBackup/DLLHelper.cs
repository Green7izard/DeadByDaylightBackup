using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Helper for managing dlls
    /// </summary>
    public static class DLLHelper
    {

        /// <summary>
        /// Adds all available dlls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null) return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }

        /// <summary>
        /// Load all available assemblies
        /// </summary>
        internal static void LoadAllDirectoryAssemblies()
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
