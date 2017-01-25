using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Awpbs.Web.App.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext db;

        public VenuesController()
        {
            db = new ApplicationDbContext();
        }

        // GET: /places/venue/id
        public ActionResult Venue(int id, bool isInIFrame = false)
        {
            BybUrlHelper.IsInIFrame = isInIFrame;

            var venue = new VenuesLogic(db).Get(id);
            return View(venue);
        }
   }
}