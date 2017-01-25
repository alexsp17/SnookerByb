//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Awpbs.Web.App.Models
//{
//    public class CountryModel
//    {
//        public Country Country { get; set; }
//        public int CountPlaces { get; set; }
//    }

//    public class PlacesModel
//    {
//        public List<CountryModel> Rank1Countries { get; set; }
//        public List<CountryModel> Rank2Countries { get; set; }
//        public List<CountryModel> AllCountries { get; set; }
//    }

//    public class PlacesMetroModel
//    {
//        public Metro Metro { get; set; }
//        public string MetroNameEncoded { get { return Metro.Country + "--" + Metro.Name; } }
//    }

//    public class PlacesInCountryModel
//    {
//        public Country Country { get; set; }
//        public List<PlacesMetroModel> Metros { get; set; }

//        public int DefaultMapZoom { get; set; }
//        public Location CountryLocation { get; set; }

//        public string ImageForFacebook
//        {
//            get
//            {
//                if (Country.IsBritain)
//                    return "http://www.snookerbyb.com/images/placesuk.png";
//                if (Country.IsUSA)
//                    return "http://www.snookerbyb.com/images/placesusa.png";
//                return "";
//            }
//        }
//    }

//    public class PlacesInMetroModel
//    {
//        public int MapZoom
//        {
//            get
//            {
//                if (VenuesWithin20miles.Count > 0)
//                    return 10;
//                return 7;
//            }
//        }

//        public Metro Metro { get; set; }
//        public List<VenueWebModel> Venues { get; set; }

//        public List<VenueWebModel> VenuesWithin20miles
//        {
//            get
//            {
//                return (from v in Venues
//                        where v.DistanceInMeters <= 20*1600
//                        orderby v.DistanceInMeters, v.Name
//                        select v).ToList();
//            }
//        }

//        public List<VenueWebModel> VenuesFurtherAway
//        {
//            get
//            {
//                return (from v in Venues
//                        where v.DistanceInMeters > 20 * 1600
//                        orderby v.DistanceInMeters, v.Name
//                        select v).ToList();
//            }
//        }
//    }

//    //public class PlaceModel
//    //{
//    //    public Venue Venue { get; set; }
//    //    public Distance Distance { get; set; }
//    //}
//}
