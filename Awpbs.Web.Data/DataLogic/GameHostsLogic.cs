using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class GameHostsLogic
    {
        ApplicationDbContext db;

        public GameHostsLogic(ApplicationDbContext db)
        {
            this.db = db;
        }

        public GameHostWebModel Get(int gameHostID, int myAthleteID)
        {
            //var model = (from gameHost in db.GameHosts
            //             where gameHost.GameHostID == gameHostID
            //             select new GameHostWebModel()
            //             {
            //                 GameHostID = gameHost.GameHostID,
            //                 HostPersonName = gameHost.Athlete.Name,
            //                 HostPersonID = gameHost.Athlete.AthleteID,
            //                 HostPersonPicture = gameHost.Athlete.Picture,
            //                 VenueID = gameHost.Venue.VenueID,
            //                 VenueName = gameHost.Venue.Name,
            //                 //Distance = Distance.Calculate(location, item.Venue.Location).Meters,
            //                 When = gameHost.When,
            //                 AthleteIDs_Invited = (from i in gameHost.GameHostInvites
            //                                       where i.IsApprovedByHost == true
            //                                       select i.AthleteID).ToList(),
            //                 AthleteIDs_Going = (from i in gameHost.GameHostInvites
            //                                     where i.IsApprovedByHost == true && i.IsApprovedByInvitee == true
            //                                     select i.AthleteID).ToList(),
            //                 AthleteIDs_WantToGo = (from i in gameHost.GameHostInvites
            //                                        where i.IsApprovedByHost == false && i.IsApprovedByInvitee == true
            //                                        select i.AthleteID).ToList()
            //             }).FirstOrDefault();

            var query = (from gameHost in db.GameHosts
                         where gameHost.GameHostID == gameHostID
                         select gameHost);

            var list = this.loadListOfGameHostsFromQuery(query, null, myAthleteID);
            if (list.Count == 1)
                return list[0];
            return null;
        }

        public List<GameHostWebModel> FindGameHosts(int myAthleteID, Location location, Distance distance, bool friendsOnly, DateTime whenBegin, DateTime whenEnd)
        {
            double latBegin = -10000;
            double latEnd = 10000;
            double lonBegin = -10000;
            double lonEnd = 10000;
            if (location != null)
            {
                Location location2 = location.OffsetRoughly(distance.Meters, distance.Meters);
                double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
                double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

                latBegin = location.Latitude - latRange;
                latEnd = location.Latitude + latRange;
                lonBegin = location.Longitude - lonRange;
                lonEnd = location.Longitude + lonRange;
            }

            var query = (from gh in db.GameHosts
                         let isFriend = gh.Athlete.Friendships1.Where(f => f.Athlete2ID == myAthleteID).Count() > 0 || gh.Athlete.Friendships2.Where(f => f.Athlete1ID == myAthleteID).Count() > 0
                         where gh.When >= whenBegin && gh.When <= whenEnd
                         where gh.Venue.Latitude > latBegin && gh.Venue.Latitude < latEnd
                         where gh.Venue.Longitude > lonBegin && gh.Venue.Longitude < lonEnd
                         where friendsOnly == false || isFriend == true || gh.AthleteID == myAthleteID
                         orderby gh.When
                         select gh);
            List<GameHostWebModel> gameHosts = this.loadListOfGameHostsFromQuery(query, location, myAthleteID);

            return gameHosts;
        }

        public List<GameHostWebModel> GetMyGameHosts(int athleteID, bool includePast)
        {
            List<GameHostWebModel> gameHosts = new List<GameHostWebModel>();

            DateTime now = DateTime.UtcNow.AddHours(-1);

            // my game hosts
            var query1 = (from gh in db.GameHosts
                          where gh.AthleteID == athleteID
                          where includePast == true || gh.When < now
                          select gh);
            gameHosts.AddRange(this.loadListOfGameHostsFromQuery(query1, null, athleteID));

            // game hosts I've been invited to
            var query2 = (from gh in db.GameHosts
                          from invite in gh.GameHostInvites
                          where invite.AthleteID == athleteID
                          where includePast == true || gh.When < now
                          select gh);
            gameHosts.AddRange(this.loadListOfGameHostsFromQuery(query2, null, athleteID));

            return gameHosts;
        }

        public List<GameHostWebModel> GetGameHostsAtVenue(int venueID, bool includePast, int myAthleteID)
        {
            List<GameHostWebModel> gameHosts = new List<GameHostWebModel>();

            DateTime now = DateTime.UtcNow.AddHours(-1);

            // my game hosts
            var query1 = (from gh in db.GameHosts
                          where gh.VenueID == venueID
                          where includePast == true || gh.When < now
                          select gh);
            gameHosts.AddRange(this.loadListOfGameHostsFromQuery(query1, null, myAthleteID));

            return gameHosts;
        }

        private List<GameHostWebModel> loadListOfGameHostsFromQuery(IQueryable<GameHost> query, Location location, int myAthleteID)
        {
            var list = (from gh in query
                        let commentsCount = gh.Comments.Where(i => i.CommentType == (int)CommentTypeEnum.Comment).Count()
                        let likesCount = gh.Comments.Where(i => i.CommentType == (int)CommentTypeEnum.Like).Count()
                        let liked = gh.Comments.Where(i => i.CommentType == (int)CommentTypeEnum.Like && i.AthleteID == myAthleteID).Count() > 0
                        orderby gh.When
                        select new
                        {
                            GameHost = gh,
                            Comments = gh.Comments.ToList(),
                            Athlete = gh.Athlete,
                            Venue = gh.Venue,
                            Invites = gh.GameHostInvites.ToList(),
                            CommentsCount = commentsCount,
                            LikesCount = likesCount,
                            Liked = liked,
                        }).Take(100).ToList();

            List<GameHostWebModel> gameHosts = new List<GameHostWebModel>();
            foreach (var item in list)
            {
                List<int> ids_invited = (from i in item.Invites
                                         where i.IsApprovedByHost == true
                                         select i.AthleteID).Distinct().ToList();
                List<int> ids_going = (from i in item.Invites
                                       where i.IsApprovedByHost == true && i.IsApprovedByInvitee == true
                                       select i.AthleteID).Distinct().ToList();
                List<int> ids_wanttogo = (from i in item.Invites
                                          where i.IsApprovedByHost == false && i.IsApprovedByInvitee == true
                                          select i.AthleteID).Distinct().ToList();
                List<int> ids_denied = (from i in item.Invites
                                        where i.IsApprovedByHost == true && i.IsDeniedByInvitee == true
                                        select i.AthleteID).Distinct().ToList();

                gameHosts.Add(new GameHostWebModel()
                {
                    GameHostID = item.GameHost.GameHostID,
                    HostPersonName = item.Athlete.Name,
                    HostPersonID = item.Athlete.AthleteID,
                    HostPersonPicture = item.Athlete.Picture,
                    VenueID = item.Venue.VenueID,
                    VenueName = item.Venue.Name,
                    When = item.GameHost.When,
                    Distance = location != null ? Distance.Calculate(location, item.Venue.Location).Meters : 0,
                    AthleteIDs_Invited = ids_invited,
                    AthleteIDs_Going = ids_going,
                    AthleteIDs_CannotGo = ids_denied,
                    AthleteIDs_WantToGo = ids_wanttogo,
                    CommentsCount = item.CommentsCount,
                    LikesCount = item.LikesCount,
                    Liked = item.Liked,
                    EventType = (EventTypeEnum)item.GameHost.EventType,
                    LimitOnNumberOfPlayers = item.GameHost.LimitOnNumberOfPlayers,
                });
            }
            return gameHosts;
        }

        //public List<GameHostWebModel> FindGameHosts(int myAthleteID, Location location, Distance distance, bool friendsOnly, DateTime whenBegin, DateTime whenEnd)
        //{
        //    double latBegin = -10000;
        //    double latEnd = 10000;
        //    double lonBegin = -10000;
        //    double lonEnd = 10000;
        //    if (location != null)
        //    {
        //        Location location2 = location.OffsetRoughly(distance.Meters, distance.Meters);
        //        double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
        //        double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

        //        latBegin = location.Latitude - latRange;
        //        latEnd = location.Latitude + latRange;
        //        lonBegin = location.Longitude - lonRange;
        //        lonEnd = location.Longitude + lonRange;
        //    }

        //    var list = (from gh in db.GameHosts
        //                let isFriend = gh.Athlete.Friendships1.Where(f => f.Athlete2ID == myAthleteID).Count() > 0 || gh.Athlete.Friendships2.Where(f => f.Athlete1ID == myAthleteID).Count() > 0
        //                where gh.When >= whenBegin && gh.When <= whenEnd
        //                where gh.Venue.Latitude > latBegin && gh.Venue.Latitude < latEnd
        //                where gh.Venue.Longitude > lonBegin && gh.Venue.Longitude < lonEnd
        //                where friendsOnly == false || isFriend == true || gh.AthleteID == myAthleteID
        //                orderby gh.When
        //                select new
        //                {
        //                    GameHost = gh,
        //                    Athlete = gh.Athlete,
        //                    Venue = gh.Venue,
        //                    Invites = gh.GameHostInvites.ToList()
        //                }).Take(100).ToList();

        //    List<GameHostWebModel> gameHosts = new List<GameHostWebModel>();
        //    foreach (var item in list)
        //    {
        //        List<int> ids_invited = (from i in item.Invites
        //                                 where i.IsApprovedByHost == true
        //                                 select i.AthleteID).Distinct().ToList();
        //        List<int> ids_going = (from i in item.Invites
        //                               where i.IsApprovedByHost == true && i.IsApprovedByInvitee == true
        //                               select i.AthleteID).Distinct().ToList();
        //        List<int> ids_wanttogo = (from i in item.Invites
        //                                  where i.IsApprovedByHost == false && i.IsApprovedByInvitee == true
        //                                  select i.AthleteID).Distinct().ToList();

        //        gameHosts.Add(new GameHostWebModel()
        //        {
        //            GameHostID = item.GameHost.GameHostID,
        //            HostPersonName = item.Athlete.Name,
        //            HostPersonID = item.Athlete.AthleteID,
        //            HostPersonPicture = item.Athlete.Picture,
        //            VenueID = item.Venue.VenueID,
        //            VenueName = item.Venue.Name,
        //            Distance = location != null ? Distance.Calculate(location, item.Venue.Location).Meters : 0,
        //            When = item.GameHost.When,
        //            AthleteIDs_Invited = ids_invited,
        //            AthleteIDs_Going = ids_going,
        //            AthleteIDs_WantToGo = ids_wanttogo
        //        });
        //    }
        //    return gameHosts;
        //}

        //public List<VenueWebModel> GetVenuesAround(Location location, Distance distance)
        //{
        //    Location location2 = location.OffsetRoughly(distance.Meters, distance.Meters);
        //    double latRange = System.Math.Abs(location2.Latitude - location.Latitude);
        //    double lonRange = System.Math.Abs(location2.Longitude - location.Longitude);

        //    var query = (from i in db.Venues.Include("Metro")
        //                 where i.Latitude > location.Latitude - latRange && i.Latitude < location.Latitude + latRange
        //                 where i.Longitude > location.Longitude - lonRange && i.Longitude < location.Longitude + lonRange
        //                 select VenueWebModel.FromVenue(i, location, null));
        //    var list = query.ToList();
        //    return list;
        //}
    }
}
