using System;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class BybLabel : Label
	{
		public BybLabel ()
		{
			this.FontSize = Config.DefaultFontSize; // this is important, because the font size does not always propagate from the Style
		}
	}
}

