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
    public class Result
    {
        public int ResultID { get; set; }
        public int AthleteID { get; set; }
        public int ResultTypeID { get; set; }
        public double? Time { get; set; }
        public double? Distance { get; set; }
        public int? Count { get; set; }
        public int? Count2 { get; set; }
        public DateTime? Date { get; set; }
        public string Notes { get; set; }
        public int? VenueID { get; set; }
        public int? Type1 { get; set; }
        public string Details1 { get; set; }
        public int? OpponentAthleteID { get; set; }
        public int OpponentConfirmation { get; set; }
        public bool IsNotAcceptedByAthleteYet { get; set; }

        public DateTime TimeModified { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public virtual Athlete Athlete { get; set; }
        public virtual ResultType ResultType { get; set; }
        public virtual Venue Venue { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public bool IsDifferent(Result result)
        {
            if (this.AthleteID != result.AthleteID)
                return true;
            if (this.ResultTypeID != result.ResultTypeID)
                return true;
            if (this.Time != result.Time)
                return true;
            if (this.Distance != result.Distance)
                return true;
            if (this.Count != result.Count)
                return true;
            if (this.Count2 != result.Count2)
                return true;
            if (Date == null && result.Date != null)
                return true;
            if (Date != null && result.Date == null)
                return true;
            if (Date != null && result.Date != null && System.Math.Abs((Date.Value - result.Date.Value).TotalSeconds) > 1)
                return true;
            if (this.Date != result.Date)
                return true;
            if (string.Compare(this.Notes, result.Notes, false) != 0)
                return true;
            if (this.VenueID != result.VenueID)
                return true;
            if (this.Type1 != result.Type1)
                return true;
            if (string.Compare(this.Details1, result.Details1, false) != 0)
                return true;
            if (this.OpponentAthleteID != result.OpponentAthleteID)
                return true;
            if (this.OpponentConfirmation != result.OpponentConfirmation)
                return true;
            if (this.IsDeleted != result.IsDeleted)
                return true;
            if (this.IsNotAcceptedByAthleteYet != result.IsNotAcceptedByAthleteYet)
                return true;
            
            return false;
        }

        public override string ToString()
        {
            string str = string.Format("ResultID={0}, AthleteID={1}, ResultTypeID={2}, Time={3}", ResultID, AthleteID, ResultTypeID, Time);
            return str;
        }
    }
}
