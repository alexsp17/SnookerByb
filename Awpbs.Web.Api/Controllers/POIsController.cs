using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/pois")]
    public class POIsController : ApiController
    {
        private ApplicationDbContext db;

        public POIsController()
        {
            this.db = new ApplicationDbContext();
        }

        [HttpGet]
        public async Task<List<POIWebModel>> Get(double lat, double lon, double radiusInMeters, string keyword = "")
        {
            GooglePlacesApi api = new GooglePlacesApi();
            var list = await api.Search(new Location(lat, lon), radiusInMeters, keyword);
            return list;
        }

        [Route("LocationFromAddress")]
        [HttpGet]
        public async Task<Location> LocationFromAddress(string address, double? approximateLat = null, double? approximateLon = null)
        {
            Location approximateLocation = null;
            if (approximateLat != null && approximateLon != null)
                approximateLocation = new Location(approximateLat.Value, approximateLon.Value);

            GoogleGeocodingApi api = new GoogleGeocodingApi();
            var location = await api.LocationFromAddress(address, approximateLocation);
            return location;
        }
    }
}