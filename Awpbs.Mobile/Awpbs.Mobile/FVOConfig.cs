using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class FVOConfig
    {
        public int VenueID { get; set; }
        public string VenueName { get; set; }

        public SnookerTableSizeEnum TableSize { get; set; }
        public string TableDescription { get; set; }

        public int NotableBreakThreshold { get; set; }

        public bool IsOk
        {
            get
            {
                return VenueID > 0 && string.IsNullOrEmpty(VenueName) == false;
            }
        }

        public static FVOConfig LoadFromKeyChain(IKeyChain keychain)
        {
            FVOConfig config = null;

            try
            {
                string str = keychain.Get("FVOConfig");
                config = Newtonsoft.Json.JsonConvert.DeserializeObject<FVOConfig>(str);
            }
            catch (Exception)
            {
            }

            if (config == null)
                config = new FVOConfig();

            return config;
        }

        public void SaveToKeyChain(IKeyChain keychain)
        {
            string str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            keychain.Add("FVOConfig", str);
        }
    }
}
