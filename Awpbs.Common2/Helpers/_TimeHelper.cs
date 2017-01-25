//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Awpbs
//{
//    public class TimeHelper
//    {
//        public string TimeToString(double timeInSeconds)
//        {
//            if (timeInSeconds <= 1)
//                return "?";

//            int hours = 0;
//            int minutes = 0;
//            int seconds = 0;
//            int milliseconds = 0;

//            hours = (int)(timeInSeconds / (60 * 60));
//            timeInSeconds -= hours * 60 * 60;

//            minutes = (int)(timeInSeconds / 60);
//            timeInSeconds -= minutes * 60;

//            seconds = (int)timeInSeconds;
//            milliseconds = (int)((timeInSeconds - seconds) * 1000);

//            string text = "";
//            if (hours > 0)
//                text = hours + ":" + minutes.ToString("D2") + ":" + seconds.ToString("D2");
//            else if (minutes > 0)
//                text = minutes.ToString() + ":" + seconds.ToString("D2");
//            else
//                text = seconds.ToString("D2");

//            if (milliseconds > 0 || (hours == 0 && minutes == 0))
//                text += "." + ((int)System.Math.Round(milliseconds / 10.0, 0)).ToString("D2");

//            return text;
//        }

//        /// <summary>
//        /// Returns time in seconds. If fails, returns 0
//        /// </summary>
//        public double Parse(string time, bool hoursExpected = false)
//        {
//            if (string.IsNullOrEmpty(time))
//                return 0;

//            string strHoursMinsSeconds = "";
//            string strMilliseconds = "";
//            var strs = time.Split('.');
//            if (strs.Length == 2)
//            {
//                strHoursMinsSeconds = strs[0];
//                strMilliseconds = strs[1];
//            }
//            else if (strs.Length == 1)
//            {
//                strHoursMinsSeconds = strs[0];
//                strMilliseconds = "";
//            }
//            else
//            {
//                return 0;
//            }

//            int milliseconds = 0;
//            if (strMilliseconds.Length > 0)
//            {
//                if (int.TryParse(strMilliseconds, out milliseconds) == false)
//                    return 0;
//                if (milliseconds <= 9)
//                    milliseconds *= 100;
//                else if (milliseconds <= 99)
//                    milliseconds *= 10;
//            }

//            int hours = 0;
//            int minutes = 0;
//            int seconds = 0;
//            var strs2 = strHoursMinsSeconds.Split(':');
//            if (strs2.Length == 3)
//            {
//                if (int.TryParse(strs2[0], out hours) == false)
//                    return 0;
//                if (int.TryParse(strs2[1], out minutes) == false)
//                    return 0;
//                if (int.TryParse(strs2[2], out seconds) == false)
//                    return 0;
//            }
//            else if (strs2.Length == 2)
//            {
//                if (hoursExpected)
//                {
//                    if (int.TryParse(strs2[0], out hours) == false)
//                        return 0;
//                    if (int.TryParse(strs2[1], out minutes) == false)
//                        return 0;
//                }
//                else
//                {
//                    if (int.TryParse(strs2[0], out minutes) == false)
//                        return 0;
//                    if (int.TryParse(strs2[1], out seconds) == false)
//                        return 0;
//                }
//            }
//            else if (strs2.Length == 1)
//            {
//                if (int.TryParse(strs2[0], out seconds) == false)
//                    return 0;
//            }
//            else
//            {
//                return 0;
//            }

//            if (hours > 24 || hours < 0)
//                return 0;
//            if (minutes > 60 || minutes < 0)
//                return 0;
//            if (seconds > 60 || seconds < 0)
//                return 0;
//            if (milliseconds > 1000 || milliseconds < 0)
//                return 0;

//            double totalSeconds = hours * 60 * 60 + minutes * 60 + seconds + milliseconds / 1000.0;
//            return totalSeconds;
//        }

//        public string GetExampleTimeString(SportEnum sport, double distance)
//        {
//            if (sport == SportEnum.Running)
//            {
//                if (distance <= 100)
//                    return "12.45 or 0:12.45";
//                if (distance <= 200)
//                    return "25.46 or 0:25.46";
//                if (distance <= 400)
//                    return "56.78 or 1:02 or 1:14.15";
//                if (distance <= 800)
//                    return "2:12.15 or 2:12";
//                if (distance <= 1600)
//                    return "6:12.15 or 6:12";
//                if (distance <= 5000)
//                    return "23:45 or 23:45.12";
//                if (distance <= 10000)
//                    return "46:12 or 1:01:12";
//                if (distance <= 13200)
//                    return "1:45:56 or 1:46";
//                return "3:55:59 or 3:56";
//            }

//            if (sport == SportEnum.Swimming)
//            {
//                if (distance <= 50)
//                    return "45 or 58.2 or 1:02.4";
//                if (distance <= 100)
//                    return "59.5 or 1:15";
//                if (distance <= 200)
//                    return "3:02 or 3:02.15";
//                if (distance > 5000)
//                    return "1:02:03.56";
//                return "6:05 or 6:05.56";
//            }

//            return "?";
//        }

//        public bool IsHourNeeded(SportEnum sport, double distance)
//        {
//            if (sport == SportEnum.Running)
//                return distance > 3000;
//            if (sport == SportEnum.Swimming)
//                return distance > 500;
//            if (sport == SportEnum.Cycling)
//                return distance > 5000;
//            return true;
//        }

//        public bool IsMillisecondNeeded(SportEnum  sport, double distance)
//        {
//            if (sport == SportEnum.Running)
//                return distance <= 10000;
//            if (sport == SportEnum.Swimming)
//                return distance <= 1000;
//            if (sport == SportEnum.Cycling)
//                return false;
//            return true;
//        }
//    }
//}
