using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Awpbs.Web.App.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ApplicationDbContext db;

        public CommunityController()
        {
            db = new ApplicationDbContext();
        }

        void parseCountryAndMetro(string country, string metro, out Country countryObj, out MetroWebModel metroObj)
        {
            // country
            countryObj = Country.Get(country);
            if (countryObj == null)
                throw new Exception("Unknown country - " + country);

            // metro
            metroObj = null;
            if (string.IsNullOrEmpty(metro) == false && metro.ToLower() != "all")
            {
                metroObj = new MetrosLogic(db).Get(countryObj.ThreeLetterCode, metro);
                if (metroObj == null)
                    throw new Exception("Unknown metro - " + metro);
            }
        }

        public ActionResult Main(string country, string parameter1, string parameter2, bool isInIFrame = false)
        {
            parameter1 = parameter1.ToLower();
            parameter2 = parameter2.ToLower();

            if (parameter2 == "players")
                return Players(country, parameter1, isInIFrame);
            if (parameter2 == "" && parameter1 == "players")
                return Players(country, "", isInIFrame);

            if (parameter2 == "venues")
                return Venues(country, parameter1, isInIFrame);
            if (parameter2 == "" && parameter1 == "venues")
                return Venues(country, "", isInIFrame);

            return Feed(country, parameter1, isInIFrame);
        }

        public ActionResult Feed(string country, string metro, bool isInIFrame)
        {
            BybUrlHelper.IsInIFrame = isInIFrame;

            Country countryObj;
            MetroWebModel metroObj;
            parseCountryAndMetro(country, metro, out countryObj, out metroObj);

            var model = new Models.CommunityFeedModel();
            model.Country = countryObj;
            model.Metro = metroObj;
            model.Metros = new MetrosLogic(db).GetMetrosInCountry(countryObj);

            NewsfeedWebModel newsfeed;
            if (metroObj == null)
                newsfeed = new FeedLogic(db).GetNewsfeedForCountry(0, countryObj);
            else
                newsfeed = new FeedLogic(db).GetNewsfeedForMetro(0, metroObj.ID);
            model.Items = newsfeed.Items;

            ViewBag.isInIFrame = isInIFrame;
            return View("Feed", model);
        }

        public ActionResult Players(string country, string metro, bool isInIFrame)
        {
            BybUrlHelper.IsInIFrame = isInIFrame;

            Country countryObj;
            MetroWebModel metroObj;
            parseCountryAndMetro(country, metro, out countryObj, out metroObj);

            Models.CommunityPlayersModel model = new Models.CommunityPlayersModel();
            model.Country = countryObj;
            model.Metro = metroObj;
            model.Metros = new MetrosLogic(db).GetMetrosInCountry(countryObj);

            if (metroObj == null)
                model.Players = new PeopleLogic(db).Find(0, "", false, countryObj, null);
            else
                model.Players = new PeopleLogic(db).Find(0, "", false, countryObj, metroObj.ID);

            return View("Players", model);
        }

        public ActionResult Venues(string country, string metro, bool isInIFrame)
        {
            BybUrlHelper.IsInIFrame = isInIFrame;

            Country countryObj;
            MetroWebModel metroObj;
            parseCountryAndMetro(country, metro, out countryObj, out metroObj);

            Models.CommunityVenuesModel model = new Models.CommunityVenuesModel();
            model.Country = countryObj;
            model.Metro = metroObj;
            model.Metros = new MetrosLogic(db).GetMetrosInCountry(countryObj);

            if (metroObj == null)
                model.Venues = new VenuesLogic(db).GetAllVenuesInCountry(countryObj.ThreeLetterCode);
            else
                model.Venues = new VenuesLogic(db).FindVenues(metroObj.Location, true, "", true, false, false);

            model.CountryLocation = new Location();
            model.CountryLocation.Latitude = model.Metros.Average(i => i.Latitude);
            model.CountryLocation.Longitude = model.Metros.Average(i => i.Longitude);

            if (model.Metro != null)
            {
                model.DefaultMapZoom = 10;
            }
            else if (model.Metros.Count > 0)
            {
                if (model.Country.AreaSize == CountrySizeEnum.VeryLarge)
                    model.DefaultMapZoom = 4;
                else
                    model.DefaultMapZoom = 6;
            }
            else
            {
                model.DefaultMapZoom = 2;
            }

            return View("Venues", model);
        }
    }
}