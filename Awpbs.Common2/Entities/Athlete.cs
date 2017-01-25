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
    public class Athlete
    {
        public int AthleteID { get; set; }
        public string Name { get; set; }
        public bool IsPro { get; set; }
        public DateTime? DOB { get; set; }
        public int Gender { get; set; }
        public string Country { get; set; }
        public int MetroID { get; set; }
        public string Picture { get; set; }
        public string SnookerAbout { get; set; }

        // internals
        public string UserName { get; set; }
        public string FacebookId { get; set; }
        public string NameInFacebook { get; set; }
        public string RealEmail { get; set; }
        public string Pin { get; set; }

        public DateTime TimeCreated { get; set; }
        public DateTime TimeModified { get; set; }

		public bool IsAdmin { get { return this.AthleteID == 12 || this.AthleteID == 16; } }

        public virtual List<Result> Results { get; set; }
        public virtual List<Friendship> Friendships1 { get; set; }
        public virtual List<Friendship> Friendships2 { get; set; }
        public virtual List<Score> ScoreAs { get; set; }
        public virtual List<Score> ScoreBs { get; set; }
        public virtual List<VenueEdit> VenueEdits { get; set; }
        public virtual List<GameHost> GameHosts { get; set; }
        public virtual List<GameHostInvite> GameHostInvites { get; set; }
        //public virtual List<GameHostComment> GameHostComments { get; set; }
        public virtual List<DeviceToken> DeviceTokens { get; set; }
        public virtual List<DeviceInfo> DeviceInfos { get; set; }
        public virtual List<Post> Posts { get; set; }

        public bool HasFacebookId { get { return string.IsNullOrEmpty(FacebookId) == false; } }
        public string NameOrUserName { get { return string.IsNullOrEmpty(Name) ? UserName : Name; } }
        public string NameOrUnknown { get { return string.IsNullOrEmpty(Name) ? ("ID" + AthleteID + " (name not shared)") : Name; } }

        public bool IsDifferent(Athlete athlete)
        {
            if (string.Compare(this.Name, athlete.Name, false) != 0)
                return true;
            if (this.IsPro != athlete.IsPro)
                return true;
            if (this.DOB != athlete.DOB)
                return true;
            if (this.Gender != athlete.Gender)
                return true;
            if (string.Compare(this.Country, athlete.Country, true) != 0)
                return true;
            if (this.MetroID != athlete.MetroID)
                return true;
            if (string.Compare(this.FacebookId, athlete.FacebookId, true) != 0)
                return true;
            if (string.Compare(this.NameInFacebook, athlete.NameInFacebook, true) != 0)
                return true;
            if (string.Compare(this.Picture, athlete.Picture, true) != 0)
                return true;
            if (string.Compare(this.SnookerAbout, athlete.SnookerAbout, false) != 0)
                return true;
            return false;
        }

        public Athlete CopyTo(Athlete athleteTo, bool copyInternalToo)
        {
            athleteTo.Name = this.Name;
            athleteTo.IsPro = this.IsPro;
            athleteTo.DOB = this.DOB;
            athleteTo.Gender = this.Gender;
            athleteTo.Country = this.Country;
            athleteTo.MetroID = this.MetroID;
            athleteTo.Picture = this.Picture;
            athleteTo.SnookerAbout = this.SnookerAbout;

            if (copyInternalToo)
            {
                athleteTo.AthleteID = this.AthleteID;
                athleteTo.UserName = this.UserName;
                athleteTo.FacebookId = this.FacebookId;
                athleteTo.NameInFacebook = this.NameInFacebook;
                athleteTo.RealEmail = this.RealEmail;
                athleteTo.Pin = this.Pin;
                athleteTo.TimeCreated = this.TimeCreated;
                athleteTo.TimeModified = this.TimeModified;
            }

            return athleteTo;
        }

        public override string ToString()
        {
            return NameOrUserName;
        }
    }
}
