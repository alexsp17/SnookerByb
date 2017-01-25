using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;

using System.ComponentModel;
using Android.Text.Method;
using Android.Util;
using Android.Views;
 //   using Extensions;
using Xamarin.Forms.Platform.Android;
//using Xamarin.Forms.Labs.Controls;


[assembly: ExportRenderer(typeof(BybLargeEntry), typeof(BybLargeEntryRenderer))]
[assembly: ExportRenderer(typeof(BybLargeEntry2), typeof(BybLargeEntry2Renderer))]

namespace Awpbs.Mobile
{
	public class BybLargeEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

			var view = (BybLargeEntry)Element;
            if (Control != null)
            {
                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
				Control.SetTypeface(font, TypefaceStyle.Normal);
                Control.SetTextColor(global::Android.Graphics.Color.White);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.VeryLargeFontSize);
				if (view.BackgroundColor != null)
					Control.SetBackgroundColor(new Android.Graphics.Color((int)(255*view.BackgroundColor.R),(int)(255*view.BackgroundColor.G),(int)(255*view.BackgroundColor.B),(int)(255*view.BackgroundColor.A)));
                Control.SetPadding(0,0,0,0);
            }
        }
    }

	public class BybLargeEntry2Renderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Normal);
                Control.SetTextColor(global::Android.Graphics.Color.White);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.VeryLargeFontSize);
            }
        }
    }
}
