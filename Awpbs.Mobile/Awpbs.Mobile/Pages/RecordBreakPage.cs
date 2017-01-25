using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Forms.Controls;

namespace Awpbs.Mobile
{
	public class RecordBreakPage : ContentPage
	{
        public event EventHandler<SnookerBreak> Done;

        public bool IsSingleNotableMode { get; set; }

        public bool IsOpponentsBreak { get; set; }

        public bool IsEditMode
        {
            get
            {
                return this.SnookerBreak.ID > 0;
            }
        }

        public SnookerBreak SnookerBreak
        {
            get;
            private set;
        }
        SnookerMatchMetadata metadata;

        double sizeOfBalls = Config.IsTablet ? 70 : (Config.DeviceScreenHeightInInches < 4 ? 44 : 55);
        double buttonSpacing = Config.IsTablet ? 10 : (Config.DeviceScreenHeightInInches < 4 ? 2 : 5);

        Label labelTitle;
		VoiceButtonControl voiceButtonControl;

        List<Button> buttonsBalls;
        Entry entryBallsCount;
        Entry entryPoints;
        Button buttonDelete;
		CheckBox foulCheckbox;

        ScrollView panelPocketedBallsOuter;
        StackLayout panelPocketedBallsInner;
        StackLayout panelBalls;
        StackLayout panelEnteringNumbers;

        // slideout
        //BoxView boxViewSlideoutCover;
        //StackLayout panelSlideoutMetadata;
        //Button buttonHideMetadata;
        //SnookerMatchMetadataControl metadataControl;

        Grid grid;
        AbsoluteLayout absoluteLayout;

        bool ignoreUIevents = false;

		public RecordBreakPage(SnookerMatchMetadata metadata, bool isOpponentsBreak, bool isSingleNotableMode)
        {
            this.IsSingleNotableMode = isSingleNotableMode;
            this.SnookerBreak = new SnookerBreak();
            this.IsOpponentsBreak = isOpponentsBreak;
            this.metadata = metadata;

            this.init();
            this.fill();
        }

		public RecordBreakPage(SnookerBreak snookerBreak, bool isOpponentsBreak, bool isSingleNotableMode)
        {
            this.IsSingleNotableMode = isSingleNotableMode;
            this.SnookerBreak = snookerBreak.Clone();
            this.IsOpponentsBreak = isOpponentsBreak;
            this.metadata = new MetadataHelper().FromBreak(snookerBreak);

            this.init();
            this.fill();
        }

