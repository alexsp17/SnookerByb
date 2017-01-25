using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybNoBorderPicker), typeof(BybNoBorderPickerRenderer))]
[assembly: ExportRenderer(typeof(BybWithBorderPicker), typeof(BybWithBorderPickerRenderer))]
[assembly: ExportRenderer(typeof(LargeBybPicker), typeof(LargeBybPickerRenderer))]

namespace Awpbs.Mobile
{
    public class BybWithBorderPickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.Font = UIFont.FromName(Config.FontFamily, Config.DefaultFontSize);
            }
        }
    }

    public class BybNoBorderPickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.TextColor = UIColor.Black;
                Control.Font = UIFont.FromName(Config.FontFamilyBold, Config.DefaultFontSize);
            }
        }
    }

    public class LargeBybPickerRenderer : Xamarin.Forms.Platform.iOS.PickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.TextColor = UIColor.White;
                Control.Font = UIFont.FromName(Config.FontFamily, Config.VeryLargeFontSize);
            }
        }
    }
}