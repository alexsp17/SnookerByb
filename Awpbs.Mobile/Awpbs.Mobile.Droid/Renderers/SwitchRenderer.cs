using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BybSwitch), typeof(BybSwitchRenderer))]

namespace Awpbs.Mobile
{
	//public class BybSwitchRenderer : Xamarin.Forms.Platform.Android.SwitchRenderer
	public class BybSwitchRenderer : SwitchRenderer
	{
		// Override the OnElementChanged method so we can tweak this renderer post-initial setup
		//protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Switch> e)
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Switch> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Control.SetMinWidth(200);
				Control.SetMinimumHeight(100);

				// This should work, but it doesn't
				Control.TextOn = "Yes";
				Control.TextOff = "No";

				/*
                Control.SetTextColor(global::Android.Graphics.Color.Red);

                Control.TextSize = (Config.DefaultFontSize);
                Control.SetBackgroundColor (global::Android.Graphics.Color.WhiteSmoke);

                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Bold);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
                Control.SetSwitchTypeface(font, TypefaceStyle.Bold);
                */
			}
		}
	}
}
