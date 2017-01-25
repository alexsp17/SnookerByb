using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using System.Linq;

namespace Awpbs.Mobile
{
    public class VoiceButtonControl : StackLayout
    {
        public Layout PageTopLevelLayout { get; set; }

        //Image image;
        Switch switcher;
        AbsoluteLayout absoluteLayout;

        double popupWidth = Config.IsTablet ? 280 : 200;

        public VoiceButtonControl()
        {
            this.Orientation = StackOrientation.Horizontal;
            this.Padding = new Thickness(0);
            this.Spacing = 0;

			var label = new BybLabel ()
			{
				Text = "Voice",
				TextColor = Config.ColorTextOnBackgroundGrayed,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
			};

			var container = new Grid() 
			{
				//OutlineColor = Config.ColorTextOnBackgroundGrayed,
                //BackgroundColor = Color.Transparent,
				//HasShadow = false,
				Padding = new Thickness(0,5,0,5),
                WidthRequest = 50,
				//Content = label,
				Children = 
				{
					label
				}
			};
			this.Children.Add(container);
			container.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { openPopup(); }) });

//            this.image = new Image()
//            {
//                Source = new FileImageSource() { File = "speaker60.png" },
//                HeightRequest = 25,
//                WidthRequest = 25,
//                HorizontalOptions = LayoutOptions.Start,
//            };
//            this.image.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { openPopup(); }) });

            //this.Children.Add(this.image);
        }

        void openPopup()
        {
            this.closePopup();

            if (this.PageTopLevelLayout == null)
                return;

            this.absoluteLayout = new AbsoluteLayout()
            {
                HeightRequest = 1000,
                WidthRequest = 1000,
            };
            if (PageTopLevelLayout as Grid != null)
                ((Grid)PageTopLevelLayout).Children.Add(this.absoluteLayout);
            else if (PageTopLevelLayout as StackLayout != null)
                ((StackLayout)PageTopLevelLayout).Children.Add(this.absoluteLayout);
            else if ((PageTopLevelLayout as AbsoluteLayout != null))
                ((AbsoluteLayout)PageTopLevelLayout).Children.Add(this.absoluteLayout);

            // a cover to cover the whole screen
			Grid panelCover = new Grid()
            {
                Opacity = 0.5,
                BackgroundColor = Color.Black,
                WidthRequest = 1000,
                HeightRequest = 1000,
            };
            panelCover.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { closePopup(); }) });
            absoluteLayout.Children.Add(panelCover, new Point(0, 0));

            // the popup
            this.switcher = new Switch()
            {
                IsToggled = App.UserPreferences.IsVoiceOn,
                VerticalOptions = LayoutOptions.Center,
            };
            switcher.Toggled += switcher_Toggled;
            Button buttonConfigure = new BybButton()
            {
                Text = "Configure ",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                HeightRequest = 40,
            };
            buttonConfigure.Clicked += (s1, e1) => { openVoiceConfigPage(); };
            Button buttonClose = new BybButton()
            {
                Text = "OK",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                HeightRequest = 40,
            };
            buttonClose.Clicked += (s1, e1) => { closePopup(); };
            absoluteLayout.Children.Add(new Frame
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0),
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(20),
                    WidthRequest = popupWidth,
                    Children =
                    {
                        new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Spacing = 10,
                            HeightRequest = 40,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new BybLabel
                                {
                                    Text = "Voice",
                                    TextColor = Color.Black,
                                    VerticalOptions = LayoutOptions.Center,
                                },
                                switcher,
                            }
                        },
                        new BybLabel
                        {
                            Text = "Remember to turn the volume on.",
                            TextColor = Config.ColorGrayTextOnWhite,
                            HeightRequest = 40,
                        },
                        buttonConfigure,
                        buttonClose,
                    }
                }
            }, new Point(PageTopLevelLayout.Width - popupWidth - 30, 30));
        }

        private void switcher_Toggled(object sender, ToggledEventArgs e)
        {
            App.UserPreferences.IsVoiceOn = this.switcher.IsToggled;
        }

        void closePopup()
        {
            if (this.PageTopLevelLayout == null)
                return;
            if (this.absoluteLayout == null)
                return;

            if (PageTopLevelLayout as Grid != null)
                ((Grid)PageTopLevelLayout).Children.Remove(this.absoluteLayout);
            else if (PageTopLevelLayout as StackLayout != null)
                ((StackLayout)PageTopLevelLayout).Children.Remove(this.absoluteLayout);
            else if ((PageTopLevelLayout as AbsoluteLayout != null))
                ((AbsoluteLayout)PageTopLevelLayout).Children.Remove(this.absoluteLayout);

            this.absoluteLayout = null;
        }

        void openVoiceConfigPage()
        {
			if (App.Navigator.GetOpenedPage (typeof(VoicePreferencesPage)) != null)
				return;
			
            this.closePopup();

            var page = new VoicePreferencesPage();
            App.Navigator.NavPage.Navigation.PushModalAsync(page);
        }
    }
}
