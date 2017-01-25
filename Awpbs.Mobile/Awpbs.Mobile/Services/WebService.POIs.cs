using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace Awpbs.Mobile
{
    public partial class WebService
    {
        public async Task<List<POIWebModel>> FindPOIs(Location location, Distance distance, string keyword = "")
        {
			if (keyword == null)
				keyword = "";
			string url = WebApiUrl + "POIs" +
				"?lat=" + location.Latitude.ToString() +
				"&lon=" + location.Longitude.ToString() +
				"&radiusInMeters=" + distance.Meters.ToString() +
				"&keyword=" + WebUtility.UrlEncode(keyword);
            try
            {
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
                List<POIWebModel> pois = JsonConvert.DeserializeObject<List<POIWebModel>>(json);
                return pois;
            }
            catch (Exception exc)
            {
				LastExceptionUrl = url;
                LastException = exc;
                return null;
            }
        }

		public async Task<Location> LocationFromAddress(string address, Location approximateLocation = null)
		{
			string url = WebApiUrl + "POIs/LocationFromAddress?" +
				"address=" + WebUtility.UrlEncode(address);
			if (approximateLocation != null)
				url += "&approximateLat=" + approximateLocation.Latitude.ToString() +
					"&approximateLon=" + approximateLocation.Longitude.ToString();

			try
			{
				string json = await this.sendGetRequestAndReceiveResponse(url, true);
				Location location = JsonConvert.DeserializeObject<Location>(json);
				return location;
			}
			catch (Exception exc)
			{
				LastExceptionUrl = url;
				LastException = exc;
				return null;
			}
		}
    }
}
