using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DeadByDaylightBackup.Data;
using System.Security.Permissions;

namespace DeadByDaylightBackup.Utility
{
    public class FileManager
    {
        internal void DeleteFile(string filepath)
        {
            var path = Path.GetDirectoryName(filepath);
            File.Delete(filepath);
            var folder = new DirectoryInfo(path);
            var parent = Directory.GetParent(folder.FullName);
            if (folder.Exists && folder.GetFileSystemInfos().Length == 0)
            {
                Directory.Delete(path);
            }
            if(parent.Exists&&parent.GetFileSystemInfos().Length==0)
            {
                Directory.Delete(parent.FullName);
            }
        }

        internal static DateTime GetLastEditDate(string fullFilePath)
        {
            return File.GetLastWriteTime(fullFilePath);
        }

        internal static string GetFileName(string fullFilePath)
        {
            return fullFilePath.Split('\\', '/').Last();
        }

        internal static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        internal static string GetFileWithExtension(string path, string extension)
        {
            path = path.Trim();
            extension = extension.Trim();
            if (!extension.StartsWith(".")) extension = "." + extension;
            if(FileExists(path)&& path.EndsWith(extension,StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }
            if(Directory.Exists(path))
            {
                return Directory.GetFiles(path).Where(x => x.EndsWith(extension, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            else { throw new IOException("Path not found: "+path); }
        }

        internal static string MergePaths(params string[] pathParts)
        {
            return Path.GetFullPath(string.Join("\\", pathParts.Select(x => x.Trim(' ', '\\', '/'))).Replace("\\\\", "\\"));
        }

        internal static void WriteToFile(string settingsFilePath, string result)
        {
           // var directory = Path.GetFullPath(settingsFilePath);
           // Directory.CreateDirectory(directory);
            File.WriteAllText(settingsFilePath, result, Encoding.UTF8);
        }

        internal static void CreateDirectory(string folder)
        {
            Directory.CreateDirectory(folder);
        }

        internal static void Copy(string filepath, string targetFile)
        {
            File.Copy(filepath, targetFile, true);
        }

        internal static string[] GetDrives()
        {
            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            return drives.Where(x => x.DriveType == DriveType.Fixed || x.DriveType == DriveType.Network || x.DriveType == DriveType.Removable|| x.DriveType == DriveType.Ram).Select(x=>x.Name)
                .Where(x=>Directory.Exists(x)).ToArray();

        }

        internal static string[] FullFileSearch(string foldername, string searchTarget, Func<string, long> function)
        {
            var drives = GetDrives();
            List<string> files = new List<string>(3);
            Parallel.ForEach(drives, new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            }, drive=>
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

        internal static ICollection<string> SearchForDirectory(string path, string directoryname)
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
                        lock(results)
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

        private static bool AccessableDirectory(string x)
        {
            if (IsDirectoryBlacklisted(x))
            { return false; }
            else if(x.Count(y=>y.Equals('\\')) > 10)
                {
                return false;
            }
            else
            {
                try
                {
                    var permission = new FileIOPermission(FileIOPermissionAccess.Write, x);
                    permission.Demand();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static bool IsDirectoryBlacklisted(string foldername)
        {
            var name = foldername.Split('/', '\\').Last().Trim();
            if (name.Contains(".")) return true;
            if (name.StartsWith("_", StringComparison.OrdinalIgnoreCase)) return true;
            if (name.EndsWith("Windows", StringComparison.OrdinalIgnoreCase)) return true;
            if(name.EndsWith("AppData", StringComparison.OrdinalIgnoreCase)) return true;
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
