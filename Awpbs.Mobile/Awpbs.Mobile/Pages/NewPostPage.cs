using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class NewPostPage : ContentPage
	{
        public bool Posted { get; private set; }

        Country country;
        int metroID;

        Label labelWhere;
        Editor editorText;


        public NewPostPage(Country country, int metroID)
        {
            this.country = country;
            this.metroID = metroID;

            this.BackgroundColor = Color.White;

            // label
            this.labelWhere = new BybLabel()
            {
                Text = "",
				TextColor = Config.ColorBlackTextOnWhite,
            };
            if (metroID == 0)
            {
                this.labelWhere.Text = country.Name;
            }
            else
            {
                MetroWebModel metro = App.Cache.Metroes.Get(metroID);
                if (metro != null)
                    this.labelWhere.Text = metro.Name;
                else
                    this.labelWhere.Text = "Metro #" + metroID;
            }

            // entry
            this.editorText = new BybEditor()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 50,
            };

            // ok, cancel
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK" };
            Button buttonCancel = new BybButton { Text = "Cancel", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked;

            var stackLayout = new StackLayout
            {
                Spacing = 5,
                Padding = new Thickness(0),

                Children =
                {
                    new BybTitle("Make a Public Post") { VerticalOptions = LayoutOptions.Start },

                    new StackLayout
                    {
                        Padding = new Thickness(10,10,10,0),
                        Children =
                        {
                            new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
								Spacing = 10,
                                Children =
                                {
                                    new BybLabel { Text = "Post to:", TextColor = Config.ColorGrayTextOnWhite, VerticalTextAlignment = TextAlignment.Center },
                                    labelWhere,
                                }
                            },
                            new BybLabel { Text = "Text to post:", TextColor = Config.ColorGrayTextOnWhite, VerticalTextAlignment = TextAlignment.Center },
                            new Frame
                            {
                                HasShadow = false,
                                BackgroundColor = Config.ColorGrayBackground,
                                Padding = new Thickness(5,5,5,5),
                                Content = editorText,
                            }
                        }
                    },

                    new BoxView() { HeightRequest = 1, VerticalOptions = LayoutOptions.Fill, BackgroundColor = Color.Transparent },

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Config.ColorBackground,
                        HorizontalOptions = LayoutOptions.Fill,
                        HeightRequest = Config.OkCancelButtonsHeight,
                        Padding = new Thickness(Config.OkCancelButtonsPadding),
                        Spacing = 1,
                        Children =
                        {
                            buttonCancel,
                            buttonOk,
                        }
                    }
                }
            };

            this.Content = stackLayout;
            this.Padding = new Thickness(0, 0, 0, 0);
		}

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        private async void buttonOk_Clicked(object sender, EventArgs e)
        {
            string text = this.editorText.Text;
            if (text == null)
                text = "";
            if (text.Length > 512)
                text = text.Substring(0, 512);

            if (text.Length == 0)
            {
                await this.DisplayAlert("Byb", "Enter something to post.", "OK");
                return;
            }

            // open "please wait"
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(pleaseWaitPage);

            // send a request to the cloud
            int? postID = await App.WebService.MakePost(this.country.ThreeLetterCode, metroID, text);
            this.Posted = true;

            // close "please wait"
            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (postID == null)
            {
                await App.Navigator.DisplayAlertErrorAsync("Couldn't post. Internet issues?");
                return;
            }

            // close this dialog
            await App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            this.editorText.Focus();
        }
    }
}
