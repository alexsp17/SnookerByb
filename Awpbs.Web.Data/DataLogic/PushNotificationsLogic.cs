using PushSharp;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class PushNotificationsLogic
    {
        private readonly ApplicationDbContext db;

        public PushNotificationsLogic(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void RegisterDeviceToken(int athleteID, string token, bool isApple, bool isAndroid, bool isProduction)
        {
            if (string.IsNullOrEmpty(token))
                throw new Exception("token is empty");
            if (isApple == true && isAndroid == true)
                throw new Exception("isApple == true && isAndroid == true");
            if (isApple == false && isAndroid == false)
                throw new Exception("isApple == false && isAndroid == false");

            if (db.DeviceTokens.Where(i => i.Token == token && i.AthleteID == athleteID).Count() > 0)
                return; // the token is already registered

            // if there are records of the same token under a different athlete - remove them
            var deviceTokensToRemove = (from i in db.DeviceTokens
                                        where i.Token == token
                                        select i).ToList();
            foreach (var dt in deviceTokensToRemove)
                db.DeviceTokens.Remove(dt);

            // register
            var deviceToken = new DeviceToken()
            {
                AthleteID = athleteID,
                IsAndroid = isAndroid,
                IsApple = isApple,
                TimeCreated = DateTime.UtcNow,
                Token = token,
                IsProduction = isProduction,
            };
            db.DeviceTokens.Add(deviceToken);

            db.SaveChanges();
        }

        public int SendNotification(int athleteID, PushNotificationMessage message)
        {
            var deviceTokens = db.DeviceTokens.Where(i => i.AthleteID == athleteID).ToList();
            foreach (var deviceToken in deviceTokens)
            {
                var notification = new Awpbs.PushNotification()
                {
                    DeviceTokenID = deviceToken.DeviceTokenID,
                    TimeCreated = DateTime.UtcNow,
                    Status = (int)PushNotificationStatusEnum.Prepared,
                    NotificationText = message.Text,
                    ObjectID1 = message.ObjectID,
                    IsProduction = Config.IsProduction,
                };
                db.PushNotifications.Add(notification);
            }
            db.SaveChanges();

            return deviceTokens.Count;
        }
    }
}
