using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class PickVenuePage : ContentPage
	{
		public event EventHandler<VenueWebModel> UserMadeSelection;

        FindVenuesControl findVenuesControl;

		public PickVenuePage()
		{
            this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBlackBackground : Config.ColorGrayBackground;

            // FindVenuesControl
            this.findVenuesControl = new FindVenuesControl(false)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0,0,0,0),
				IsFindVenueMode = true,
            };
            this.findVenuesControl.UserClickedOnVenue += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(s1, e1);
            };

            // cancel and "not listed" button
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, null);
            };
            Button buttonNotListed = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "It's not listed here" };
            buttonNotListed.Clicked += buttonNotListed_Clicked;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                BackgroundColor = Config.ColorGrayBackground,
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonNotListed,
                }
            };

			Content = new StackLayout
			{
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
				Children =
                {
                    new BybTitle("Pick Venue") { VerticalOptions = LayoutOptions.Start },
                    new ScrollView
                    {
                        Padding = new Thickness(0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Content = new StackLayout
                        {
                            Padding = new Thickness(0),
                            Spacing = 0,
                            Orientation = StackOrientation.Vertical,
                            Children = 
                            {
                                this.findVenuesControl,
                            }
                        }
                    },

                    panelOkCancel
				}
			};
            Padding = new Thickness(0);
		}

        void buttonNotListed_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(NewVenuePage)) != null)
				return;
			
            NewVenuePage dialog = new NewVenuePage();
            App.Navigator.NavPage.Navigation.PushModalAsync(dialog);
            dialog.UserClickedCanceled += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();
            };
            dialog.VenueCreated += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(s1, e1);
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

			this.findVenuesControl.ReloadAsync(this.findVenuesControl.CurrentCommunity);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            this.findVenuesControl.Destroy();
        }
	}
}
