using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class LoginHelper
    {
        public static string BuildLoginEmailFromFacebookId(string facebookId)
        {
            string email = "fb" + facebookId.ToLower() + "@bestyourbest.com";
            return email;
        }

        public static string BuildLoginPasswordFromFacebookId(string facebookId)
        {
            string loginEmail = BuildLoginEmailFromFacebookId(facebookId);
            return BuildLoginPasswordFromLoginEmail(loginEmail);
        }

        public static string BuildLoginPasswordFromLoginEmail(string loginEmail)
        {
            string password = Crypto.Encrypt(loginEmail.ToLower(), "FACEBOOK");
            if (password.Length > 50)
                password = password.Substring(0, 50);
            return password;
        }

        // pre 9/4/2015:
        public static string BuildPasswordFromEmail_PreAugust2015(string email)
        {
            string password = Crypto.Encrypt(email.ToLower(), "FACEBOOK");
            if (password.Length > 40)
                password = password.Substring(0, 30);
            return password;
        }
    }
}
