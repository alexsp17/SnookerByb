using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class PersonBasicWebModel
    {
        /// <summary>
        /// Maximum number items returned by the API
        /// </summary>
        public const int MaxItems = 100;

        public int ID { get; set; }
        public string Name { get; set; }
        public int MetroID { get; set; }
        public string Metro { get; set; }
        public DateTime? DOB { get; set; }
        public GenderEnum Gender { get; set; }
        public string Picture { get; set; }
        public DateTime TimeCreated { get; set; }

        public bool IsFriend { get; set; }
        public bool IsFriendRequestSent { get; set; }

        public string SnookerAbout { get; set; }

        public bool HasMetro
        {
            get { return string.IsNullOrEmpty(Metro) == false; }
        }

        public void CopyTo(PersonBasicWebModel person)
        {
            person.ID = this.ID;
            person.Name = this.Name;
            person.MetroID = this.MetroID;
            person.Metro = this.Metro;
            person.DOB = this.DOB;
            person.Gender = this.Gender;
            person.Picture = this.Picture;
            person.TimeCreated = this.TimeCreated;
            person.IsFriend = this.IsFriend;
            person.IsFriendRequestSent = this.IsFriendRequestSent;
            person.SnookerAbout = this.SnookerAbout;
        }

		public void CopyFrom(Athlete athlete)
		{
			this.ID = athlete.AthleteID;
			this.Name = athlete.Name;
			this.MetroID = athlete.MetroID;
			this.DOB = athlete.DOB;
			this.Gender = (GenderEnum)athlete.Gender;
			this.Picture = athlete.Picture;
			this.TimeCreated = athlete.TimeCreated;
			this.SnookerAbout = athlete.SnookerAbout;
		}

        public override string ToString()
        {
            return this.Name;
        }
    }

    public class PersonFullWebModel : PersonBasicWebModel
    {
        public SnookerStatsModel SnookerStats { get; set; }

        public bool? IsFriendRequestSentByThisPerson { get; set; } // added 1/25/2016
    }

    public class SnookerStatsModel
    {
        public int CountBests { get; set; }
        public int CountMatches { get; set; }
        public int CountOpponentsPlayed { get; set; }
        public int CountVenuesPlayed { get; set; }
        public int CountContributions { get; set; }

        public int BestPoints { get; set; }
        public int BestBallCount { get; set; }
        public int BestFrameScore { get; set; }
    }
}
