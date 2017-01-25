using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class VenueEdit
    {
        public int VenueEditID { get; set; }
        public int VenueID { get; set; }
        public int AthleteID { get; set; }
        public DateTime Time { get; set; }
        public int Type { get; set; }
        public string Backup { get; set; }

        public VenueEditTypeEnum TypeEnum { get { return (VenueEditTypeEnum)this.Type; } }

        public virtual Venue Venue { get; set; }
        public virtual Athlete Athlete { get; set; }

        public override string ToString()
        {
            return "VenueID=" + VenueID.ToString() + " AthleteID=" + AthleteID + " " + Time.ToShortDateString();
        }
    }
}
