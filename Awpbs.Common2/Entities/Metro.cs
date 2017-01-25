using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Metro
    {
        public int MetroID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string UrlName { get; set; }

        public string CountryDisplay
        {
            get
            {
                if (Country == "GBR")
                    return "UK";
                return Country;
            }
        }

        public Location Location
        {
            get
            {
                return new Location(Latitude, Longitude);
            }
        }

        public virtual List<Post> Posts { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
