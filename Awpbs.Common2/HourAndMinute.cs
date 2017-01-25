using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class HourAndMinute
    {
        public HourAndMinute()
        {

        }
        public HourAndMinute(int hour, int minute)
        {
            this.Hour = hour;
            this.Minute = minute;
        }
        public static HourAndMinute Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            bool isPM = false;
            if (text.EndsWith("pm"))
            {
                text = text.Remove(text.Length - 2, 2);
                isPM = true;
            }
            else if (text.EndsWith("am"))
            {
                text = text.Remove(text.Length - 2, 2);
            }
            text = text.Trim();

            string[] strs = text.Split(':');
            if (strs.Length != 2)
                return null;
            int hour;
            int minute;
            if (int.TryParse(strs[0].Trim(), out hour) == false)
                return null;
            if (int.TryParse(strs[1].Trim(), out minute) == false)
                return null;
            if (isPM && hour != 12)
                hour += 12;

            HourAndMinute obj = new HourAndMinute(hour, minute);
            if (obj.IsValid == false)
                return null;
            return obj;
        }

        public int Hour { get; set; }
        public int Minute { get; set; }

        public bool IsValid
        {
            get
            {
                return Hour >= 0 && Hour <= 23 && Minute >= 0 && Minute <= 59;
            }
        }

        public string Text
        {
            get
            {
                string text = "";
                if (Hour >= 13)
                    text += (Hour - 12).ToString();
                else
                    text += Hour.ToString();
                text += ":";
                if (Minute < 10)
                    text += "0";
                text += Minute.ToString();
                if (Hour >= 12)
                    text += " pm";
                else
                    text += " am";
                return text;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
