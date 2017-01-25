using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    /// <summary>
    /// An entry field in a large-font, to enter a number
    /// </summary>
    public class LargeNumberEntry2 : Grid
    {
        public event EventHandler NumberChanged;
        public event EventHandler FocusedOnNumber;
        public event EventHandler UnfocusedFromNumber;

        BybLargeEntry entry;

        bool ignoreUIEvents;

        public TextAlignment TextAlignment
        {
            get { return this.entry.HorizontalTextAlignment; }
            set { this.entry.HorizontalTextAlignment = value; }
        }

        public new bool IsEnabled
        {
            get { return this.entry.IsEnabled; }
            set { this.entry.IsEnabled = value;  }
        }

        // TextAlignment bindable property
        //public static readonly BindableProperty TextAlignmentProperty =
        //    BindableProperty.Create("TextAlignment", typeof(TextAlignment), typeof(LargeNumberEntry2), TextAlignment.Center);
        //public TextAlignment TextAlignment
        //{
        //    get
        //    {
        //        return (TextAlignment)GetValue(TextAlignmentProperty);
        //    }
        //    set
        //    {
        //        SetValue(TextAlignmentProperty, value);
        //    }
        //}

        public int? Number
        {
            get
            {
                int number;
                if (int.TryParse(entry.Text, out number) == false)
                    return null;
                return number;
            }
            set
            {
                this.ignoreUIEvents = true;
                if (value == null)
                    entry.Text = "";
                else
                    entry.Text = value.ToString();
                this.ignoreUIEvents = false;
            }
        }

        public LargeNumberEntry2(Color backgroundColor)
        {
            //this.HasShadow = false;
            this.Padding = new Thickness(0);

            entry = new BybLargeEntry()
            {
                KeepOnlyNumbers = true,
                //WidthRequest = 100,
                Keyboard = Keyboard.Numeric,
                BackgroundColor = backgroundColor,
                VerticalOptions =  Config.App == MobileAppEnum.SnookerForVenues ? LayoutOptions.FillAndExpand :LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Fill,//LayoutOptions.Center,
                //HorizontalTextAlignment = TextAlignment.Center,
                Placeholder = "--",
                Text = null,
            };
            entry.TextChanged += (s1, e1) =>
            {
                if (this.ignoreUIEvents)
                    return;
                if (this.NumberChanged != null)
                    this.NumberChanged(this, EventArgs.Empty);
            };
            entry.Focused += (s1, e1) =>
            {
                if (this.FocusedOnNumber != null)
                    this.FocusedOnNumber(this, EventArgs.Empty);
            };
            entry.Unfocused += (s1, e1) =>
            {
                if (this.UnfocusedFromNumber != null)
                    this.UnfocusedFromNumber(this, EventArgs.Empty);
            };
			this.Children.Add(entry);
            //this.Content = entry;
        }
    }

    /// <summary>
    /// An entry field in a large-font, to enter a number
    /// </summary>
    public class LargeNumberEntry : StackLayout
    {
        public event EventHandler NumberChanged;
        public event EventHandler FocusedOnNumber;
        public event EventHandler UnfocusedFromNumber;

		BybLargeEntry entry;
        Label label;

        bool ignoreUIEvents;

        public new bool IsEnabled
        {
            get { return this.entry.IsEnabled; }
            set { this.entry.IsEnabled = value; }
        }

		public bool IsLabelVisible
		{
			get { return this.label.IsVisible; }
			set { this.label.IsVisible = value; this.Spacing = value ? 3 : 0; }
		}

        public int? Number
        {
            get
            {
                int number;
                if (int.TryParse(entry.Text, out number) == false)
                    return null;
                return number;
            }
            set
            {
                this.ignoreUIEvents = true;
                if (value == null)
                    entry.Text = "";
                else
                    entry.Text = value.ToString();
                this.ignoreUIEvents = false;
            }
        }

        public string Title
        {
            get
            {
                return this.label.Text;
            }
            set
            {
                this.label.Text = value;
            }
        }

        public string Placeholder
        {
            get
            {
                return entry.Placeholder;
            }
            set
            {
                this.entry.Placeholder = value;
            }
        }

		public LargeNumberEntry(string title, Color backgroundColor)
        {
			entry = new BybLargeEntry()
            {
                KeepOnlyNumbers = true,
                WidthRequest = 100,
                Keyboard = Keyboard.Numeric,
				BackgroundColor = backgroundColor,//Config.ColorDarkGrayBackground,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,

                //#if __ANDROID__
                HorizontalTextAlignment = TextAlignment.Center,
                Placeholder = "--",
                Text = null,
                //#endif
            };
            entry.TextChanged += (s1, e1) =>
            {
                if (this.ignoreUIEvents)
                    return;
                if (this.NumberChanged != null)
                    this.NumberChanged(this, EventArgs.Empty);
            };
            entry.Focused += (s1, e1) =>
            {
                if (this.FocusedOnNumber != null)
                    this.FocusedOnNumber(this, EventArgs.Empty);
            };
            entry.Unfocused += (s1, e1) =>
            {
                if (this.UnfocusedFromNumber != null)
                    this.UnfocusedFromNumber(this, EventArgs.Empty);
            };

            this.label = new BybLabel
            {
                Text = title,
                TextColor = Config.ColorTextOnBackground,
                //WidthRequest = Config.IsTablet ? 120 : 60,
                HorizontalOptions = LayoutOptions.Fill,//LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Start,
            };

            this.Orientation = StackOrientation.Vertical;
			this.BackgroundColor = backgroundColor;//Config.ColorDarkGrayBackground;
            this.Spacing = 3;
            this.Children.Add(label);
            this.Children.Add(entry);
            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    this.entry.Focus();
                }),
                NumberOfTapsRequired = 1
            });
        }

        public void FocusOnNumber()
        {
            this.entry.Focus();
        }
    }
}
