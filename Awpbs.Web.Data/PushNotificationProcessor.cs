using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    /// <summary>
    /// Uses PushSharp to push notifications to devices thru Apple and Google servers.
    /// 
    /// Singleton: must have a single instance of this class.
    /// 
    /// Uses two instances of PushBroker - one to talk to Apple's Sandbox servers, and the other to Apple's Production servers
    /// </summary>
    public class PushNotificationProcessor
    {
        public static PushNotificationProcessor TheProcessor
        {
            get;
            private set;
        }

        public static void InitSingleInstance()
        {
            if (TheProcessor == null)
                TheProcessor = new PushNotificationProcessor();
        }

        public static void Destroy()
        {
            if (TheProcessor != null)
                TheProcessor.destroy();
            TheProcessor = null;
        }

        PushBroker pushBrokerSandbox;
        PushBroker pushBrokerProduction;
        ApplicationDbContext db;

        private PushNotificationProcessor()
        {
            this.db = new ApplicationDbContext();
        }

        private void initialize()
        {
            if (this.pushBrokerProduction == null)
                this.pushBrokerProduction = this.createPushBrokerObject(true);
            if (this.pushBrokerSandbox == null)
                this.pushBrokerSandbox = this.createPushBrokerObject(false);
        }

        private PushBroker createPushBrokerObject(bool isProduction)
        {
            var appleCert = LoadAppleCertificate(isProduction);
            if (appleCert == null)
                TraceHelper.TraceInfo("Could not load Apple certificate!");

            var pushBroker = new PushBroker();
            pushBroker.OnNotificationFailed += pushBroker_OnNotificationFailed;
            pushBroker.OnNotificationSent += pushBroker_OnNotificationSent;
            pushBroker.OnServiceException += pushBroker_OnServiceException;

            if (isProduction)
                pushBroker.StopAllServices(); // just in case

            if (appleCert != null)
                pushBroker.RegisterAppleService(new PushSharp.Apple.ApplePushChannelSettings(isProduction, appleCert));

            string googleApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleMessagingAPIkey"];
            pushBroker.RegisterGcmService(new PushSharp.Android.GcmPushChannelSettings(googleApiKey));

            return pushBroker;
        }

        private void destroy()
        {
            if (pushBrokerSandbox != null)
                pushBrokerSandbox.StopAllServices(true);
            if (pushBrokerProduction != null)
                pushBrokerProduction.StopAllServices(true);
            pushBrokerSandbox = null;
            pushBrokerProduction = null;
        }

        private void pushBroker_OnServiceException(object sender, Exception error)
        {
        }

        private void pushBroker_OnNotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            if (notification.Tag == null || notification.Tag is long == false)
                return;
            long id = (long)notification.Tag;

            // remember that the notification was sent
            try
            {
                var obj = db.PushNotifications.Where(i => i.PushNotificationID == id).FirstOrDefault();
                if (obj != null)
                {
                    obj.Status = (int)PushNotificationStatusEnum.SentOk;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        private void pushBroker_OnNotificationFailed(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            if (notification.Tag == null || notification.Tag is long == false)
                return;
            long id = (long)notification.Tag;

            // remember that the notification has failed
            try
            {
                var obj = db.PushNotifications.Where(i => i.PushNotificationID == id).FirstOrDefault();
                if (obj != null)
                {
                    obj.Status = (int)PushNotificationStatusEnum.SentOk;
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        public X509Certificate2 LoadAppleCertificate(bool isProduction)
        {
            try
            {
                string appleCertificateName = isProduction ? "Apple Production IOS Push Services: com.bestyourbest.snooker" : "Apple Development IOS Push Services: com.bestyourbest.snooker";

                // check for the certificate in the CurrentUser storage
                var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                           X509FindType.FindBySubjectName, // alternatively - use "thumbprint"
                                           appleCertificateName,
                                           false);
                certStore.Close();
                if (certCollection.Count > 0)
                    return certCollection[0];

                // check for the certificate in the LocalMachine storage
                certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);
                certCollection = certStore.Certificates.Find(
                                           X509FindType.FindBySubjectName, // alternatively - use "thumbprint"
                                           appleCertificateName,
                                           false);
                certStore.Close();
                if (certCollection.Count > 0)
                    return certCollection[0];

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool PushAllPendingNotifications()
        {
            try
            {
                this.initialize();

                var pendingNotifications = (from i in db.PushNotifications.Include("DeviceToken")
                                            where i.Status == (int)PushNotificationStatusEnum.Prepared
                                            select i).ToList();

                foreach (var notification in pendingNotifications)
                {
                    if ((DateTime.UtcNow - notification.TimeCreated).TotalHours > 3)
                    {
                        // the notification is too old. discard it.
                        notification.Status = (int)PushNotificationStatusEnum.FailedWithTimeoutOnFire;
                        db.SaveChanges();
                        continue;
                    }

                    string deviceToken = this.fixAppleToken(notification.DeviceToken.Token);
                    string notificationText = notification.NotificationText;
                    int notificationObjectID = notification.ObjectID1 ?? 0;

                    // talk to either "production" or "sandbox" servers
                    bool isProductionDeviceToken = notification.DeviceToken.IsProduction;
                    var pushBroker = isProductionDeviceToken ? this.pushBrokerProduction : this.pushBrokerSandbox;
                    if (pushBroker == null)
                        continue;

                    if (notification.DeviceToken.IsApple)
                    {
                        // send the notification to an Apple device
                        pushBroker.QueueNotification(new AppleNotification()
                                               .ForDeviceToken(deviceToken)
                                               .WithAlert(notificationText)
                                               .WithCustomItem("id", notificationObjectID)
                                               //.WithBadge(4)
                                               //.WithSound("sound.caf")  // CONSIDER A CUSTOM SOUND!!! LIKE BALLS HITTING EACH OTHER !!!
                                               .WithTag(notification.PushNotificationID));

                        notification.Status = (int)PushNotificationStatusEnum.Fired;
                        db.SaveChanges();
                    }
                    else
                    {
                        // send the notification to an Android device
                        pushBroker.QueueNotification(new GcmNotification()
                            .ForDeviceRegistrationId(deviceToken)
                            .WithTag(notification.PushNotificationID)
                            .WithData(new Dictionary<string, string>()
                            {
                                { "message", notificationText },
                                { "objectID", notificationObjectID.ToString() },
                            }));

                        notification.Status = (int)PushNotificationStatusEnum.Fired;
                        db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        string fixAppleToken(string token)
        {
            string fixedToken = token.Replace("<", "").Replace(">", "").Replace(" ", "");
            return fixedToken;
        }
    }
}
