using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;

namespace Awpbs.Mobile
{
    public class SnookerMatchMetadataControl : StackLayout
    {
        public event EventHandler VenueSelected;
        public event EventHandler OpponentSelected;

        public SnookerMatchMetadata Metadata
        {
            get;
            private set;
        }
        bool isOpponentMarkedAsUnknown;

        BybDatePicker pickerDate;

        BybPersonImage imageYou;
        BybPersonImage imageOpponent;
        Label labelYou;
        Label labelOpponent;

        Label labelVenue;
        Frame frameClearVenue;

        BybNoBorderPicker pickerTable;

        public SnookerMatchMetadataControl(SnookerMatchMetadata metadata, bool showPlayers)//, bool pausedMatchMode = false)
        {
            //this.PausedMatchMode = pausedMatchMode;

            this.Orientation = StackOrientation.Vertical;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.Padding = new Thickness(0);
            this.Spacing = 0;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.Start;

            // date
			Label labelDateLabel = new BybLabel ()
			{
				Text = "Date",
				WidthRequest = 65,
				TextColor = Config.ColorTextOnBackgroundGrayed,
				VerticalTextAlignment = TextAlignment.Center,
				VerticalOptions = LayoutOptions.Center,
			};
            this.pickerDate = new BybDatePicker()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				HeightRequest = Config.LargeButtonsHeight + 8,
                Format = "D",
                MinimumDate = new DateTime(1980, 1, 1),
                MaximumDate = DateTime.Now.Date,
            };
            this.pickerDate.DateSelected += pickerDate_DateSelected;
			Image imageDate = new Image () {
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource () { File = "arrowRight.png" }
			};
			var panelDate = new StackLayout ()
			{
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness (12, 0, 12, 0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Children = {
					labelDateLabel,
					this.pickerDate,
					imageDate,
				}
			};
//			imageDate.GestureRecognizers.Add (new TapGestureRecognizer () {
//				Command = new Command (() => {
//					this.pickerDate.Focus();
//				}),
//			});
			labelDateLabel.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.pickerDate.Focus(); })});
			panelDate.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.pickerDate.Focus(); })});
            this.Children.Add(panelDate);
            this.Children.Add(new BoxView { Color = Color.Transparent, HeightRequest = 1 });

            // venue
			Label labelVenueLabel = new BybLabel {
				Text = "Venue",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				WidthRequest = 65,
				VerticalTextAlignment = TextAlignment.Center
			};
            this.labelVenue = new BybLabel()
            {
                TextColor = Color.Black,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
				//BackgroundColor = Color.Aqua,
            };
			Image imageVenue = new Image () {
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource () { File = "arrowRight.png" },
				BackgroundColor = Color.White,
			};
            Image imageClearVenue = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = Config.RedArrowImageSize,
                HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "delete.png" },
                BackgroundColor = Color.White,
            };
            this.frameClearVenue = new Frame()
            {
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0),
                BackgroundColor = Color.White,
                Content = imageClearVenue,
                IsVisible = false,
            };
            this.frameClearVenue.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.clearVenueClicked(); })
            });
            var panelVenue = new StackLayout () {
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (12, 0, 12, 0),
				HeightRequest = Config.LargeButtonsHeight + 8,//50,
				Children = {
					labelVenueLabel,
					labelVenue,
                    frameClearVenue,
					imageVenue,
				}
			};
			labelVenueLabel.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { venueClicked(); })});
			labelVenue.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { venueClicked(); })});
			panelVenue.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { venueClicked(); })});
			this.Children.Add(panelVenue);

            this.Children.Add(new BoxView { Color = Color.Transparent, HeightRequest = 1 });

            // table
			Label labelTableLabel = new BybLabel
			{
				Text = "Table",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				WidthRequest = 65,
				VerticalTextAlignment = TextAlignment.Center
			};
            this.pickerTable = new BybNoBorderPicker()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
            };
            this.pickerTable.Items.Add("10' table");
            this.pickerTable.Items.Add("12' table");
            this.pickerTable.SelectedIndex = 1;
            this.pickerTable.SelectedIndexChanged += pickerTable_SelectedIndexChanged;
			Image imageTable = new Image () {
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource () { File = "arrowRight.png" }
			};
			var panelTable = new StackLayout () {
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				Padding = new Thickness (12, 0, 12, 0),
				HeightRequest = Config.LargeButtonsHeight + 8,// 50,
				Children = {
					labelTableLabel,
					this.pickerTable,
					imageTable,
				}
			};
			labelTableLabel.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.pickerTable.Focus(); })});
