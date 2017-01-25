using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class EmailAddressHelper
    {
        public bool Validate(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;
            if (email.Length < 5)
                return false;
            if (email.Split('@').Length != 2)
                return false;
            if (email.Contains(' '))
                return false;
            if (email.Split('.').Length < 2)
                return false;
            if (email.EndsWith("."))
                return false;

            return true;
        }
    }
}
