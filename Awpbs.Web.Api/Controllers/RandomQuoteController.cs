using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Awpbs.Web.Api.Controllers
{
    [RoutePrefix("api/RandomQuote")]
    public class RandomQuoteController : ApiController
    {
        public Quote Get()
        {
            var db = new ApplicationDbContext();
            var quote = new QuotesLogic(db).GetRandom(SportEnum.Unknown);
            db.Dispose();
            return quote;
        }

        [HttpGet]
        [Route("Snooker", Name = "Snooker")]
        public Quote Snooker()
        {
            var db = new ApplicationDbContext();
            var quote = new QuotesLogic(db).GetRandomForSnooker();
            db.Dispose();
            return quote;
        }
    }
}
