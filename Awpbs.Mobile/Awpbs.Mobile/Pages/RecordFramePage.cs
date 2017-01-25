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
	public class RecordFramePage : ContentPage
	{
		static double panelTopHeight = Config.IsTablet ? 80 : 50;
		static double panelSecondHeight = Config.App == MobileAppEnum.SnookerForVenues ? 180 : (Config.IsTablet ? 150 : 100);
		static double panelSecondPaddingTop = Config.IsTablet ? 10 : 10;
        static double panelSecondPaddingBottom = Config.IsTablet ? 10 : 5;
        static double imageSize = panelSecondHeight - panelSecondPaddingTop - panelSecondPaddingBottom;
		static double defaultHeightOfPottedBallsPanel = Config.App == MobileAppEnum.SnookerForVenues ? 150 : (Config.IsTablet ? 220 : 60);
        static double heightOfLabelPastBreaks = Config.IsTablet ? 50 : 32;

        public event Action UserTappedDone;
        public event Action UserTappedBack;

        public SnookerMatchScore MatchScore
        {
            get;
            private set;
        }

        public SnookerFrameScore CurrentFrameScore
        {
            get;
            private set;
        }

		private bool isMatchEditMode;

        // top panel
        Grid panelTop;
		BybBackButton buttonBack;
        Label labelTop;
        Label labelDone;

        // second panel
        Grid panelSecond;
        BybPersonImage imageYou;
        BybPersonImage imageOpponent;

        // panel with the frame score, inside secondPanel
        StackLayout panelFrame;
        LargeNumberEntry2 entryCurrentFrameA;
        LargeNumberEntry2 entryCurrentFrameB;
		//Label labelTapToEditFrameScore;
		Label labelMatchScore;
		Label labelMatchScoreDigits;
        Label labelPointsLeft;

        // the current break and the previous breaks
        SnookerBreakControl snookerBreakControl;
        SimpleButtonWithLittleDownArrow labelPastBreaks;
        Label labelPastBreaks2;
		StackLayout panelPastBreaks;
        ListOfBreaksInMatchControl listOfBreaksInMatchControl;
        StackLayout panelEnteringFrameScore;
        Label labelEnteringFrameScore;

        // content
        AbsoluteLayout absoluteLayout;

		public RecordFramePage (SnookerMatchScore matchScore, SnookerFrameScore frameScore, bool isMatchEditMode)
        {
			this.MatchScore = matchScore;
			this.CurrentFrameScore = frameScore;
			this.isMatchEditMode = isMatchEditMode;

            init();
			updateImages();

            if (frameScore.IsEmpty == false)
            {
                this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);

            }
            else
            {
                this.CurrentFrameScore.A = 0;
                this.CurrentFrameScore.B = 0;
            }

            updateFrameScoreInSnookerBreakControl();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(() => { this.DisplayAlert("Byb", "Please use the < button in the top-right corner instead of the Back button", "OK"); });
            return true;
        }

        void init()
		{
            /// top panel
            /// 
            
			this.buttonBack = new BybBackButton ()
			{
				LabelText = Config.App == MobileAppEnum.SnookerForVenues ? "All frames" : ""
			};
			this.buttonBack.Clicked += (s1, e1) =>
			{
				if (this.UserTappedBack != null)
					UserTappedBack();
			};
            this.labelTop = new BybLabel()
            {
				Text = "Frame " + (MatchScore.FrameScores.IndexOf(CurrentFrameScore) + 1).ToString(),
                TextColor = Color.White,
                FontFamily = Config.FontFamily,
				FontSize = Config.LargerFontSize,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            this.labelDone = new BybLabel()
            {
                Text = "Done",
                TextColor = Color.White,
                FontFamily = Config.FontFamily,
                FontSize = Config.LargerFontSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center,
            };
            Grid buttonDone = new Grid()
            {
                Padding = new Thickness(0,0,Config.IsTablet ? 20 : 10,0),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //HasShadow = false,
                BackgroundColor = Config.ColorBlackBackground,
                //Content = this.labelDone,
				Children = 
				{
					this.labelDone
				}
            };
            buttonDone.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (this.UserTappedDone != null)
                        UserTappedDone();
                })
            });
            this.panelTop = new Grid()
            {
				BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.FillAndExpand,
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
                    new ColumnDefinition() { Width = new GridLength(Config.App == MobileAppEnum.SnookerForVenues ? 200 : 100, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(Config.App == MobileAppEnum.SnookerForVenues ? 200 : 100, GridUnitType.Absolute) },
                }
            };
            panelTop.Children.Add(buttonBack, 0, 0);
            panelTop.Children.Add(this.labelTop, 1, 0);
            panelTop.Children.Add(buttonDone, 2, 0);
            if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                // add a horizontal line
                panelTop.Children.Add(new BoxView()
                {
                    HeightRequest = 1,
                    BackgroundColor = Config.ColorBackground,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End,
                }, 0, 3, 0, 1);
            }

            /// panel with the frame score, inside secondPanel
            /// 
			this.entryCurrentFrameA = new LargeNumberEntry2(Config.ColorBlackBackground)
            {
				BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
                WidthRequest = Config.App == MobileAppEnum.SnookerForVenues ? 200 : (Config.IsTablet ? 70 : 60),
                TextAlignment = TextAlignment.End,
            };
			this.entryCurrentFrameB = new LargeNumberEntry2(Config.ColorBlackBackground)
            {
				BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
                WidthRequest = Config.App == MobileAppEnum.SnookerForVenues ? 200 : (Config.IsTablet ? 70 : 60),
                TextAlignment = TextAlignment.Start,
            };
			this.labelMatchScore = new BybLabel () 
			{
				//Text = "Edit score",
				Text = "Match score",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
			};
            this.labelMatchScoreDigits = new BybLabel()
            {
                //Text = "Edit score",
                Text = (this.MatchScore.MatchScoreA).ToString() + ":" + (this.MatchScore.MatchScoreB).ToString(),
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
			};

            /*
			this.labelTapToEditFrameScore = new BybLabel () 
			{
				//Text = "Edit score",
				Text = "Frame score",
				TextColor = Config.ColorTextOnBackgroundGrayed,
				HorizontalOptions = LayoutOptions.Center,
			};
            */
            this.labelPointsLeft = new BybLabel()
            {
                Text = "",
                TextColor = Config.ColorTextOnBackgroundGrayed,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            if (Config.IsTablet)
            {
                this.labelPointsLeft.HeightRequest = 40;
            }
            this.entryCurrentFrameA.FocusedOnNumber += entryCurrentFrameA_FocusedOnNumber;
            this.entryCurrentFrameB.FocusedOnNumber += entryCurrentFrameB_FocusedOnNumber;
            this.entryCurrentFrameA.UnfocusedFromNumber += entryCurrentFrameA_UnfocusedFromNumber;
            this.entryCurrentFrameB.UnfocusedFromNumber += entryCurrentFrameB_UnfocusedFromNumber;
            this.entryCurrentFrameA.NumberChanged += entryCurrentFrameA_NumberChanged;
            this.entryCurrentFrameB.NumberChanged += entryCurrentFrameB_NumberChanged;
            this.panelFrame = new StackLayout()
            {
                IsVisible = true,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Spacing = 0,
                Padding = new Thickness(0,0,0,0),
                //BackgroundColor = Color.Olive,
                Children =
                {
                    this.labelMatchScore,
                    this.labelMatchScoreDigits,
                    //this.labelTapToEditFrameScore,
                    new StackLayout()
                    {
                        //BackgroundColor = Color.Red,
                        Orientation = StackOrientation.Horizontal,
                        Spacing = 0,
                        Padding = new Thickness(0, 0, 0, 0),
                        HorizontalOptions = LayoutOptions.Center,
                        Children =
                        {
                            this.entryCurrentFrameA,
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
                            this.entryCurrentFrameB,
                        }
                    },
                    this.labelPointsLeft,
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
            this.panelSecond = new Grid
            {
				BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.Fill,
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
                },
            };
            panelSecond.Children.Add(this.imageYou, 0, 0);
            panelSecond.Children.Add(this.panelFrame, 1, 0);
            panelSecond.Children.Add(this.imageOpponent, 2, 0);

            /// a panel for entering frame score
            /// 
            this.labelEnteringFrameScore = new BybLabel
            {
                Text = "",
                TextColor = Config.ColorTextOnBackground,
				HeightRequest = 40,
                HorizontalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            panelEnteringFrameScore = new StackLayout()
            {
                IsVisible = false,
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0,20,0,0),
                Spacing = 5,
				BackgroundColor = Config.ColorBlackBackground,
                Children =
                {
                    this.labelEnteringFrameScore,
                    new BybLabel
                    {
                        Text = "Tap here when done.",
                        TextColor = Config.ColorTextOnBackground,
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        //VerticalTextAlignment = TextAlignment.Center
                    }
                }
            };

            /// the current snooker break
            /// 
			this.snookerBreakControl = new SnookerBreakControl(new MetadataHelper().FromScoreForYou(this.MatchScore), labelPointsLeft, CurrentFrameScore.ballsOnTable, entryCurrentFrameA, entryCurrentFrameB);
			this.snookerBreakControl.Padding = new Thickness (0,0,0,0);
			this.snookerBreakControl.VerticalOptions = LayoutOptions.FillAndExpand;
            this.snookerBreakControl.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.snookerBreakControl.DoneLeft += snookerBreakControl_DoneLeft;
            this.snookerBreakControl.DoneRight += snookerBreakControl_DoneRight;
            this.snookerBreakControl.BallsChanged += snookerBreakControl_BallsChanged;
 
			/// list of past breaks
			/// 
			this.listOfBreaksInMatchControl = new ListOfBreaksInMatchControl();
			this.listOfBreaksInMatchControl.Padding = new Thickness (Config.IsTablet ? 15 : 0, 0, Config.IsTablet ? 15 : 0, 0);
			this.listOfBreaksInMatchControl.UserTappedOnBreak += this.listOfBreaksInMatchControl_UserTappedOnBreak;
			this.labelPastBreaks = new SimpleButtonWithLittleDownArrow (false)
            {
				Text = "Show previous",
                IsBold = false,
                //IsSmallerFont = true,
				HeightRequest = heightOfLabelPastBreaks,
				HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = Config.App != MobileAppEnum.SnookerForVenues,
            };
            this.labelPastBreaks2 = new BybLabel()
            {
                IsVisible = false,
                Text = "",
                TextColor = Config.ColorTextOnBackgroundGrayed,
                HeightRequest = heightOfLabelPastBreaks,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            this.panelPastBreaks = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Spacing = 0,
                BackgroundColor = Config.ColorBackground,//Color.Black,
                Children =
                {
                    new BoxView()
                    {
                        HeightRequest = 1,
                        BackgroundColor = Config.ColorBackground,
                    },
					this.labelPastBreaks,
//                    new Frame
//                    {
//                        IsVisible = Config.App != MobileAppEnum.SnookerForVenues,
//                        HeightRequest = heightOfLabelPastBreaks,
//                        Padding = new Thickness(0),
//                        HorizontalOptions = LayoutOptions.Center,
//                        VerticalOptions = LayoutOptions.Start,
//                        HasShadow = false,
//                        BackgroundColor = Color.Transparent,
//                        Content = this.labelPastBreaks,
//                    },
                    this.labelPastBreaks2,
					this.listOfBreaksInMatchControl,
				}
			};
            this.labelPastBreaks.Clicked += (s1, e1) => { this.panelPastBreaksTapped(); };
			this.panelPastBreaks.GestureRecognizers.Add (new TapGestureRecognizer () {
				Command = new Command(() => { this.panelPastBreaksTapped(); })
			});

            /// content
            /// 
            this.absoluteLayout = new AbsoluteLayout()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
            };
            this.absoluteLayout.Children.Add(this.panelTop);
            this.absoluteLayout.Children.Add(this.panelSecond);
            this.absoluteLayout.Children.Add(this.snookerBreakControl);
            this.absoluteLayout.Children.Add(this.panelPastBreaks);
            this.absoluteLayout.Children.Add(this.panelEnteringFrameScore);
            Content = this.absoluteLayout;

            this.Padding = new Thickness(0, 0, 0, 0);
			this.BackgroundColor = Color.Black;

            this.snookerBreakControl.VoiceButtonControl.PageTopLevelLayout = absoluteLayout;
            this.snookerBreakControl.HelpButtonControl.PageTopLevelLayout = absoluteLayout;
		}

        void updateFrameScoreInSnookerBreakControl()
        {
            this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
            this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
        }

        Rectangle rectBoundsPastBreaks_Default;
        Rectangle rectBoundsPastBreaks_Expanded;
        bool isPastBreaksExpanded = Config.App == MobileAppEnum.SnookerForVenues ? true : false;

        protected override void OnSizeAllocated(double width, double height)
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
            {
                double widthLeft = (width > height) ? (width * 0.63) : (width * 0.70);
                this.rectBoundsPastBreaks_Default = this.rectBoundsPastBreaks_Expanded =
                    new Rectangle(widthLeft, panelTopHeight, width - widthLeft, height);

                AbsoluteLayout.SetLayoutBounds(this.panelTop, new Rectangle(0, 0, width, panelTopHeight));
                AbsoluteLayout.SetLayoutBounds(this.panelPastBreaks, rectBoundsPastBreaks_Default);
                AbsoluteLayout.SetLayoutBounds(this.panelSecond, new Rectangle(0, panelTopHeight, widthLeft, panelSecondHeight));
                AbsoluteLayout.SetLayoutBounds(this.snookerBreakControl, new Rectangle(0, panelTopHeight + panelSecondHeight, widthLeft, height - panelTopHeight - panelSecondHeight));
                AbsoluteLayout.SetLayoutBounds(this.panelEnteringFrameScore, new Rectangle(0, panelTopHeight + panelSecondHeight, widthLeft, 100));
            }
            else
            {
                this.rectBoundsPastBreaks_Default = new Rectangle(0, height - defaultHeightOfPottedBallsPanel, width, 1000);
                this.rectBoundsPastBreaks_Expanded = new Rectangle(0, panelTopHeight + panelSecondHeight, width, 1000);

                AbsoluteLayout.SetLayoutBounds(this.panelTop, new Rectangle(0, 0, width, panelTopHeight));
                AbsoluteLayout.SetLayoutBounds(this.panelPastBreaks, isPastBreaksExpanded ? rectBoundsPastBreaks_Expanded : rectBoundsPastBreaks_Default);
                AbsoluteLayout.SetLayoutBounds(this.panelSecond, new Rectangle(0, panelTopHeight, width, panelSecondHeight));
                AbsoluteLayout.SetLayoutBounds(this.snookerBreakControl, new Rectangle(0, panelTopHeight + panelSecondHeight, width, height - panelTopHeight - panelSecondHeight - defaultHeightOfPottedBallsPanel));
                AbsoluteLayout.SetLayoutBounds(this.panelEnteringFrameScore, new Rectangle(0, panelTopHeight + panelSecondHeight, width, 100));
            }

            base.OnSizeAllocated(width, height);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

		void panelPastBreaksTapped()
		{
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;

			this.labelPastBreaks2.Text = (this.listOfBreaksInMatchControl.Breaks != null && this.listOfBreaksInMatchControl.Breaks.Count() > 0) ? "Tip: tap on breaks below to edit or delete them" : "empty";
			
            if (this.isPastBreaksExpanded)
			{
				this.panelPastBreaks.LayoutTo(this.rectBoundsPastBreaks_Default, 250, Easing.Linear);
				this.labelPastBreaks.Text = "Show previous";
                this.labelPastBreaks2.IsVisible = false;
			}
			else
			{
				this.panelPastBreaks.LayoutTo(this.rectBoundsPastBreaks_Expanded, 250, Easing.SinOut);
				this.labelPastBreaks.Text = "Hide previous";
                this.labelPastBreaks2.IsVisible = true;
			}

            this.isPastBreaksExpanded = !this.isPastBreaksExpanded;
		}

        private void snookerBreakControl_DoneLeft(object sender, EventArgs e)
        {
            snookerBreakControl_Done(false);
        }

        private void snookerBreakControl_DoneRight(object sender, EventArgs e)
        {
            snookerBreakControl_Done(true);
        }

        private void snookerBreakControl_BallsChanged(object sender, EventArgs e)
        {
			var balls = snookerBreakControl.EnteredBalls;
            //this.labelTapToEditFrameScore.Opacity = balls.Count == 0 ? 1.0 : 0.1;
            //this.labelTapToEditFrameScore.Text = "(edit any time)";

            this.changeOpacityOnControls(balls.Count > 0);
        }

        void changeOpacityOnControls(bool transparent)
        {
            //this.imageBack.Opacity = transparent ? 0.05 : 1.0;
            this.buttonBack.Opacity = transparent ? 0.05 : 1.0;
            this.labelDone.Opacity = transparent ? 0.05 : 1.0;
        }

        void snookerBreakControl_Done(bool isOpponent)
        {
            var balls = snookerBreakControl.EnteredBalls;
            if (balls.Count == 0)
            {
                snookerBreakControl.ClearBalls();
                return;
            }

            //this.labelTapToEditFrameScore.Opacity = 1.0;
            //this.labelTapToEditFrameScore.Text = "(edit any time)";
            this.changeOpacityOnControls(false);



            SnookerBreak snookerBreak = new SnookerBreak();
            snookerBreak.AthleteID = !isOpponent ? MatchScore.YourAthleteID : MatchScore.OpponentAthleteID;
            snookerBreak.OpponentAthleteID = !isOpponent ? MatchScore.OpponentAthleteID : MatchScore.YourAthleteID;
            snookerBreak.Date = DateTime.Now;
			snookerBreak.IsFoul = snookerBreakControl.isFoul;
            snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
            snookerBreak.Balls = balls;
            snookerBreak.CalcFromBalls();
            if (this.MatchScore.YourBreaks == null)
                this.MatchScore.YourBreaks = new List<SnookerBreak>();
            if (this.MatchScore.OpponentBreaks == null)
                this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
            if (!isOpponent)
                this.MatchScore.YourBreaks.Add(snookerBreak);
            else
                this.MatchScore.OpponentBreaks.Add(snookerBreak);

			snookerBreakControl.ClearBalls();

            if (!isOpponent)
                this.CurrentFrameScore.A += snookerBreak.Points;
            else
                this.CurrentFrameScore.B += snookerBreak.Points;

            // Fill list of breaks AFTER the currentFrameScore has been updated
			this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);

            updateFrameScoreInSnookerBreakControl();

			if (App.UserPreferences.IsVoiceOn) 
			{
				// Announce break points and name
				string name = isOpponent ? MatchScore.OpponentName : MatchScore.YourName;

				string textToPronounce = snookerBreak.Points.ToString();
				if (string.IsNullOrEmpty(name) == false)
					textToPronounce += ". " + name;

				// Also say the new frame score
                if (Config.IsIOS == false)
				    textToPronounce += ". " + CurrentFrameScore.A.ToString() + ". " + CurrentFrameScore.B.ToString();

				App.ScorePronouncer.Pronounce (textToPronounce, App.UserPreferences.Voice, App.UserPreferences.VoiceRate, App.UserPreferences.VoicePitch);
			}

			//if (this.isMatchEditMode == false)
				new TempSavedMatchHelper (App.KeyChain).Save (MatchScore);
        }

        private async void listOfBreaksInMatchControl_UserTappedOnBreak(object sender, SnookerBreak snookerBreak)
        {
			if (Config.IsTablet == false && this.isPastBreaksExpanded == false)
            {
				this.panelPastBreaksTapped();
				return;
			}
			
            string strPoints = snookerBreak.Points.ToString();
            string strOtherPlayer = ((snookerBreak.AthleteID == MatchScore.YourAthleteID) ? (MatchScore.OpponentName ?? "Opponent") : MatchScore.YourName);

            string strEdit = "Edit";
            string strDelete = "Delete";
            string strReassign = "Assign to " + strOtherPlayer;

            string strResult1 = await this.DisplayActionSheet(snookerBreak.Points.ToString() + " point break", "Cancel", null, strDelete, strReassign, strEdit);

            if (string.IsNullOrEmpty(strResult1) || strResult1 == "Cancel")
                return;

            if (strResult1 == strEdit)
            {
                RecordBreakPage page = new RecordBreakPage(snookerBreak, false, false);
                await this.Navigation.PushModalAsync(page);
                page.Done += (s1, updatedBreak) =>
                {
                    if (updatedBreak == null || updatedBreak.Points == 0)
                        return;

                    // update balls on table
                    this.snookerBreakControl.breakEdited(snookerBreak, updatedBreak);

                    int diff = updatedBreak.Points - snookerBreak.Points;
                    if (snookerBreak.OpponentAthleteID != this.MatchScore.YourAthleteID)
                        this.CurrentFrameScore.A = System.Math.Max(0, this.CurrentFrameScore.A + diff);
                    else
                        this.CurrentFrameScore.B = System.Math.Max(0, this.CurrentFrameScore.B + diff);
                    snookerBreak.Points = updatedBreak.Points;
                    snookerBreak.NumberOfBalls = updatedBreak.NumberOfBalls;
                    snookerBreak.Balls = updatedBreak.Balls.ToList();
                    snookerBreak.IsFoul = updatedBreak.IsFoul;

                    this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);

                    updateFrameScoreInSnookerBreakControl();

                    this.snookerBreakControl.updateFrameScoreOnBreakEdit((int)this.entryCurrentFrameA.Number, (int)this.entryCurrentFrameB.Number);
                };

                return;
            }
            else if (strResult1 == strDelete)
            {
                string strSubtractScore = "Remove " + strPoints + " from the score?";
                string strKeepScore = "Keep the score";
                string strResult2 = await this.DisplayActionSheet("Frame score", "Cancel", null, strSubtractScore, strKeepScore);

                if (string.IsNullOrEmpty(strResult2) || strResult2 == "Cancel")
                    return;

                if (strResult2 == strSubtractScore)
                {
                    // update ballsOnTable
                    this.snookerBreakControl.breakDeleted(snookerBreak);

                    if (snookerBreak.OpponentAthleteID != this.MatchScore.YourAthleteID)
                        this.CurrentFrameScore.A = System.Math.Max(0, this.CurrentFrameScore.A - snookerBreak.Points);
                    else if (snookerBreak.OpponentAthleteID == this.MatchScore.YourAthleteID)
                        this.CurrentFrameScore.B = System.Math.Max(0, this.CurrentFrameScore.B - snookerBreak.Points);
                    this.MatchScore.YourBreaks.Remove(snookerBreak);
                    this.MatchScore.OpponentBreaks.Remove(snookerBreak);
                }
                else if (strResult2 == strKeepScore)
                {
                    this.MatchScore.YourBreaks.Remove(snookerBreak);
                    this.MatchScore.OpponentBreaks.Remove(snookerBreak);
                }

                this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);

                updateFrameScoreInSnookerBreakControl();

                return;
            }
            else if (strResult1 == strReassign)
            {
                string strMoveScore;
                if (snookerBreak.OpponentAthleteID != this.MatchScore.YourAthleteID)
                    strMoveScore = MatchScore.YourName + " -" + snookerBreak.Points.ToString() + ", " + MatchScore.OpponentName + " +" + snookerBreak.Points.ToString();
                else
                    strMoveScore = MatchScore.YourName + " +" + snookerBreak.Points.ToString() + ", " + MatchScore.OpponentName + " -" + snookerBreak.Points.ToString();

                string strKeepScore = "Do NOT change the frame scores";
                string strResult2 = await this.DisplayActionSheet("Re-assign the break and...", "Cancel", null, strMoveScore, strKeepScore);
                if (string.IsNullOrEmpty(strResult2) || strResult2 == "Cancel")
                    return;
                if (strResult2 == strMoveScore)
                {
                    if (snookerBreak.OpponentAthleteID != this.MatchScore.YourAthleteID)
                    {
                        this.CurrentFrameScore.A = System.Math.Max(0, this.CurrentFrameScore.A - snookerBreak.Points);
                        this.CurrentFrameScore.B += snookerBreak.Points;
                    }
                    else if (snookerBreak.OpponentAthleteID == this.MatchScore.YourAthleteID)
                    {
                        this.CurrentFrameScore.B = System.Math.Max(0, this.CurrentFrameScore.B - snookerBreak.Points);
                        this.CurrentFrameScore.A += snookerBreak.Points;
                    }
                }
                if (strResult2 == strMoveScore || strResult2 == strKeepScore)
				{
					this.MatchScore.YourBreaks.Remove (snookerBreak);
					this.MatchScore.OpponentBreaks.Remove (snookerBreak);

					int a = snookerBreak.AthleteID;
					snookerBreak.AthleteID = snookerBreak.OpponentAthleteID;
					snookerBreak.OpponentAthleteID = a;
					string n = snookerBreak.AthleteName;
					snookerBreak.AthleteName = snookerBreak.OpponentName;
					snookerBreak.OpponentName = snookerBreak.AthleteName;
					snookerBreak.OpponentConfirmation = OpponentConfirmationEnum.NotYet;
					if (snookerBreak.AthleteID == this.MatchScore.YourAthleteID)
						this.MatchScore.YourBreaks.Add (snookerBreak);
					else
						this.MatchScore.OpponentBreaks.Add (snookerBreak);
				}

                this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);

                updateFrameScoreInSnookerBreakControl();

                return;
            }
        }

        private void entryCurrentFrameB_NumberChanged(object sender, EventArgs e)
        {
            this.CurrentFrameScore.B = this.entryCurrentFrameB.Number ?? 0;
            this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
        }

        private void entryCurrentFrameA_NumberChanged(object sender, EventArgs e)
        {
            this.CurrentFrameScore.A = this.entryCurrentFrameA.Number ?? 0;
            this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
        }

        private void entryCurrentFrameB_FocusedOnNumber(object sender, EventArgs e)
        {
			if (this.entryCurrentFrameB.Number == 0)
				this.entryCurrentFrameB.Number = null;
			
            this.switchToEnterFrameScore(true);
        }

        private void entryCurrentFrameA_FocusedOnNumber(object sender, EventArgs e)
        {
			if (this.entryCurrentFrameA.Number == 0)
				this.entryCurrentFrameA.Number = null;
			
            this.switchToEnterFrameScore(true);
        }

        private void entryCurrentFrameB_UnfocusedFromNumber(object sender, EventArgs e)
        {
            this.switchToEnterFrameScore(false);
        }

        private void entryCurrentFrameA_UnfocusedFromNumber(object sender, EventArgs e)
        {
            this.switchToEnterFrameScore(false);
        }

        private void imageYou_Clicked()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;

            App.Navigator.DisplayAlertRegular("Tap on the opponent picture to select the opponent.");
        }

        private async void imageOpponent_Clicked()
        {
            if (Config.App == MobileAppEnum.SnookerForVenues)
                return;

            if (this.MatchScore.OpponentAthleteID > 0)
            {
                string strCancel = "Cancel";
                string strPickOther = "Change the opponent";

                if (await this.DisplayActionSheet("Byb", strCancel, strPickOther) != strPickOther)
                    return;
            }

			if (App.Navigator.GetOpenedPage (typeof(PickAthletePage)) != null)
				return;

            PickAthletePage dlg = new PickAthletePage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
            dlg.UserMadeSelection += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();
                if (e1.Person != null)
                {
                    this.doOnOpponentSelected(e1.Person);
                }
            };
        }

        void doOnOpponentSelected(PersonBasicWebModel person)
        {
            this.MatchScore.OpponentAthleteID = person.ID;
            this.MatchScore.OpponentName = person.Name;
            this.MatchScore.OpponentPicture = person.Picture;
			if (this.MatchScore.OpponentBreaks != null)
			{
				foreach (var b in this.MatchScore.OpponentBreaks)
					b.AthleteID = person.ID;
			}

            this.updateImages();
			this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);
        }

        void updateImages()
        {
            this.imageYou.SetImage(MatchScore.YourName, MatchScore.YourPicture);

            if (MatchScore.OpponentAthleteID == 0)
                this.imageOpponent.SetImagePickOpponent();
            else
                this.imageOpponent.SetImage(MatchScore.OpponentName, MatchScore.OpponentPicture);
        }

        void switchToEnterFrameScore(bool yes)
        {
            if (yes)
            {
				if (this.snookerBreakControl.EnteredBalls.Count () > 0)
					return;
				
                int frameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
                this.labelEnteringFrameScore.FormattedText = new FormattedString()
                {
                    Spans =
                    {
                        new Span() { Text = "Entering " },
                        new Span() { Text = " frame #" + frameNumber.ToString() + " total score.", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.LargerFontSize, ForegroundColor = Color.Red },
                    }
                };

                this.panelEnteringFrameScore.IsVisible = true;
				this.panelEnteringFrameScore.ForceLayout ();
				this.panelPastBreaks.IsVisible = false;
				this.snookerBreakControl.IsVisible = false;
                //this.labelTapToEditFrameScore.Opacity = 0.1;
            }
            else
            {
                this.panelEnteringFrameScore.IsVisible = false;
				this.panelPastBreaks.IsVisible = true;
				this.snookerBreakControl.IsVisible = true;
                //this.labelTapToEditFrameScore.Opacity = 1.0;
            }
        }
    }
}