        void init()
		{
            /// title panel
            /// 
            this.labelTitle = new BybLabel()
            {
                TextColor = Color.White,
                FontFamily = Config.FontFamily,
                //FontAttributes = FontAttributes.Bold,
                FontSize = Config.LargerFontSize,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = Config.TitleHeight,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            this.labelTitle.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => { this.labelTitleClicked(); }),
                NumberOfTapsRequired = 1
            });
//            this.imageOpenMetadata = new Image()
//            {
//                Source = new FileImageSource() { File = "down.png" },
//                HeightRequest = 20,
//                WidthRequest = 20,
//            };
//            this.imageOpenMetadata.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.openMetadataSlideout(); }),
//                NumberOfTapsRequired = 1
//            });
            var panelTitle = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 20, 10, 0),
                Spacing = 0,
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            //imageOpenMetadata,
                            labelTitle,
                        }
                    },
                }
            };

            /// pocketed balls
            /// 
            this.panelPocketedBallsInner = new StackLayout()
            {
                HeightRequest = Config.SmallBallSize + 5,
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 2.0,
                Padding = new Thickness(0)
            };
            this.buttonDelete = new BybButton()
            {
				Text = "X",
                BackgroundColor = Config.ColorBlackBackground, // it's not DarkGrey, it's black
                HorizontalOptions = LayoutOptions.End,
                TextColor = Config.ColorTextOnBackground
            };
            buttonDelete.Clicked += buttonDelete_Clicked;

			this.foulCheckbox = new CheckBox () {
                IsVisible = false,
				Checked = false,
				DefaultText   = "- tap here if a foul -",
				UncheckedText = "- tap here if a foul -",
				CheckedText = "Foul",
                FontSize = Config.DefaultFontSize,
                FontName = Config.FontFamily,
                //HorizontalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
				//MinimumWidthRequest = 120
			};
			this.foulCheckbox.CheckedChanged += (s1, e1) =>
			{
				updateFoul(this.foulCheckbox.Checked);
			};
            this.panelPocketedBallsOuter = new ScrollView
            {
                Padding = new Thickness(0, 0, 0, 0),
                Orientation = ScrollOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(15,0,15,0),
                    Children = 
                    {
                        this.panelPocketedBallsInner,
					    this.foulCheckbox,
                        buttonDelete
                    }
                }
            };

            /// panel with the score
            /// 
			this.entryPoints = new BybLargeEntry2()
            {
                KeepOnlyNumbers = true,
                WidthRequest = 100,
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Placeholder = "--",
                Text = null//"0"
            };
			this.entryBallsCount = new BybLargeEntry2()
            {
                KeepOnlyNumbers = true,
                WidthRequest = 100,
                Keyboard = Keyboard.Numeric,
                BackgroundColor = Color.Transparent,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                Placeholder = "--",
                Text = null//"0"
            };
            this.entryPoints.TextChanged += entryScore_TextChanged;
            this.entryBallsCount.TextChanged += entryBallsCount_TextChanged;
            this.entryPoints.Focused += entryScore_Focused;
            this.entryBallsCount.Focused += entryScore_Focused;
            this.entryPoints.Unfocused += entryScore_Unfocused;
            this.entryBallsCount.Unfocused += entryScore_Unfocused;
            var panelScoreInner = new Grid
            {
                Padding = new Thickness(0,0,0,0),
                ColumnSpacing = 1,
                RowSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions =
                {
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Absolute) },
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition{ Height = new GridLength(1, GridUnitType.Absolute) },
                }
            };
            var panelScoreBalls = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(15, 10, 0, 10),
                Spacing = 3,
                Children =
                {
                    new BybLabel { Text = " Balls", WidthRequest = 60, TextColor = Config.ColorTextOnBackgroundGrayed, HorizontalOptions = LayoutOptions.Start },
                    this.entryBallsCount
                }
            };
            panelScoreBalls.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => 
                {
                    this.entryBallsCount.Focus(); 
                }),
                NumberOfTapsRequired = 1
            });
            panelScoreInner.Children.Add(panelScoreBalls, 0, 1);
            var panelScorePoints = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(15, 10, 0, 10),
                Spacing = 3,
                Children =
                {
                    new BybLabel { Text = " Points", WidthRequest = 60, TextColor = Config.ColorTextOnBackgroundGrayed, HorizontalOptions = LayoutOptions.Start },
                    this.entryPoints
                }
            };
            panelScorePoints.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    this.entryPoints.Focus();
                }),
                NumberOfTapsRequired = 1
            });
            panelScoreInner.Children.Add(panelScorePoints, 2, 1);
            StackLayout panelScore = new StackLayout()
            {
                //BackgroundColor = Color.Blue,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(15,0,15,0),
                Children =
                {
                    panelScoreInner
                }
            };

            /// balls
            /// 
            buttonsBalls = new List<Button>();
            foreach (var color in Config.BallColors)
            {
                Color textColor = Color.White;
                if (Config.BallColors.IndexOf(color) == 0)
                    textColor = Color.Gray;
                if (Config.BallColors.IndexOf(color) == 2)
                    textColor = Color.Gray;
                Color borderColor = Color.Black;
                if (Config.BallColors.IndexOf(color) == 7)
                    borderColor = Config.ColorTextOnBackgroundGrayed;

                var buttonBall = new BybButton
                {
                    Text = Config.BallColors.IndexOf(color) == 0 ? "x" : Config.BallColors.IndexOf(color).ToString(),
                    BackgroundColor = color,
                    BorderColor = borderColor,
                    TextColor = textColor,
                    BorderWidth = 1,
                    BorderRadius = (int)(sizeOfBalls / 2),
                    HeightRequest = sizeOfBalls,
                    MinimumHeightRequest = sizeOfBalls,
                    WidthRequest = sizeOfBalls,
                    MinimumWidthRequest = sizeOfBalls,

                    FontFamily = Config.FontFamily,
                    FontSize = Config.LargerFontSize,
                    FontAttributes = Config.BallColors.IndexOf(color) == 1 ? FontAttributes.Bold : FontAttributes.None
                };
                buttonBall.Clicked += buttonBall_Clicked;
                buttonsBalls.Add(buttonBall);
            }
            this.panelBalls = new StackLayout
            {
                Padding = new Thickness(0),
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                Spacing = buttonSpacing,
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = buttonSpacing,
                        Children =
                        {
                            buttonsBalls[7],
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = buttonSpacing,
                        Children =
                        {
                            buttonsBalls[1],
                            //buttonsBalls[0],
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = buttonSpacing,
                        Children =
                        {
                            buttonsBalls[6],
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = buttonSpacing,
                        Children =
                        {
                            buttonsBalls[5],
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = buttonSpacing,
                        Children =
                        {
                            buttonsBalls[3],
                            buttonsBalls[4],
                            buttonsBalls[2],
                        }
                    },
                }
            };

            panelEnteringNumbers = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(20, 30, 20, 20),
                Spacing = 20,
                Children =
                {
                    new BybLabel
                    {
                        Text = "Tap here when done",
                        TextColor = Config.ColorTextOnBackground,
                        HorizontalOptions = LayoutOptions.Center,
                    },
                }
            };
            panelEnteringNumbers.IsVisible = false;

            /// buttons
            /// 
            Button buttonOk = new BybButton { Text = "Done", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            Button buttonCancel = new BybButton { Text = "Cancel", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonOk,
                }
            };
            
            /// top-level Grid
            /// 
            this.grid = new Grid
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
                Padding = new Thickness(0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },                              // title
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },                          // empty space
					new RowDefinition { Height = new GridLength(80 + (Config.IsTablet ? 40 : 0), GridUnitType.Absolute) }, // score
                    new RowDefinition { Height = new GridLength(Config.IsTablet ? 60 : 35, GridUnitType.Absolute) },  // pocketed balls

                    new RowDefinition { Height = new GridLength(0, GridUnitType.Star) },  // empty space
                    new RowDefinition { Height = new GridLength(10, GridUnitType.Star) }, // balls
                    new RowDefinition { Height = new GridLength(0, GridUnitType.Star) },  // empty space

                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding * 2, GridUnitType.Absolute) },
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };
            grid.Children.Add(panelTitle, 0, 0);
            grid.Children.Add(panelScore, 0, 2);
            grid.Children.Add(panelPocketedBallsOuter, 0, 3);
            grid.Children.Add(panelBalls, 0, 5);
            //grid.Children.Add(panelVoice, 0, 5);
            grid.Children.Add(panelEnteringNumbers, 0, 5);
            grid.Children.Add(panelOkCancel, 0, 7);

            // voice button
            this.voiceButtonControl = new VoiceButtonControl()
            {
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(15, 15, 0, 0),
            };
            grid.Children.Add(this.voiceButtonControl, 0, 5);


            /// slide-out panel with metadata
            /// 
            //            this.metadataControl = new SnookerMatchMetadataControl(this.metadata, true)
            //            {
            //                Padding = new Thickness(15, 15, 15, 15)
            //            };
            //            this.metadataControl.VenueSelected += (s1, e1) => { this.removeSlideouts(); };
            //            this.metadataControl.OpponentSelected += (s1, e1) => { this.removeSlideouts(); };
            //            this.buttonHideMetadata = new BybButton() { Text = "OK", Style = (Style)App.Current.Resources["BlackButtonStyle"], HeightRequest = Config.OkCancelButtonsHeight, VerticalOptions = LayoutOptions.Start };
            //            this.buttonHideMetadata.Clicked += (s1, e1) => { this.removeSlideouts(); };
            //            this.panelSlideoutMetadata = new StackLayout()
            //            {
            //                Orientation = StackOrientation.Vertical,
            //                Padding = new Thickness(15, 0, 15, 0),
            //                HorizontalOptions = LayoutOptions.FillAndExpand,
            //                IsVisible = false,
            //                BackgroundColor = Config.ColorBackground,
            //                Children =
            //                {
            //                    this.metadataControl,
            //                    this.buttonHideMetadata
            //                }
            //            };

            /// content
            /// 
            this.absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            this.absoluteLayout.Children.Add(grid, new Point(0, 0));
            //this.absoluteLayout.Children.Add(panelSlideoutMetadata, new Rectangle(0, -100, this.Width, 100));
            Content = this.absoluteLayout;
            this.Padding = new Thickness(0, 0, 0, 0);
			this.BackgroundColor = Color.Black;

            this.updateDeleteButton();
            this.updateTitle();

			this.voiceButtonControl.PageTopLevelLayout = absoluteLayout;
        }

        void fill()
        {
            if (this.SnookerBreak.HasBalls)
            {
                this.ignoreUIevents = true;
                foreach (var ball in SnookerBreak.Balls)
                    this.addPocketedBall(ball);
                this.ignoreUIevents = false;

                this.updateScoreEntriesFromPocketedBalls();
            }
            else
            {
                this.ignoreUIevents = true;
                if (this.SnookerBreak.NumberOfBalls > 0)
                    this.entryBallsCount.Text = this.SnookerBreak.NumberOfBalls.ToString();
                else
                    this.entryBallsCount.Text = null;
                if (this.SnookerBreak.Points > 0)
                    this.entryPoints.Text = this.SnookerBreak.Points.ToString();
                else
                    this.entryPoints.Text = null;
                this.ignoreUIevents = false;
            }
        }

        void labelTitleClicked()
        {
            if (this.IsEditMode)
                return;
            if (this.IsSingleNotableMode == false)
                return;

            var page = new PickOwnerOfABreakPage(this.metadata);
            this.Navigation.PushModalAsync(page);
            page.UserMadeSelection += (s1, e1) =>
            {
                this.Navigation.PopModalAsync();
                if (e1 == null)
                    return;
                int athleteID = e1.Value;
                this.IsOpponentsBreak = athleteID != metadata.PrimaryAthleteID;
                if (this.IsOpponentsBreak && metadata.OpponentAthleteID != athleteID)
                {
                    this.metadata.OpponentAthleteID = athleteID;
                    var opponent = App.Cache.People.Get(athleteID);
                    if (opponent != null)
                    {
                        this.metadata.OpponentAthleteName = opponent.Name;
                        this.metadata.OpponentPicture = opponent.Picture;
                    }
                }
                this.updateTitle();
            };
        }

        void entryScore_Focused(object sender, FocusEventArgs e)
        {
            //this.panelVoice.IsVisible = false;
            this.panelBalls.IsVisible = false;
            this.panelEnteringNumbers.IsVisible = true;
        }

        void entryScore_Unfocused(object sender, FocusEventArgs e)
        {
            //this.panelVoice.IsVisible = true;
            this.panelBalls.IsVisible = true;
            this.panelEnteringNumbers.IsVisible = false;
        }

        void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopModalAsync();

            if (this.Done != null)
                this.Done(this, null);
        }

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
            this.SnookerBreak.Date = this.metadata.Date;
            this.SnookerBreak.OpponentAthleteID = this.metadata.OpponentAthleteID;
            this.SnookerBreak.OpponentName = this.metadata.OpponentAthleteName;
            this.SnookerBreak.VenueID = this.metadata.VenueID;
            this.SnookerBreak.VenueName = this.metadata.VenueName;
            this.SnookerBreak.TableSize = this.metadata.TableSize;

            // balls & score
            SnookerBreak.Balls = this.getScoresFromPocketedBalls();
            if (SnookerBreak.HasBalls == true)
            {
                SnookerBreak.CalcFromBalls();
            }
            else if (SnookerBreak.HasBalls == false)
            {
                int points = 0;
                if (int.TryParse(this.entryPoints.Text, out points) == false)
                    points = 0;
                int numberOfBalls;
                if (int.TryParse(this.entryBallsCount.Text, out numberOfBalls) == false)
                    numberOfBalls = 0;
                SnookerBreak.Points = points;
                SnookerBreak.NumberOfBalls = numberOfBalls;
            }

            // validate
            if (SnookerBreak.Points == 0)
            {
                await DisplayAlert("Byb", "Cannot record a 0 point break.", "OK");
                return;
            }
            if (this.IsSingleNotableMode)
            {
                string message;
                if (SnookerBreak.Validate(out message) == false)
                {
                    await DisplayAlert("Byb", "Validation - " + message, "OK");
                    return;
                }
            }
            //var dateValidator = new DateValidator();
            //if (!dateValidator.Validate(SnookerBreak.Date))
            //{
            //    DisplayAlert("Byb", "Validation - Date " + dateValidator.ErrorText, "OK");
            //    return;
            //}

            if (this.IsOpponentsBreak && this.IsSingleNotableMode)
            {
                string strYes = "Yes, a notable break for '" + metadata.OpponentAthleteName + "'";
                string answer = await this.DisplayActionSheet("Confirm", "Cancel", null, strYes);
                if (answer != strYes)
                    return;
            }

            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (this.Done != null)
                this.Done(this, this.SnookerBreak);
        }

        void buttonDelete_Clicked(object sender, EventArgs e)
        {
            if (this.panelPocketedBallsInner.Children.Count > 0)
            {
                this.panelPocketedBallsInner.Children.RemoveAt(this.panelPocketedBallsInner.Children.Count - 1);
                this.updateScoreEntriesFromPocketedBalls();
            }

            this.updateDeleteButton();
            this.updateFoulBox();
        }

        void buttonBall_Clicked(object sender, EventArgs e)
        {
            Button buttonBall = sender as Button;
            int ballScore = 0;
            int.TryParse(buttonBall.Text, out ballScore);

            if (ballScore == 0)
            {
            }
            else
            {
                this.addPocketedBall(ballScore);
            }

            this.updateScoreEntriesFromPocketedBalls();
        }

        void addPocketedBall(int ballScore)
        {
            Color color = Config.BallColors[ballScore];
            Color borderColor = color;
            if (ballScore == 7)
                borderColor = Color.Gray;
            Color textColor = Color.White;
            if (ballScore == 2)
                textColor = Color.Black;

            var ball = new BybButton
            {
                Text = "",
                BackgroundColor = color,
                BorderColor = borderColor,
                FontFamily = Config.FontFamily,
                FontSize = Config.LargerFontSize,
                TextColor = textColor,
                BorderWidth = 1,
                BorderRadius = (int)(Config.SmallBallSize / 2),
                HeightRequest = Config.SmallBallSize,
                MinimumHeightRequest = Config.SmallBallSize,
                WidthRequest = Config.SmallBallSize,
                MinimumWidthRequest = Config.SmallBallSize,
                VerticalOptions = LayoutOptions.Center
            };
            panelPocketedBallsInner.Children.Add(ball);

            this.updateDeleteButton();
            this.updateFoulBox();
            this.pronounceScore();
        }

        List<int> getScoresFromPocketedBalls()
        {
            List<int> ballScores = new List<int>();
            foreach (var view in this.panelPocketedBallsInner.Children)
            {
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);
                ballScores.Add(ballScore);
            }
            return ballScores;
        }

        void entryScore_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ignoreUIevents)
                return;
            this.panelPocketedBallsInner.Children.Clear();
            this.updateFoulBox();
        }

        void entryBallsCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ignoreUIevents)
                return;
            this.panelPocketedBallsInner.Children.Clear();
            this.updateFoulBox();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            this.grid.WidthRequest = this.Width;
            this.grid.HeightRequest = this.Height;

            base.OnSizeAllocated(width, height);
        }

        void updateScoreEntriesFromPocketedBalls()
        {
            var ballScores = this.getScoresFromPocketedBalls();

            ignoreUIevents = true;
            if (ballScores.Count == 0)
            {
                this.entryBallsCount.Text = "0";
                this.entryPoints.Text = "0";
            }
            else
            {
                this.entryBallsCount.Text = ballScores.Count().ToString();
                this.entryPoints.Text = ballScores.Sum().ToString();
            }
            ignoreUIevents = false;
        }

        void updateDeleteButton()
        {
            this.buttonDelete.IsVisible = panelPocketedBallsInner.Children.Count > 0;
        }

		void updateFoulBox()
        {
            this.foulCheckbox.IsVisible = false;

            if (1 == this.panelPocketedBallsInner.Children.Count)
            {
                var view = this.panelPocketedBallsInner.Children.ElementAt(0);
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);

                if (ballScore > 3)
                {
                    this.foulCheckbox.IsVisible = true;
                    updateFoul(this.SnookerBreak.IsFoul); 
                }
            }
            else
            {
                updateFoul(false); 
            }
        }

		void updateFoul(bool localIsFoul)
        {
			if (localIsFoul)
			{
				this.foulCheckbox.Checked = true;
				this.foulCheckbox.TextColor = Config.ColorRed;
                this.SnookerBreak.IsFoul = true; 
			}
			else
			{
				this.foulCheckbox.Checked = false;
				this.foulCheckbox.TextColor = Color.White;
                this.SnookerBreak.IsFoul = false; 
			}
        }

        void pronounceScore()
        {
            if (this.ignoreUIevents)
                return;

            var ballScores = this.getScoresFromPocketedBalls();
            int score = ballScores.Sum();
            if (score == 0)
                return;

			if (App.UserPreferences.IsVoiceOn == true)//IsVoiceOn == true)
            {
                //if (Config.IsIOS)
                //{
                    // NOTE: This was causing a huge delay for Android
                    // It doesn't seem it's really needed for iOs either
                //    var voices = App.ScorePronouncer.GetVoices();
                //}
				App.ScorePronouncer.Pronounce(score.ToString(), App.UserPreferences.Voice, App.UserPreferences.VoiceRate, App.UserPreferences.VoicePitch);
            }
        }

