using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Awpbs.Mobile;

namespace Awpbs.Mobile
{
    // see "renderers"

    public class BybNavigationPage : NavigationPage
    {
        public BybNavigationPage(Page root)
            : base(root)
        {
        }
			
		protected override void OnParentSet()
		{
			base.OnParentSet ();
		}
    }
}
