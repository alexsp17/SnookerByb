using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class MetroWebModel
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double DistanceInMeters { get; set; }
        public string UrlName { get; set; }

        public Location Location
        {
            get
            {
                return new Location(Latitude, Longitude);
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
