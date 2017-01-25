using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public abstract class IKeyChain
    {
        public abstract bool Add(string key, string value);
        public abstract void Delete(string key);
        public abstract string Get(string key);

        public string AccessToken
        {
            get { return this.Get("AccessToken"); }
            set { this.Add("AccessToken", value); }
        }

		public string PinCode
		{
			get { return this.Get ("PinCode"); }
			set { this.Add ("PinCode", value); }
		}

        public string RegisteredDeviceToken
        {
            get { return this.Get("RegisteredDeviceToken2"); }
            set { this.Add("RegisteredDeviceToken2", value); }
        }

        public string DeviceTokenToRegisterWhenLoggedIn
        {
            get { return this.Get("DeviceTokenToRegisterWhenLoggedIn"); }
            set { this.Add("DeviceTokenToRegisterWhenLoggedIn", value); }
        }
    }
}
