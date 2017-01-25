using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Awpbs.Web.App.Models
{
    public class CommunityModel
    {
        public bool IsPlayers
        {
            get
            {
                return this as CommunityPlayersModel != null;
            }
        }

        public bool IsVenues
        {
            get
            {
                return this as CommunityVenuesModel != null;
            }
        }

        public bool IsFeed
        {
            get
            {
                return this as CommunityFeedModel != null;
            }
        }

        public Country Country { get; set; }
        public MetroWebModel Metro { get; set; }
        public List<MetroWebModel> Metros { get; set; }

        //public string Url
        //{
        //    get
        //    {
        //        //string url = "/" + Country.UrlName;
        //        //if (Metro != null)
        //        //    url += "/" + HttpUtility.UrlEncode(Metro.UrlName).ToLower();
        //        //return url;
        //        if (Metro)
        //    }
        //}

        public string UrlVenues
        {
            get
            {
                return Metro != null ? BybUrlHelper.BuildUrl(Metro, CommunityPageEnum.Venues) : BybUrlHelper.BuildUrl(Country.ThreeLetterCode, CommunityPageEnum.Venues);
            }
        }

        public string UrlPlayers
        {
            get
            {
                return Metro != null ? BybUrlHelper.BuildUrl(Metro, CommunityPageEnum.Players) : BybUrlHelper.BuildUrl(Country.ThreeLetterCode, CommunityPageEnum.Players);
            }
        }

        public string UrlFeed
        {
            get
            {
                return Metro != null ? BybUrlHelper.BuildUrl(Metro, CommunityPageEnum.Feed) : BybUrlHelper.BuildUrl(Country.ThreeLetterCode, CommunityPageEnum.Feed);
            }
        }

        public string CommunityName
        {
            get
            {
                if (Metro != null)
                    return Metro.Name + ", " + Country.ThreeLetterCode;
                return Country.LocalizedName;
            }
        }

        public string CommunityName2
        {
            get
            {
                if (Metro != null)
                    return Metro.Name + ", " + Country.LocalizedName;
                return Country.LocalizedName;
            }
        }
    }

    public class CommunityVenuesModel : CommunityModel
    {
        public List<VenueWebModel> Venues { get; set; }

        public int DefaultMapZoom { get; set; }
        public Location CountryLocation { get; set; }

        public Location MapCenterLocation
        {
            get
            {
                if (this.Metro == null)
                    return this.CountryLocation;
                else
                    return this.Metro.Location;
            }
        }

        public string ImageForFacebook
        {
            get
            {
                if (Country.IsBritain)
                    return "http://www.snookerbyb.com/images/placesuk.png";
                if (Country.IsUSA)
                    return "http://www.snookerbyb.com/images/placesusa.png";
                return "";
            }
        }
    }

    public class CommunityPlayersModel : CommunityModel
    {
        public List<Awpbs.PersonBasicWebModel> Players { get; set; }

        public string ImageForSocial
        {
            get
            {
                if (Country.IsUSA)
                    return "placesusa.png";
                return "placesuk.png";
            }
        }
    }

    public class CommunityFeedModel : CommunityModel
    {
        public List<NewsfeedItemWebModel> Items { get; set; }
    }
}
