using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybLargeEntry), typeof(BybLargeEntryRenderer))]
[assembly: ExportRenderer(typeof(BybLargeEntry2), typeof(BybLargeEntry2Renderer))]

namespace Awpbs.Mobile
{
    public class BybLargeEntryRenderer : Xamarin.Forms.Platform.iOS.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                switch (Element.HorizontalTextAlignment)
                {
                    case TextAlignment.Center: Control.TextAlignment = UITextAlignment.Center; break;
                    case TextAlignment.Start: Control.TextAlignment = UITextAlignment.Left; break;
                    case TextAlignment.End: Control.TextAlignment = UITextAlignment.Right; break;
                }
                //Control.TextAlignment = UITextAlignment.Center;

                float fontSize = Config.SuperLargeFontSize;

                Control.TextColor = UIColor.White;
                Control.Font = UIFont.FromName(Config.FontFamily, fontSize);
                Control.AttributedPlaceholder = new NSAttributedString(
                    "--",
                    font: UIFont.FromName(Config.FontFamily, fontSize),
                    foregroundColor: UIColor.FromRGB((byte)(int)(Config.ColorTextOnBackgroundGrayed.R * 255), (byte)(int)(Config.ColorTextOnBackgroundGrayed.G * 255), (byte)(int)(Config.ColorTextOnBackgroundGrayed.B * 255)),// UIColor.White,
                    strokeWidth: 0
                );
            }
        }
    }

    public class BybLargeEntry2Renderer : Xamarin.Forms.Platform.iOS.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null && Element != null)
            {
                switch (Element.HorizontalTextAlignment)
                {
                    case TextAlignment.Center: Control.TextAlignment = UITextAlignment.Center; break;
                    case TextAlignment.Start: Control.TextAlignment = UITextAlignment.Left; break;
                    case TextAlignment.End: Control.TextAlignment = UITextAlignment.Right; break;
                }

                //Control.TextAlignment = UITextAlignment.Left;

                Control.TextColor = UIColor.White;
                Control.Font = UIFont.FromName(Config.FontFamily, Config.SuperLargeFontSize);
                Control.AttributedPlaceholder = new NSAttributedString(
                    "--",
                    font: UIFont.FromName(Config.FontFamily, Config.SuperLargeFontSize),
                    foregroundColor: UIColor.FromRGB((byte)(int)(Config.ColorTextOnBackgroundGrayed.R * 255), (byte)(int)(Config.ColorTextOnBackgroundGrayed.G * 255), (byte)(int)(Config.ColorTextOnBackgroundGrayed.B * 255)),// UIColor.White,
                    strokeWidth: 0
                );
            }
        }
    }
}