using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Awpbs.Mobile;

namespace Awpbs.Mobile
{
    // see "renderers"

    public class BybNoBorderPicker : Picker
    {
        public BybNoBorderPicker()
        {
            this.MinimumWidthRequest = 30;
            this.BackgroundColor = Color.Transparent;
        }
    }

	public class BybWithBorderPicker : Picker
	{
		public BybWithBorderPicker()
		{
			this.MinimumWidthRequest = 30;
			this.BackgroundColor = Color.Transparent;
		}
	}

    public class LargeBybPicker : Picker
    {
        public LargeBybPicker()
        {
        }
    }

    public class BybDatePicker : DatePicker
    {
    }
}
