using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// Latigude, Longitude
    /// </summary>
    public class Location
    {
        public Location()
        {
        }

        public Location(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double LatitudeInRadians
        {
            get
            {
                return Latitude * System.Math.PI / 180;
            }
        }

        public double LongitudeInRadians
        {
            get
            {
                return Longitude * System.Math.PI / 180;
            }
        }

        public Location OffsetRoughly(double metersWest, double metersNorth)
        {
            // http://www.csgnetwork.com/degreelenllavcalc.html

            double deltaLat;
            double deltaLon;

            if (System.Math.Abs(Latitude) < 20)
            {
                // close to Equator
                deltaLat = metersNorth / 110570.0;
                deltaLon = metersWest / 111320.0;
            }
            else if (System.Math.Abs(Latitude) < 30)
            {
                // at 20 degrees
                deltaLat = metersNorth / 110704.0;
                deltaLon = metersWest / 104647.0;
            }
            else if (System.Math.Abs(Latitude) < 40)
            {
                // at 30 degrees
                deltaLat = metersNorth / 110852.0;
                deltaLon = metersWest / 96486.0;
            }
            else if (System.Math.Abs(Latitude) < 50)
            {
                // at 40 degrees
                deltaLat = metersNorth / 111030.0;
                deltaLon = metersWest / 85390.0;
            }
            else if (System.Math.Abs(Latitude) < 60)
            {
                // at 50 degrees
                deltaLat = metersNorth / 111229.0;
                deltaLon = metersWest / 71695.0;
            }
            else if (System.Math.Abs(Latitude) < 70)
            {
                // at 60 degrees
                deltaLat = metersNorth / 111412.0;
                deltaLon = metersWest / 55780.0;
            }
            else if (System.Math.Abs(Latitude) < 75)
            {
                // at 70 degrees
                deltaLat = metersNorth / 111561.0;
                deltaLon = metersWest / 38186.0;
            }
            else
            {
                // at 80 degrees
                deltaLat = metersNorth / 111659.0;
                deltaLon = metersWest / 19393.0;
            }

            return new Location(Latitude + deltaLat, Longitude + deltaLon);
            //double lat = Latitude + metersNorth / 111111.0;
            //double lon = Longitude + System.Math.Cos(this.Longitude) * metersWest / 111111.0;
            //return new Location(lat, lon);
        }

        public override string ToString()
        {
            return Latitude.ToString("F4") + ", " + Longitude.ToString("F4");
        }
    }
}
