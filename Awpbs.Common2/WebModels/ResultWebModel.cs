using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class ResultWebModel
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
        public DateTime TimeModified { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }
        public int? VenueID { get; set; }
        public int? Type1 { get; set; }
        public int? OpponentAthleteID { get; set; }
        public OpponentConfirmationEnum OpponentConfirmation { get; set; }
        public string Details1 { get; set; }

        public static ResultWebModel FromResult(Result result)
        {
            ResultWebModel obj = new ResultWebModel();
            obj.ResultID = result.ResultID;
            obj.AthleteID = result.AthleteID;
            obj.ResultTypeID = result.ResultTypeID;
            obj.Time = result.Time;
            obj.Distance = result.Distance;
            obj.Count = result.Count;
            obj.Count2 = result.Count2;
            obj.Date = result.Date;
            obj.Notes = result.Notes;
            obj.TimeModified = result.TimeModified;
            obj.IsDeleted = result.IsDeleted;
            obj.Guid = result.Guid;
            obj.VenueID = result.VenueID;
            obj.Type1 = result.Type1;
            obj.OpponentAthleteID = result.OpponentAthleteID;
            obj.OpponentConfirmation = (OpponentConfirmationEnum)result.OpponentConfirmation;
            obj.Details1 = result.Details1;
            return obj;
        }

        public Result ToResult()
        {
            Result result = new Result();
            result.ResultID = this.ResultID;
            result.AthleteID = this.AthleteID;
            result.ResultTypeID = this.ResultTypeID;
            result.Time = this.Time;
            result.Distance = this.Distance;
            result.Count = this.Count;
            result.Count2 = this.Count2;
            result.Date = this.Date;
            result.Notes = this.Notes;
            result.TimeModified = this.TimeModified;
            result.IsDeleted = this.IsDeleted;
            result.Guid = this.Guid;
            result.VenueID = this.VenueID;
            result.Type1 = this.Type1;
            result.OpponentAthleteID = this.OpponentAthleteID;
            result.OpponentConfirmation = (int)this.OpponentConfirmation;
            result.Details1 = this.Details1;
            return result;
        }
    }
}
