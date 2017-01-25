using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/Metros")]
    public class MetrosController : ApiController
    {
        private ApplicationDbContext db;

        public MetrosController()
        {
            this.db = new ApplicationDbContext();
        }

        public MetroWebModel Get(int id)
        {
            var metro = new MetrosLogic(db).Get(id);
            return metro;
        }

        public IEnumerable<MetroWebModel> Get(string country)
        {
            Country countryObj = Country.Get(country);
            var metros = new MetrosLogic(db).GetMetrosInCountry(countryObj);
            return metros;
        }

        [HttpGet]
        [Route("Closest")]
        public IEnumerable<MetroWebModel> Closest(double latitude, double longitude)
        {
            var metros = new MetrosLogic(db).GetMetrosAround(new Location(latitude, longitude));
            return metros;
        }
    }
}