using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web.App
{
    public class Config
    {
        public static string GoogleApiKey
        {
            get
            {
                string key = System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleApiKey"];
                return key;
            }
        }
        public static string FacebookAppId
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["FacebookAppId"];
            }
        }
    }
}
