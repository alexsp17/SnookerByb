using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class FriendshipLogic
    {
        private readonly ApplicationDbContext db;

        public FriendshipLogic(ApplicationDbContext context)
        {
            this.db = context;
        }

        public IQueryable<Athlete> GetFriendsQuery(int athleteID, bool confirmedOnly)
        {
            if (confirmedOnly == false)
            {
                var query = (from i in db.Athletes
                             where i.Friendships1.Where(f => f.Athlete2ID == athleteID).Count() > 0 || i.Friendships2.Where(f => f.Athlete1ID == athleteID).Count() > 0
                             select i);
                return query;
            }
            else
            {
                var query = (from i in db.Athletes
                             where i.Friendships1.Where(f => f.Athlete2ID == athleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Confirmed).Count() > 0 || i.Friendships2.Where(f => f.Athlete1ID == athleteID && f.FriendshipStatus == (int)FriendshipStatusEnum.Confirmed).Count() > 0
                             select i);
                return query;
            }
        }

        public List<Athlete> GetFriends(int athleteID, bool confirmedOnly)
        {
            return this.GetFriendsQuery(athleteID, confirmedOnly).ToList();
        }

        public Friendship GetFriendship(int athleteID1, int athleteID2)
        {
            var existingFriendship = (from f in db.Friendships
                                      where (f.Athlete1ID == athleteID1 && f.Athlete2ID == athleteID2) || (f.Athlete1ID == athleteID2 && f.Athlete2ID == athleteID1)
                                      where f.FriendshipStatus != (int)FriendshipStatusEnum.Declined
                                      select f).FirstOrDefault();
            return existingFriendship;
        }

        public void AddFriendship(int initiatorAthleteID, int invitedAthleteID)
        {
            if (initiatorAthleteID == invitedAthleteID)
                throw new Exception("Can't friend yourself");

            Friendship existingFriendship = this.GetFriendship(initiatorAthleteID, invitedAthleteID);
            if (existingFriendship != null)
                throw new Exception("Friendship already exists");

            db.Friendships.Add(new Friendship()
                {
                    Athlete1ID = initiatorAthleteID,
                    Athlete2ID = invitedAthleteID,
                    FriendshipStatus = (int)FriendshipStatusEnum.Initiated,
                });
            db.SaveChanges();
        }

        public void Unfriend(int myAthleteID, int friendAthleteID)
        {
            var friendships = (from f in db.Friendships
                               where
                                (f.Athlete1ID == myAthleteID && f.Athlete2ID == friendAthleteID) ||
                                (f.Athlete1ID == friendAthleteID && f.Athlete2ID == myAthleteID)
                               select f).ToList();
            foreach (var f in friendships)
                db.Friendships.Remove(f);
            db.SaveChanges();
        }

        public void AcceptFriendRequest(int friendshipID, int myAthleteID)
        {
            var friendship = db.Friendships.Where(i => i.FriendshipID == friendshipID).Single();

            if (friendship.Athlete2ID != myAthleteID)
                throw new Exception("Wrong person");
            if (friendship.FriendshipStatus != (int)FriendshipStatusEnum.Initiated)
                throw new Exception("Wrong status");

            friendship.FriendshipStatus = (int)FriendshipStatusEnum.Confirmed;
            db.SaveChanges();
        }

        public void AcceptFriendRequest2(int athleteID, int myAthleteID)
        {
            var friendship = (from f in db.Friendships
                              where f.FriendshipStatus == (int)FriendshipStatusEnum.Initiated
                              where (f.Athlete1ID == athleteID && f.Athlete2ID == myAthleteID) || (f.Athlete1ID == myAthleteID && f.Athlete2ID == athleteID)
                              select f).First();

            friendship.FriendshipStatus = (int)FriendshipStatusEnum.Confirmed;
            db.SaveChanges();
        }

        public void DeclineFriendRequest(int friendshipID, int myAthleteID)
        {
            var friendship = db.Friendships.Where(i => i.FriendshipID == friendshipID).Single();

            if (friendship.Athlete2ID != myAthleteID)
                throw new Exception("Wrong person");
            if (friendship.FriendshipStatus != (int)FriendshipStatusEnum.Initiated)
                throw new Exception("Wrong status");

            friendship.FriendshipStatus = (int)FriendshipStatusEnum.Declined;
            db.SaveChanges();
        }

        public void WithdrawFriendRequest(int friendshipID, int myAthleteID)
        {
            var friendship = db.Friendships.Where(i => i.FriendshipID == friendshipID).Single();

            if (friendship.Athlete1ID != myAthleteID)
                throw new Exception("Wrong person");
            if (friendship.FriendshipStatus != (int)FriendshipStatusEnum.Initiated)
                throw new Exception("Wrong status");

            db.Friendships.Remove(friendship);
            db.SaveChanges();
        }
    }
}
