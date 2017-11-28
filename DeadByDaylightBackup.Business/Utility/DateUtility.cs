using System;
using System.Globalization;

namespace DeadByDaylightBackup.Utility
{
    /// <summary>
    /// Provides some simple date format functions
    /// </summary>
    public static class DateUtility
    {
        #region shortdate

        /// <summary>
        /// Datestring that is not seperated
        /// </summary>
        public const string ShortDateFormat = "yyyyMMddHHmm";

        /// <summary>
        /// Convert datetime to non seperated datestring
        /// </summary>
        /// <param name="time">Input datetime</param>
        /// <returns> string representation</returns>
        public static string SimpleShortFormat(this DateTime? time)
        {
            return SimpleShortFormat(time.GetValueOrDefault(DateTime.Now));
        }

        /// <summary>
        /// Convert datetime to non seperated datestring
        /// </summary>
        /// <param name="time">Input datetime</param>
        /// <returns> string representation</returns>
        public static string SimpleShortFormat(this DateTime time)
        {
            return time.ToString(ShortDateFormat);
        }

        /// <summary>
        /// Convert String in short format to DateTime
        /// </summary>
        /// <param name="timestamp">input string</param>
        /// <returns>DateTime result</returns>
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

        /// <summary>
        /// Datestring that is seperated
        /// </summary>
        public const string LongDateFormat = "yyyy-MM-dd HH:mm";

        /// <summary>
        /// Convert datetime to  seperated datestring
        /// </summary>
        /// <param name="time">Input datetime</param>
        /// <returns> string representation</returns>
        public static string SimpleLongFormat(this DateTime? time)
        {
            return SimpleLongFormat(time.GetValueOrDefault(DateTime.Now));
        }

        /// <summary>
        /// Convert datetime to seperated datestring
        /// </summary>
        /// <param name="time">Input datetime</param>
        /// <returns> string representation</returns>
        public static string SimpleLongFormat(this DateTime time)
        {
            return time.ToString(LongDateFormat);
        }

        /// <summary>
        /// Convert String in long format to DateTime
        /// </summary>
        /// <param name="timestamp">input string</param>
        /// <returns>DateTime result</returns>
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
