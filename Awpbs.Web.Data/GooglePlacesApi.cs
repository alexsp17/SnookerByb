using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Awpbs.Web
{
    /// <summary>
    /// This is a wrapper around "Google Place API"
    /// </summary>
    public class GooglePlacesApi
    {
        public class googlePlacesResponseResultLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class googlePlacesResponseResultGeometry
        {
            public googlePlacesResponseResultLocation location { get; set; }
        }

        public class googlePlacesResponseResult
        {
            public googlePlacesResponseResultGeometry geometry { get; set; }
            public string name { get; set; }
            public string place_id { get; set; }
        }

        public class googlePlacesResponse
        {
            public List<googlePlacesResponseResult> results { get; set; }
            public string status { get; set; }
        }

        public GooglePlacesApi()
        {
            this.googleApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["GooglePlacesAPIkey"];
        }

        private string googleApiKey;

        public async Task<List<POIWebModel>> Search(Location location, double radiusInMeters, string keyword)
        {
            try
            {
                /// Ask for places
                /// 
                List<POIWebModel> pois = new List<POIWebModel>();
                if (true)
                {
                    string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json" +
                        "?key=" + googleApiKey +
                        "&location=" + location.Latitude.ToString() + "," + location.Longitude.ToString() +
                        "&radius=" + radiusInMeters.ToString();
                    if (string.IsNullOrEmpty(keyword) == false)
                        url += "&keyword=" + HttpUtility.UrlEncode(keyword);
                    var request = HttpWebRequest.Create(url);
                    request.Timeout = 5000;
                    request.Method = "GET";

                    HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                    string json;
                    using (Stream responseStream = httpResponse.GetResponseStream())
                    {
                        json = new StreamReader(responseStream).ReadToEnd();
                    }
                    googlePlacesResponse response = JsonConvert.DeserializeObject<googlePlacesResponse>(json);

                    pois = (from i in response.results
                            let loc = new Location() { Latitude = i.geometry.location.lat, Longitude = i.geometry.location.lng }
                            select new POIWebModel
                            {
                                PoiID = i.place_id,
                                Name = i.name,
                                Location = loc,
                                Distance = Distance.Calculate(location, loc)
                            }).Take(30).ToList();
                }

                /// Query each place for the address
                /// 
                foreach (var poi in pois)
                {
                    string url = "https://maps.googleapis.com/maps/api/place/details/json" +
                        "?key=" + googleApiKey +
                        "&placeid=" + poi.PoiID;
                    var request = HttpWebRequest.Create(url);
                    request.Timeout = 5000;
                    request.Method = "GET";

                    HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                    string json;
                    using (Stream responseStream = httpResponse.GetResponseStream())
                    {
                        json = new StreamReader(responseStream).ReadToEnd();
                    }
                    googlePlacesDetailsResponse response = JsonConvert.DeserializeObject<googlePlacesDetailsResponse>(json);

                    poi.Address = response.result.formatted_address;
                    poi.Phone = response.result.formatted_phone_number;
                    poi.Website = response.result.website;
                }

                return pois;
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to call into Google Place API", exc);
            }
        }

        public class googlePlacesDetailsResponse
        {
            public string status { get; set; }
            public googlePlacesDetailsResponse_Result result { get; set; }
        }

        public class googlePlacesDetailsResponse_Result
        {
            public string formatted_address { get; set; }
            public string formatted_phone_number { get; set; }
            public string website { get; set; }
        }
    }
}
