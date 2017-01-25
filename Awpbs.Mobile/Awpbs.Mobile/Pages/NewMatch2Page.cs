//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xamarin.Forms;
//using XLabs.Ioc;
//using XLabs.Platform.Device;
//
//namespace Awpbs.Mobile
//{
//	public class NewMatch2Page : ContentPage
//	{
//        public bool IsEditMode { get; private set; }
//
//        public SnookerMatchScore MatchScore
//        {
//            get;
//            private set;
//        }
//
//        public SnookerFrameScore CurrentFrameScore
//        {
//            get;
//            private set;
//        }
//
//        public bool IsReadonlyMode
//        {
//            get;
//            private set;
//        }
//
//		double panelTopHeight = Config.IsTablet ? Config.TitleHeight : 50;
//        double panelFrameWidth = Config.IsTablet ? 200 : 150;
//		double panelSecondHeight = Config.IsTablet ? 120 : 70;
//		double panelSecondPadding = Config.IsTablet ? 15 : 5;
//
//        // top panel
//        Image imageBack;
//        Label labelTop;
//        VoiceButtonControl voiceButtonControl;
//
//        // second panel
//        Image imageYou;
//        Image imageOpponent;
//        StackLayout panelOfferEnterMatchScore;
//
//        // panel with the frame score, inside secondPanel
//        StackLayout panelFrame;
//        Label buttonPrevFrame;
//        Label buttonCurFrame;
//        Label buttonNextFrame;
//        LargeNumberEntry entryCurrentFrameA;
//        LargeNumberEntry entryCurrentFrameB;
//
//        // panel for entering match score
//        LargeNumberEntry entryMatchScoreA;
//        LargeNumberEntry entryMatchScoreB;
//        StackLayout panelEnteringMatchScore;
//
//        // the scrollview
//        ScrollView theScrollView;
//        SnookerBreakControl snookerBreakControl;
//        ListOfBreaksInMatchControl listOfBreaksInMatchControl;
//        StackLayout panelEnteringFrameScore;
//        Label labelEnteringFrameScore;
//
//        // buttons and slideout with buttons
//        Button buttonOKMatchScore;
//        Button buttonPause;
//        Button buttonFinish;
//        Button buttonCancelMatch;
//        StackLayout panelSlideoutButtons;
//        BoxView boxViewSlideoutCover;
//        Button buttonStartNewFrame;
//        Button buttonFinishMatch;
//        Button buttonCancelSlideout;
//
//        // content
//        AbsoluteLayout absoluteLayout;
//        Grid grid;
//
//        public NewMatch2Page(SnookerMatchScore matchScore, bool isReadonlyMode)
//        {
//            this.IsEditMode = true;
//            this.IsReadonlyMode = isReadonlyMode;
//
//            this.MatchScore = matchScore.Clone();
//            if (this.MatchScore.FrameScores == null)
//                this.MatchScore.FrameScores = new List<SnookerFrameScore>();
//
//            init();
//            fill();
//        }
//
//        public NewMatch2Page(SnookerMatchMetadata metadata)
//        {
//            this.IsEditMode = false;
//            
//            this.MatchScore = new SnookerMatchScore();
//            this.MatchScore.FrameScores = new List<SnookerFrameScore>();
//            new MetadataHelper().ToScore(metadata, this.MatchScore);
//
//            init();
//            fill();
//        }
//
//        void init()
//		{
////            if (Config.IsTablet)
////                this.Padding = new Thickness(50, 20, 50, 50);
////            else
////                this.Padding = new Thickness(0);
//
//            /// top panel
//            /// 
//            this.imageBack = new Image()
//            {
//                Source = new FileImageSource() { File = "back.png" },
//                HeightRequest = 25,
//                WidthRequest = 25,
//            };
//            this.imageBack.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() =>
//                {
//                    this.buttonPause_Clicked(this, EventArgs.Empty);
//                }),
//                NumberOfTapsRequired = 1
//            });
//            this.labelTop = new Label()
//            {
//                TextColor = Color.White,
//                FontFamily = Config.FontFamily,
//                FontAttributes = FontAttributes.Bold,
//                FontSize = Config.LargerFontSize,
//                HorizontalOptions = LayoutOptions.Center,
//                VerticalOptions = LayoutOptions.Center,
//            };
//            this.voiceButtonControl = new VoiceButtonControl();
//            var panelTop = new Grid()
//            {
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Fill,
//                Padding = new Thickness(0),
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                RowDefinitions = 
//                {
//                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
//                },
//                ColumnDefinitions =
//                {
//                    new ColumnDefinition() { Width = new GridLength(25, GridUnitType.Absolute) },
//                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition() { Width = new GridLength(30, GridUnitType.Absolute) },
//                }
//            };
//            panelTop.Children.Add(this.imageBack, 0, 0);
//            panelTop.Children.Add(this.labelTop, 1, 0);
//            panelTop.Children.Add(this.voiceButtonControl, 2, 0);
//
//            /// panel with the frame score, inside secondPanel
//            /// 
//			this.buttonPrevFrame = new Label () {
//				Opacity = 0.1,
//				Text = "<<",
//				TextColor = Config.ColorTextOnBackground,
//				WidthRequest = panelFrameWidth * 0.25,
//				VerticalOptions = LayoutOptions.FillAndExpand,
//				VerticalTextAlignment = TextAlignment.Center,
//				HorizontalTextAlignment = TextAlignment.Center,
//			};
//			this.buttonCurFrame = new Label () {
//				Opacity = 1.0,
//				Text = "<<",
//				TextColor = Config.ColorTextOnBackground,
//				WidthRequest = panelFrameWidth * 0.50,
//				VerticalOptions = LayoutOptions.FillAndExpand,
//				VerticalTextAlignment = TextAlignment.Center,
//				HorizontalTextAlignment = TextAlignment.Center,
//			};
//			this.buttonNextFrame = new Label () {
//				Opacity = 0.1,
//				Text = ">>",
//				TextColor = Config.ColorTextOnBackground,
//				WidthRequest = panelFrameWidth * 0.25,
//				VerticalOptions = LayoutOptions.FillAndExpand,
//				VerticalTextAlignment = TextAlignment.Center,
//				HorizontalTextAlignment = TextAlignment.Center,
//			};
//			this.buttonPrevFrame.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.buttonPrevFrame_Clicked(this, EventArgs.Empty); })});
//			this.buttonNextFrame.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.buttonNextFrame_Clicked(this, EventArgs.Empty); })});
//			this.buttonCurFrame.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.buttonCurFrame_Clicked(this, EventArgs.Empty); })});
//			this.entryCurrentFrameA = new LargeNumberEntry("", Config.ColorDarkGrayBackground)
//            {
//				IsLabelVisible = false,
//				BackgroundColor = Config.ColorDarkGrayBackground,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
//                WidthRequest = Config.IsTablet ? 70 : 45,//(panelFrameWidth - 20) * 0.5,
//            };
//			this.entryCurrentFrameB = new LargeNumberEntry("", Config.ColorDarkGrayBackground)
//            {
//				IsLabelVisible = false,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0, Config.IsTablet ? 15 : 5, 0, 0),
//                WidthRequest = Config.IsTablet ? 70 : 45,//(panelFrameWidth - 20) * 0.5,
//            };
//            this.entryCurrentFrameA.FocusedOnNumber += entryCurrentFrameA_FocusedOnNumber;
//            this.entryCurrentFrameB.FocusedOnNumber += entryCurrentFrameB_FocusedOnNumber;
//            this.entryCurrentFrameA.UnfocusedFromNumber += entryCurrentFrameA_UnfocusedFromNumber;
//            this.entryCurrentFrameB.UnfocusedFromNumber += entryCurrentFrameB_UnfocusedFromNumber;
//            this.entryCurrentFrameA.NumberChanged += entryCurrentFrameA_NumberChanged;
//            this.entryCurrentFrameB.NumberChanged += entryCurrentFrameB_NumberChanged;
//            this.panelFrame = new StackLayout()
//            {
//                IsVisible = false,
//                Orientation = StackOrientation.Vertical,
//                Spacing = 0,
//                Padding = new Thickness(0),
//				//BackgroundColor = Color.Navy,
//                Children =
//                {
//                    new StackLayout()
//                    {
//                        Orientation = StackOrientation.Horizontal,
//						HeightRequest = (panelSecondHeight - panelSecondPadding*2) * 0.45,// 25,
//						HorizontalOptions = LayoutOptions.Center,
//                        Spacing = 0,
//                        Padding = new Thickness(0),
//						//BackgroundColor = Color.Fuchsia,
//                        Children =
//                        {
//                            this.buttonPrevFrame,
//                            this.buttonCurFrame,
//                            this.buttonNextFrame,
//                        }
//                    },
//                    new StackLayout()
//                    {
//                        Orientation = StackOrientation.Horizontal,
//						HeightRequest = (panelSecondHeight - panelSecondPadding*2) * 0.55,//35,
//                        Spacing = 0,
//                        Padding = new Thickness(0),
//                        HorizontalOptions = LayoutOptions.Center,
//						//BackgroundColor = Color.Aqua,
//                        Children =
//                        {
//                            this.entryCurrentFrameA,
//                            new Label()
//                            {
//                                Text = ":",
//                                FontSize = Config.VeryLargeFontSize,
//                                TextColor = Config.ColorTextOnBackgroundGrayed,
//                                WidthRequest = 15,
//                                VerticalOptions = LayoutOptions.Center,
//                                HorizontalTextAlignment = TextAlignment.Center,
//                            },
//                            this.entryCurrentFrameB,
//                        }
//                    }
//                }
//            };
//
//            /// second panel
//            /// 
//            this.imageYou = new Image()
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Fill,
//                Source = App.ImagesService.GetImageSource(null, BackgroundEnum.Background1, true),
//                BackgroundColor = Color.Transparent,
//            };
//            this.imageOpponent = new Image()
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Fill,
//                Source = App.ImagesService.GetImageSource(null, BackgroundEnum.Background1, true),
//                BackgroundColor = Color.Transparent
//            };
//            this.imageYou.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.imageYou_Clicked(); }),
//                NumberOfTapsRequired = 1
//            });
//            this.imageOpponent.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.imageOpponent_Clicked(); }),
//                NumberOfTapsRequired = 1
//            });
//            this.panelOfferEnterMatchScore = new StackLayout()
//            {
//                IsVisible = true,
//                Orientation = StackOrientation.Vertical,
//                HorizontalOptions = LayoutOptions.Center,
//                VerticalOptions = LayoutOptions.Center,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                Children =
//                {
//                    new Label()
//                    {
//                        Text = "Tap here to simply 'enter' match score.",
//                        TextColor = Config.ColorTextOnBackground,
//                        HorizontalOptions = LayoutOptions.Center,
//                        HorizontalTextAlignment = TextAlignment.Center,
//                        VerticalTextAlignment = TextAlignment.Center,
//                    }
//                }
//            };
//            this.panelOfferEnterMatchScore.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.switchToEnterMatchScore(true); }) });
//            var panelSecond = new Grid
//            {
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.Fill,
//				Padding = new Thickness(0, panelSecondPadding, 0, panelSecondPadding),
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                ColumnDefinitions =
//				{
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(panelFrameWidth, GridUnitType.Absolute) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                },
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
//                },
//            };
//            panelSecond.Children.Add(this.imageYou, 0, 0);
//            panelSecond.Children.Add(this.panelFrame, 1, 0);
//            panelSecond.Children.Add(this.panelOfferEnterMatchScore, 1, 0);
//            panelSecond.Children.Add(this.imageOpponent, 2, 0);
//
//            /// a panel for entering match score
//            /// 
//			entryMatchScoreA = new LargeNumberEntry(this.MatchScore.YourName, Config.ColorBackground)
//            {
//                //BackgroundColor = Config.ColorBackground,
//                Placeholder = "-",
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.Center,
//                WidthRequest = Config.IsTablet ? 200 : 120,
//                HeightRequest = Config.IsTablet ? 83 : 50,
//                Padding = new Thickness(10,10,10,10),
//            };
//			entryMatchScoreB = new LargeNumberEntry(this.MatchScore.OpponentName ?? "Unknown", Config.ColorBackground)
//            {
//                //BackgroundColor = Config.ColorBackground,
//                Placeholder = "-",
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.Center,
//                WidthRequest = Config.IsTablet ? 200 : 120,
//                HeightRequest = Config.IsTablet ? 83 : 50,
//                Padding = new Thickness(10, 10, 10, 10),
//            };
//            var labelInfo = new Label()
//            {
//                Opacity = 0.0,
//                Text = "Tap here when done",
//                TextColor = Config.ColorTextOnBackground,
//                HorizontalOptions = LayoutOptions.Center,
//                VerticalTextAlignment = TextAlignment.Center,
//                HeightRequest = 30,
//            };
//            entryMatchScoreA.FocusedOnNumber += (s1, e1) => { labelInfo.Opacity = 1.0; };
//            entryMatchScoreB.FocusedOnNumber += (s1, e1) => { labelInfo.Opacity = 1.0; };
//            entryMatchScoreA.UnfocusedFromNumber += (s1, e1) => { labelInfo.Opacity = 0.0; };
//            entryMatchScoreB.UnfocusedFromNumber += (s1, e1) => { labelInfo.Opacity = 0.0; };
//            panelEnteringMatchScore = new StackLayout()
//            {
//                IsVisible = false,
//                Orientation = StackOrientation.Vertical,
//                Padding = new Thickness(20,10,20,10),
//                Spacing = 5,
//				HorizontalOptions = LayoutOptions.Fill,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                Children =
//                {
//                    new Label
//                    {
//                        FormattedText = new FormattedString()
//                        {
//                            Spans = 
//                            {
//                                new Span() { Text = "Entering "},
//                                new Span() { Text = " match score", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.LargerFontSize, ForegroundColor = Color.Red },
//                            }
//                        },
//                        TextColor = Config.ColorTextOnBackground,
//                        HorizontalOptions = LayoutOptions.Center,
//                    },
//                    labelInfo,
//                    new StackLayout()
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        HorizontalOptions = LayoutOptions.Center,
//                        Children =
//                        {
//                            this.entryMatchScoreA,
//                            this.entryMatchScoreB,
//                        }
//                    }
//                }
//            };
//
//            /// a panel for entering frame score
//            /// 
//            this.labelEnteringFrameScore = new Label
//            {
//                Text = "",
//                TextColor = Config.ColorTextOnBackground,
//                HorizontalOptions = LayoutOptions.Center,
//                HorizontalTextAlignment = TextAlignment.Center,
//                VerticalTextAlignment = TextAlignment.Center
//            };
//            panelEnteringFrameScore = new StackLayout()
//            {
//                IsVisible = false,
//                Orientation = StackOrientation.Vertical,
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                HeightRequest = 52,
//                Padding = new Thickness(0,20,0,0),
//                Spacing = 5,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                Children =
//                {
//                    this.labelEnteringFrameScore,
//                    new Label
//                    {
//                        Text = "Tap here when done.",
//                        TextColor = Config.ColorTextOnBackground,
//                        HorizontalOptions = LayoutOptions.Center,
//                        HorizontalTextAlignment = TextAlignment.Center,
//                        VerticalTextAlignment = TextAlignment.Center
//                    }
//                }
//            };
//
//            /// the scrollview with balls
//            /// 
//            this.snookerBreakControl = new SnookerBreakControl(new MetadataHelper().FromScoreForYou(this.MatchScore), false);
//            this.snookerBreakControl.VerticalOptions = LayoutOptions.Fill;
//            this.snookerBreakControl.HorizontalOptions = LayoutOptions.Fill;
//            this.snookerBreakControl.DoneLeft += snookerBreakControl_DoneLeft;
//            this.snookerBreakControl.DoneRight += snookerBreakControl_DoneRight;
//            this.snookerBreakControl.BallsChanged += snookerBreakControl_BallsChanged;
////            this.snookerBreakControl.TipClicked += (s1, e1) =>
////            {
////                this.switchToEnterMatchScore(false);
////                this.snookerBreakControl.TipText = "Tip: tap on a ball or tap on the score above.";
////            };
//            this.listOfBreaksInMatchControl = new ListOfBreaksInMatchControl();
//            this.listOfBreaksInMatchControl.UserTappedOnBreak += listOfBreaksInMatchControl_UserTappedOnBreak;
//            this.theScrollView = new ScrollView()
//            {
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0),
//                Content = new StackLayout()
//                {
//                    Orientation = StackOrientation.Vertical,
//                    Padding = new Thickness(0),
//                    Spacing = 0,
//                    Children =
//                    {
//                        this.snookerBreakControl,
//                        this.listOfBreaksInMatchControl,
//                    }
//                }
//            };
//
//            /// buttons
//            /// 
//            this.buttonOKMatchScore = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK", IsVisible = false };
//            this.buttonPause = new Button() { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Pause" };
//            this.buttonFinish = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Next or finish" };
//            this.buttonCancelMatch = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel match" };
//            this.buttonOKMatchScore.Clicked += buttonOKMatchScore_Clicked;
//            this.buttonPause.Clicked += buttonPause_Clicked;
//            this.buttonFinish.Clicked += buttonFinish_Clicked;
//            this.buttonCancelMatch.Clicked += buttonCancelMatch_Clicked;
//            var panelOkCancel = new StackLayout()
//            {
//                Orientation = StackOrientation.Horizontal,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.Fill,
//                HeightRequest = Config.OkCancelButtonsHeight,
//                Padding = new Thickness(Config.OkCancelButtonsPadding),
//                Spacing = 1,
//                Children =
//                {
//                    buttonPause,
//                    buttonFinish,
//                    buttonCancelMatch,
//                    buttonOKMatchScore,
//                }
//            };
//
//            /// Top-level Grid
//            /// 
//            this.grid = new Grid
//            {
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                Padding = new Thickness(0),
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Start,//.FillAndExpand,
//                BackgroundColor = Config.ColorBackground,
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(panelTopHeight, GridUnitType.Absolute) },                                                 // top panel
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },                                                              // line
//					new RowDefinition { Height = new GridLength(panelSecondHeight, GridUnitType.Absolute) },                                              // second panel
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },                                                              // line
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },                                                                  // balls
//                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding*2, GridUnitType.Absolute) }, // buttons
//                },
//                ColumnDefinitions =
//                {
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                }
//            };
//            grid.Children.Add(panelTop, 0, 0);
//            grid.Children.Add(panelSecond, 0, 2);
//            grid.Children.Add(theScrollView, 0, 4);
//            grid.Children.Add(panelEnteringMatchScore, 0, 4);
//            grid.Children.Add(panelEnteringFrameScore, 0, 4);
//            grid.Children.Add(panelOkCancel, 0, 5);
//
//            /// slide-out panel with buttons
//            /// 
//            this.buttonStartNewFrame = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Start next frame" };
//            this.buttonFinishMatch = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Finish match" };
//            this.buttonCancelSlideout = new Button() { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
//            buttonStartNewFrame.Clicked += buttonStartNewFrame_Clicked;
//            buttonFinishMatch.Clicked += buttonFinishMatch_Clicked;
//            buttonCancelSlideout.Clicked += (s1, e1) => { this.removeSlideout(); };
//            this.panelSlideoutButtons = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                Padding = new Thickness(Config.OkCancelButtonsPadding),
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                IsVisible = false,
//                BackgroundColor = Config.ColorBackground,
//                Children =
//                {
//                    buttonStartNewFrame,
//                    buttonFinishMatch,
//                    buttonCancelSlideout
//                }
//            };
//
//            /// content
//            /// 
//            this.absoluteLayout = new AbsoluteLayout()
//            {
//                VerticalOptions = LayoutOptions.FillAndExpand,
//            };
//            this.absoluteLayout.Children.Add(grid, new Point(0, 0));
//            this.absoluteLayout.Children.Add(panelSlideoutButtons, new Rectangle(0, this.Height - 100, this.Width, 100));
//            Content = this.absoluteLayout;
//            this.Padding = new Thickness(0, 0, 0, 0);
//			this.BackgroundColor = Color.Black;
//
//            this.voiceButtonControl.PageTopLevelLayout = absoluteLayout;
//		}
//
//        void fill()
//        {
//            this.fillTopPanel();
//
//            this.goToNextFrame();
//            this.updateBottomButtons();
//
//            if (this.IsEditMode)
//            {
//                this.entryMatchScoreA.Number = MatchScore.MatchScoreA;
//                this.entryMatchScoreB.Number = MatchScore.MatchScoreB;
//
//                this.switchToEnterMatchScore(this.MatchScore.HasFrameScores == false);
//            }
//
//            if (this.IsReadonlyMode)
//            {
//                this.entryMatchScoreA.IsEnabled = false;
//                this.entryMatchScoreB.IsEnabled = false;
//                this.entryCurrentFrameA.IsEnabled = false;
//                this.entryCurrentFrameB.IsEnabled = false;
//                this.buttonCurFrame.IsEnabled = false;
//                this.snookerBreakControl.IsVisible = false;
//				this.voiceButtonControl.IsVisible = false;
//            }
//
////            if (this.IsEditMode || this.IsReadonlyMode)
////                this.snookerBreakControl.TipText = "Tip: tap on a ball or tap on the score above.";
////            else
////                this.snookerBreakControl.TipText = "Start";
//        }
//
//        protected override void OnSizeAllocated(double width, double height)
//        {
//            base.OnSizeAllocated(width, height);
//
//            // use all of the available space for the grid
//            this.grid.HeightRequest = this.Height;
//            this.grid.WidthRequest = this.Width;
//
//            // make sure that the scrollview takes up all available space within the egrid
//            double heightForScrollview = this.Height;
//            foreach (var row in this.grid.RowDefinitions)
//                if (row.Height.IsAbsolute)
//                    heightForScrollview -= row.Height.Value;
//            this.theScrollView.HeightRequest = heightForScrollview;
//
//            // set the size of the button-balls control
//            double percent;
//            if (Config.IsTablet)
//                percent = 0.75;
//            else
//                percent = 0.86;
//            //bool isSmallScreen = heightForScrollview < 310;
//            this.snookerBreakControl.HeightRequest = heightForScrollview * percent;
//        }
//
//        protected override void OnDisappearing()
//        {
//            base.OnDisappearing();
//        }
//
//        protected override void OnAppearing()
//        {
//            base.OnAppearing();
//        }
//
//        private void snookerBreakControl_DoneLeft(object sender, EventArgs e)
//        {
//            snookerBreakControl_Done(false);
//            this.highlightActivePlayer();
//        }
//
//        private void snookerBreakControl_DoneRight(object sender, EventArgs e)
//        {
//            snookerBreakControl_Done(true);
//            this.highlightActivePlayer();
//        }
//
//        private void snookerBreakControl_BallsChanged(object sender, EventArgs e)
//        {
//            //this.snookerBreakControl.TipText = "Tip: tap on a ball or tap on the score above.";
//
//            this.switchToEnterMatchScore(false);
//            this.highlightActivePlayer();
//        }
//
//        void snookerBreakControl_Done(bool isOpponent)
//        {
//            var balls = snookerBreakControl.EnteredBalls;
//            if (balls.Count == 0)
//            {
//                snookerBreakControl.ClearBalls();
//                return;
//            }
//
//            snookerBreakControl.ClearBalls();
//
//            SnookerBreak snookerBreak = new SnookerBreak();
//            snookerBreak.AthleteID = !isOpponent ? MatchScore.YourAthleteID : MatchScore.OpponentAthleteID;
//            snookerBreak.OpponentAthleteID = !isOpponent ? MatchScore.OpponentAthleteID : MatchScore.YourAthleteID;
//            snookerBreak.Date = DateTime.Now;
//            snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//            snookerBreak.Balls = balls;
//            snookerBreak.CalcFromBalls();
//            if (this.MatchScore.YourBreaks == null)
//                this.MatchScore.YourBreaks = new List<SnookerBreak>();
//            if (this.MatchScore.OpponentBreaks == null)
//                this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//            if (!isOpponent)
//                this.MatchScore.YourBreaks.Add(snookerBreak);
//            else
//                this.MatchScore.OpponentBreaks.Add(snookerBreak);
//
//            this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);
//
//            if (!isOpponent)
//                this.CurrentFrameScore.A += snookerBreak.Points;
//            else
//                this.CurrentFrameScore.B += snookerBreak.Points;
//
//            this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//            this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//            this.updateBottomButtons();
//            this.updatePrevNextButtons();
//        }
//
//        private async void listOfBreaksInMatchControl_UserTappedOnBreak(object sender, SnookerBreak snookerBreak)
//        {
//            if (this.IsReadonlyMode)
//            {
//                if (this.MatchScore.YourBreaks != null && this.MatchScore.YourBreaks.Contains(snookerBreak) == false)
//                    return;
//
//                if (await this.DisplayAlert("Notable break?", "Would you like to make this a notable break?", "Yes", "Cancel") == true)
//                {
//                    int myAthleteID = App.Repository.GetMyAthleteID();
//                    var allResults = App.Repository.GetResults(myAthleteID, false);
//
//                    var existingResult = (from i in allResults
//                                          where i.Date != null
//                                          where i.Count != null
//                                          where System.Math.Abs((i.Date.Value - snookerBreak.Date).TotalMinutes) < 1
//                                          where i.Count.Value == snookerBreak.Points
//                                          select i).ToList();
//                    if (existingResult.Count > 0)
//                    {
//                        await this.DisplayAlert("Byb", "This break is already a notable break.", "OK");
//                        return;
//                    }
//
//                    var result = new Result();
//                    snookerBreak.PostToResult(result);
//                    result.VenueID = MatchScore.VenueID;
//                    result.OpponentAthleteID = MatchScore.OpponentAthleteID;
//                    result.AthleteID = myAthleteID;
//                    result.TimeModified = DateTimeHelper.GetUtcNow();
//                    App.Repository.AddResult(result);
//
//                    await App.Navigator.NavPage.Navigation.PopModalAsync();
//                    App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Breaks);
//                }
//            }
//            else
//            {
//                string strSubtractFromTheScore = "Delete " + snookerBreak.Points.ToString() + ", subtract " + snookerBreak.Points.ToString() + " from the score";
//                string strKeepTheScore = "Delete " + snookerBreak.Points.ToString() + ", keep the score";
//                //string strKeepTheScore = "Delete the break, keep the score";
//                //string strSubtractFromTheScore = "Delete the break, subtract " + snookerBreak.Points.ToString() + " from the score";
//                string strResult = await this.DisplayActionSheet ("Delete the break?", "Cancel", null, strSubtractFromTheScore, strKeepTheScore);
//				
//				if (strResult == strKeepTheScore || strResult == strSubtractFromTheScore)
//                {
//                    this.MatchScore.YourBreaks.Remove(snookerBreak);
//                    this.MatchScore.OpponentBreaks.Remove(snookerBreak);
//                    this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);
//                }
//				if (strResult == strSubtractFromTheScore) {
//					if (snookerBreak.OpponentAthleteID != this.MatchScore.YourAthleteID)
//						this.CurrentFrameScore.A = System.Math.Max(0, this.CurrentFrameScore.A - snookerBreak.Points);
//					else if (snookerBreak.OpponentAthleteID == this.MatchScore.YourAthleteID)
//						this.CurrentFrameScore.B = System.Math.Max(0, this.CurrentFrameScore.B - snookerBreak.Points);
//
//					this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//					this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//					this.updateBottomButtons();
//					this.updatePrevNextButtons();
//				}
//            }
//        }
//
//        private async void buttonCurFrame_Clicked(object sender, EventArgs e)
//        {
//            if (this.MatchScore.IsEmpty)
//                return;
//            if (this.IsReadonlyMode)
//                return;
//
//            string title = this.buttonCurFrame.Text;
//            string cancel = "Cancel";
//            string delete = "Delete this frame";
//
//            string result = await this.DisplayActionSheet(title, cancel, delete);
//
//            if (result == delete)
//            {
//                this.MatchScore.FrameScores.Remove(this.CurrentFrameScore);
//                this.MatchScore.CalculateMatchScoreFromFrameScores();
//                if (this.MatchScore.HasFrameScores == false)
//                {
//                    this.MatchScore.MatchScoreA = 0;
//                    this.MatchScore.MatchScoreB = 0;
//                }
//                this.CurrentFrameScore = null;
//
//                this.goToNextFrame();
//            }
//        }
//
//        private void buttonNextFrame_Clicked(object sender, EventArgs e)
//        {
//            this.goToNextFrame();
//        }
//
//        private void buttonPrevFrame_Clicked(object sender, EventArgs e)
//        {
//            this.goToPrevFrame();
//        }
//
//        private void entryCurrentFrameB_NumberChanged(object sender, EventArgs e)
//        {
//            this.CurrentFrameScore.B = this.entryCurrentFrameB.Number ?? 0;
//            this.updateBottomButtons();
//        }
//
//        private void entryCurrentFrameA_NumberChanged(object sender, EventArgs e)
//        {
//            this.CurrentFrameScore.A = this.entryCurrentFrameA.Number ?? 0;
//            this.updateBottomButtons();
//        }
//
//        private void entryCurrentFrameB_FocusedOnNumber(object sender, EventArgs e)
//        {
//            this.switchToEnterFrameScore(true);
//        }
//
//        private void entryCurrentFrameA_FocusedOnNumber(object sender, EventArgs e)
//        {
//            this.switchToEnterFrameScore(true);
//        }
//
//        private void entryCurrentFrameB_UnfocusedFromNumber(object sender, EventArgs e)
//        {
//            this.switchToEnterFrameScore(false);
//        }
//
//        private void entryCurrentFrameA_UnfocusedFromNumber(object sender, EventArgs e)
//        {
//            this.switchToEnterFrameScore(false);
//        }
//
//        private async void buttonCancelMatch_Clicked(object sender, EventArgs e)
//        {
//            if (this.IsReadonlyMode)
//            {
//                // just close
//                await App.Navigator.NavPage.Navigation.PopModalAsync();
//                return;
//            }
//
//            this.finishOrPauseMatch(false);
//        }
//
//        void removeSlideout()
//        {
//            this.grid.FadeTo(1.0, 250);
//            this.panelSlideoutButtons.IsVisible = false;
//            this.boxViewSlideoutCover.IsVisible = false;
//
//            this.absoluteLayout.Children.Remove(this.boxViewSlideoutCover);
//            this.boxViewSlideoutCover = null;
//        }
//
//        void openButtonsSlideout()
//        {
//            double slideoutHeight = Config.OkCancelButtonsHeight * 3 + Config.OkCancelButtonsPadding * 2;
//
//            // cover the page with a boxview to avoid taps on other elements
//            this.boxViewSlideoutCover = new BoxView() { Color = Color.FromRgba(0, 0, 0, 50) };
//            this.boxViewSlideoutCover.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.removeSlideout(); }),
//                NumberOfTapsRequired = 1
//            });
//            this.absoluteLayout.Children.Add(boxViewSlideoutCover, new Rectangle(0, 0, this.Width, Height - slideoutHeight));
//
//            // slide-out
//            AbsoluteLayout.SetLayoutBounds(this.panelSlideoutButtons, new Rectangle(0, this.Height, this.Width, slideoutHeight));
//            this.panelSlideoutButtons.LayoutTo(new Rectangle(0, this.Height - slideoutHeight, this.Width, slideoutHeight), 250);
//            this.grid.FadeTo(0.5, 250);
//            this.panelSlideoutButtons.IsVisible = true;
//
//			//this.panelSlideoutButtons.BackgroundColor = Color.Yellow;
//			this.buttonStartNewFrame.HeightRequest = 40; // if this is not done, the buttons do not become visisble
//        }
//
//        private void buttonStartNewFrame_Clicked(object sender, EventArgs e)
//        {
//            this.removeSlideout();
//            this.startNewFrame();
//        }
//
//        private void buttonFinishMatch_Clicked(object sender, EventArgs e)
//        {
//            this.removeSlideout();
//            this.finishOrPauseMatch(false);
//        }
//
//        private async void buttonFinish_Clicked(object sender, EventArgs e)
//        {
//            if (boxViewSlideoutCover != null)
//                return;
//
//            int enteredBalls = this.snookerBreakControl.EnteredBalls.Sum();
//            if (enteredBalls > 0)
//            {
//                string strIgnore = "Ignore the un-filed " + enteredBalls + " points";
//                if (await this.DisplayActionSheet("Incomplete break", "Cancel", null, strIgnore) != strIgnore)
//                    return;
//            }
//
//            // update buttons in the slideout
//            this.buttonStartNewFrame.IsEnabled = this.CurrentFrameScore.IsEmpty == false;
//            if (this.CurrentFrameScore.IsValid || (this.CurrentFrameScore.A == 0 && this.CurrentFrameScore.B == 0))
//            {
//                int a = this.MatchScore.MatchScoreA;
//                int b = this.MatchScore.MatchScoreB;
//                if (a + b < this.MatchScore.FrameScores.Count)
//                {
//                    if (this.CurrentFrameScore.A > 0 || this.CurrentFrameScore.B > 0)
//                    {
//                        if (this.CurrentFrameScore.A > this.CurrentFrameScore.B)
//                            a++;
//                        else
//                            b++;
//                    }
//                }
//                this.buttonFinishMatch.Text = "Finish match (" + a.ToString() + " : " + b.ToString() + ")";
//                this.buttonFinishMatch.IsEnabled = true;
//                this.buttonStartNewFrame.IsEnabled = true;
//            }
//            else
//            {
//                this.buttonFinishMatch.Text = "Finish match";
//                this.buttonFinishMatch.IsEnabled = false;
//                this.buttonStartNewFrame.IsEnabled = false;
//            }
//
//            this.openButtonsSlideout();
//        }
//
//        private void buttonOKMatchScore_Clicked(object sender, EventArgs e)
//        {
//            int a = this.entryMatchScoreA.Number ?? 0;
//            int b = this.entryMatchScoreB.Number ?? 0;
//
//            if (a < 0 || b < 0 || a > 21 || b > 21 || (a == 0 && b == 0))
//            {
//                DisplayAlert("Byb", "Please enter a proper match score.", "OK");
//                return;
//            }
//
//            this.MatchScore.FrameScores.Clear();
//            this.MatchScore.MatchScoreA = a;
//            this.MatchScore.MatchScoreB = b;
//
//            this.finishOrPauseMatch(false);
//        }
//
//        private void buttonPause_Clicked(object sender, EventArgs e)
//        {
//            finishOrPauseMatch(true);
//        }
//
//        //private void buttonNewBreakA_Clicked(object sender, EventArgs e)
//        //{
//        //    var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);
//        //    var page = new NewSnookerBreakPage(metadata, false, false);
//        //    page.Done += (s1, snookerBreak) =>
//        //    {
//        //        if (snookerBreak == null)
//        //            return;
//        //        snookerBreak.Date = DateTime.Now;
//        //        snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//
//        //        if (this.MatchScore.YourBreaks == null)
//        //            this.MatchScore.YourBreaks = new List<SnookerBreak>();
//        //        if (this.MatchScore.OpponentBreaks == null)
//        //            this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//        //        this.MatchScore.YourBreaks.Add(snookerBreak);
//        //        this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);
//
//        //        this.CurrentFrameScore.A += snookerBreak.Points;
//        //        this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//        //        this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//        //        this.updateBottomButtons();
//        //        this.updatePrevNextButtons();
//                
//        //    };
//        //    App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        //}
//
//        //private void buttonNewBreakB_Clicked(object sender, EventArgs e)
//        //{
//        //    var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);//.FromScoreForOpponent(this.MatchScore);
//        //    var page = new NewSnookerBreakPage(metadata, true, false);
//        //    page.Done += (s1, snookerBreak) =>
//        //    {
//        //        if (snookerBreak == null)
//        //            return;
//        //        snookerBreak.Date = DateTime.Now;
//        //        snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//        //        snookerBreak.OpponentAthleteID = this.MatchScore.YourAthleteID;
//
//        //        if (this.MatchScore.YourBreaks == null)
//        //            this.MatchScore.YourBreaks = new List<SnookerBreak>();
//        //        if (this.MatchScore.OpponentBreaks == null)
//        //            this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//        //        this.MatchScore.OpponentBreaks.Add(snookerBreak);
//        //        this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);
//
//        //        this.CurrentFrameScore.B += snookerBreak.Points;
//        //        this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//        //        this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//        //        this.updateBottomButtons();
//        //        this.updatePrevNextButtons();
//        //    };
//        //    App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        //}
//
//        private void imageYou_Clicked()
//        {
//            if (this.IsReadonlyMode)
//                return;
//            App.Navigator.DisplayAlertRegular("Tap on the opponent picture to select the opponent.");
//        }
//
//        private async void imageOpponent_Clicked()
//        {
//            if (this.IsReadonlyMode)
//                return;
//            if (this.MatchScore.OpponentAthleteID > 0)
//            {
//                string strCancel = "Cancel";
//                string strPickOther = "Change the opponent";
//
//                if (await this.DisplayActionSheet("Byb", strCancel, strPickOther) != strPickOther)
//                    return;
//            }
//
//            PickAthletePage dlg = new PickAthletePage();
//            await App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
//            dlg.UserMadeSelection += (s1, e1) =>
//            {
//                App.Navigator.NavPage.Navigation.PopModalAsync();
//                if (e1.Person != null)
//                {
//                    this.doOnOpponentSelected(e1.Person);
//                }
//            };
//        }
//
//        void doOnOpponentSelected(PersonBasicWebModel person)
//        {
//            this.MatchScore.OpponentAthleteID = person.ID;
//            this.MatchScore.OpponentName = person.Name;
//            this.MatchScore.OpponentPicture = person.Picture;
//            this.entryMatchScoreB.Title = person.Name;
//
//            this.fillTopPanel();
//        }
//
//        void fillTopPanel()
//        {
//            // label
//            string name1 = MatchScore.YourName;
//            string name2 = MatchScore.OpponentName ?? "Unknown";
//            string text = name1 + " vs. " + name2;
//            if (this.panelEnteringMatchScore.IsVisible != true && this.panelOfferEnterMatchScore.IsVisible != true)
//                text += ", " + MatchScore.MatchScoreA.ToString() + " : " + MatchScore.MatchScoreB.ToString();
//            this.labelTop.Text = text;
//
//            // images
//            this.imageYou.Source = App.ImagesService.GetImageSource(MatchScore.YourPicture, BackgroundEnum.Black);
//            if (MatchScore.OpponentAthleteID == 0)
//                this.imageOpponent.Source = new FileImageSource() { File = "plusBlack2.png" };
//            else
//                this.imageOpponent.Source = App.ImagesService.GetImageSource(MatchScore.OpponentPicture, BackgroundEnum.Black);
//        }
//
//        void startNewFrame()
//        {
//            string validationMessage;
//            if (this.CurrentFrameScore.Validate(out validationMessage) == false)
//            {
//                this.DisplayAlert("Byb", validationMessage, "Cancel");
//                return;
//            }
//
//            this.MatchScore.CalculateMatchScoreFromFrameScores();
//            this.MatchScore.FrameScores.Add(new SnookerFrameScore());
//
//            this.snookerBreakControl.ClearBalls();
//
//            this.goToNextFrame();
//            this.updateBottomButtons();
//        }
//
//        async void finishOrPauseMatch(bool justPause)
//        {
//            if (this.IsReadonlyMode || this.MatchScore.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
//            {
//                // just close
//                await App.Navigator.NavPage.Navigation.PopModalAsync();
//                return;
//            }
//
//            if (justPause == false)
//            {
//                // validate all frames
//                foreach (var frame in MatchScore.FrameScores)
//                {
//                    if (frame.IsEmpty == true)
//                        continue;
//
//                    string validationMessage;
//                    if (frame.Validate(out validationMessage) == false)
//                    {
//                        await this.DisplayAlert("Byb", "Cannot finish the match. Frame #" + (MatchScore.FrameScores.IndexOf(frame) + 1).ToString() + " is invalid. Message: " + validationMessage, "Cancel");
//                        return;
//                    }
//                }
//
//                // remove empty frames
//                var emptyFrames = this.MatchScore.FrameScores.Where(i => i.IsEmpty).ToList();
//                foreach (var frame in emptyFrames)
//                    this.MatchScore.FrameScores.Remove(frame);
//
//                if (this.MatchScore.FrameScores.Count > 0)
//                    this.MatchScore.CalculateMatchScoreFromFrameScores();
//            }
//
//            bool isEmpty = this.MatchScore.MatchScoreA == 0 && this.MatchScore.MatchScoreB == 0 && this.MatchScore.HasFrameScores == false;
//
//            if (isEmpty == false && justPause == false)
//            {
//                if (MatchScore.OpponentAthleteID <= 0)
//                {
//                    string strSelectOpponent = "Select the opponent";
//                    string strNoOpponent = "Save without opponent";
//                    string strAnswer = await this.DisplayActionSheet("No oppponent selected", "Cancel", null, strSelectOpponent, strNoOpponent);
//
//                    if (strAnswer == strSelectOpponent)
//                    {
//                        PickAthletePage dlg = new PickAthletePage();
//                        await App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
//                        dlg.UserMadeSelection += (s1, e1) =>
//                        {
//                            App.Navigator.NavPage.Navigation.PopModalAsync();
//                            if (e1.Person != null)
//                            {
//                                this.doOnOpponentSelected(e1.Person);
//                                finishOrPauseMatch(justPause);
//                            }
//                        };
//
//                        return;
//                    }
//                    else if (strAnswer == strNoOpponent)
//                    {
//                        // continue to saving
//                    }
//                    else
//                    {
//                        return;
//                    }
//
//                    //string text = "Cannot 'finish' the match until you select the opponent.";
//                    //if (this.panelEnteringMatchScore.IsVisible == false)
//                    //    text += " You can 'pause' instead.";
//                    //await this.DisplayAlert("Byb", text, "Cancel");
//                    //return;
//                }
//
//                SnookerBreak bestBreak = null;
//                if (this.MatchScore.YourBreaks != null)
//                    bestBreak = this.MatchScore.YourBreaks.OrderByDescending(i => i.Points).FirstOrDefault();
//                if (bestBreak != null)
//                {
//                    // what should be the threshold for a notable break?
//                    int threshold = 20;
//                    var highestEverBreak = App.Repository.GetMyBestResult();
//                    if (highestEverBreak != null && highestEverBreak.Count != null)
//                        threshold = (int)(highestEverBreak.Count.Value * 0.6);
//                    if (threshold > 100)
//                        threshold = 100;
//                    if (threshold < 20)
//                        threshold = 20;
//
//                    if (bestBreak.Points > threshold)
//                    {
//                        string strYes = "Yes, it's a notable break";
//                        string strCancel = "Cancel";
//                        string answer = await this.DisplayActionSheet("Record the break of " + bestBreak.Points.ToString() + " as a notable break?", strCancel, "No, not notable", strYes);
//                        if (answer == null || answer == strCancel)
//                            return;
//                        if (answer == strYes)
//                        {
//                            var result = new Result();
//                            bestBreak.PostToResult(result);
//                            result.VenueID = MatchScore.VenueID;
//                            result.OpponentAthleteID = MatchScore.OpponentAthleteID;
//                            result.AthleteID = MatchScore.YourAthleteID;
//                            result.TimeModified = DateTimeHelper.GetUtcNow();
//                            App.Repository.AddResult(result);
//                        }
//                    }
//                }
//            }
//
//            if (this.IsEditMode == false)
//            {
//                // save
//                if (isEmpty == false)
//                {
//                    Score score = new Score();
//                    MatchScore.PostToScore(score);
//                    score.TimeModified = DateTimeHelper.GetUtcNow();
//                    score.Guid = Guid.NewGuid();
//                    score.IsUnfinished = justPause;
//                    App.Repository.AddScore(score);
//                }
//            }
//            else
//            {
//                // update
//                Score score = App.Repository.GetScore(MatchScore.ID);
//                if (isEmpty)
//                {
//                    if (await this.DisplayAlert("Byb", "You emptied the match. Delete it?", "Yes, delete it", "No") != true)
//                        return;
//                    score.TimeModified = DateTimeHelper.GetUtcNow();
//                    score.IsDeleted = true;
//                }
//                MatchScore.PostToScore(score);
//                score.IsUnfinished = justPause;
//                App.Repository.UpdateScore(score);
//            }
//
//            await App.Navigator.NavPage.Navigation.PopModalAsync();
//            if (justPause || isEmpty)
//                App.Navigator.GoToRecord();
//            else
//            {
//                App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Matches);
//                App.Navigator.StartSyncAndCheckForNotifications();
//            }
//        }
//
//        void goToNextFrame()
//        {
//            if (this.CurrentFrameScore == null)
//            {
//                if (this.MatchScore.FrameScores.Count == 0)
//                    this.MatchScore.FrameScores.Add(new SnookerFrameScore());
//                this.CurrentFrameScore = this.MatchScore.FrameScores.Last();
//            }
//            else
//            {
//                int index = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//                if (index >= this.MatchScore.FrameScores.Count)
//                    return;
//                this.CurrentFrameScore = this.MatchScore.FrameScores[index];
//            }
//
//            if (this.CurrentFrameScore.A == 0 && this.CurrentFrameScore.B == 0)
//            {
//                this.entryCurrentFrameA.Number = null;
//                this.entryCurrentFrameB.Number = null;
//            }
//            else
//            {
//                this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//                this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//            }
//
//            this.buttonCurFrame.Text = "Frame #" + (this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1).ToString();
//            this.updateBottomButtons();
//            this.updatePrevNextButtons();
//            this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);
//
//            bool isLast = this.CurrentFrameScore == this.MatchScore.FrameScores.Last();
//            this.snookerBreakControl.IsVisible = isLast && this.IsReadonlyMode == false;
//        }
//
//        void goToPrevFrame()
//        {
//            if (this.CurrentFrameScore == null)
//                return;
//
//            int index = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore);
//            if (index <= 0)
//                return;
//
//            this.CurrentFrameScore = this.MatchScore.FrameScores[index - 1];
//            this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//            this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//
//            this.buttonCurFrame.Text = "Frame #" + (this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1).ToString();
//            this.updateBottomButtons();
//            this.updatePrevNextButtons();
//            this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);
//
//            bool isLast = this.CurrentFrameScore == this.MatchScore.FrameScores.Last();
//            this.snookerBreakControl.IsVisible = isLast && this.IsReadonlyMode == false;
//        }
//
//        void updatePrevNextButtons()
//        {
//            if (this.MatchScore.FrameScores.Count == 0 || this.CurrentFrameScore == null)
//            {
//                this.buttonPrevFrame.Opacity = 0.1;
//                this.buttonNextFrame.Opacity = 0.1;
//                return;
//            }
//            int index = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore);
//            this.buttonPrevFrame.Opacity = index > 0 ? 1.0 : 0.1;
//            this.buttonNextFrame.Opacity = (index < this.MatchScore.FrameScores.Count - 1) ? 1.0 : 0.1;
//        }
//
//        void updateBottomButtons()
//        {
//            this.buttonOKMatchScore.IsVisible = false;
//
//            if (this.MatchScore.IsEmpty || this.IsReadonlyMode)
//            {
//                this.buttonCancelMatch.IsVisible = true;
//                this.buttonCancelMatch.Text = "Cancel";// this.IsReadonlyMode ? "Cancel" : "Cancel match";
//                this.buttonPause.IsVisible = false;
//                this.buttonFinish.IsVisible = false;
//                return;
//            }
//
//            bool isOnLastFrame = true;
//            if (this.MatchScore.FrameScores.Count > 1)
//                isOnLastFrame = this.MatchScore.FrameScores.Last() == this.CurrentFrameScore;
//
//            this.buttonCancelMatch.IsVisible = false;
//            this.buttonPause.IsVisible = true;
//            this.buttonFinish.IsVisible = true;
//
//            this.buttonFinish.IsEnabled = isOnLastFrame;
//        }
//
//        void switchToEnterMatchScore(bool yes)
//        {
//            if (yes)
//            {
//                this.theScrollView.IsVisible = false;
//                this.panelEnteringMatchScore.IsVisible = true;
//                this.panelOfferEnterMatchScore.IsVisible = false;
//                this.panelFrame.IsVisible = false;
//
//                this.buttonOKMatchScore.IsVisible = true;
//                this.buttonCancelMatch.IsVisible = true;
//                this.buttonCancelMatch.Text = "Cancel";
//                this.buttonPause.IsVisible = false;
//                this.buttonFinish.IsVisible = false;
//
//                this.fillTopPanel();
//            }
//            else
//            {
//                this.theScrollView.IsVisible = true;
//                this.panelEnteringMatchScore.IsVisible = false;
//                this.panelOfferEnterMatchScore.IsVisible = false;
//                this.panelFrame.IsVisible = true;
//
//                this.updateBottomButtons();
//            }
//        }
//
//        void switchToEnterFrameScore(bool yes)
//        {
//            if (yes)
//            {
//                int frameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//                this.labelEnteringFrameScore.FormattedText = new FormattedString()
//                {
//                    Spans =
//                    {
//                        new Span() { Text = "Entering " },
//                        new Span() { Text = " frame #" + frameNumber.ToString() + " total score.", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.LargerFontSize, ForegroundColor = Color.Red },
//                    }
//                };
//
//                this.panelEnteringFrameScore.IsVisible = true;
//                this.theScrollView.IsVisible = false;
//            }
//            else
//            {
//                this.panelEnteringFrameScore.IsVisible = false;
//                this.theScrollView.IsVisible = true;
//            }
//        }
//
//        void highlightActivePlayer()
//        {
//            switch (this.snookerBreakControl.Status)
//            {
//                case SnookerBreakStatusEnum.Left:
//                    this.imageYou.Opacity = 1.0;
//                    this.imageOpponent.Opacity = 0.3;
//                    this.entryCurrentFrameA.Opacity = 1.0;
//                    this.entryCurrentFrameB.Opacity = 0.3;
//                    break;
//                case SnookerBreakStatusEnum.Right:
//                    this.imageYou.Opacity = 0.3;
//                    this.imageOpponent.Opacity = 1.0;
//                    this.entryCurrentFrameA.Opacity = 0.3;
//                    this.entryCurrentFrameB.Opacity = 1.0;
//                    break;
//                default:
//                    this.imageYou.Opacity = 1.0;
//                    this.imageOpponent.Opacity = 1.0;
//                    this.entryCurrentFrameA.Opacity = 1.0;
//                    this.entryCurrentFrameB.Opacity = 1.0;
//                    break;
//            }
//        }
//    }
//}
