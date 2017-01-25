using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Awpbs.Web.App.Models;

namespace Awpbs.Web.App.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;

        public HomeController()
        {
            db = new ApplicationDbContext();
        }

        // this url (routings) is required by apple for Snooker Byb app to claim that the relationship with snookerbyb.com (https://developer.apple.com/library/ios/documentation/General/Conceptual/AppSearch/UniversalLinks.html)
        [HttpGet()]
        public JsonResult AppleAppSiteAssociation()
        {
            var data = new
            {
                applinks = new
                {
                    apps = new List<string>(),
                    details = new []
                    {
                        new {
                            appID = "CUME37443Q.com.bestyourbest.snooker",
                            paths = new []
                            {
                                "/",
                                "*"
                            }
                        }
                    }
                }
            };

            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Test()
        {
            return View("Test");
        }

        public ActionResult Test2()
        {
            return View("Test2");
        }

        public ActionResult AmbassadorProgram()
        {
            return View();
        }

        public ActionResult AllCountries()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View("IndexSnookerByb");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Support()
        {
            return this.RedirectToAction("About");
        }

        public ActionResult Scoreboard()
        {
            return this.View("Scoreboard");
        }

        public ActionResult ForVenues()
        {
            return this.View("ForVenues");
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}