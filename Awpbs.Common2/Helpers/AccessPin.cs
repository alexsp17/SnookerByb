using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class AccessPinHelper
    {
		public const int PinLength = 4;
		
        public bool Validate(string pin)
        {
            if (string.IsNullOrEmpty(pin))
                return false;

			if (pin.Length != PinLength)
                return false;

            int intPin;
            if (int.TryParse(pin, out intPin) == false)
                return false;

            return true;
        }
    }
}
