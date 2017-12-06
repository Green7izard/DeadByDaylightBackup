using System.Reflection;
using System.Resources;

[assembly: AssemblyTitle(DeadByDaylightBackup.AssemblyInformation.AssemblyTitle)]
[assembly: AssemblyDescription(DeadByDaylightBackup.AssemblyInformation.AssemblyDescription)]
[assembly: AssemblyConfiguration(DeadByDaylightBackup.AssemblyInformation.AssemblyConfiguration)]
[assembly: AssemblyCompany(DeadByDaylightBackup.AssemblyInformation.AssemblyCompany)]
[assembly: AssemblyProduct(DeadByDaylightBackup.AssemblyInformation.AssemblyProduct)]
[assembly: AssemblyCopyright(DeadByDaylightBackup.AssemblyInformation.AssemblyCopyright)]
[assembly: NeutralResourcesLanguage(DeadByDaylightBackup.AssemblyInformation.NeutralResourcesLanguage)]

namespace DeadByDaylightBackup
{
    /// <summary>
    /// Holds about the shared information
    /// </summary>
    public static class AssemblyInformation
    {
        public const string AssemblyTitle = "DeadByDaylightBackup";
        public const string AssemblyDescription = "BackUp tool for dead by daylight";
        public const string AssemblyConfiguration = "Retail";
        public const string AssemblyCompany = "Bas van Summeren";
        public const string AssemblyProduct = "DeadByDaylightBackup";
        public const string AssemblyCopyright = "Copyright Bas van Summeren©  2017";
        public const string NeutralResourcesLanguage = "en";
    }


    public static class VersionInformation
    {
        public const string LoggingInterface = "1.0.0.1";
        public const string Business = "1.0.2.0";
        public const string WPFapplication = "1.0.2.0";
    }
}
