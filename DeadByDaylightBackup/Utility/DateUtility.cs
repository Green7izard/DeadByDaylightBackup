using System;
using System.Globalization;

namespace DeadByDaylightBackup.Utility
{
    public static class DateUtility
    {
        #region shortdate

        public const string ShortDateFormat = "yyyyMMddHHmm";

        public static string SimpleShortFormat(this DateTime? time)
        {
            return SimpleShortFormat(time.GetValueOrDefault(DateTime.Now));
        }

        public static string SimpleShortFormat(this DateTime time)
        {
            return time.ToString(ShortDateFormat);
        }

        public static DateTime ToSimpleShortDate(this string timestamp)
        {
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                throw new NullReferenceException("Input of ToSimpleShortDate cannot be null or a empty string");
            }
            return DateTime.ParseExact(timestamp.Trim(' ', '\t', '\r', '\n', '.', ','), ShortDateFormat, CultureInfo.InvariantCulture);
        }

        #endregion shortdate

        #region longdate

        public const string LongDateFormat = "yyyy-MM-dd HH:mm";

        public static string SimpleLongFormat(this DateTime? time)
        {
            return SimpleLongFormat(time.GetValueOrDefault(DateTime.Now));
        }

        public static string SimpleLongFormat(this DateTime time)
        {
            return time.ToString(LongDateFormat);
        }

        public static DateTime ToSimpleLongDate(this string timestamp)
        {
            if (string.IsNullOrWhiteSpace(timestamp))
            {
                throw new NullReferenceException("Input of ToSimpleShortDate cannot be null or a empty string");
            }
            return DateTime.ParseExact(timestamp.Trim(' ', '\t', '\r', '\n', '.', ','), LongDateFormat, CultureInfo.InvariantCulture);
        }

        #endregion longdate
    }
}