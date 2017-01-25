using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class GameHost
    {
        public int GameHostID { get; set; }
        public int AthleteID { get; set; }
        public int? VenueID { get; set; }
        public DateTime When { get; set; }
        public DateTime When_InLocalTimeZone { get; set; }
        public int Visibility { get; set; } // obsolete
        public DateTime TimeCreated { get; set; }
        public int EventType { get; set; }
        public int LimitOnNumberOfPlayers { get; set; }

        public virtual Athlete Athlete { get; set; }
        public virtual Venue Venue { get; set; }

        public virtual List<GameHostInvite> GameHostInvites { get; set; }
        public virtual List<Comment> Comments { get; set; }
        //public virtual List<GameHostComment> GameHostComments { get; set; }
    }

    public class GameHostInvite
    {
        public int GameHostInviteID { get; set; }
        public int GameHostID { get; set; }
        public int AthleteID { get; set; }
        public DateTime TimeCreated { get; set; }
        public bool IsApprovedByHost { get; set; }
        public bool IsApprovedByInvitee { get; set; }
        public bool IsConfirmedByHost { get; set; }
        public bool IsDeniedByInvitee { get; set; }

        public virtual GameHost GameHost { get; set; }
        public virtual Athlete Athlete { get; set; }
    }

    //public class GameHostComment
    //{
    //    public int GameHostCommentID { get; set; }
    //    public int GameHostID { get; set; }
    //    public int AthleteID { get; set; }
    //    public DateTime TimeCreated { get; set; }

    //    public virtual GameHost GameHost { get; set; }
    //    public virtual Athlete Athlete { get; set; }
    //}
}
