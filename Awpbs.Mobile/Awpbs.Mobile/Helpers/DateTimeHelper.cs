using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class DateTimeHelper
    {
        public static DateTime GetUtcNow()
        {
            DateTime dateTime = DateTime.UtcNow;

            // note: Android emulators are a little crazy and report incorrect UTC time (as of July 2015). iOS emulators appear to be fine
            //dateTime = dateTime.AddHours(9);

            return dateTime;
        }

        public static string DateToString(DateTime date)
        {
            string str = date.ToString("MMM dd");
            if (date.Year != DateTime.Now.Year)
                str += ", " + date.Year.ToString();

            return str;
        }

        public static string DateAndTimeToString(DateTime date)
        {
            string str = DateToString(date);
            str += " - " + date.ToShortTimeString();
            return str;
        }

#if __IOS__
        public static Foundation.NSDate DateTimeToNSDate(DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return (Foundation.NSDate)date;
        }
#endif

    }
}
