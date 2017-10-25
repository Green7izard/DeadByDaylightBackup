using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Data
{
    [Serializable]
    public abstract class Identifyable
    {
        public long Id { get; set; }
    }
}
