using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class SendMessagePage : ContentPage
	{
        PersonFullWebModel person;

        Editor editorNote;
        BybSwitch switchShareMyEmail;
        //Switch switchFriendRequest;

        public SendMessagePage(PersonFullWebModel person)
		{
            this.person = person;

            // ok, cancel
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Send" };
            buttonOk.Clicked += buttonOk_Clicked;
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += buttonCancel_Clicked;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorDarkGrayBackground,
                HorizontalOptions = LayoutOptions.Fill,
				Padding = new Thickness(0,Config.OkCancelButtonsPadding,0,Config.OkCancelButtonsPadding),
                Spacing = 1,
                HeightRequest = Config.OkCancelButtonsHeight,
                Children =
                {
                    buttonCancel,
                    buttonOk,
                }
            };

            editorNote = new BybEditor() { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 40 };
            switchShareMyEmail = new BybSwitch();
            switchShareMyEmail.IsToggled = true;
            //switchFriendRequest = new Switch();
            //switchFriendRequest.IsToggled = person.IsFriend == false && person.IsFriendRequestSent == false;

            // content
			Content = new StackLayout {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15),
                Spacing = 5,
				Children = {
                        new BybLabel()
                        {
                            Text = "Your message to '" + this.person.Name + "':"
                        },
                        new Frame
                        {
							HasShadow = false,
							BackgroundColor = Config.ColorGrayBackground,
							Padding = new Thickness(5,5,5,5),
							Content = editorNote,
							HeightRequest = 60,
							HorizontalOptions = LayoutOptions.FillAndExpand,

//                                Padding = new Thickness(0),
//                                HorizontalOptions = LayoutOptions.FillAndExpand,
//                                HeightRequest = 60,
//                                Content = editorNote
                        },
                        new BybLabel()
                        {
                            Text = "'" + this.person.Name + "' will receive an email with the message and will be able to open your Byb profile from the email.",
							TextColor = Config.ColorGrayTextOnWhite,
                        },
//                        new StackLayout
//                        {
//                            Orientation = StackOrientation.Horizontal,
//                            Children = 
//                            {
//                                new BybLabel { Text = "Share your email address?", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.FillAndExpand },
//                                this.switchShareMyEmail
//                            }
//                        },
	                    //new BoxView() { VerticalOptions = LayoutOptions.FillAndExpand },
						new BoxView() { HeightRequest = 1, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.Transparent },
	                    panelOkCancel
				}
			};
            Padding = new Thickness(0);
            Title = "Send a Message";
			this.BackgroundColor = Config.ColorBackgroundWhite;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			this.editorNote.Focus();
		}

        void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopAsync();
        }

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
            string emailText = this.editorNote.Text;
            if (emailText == null)
                emailText = "";
            bool shareMyEMail = this.switchShareMyEmail.IsToggled;

            //bool friendRequest = this.switchFriendRequest.IsToggled && person.IsFriend == false && person.IsFriendRequestSent == false;

            this.IsBusy = true;

            //bool ok1 = true;
            bool ok2 = true;

            //if (friendRequest)
            //    ok1 = await App.WebService.RequestFriend(person.ID);
            ok2 = await App.WebService.SendMessage(person.ID, emailText, shareMyEMail);

            this.IsBusy = false;

            if (!ok2)
            {
                App.Navigator.DisplayAlertError("Failed to send the message. Internet issues?");
                return;
            }

            await App.Navigator.NavPage.Navigation.PopAsync();
            App.Navigator.DisplayAlertRegular("An email has been sent to " + person.Name + ". It takes a minute to get processed.");
        }
	}
}
