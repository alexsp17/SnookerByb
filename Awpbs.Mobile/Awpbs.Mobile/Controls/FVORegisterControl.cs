using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class FVORegisterControl: Grid
    {
        public event Action UserClickedCancel;

        public event Action<PersonBasicWebModel> UserRegistered;

        Entry editorName;
        Entry editorEmail;
        //Entry editorPin;

        public void Clear()
        {
            this.editorName.Text = "";
            this.editorEmail.Text = "";
            //this.editorPin.Text = "";
        }

        public FVORegisterControl()
        {
            editorName = new BybStandardEntry()
            {
                Placeholder = "Your name",
                Keyboard = Keyboard.Text,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
            };
            editorEmail = new BybStandardEntry()
            {
                Placeholder = "Your e-mail address",
                Keyboard = Keyboard.Email,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
            };
            //editorPin = new BybPinEntry()
            //{
            //    Placeholder = "Access pin (4 numbers)",
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    HeightRequest = 40,
            //};

            editorName.Completed += (s1, e1) => {
                editorEmail.Focus();
            };
   //         editorEmail.Completed += (s1, e1) => {
   //             editorPin.Focus();
			//};

            Button buttonOk = new BybButton()
            {
                Text = "Register",
                Style = (Style)App.Current.Resources["LargeButtonStyle"]
            };
            buttonOk.Clicked += buttonOk_Clicked;
            Button buttonCancel = new BybButton()
            {
                Text = "Cancel",
                Style = (Style)App.Current.Resources["BlackButtonStyle"]
            };
            buttonCancel.Clicked += buttonCancel_Clicked;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(0),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonOk,
                }
            };

            this.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(10,10,10,10),
                Children =
                {
                    editorName,
                    editorEmail,
                    //editorPin,
                    panelOkCancel,
                    new BoxView() { HeightRequest = 10 },
                    new BybLabel()
                    {
                        Text = "Register here or\r\ndownload 'Snooker Byb app' on your device.\r\n\r\nOnly register once, your account will work everywhere on Planet Earth.",
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Config.ColorTextOnBackgroundGrayed,
                        HorizontalTextAlignment = TextAlignment.Center,
                    },
                    new BoxView() { HeightRequest = 10 },
                }
            });
            this.BackgroundColor = Config.ColorBackground;
        }

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            this.Clear();

            if (this.UserClickedCancel != null)
                this.UserClickedCancel();
        }

        private async void buttonOk_Clicked(object sender, EventArgs e)
        {
            string name = editorName.Text ?? "";
            name = name.Trim();
            if (name.Length < 3)
            {
                await App.Navigator.DisplayAlertRegularAsync("Please enter a proper name");
                return;
            }

            string email = editorEmail.Text ?? "";
            email = email.Trim();
            if (new EmailAddressHelper().Validate(email) == false)
            {
                await App.Navigator.DisplayAlertRegularAsync("Not a proper email");
                return;
            }

            //string pin = this.editorPin.Text;
            //if (new AccessPinHelper().Validate(pin) == false)
            //{
            //    await App.Navigator.DisplayAlertRegularAsync("The pin must be 4 DIGITS.");
            //    return;
            //}

            App.LoginAndRegistrationLogic.AskUserToEnterPin(true, true,
                (pin) =>
                {
                    this.register(pin, name, email);
                },
                () =>
                {
                    this.Clear();
                });

            // ask the user to confirm the pin
            //var page = new EnterPinPage(true);
            //page.TheTitle = "Confirm Your PIN";
            //await this.Navigation.PushModalAsync(page);
            //page.UserClickedCancel += () =>
            //{
            //    this.Navigation.PopModalAsync();
            //};
            //page.UserEnteredPin += () =>
            //{
            //    this.Navigation.PopModalAsync();

            //    if (string.Compare(page.Pin, pin, false) != 0)
            //    {
            //        App.Navigator.DisplayAlertRegular("The pins don't match.");
            //        return;
            //    }

            //    this.register(pin, name, email);
            //};
        }

        async void register(string pin, string name, string email)
        {
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            await this.Navigation.PushModalAsync(pleaseWaitPage);

            string password = pin;
            if (password == null)
                password = "";

            // register
            var config = FVOConfig.LoadFromKeyChain(App.KeyChain);
            int? athleteID = await App.WebService.RegisterFVO(email, password, name, config.VenueID);
            if (athleteID == null)
            {
                await this.Navigation.PopModalAsync();
                await App.Navigator.DisplayAlertErrorAsync("Could not register. Internet issues? Already registered?");
                return;
            }

            this.Clear();

            // load the athlete record
            var athlete = await App.WebService.GetPersonByID(athleteID.Value);
            if (athlete == null)
            {
                await this.Navigation.PopModalAsync();
                await App.Navigator.DisplayAlertErrorAsync("Unspecified error. Internet issues?");
                return;
            }

            await this.Navigation.PopModalAsync();

            if (this.UserRegistered != null)
                this.UserRegistered(athlete);
        }
    }
}
