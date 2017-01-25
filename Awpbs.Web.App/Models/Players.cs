using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web.App.Models
{
    //public class PlayersInCountryModel
    //{
    //    public Country Country { get; set; }

    //    public List<Awpbs.PersonBasicWebModel> Players { get; set; }

    //    public string ImageForSocial
    //    {
    //        get
    //        {
    //            if (Country.IsUSA)
    //                return "placesusa.png";
    //            return "placesuk.png";
    //        }
    //    }
    //}

    public class SnookerBreakModel
    {
        public Awpbs.PersonBasicWebModel Player { get; set; }
        public SnookerBreak Break { get; set; }
    }
}
