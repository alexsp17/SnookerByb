//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Awpbs
//{
//    public class TimeValidator
//    {
//        public bool IsOk { get; set; }
//        public double TimeInSeconds { get; set; }
//        public string ErrorText { get; set; }

//        public bool Validate(ResultType resultType, double time)
//        {
//            this.ErrorText = "";
//            TimeInSeconds = time;
//            IsOk = validateTime(resultType);
//            return IsOk;
//        }
        
//        public bool ParseAndValidate(ResultType resultType, string strTime)
//        {
//            this.ErrorText = "";
//            this.TimeInSeconds = 0;

//            // parse the time
//            bool hoursExpected = false;
//            switch ((SportEnum)resultType.SportID)
//            {
//                case SportEnum.Running: hoursExpected = resultType.Distance >= 13000; break;
//                case SportEnum.Swimming: hoursExpected = resultType.Distance >= 3000; break;
//                case SportEnum.Cycling: hoursExpected = resultType.Distance >= 50000; break;
//            }
//            TimeInSeconds = new TimeHelper().Parse(strTime, hoursExpected);

//            // validate the time
//            IsOk = validateTime(resultType);
//            return IsOk;
//        }

//        private bool validateTime(ResultType resultType)
//        {
//            if (TimeInSeconds == 0)
//            {
//                ErrorText = "Please enter a proper time.";
//                return false;
//            }

//            double distance = resultType.Distance.Value;

//            List<threshold> thresholds = null;
//            switch ((SportEnum)resultType.SportID)
//            {
//                case SportEnum.Running: thresholds = thresholdsRunning; break;
//                case SportEnum.Swimming: thresholds = thresholdsSwimming; break;
//                case SportEnum.Cycling: thresholds = thresholdsCycling; break;
//            }
//            if (thresholds == null || thresholds.Count == 0)
//                return true;

//            threshold theThreshold = thresholds.Where(t => t.Distance >= distance).FirstOrDefault();
//            if (theThreshold == null)
//                theThreshold = thresholds.Last();

//            double scaledTime = TimeInSeconds * (theThreshold.Distance / distance);

//            if (scaledTime <= theThreshold.MinSeconds)
//            {
//                ErrorText = "We cannot believe that your personal best is better than the world record :-)";
//                return false;
//            }
//            if (scaledTime >= theThreshold.MaxSeconds)
//            {
//                ErrorText = "You cannot possibly be that slow :-)";
//                return false;
//            }

//            return true;
//        }

//        private List<threshold> thresholdsRunning = new List<threshold>()
//        {
//            new threshold(100, "9.58", "60"),
//            new threshold(200, "19.19", "2:00"),
//            new threshold(400, "43.18", "4:00"),
//            new threshold(800, "1:40.91", "6:00"),
//            new threshold(1500, "3:26.00", "12:00"),
//            new threshold(5000, "12:37", "59:00"),
//            new threshold(10000, "26:17.00", "2:00:00"),
//            new threshold(13100, "58:23.00", "5:00:00"),
//            new threshold(26200, "2:02.57", "10:00:00"),
//        };

//        private List<threshold> thresholdsSwimming = new List<threshold>()
//        {
//            new threshold(50, "20", "2:00"),
//            new threshold(100, "46", "5:00"),
//            new threshold(400, "3:40", "30:00"),
//            new threshold(1500, "14:31", "2:00:00"),
//        };

//        private List<threshold> thresholdsCycling = new List<threshold>()
//        {
//            new threshold(1000, "56", "10:00"),
//            new threshold(4000, "4:10", "40:00"),
//            new threshold(20000, "15:00", "3:00:00"),
//        };

//        private class threshold
//        {
//            public threshold(double distance, string timeMin, string timeMax)
//            {
//                this.Distance = distance;
//                this.MinSeconds = new TimeHelper().Parse(timeMin);
//                this.MaxSeconds = new TimeHelper().Parse(timeMax);
//            }

//            public double Distance { get; set; }
//            public double MinSeconds { get; set; }
//            public double MaxSeconds { get; set; }
//        }
//    }
//}
