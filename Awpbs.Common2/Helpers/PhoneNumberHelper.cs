using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class PhoneNumberHelper
    {
        public static string ToDialable(string phoneNumber, string country)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return "";

            string phone = phoneNumber;
            phone = phone.Replace(" ", "");
            phone = phone.Replace(")", "");
            phone = phone.Replace("(", "");
            phone = phone.Replace("-", "");
            phone = phone.Replace(" ", "");

            if (string.IsNullOrEmpty(phoneNumber))
                return "";

            if (phone.StartsWith("+"))
                return phone;

            string countryCode = "";
            var countryObj = Country.Get(country);
            if (countryObj != null)
            {
                if (countryObj.IsBritain)
                    countryCode = "44";
                else if (countryObj.IsUSA)
                    countryCode = "1";
            }

            if (countryCode != "" && phone.StartsWith(countryCode))
                return phone;

            phone = "+" + countryCode + phone;
            return phone;
        }
    }
}
