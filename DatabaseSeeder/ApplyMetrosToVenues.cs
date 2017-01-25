using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class ApplyMetrosToVenues
    {
        private readonly ApplicationDbContext db;

        public ApplyMetrosToVenues(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void ApplyMetrosToVenuesWithoutMetros(out int countVenuesUpdated, out int countVenuesSkipped)
        {
            countVenuesUpdated = 0;
            countVenuesSkipped = 0;

            var metros = db.Metros.ToList();
            var venues = (from v in db.Venues
                          where v.MetroID == null
                          select v).ToList();

            foreach (var venue in venues)
            {
                var metrosInTheCountry = (from m in metros
                                          where m.Country == venue.Country
                                          select m).ToList();

                Metro metroWithMinDistance = null;
                Distance minDistance = null;

                foreach (var metro in metrosInTheCountry)
                {
                    Distance distance = Distance.Calculate(venue.Location, metro.Location);
                    if (minDistance == null || distance.Meters < minDistance.Meters)
                    {
                        minDistance = distance;
                        metroWithMinDistance = metro;
                    }
                }

                if (minDistance != null && minDistance.Miles < 200)
                {
                    venue.MetroID = metroWithMinDistance.MetroID;
                    countVenuesUpdated++;
                }
                else
                {
                    countVenuesSkipped++;
                }
            }

            db.SaveChanges();
        }
    }
}
