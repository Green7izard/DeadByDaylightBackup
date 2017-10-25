using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeadByDaylightBackup.Utility
{
    public static class DateUtility
    {
        public const string DateFormat = "yyyyMMddhhmm";

        public static string SimpleFormat(this DateTime? time)
        {
            return SimpleFormat(time.GetValueOrDefault(DateTime.Now));
        }

        public static string SimpleFormat(this DateTime time)
        {
            return time.ToString(DateFormat);
        }

        public static DateTime ToSimpleDate(this string timestamp)
        {
            if(string.IsNullOrWhiteSpace(timestamp))
            {
                throw new NullReferenceException("Input of ToSimpleDate cannot be null or a empty string");
            }
            return DateTime.ParseExact(timestamp.Trim(' ', '\t','\r', '\n','.',','), DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
