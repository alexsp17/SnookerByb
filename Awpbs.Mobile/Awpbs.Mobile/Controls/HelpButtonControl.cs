using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;
using System.Linq;

namespace Awpbs.Mobile
{
    public class HelpButtonControl : StackLayout
    {
        public Layout PageTopLevelLayout { get; set; }

        AbsoluteLayout absoluteLayout;

        double popupWidth = Config.IsTablet ? 350 : 250;

        public HelpButtonControl()
        {
            this.Orientation = StackOrientation.Vertical;
            this.Padding = new Thickness(2);
            this.Spacing = 10;

			var container = new Grid()
            {
				//OutlineColor = Config.ColorTextOnBackgroundGrayed,
                BackgroundColor = Color.Transparent,
				//HasShadow = false,
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(0, 5, 0, 5),
                WidthRequest = 50,
                //Content = 
				Children = 
				{
					new BybLabel()
	                {
	                    Text = "Help",
	                    TextColor = Config.ColorTextOnBackgroundGrayed,
	                    HorizontalOptions = LayoutOptions.Center,
	                    VerticalOptions = LayoutOptions.Center,
	                }
				}
            };
            this.Children.Add(container);
			container.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { openPopup(); }) });
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
            //Frame frameCover = new Frame()
			Grid panelCover = new Grid()
            {
                Opacity = 0.5,
                BackgroundColor = Color.Black,
                WidthRequest = 1000,
                HeightRequest = 1000,
            };
            panelCover.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { closePopup(); }) });
			absoluteLayout.Children.Add(panelCover, new Point(0, 0));

            // video
            Button buttonVideo = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                Text = "watch here",
                VerticalOptions = LayoutOptions.Center,
            };
            buttonVideo.Clicked += (s1, e1) =>
            {
                App.Navigator.OpenBrowserApp("https://www.youtube.com/watch?v=PwSGdl_JgNg");
            };

            // the popup
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
                            Children =
                            {
                                new BybLabel()
                                {
                                    Text = "Rules of snooker:",
                                    TextColor = Config.ColorGrayTextOnWhite,
                                    VerticalOptions = LayoutOptions.Center,
                                },
                                buttonVideo,
                            }
                        },

                        new BybLabel
                        {
                            Text = "Tap on arrow to indicate who's at the table.",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BybLabel
                        {
                            Text = "Tap on a pocketed ball during break, swipe left or right when the break is finished.",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BybLabel
                        {
                            Text = "Suggestion: while you are at the table, your opponent can tap pocketed balls for you. The running break score will be announced after each ball, if \"Voice\" is enabled.",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BybLabel
                        {
                            Text = "\"Remaining points\", just under frame score, is based on balls remaining on the table. It can be edited for special cases (free balls, red balls pocketed as fouls, etc.).",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BybLabel
                        {
                            Text = "Fouls: use balls 4-7. There is an option to mark it as 'foul'. Assign it to the player to gets the 'foul' points.",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BybLabel
                        {
                            Text = "If you'd like to just record a match score (no frame score details) or a frame score (no break details), you can do that by tapping the score.",
                            TextColor = Config.ColorGrayTextOnWhite,
                        },

                        new BoxView()
                        {
                            BackgroundColor = Color.Transparent,
                            HeightRequest = 20,
                        },

                        buttonClose,
                    }
                }
            }, new Point(PageTopLevelLayout.Width - popupWidth - 30, 30));
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
    }
}
