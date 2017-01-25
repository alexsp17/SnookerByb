using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XLabs.Forms.Controls;
using Awpbs.Mobile;

namespace Awpbs.Mobile
{
    // see "renderers"

	public class BybStandardEntry : Entry
	{
	}

    public class BybPinEntry : Entry
    {
        public BybPinEntry()
        {
            this.Keyboard = Keyboard.Telephone;
            this.IsPassword = true;
			this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackgroundWhite : Config.ColorBackground;
			this.TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBlackTextOnWhite : Config.ColorTextOnBackground;

            this.TextChanged += textChanged;
        }

        private void textChanged(object sender, TextChangedEventArgs e)
        {
            if ((this.Text ?? "").Length == 4)
                this.Unfocus();
        }
    }

    public class BybNoBorderEntry : Entry
    {
    }

    public class BybLargeEntry : Entry
    {
        /// <summary>
        /// The HorizontalTextAlignment property
        /// </summary>
        public static readonly BindableProperty XAlignProperty =
            BindableProperty.Create("HorizontalTextAlignment", typeof(TextAlignment), typeof(ExtendedEntry), 
            TextAlignment.Start);

        /// <summary>
        /// Gets or sets the X alignment of the text
        /// </summary>
        //public TextAlignment HorizontalTextAlignment
        //{
        //    get { return (TextAlignment)GetValue(XAlignProperty); }
        //    set { SetValue(XAlignProperty, value); }
        //}

        public bool KeepOnlyNumbers
        {
            get;
            set;
        }

		public BybLargeEntry()
        {
            this.Unfocused += (s1, e1) =>
            {
                if (KeepOnlyNumbers == true)
                    this.removeAllButNumbers();
            };
        }

        void removeAllButNumbers()
        {
            // get rid of "dots"
            string text = this.Text;
            if (text == null)
                text = "";
            text = text.Trim();
            text = text.Replace(".", "");
            text = text.Replace(",", "");

            // get rid of zeroes at the beginning
            for (;;)
            {
                if (text.Length == 0)
                    break;
                if (text.StartsWith("0") && text.Length > 1)
                    text = text.Remove(0, 1);
                else
                    break;
            }

            this.Text = text;
        }
    }

    public class BybLargeEntry2 : Entry
    {
        public bool KeepOnlyNumbers
        {
            get;
            set;
        }

		public BybLargeEntry2()
        {
            this.Unfocused += (s1, e1) =>
            {
                if (KeepOnlyNumbers == true)
                    this.removeAllButNumbers();
            };
        }

        void removeAllButNumbers()
        {
            // get rid of "dots"
            string text = this.Text;
            if (text == null)
                text = "";
            text = text.Trim();
            text = text.Replace(".", "");
            text = text.Replace(",", "");

            // get rid of zeroes at the beginning
            for (;;)
            {
                if (text.Length == 0)
                    break;
                if (text.StartsWith("0") && text.Length > 1)
                    text = text.Remove(0, 1);
                else
                    break;
            }

            this.Text = text;
        }
    }
}
