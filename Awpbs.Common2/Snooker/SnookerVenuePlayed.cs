using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class SnookerVenuePlayed
    {
        public VenueWebModel Venue { get; set; }
        public int CountBests { get; set; }
        public int CountMatches { get; set; }

        public static List<SnookerVenuePlayed> Sort(List<SnookerVenuePlayed> list, SnookerVenuesPlayedSortEnum sort)
        {
            if (sort == SnookerVenuesPlayedSortEnum.ByName)
            {
                return (from i in list
                        orderby i.Venue.Name
                        select i).ToList();
            }
            if (sort == SnookerVenuesPlayedSortEnum.ByCount)
            {
                return (from i in list
                        orderby i.CountMatches + i.CountBests descending, i.Venue.Name
                        select i).ToList();
            }
            return list;
        }
    }
}
