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
    public class GoogleGeocodingApi
    {
        public class googleGeocodingResponseResultGeometryLocation
        {
            public double lat { get; set; }
            public double lng { get; set; }
        }

        public class googleGeocodingResponseResultGeometry
        {
            public googleGeocodingResponseResultGeometryLocation location { get; set; }
        }

        public class googleGeocodingResponseResult
        {
            public string formatted_address { get; set; }
            public googleGeocodingResponseResultGeometry geometry { get; set; }
        }

        public class googleGeocodingResponse
        {
            public List<googleGeocodingResponseResult> results { get; set; }
            public string status { get; set; }
        }

        public GoogleGeocodingApi()
        {
            this.googleApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleGeocodingAPIkey"];
        }

        private string googleApiKey;

        public async Task<Location> LocationFromAddress(string address, Location approximateLocation = null)
        {
            if (string.IsNullOrEmpty(address) || address.Trim().Length == 0)
                throw new Exception("address is empty");
            if (address.Length > 512)
                throw new Exception("address is too long");

            try
            {
                string url = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(address);
                if (approximateLocation != null)
                {
                    url += "&bounds=" + (approximateLocation.Latitude - 1).ToString("F4") + "," + (approximateLocation.Longitude - 1).ToString("F4") +
                        "|" + (approximateLocation.Latitude + 1).ToString("F4") + "," + (approximateLocation.Longitude + 1).ToString("F4");
                }
                url += "&key=" + HttpUtility.UrlEncode(googleApiKey);
                var request = HttpWebRequest.Create(url);
                request.Timeout = 5000;
                request.Method = "GET";
                
                HttpWebResponse httpResponse = (HttpWebResponse)(await request.GetResponseAsync());
                string json;
                using (Stream responseStream = httpResponse.GetResponseStream())
                {
                    json = new StreamReader(responseStream).ReadToEnd();
                }
                googleGeocodingResponse response = JsonConvert.DeserializeObject<googleGeocodingResponse>(json);

                double lat = response.results[0].geometry.location.lat;
                double lon = response.results[0].geometry.location.lng;
                return new Location(lat, lon);
            }
            catch (Exception exc)
            {
                throw new Exception("Failed to call into Google Geocoding API", exc);
            }
        }

        public void SleepToAvoidGoogleApiThreshold()
        {
            // to avoid the Google API requests/second limit
            Thread.Sleep(70);
        }
    }
}
