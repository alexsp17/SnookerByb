using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Awpbs.Mobile;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BybNavigationPage), typeof(NavigationPageRenderer))]

namespace Awpbs.Mobile
{
    public class NavigationPageRenderer : Xamarin.Forms.Platform.iOS.NavigationRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.NavigationBar.TitleTextAttributes = new UIStringAttributes()
            {
				ForegroundColor = UIKit.UIColor.FromRGB((byte)(Config.ColorPageTitleBarTextNormal.R*255), (byte)(Config.ColorPageTitleBarTextNormal.G*255), (byte)(Config.ColorPageTitleBarTextNormal.B*255)),
                Font = UIFont.FromName(Config.FontFamily, (nfloat)Config.LargerFontSize)
            };

            //NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
            //this.NavigationBar.BarStyle = UIBarStyle.

        }
    }
}