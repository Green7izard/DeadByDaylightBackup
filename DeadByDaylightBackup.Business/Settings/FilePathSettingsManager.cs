using DeadByDaylightBackup.Data;
using DeadByDaylightBackup.Utility.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeadByDaylightBackup.Settings
{
    public class FilePathSettingsManager : SettingsManager<FilePath>
    {
        public FilePathSettingsManager() : base("FilePaths")
        { }

        protected override ICollection<FilePath> ConvertFromText(string input)
        {
            var results = SplitPerLine(input).Select(x => new FilePath
            {
                Path = x
            }).ToArray();

            return results;
        }

        protected override string ConvertToText(ICollection<FilePath> input)
        {
            return string.Join(Environment.NewLine, input.Select(x => x.Path).OrderBy(x => x));
        }
    }
}
