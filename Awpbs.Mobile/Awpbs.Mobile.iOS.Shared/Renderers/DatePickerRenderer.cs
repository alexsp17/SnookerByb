using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybDatePicker), typeof(BybDatePickerRenderer))]

namespace Awpbs.Mobile
{
    public class BybDatePickerRenderer : Xamarin.Forms.Platform.iOS.DatePickerRenderer
    {
        // Override the OnElementChanged method so we can tweak this renderer post-initial setup
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
                Control.TextColor = UIColor.Black;
                Control.Font = UIFont.FromName(Config.FontFamilyBold, (float)Config.DefaultFontSize);
            }
        }
    }
}