using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;
using Android;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

[assembly: ExportRenderer(typeof(BybStandardEntry), typeof(BybStandardEntryRenderer))]
[assembly: ExportRenderer(typeof(BybNoBorderEntry), typeof(BybNoBorderEntryRenderer))]
[assembly: ExportRenderer(typeof(BybPinEntry), typeof(BybPinEntryRenderer))]

namespace Awpbs.Mobile
{
	public class BybPinEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
	{
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);
		}
	}
	
	public class BybStandardEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
	{
		// Override the OnElementChanged method so we can tweak this renderer post-initial setup
		protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				try
				{
					Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
					Control.SetTypeface(font, TypefaceStyle.Normal);
					Control.SetTextColor(global::Android.Graphics.Color.Black);
					Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
					Control.SetBackgroundColor (Android.Graphics.Color.White);
					Control.SetHintTextColor(Android.Graphics.Color.Gray);
				}
				catch (Exception exc)
				{
					var failureInfo = TraceHelper.ExceptionToString(exc);
					Console.WriteLine("BybStandardEntryRenderer: OnElementChanged(): Exception " + failureInfo);

					return;
				}
			}
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
	

	public class BybNoBorderEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
                Control.SetTypeface(font, TypefaceStyle.Normal);
				Control.SetTextColor(global::Android.Graphics.Color.Blue);
                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
				Control.SetBackgroundColor (Android.Graphics.Color.White);
				Control.SetHintTextColor(Android.Graphics.Color.Gray);
            }
        }
    }

//	public class BybEntry2Renderer : Xamarin.Forms.Platform.Android.EntryRenderer
//    {
//        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
//        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
//        {
//            base.OnElementChanged(e);
//
//            if (Control != null)
//            {
//                Control.SetTextColor(global::Android.Graphics.Color.Black);
//                Control.SetBackgroundColor(Android.Graphics.Color.White);
//				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
//				Control.SetTypeface(font, TypefaceStyle.Normal);
//				Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
//
//                // When the text doesn't fit in the field
//                Control.HorizontalFadingEdgeEnabled = true;
//                Control.SetFadingEdgeLength(30);
//                Control.SetHorizontallyScrolling(true);
//            }
//        }
//    }

//    public class BybEntry2Renderer_whiteBkgnd : Xamarin.Forms.Platform.Android.EntryRenderer
//    {
//        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
//        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Entry> e)
//        {
//            base.OnElementChanged(e);
//
//            if (Control != null)
//            {
//                Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
//                Control.SetTypeface(font, TypefaceStyle.Normal);
//                Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
//                Control.SetBackgroundColor(Android.Graphics.Color.White);
//            }
//        }
//    }

}