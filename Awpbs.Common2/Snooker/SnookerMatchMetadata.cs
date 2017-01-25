using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class SnookerMatchMetadata
    {
        public DateTime Date { get; set; }

        // primary athlete
        public int PrimaryAthleteID { get; set; }
        public string PrimaryAthleteName { get; set; }
        public string PrimaryAthletePicture { get; set; }

        // opponent
        public int OpponentAthleteID { get; set; }
        public string OpponentAthleteName { get; set; }
        public string OpponentPicture { get; set; }

        // venue
        public int VenueID { get; set; }
        public string VenueName { get; set; }

        // table
        public SnookerTableSizeEnum TableSize { get; set; }

        public bool HasPrimaryAthlete {  get { return this.PrimaryAthleteID > 0; } }
        public bool HasOpponentAthlete { get { return this.OpponentAthleteID > 0; } }
        public bool HasVenue {  get { return this.VenueID > 0; } }
    }
}
