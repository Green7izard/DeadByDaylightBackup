using DeadByDaylightBackup.Utility;
using System;

namespace DeadByDaylightBackup.Data
{
    [Serializable]
    public class FilePath : Identifyable
    {
        public string Path { get; set; }

        public string FileName
        {
            get
            {
                return FileManager.GetFileName(Path);
            }
        }

        public string UserCode
        {
            get
            {
                var st = Path.Split('\\');
                bool found = false;
                for (int i = st.Length - 1; i >= 0; i--)
                {
                    if (found)
                    {
                        return st[i];
                    }

                    found = st[i].Equals("381210");
                }
                return null;
            }
        }

        public DateTime LastEdited
        {
            get
            {
                return FileManager.GetLastEditDate(Path);
            }
        }
    }
}
