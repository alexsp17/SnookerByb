using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace Awpbs.Mobile
{
	public class RecordMatchPage : ContentPage
	{
		static double panelTopHeight = Config.IsTablet ? 80 : 50;
        static double panelSecondHeight = Config.App == MobileAppEnum.SnookerForVenues ? 180 : (Config.IsTablet ? 150 : 100);
        static double panelSecondPaddingTop = Config.IsTablet ? 10 : 10;
        static double panelSecondPaddingBottom = Config.IsTablet ? 10 : 5;
        static double imageSize = panelSecondHeight - panelSecondPaddingTop - panelSecondPaddingBottom;
		
        public bool IsEditMode { get; private set; }
		public bool IsReadonlyMode { get; private set; }

        public SnookerMatchScore MatchScore
        {
            get;
            private set;
        }

        // top panel
		BybBackButton buttonBack;
		Label labelTop;

        // second panel
        BybPersonImage imageYou;
        BybPersonImage imageOpponent;
		Label labelNameYou;
		Label labelNameOpponent;

        // panel with the match score, inside secondPanel
        StackLayout panelMatch;
        LargeNumberEntry2 entryMatchA;
        LargeNumberEntry2 entryMatchB;
		Label labelTapToEditMatchScore;

        // the scrollview
        ScrollView theScrollView;
		StackLayout panelEnteringMatchScore;

        // buttons at the bottom
        StackLayout panelBottom;
        Button buttonOKMatchScore;
        //Button buttonPause;
        Button buttonFinishMatch;
        Button buttonCancelMatch;
		Button buttonStartNewFrame;

        // content
        AbsoluteLayout absoluteLayout;
        Grid grid;

		public RecordMatchPage(SnookerMatchScore matchScore, bool isReadonlyMode)
        {
            this.IsEditMode = true;
            this.IsReadonlyMode = isReadonlyMode;

            this.MatchScore = matchScore.Clone();
            if (this.MatchScore.FrameScores == null)
                this.MatchScore.FrameScores = new List<SnookerFrameScore>();

            init();
            fill();
        }

		public RecordMatchPage(SnookerMatchMetadata metadata)
        {
            this.IsEditMode = false;
            
            this.MatchScore = new SnookerMatchScore();
            this.MatchScore.FrameScores = new List<SnookerFrameScore>();
            new MetadataHelper().ToScore(metadata, this.MatchScore);

            init();
            fill();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(() => { this.DisplayAlert("Byb", "Please use the < button in the top-left corner instead of the Back button", "OK"); });
            return true;
        }

        void init()
		{
            /// top panel
            /// 
			this.buttonBack = new BybBackButton ();
			this.buttonBack.Clicked += async (s1, e1) =>
            {
                await this.finishOrPauseMatch(true);
            };
            this.labelTop = new BybLabel()
            {
                TextColor = Color.White,
                FontFamily = Config.FontFamily,
				FontSize = Config.LargerFontSize,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
			if (this.IsReadonlyMode)
				this.labelTop.Text = "Match";
			else if (this.IsEditMode)
				this.labelTop.Text = "Editing Match";
			else
				this.labelTop.Text = "New Match";
            var panelTop = new Grid()
            {
				BackgroundColor = Config.ColorBlackBackground,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0,20,0,0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = 
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Absolute) },
                }
            };
            panelTop.Children.Add(buttonBack, 0, 0);
            panelTop.Children.Add(this.labelTop, 1, 0);

            /// panel with the match score, inside secondPanel
            /// 
			this.entryMatchA = new LargeNumberEntry2(Config.ColorBlackBackground)
            {
				BackgroundColor = Config.ColorBlackBackground,
                //HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
                WidthRequest = Config.App == MobileAppEnum.SnookerForVenues ? 180 : (Config.IsTablet ? 70 : 60),
                TextAlignment = TextAlignment.End,
            };
			this.entryMatchB = new LargeNumberEntry2(Config.ColorBlackBackground)
            {
				BackgroundColor = Config.ColorBlackBackground,
                //HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
                WidthRequest = Config.App == MobileAppEnum.SnookerForVenues ? 180 : (Config.IsTablet ? 70 : 60),
                TextAlignment = TextAlignment.Start,
            };
			this.entryMatchA.FocusedOnNumber += entryMatchA_FocusedOnNumber;
			this.entryMatchB.FocusedOnNumber += entryMatchB_FocusedOnNumber;
			this.entryMatchA.UnfocusedFromNumber += entryMatchA_UnfocusedFromNumber;
			this.entryMatchB.UnfocusedFromNumber += entryMatchB_UnfocusedFromNumber;
			this.entryMatchA.NumberChanged += entryMatchA_NumberChanged;
			this.entryMatchB.NumberChanged += entryMatchB_NumberChanged;
			this.labelTapToEditMatchScore = new BybLabel () 
			{
				Text = "(tap to edit)",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
			};
            this.labelTapToEditMatchScore.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.entryMatchA.Focus(); })
            });
			if (this.IsReadonlyMode || (this.IsEditMode && this.MatchScore.FrameScores.Count > 0))
				this.labelTapToEditMatchScore.IsVisible = false;
            if (Config.IsTablet)
            {
                this.labelTapToEditMatchScore.HeightRequest = 40;
            }
            this.panelMatch = new StackLayout()
            {
                IsVisible = true,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                Padding = new Thickness(0,0,0,0),
				//BackgroundColor = Color.Navy,
                Children =
                {
					new BybLabel() { Text = "Match Score", TextColor = Config.ColorTextOnBackgroundGrayed, HorizontalOptions = LayoutOptions.Center },
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 0,
                        Padding = new Thickness(0),
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                        {
							this.entryMatchA,
                            new BybLabel()
                            {
                                Text = ":",
                                FontSize = Config.App == MobileAppEnum.SnookerForVenues ? 50 : Config.VeryLargeFontSize,
                                TextColor = Config.ColorTextOnBackgroundGrayed,
                                WidthRequest = 15,
                                HeightRequest = Config.App == MobileAppEnum.SnookerForVenues ? 100 : 38,
                                VerticalOptions = LayoutOptions.End,
                                HorizontalTextAlignment = TextAlignment.Center,
								VerticalTextAlignment = TextAlignment.Center,
                            },
							this.entryMatchB,
                        }
                    },
					this.labelTapToEditMatchScore,
               }
            };

            /// second panel
            /// 
            this.imageYou = new BybPersonImage()
            {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = imageSize,
				HeightRequest = imageSize,
                BackgroundColor = Color.Transparent,
                Background = BackgroundEnum.Black,
            };
            this.imageOpponent = new BybPersonImage()
            {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = imageSize,
				HeightRequest = imageSize,
                BackgroundColor = Color.Transparent,
                Background = BackgroundEnum.Black,
            };
            this.imageYou.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => { this.imageYou_Clicked(); }),
                NumberOfTapsRequired = 1
            });
            this.imageOpponent.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => { this.imageOpponent_Clicked(); }),
                NumberOfTapsRequired = 1
            });
			this.labelNameYou = new BybLabel () {
				Text = this.MatchScore.YourName,
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
			this.labelNameOpponent = new BybLabel () {
				Text = this.MatchScore.OpponentName ?? "Opponent",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center,
			};
            var panelSecond = new Grid
            {
				BackgroundColor = Config.ColorBlackBackground,
                //HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
				Padding = new Thickness(15, panelSecondPaddingTop, 15, panelSecondPaddingBottom),
                ColumnSpacing = 0,
                RowSpacing = 0,
                ColumnDefinitions =
				{
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
					//new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) },
                },
            };
            panelSecond.Children.Add(this.imageYou, 0, 0);
            panelSecond.Children.Add(this.panelMatch, 1, 0);
            panelSecond.Children.Add(this.imageOpponent, 2, 0);
			//panelSecond.Children.Add (this.labelNameYou, 0, 1);
			//panelSecond.Children.Add (this.labelNameOpponent, 2, 1);

			/// panelEnteringMatchScore
			/// 
			panelEnteringMatchScore = new StackLayout()
			{
				IsVisible = false,
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(20,10,20,10),
				Spacing = 5,
				//HorizontalOptions = LayoutOptions.Fill,
				BackgroundColor = Config.ColorBlackBackground,
				Children =
				{
					new BybLabel
					{
						FormattedText = new FormattedString()
						{
							Spans = 
							{
								new Span() { Text = "Entering "},
								new Span() { Text = " match score", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.LargerFontSize, ForegroundColor = Color.Red },
							}
						},
						TextColor = Config.ColorTextOnBackground,
						HorizontalOptions = LayoutOptions.Center,
					},
					new BybLabel
					{
						FormattedText = new FormattedString()
						{
							Spans = 
							{
								new Span() { Text = "Tap here when done"},
							}
						},
						TextColor = Config.ColorTextOnBackground,
						HorizontalOptions = LayoutOptions.Center,
					}
				}
			};

            /// the scrollview
            /// 
            this.theScrollView = new ScrollView()
            {
				BackgroundColor = Config.ColorBlackBackground,
                //HorizontalOptions = LayoutOptions.Fill,
                //VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0),
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(0),
                    Spacing = 0,
                }
            };

            /// buttons
            /// 
            this.buttonOKMatchScore = new BybButton() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK", IsVisible = false };
            //this.buttonPause = new BybButton() { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Pause" };
            this.buttonFinishMatch = new BybButton() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = Config.App == MobileAppEnum.SnookerForVenues ? "Finish" : "Finish match" };
            this.buttonCancelMatch = new BybButton() { Style = (Style)App.Current.Resources[this.IsReadonlyMode ? "LargeButtonStyle" : "BlackButtonStyle"], Text = "Cancel" };
			this.buttonStartNewFrame = new BybButton() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Start frame" };
            this.buttonOKMatchScore.Clicked += buttonOKMatchScore_Clicked;
            //this.buttonPause.Clicked += buttonPause_Clicked;
            this.buttonFinishMatch.Clicked += buttonFinishMatch_Clicked;
            this.buttonCancelMatch.Clicked += buttonCancelMatch_Clicked;
			this.buttonStartNewFrame.Clicked += buttonStartNewFrame_Clicked;
            this.panelBottom = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
				BackgroundColor = Config.ColorBlackBackground,//.ColorBackground,
                Children =
                {
                    //buttonPause,
                    buttonFinishMatch,
                    buttonCancelMatch,
                    buttonOKMatchScore,
					buttonStartNewFrame,
                }
            };

            /// Top-level Grid
            /// 
            this.grid = new Grid
            {
                ColumnSpacing = 0,
                RowSpacing = 0,
                Padding = new Thickness(0),
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,//.FillAndExpand,
				BackgroundColor = Config.ColorBlackBackground,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(panelTopHeight, GridUnitType.Absolute) },                                                 // top panel
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },                                                              // line
					new RowDefinition { Height = new GridLength(panelSecondHeight, GridUnitType.Absolute) },                                              // second panel
                    new RowDefinition { Height = new GridLength(0, GridUnitType.Absolute) },                                                              // line
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },                                                                  // balls
                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding*2, GridUnitType.Absolute) }, // buttons
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                }
            };
            grid.Children.Add(panelTop, 0, 0);
            grid.Children.Add(panelSecond, 0, 2);
            grid.Children.Add(theScrollView, 0, 4);
            grid.Children.Add(panelEnteringMatchScore, 0, 4);
            grid.Children.Add(this.panelBottom, 0, 5);

            /// content
            /// 
            this.absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            this.absoluteLayout.Children.Add(grid, new Point(0, 0));
            //this.absoluteLayout.Children.Add(panelSlideoutButtons, new Rectangle(0, this.Height - 100, this.Width, 100));
            Content = this.absoluteLayout;
            this.Padding = new Thickness(0, 0, 0, 0);
			this.BackgroundColor = Config.ColorBlackBackground;

            //this.voiceButtonControl.PageTopLevelLayout = absoluteLayout;
		}

        void fill()
        {
            this.updateImages();
			this.fillScrollView ();
            this.updateBottomButtons();

			this.entryMatchA.Number = MatchScore.MatchScoreA;
			this.entryMatchB.Number = MatchScore.MatchScoreB;

            if (this.IsEditMode && this.MatchScore.HasFrameScores == false && this.IsReadonlyMode == false)
            {
				this.switchToEnterMatchScore(false);
            }

            if (this.IsReadonlyMode)
            {
                this.entryMatchA.IsEnabled = false;
                this.entryMatchB.IsEnabled = false;
                this.labelTapToEditMatchScore.IsVisible = false;
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
		    TraceHelper.TraceInfoForResponsiveness("OnSizeAllocated() Enter");

			if (this.grid.WidthRequest == this.Width && this.grid.HeightRequest == this.Height) {
				TraceHelper.TraceInfoForResponsiveness ("OnSizeAllocated() Exit 1");
				return;
			}

            base.OnSizeAllocated(width, height);

			if (this.grid.WidthRequest == this.Width && this.grid.HeightRequest == this.Height) {
				TraceHelper.TraceInfoForResponsiveness ("OnSizeAllocated() Exit 2");
				return; // no need to do anything
			}

            // use all of the available space for the grid
            this.grid.HeightRequest = this.Height;
            this.grid.WidthRequest = this.Width;

            // make sure that the scrollview takes up all available space within the grid
            double heightForScrollview = this.Height;
            foreach (var row in this.grid.RowDefinitions)
                if (row.Height.IsAbsolute)
                    heightForScrollview -= row.Height.Value;
            this.theScrollView.HeightRequest = heightForScrollview;

		    TraceHelper.TraceInfoForResponsiveness("OnSizeAllocated() Exit");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
		    TraceHelper.TraceInfoForResponsiveness("OnAppearing() Enter");
            base.OnAppearing();
        }

		private void entryMatchB_NumberChanged(object sender, EventArgs e)
        {
            this.updateBottomButtons();
        }

		private void entryMatchA_NumberChanged(object sender, EventArgs e)
        {
            this.updateBottomButtons();
        }

		private void entryMatchB_FocusedOnNumber(object sender, EventArgs e)
        {
            if (this.entryMatchB.Number == 0)
                this.entryMatchB.Number = null;

			this.switchToEnterMatchScore (true);
        }

		private void entryMatchA_FocusedOnNumber(object sender, EventArgs e)
        {
            if (this.entryMatchA.Number == 0)
                this.entryMatchA.Number = null;

			this.switchToEnterMatchScore(true);
        }

		private void entryMatchB_UnfocusedFromNumber(object sender, EventArgs e)
        {
			this.panelEnteringMatchScore.IsVisible = false;
        }

		private void entryMatchA_UnfocusedFromNumber(object sender, EventArgs e)
        {
			this.panelEnteringMatchScore.IsVisible = false;
        }

        private async void buttonCancelMatch_Clicked(object sender, EventArgs e)
        {
			new TempSavedMatchHelper(App.KeyChain).Remove();
			
			// just close
			await App.Navigator.NavPage.Navigation.PopModalAsync();
        }


        private async void buttonStartNewFrame_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(RecordFramePage)) != null)
				return;
			
			var frameScore = new SnookerFrameScore ();
			this.MatchScore.FrameScores.Add (frameScore);

			RecordFramePage page = new RecordFramePage (this.MatchScore, frameScore, IsEditMode);
			await App.Navigator.NavPage.Navigation.PushModalAsync (page);
            page.UserTappedDone += async () =>  { await this.onRecordFramePageDone(frameScore); };
            page.UserTappedBack += async () => { await this.onRecordFramePageDone(frameScore); };
        }

		async void editOrDeleteFramePage(SnookerFrameScore frameScore)
		{
            if (this.IsReadonlyMode)
                return;

			string strEdit = "Edit the frame";
            if (MatchScore.FrameScores.IndexOf(frameScore) == MatchScore.FrameScores.Count() - 1)
                strEdit = "Continue the frame";
			string strDelete = "Delete the frame";

			string strAnswer = await this.DisplayActionSheet ("Frame #" + (MatchScore.FrameScores.IndexOf (frameScore) + 1).ToString (), "Cancel", null, strDelete, strEdit);

			if (strAnswer == strDelete)
			{
				this.MatchScore.DeleteFrame (frameScore);
                this.MatchScore.CalculateMatchScoreFromFrameScores();
                if (this.MatchScore.HasFrameScores == false)
                {
                    this.MatchScore.MatchScoreA = 0;
                    this.MatchScore.MatchScoreB = 0;
                }
            }
			else if (strAnswer == strEdit)
			{
				RecordFramePage page = new RecordFramePage (this.MatchScore, frameScore, IsEditMode);
				await this.Navigation.PushModalAsync (page);
				page.UserTappedDone += async () => 
				{
					await this.onRecordFramePageDone (frameScore);
				};
				page.UserTappedBack += async () => 
				{
					await this.onRecordFramePageDone (frameScore);
				};
			}

			this.fill();
		}

		async Task onRecordFramePageDone(SnookerFrameScore frameScore)
		{
			//if (this.IsEditMode == false)
				new TempSavedMatchHelper (App.KeyChain).Save (MatchScore);
			
			await App.Navigator.NavPage.Navigation.PopModalAsync ();

			if (frameScore.IsEmpty)
				this.MatchScore.FrameScores.Remove(frameScore);
            this.MatchScore.CalculateMatchScoreFromFrameScores();

            this.fill();
		}

        private async void buttonFinishMatch_Clicked(object sender, EventArgs e)
        {
			await this.finishOrPauseMatch (false);
        }

        private async void buttonOKMatchScore_Clicked(object sender, EventArgs e)
        {
			int a = this.entryMatchA.Number ?? 0;
			int b = this.entryMatchB.Number ?? 0;

            if (a < 0 || b < 0 || a > 21 || b > 21 || (a == 0 && b == 0 && this.IsEditMode == false))
            {
                await DisplayAlert("Byb", "Please enter a proper match score.", "OK");
                return;
            }

            this.MatchScore.FrameScores.Clear();
            this.MatchScore.MatchScoreA = a;
            this.MatchScore.MatchScoreB = b;

            await this.finishOrPauseMatch(false);
        }

   //     private void buttonPause_Clicked(object sender, EventArgs e)
   //     {
			//this.finishOrPauseMatch (true);
   //     }

        private void imageYou_Clicked()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;
            if (this.IsReadonlyMode)
                return;
            App.Navigator.DisplayAlertRegular("Tap on the opponent picture to select the opponent.");
        }

        private async void imageOpponent_Clicked()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;
            if (this.IsReadonlyMode)
                return;
            if (this.MatchScore.OpponentAthleteID > 0)
            {
                string strCancel = "Cancel";
                string strPickOther = "Change the opponent";

                if (await this.DisplayActionSheet("Byb", strCancel, strPickOther) != strPickOther)
                    return;
            }

			await this.runPickOpponentPage ();
        }

		async Task runPickOpponentPage()
		{
			if (App.Navigator.GetOpenedPage (typeof(PickAthletePage)) != null)
				return;
			
			PickAthletePage dlg = new PickAthletePage();
			await App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
			dlg.UserMadeSelection += (s1, e1) =>
			{
				App.Navigator.NavPage.Navigation.PopModalAsync();
				if (e1.Person != null)
				{
					var person = e1.Person;
					this.MatchScore.OpponentAthleteID = person.ID;
					this.MatchScore.OpponentName = person.Name;
					this.MatchScore.OpponentPicture = person.Picture;
					this.updateImages();
				}
			};
		}

        void updateImages()
        {
            this.imageYou.SetImage(MatchScore.YourName, MatchScore.YourPicture);

            if (MatchScore.OpponentAthleteID == 0)
                this.imageOpponent.SetImagePickOpponent();
            else
                this.imageOpponent.SetImage(MatchScore.OpponentName, MatchScore.OpponentPicture);
        }

        void updateBottomButtons()
        {
			if (this.MatchScore.IsEmpty || this.IsReadonlyMode)
			{
				this.buttonCancelMatch.IsVisible = true;
				//this.buttonPause.IsVisible = false;
				this.buttonFinishMatch.IsVisible = false;
			}
			else
			{
				this.buttonCancelMatch.IsVisible = false;
				//this.buttonPause.IsVisible = true;
				this.buttonFinishMatch.IsVisible = true;
			}

			if (this.IsReadonlyMode)
				this.buttonStartNewFrame.IsVisible = false;
			else
				this.buttonStartNewFrame.Text = this.MatchScore.FrameScores.Count () == 0 ? "Start frame" : "New frame";
        }

		void switchToEnterMatchScore(bool focused)
        {
			this.BackgroundColor = Config.ColorBlackBackground;
			this.panelBottom.BackgroundColor = Config.ColorBlackBackground;

            this.theScrollView.IsVisible = false;
            this.panelEnteringMatchScore.IsVisible = focused == true;
			this.labelTapToEditMatchScore.IsVisible = this.IsReadonlyMode == false;

            this.buttonOKMatchScore.IsVisible = true;
            this.buttonCancelMatch.IsVisible = true;
            //this.buttonPause.IsVisible = false;
            this.buttonFinishMatch.IsVisible = false;
			this.buttonStartNewFrame.IsVisible = false;
        }

		private async void listOfBreaksInMatchControl_UserTappedOnBreak(object sender, SnookerBreak snookerBreak)
		{
			if (this.MatchScore.YourBreaks != null && this.MatchScore.YourBreaks.Contains(snookerBreak) == false)
				return;

			if (await this.DisplayAlert("Notable break?", "Would you like to make this a notable break?", "Yes", "Cancel") == true)
			{
				int myAthleteID = App.Repository.GetMyAthleteID();
				var allResults = App.Repository.GetResults(myAthleteID, false);

				var existingResult = (from i in allResults
					where i.Date != null
					where i.Count != null
					where System.Math.Abs((i.Date.Value - snookerBreak.Date).TotalMinutes) < 1
					where i.Count.Value == snookerBreak.Points
					select i).ToList();
				if (existingResult.Count > 0)
				{
					await this.DisplayAlert("Byb", "This break is already a notable break.", "OK");
					return;
				}

				var result = new Result();
				snookerBreak.PostToResult(result);
				result.VenueID = MatchScore.VenueID;
				result.OpponentAthleteID = MatchScore.OpponentAthleteID;
				result.AthleteID = myAthleteID;
				result.TimeModified = DateTimeHelper.GetUtcNow();
				App.Repository.AddResult(result);

				await App.Navigator.NavPage.Navigation.PopModalAsync();
				await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Breaks, true);
			}
		}

		SnookerFrameScore activeFrame = null;

		void fillScrollView()
		{
			this.labelTapToEditMatchScore.IsVisible = this.MatchScore.FrameScores.Count == 0;

            if (this.IsReadonlyMode && this.MatchScore.FrameScores.Count == 0)
                return;

			if (activeFrame == null && this.MatchScore.FrameScores.Count > 0)
				activeFrame = this.MatchScore.FrameScores [0];
			
			StackLayout stack = new StackLayout ();
			stack.Padding = new Thickness (0);
			stack.Spacing = 0;
			stack.Padding = new Thickness (Config.IsTablet ? 30 : 15, 0, Config.IsTablet ? 30 : 15, 0);
			stack.Orientation = StackOrientation.Vertical;


			//for (int iFrame = System.Math.Max (1, this.MatchScore.FrameScores.Count) - 1; iFrame >= 0; --iFrame)
			for (int iFrame = 0; iFrame < System.Math.Max (1, this.MatchScore.FrameScores.Count); ++iFrame)
			{
				int frameNumber = iFrame + 1;

                StackLayout stackCurrentFrame = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    //HorizontalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(0,10,0,0),
                    Spacing = 0,
					BackgroundColor = Config.ColorBlackBackground,
                };
                stack.Children.Add(stackCurrentFrame);

				SnookerFrameScore frame = null;
				if (iFrame < this.MatchScore.FrameScores.Count)
					frame = this.MatchScore.FrameScores [iFrame];

                var breaks = this.MatchScore.GetBreaksForFrame(iFrame + 1);

                // frame number
                var button = new SimpleButtonWithRightOrDownArrow()
                {
                    Text = "Frame " + (iFrame + 1).ToString(),
                    Padding = new Thickness(0,0,0,5),
                    HorizontalOptions = LayoutOptions.Center,
                    IsArrowVisible = breaks.Count > 0,
                    IsRight = true,
                };
                stackCurrentFrame.Children.Add(button);

                // frame score
				var panelWithScoreA = new StackLayout () {
					Orientation = StackOrientation.Vertical,
					//HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Config.ColorBackground,// Color.White,
					Children = {
						new BybLabel () {
							Text = frame != null ? frame.A.ToString () : "",
							FontFamily = Config.FontFamily,
							FontSize = Config.LargerFontSize,
							TextColor = Color.White,//Config.ColorBlackTextOnWhite,
							//HorizontalOptions = LayoutOptions.FillAndExpand,
							HorizontalTextAlignment = TextAlignment.Center,
							VerticalTextAlignment = TextAlignment.Center,
							HeightRequest = Config.IsTablet ? 70 : 40,
						}
					}
				};
				var panelWithScoreB = new StackLayout () {
					Orientation = StackOrientation.Vertical,
					//HorizontalOptions = LayoutOptions.FillAndExpand,
					BackgroundColor = Config.ColorBackground,//Color.White,
					Children = {
						new BybLabel () {
							Text = frame != null ? frame.B.ToString () : "",
							FontFamily = Config.FontFamily,
							FontSize = Config.LargerFontSize,
							TextColor = Color.White,//Config.ColorBlackTextOnWhite,
							//HorizontalOptions = LayoutOptions.FillAndExpand,
							HorizontalTextAlignment = TextAlignment.Center,
							VerticalTextAlignment = TextAlignment.Center,
							HeightRequest = Config.IsTablet ? 70 : 40,
						}
					}
				};
                var panelWithScore = new Grid()
                {
					BackgroundColor = Config.ColorBlackBackground,//Color.White,
                    //Opacity = frame == null ? 0.75 : 1.0,
                    Padding = new Thickness(0, 5, 0, 1),
                    ColumnSpacing = 1,
                    RowDefinitions =
                    {
                        new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    },
                };
                panelWithScore.Children.Add(panelWithScoreA, 0, 0);
                panelWithScore.Children.Add(panelWithScoreB, 1, 0);
                stackCurrentFrame.Children.Add(panelWithScore);

				var listOfBreaks = new ListOfBreaksInMatchControl (true);
				listOfBreaks.UserTappedOnBreak += this.listOfBreaksInMatchControl_UserTappedOnBreak;

				TapGestureRecognizer newGestureRec = new TapGestureRecognizer ();
				newGestureRec.Tapped += (s, e) => 
				{
					if (this.IsReadonlyMode)
					{
						this.openOrCloseFrame(frameNumber, button, stackCurrentFrame, listOfBreaks); 
						return;
					}

					if (frame == null)
						this.buttonStartNewFrame_Clicked(this, EventArgs.Empty);
					else
						editOrDeleteFramePage(frame);
				};

				// events
				panelWithScoreA.GestureRecognizers.Add (newGestureRec);
				panelWithScoreB.GestureRecognizers.Add (newGestureRec);


                if (frame != null && breaks.Count > 0)
                {
                    button.Clicked += (s1, e1) => 
					{
						this.openOrCloseFrame(frameNumber, button, stackCurrentFrame, listOfBreaks); 
                    };

					if (this.IsReadonlyMode) 
					{
						stackCurrentFrame.GestureRecognizers.Add (new TapGestureRecognizer () 
						{
                            Command = new Command (() =>
                            {
						        this.openOrCloseFrame(frameNumber, button, stackCurrentFrame, listOfBreaks); 
							})
						});
					}
                }
            }
			
			this.theScrollView.Content = stack;
		}

		async void openOrCloseFrame(int frameNumber, SimpleButtonWithRightOrDownArrow button, StackLayout currentFrameStack, ListOfBreaksInMatchControl listOfBreaks)
		{
			bool shouldOpen = button.IsRight;

			if (shouldOpen)
			{
				// if it's already loading, return to prevent a crash
				// this happens when you quickly do many taps on frames
				if (button.isLoading) 
				{
					TraceHelper.TraceInfoForResponsiveness(String.Format ("openOrCloseFrame(): frame {0}, ALREADY LOADING, RETURN ============================================", frameNumber));
										return;
				}

			    button.setIsLoading(true);

				Task doBreakListFilling = new Task(() =>  
				{
					listOfBreaks.Fill (MatchScore, frameNumber);
				});
				doBreakListFilling.Start();

                // at this point this function returns, UI thread is freed up, 
                // so that progress indicator becomes active
				await doBreakListFilling;

                // This takes a whole second, causes a visible delay
				TraceHelper.TraceInfoForResponsiveness(String.Format ("openOrCloseFrame(): frame {0}, BEFORE panel add", frameNumber));
                currentFrameStack.Children.Add(listOfBreaks);
				TraceHelper.TraceInfoForResponsiveness(String.Format ("openOrCloseFrame(): frame {0}, AFTER panel add", frameNumber));

                button.setIsLoading(false);
			}
			else 
			{
                int numChildren = currentFrameStack.Children.Count;
                if (numChildren > 0) 
                {
                    currentFrameStack.Children.RemoveAt (numChildren - 1);
                }
			}

		    button.IsRight = !button.IsRight;
		}

		async Task finishOrPauseMatch(bool justPause)
        {
            if (this.IsReadonlyMode || this.MatchScore.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
            {
                // just close
				new TempSavedMatchHelper(App.KeyChain).Remove();
                await App.Navigator.NavPage.Navigation.PopModalAsync();
                return;
            }

            // remove empty frames
            var emptyFrames = this.MatchScore.FrameScores.Where(i => i.IsEmpty).ToList();
            foreach (var frame in emptyFrames)
                this.MatchScore.FrameScores.Remove(frame);
            if (this.MatchScore.FrameScores.Count > 0)
                this.MatchScore.CalculateMatchScoreFromFrameScores();

            // is empty?
            bool isEmpty = this.MatchScore.MatchScoreA == 0 && this.MatchScore.MatchScoreB == 0 && this.MatchScore.HasFrameScores == false;
            bool emptiedNow = false;

            if (isEmpty == false && Config.App == MobileAppEnum.SnookerForVenues)
            {
                justPause = false;

                string strCancel = "Cancel";
                string strFinish = "Finish the match";
                string strResult = await DisplayActionSheet("Finish the match?", null, null, strFinish, strCancel);
                if (strResult != strFinish)
                    return;
            }

            if (justPause == true && isEmpty == false)
            {
                string strPause = "Pause the match";
                string strComplete = "Finish the match";
                string strDelete = "Delete the match";
                string strResult = await DisplayActionSheet("Pause or Finish?", "Cancel", null, strDelete, strPause, strComplete);
                if (strResult == strPause)
                    justPause = true;
                else if (strResult == strComplete)
                    justPause = false;
                else if (strResult == strDelete)
                {
                    if (isEmpty == false)
                    {
                        if (await this.DisplayAlert("Byb", "This match will be deleted.", "OK", "Cancel") != true)
                            return;
                    }
                    emptiedNow = true;
                    isEmpty = true;
                }
                else
                    return;
            }

            if (justPause == false)
            {
                // validate all frames
                foreach (var frame in MatchScore.FrameScores)
                {
                    if (frame.IsEmpty == true)
                        continue;

                    string validationMessage;
                    if (frame.Validate(out validationMessage) == false)
                    {
                        if (await this.DisplayAlert("Byb", "Cannot finish the match. Frame #" + (MatchScore.FrameScores.IndexOf(frame) + 1).ToString() + " is invalid. Message: " + validationMessage, "Pause the game instead", "Cancel"))
                        {
                            justPause = true;
                            break;
                        }
                        return;
                    }
                }
            }

            if (isEmpty == false && justPause == false)
            {
                if (MatchScore.OpponentAthleteID <= 0)
                {
                    string strSelectOpponent = "Select the opponent";
                    string strNoOpponent = "Save without opponent";
                    string strAnswer = await this.DisplayActionSheet("No oppponent selected", "Cancel", null, strSelectOpponent, strNoOpponent);

                    if (strAnswer == strSelectOpponent)
                    {
						await this.runPickOpponentPage ();
                        return;
                    }
                    else if (strAnswer == strNoOpponent)
                    {
                        // continue to saving
                    }
                    else
                    {
                        return;
                    }
                }

                if (Config.App == MobileAppEnum.SnookerForVenues)
                {
                    int threshold = FVOConfig.LoadFromKeyChain(App.KeyChain).NotableBreakThreshold;

                    // save Primary Player's best break
                    SnookerBreak bestBreak = this.MatchScore.YourBreaks != null ? this.MatchScore.YourBreaks.OrderByDescending(i => i.Points).FirstOrDefault() : null;
                    if (bestBreak != null && bestBreak.Points > threshold)
                    {
                        var result = new Result();
                        bestBreak.PostToResult(result);
                        result.VenueID = MatchScore.VenueID;
                        result.OpponentAthleteID = MatchScore.OpponentAthleteID;
                        result.AthleteID = MatchScore.YourAthleteID;
                        result.TimeModified = DateTimeHelper.GetUtcNow();
                        App.Repository.AddResult(result);
                    }

                    // save the Opponent's best break
                    bestBreak = this.MatchScore.OpponentBreaks != null ? this.MatchScore.OpponentBreaks.OrderByDescending(i => i.Points).FirstOrDefault() : null;
                    if (bestBreak != null && bestBreak.Points > threshold)
                    {
                        var result = new Result();
                        bestBreak.PostToResult(result);
                        result.VenueID = MatchScore.VenueID;
                        result.OpponentAthleteID = MatchScore.YourAthleteID;
                        result.AthleteID = MatchScore.OpponentAthleteID;
                        result.TimeModified = DateTimeHelper.GetUtcNow();
                        App.Repository.AddResult(result);
                    }
                }
                else if (Config.App == MobileAppEnum.Snooker)
                {
                    // save Primary Player's best break
                    SnookerBreak bestBreak = null;
                    if (this.MatchScore.YourBreaks != null)
                        bestBreak = this.MatchScore.YourBreaks.OrderByDescending(i => i.Points).FirstOrDefault();
                    if (bestBreak != null)
                    {
                        // what should be the threshold for a notable break?
                        int threshold = 20;
                        if (Config.App == MobileAppEnum.SnookerForVenues)
                        {
                            threshold = FVOConfig.LoadFromKeyChain(App.KeyChain).NotableBreakThreshold;
                        }
                        else
                        {
                            var highestEverBreak = App.Repository.GetMyBestResult();
                            if (highestEverBreak != null && highestEverBreak.Count != null)
                                threshold = (int)(highestEverBreak.Count.Value * 0.6);
                            if (threshold > 100)
                                threshold = 100;
                            if (threshold < 20)
                                threshold = 20;
                        }

                        if (bestBreak.Points >= threshold)
                        {
                            string strYes = "Yes, it's a notable break";
                            string strCancel = "Cancel";
                            string answer = await this.DisplayActionSheet("Record the break of " + bestBreak.Points.ToString() + " as a notable break?", strCancel, null, "No, not notable", strYes);
                            if (answer == null || answer == strCancel)
                                return;
                            if (answer == strYes)
                            {
                                var result = new Result();
                                bestBreak.PostToResult(result);
                                result.VenueID = MatchScore.VenueID;
                                result.OpponentAthleteID = MatchScore.OpponentAthleteID;
                                result.AthleteID = MatchScore.YourAthleteID;
                                result.TimeModified = DateTimeHelper.GetUtcNow();
                                App.Repository.AddResult(result);
                            }
                        }
                    }
                }
            }

            if (this.IsEditMode == false)
            {
                if (isEmpty == false)
                {
                    // save
                    Score score = new Score();
                    MatchScore.PostToScore(score);
                    score.TimeModified = DateTimeHelper.GetUtcNow();
                    score.Guid = Guid.NewGuid();
                    score.IsUnfinished = justPause;
                    App.Repository.AddScore(score);
                }
            }
            else
            {
                // update
                Score score = App.Repository.GetScore(MatchScore.ID);
                if (isEmpty)
                {
                    if (emptiedNow == false)
                    {
                        if (await this.DisplayAlert("Byb", "You emptied the match. Delete it?", "Yes, delete it", "No") != true)
                            return;
                    }
                    score.TimeModified = DateTimeHelper.GetUtcNow();
                    score.IsDeleted = true;
                }
                MatchScore.PostToScore(score);
                score.IsUnfinished = justPause;
                App.Repository.UpdateScore(score);
            }

			new TempSavedMatchHelper (App.KeyChain).Remove (); // no need to keep this match in the temp, because it's now saved into the database

            // close this page
            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (justPause || isEmpty)
            {
                if (Config.App != MobileAppEnum.SnookerForVenues)
                    await App.Navigator.GoToRecord();
            }
            else
            {
                if (Config.App != MobileAppEnum.SnookerForVenues)
                    await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Matches, true);
                App.Navigator.StartSyncAndCheckForNotifications();
            }
        }
    }
}
