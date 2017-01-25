using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BybEditor), typeof(BybEditorRenderer))]

namespace Awpbs.Mobile
{
	public class BybEditorRenderer : EditorRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				Typeface font = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
				Control.SetTypeface(font, TypefaceStyle.Normal);
				Control.SetTextColor(global::Android.Graphics.Color.Black);
				Control.SetTextSize (Android.Util.ComplexUnitType.Sp, Config.DefaultFontSize);
				Control.SetBackgroundColor (Android.Graphics.Color.White);
				Control.SetHintTextColor(Android.Graphics.Color.Gray);
			}
		}
	}
}