//        void openMetadataSlideout()
//        {
//            if (this.IsSingleNotableMode != true)
//                return;
//
//            double slideoutPadding = 20;
//            double slideoutHeight = 380;
//
//            // cover the page with a boxview to avoid taps on other elements
//            this.boxViewSlideoutCover = new BoxView() { Color = Color.FromRgba(0, 0, 0, 50) };
//            this.boxViewSlideoutCover.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.removeSlideouts(); }),
//                NumberOfTapsRequired = 1
//            });
//            this.absoluteLayout.Children.Add(boxViewSlideoutCover, new Rectangle(0, slideoutHeight + slideoutPadding, this.Width, Height - slideoutHeight - slideoutPadding));
//
//            // slide-out
//            AbsoluteLayout.SetLayoutBounds(this.panelSlideoutMetadata, new Rectangle(0, 0 - slideoutHeight - slideoutPadding, this.Width, slideoutHeight + slideoutPadding));
//            this.panelSlideoutMetadata.LayoutTo(new Rectangle(0, 20, this.Width, slideoutHeight), 250);
//            this.grid.FadeTo(0.5, 250);
//            this.panelSlideoutMetadata.IsVisible = true;
//        }

//        void removeSlideouts()
//        {
//            this.grid.FadeTo(1.0, 250);
//            //this.panelSlideoutMetadata.IsVisible = false;
//            this.boxViewSlideoutCover.IsVisible = false;
//
//            this.absoluteLayout.Children.Remove(this.boxViewSlideoutCover);
//            this.boxViewSlideoutCover = null;
//        }

        void updateTitle()
        {
            string titleText;
            if (this.IsOpponentsBreak == false)
            {
                titleText = this.metadata.PrimaryAthleteName + "'s Break";
            }
            else
            {
                string opponentsName = string.IsNullOrEmpty(this.metadata.OpponentAthleteName) ? "Opponent" : this.metadata.OpponentAthleteName;
                titleText = opponentsName + "'s Break";
            }
            if (this.IsSingleNotableMode && this.IsEditMode == false)
                titleText += " (tap to change)";
            this.labelTitle.Text = titleText;

            //this.imageOpenMetadata.IsVisible = this.IsEditMode;
        }
    }
}