//			imageTable.GestureRecognizers.Add (new TapGestureRecognizer () {
//				Command = new Command (() => {
//					this.pickerTable.Focus();
//				})
//			});
			panelTable.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.pickerTable.Focus(); })});
            this.Children.Add(panelTable);

            this.Children.Add(new BoxView { Color = Color.Transparent, HeightRequest = 1 });

            // what should be the image size?
            double imageSize = 100;// Config.DeviceScreenHeightInInches < 4 ? 80 : 110;

            // you vs opponent
            this.imageYou = new BybPersonImage()
            {
                //HorizontalOptions = LayoutOptions.Fill,
				//VerticalOptions = LayoutOptions.Center,
                Background = BackgroundEnum.White,
                UseNameAbbreviationIfNoPicture = false,
				//BackgroundColor = Color.Red,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 200,
				HeightRequest = 200,
            };
            this.imageOpponent = new BybPersonImage()
            {
                //HorizontalOptions = LayoutOptions.Fill,
				//VerticalOptions = LayoutOptions.Center,
                Background = BackgroundEnum.White,
                UseNameAbbreviationIfNoPicture = false,
				//BackgroundColor = Color.Yellow,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = 200,
				HeightRequest = 200,
            };
            this.labelYou = new BybLabel()
            {
                TextColor = Config.ColorBlackTextOnWhite,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "You"
            };
            this.labelOpponent = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
				TextColor = Config.ColorBlackTextOnWhite,
            };
            Grid gridWithImages = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                BackgroundColor = Color.White,
            };
            if (showPlayers)
                this.Children.Add(gridWithImages);
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Absolute) });
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(12, GridUnitType.Absolute) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(imageSize, GridUnitType.Absolute) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            gridWithImages.Children.Add(this.imageYou, 0, 1);
            gridWithImages.Children.Add(this.imageOpponent, 2, 1);
            gridWithImages.Children.Add(this.labelYou, 0, 2);
            gridWithImages.Children.Add(this.labelOpponent, 2, 2);
            gridWithImages.Children.Add(new BoxView() { BackgroundColor = Config.ColorGrayBackground }, 1, 2, 0, 3);

            this.imageYou.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { youClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.labelYou.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { youClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.imageOpponent.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { opponentClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.labelOpponent.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { opponentClicked(); }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(new BoxView { Color = Color.Transparent, HeightRequest = 1 });

            this.Fill(metadata);

            //if (this.PausedMatchMode)
            //{
            //    this.pickerDate.IsEnabled = false;
            //    this.pickerTable.IsEnabled = false;
            //    this.labelVenue.IsEnabled = false;
            //    this.labelYou.IsEnabled = false;
            //    this.labelOpponent.IsEnabled = false;

            //    this.pickerDate.Opacity = 0.5;
            //    this.pickerTable.Opacity = 0.5;
            //    this.labelVenue.Opacity = 0.5;
            //    this.labelYou.Opacity = 0.5;
            //    this.labelOpponent.Opacity = 0.5;
            //}
        }

        public void Fill(SnookerMatchMetadata metadata)
        {
            this.Metadata = metadata;
            this.isOpponentMarkedAsUnknown = false;
            this.fill();
        }

		public void Refill()
		{
			this.fill ();
		}

        void fill()
        {
            this.pickerDate.Date = this.Metadata.Date;

            var me = App.Repository.GetMyAthlete();
            this.imageYou.SetImage(me.Name, me.Picture);

            if (this.Metadata.HasOpponentAthlete == false && isOpponentMarkedAsUnknown)
            {
                this.imageOpponent.SetImage(this.Metadata.OpponentAthleteName, this.Metadata.OpponentPicture);
                this.labelOpponent.Text = "Unknown";
            }
            else if (this.Metadata.HasOpponentAthlete == false)
            {
                this.imageOpponent.SetImagePickOpponent();
                this.labelOpponent.Text = "Pick opponent";
                this.labelOpponent.TextColor = Config.ColorBlackTextOnWhite;
            }
            else
            {
                this.imageOpponent.SetImage(this.Metadata.OpponentAthleteName, this.Metadata.OpponentPicture);
                this.labelOpponent.Text = this.Metadata.OpponentAthleteName;
            }

            if (this.Metadata.HasVenue)
            {
                this.labelVenue.Text = this.Metadata.VenueName;
            }
            else
            {
                this.labelVenue.FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span() { Text = "Pick venue ", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize, ForegroundColor = Color.Black, },
                        new Span() { Text = " (optional)", FontAttributes = FontAttributes.None, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize, ForegroundColor = Config.ColorGrayTextOnWhite, },
                    }
                };
            }
            //this.labelVenue.Text = this.Metadata.HasVenue ? this.Metadata.VenueName : "Pick venue (optional)";
            if (this.Metadata.TableSize == SnookerTableSizeEnum.Table10Ft)
                this.pickerTable.SelectedIndex = 0;
            else
                this.pickerTable.SelectedIndex = 1;

            this.frameClearVenue.IsVisible = Metadata.HasVenue;

            // animate the venue name
            if (Metadata.HasVenue)
            {
                int timerCount = 0;
                Timer timer = new Timer();
                timer.Interval = 250;
                timer.Elapsed += (s1, e1) =>
                {
                    timerCount++;
                    if (timerCount > 9)
                    {
                        timer.Stop();
                        timer.Dispose();
                        return;
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (timerCount % 2 == 0)
                            this.labelVenue.TextColor = Color.Red;
                        else
                            this.labelVenue.TextColor = Color.Black;
                    });
                };
                timer.Start();
            }
        }

        void venueClicked()
        {
			if (App.Navigator.GetOpenedPage (typeof(PickVenuePage)) != null)
				return;
			
            PickVenuePage dlg = new PickVenuePage();
            App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
            dlg.Disappearing += (s1, e1) =>
            {
                if (this.VenueSelected != null)
                    this.VenueSelected(this, EventArgs.Empty);
            };
            dlg.UserMadeSelection += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();
                var venue = e1;
                if (venue != null)
                {
                    this.Metadata.VenueID = venue.ID;
                    this.Metadata.VenueName = venue.Name;
                    App.Cache.Venues.Put(venue);
                }
                //else
                //{
                //    this.Metadata.VenueID = 0;
                //    this.Metadata.VenueName = null;
                //}
                this.fill();
            };
        }

        void clearVenueClicked()
        {
            this.Metadata.VenueID = 0;
            this.Metadata.VenueName = "";
            this.fill();
        }

        void youClicked()
        {
            //if (this.PausedMatchMode)
            //    return;

            App.Navigator.DisplayAlertRegular("Tap on the opponent picture to select the opponent.");
        }

        void opponentClicked()
        {
            //if (this.PausedMatchMode)
            //    return;

			if (App.Navigator.GetOpenedPage (typeof(PickAthletePage)) != null)
				return;

            PickAthletePage dlg = new PickAthletePage();
            App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
            dlg.UserMadeSelection += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();
                if (e1.Person != null)
                {
                    this.Metadata.OpponentAthleteID = e1.Person.ID;
                    this.Metadata.OpponentAthleteName = e1.Person.Name;
                    this.Metadata.OpponentPicture = e1.Person.Picture;
                    this.isOpponentMarkedAsUnknown = false;
                }
                else if (e1.IsUnknown)
                {
                    this.Metadata.OpponentAthleteID = 0;
                    this.Metadata.OpponentAthleteName = "";
                    this.Metadata.OpponentPicture = "";
                    this.isOpponentMarkedAsUnknown = true;
                }
                if (this.OpponentSelected != null)
                    this.OpponentSelected(this, EventArgs.Empty);
                this.fill();
            };
        }

        private void pickerTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.pickerTable.SelectedIndex == 0)
                this.Metadata.TableSize = SnookerTableSizeEnum.Table10Ft;
            else
                this.Metadata.TableSize = SnookerTableSizeEnum.Table12Ft;
        }

        private void pickerDate_DateSelected(object sender, DateChangedEventArgs e)
        {
            this.Metadata.Date = this.pickerDate.Date.Date;
        }
    }
}
