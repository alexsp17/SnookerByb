using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotificationsTester
{
    public partial class Form1 : Form
    {
        bool isProduction = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonSendIOSMessage_Click(object sender, EventArgs e)
        {
            this.sendNotification(true, false, this.textBoxIOSToken.Text, this.textBoxMessage.Text);
        }

        private void buttonSendAndroidMessage_Click(object sender, EventArgs e)
        {
            this.sendNotification(false, true, this.textBoxAndroidToken.Text, this.textBoxMessage.Text);
        }

        private void sendNotification(bool isApple, bool isAndroid, string deviceToken, string notificationText)
        {
            if (string.IsNullOrEmpty(deviceToken))
            {
                MessageBox.Show(this, "Enter device token");
                return;
            }

            var pushBroker = new PushBroker();
            pushBroker.OnNotificationFailed += pushBroker_OnNotificationFailed1;
            pushBroker.OnNotificationSent += pushBroker_OnNotificationSent;
            pushBroker.OnServiceException += pushBroker_OnServiceException;

            if (isApple)
            {
                var appleCert = loadAppleCertificate();
                pushBroker.RegisterAppleService(new PushSharp.Apple.ApplePushChannelSettings(isProduction, appleCert));

                pushBroker.QueueNotification(new AppleNotification()
                                       .ForDeviceToken(this.fixAppleToken(deviceToken))
                                       .WithAlert(notificationText)
                                       .WithTag(1));
            }

            if (isAndroid)
            {
                string googleApiKey = System.Web.Configuration.WebConfigurationManager.AppSettings["GoogleMessagingAPIkey"];
                pushBroker.RegisterGcmService(new PushSharp.Android.GcmPushChannelSettings(googleApiKey));

                pushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(deviceToken)
                        .WithTag("77")
                        .WithData(new Dictionary<string, string>()
                        {
                            { "message", notificationText },
                            { "title", "Test" }
                        }));
            }

            pushBroker.StopAllServices();
        }

        private void pushBroker_OnServiceException(object sender, Exception error)
        {
            this.BeginInvoke((Action)(() =>
            {
                MessageBox.Show(this, "Exception: " + Awpbs.TraceHelper.ExceptionToString(error), "Exception.");
            }));
        }

        private void pushBroker_OnNotificationSent(object sender, PushSharp.Core.INotification notification)
        {
            this.BeginInvoke((Action)(() =>
            {
                MessageBox.Show(this, "Sent.", "Done");
            }));
        }

        private void pushBroker_OnNotificationFailed1(object sender, PushSharp.Core.INotification notification, Exception error)
        {
            this.BeginInvoke((Action)(() =>
            {
                MessageBox.Show(this, "Failed: " + Awpbs.TraceHelper.ExceptionToString(error), "Failed.");
            }));
            
        }

        private X509Certificate2 loadAppleCertificate()
        {
            try
            {
                // check for the certificate in the CurrentUser storage
                var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                certStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                                           X509FindType.FindBySubjectName, // alternatively - use "thumbprint"
                                           isProduction ? "Apple Production IOS Push Services: com.bestyourbest.snooker" : "Apple Development IOS Push Services: com.bestyourbest.snooker",
        
                                           false);
                certStore.Close();
                if (certCollection.Count > 0)
                    return certCollection[0];

                // check for the certificate in the LocalMachine storage
                certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);
                certCollection = certStore.Certificates.Find(
                                           X509FindType.FindBySubjectName, // alternatively - use "thumbprint"
                                           isProduction ? "Apple Production IOS Push Services: com.bestyourbest.snooker" : "Apple Development IOS Push Services: com.bestyourbest.snooker",
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

        string fixAppleToken(string token)
        {
            string fixedToken = token.Replace("<", "").Replace(">", "").Replace(" ", "");
            return fixedToken;
        }
    }
}
