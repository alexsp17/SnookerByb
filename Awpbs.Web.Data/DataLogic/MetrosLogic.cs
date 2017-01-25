using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class MetrosLogic
    {
        private readonly ApplicationDbContext db;

        public MetrosLogic(ApplicationDbContext context)
        {
            this.db = context;
        }

        public MetroWebModel Get(int id)
        {
            return (from i in db.Metros
                    where i.MetroID == id
                    select new MetroWebModel
                    {
                        ID = i.MetroID,
                        Name = i.Name,
                        UrlName = i.UrlName,
                        Country = i.Country,
                        Latitude = i.Latitude,
                        Longitude = i.Longitude
                    }).Single();
        }

        public MetroWebModel Get(string country, string name)
        {
            return (from i in db.Metros
                    where i.Country == country && i.UrlName == name
                    select new MetroWebModel
                    {
                        ID = i.MetroID,
                        Name = i.Name,
                        UrlName = i.UrlName,
                        Country = i.Country,
                        Latitude = i.Latitude,
                        Longitude = i.Longitude
                    }).Single();
        }

        public List<MetroWebModel> GetMetrosInCountry(Country country)
        {
            return (from i in db.Metros
                    where i.Country == country.ThreeLetterCode
                    orderby i.Name
                    select new MetroWebModel
                    {
                        ID = i.MetroID,
                        Name = i.Name,
                        UrlName = i.UrlName,
                        Country = i.Country,
                        Latitude = i.Latitude,
                        Longitude = i.Longitude,
                    }).ToList();
        }

        public List<MetroWebModel> GetMetrosAround(Location location)
        {
            List<int> kmAway = new List<int> { 100, 1000, 10000 };

            List<MetroWebModel> metros = new List<MetroWebModel>();
            foreach (int km in kmAway)
            {
                Location location2 = location.OffsetRoughly(km * 1000, km * 1000);
                double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
                double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

                var list = (from i in db.Metros
                            where i.Latitude > location.Latitude - latRange && i.Latitude < location.Latitude + latRange
                            where i.Longitude > location.Longitude - lonRange && i.Longitude < location.Longitude + lonRange
                            select new MetroWebModel
                            {
                                ID = i.MetroID,
                                Name = i.Name,
                                UrlName = i.UrlName,
                                Country = i.Country,
                                Latitude = i.Latitude,
                                Longitude = i.Longitude,
                            }).Take(20).ToList();

                foreach (var metro in list)
                    if (metros.Where(i => i.ID == metro.ID).Count() == 0)
                        metros.Add(metro);

                if (metros.Count > 0)
                    break;
            }

            foreach (var metro in metros)
                metro.DistanceInMeters = (int)Distance.Calculate(location, metro.Location).Meters;

            metros = (from metro in metros
                      orderby metro.DistanceInMeters, metro.Name
                      select metro).ToList();

            return metros;
        }

        public MetroWebModel GetClosestMetro(Location location)
        {
            var metro = (from i in db.Metros
                         orderby System.Math.Abs(i.Latitude - location.Latitude) + System.Math.Abs(i.Longitude - location.Longitude)
                         select new MetroWebModel
                         {
                             ID = i.MetroID,
                             Name = i.Name,
                             UrlName = i.UrlName,
                             Country = i.Country,
                             Latitude = i.Latitude,
                             Longitude = i.Longitude,
                         }).FirstOrDefault();
            return metro;
        }
    }
}
