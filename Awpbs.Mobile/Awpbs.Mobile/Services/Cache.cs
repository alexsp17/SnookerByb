using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Mobile
{
	/// <summary>
	/// A cache for PersonBasicWebModel objects
	/// </summary>
    public class PeopleCache
    {
        readonly TimeSpan lifeTime = TimeSpan.FromDays(1);

        class CacheItem
        {
            public DateTime TimeLoaded { get; set; }
            public PersonBasicWebModel Person { get; set; }
        }

        List<CacheItem> items = new List<CacheItem>();

        public PersonBasicWebModel Get(int id)
        {
            var item = items.Where(i => i.Person.ID == id).FirstOrDefault();
            if (item == null)
                return null;
            if ((DateTimeHelper.GetUtcNow() - item.TimeLoaded) > lifeTime)
                item = null;
            return item.Person;
        }

        public List<PersonBasicWebModel> GetFriends()
        {
            var friends = (from i in items
                           where i.Person.IsFriend == true || i.Person.IsFriendRequestSent
                           select i.Person).ToList();
            return friends;
        }

        public List<PersonBasicWebModel> Get(IEnumerable<int> ids, bool absentIDsOk = false)
        {
            List<PersonBasicWebModel> list = new List<PersonBasicWebModel>();
            foreach (var id in ids)
            {
                var obj = this.Get(id);
                if (obj == null)
                {
                    if (absentIDsOk)
                        continue;
                    return null;
                }
                list.Add(obj);
            }
            return list;
        }

        public void Put(PersonBasicWebModel person)
        {
            var item = items.Where(i => i.Person.ID == person.ID).FirstOrDefault();
            if (item != null)
                items.Remove(item);
            items.Add(new CacheItem() { Person = person, TimeLoaded = DateTimeHelper.GetUtcNow() });
        }

        public void Put(IEnumerable<PersonBasicWebModel> persons)
        {
            foreach (var person in persons)
                if (person != null)
                    this.Put(person);
        }
    }

	/// <summary>
	/// A cache for VenueWebModel objects
	/// </summary>
    public class VenuesCache
    {
        readonly TimeSpan lifeTime = TimeSpan.FromDays(1);

        class CacheItem
        {
            public DateTime TimeLoaded { get; set; }
            public VenueWebModel Venue { get; set; }
        }

        List<CacheItem> items = new List<CacheItem>();

        public VenueWebModel Get(int id)
        {
            var item = items.Where(i => i.Venue.ID == id).FirstOrDefault();
            if (item == null)
                return null;
            if ((DateTimeHelper.GetUtcNow() - item.TimeLoaded) > lifeTime)
                item = null;
            return item.Venue;
        }

        public List<VenueWebModel> Get(IEnumerable<int> ids)
        {
            List<VenueWebModel> list = new List<VenueWebModel>();
            foreach (var id in ids)
            {
                var obj = this.Get(id);
                if (obj == null)
                    return null;
                list.Add(obj);
            }
            return list;
        }

        public VenueWebModel GetClosest(Location location, Distance minDistance)
        {
            Distance closestDistance = null;
            VenueWebModel closestVenue = null;
            foreach (var item in items)
            {
                Distance distance = Distance.Calculate(item.Venue.Location, location);
                if (closestDistance == null || distance.Meters < closestDistance.Meters)
                {
                    closestDistance = distance;
                    closestVenue = item.Venue;
                }
            }
            if (closestDistance != null && minDistance != null && closestDistance.Meters > minDistance.Meters)
                return null;
            return closestVenue;
        }

        public void Put(VenueWebModel venue)
        {
            var item = items.Where(i => i.Venue.ID == venue.ID).FirstOrDefault();
            if (item != null)
                items.Remove(item);
            items.Add(new CacheItem() { Venue = venue, TimeLoaded = DateTimeHelper.GetUtcNow() });
        }

        public void Put(IEnumerable<VenueWebModel> venues)
        {
            foreach (var venue in venues)
                this.Put(venue);
        }
    }

	/// <summary>
	/// A cache for MetroWebModel objects
	/// </summary>
    public class MetroesCache
    {
        readonly TimeSpan lifeTime = TimeSpan.FromDays(300);

        class CacheItem
        {
            public DateTime TimeLoaded { get; set; }
            public MetroWebModel Metro { get; set; }
        }

        List<CacheItem> items = new List<CacheItem>();

        public MetroWebModel Get(int id)
        {
            var item = items.Where(i => i.Metro.ID == id).FirstOrDefault();
            if (item == null)
                return null;
            if ((DateTimeHelper.GetUtcNow() - item.TimeLoaded) > lifeTime)
                item = null;
            return item.Metro;
        }

		public List<MetroWebModel> GetForCountry(string country)
		{
			List<MetroWebModel> list = items.Where (i => i.Metro.Country == country).Select(i => i.Metro).ToList ();
			return list;
		}

        public void Put(MetroWebModel metro)
        {
            var item = items.Where(i => i.Metro.ID == metro.ID).FirstOrDefault();
            if (item != null)
                items.Remove(item);
            items.Add(new CacheItem() { Metro = metro, TimeLoaded = DateTimeHelper.GetUtcNow() });
        }

        public void Put(IEnumerable<MetroWebModel> metros)
        {
            foreach (var metro in metros)
                this.Put(metro);
        }
    }

	/// <summary>
	/// Cache service. Contains PeopleCache, VenuesCache, MetroesCache
	/// </summary>
    public class CacheService
    {
        public CacheService(WebService webservice)
        {
            this.webservice = webservice;
            People = new PeopleCache();
            Venues = new VenuesCache();
            Metroes = new MetroesCache();
        }

        private WebService webservice;

        public PeopleCache People { get; private set; }
        public VenuesCache Venues { get; private set; }
        public MetroesCache Metroes { get; private set; }

        public async Task LoadFromWebserviceIfNecessary_People(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return;
            if (this.People.Get(ids) != null)
                return; // already available in the cache

            // try downloading from the webservice
            var people = await webservice.GetPeopleByID(ids);
            if (people != null)
                this.People.Put(people);
        }

        public async Task LoadFromWebserviceIfNecessary_Venues(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return;
            if (App.Cache.Venues.Get(ids) != null)
                return; // already available in the cache

            // try downloading from the webservice
            var venues = await webservice.GetVenues(ids);
            if (venues != null)
                this.Venues.Put(venues);
        }

        public async Task LoadFromWebserviceIfNecessary_Metro(int metroID)
        {
            if (this.Metroes.Get(metroID) != null)
                return; // already available in the cache

            // try downloading from the webservice
            var metro = await webservice.GetMetro(metroID);
            if (metro != null)
                this.Metroes.Put(metro);
        }
    }
}
