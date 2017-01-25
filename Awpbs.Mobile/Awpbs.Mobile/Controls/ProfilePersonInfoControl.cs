using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class ProfilePersonInfoControl : StackLayout
    {
        public event EventHandler ClickedOnBestBreak;
        public event EventHandler ClickedOnBestFrame;
        public event EventHandler ClickedOnContributions;
        public event EventHandler ClickedOnAbout;

        bool isMyAthlete;
        FullSnookerPlayerData fullPlayerData;

        Label labelLocation;
        Image image;

        Label labelBestBreak;
        Label labelBestFrame;
        Label labelContributions;
        Label labelAbout;

        public ProfilePersonInfoControl()
        {
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Config.ColorBackground;
            Spacing = 0;

            // Location
            this.labelLocation = new BybLabel() { Text = "", Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"], HorizontalOptions = LayoutOptions.Center };
            this.Children.Add(new StackLayout()
            {
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, 0, 0, 10),
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    this.labelLocation
                }
            });

            // image
            this.image = new Image()
            {
                HeightRequest = Config.MyImageSize,
                WidthRequest = Config.MyImageSize,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center,
            };
            StackLayout panelImage = new StackLayout()
            {
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, Config.IsTablet ? 15 : 0, 0, Config.IsTablet ? 15 : 10),
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    this.image,
                }
            };
            this.Children.Add(panelImage);

            // The 3 numbers
			Label labelBestBreakLabel = new BybLabel { Text = "Best break", Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"], VerticalTextAlignment = TextAlignment.Center, HeightRequest = 30, };
            this.labelBestBreak = new BybLabel()
            {
                Text = "",
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                BackgroundColor = Color.Transparent,
                TextColor = Config.ColorRedBackground,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
			Label labelBestFrameLabel = new BybLabel { Text = "Max frame", Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"], VerticalTextAlignment = TextAlignment.Center, HeightRequest = 30, };
            this.labelBestFrame = new BybLabel()
            {
                Text = "",
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                BackgroundColor = Color.Transparent,
                TextColor = Config.ColorRedBackground,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
			Label labelContributionsLabel = new BybLabel { Text = "Contributions", Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"], VerticalTextAlignment = TextAlignment.Center, HeightRequest = 30, };
            this.labelContributions = new BybLabel()
            {
                Text = "",
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                BackgroundColor = Color.Transparent,
                TextColor = Config.ColorRedBackground,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
            this.Children.Add(new StackLayout()
            {
                Padding = new Thickness(0, 0, 0, 0),
                Spacing = 5,
				BackgroundColor = Config.ColorBackground,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    labelBestBreakLabel,
                    this.labelBestBreak,
					new BoxView() { WidthRequest = Config.IsTablet ? 10 : 2 },
                    labelBestFrameLabel,
                    this.labelBestFrame,
					new BoxView() { WidthRequest = Config.IsTablet ? 10 : 2 },
                    labelContributionsLabel,
                    this.labelContributions
                }
            });

            // about
            this.labelAbout = new BybLabel()
            {
                LineBreakMode = LineBreakMode.WordWrap,
                Style = (Style)App.Current.Resources["LabelOnBackgroundStyle"],
                VerticalTextAlignment = TextAlignment.Start,
                HorizontalTextAlignment = TextAlignment.Center
            };
            this.Children.Add(this.labelAbout);

            /// Event handlers
            /// 
            this.image.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    this.doOnImageClicked();
                }),
                NumberOfTapsRequired = 1
            });
            this.labelBestBreak.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnBestBreak != null)
                        ClickedOnBestBreak(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            labelBestBreakLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnBestBreak != null)
                        ClickedOnBestBreak(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            this.labelBestFrame.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnBestFrame != null)
                        ClickedOnBestFrame(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            labelBestFrameLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnBestFrame != null)
                        ClickedOnBestFrame(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            this.labelContributions.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnContributions != null)
                        ClickedOnContributions(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            labelContributionsLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => {
                    if (ClickedOnContributions != null)
                        ClickedOnContributions(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
            this.labelAbout.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (ClickedOnAbout != null)
                        ClickedOnAbout(this, EventArgs.Empty);
                }),
                NumberOfTapsRequired = 1
            });
        }

        public void SetImage(string picture)
        {
            image.Source = App.ImagesService.GetImageSource(picture, BackgroundEnum.Background1, false);
        }
        
        public void FillWaiting()
        {
            this.labelLocation.Text = "...";
            this.labelBestBreak.Text = "...";
            this.labelBestFrame.Text = "...";
            this.labelContributions.Text = "...";
            this.labelAbout.Text = "...";
        }

        public void Fill(FullSnookerPlayerData fullPlayerData, bool isMyAthlete)
        {
            this.fullPlayerData = fullPlayerData;
            this.isMyAthlete = isMyAthlete;

            int? bestBreak = null;
            if (fullPlayerData != null)
                bestBreak = fullPlayerData.BestBreak;
            labelBestBreak.Text = bestBreak != null ? bestBreak.Value.ToString() : "-";

            int? bestFrame = null;
            if (fullPlayerData != null)
                bestFrame = fullPlayerData.BestFrame;
            labelBestFrame.Text = bestFrame != null ? bestFrame.Value.ToString() : "-";

			if (fullPlayerData == null || fullPlayerData.InternetIssues)
				this.labelLocation.Text = "Couldn't load data. Internet issues?";
			else
				this.labelLocation.Text = (fullPlayerData.Person != null && fullPlayerData.Person.HasMetro) ? fullPlayerData.Person.Metro : "";

            if (fullPlayerData != null)
            {
				if (fullPlayerData.Person != null && fullPlayerData.InternetIssues == false)
                {
                    this.SetImage(fullPlayerData.Person.Picture);
                    this.labelContributions.Text = fullPlayerData.Person.SnookerStats.CountContributions.ToString();
                    this.labelAbout.Text = fullPlayerData.Person.SnookerAbout;
                }
                else
                {
                    var myAthlete = App.Repository.GetMyAthlete();
                    if (myAthlete.AthleteID == fullPlayerData.AthleteID)
                    {
                        this.SetImage(myAthlete.Picture);
                        this.labelContributions.Text = "";
                        this.labelAbout.Text = myAthlete.SnookerAbout;
                    }
                }
            }
        }

        void doOnImageClicked()
        {
            if (this.isMyAthlete)
            {
                App.Navigator.OpenProfileImageEditPage();
                return;
            }

            //if (this.person != null && string.IsNullOrEmpty(this.person.Picture) == false)
            if (this.fullPlayerData != null && this.fullPlayerData.Person != null && string.IsNullOrEmpty(this.fullPlayerData.Person.Picture) == false)
                App.Navigator.OpenProfileImageViewPage(this.fullPlayerData.Person);
        }
    }
}
