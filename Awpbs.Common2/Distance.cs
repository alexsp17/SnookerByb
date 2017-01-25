using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// Distance from one "Location" to another
    /// </summary>
    public class Distance
    {
        public static Distance FromMiles(double miles)
        {
            return new Distance() { Meters = (int)(miles * 1609.34) };
        }

        public static Distance FromMeters(double meters)
        {
            return new Distance() { Meters = meters };
        }

        public static Distance Calculate(Location location1, Location location2)
        {
            double R = 6371000; // earth radius, m
            double lat1 = location1.LatitudeInRadians;
            double lat2 = location2.LatitudeInRadians;
            double lon1 = location1.LongitudeInRadians;
            double lon2 = location2.LongitudeInRadians;

            double a = System.Math.Sin((lat2 - lat1) / 2.0) * System.Math.Sin((lat2 - lat1) / 2.0) +
                System.Math.Cos(lat1) * System.Math.Cos(lat2) *
                System.Math.Sin((lon2 - lon1) / 2.0) * System.Math.Sin((lon2 - lon1) / 2.0);
            double c = 2.0 * System.Math.Atan2(System.Math.Sqrt(a), System.Math.Sqrt(1 - a));
            double distance = R * c;

            return new Distance() { Meters = distance };
        }

        public double Meters { get; set; }

        public double Miles
        {
            get
            {
                return Meters / 1609.34;
            }
        }

        public override string ToString()
        {
            if (Miles > 2)
                return Miles.ToString("F0") + " miles";
            return Miles.ToString("F1") + " miles";
        }
    }
}
