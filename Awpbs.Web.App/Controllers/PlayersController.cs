using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Awpbs.Web.App.Controllers
{
    public class PlayersController : Controller
    {
        private readonly ApplicationDbContext db;

        public PlayersController()
        {
            db = new ApplicationDbContext();
        }

        // GET: Players
        public ActionResult Index()
        {
            return View();
        }

        // GET: /players/country/countrycode
        //public ActionResult Country(string id)
        //{
        //    Country country = Awpbs.Country.Get(id);
        //    if (country == null)
        //        throw new Exception("unknown country - " + id);

        //    Models.PlayersInCountryModel model = new Models.PlayersInCountryModel();
        //    model.Country = country;
        //    model.Players = new PeopleLogic(db).Find(0, "", false, country, null);

        //    return View("Country", model);
        //}

        // GET: SnookerPlayer
        public ActionResult SnookerPlayer(int id, bool isInIFrame = false)
        {
            BybUrlHelper.IsInIFrame = isInIFrame;

            var person = new PeopleLogic(db).GetFull(0, id);

            int bestResultID = 0;
            var res = db.Results.Where(i => i.AthleteID == id && i.Count != null).OrderByDescending(i => i.Count.Value).FirstOrDefault();
            if (res != null)
                bestResultID = res.ResultID;
            ViewBag.BestResultID = bestResultID;

            return View("SnookerPlayer", person);
        }

        public ActionResult MiniSnookerProfile(int athleteID, string extraText = "", string nameIfNotFound = "Not found")
        {
            var person = new PeopleLogic(db).GetBasic(0, athleteID);

            string picture = person != null ? person.Picture : null;
            if (string.IsNullOrEmpty(picture) == false)
                picture = ImageUrlHelper.MakeUrlForWebProfile(picture);
            else
                picture = new Uri("/images/default-snookerplayer.png", UriKind.Relative).ToString();

            ViewBag.Picture = picture;

            ViewBag.ExtraText = extraText;

            if (person == null)
            {
                person = new PersonBasicWebModel();
                person.Name = Url.Encode(nameIfNotFound);
                return PartialView("MiniSnookerProfile", person);
            }

            return PartialView("MiniSnookerProfile", person);
        }

        // GET: /players/break/id
        public ActionResult Break(int id)
        {
            var result = db.Results.Where(i => i.ResultID == id).Single();
            var person = new PeopleLogic(db).GetBasic(0, result.AthleteID);

            Models.SnookerBreakModel model = new Models.SnookerBreakModel();
            model.Break = SnookerBreak.FromResult(result);
            model.Player = person;

            return View("Break", model);
        }
    }
}