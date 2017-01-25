using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class PickAthletePage : ContentPage
	{
        public event EventHandler<SelectedPersonEventArgs> UserMadeSelection;

        public string TitleText
        {
            get { return this.title.Text; }
            set { this.title.Text = value; }
        }

        public bool ShowUnknown
        {
            get
            {
                return this.buttonUnknown.IsVisible;
            }
            set
            {
                this.buttonUnknown.IsVisible = value;
            }
        }

        Button buttonUnknown;

        BybTitle title;
        FindPeopleControl findPeopleControl;

        public PickAthletePage()
		{
            this.BackgroundColor = Config.ColorGrayBackground;

            bool friendsByDefault = App.Cache.People.GetFriends().Count() > 0;

            // FindPeopleControl
            this.findPeopleControl = new FindPeopleControl(friendsByDefault)
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, 0, 0, 0)
            };
            this.findPeopleControl.UserClickedOnPerson += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, new SelectedPersonEventArgs() { Person = e1.Person });
            };

            // buttons
            Button buttonCancel = new BybButton { Text = "Cancel", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            buttonCancel.Clicked += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, new SelectedPersonEventArgs() { Person = null });
            };
            buttonUnknown = new BybButton { Text = "Unknown", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonUnknown.Clicked += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, new SelectedPersonEventArgs() { Person = null, IsUnknown = true, });
            };
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonUnknown,
                }
            };

            this.title = new BybTitle("Pick Opponent") { VerticalOptions = LayoutOptions.Start };

            // content
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Spacing = 0,
                Children = {
                    title,
                    findPeopleControl,
                    panelOkCancel
				}
			};
            Padding = new Thickness(0, 0, 0, 0);
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();

			this.findPeopleControl.ReloadAsync(this.findPeopleControl.CurrentCommunity, false);
        }
	}
}
