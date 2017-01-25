using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybDatePicker), typeof(BybDatePickerRenderer))]

namespace Awpbs.Mobile
{
	public class BybDatePickerRenderer : Xamarin.Forms.Platform.Android.DatePickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
 		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // 
                // I tried this line in BybDatePicker() constructor, it didn't work
                //  this.BackgroundColor = Color.Transparent;
                //
                Control.SetBackgroundColor (global::Android.Graphics.Color.Transparent);

				Control.TextAlignment = (global::Android.Views.TextAlignment.Center);
                Control.SetTextColor(Android.Graphics.Color.Black);
                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Bold);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
				Control.SetPadding (0, 0, 0, 0);
            }
        }
    }
}