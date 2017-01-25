using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    /// <summary>
    /// A time picker
    /// </summary>
    public class BybTimePicker : Grid
    {
        public event EventHandler TimeSelected;

        public HourAndMinute SelectedTime
        {
            get
            {
                if (this.picker.SelectedIndex < 0)
                    return null;
                return items[this.picker.SelectedIndex];
            }
        }

        public new void Focus()
        {
            this.picker.Focus();
        }

        Picker picker;

        List<HourAndMinute> items;

        public BybTimePicker()
        {
            this.items = new List<HourAndMinute>();
            for (int hour = 6; hour <= 23; ++hour)
                for (int minute = 0; minute <= 45; minute += 15)
                    this.items.Add(new HourAndMinute(hour, minute));

            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.HeightRequest = 50;
            this.Padding = new Thickness(0);
            //this.BackgroundColor = Color.Transparent;
            //this.HasShadow = false;

            this.picker = new BybNoBorderPicker()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = Color.Transparent,
                Title = "Pick time"
            };
            foreach (var item in items)
                this.picker.Items.Add(item.Text);
            this.picker.SelectedIndex = items.Count - 16;
            this.picker.SelectedIndexChanged += picker_SelectedIndexChanged;

//            this.Content = new StackLayout
//            {
//                Orientation = StackOrientation.Horizontal,
//                Spacing = 5,
//                Padding = new Thickness(0, 0, 0, 0),
//                Children =
//                {
//                    this.picker,
//                }
//            };
			this.Children.Add(this.picker);

            this.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    picker.Focus();
                }),
                NumberOfTapsRequired = 1
            });
        }

        private void picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TimeSelected != null)
                this.TimeSelected(null, EventArgs.Empty);
        }
    }
}
