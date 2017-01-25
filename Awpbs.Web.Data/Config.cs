using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class Config
    {
        public static readonly string ProductName = "Snooker Byb";
        public static readonly string CompanyName = "Best Your Best";

        public static bool IsSnookerByb { get { return true; } }

        public static bool IsProduction
        {
            get
            {
                if (isProduction == null)
                {
                    string str = Environment.GetEnvironmentVariable("BYB_IS_PRODUCTION");
                    isProduction = string.Compare(str, "true") == 0;
                }

                return isProduction.Value;
            }
        }
        private static bool? isProduction;
    }
}
