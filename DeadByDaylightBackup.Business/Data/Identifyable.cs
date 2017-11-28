using System;

namespace DeadByDaylightBackup.Data
{
    [Serializable]
    public abstract class Identifyable
    {
        public long Id { get; set; }
    }
}
