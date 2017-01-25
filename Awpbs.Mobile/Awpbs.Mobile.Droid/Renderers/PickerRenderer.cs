using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

//using Foundation;
//using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(BybWithBorderPicker), typeof(BybWithBorderPickerRenderer))]
[assembly: ExportRenderer(typeof(BybNoBorderPicker), typeof(BybNoBorderPickerRenderer))]
[assembly: ExportRenderer(typeof(LargeBybPicker), typeof(LargeBybPickerRenderer))]

namespace Awpbs.Mobile
{
	public class BybWithBorderPickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
				Control.SetTextColor(global::Android.Graphics.Color.Black);
				Control.SetBackgroundColor (global::Android.Graphics.Color.Red);

                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Bold);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
            }

			// MG, 11/20/2015 - this is a fix of an issue with a Picker on an Android platform - https://bugzilla.xamarin.com/show_bug.cgi?id=24871
			// MG, 11/24/2015 - now I don't think this is necessary
//			if (e.OldElement != null)
//			{
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.TitleProperty.PropertyName));
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.SelectedIndexProperty.PropertyName));
//			}
        }

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
			
			var nativeEditText = Control;

			// add a gray border
			var shape = new ShapeDrawable(new RectShape());
			shape.Paint.Color = Android.Graphics.Color.Gray;
			shape.Paint.StrokeWidth = 2;
			shape.Paint.SetStyle(Paint.Style.Stroke);
			nativeEditText.SetBackgroundDrawable(shape);
		}
    }

	public class BybNoBorderPickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
	    protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // ANDROID_TODO: no border
                // Control.BorderStyle = UITextBorderStyle.None;
                //
                // Note:
                //  this.BackgroundColor = Color.Transparent;
                // does not seem to make a difference for Android 4.4.2
                //
                Control.SetBackgroundColor (global::Android.Graphics.Color.Transparent);

				Control.SetTextColor(global::Android.Graphics.Color.Black);

				// set font and font size
				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
				Control.SetTypeface(font, TypefaceStyle.Bold);
				Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
				Control.SetHintTextColor(Android.Graphics.Color.Gray);
				Control.SetPadding (0, 0, 0, 0);

				Control.SetSingleLine ();
            }

			// MG, 11/20/2015 - this is a fix of an issue with a Picker on an Android platform - https://bugzilla.xamarin.com/show_bug.cgi?id=24871
//			if (e.OldElement != null)
//			{
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.TitleProperty.PropertyName));
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.SelectedIndexProperty.PropertyName));
//			}
        }
    }

    public class LargeBybPickerRenderer : Xamarin.Forms.Platform.Android.PickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // ANDROID_TODO: no border
                // Control.BorderStyle = UITextBorderStyle.None;

                Control.SetTextColor(global::Android.Graphics.Color.White);

                // set font and font size
                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Normal);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.VeryLargeFontSize);
            }

			// MG, 11/20/2015 - this is a fix of an issue with a Picker on an Android platform - https://bugzilla.xamarin.com/show_bug.cgi?id=24871
//			if (e.OldElement != null)
//			{
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.TitleProperty.PropertyName));
//				OnElementPropertyChanged(this, new PropertyChangedEventArgs(Picker.SelectedIndexProperty.PropertyName));
//			}
        }
    }

}