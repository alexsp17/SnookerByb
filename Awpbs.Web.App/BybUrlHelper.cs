using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Awpbs.Web.App
{
    public enum CommunityPageEnum
    {
        Default = 0,
        Feed = 1,
        Players = 2,
        Venues = 3,
    }

    public static class BybUrlHelper
    {
        /// <summary>
        /// =true if according to the URL the page is running inside an IFrame
        /// </summary>
        public static bool IsInIFrame { get; set; }

        public static string BuildUrl(MetroWebModel metro, CommunityPageEnum page = CommunityPageEnum.Default)
        {
            Country countryObj = Country.Get(metro.Country);
            string countryStr = countryObj != null ? countryObj.UrlName : "unknown";

            string url = "/" + countryStr + "/" + HttpUtility.UrlEncode(metro.UrlName);
            if (page == CommunityPageEnum.Players)
                url += "/players";
            else if (page == CommunityPageEnum.Venues)
                url += "/venues";

            if (IsInIFrame)
                url += "?IsInIFrame=true";
            return url.ToLower();
        }

        public static string BuildUrl(string country, CommunityPageEnum page = CommunityPageEnum.Default)
        {
            Country countryObj = Country.Get(country);
            return BuildUrl(countryObj, page);
        }

        public static string BuildUrl(Country country, CommunityPageEnum page = CommunityPageEnum.Default)
        {
            string countryStr = country != null ? country.UrlName : "unknown";

            string url = "/" + countryStr;
            if (page == CommunityPageEnum.Players)
                url += "/players";
            else if (page == CommunityPageEnum.Venues)
                url += "/venues";

            if (IsInIFrame)
                url += "?IsInIFrame=true";
            return url.ToLower();
        }

        public static string BuildPlayerProfileUrl(int athleteID)
        {
            string url = "/players/" + athleteID;

            if (IsInIFrame)
                url += "?IsInIFrame=true";
            return url;
        }

        public static string BuildVenueProfileUrl(int venueID)
        {
            string url = "/venues/" + venueID;

            if (IsInIFrame)
                url += "?IsInIFrame=true";
            return url;
        }
    }
}