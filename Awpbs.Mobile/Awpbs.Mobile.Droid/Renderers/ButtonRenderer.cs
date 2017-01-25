using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Graphics;

using Awpbs.Mobile;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BybButton), typeof(BybButtonRenderer))]

namespace Awpbs.Mobile
{
	public class BybButtonRenderer : ButtonRenderer
	{
		static Typeface typefaceRegular;
		static Typeface typefaceBold;
		
		public BybButtonRenderer()
		{
		}

		static void createTypefaces()
		{
			if (typefaceRegular == null || typefaceBold == null)
			{
				typefaceRegular = Typeface.CreateFromAsset(Forms.Context.Assets, "fonts/Lato-Regular.ttf");
				typefaceBold = Typeface.CreateFromAsset(Forms.Context.Assets, "fonts/Lato-Bold.ttf");
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
				return;
			
			try
			{
				createTypefaces();
				Control.SetTypeface(this.Element.FontAttributes == FontAttributes.Bold ? typefaceBold : typefaceRegular, TypefaceStyle.Normal);
				Control.SetTextSize(Android.Util.ComplexUnitType.Sp, (float)this.Element.FontSize);
				
				Control.StateListAnimator = null; // this removed the padding and the shadow from the button. This fires exception on older Android, so set it last
			}
			catch
			{
			}
		}
	}
}
