using System;
using Android.App;
using Android.Graphics.Drawables;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Awpbs.Mobile;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]

namespace Awpbs.Mobile
{
    //public class CustomNavigationRenderer : Xamarin.Forms.Platform.Android.NavigationRenderer
    public class CustomNavigationRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged (e);

			this.Element.BarTextColor = Config.ColorPageTitleBarTextNormal;

			// tried to limit orientation here, didn't work...
			//var activity = (Activity)Context;
			//activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

            //removeAppIconFromActionBar ();
        }

        void removeAppIconFromActionBar()
        {
            // http://stackoverflow.com/questions/14606294/remove-icon-logo-from-action-bar-on-android
            var actionBar = ((Activity)Context).ActionBar;
            actionBar.SetIcon (new ColorDrawable(Xamarin.Forms.Color.Transparent.ToAndroid()));
        }
    }
}