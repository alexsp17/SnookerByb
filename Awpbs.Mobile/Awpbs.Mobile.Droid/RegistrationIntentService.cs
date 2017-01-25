using System;
using Android.App;
using Android.Content;
using Android.Util;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
//using Android.Gms.Common;

namespace Awpbs.Mobile.Droid
{
    [Service(Exported = false)]
    class RegistrationIntentService : IntentService
    {
        static object locker = new object();

        public RegistrationIntentService() : base("RegistrationIntentService") { }

        protected override void OnHandleIntent (Intent intent)
        {
            try
            {
                Log.Info ("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (locker)
                {
                    var instanceID = InstanceID.GetInstance (this);
                    var token = instanceID.GetToken ("649212692032", GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info ("RegistrationIntentService", "GCM Registration Token: " + token);

                    SendRegistrationToAppServer (token);

                    //Subscribe (token); // we are not using topics
                }
            }
            catch (Exception e)
            {
                var failureInfo = TraceHelper.ExceptionToString(e);
                Console.WriteLine("RegistrationIntentService::OnHandleIntent() Exception(): " + failureInfo);

                return;
            }
        }


        public async void SendRegistrationToAppServer(string deviceTokenStr)
        {
            // Add custom implementation here as needed.
            bool deviceTokenAlreadyRegistered = string.Compare(deviceTokenStr, Awpbs.Mobile.App.KeyChain.RegisteredDeviceToken, false) == 0;

            if (deviceTokenAlreadyRegistered == false)
            {
                if (App.LoginAndRegistrationLogic.RegistrationStatus == RegistrationStatusEnum.Registered)
                {
                    // already logged-in as a user -> register the device token
                    bool result = await App.WebService.RegisterDeviceToken(deviceTokenStr, Config.IsIOS, Config.IsAndroid);

                    if (result == true)
                    {
                        Awpbs.Mobile.App.KeyChain.RegisteredDeviceToken = deviceTokenStr;
                    }
                    else
                    {
                        Log.Debug("SendRegistrationToAppServer()", "Failed to register device token with app server");
                    }
                }
                else
                {
                    // not yet logged-in as a user -> register the device token once logged-in
                    Awpbs.Mobile.App.KeyChain.DeviceTokenToRegisterWhenLoggedIn = deviceTokenStr;
                }

            }
        }

        void Subscribe (string token)
        {
            try
            {
                var pubSub = GcmPubSub.GetInstance(this);
                pubSub.Subscribe(token, "/topic/global", null);
            }
            catch (Exception e)
            {
                Console.WriteLine("RegistrationIntentService::Subscribe(): Exception " + TraceHelper.ExceptionToString(e));
                return;
            }
        }
    }
}
