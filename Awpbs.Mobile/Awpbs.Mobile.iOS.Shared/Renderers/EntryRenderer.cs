using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybNoBorderEntry), typeof(BybNoBorderEntryRenderer))]
[assembly: ExportRenderer(typeof(BybStandardEntry), typeof(BybStandardEntryRenderer))]

namespace Awpbs.Mobile
{
    public class BybNoBorderEntryRenderer : Xamarin.Forms.Platform.iOS.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.Font = UIFont.FromName(Config.FontFamily, Config.DefaultFontSize);

                if (e != null && e.NewElement != null && string.IsNullOrEmpty(e.NewElement.Placeholder) == false)
                    Control.AttributedPlaceholder = new NSAttributedString(
                        e.NewElement.Placeholder,
                        font: UIFont.FromName(Config.FontFamily, Config.DefaultFontSize),
                        foregroundColor: UIColor.FromRGB((byte)(int)(Config.ColorGrayTextOnWhite.R * 255), (byte)(int)(Config.ColorGrayTextOnWhite.G * 255), (byte)(int)(Config.ColorGrayTextOnWhite.B * 255)),// UIColor.White,
                        strokeWidth: 0
                    );
            }
        }
    }

    public class BybStandardEntryRenderer : Xamarin.Forms.Platform.iOS.EntryRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Font = UIFont.FromName(Config.FontFamily, Config.DefaultFontSize);
            }
        }
    }
}