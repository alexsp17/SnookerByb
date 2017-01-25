using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class RegisterDeviceTokenWebModel
    {
        public string Token { get; set; }
        public bool IsApple { get; set; }
        public bool IsAndroid { get; set; }
        public bool IsNotProduction { get; set; }
    }
}
