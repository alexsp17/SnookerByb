using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class BybApiAboutWebModel
    {
        public int ThisApiVersionNumber { get; set; }
        public string ThisApiVersionName { get; set; }

        public int AppVersionLatestNumber { get; set; }
        public string AppVersionLatestName { get; set; }
        public int AppVersionUpgradeRecommended { get; set; }
        public int AppVersionUpgradeRequired { get; set; }
    }
}
