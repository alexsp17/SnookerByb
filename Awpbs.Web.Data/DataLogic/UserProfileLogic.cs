using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class UserProfileLogic
    {
        private readonly ApplicationDbContext db;

        public UserProfileLogic(ApplicationDbContext context)
        {
            this.db = context;
        }

        public ApplicationUser FindUser(string userName)
        {
            var user = db.Users.Where(i => string.Compare(i.UserName, userName, true) == 0).FirstOrDefault();
            return user;
        }

        public string GetFacebookIdForUser(string userName)
        {
            var user = this.FindUser(userName);
            if (user == null)
                throw new Exception("no such user");
            string facebookKey = user.Logins.Where(i => i.LoginProvider == "Facebook").Select(i => i.ProviderKey).FirstOrDefault();
            return facebookKey;
        }

        public Athlete CreateAthleteForUserName(string userName, string facebookId, string nameInFacebook)
        {
            var user = FindUser(userName);
            if (user == null)
                throw new Exception("user == null");
            var athlete = db.Athletes.Where(i => string.Compare(userName, i.UserName) == 0).FirstOrDefault();
            if (athlete != null)
                throw new Exception("athlete != null");

            DateTime now = DateTime.UtcNow;
            athlete = new Athlete()
            {
                IsPro = false,
                UserName = userName,
                Name = "",
                NameInFacebook = nameInFacebook,
                Gender = 0,
                FacebookId = facebookId,
                TimeModified = now,
                TimeCreated = now
            };
            db.Athletes.Add(athlete);
            db.SaveChanges();

            return this.GetAthleteForUserName(userName);
        }

        public int GetAthleteIDForUserName(string userName)
        {
            var user = FindUser(userName);
            if (user == null)
                throw new Exception("user == null");
            int athleteID = (from a in db.Athletes
                             where string.Compare(userName, a.UserName) == 0
                             select a.AthleteID).First();
            return athleteID;
        }

        public Athlete GetAthleteForUserName(string userName)
        {
            var user = FindUser(userName);
            if (user == null)
                throw new Exception("user == null");
            var athlete = db.Athletes.Where(i => string.Compare(userName, i.UserName) == 0).First();
            return athlete;
        }

        public void UpdateAthleteProfile(int athleteID, string newName, GenderEnum gender, DateTime? dob = null)
        {
            if (newName == null)
                newName = "";
            if (newName != "" && dob != null)
            {
                List<Athlete> existingAthletes = (from a in db.Athletes
                                                  where string.Compare(a.Name, newName, true) == 0
                                                  where a.Gender == (int)gender
                                                  where a.AthleteID != athleteID
                                                  where a.DOB == dob
                                                  select a).ToList();
                if (existingAthletes.Count > 0)
                    throw new Exception("Such athlete already exists");
            }

            Athlete athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);
            athlete.Name = newName;
            athlete.DOB = dob;
            athlete.Gender = (int)gender;
            athlete.TimeModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public void UpdateAthletePicture(int athleteID, string pictureUrl)
        {
            Athlete athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);
            athlete.Picture = pictureUrl;
            athlete.TimeModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public void UpdateRealEmail(int athleteID, string email)
        {
            Athlete athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);
            athlete.RealEmail = email;
            athlete.TimeModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public void SetPin(int athleteID, string pin)
        {
            Athlete athlete = db.Athletes.SingleOrDefault(i => i.AthleteID == athleteID);
            string encryptionPassword = athlete.UserName; // use the user name as the encryption password
            athlete.Pin = Crypto.Encrypt(pin, encryptionPassword);
            athlete.TimeModified = DateTime.UtcNow;
            db.SaveChanges();
        }

        public bool VerifyPin(int athleteID, string pin)
        {
            if (string.IsNullOrEmpty(pin))
                return false;
            string pinFromDb = this.GetPin(athleteID);
            bool verified = string.Compare(pinFromDb, pin) == 0;
            return verified;
        }

        public string GetPin(int athleteID)
        {
            Athlete athlete = db.Athletes.Where(i => i.AthleteID == athleteID).Single();
            if (string.IsNullOrEmpty(athlete.Pin))
                return null;
            string encryptionPassword = athlete.UserName; // the user name is the encryption password
            string pinFromDb = Crypto.Decrypt(athlete.Pin, encryptionPassword);
            return pinFromDb;
        }
    }
}
