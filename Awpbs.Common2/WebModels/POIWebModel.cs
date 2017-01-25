using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class POIWebModel
    {
        public string PoiID { get; set; }
        public Location Location { get; set; }
        public string Name { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        public bool HasPoiID { get { return string.IsNullOrEmpty(PoiID) == false; } }
        public bool HasWebsite { get { return string.IsNullOrEmpty(Website) == false; } }
        public bool HasAddress { get { return string.IsNullOrEmpty(Address) == false; } }
        public bool HasPhone { get { return string.IsNullOrEmpty(Phone) == false; } }

        public Distance Distance { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
