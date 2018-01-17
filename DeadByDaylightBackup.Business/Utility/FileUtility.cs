using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Utility
{
    /// <summary>
    /// Utility class for files
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="filepath">file to delete</param>
        public static void DeleteFile(string filepath)
        {
            var path = Path.GetDirectoryName(filepath);
            File.Delete(filepath);
            var folder = new DirectoryInfo(path);
            var parent = Directory.GetParent(folder.FullName);
            if (folder.Exists && folder.GetFileSystemInfos().Length == 0)
            {
                Directory.Delete(path);
            }
            if (parent.Exists && parent.GetFileSystemInfos().Length == 0)
            {
                try {
                    Directory.Delete(parent.FullName);
                }
                catch
                {
                    //Dont break on cleaning a directory
                }
            }
        }

        /// <summary>
        /// Get the last time the file was edited
        /// </summary>
        /// <param name="fullFilePath">The filename</param>
        /// <returns>DateTime</returns>
        public static DateTime GetLastEditDate(string fullFilePath)
        {
            return File.GetLastWriteTime(fullFilePath);
        }

        /// <summary>
        /// Get the name of a file
        /// </summary>
        /// <param name="fullFilePath">filepath of the file</param>
        /// <returns>filename</returns>
        public static string GetFileName(string fullFilePath)
        {
            return fullFilePath.Split('\\', '/').Last();
        }

        /// <summary>
        /// Check if file exits
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <returns>true if it exists</returns>
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// In a path, get the file with the specified extesion
        /// </summary>
        /// <param name="path">Path to search</param>
        /// <param name="extension">desired extension</param>
        /// <returns>Filepath</returns>
        public static string GetFileWithExtension(string path, string extension)
        {
            path = path.Trim();
            extension = extension.Trim();
            if (!extension.StartsWith(".")) extension = "." + extension;
            if (FileExists(path) && path.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
            if (Directory.Exists(path))
            {
                return Directory.GetFiles(path).Where(x => x.EndsWith(extension, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            else { throw new IOException("Path not found: " + path); }
        }

        /// <summary>
        /// Combine multiple parts into one path
        /// </summary>
        /// <param name="pathParts">parts of the path</param>
        /// <returns>combined path</returns>
        public static string MergePaths(params string[] pathParts)
        {
            return Path.GetFullPath(string.Join("\\", pathParts.Select(x => x.Trim(' ', '\\', '/'))).Replace("\\\\", "\\"));
        }

        /// <summary>
        /// Write string data to a file
        /// </summary>
        /// <param name="filePath">the filepath where the data needs to go</param>
        /// <param name="content">The content for the file</param>
        public static void WriteToFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content, Encoding.UTF8);
        }

        /// <summary>
        /// Create the specified directory
        /// </summary>
        /// <param name="folder">the folder to create</param>
        public static void CreateDirectory(string folder)
        {
            Directory.CreateDirectory(folder);
            DirectorySecurity sec = Directory.GetAccessControl(folder);
            // Using this instead of the "Everyone" string means we work on non-English systems.
            SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(folder, sec);
        }

        /// <summary>
        /// Get the size of a file
        /// </summary>
        /// <param name="fileName">the fileName</param>
        /// <returns></returns>
        public static long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length;
        }

        /// <summary>
        /// Get the size of a file in readable style
        /// </summary>
        /// <param name="fileName">filepath</param>
        /// <returns>Readable string like "2.96 KB"</returns>
        public static string GetReadableFileSize(string fileName)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = GetFileSize(fileName);
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            string result = String.Format("{0:0.##} {1}", len, sizes[order]);
            return result;
        }

        /// <summary>
        /// Copy filepath to targetfile
        /// </summary>
        /// <param name="filepath">filepath of the desired file</param>
        /// <param name="targetFile">filepath of the desired new location</param>
        public static void Copy(string filepath, string targetFile)
        {
            File.Copy(filepath, targetFile, true);
        }

        /// <summary>
        /// Get a list of drives that can be searched
        /// </summary>
        /// <returns>Array of drives</returns>
        public static string[] GetDrives()
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            return drives.Where(x => x.DriveType == DriveType.Fixed || x.DriveType == DriveType.Network || x.DriveType == DriveType.Removable || x.DriveType == DriveType.Ram).Select(x => x.Name)
                .Where(x => Directory.Exists(x)).ToArray();
        }

        /// <summary>
        /// Search all drives for the specified folder name, and the searchtarget in it.
        /// </summary>
        /// <param name="foldername">Name of the folder, no wildcards</param>
        /// <param name="searchTarget">desired file, wildcards allowed</param>
        /// <returns>array of filepaths</returns>
        public static string[] FullFileSearch(string foldername, string searchTarget)
        {
            var drives = GetDrives();
            List<string> files = new List<string>(3);
            Parallel.ForEach(drives, new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            }, drive =>
                {
                    var topLevelDirectories = Directory.GetDirectories(drive).Where(x => AccessableDirectory(x)).ToArray();
                    Parallel.ForEach(topLevelDirectories, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = 2
                    }, dir =>
                    {
                        try
                        {
                            var directories = SearchForDirectory(dir, foldername);
                            Parallel.ForEach(directories, directory =>
                             {
                                 var result = Directory.GetFiles(directory, searchTarget.Trim(), SearchOption.AllDirectories);
                                 if (result.Any())
                                 {
                                     lock (files)
                                     { files.AddRange(result); }
                                 }
                             });
                        }
                        catch
                        {
                        }
                    });
                });
            return files.ToArray();
        }

        /// <summary>
        /// Search for a specific directory
        /// Recursive operation
        /// </summary>
        /// <param name="path">The path that needs to be searched for the folder</param>
        /// <param name="directoryname">the foldername to find</param>
        /// <returns>Collection of strings</returns>
        public static ICollection<string> SearchForDirectory(string path, string directoryname)
        {
            var info = new DirectoryInfo(path);
            List<string> results = new List<string>(3);
            try
            {
                var directories = info.GetDirectories().Where(x => AccessableDirectory(x.FullName));
                Parallel.ForEach(directories, directory =>
                 {
                     if (directory.Name.Equals(directoryname, StringComparison.OrdinalIgnoreCase))
                     {
                         lock (results)
                         results.Add(directory.FullName);
                     }
                     else
                     {
                         try
                         {
                             var result = SearchForDirectory(directory.FullName, directoryname);
                             if (result.Any())
                                 lock (results) results.AddRange(result);
                         }
                         catch
                         {
                             //ERROR
                         }
                     }
                 });
            }
            catch { }
            return results;
        }

        /// <summary>
        /// Check if its possible to acces (read/write) to a folder
        /// </summary>
        /// <param name="directory">the directory to search</param>
        /// <returns>true if you can acces is</returns>
        private static bool AccessableDirectory(string directory)
        {
            if (IsDirectoryBlacklisted(directory))
            { return false; }
            else if (directory.Count(y => y.Equals('\\')) > 10)
            {
                return false;
            }
            else
            {
                try
                {
                    var permission = new FileIOPermission(FileIOPermissionAccess.Write, directory);
                    permission.Demand();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Crude way to see if you may access a folder
        /// </summary>
        /// <param name="foldername">the folder to find</param>
        /// <returns>true if its blacklisted, false if you may acces it</returns>
        private static bool IsDirectoryBlacklisted(string foldername)
        {
            var name = foldername.Split('/', '\\').Last().Trim();
            if (name.Contains(".")) return true;
            if (name.StartsWith("_", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("Windows", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("AppData", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("ProgramData", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("Boot", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("Documents and Setting", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("K - Lite Codec Pack", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("config.msi", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("System Volume Information", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("df204f15c26da88c2b905180fb27acf7", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("vrpanorama", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("steamapps", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("dota2launcher", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("controller_base", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("bin", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("drivers", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("frameworks", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("(1)", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("(2)", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("(3)", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("tenfoot", StringComparison.OrdinalIgnoreCase)) return true;
            return false;
        }
    }
}
