using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace Awpbs.Mobile
{
    public partial class WebService
    {
        public async Task<List<Venue>> FindVenues(bool snooker, Location location, string searchQuery)
        {
			string url = WebApiUrl + "Venues/Find?snooker=" + snooker.ToString();
			if (location != null)
				url += "&latitude=" + location.Latitude + "&longitude=" + location.Longitude;
			if (searchQuery == null)
				searchQuery = "";
			searchQuery = searchQuery.Trim();
			url += "&searchQuery=" + System.Net.WebUtility.UrlEncode(searchQuery);
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<VenueWebModel> venues = JsonConvert.DeserializeObject<List<VenueWebModel>>(json);
                return (from i in venues
                        select i.ToVenue()).ToList();
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

        public async Task<List<VenueWebModel>> FindSnookerVenues(Location location, string searchQuery, bool requireSnookerTables, bool require12ftSnookerTables)
        {
			string url = WebApiUrl + "Venues/FindSnooker?requireSnookerTables=" + requireSnookerTables.ToString();
			url += "&require12ftSnookerTables=" + require12ftSnookerTables.ToString();
			if (location != null)
				url += "&latitude=" + location.Latitude + "&longitude=" + location.Longitude;
			if (searchQuery == null)
				searchQuery = "";
			searchQuery = searchQuery.Trim();
			url += "&searchQuery=" + System.Net.WebUtility.UrlEncode(searchQuery);
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<VenueWebModel> venues = JsonConvert.DeserializeObject<List<VenueWebModel>>(json);
                return venues;
            }
            catch (Exception exc)
            {
                LastException = exc;
				LastExceptionUrl = url;
                return null;
            }
        }

		public async Task<FindVenuesWebModel> FindSnookerVenues2(Location location, string country, int radiusInMeters, string searchQuery, int maxCount)
		{
			string url = WebApiUrl + "Venues/FindSnooker2?radiusInMeters=" + radiusInMeters.ToString("F0");
			url += "&country=" + country;
			url += "&maxCount=" + maxCount;
			if (location != null)
				url += "&latitude=" + location.Latitude + "&longitude=" + location.Longitude;
			if (searchQuery == null)
				searchQuery = "";
			searchQuery = searchQuery.Trim();
			if (searchQuery.Length > 0)
				url += "&searchQuery=" + System.Net.WebUtility.UrlEncode(searchQuery);
			try
			{
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
				FindVenuesWebModel result = JsonConvert.DeserializeObject<FindVenuesWebModel>(json);
				TraceHelper.TraceInfo("FindSnookerVenues2 - ok");
				return result;
			}
			catch (Exception exc)
			{
				TraceHelper.TraceException ("FindSnookerVenues2 exception", exc);
				LastException = exc;
				LastExceptionUrl = url;
				return null;
			}
		}

        public async Task<int> CreateNewVenue(VenueWebModel venue)
        {
			string url = WebApiUrl + "Venues";
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, venue, true);
                int venueID = int.Parse(json);
                return venueID;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return 0;
            }
        }

        public async Task<VenueWebModel> GetVenue(int venueID)
        {
			string url = WebApiUrl + "Venues/" + venueID;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                VenueWebModel venue = JsonConvert.DeserializeObject<VenueWebModel>(json);
                return venue;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<List<VenueWebModel>> GetVenues(List<int> venueIDs)
        {
			string str = "";
			foreach (int id in venueIDs)
			{
				if (str.Length > 0)
					str += ",";
				str += id.ToString();
			}
			string url = WebApiUrl + "Venues?ids=" + str;
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<VenueWebModel> venues = JsonConvert.DeserializeObject<List<VenueWebModel>>(json);
                return venues;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

        public async Task<bool> VerifyOrEditVenue(VenueEditWebModel venueEdit)
        {
			string url = WebApiUrl + "Venues/VerifyOrEditVenue";
            try
            {
				string json = await this.sendPostRequestAndReceiveResponse(url, venueEdit, true);
                bool ok = JsonConvert.DeserializeObject<bool>(json);
                return ok;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return false;
            }
        }
    }
}
