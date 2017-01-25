using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Awpbs.Web.App
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapRoute("universal-links", new Route("apple-app-site-association", new FileRout "~/content/apple-app-site-association.json");
            routes.MapRoute("universal-links", "apple-app-site-association", new { controller = "Home", action = "AppleAppSiteAssociation" });

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("AboutRoute", "about", new { controller = "Home", action = "About" });
            routes.MapRoute("AboutRoute2", "team", new { controller = "Home", action = "About" });
            routes.MapRoute("SupportRoute", "support", new { controller = "Home", action = "Support" });
            routes.MapRoute("AmbassadorProgramRoute2", "ap", new { controller = "Home", action = "AmbassadorProgram" });
            routes.MapRoute("AmbassadorProgramRoute", "AmbassadorProgram", new { controller = "Home", action = "AmbassadorProgram" });
            routes.MapRoute("Scoreboard", "scoreboard", new { controller = "Home", action = "Scoreboard" });
            routes.MapRoute("ForVenues", "forvenues", new { controller = "Home", action = "ForVenues" });

            // players
            routes.MapRoute(
                name: "Players",
                url: "players/{id}",
                defaults: new { controller = "Players", action = "SnookerPlayer", id = 0 }
            );

            // venues
            routes.MapRoute(
                name: "Venues",
                url: "venues/{id}",
                defaults: new { controller = "Venues", action = "Venue", id = 0 }
            );

            // invites
            routes.MapRoute(
                name: "Invites",
                url: "invites/{id}",
                defaults: new { controller = "Invites", action = "Invite", id = 0 }
            );

            // for all important countries
            var countries = Country.List.Where(i => i.Snooker != CountryImportanceEnum.Importance0).ToList();
            foreach (var country in countries)
            {
                string countryName = country.UrlName;
                routes.MapRoute(
                    name: "CommunitiesRouteCountry" + countryName,
                    url: countryName + "/{parameter1}/{parameter2}",
                    defaults: new { controller = "Community", action = "Main", country = country.ThreeLetterCode, parameter1 = "", parameter2 = "" }
                );
            }

            // default route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
