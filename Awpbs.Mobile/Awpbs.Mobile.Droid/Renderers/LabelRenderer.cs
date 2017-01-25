using System;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;

using Awpbs.Mobile;
using Awpbs.Mobile.Droid;
using Android.Graphics;
using System.Text;
using Android.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Util;

[assembly: ExportRenderer(typeof(BybLabel), typeof(BybLabelRenderer))]

namespace Awpbs.Mobile.Droid
{
	/// <summary>
	///  This renderer works for Labels with regular Text, as well as Labels with Spans
	/// </summary>
	public class BybLabelRenderer : LabelRenderer
	{
		Typeface typefaceRegular;
		Typeface typefaceBold;
		
		public BybLabelRenderer ()
		{
		}

		private void createTypefaces()
		{
			if (this.typefaceRegular == null)
				this.typefaceRegular = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Regular.ttf");
			if (this.typefaceBold == null)
				this.typefaceBold = Typeface.CreateFromAsset (Forms.Context.Assets, "fonts/Lato-Bold.ttf");
		}

		// setup
		protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged(e);

			if (Control == null || Element == null)
				return;

			this.updateFontFamily ();
		}

		// changed property
		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (Control == null || Element == null)
				return;

			if (e.PropertyName == "FontAttributes" || e.PropertyName == "Font" || e.PropertyName == "FontSize" || 
				e.PropertyName == "Text" || e.PropertyName == "FormattedText")
				this.updateFontFamily ();
		}

		void updateFontFamily()
		{
			try
			{
				this.createTypefaces();
				
				// this works for regular Labels
				Control.SetTypeface (this.Element.FontAttributes == FontAttributes.Bold ? typefaceBold : typefaceRegular, TypefaceStyle.Normal);
				Control.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);

				// this is needed for Labels with spans
				if (Element.FormattedText != null)
				{
					Control.TextFormatted = buildSpannableString(Element.FormattedText);
				}
			}
			catch (Exception)
			{
			}
		}

		SpannableString buildSpannableString(FormattedString formattedString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Span current in formattedString.Spans)
				if (current.Text != null)
					stringBuilder.Append(current.Text);

			SpannableString spannableString = new SpannableString(stringBuilder.ToString());

			int pos = 0;
			foreach (Span span in formattedString.Spans)
			{
				if (span.Text == null)
					continue;
				
				int posBegin = pos;
				int posEnd = posBegin + span.Text.Length;
				pos = posEnd;

				if (span.ForegroundColor != Xamarin.Forms.Color.Default)
				{
					spannableString.SetSpan(new ForegroundColorSpan(span.ForegroundColor.ToAndroid()), posBegin, posEnd, SpanTypes.InclusiveExclusive);
				}
				if (span.BackgroundColor != Xamarin.Forms.Color.Default)
				{
					spannableString.SetSpan(new BackgroundColorSpan(span.BackgroundColor.ToAndroid()), posBegin, posEnd, SpanTypes.InclusiveExclusive);
				}
				if (true)
				{
					spannableString.SetSpan(new FontSpan(span.FontAttributes == FontAttributes.Bold ? typefaceBold : typefaceRegular), posBegin, posEnd, SpanTypes.InclusiveExclusive);
				}
			}
			return spannableString;
		}

		class FontSpan : MetricAffectingSpan
		{
			private Typeface typeface;

			public FontSpan(Typeface typeface)
			{
				this.typeface = typeface;
			}

			public override void UpdateDrawState(TextPaint tp)
			{
				this.apply(tp);
			}

			public override void UpdateMeasureState(TextPaint p)
			{
				this.apply(p);
			}

			private void apply(Paint paint)
			{
				paint.SetTypeface (typeface);
				//float value = this.Font.ToScaledPixel();
				//paint.TextSize = TypedValue.ApplyDimension(ComplexUnitType.Sp, value, this.TextView.Resources.DisplayMetrics);
			}
		}
	}
}

