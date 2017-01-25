using System;
using System.Collections.Generic;
using System.Linq;
//using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class ReloginPage : ContentPage
    {
        public event EventHandler UserWantsToCancel;

        //BybTitle title;
        Label labelDescription;
        Label labelReloginInProgress;

        Button newUserButton;
        Button existingUserButton;
        Button cancelButton;

        public ReloginPage(string description)
        {
            //this.title = new BybTitle(Config.ProductName);

            this.labelDescription = new BybLabel()
            {
				FontSize = Config.LargerFontSize,
				TextColor = Config.ColorBlackTextOnWhite,
                Text = description
            };

            this.labelReloginInProgress = new BybLabel()
            {
				FontSize = Config.LargerFontSize,
				TextColor = Config.ColorBlackTextOnWhite,
                Text = "",
                IsVisible = false
            };

            newUserButton = new BybButton() { Text = "New user", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            newUserButton.Clicked += newUserButton_Clicked;

            existingUserButton = new BybButton() { Text = "Existing user", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            existingUserButton.Clicked += existingUserButton_Clicked;

            cancelButton = new BybButton() { Text = "Cancel", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            cancelButton.Clicked += cancelButton_Clicked;

            this.Content = new StackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"]},
                    new StackLayout
                    {
                        VerticalOptions = LayoutOptions.StartAndExpand,
                        Padding = new Thickness(40,0,40,0),
                        Spacing = 15,
                        Children =
                        {
                            this.labelDescription,
                            this.labelReloginInProgress,
                            existingUserButton,
                            newUserButton,
                            cancelButton
                        }
                    },
                }
            };

            Title = Config.ProductName;
            this.Padding = new Thickness(0);
        }

        private void cancelButton_Clicked(object sender, EventArgs e)
        {
            if (this.UserWantsToCancel != null)
                this.UserWantsToCancel(this, EventArgs.Empty);
        }

        public bool ShowOtherOptions
        {
            get
            {
                return this.newUserButton.IsEnabled == true;
            }
            set
            {
                this.newUserButton.IsEnabled = value;
                this.existingUserButton.IsEnabled = value;
            }
        }

        public void ShowAutoRelogin(bool inProgress)
        {
            this.labelReloginInProgress.IsVisible = true;

            if (inProgress)
                this.labelReloginInProgress.Text = "Trying to automatically re-login. Please wait...";
            else
                this.labelReloginInProgress.Text = "Could NOT automatically re-login. Please use one of the options below:";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void newUserButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartSetupNewUser();
        }

        private void existingUserButton_Clicked(object sender, EventArgs e)
        {
            App.LoginAndRegistrationLogic.StartSetupExistingUser();
        }
    }

}
