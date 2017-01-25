using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Awpbs
{
    public class TraceHelper
    {
		public static void TraceInfo(string text, bool addExactTime = false)
        {
            try
            {
				if (addExactTime)
					text += " (EXACT TIME: " + DateTime.Now.ToLongTimeString() + ")";
                Console.WriteLine(text);
                //System.Diagnostics.Trace.TraceInformation(addTimeToText(text));
            }
            catch { }
        }

		public static void TraceInfoForResponsiveness(string text)
		{
			try
			{
				DateTime now = DateTime.Now;
				string theText = "~~~~~~~~~~ RESPONSIVENESS: " + now.ToString("HH:mm:ss.ff") + ")" + " " + text;
				Console.WriteLine(theText);
				//System.Diagnostics.Trace.TraceInformation(addTimeToText(text));
			}
			catch { }
		}

        public static void TraceError(string text)
        {
            try
            {
                Console.WriteLine("ERROR!!! " + text);
                //System.Diagnostics.Trace.TraceError(addTimeToText(text));
            }
            catch { }
        }

        public static void TraceException(string text, Exception exc)
        {
            try
            {
                Console.WriteLine("EXCEPTION!!!  " + text + " EXCEPTION INFO: " + exceptionToString(exc, 0));
                //System.Diagnostics.Trace.TraceError(addTimeToText(text + " EXCEPTION INFO: " + exceptionToString(exc, 0)));
            }
            catch { }
        }

        public static string ExceptionToString(Exception exc)
        {
            try
            {
                string fullText = "XXXXX Exception: " + exceptionToString(exc, 0);
                return fullText;
            }
            catch
            {
                return "???unknown???";
            }
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;end]= " + (timeEnd - timeSection1).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeSection2, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;2]= " + (timeSection2 - timeSection1).TotalSeconds.ToString("F1") +
                " [2;end]= " + (timeEnd - timeSection2).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeSection2, DateTime timeSection3, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;2]= " + (timeSection2 - timeSection1).TotalSeconds.ToString("F1") +
                " [2;3]= " + (timeSection3 - timeSection2).TotalSeconds.ToString("F1") +
                " [3;end]= " + (timeEnd - timeSection3).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeSection2, DateTime timeSection3, DateTime timeSection4, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;2]= " + (timeSection2 - timeSection1).TotalSeconds.ToString("F1") +
                " [2;3]= " + (timeSection3 - timeSection2).TotalSeconds.ToString("F1") +
                " [3;4]= " + (timeSection4 - timeSection3).TotalSeconds.ToString("F1") +
                " [4;end]= " + (timeEnd - timeSection4).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeSection2, DateTime timeSection3, DateTime timeSection4, DateTime timeSection5, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;2]= " + (timeSection2 - timeSection1).TotalSeconds.ToString("F1") +
                " [2;3]= " + (timeSection3 - timeSection2).TotalSeconds.ToString("F1") +
                " [3;4]= " + (timeSection4 - timeSection3).TotalSeconds.ToString("F1") +
                " [4;5]= " + (timeSection5 - timeSection4).TotalSeconds.ToString("F1") +
                " [5;end]= " + (timeEnd - timeSection5).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        public static void TraceTimeSpan(DateTime timeBegin, DateTime timeSection1, DateTime timeSection2, DateTime timeSection3, DateTime timeSection4, DateTime timeSection5, DateTime timeSection6, DateTime timeEnd, string text)
        {
            string fullText = text + ": TimeSpan in seconds:" +
                " [begin;end]= " + (timeEnd - timeBegin).TotalSeconds.ToString("F1") +
                " [begin;1]= " + (timeSection1 - timeBegin).TotalSeconds.ToString("F1") +
                " [1;2]= " + (timeSection2 - timeSection1).TotalSeconds.ToString("F1") +
                " [2;3]= " + (timeSection3 - timeSection2).TotalSeconds.ToString("F1") +
                " [3;4]= " + (timeSection4 - timeSection3).TotalSeconds.ToString("F1") +
                " [4;5]= " + (timeSection5 - timeSection4).TotalSeconds.ToString("F1") +
                " [5;6]= " + (timeSection6 - timeSection5).TotalSeconds.ToString("F1") +
                " [6;end]= " + (timeEnd - timeSection6).TotalSeconds.ToString("F1");
            Console.WriteLine(fullText);
            //System.Diagnostics.Trace.TraceInformation(addTimeToText(fullText));
        }

        private static string exceptionToString(Exception exc, int iter)
        {
            string str = "";
            if (string.IsNullOrEmpty(exc.Message) == false)
                str += "\r\nException.Message: " + exc.Message;
            if (string.IsNullOrEmpty(exc.Source) == false)
                str += "\r\nException.Source: " + exc.Source;
            if (string.IsNullOrEmpty(exc.StackTrace) == false)
                str += "\r\nException.StackTrace: " + exc.StackTrace;
            if (exc.InnerException != null)
            {
                if (iter > 10)
                    str += "\r\nTHERE ARE MORE INNER EXCEPTIONS. SKIPPING THEM";
                else
                    str += "\r\nInnerException: " + exceptionToString(exc.InnerException, iter + 1);
            }
            return str;
        }

        //private static string addTimeToText(string text)
        //{
        //    DateTime dateTime = DateTime.UtcNow;
        //    string newText = "UTC " + dateTime.ToShortDateString() + " - " + dateTime.ToShortTimeString() + " - " + text;
        //    return newText;
        //}
    }
}
