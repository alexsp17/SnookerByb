
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

using Awpbs.Mobile;

namespace Awpbs.Mobile.Droid
{
    using System.Threading;

    using Android.App;
    using Android.OS;

    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            StartActivity(typeof(MainActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }

        //
        // activity becomes invisible, kill it
        protected override void OnStop()
        {
            base.OnStop();

            this.Finish();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

    }

}

