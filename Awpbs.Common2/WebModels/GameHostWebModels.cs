using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class NewGameHostWebModel
    {
        public int VenueID { get; set; }
        public DateTime When { get; set; }
        public DateTime When_InLocalTimeZone { get; set; }
        public int Visibility { get; set; }
    }

    public class NewGameHostWebModel2
    {
        public int VenueID { get; set; }
        public DateTime When { get; set; }
        public DateTime When_InLocalTimeZone { get; set; }
        public EventTypeEnum EventType { get; set; }
        public int LimitOnNumberOfPlayers { get; set; }
        public string Comments { get; set; }

        public List<int> Invitees { get; set; }
    }

    public class NewGameHostInviteWebModel
    {
        public int GameHostID { get; set; }
        public int AthleteID { get; set; }
        public DateTime When_InLocalTimeZone { get; set; }
    }

    public class GameHostWebModel
    {
        public int GameHostID { get; set; }
        public string HostPersonName { get; set; }
        public int HostPersonID { get; set; }
        public string HostPersonPicture { get; set; }
        public string VenueName { get; set; }
        public int VenueID { get; set; }
        public double Distance { get; set; }
        public DateTime When { get; set; }

        public int CommentsCount { get; set; }
        public int LikesCount { get; set; }
        public bool Liked { get; set; }

        public EventTypeEnum EventType { get; set; }
        public int LimitOnNumberOfPlayers { get; set; }

        public List<int> AthleteIDs_Invited { get; set; }
        public List<int> AthleteIDs_WantToGo { get; set; }
        public List<int> AthleteIDs_Going { get; set; }
        public List<int> AthleteIDs_CannotGo { get; set; }

        public bool IsAthleteIncluded(int id)
        {
            if (IsInvited(id))
                return true;
            if (IsGoing(id))
                return true;
            if (IsCannotGo(id))
                return true;
            if (IsWantToGo(id))
                return true;
            return false;
        }

        public bool IsInvited(int id)
        {
            if (AthleteIDs_Invited == null)
                return false;
            return AthleteIDs_Invited.Contains(id);
        }

        public bool IsGoing(int id)
        {
            if (AthleteIDs_Going == null)
                return false;
            return AthleteIDs_Going.Contains(id);
        }

        public bool IsCannotGo(int id)
        {
            if (AthleteIDs_CannotGo == null)
                return false;
            return AthleteIDs_CannotGo.Contains(id);
        }

        public bool IsWantToGo(int id)
        {
            if (AthleteIDs_WantToGo == null)
                return false;
            return AthleteIDs_WantToGo.Contains(id);
        }

        public List<int> GetAthleteIDs_Unresponded()
        {
            List<int> ids = new List<int>();
            if (AthleteIDs_Invited != null)
                ids.AddRange(AthleteIDs_Invited);
            if (AthleteIDs_Going != null)
                foreach (var id in AthleteIDs_Going)
                    if (ids.Contains(id))
                        ids.Remove(id);
            if (AthleteIDs_CannotGo != null)
                foreach (var id in AthleteIDs_CannotGo)
                    if (ids.Contains(id))
                        ids.Remove(id);
            return ids;
        }
    }
}
