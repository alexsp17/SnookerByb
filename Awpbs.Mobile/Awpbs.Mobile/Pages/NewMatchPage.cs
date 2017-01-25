//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Xamarin.Forms;
//using XLabs.Ioc;
//using XLabs.Platform.Device;

//namespace Awpbs.Mobile
//{
//	public class NewMatchPage : ContentPage
//	{
//        public bool IsEditMode { get; private set; }
//        bool ignoreUIevents;

//        public SnookerMatchScore MatchScore
//        {
//            get;
//            private set;
//        }

//        public SnookerFrameScore CurrentFrameScore
//        {
//            get;
//            private set;
//        }

//        public bool IsReadonlyMode
//        {
//            get;
//            private set;
//        }

//        Label labelTitle;
//        Image imageBack;
//        //Image imageOpenMetadata;

//        Image imageYou;
//        Image imageOpponent;
//        LargeNumberEntry entryMatchScoreA;
//        LargeNumberEntry entryMatchScoreB;
//        StackLayout panelMatchScoreInner;

//        StackLayout panelEnteringMatchScore;

//        Grid panelPrevNext;
//        Button buttonPrevFrame;
//        Button buttonCurFrame;
//        Button buttonNextFrame;

//        StackLayout panelEnteringFrameScore;

//        ScrollView scrollViewCurrentFrame;
//        Grid panelCurrentFrame;
//        LargeNumberEntry entryCurrentFrameA;
//        LargeNumberEntry entryCurrentFrameB;
//        Button buttonNewBreakA;
//        Button buttonNewBreakB;
//        //Button buttonAddPointsA;
//        //Button buttonAddPointsB;
//        ListOfBreaksInMatchControl listOfBreaksInMatchControl;

//        Button buttonPause;
//        Button buttonFinish;
//        Button buttonCancelMatch;

//        // slideout for metadata
//        //StackLayout panelSlideoutMetadata;
//        //SnookerMatchMetadataControl metadataControl;
//        //Button buttonHideMetadata;

//        // slideout with buttons
//        StackLayout panelSlideoutButtons;
//        Button buttonStartNewFrame;
//        Button buttonFinishMatch;
//        Button buttonCancelSlideout;

//        BoxView boxViewSlideoutCover;
//        AbsoluteLayout absoluteLayout;
//        Grid grid;

//        public NewMatchPage(SnookerMatchScore matchScore, bool isReadonlyMode)
//        {
//            this.IsEditMode = true;
//            this.IsReadonlyMode = isReadonlyMode;

//            this.MatchScore = matchScore.Clone();
//            if (this.MatchScore.FrameScores == null)
//                this.MatchScore.FrameScores = new List<SnookerFrameScore>();

//            init();
//            fill();
//        }

//        public NewMatchPage(SnookerMatchMetadata metadata)
//        {
//            this.IsEditMode = false;
            
//            this.MatchScore = new SnookerMatchScore();
//            this.MatchScore.FrameScores = new List<SnookerFrameScore>();
//            new MetadataHelper().ToScore(metadata, this.MatchScore);

//            init();
//            fill();
//        }

//        void init()
//		{
//            this.BackgroundColor = Color.Black;
//            if (Config.IsTablet)
//                this.Padding = new Thickness(50, 20, 50, 50);
//            else
//                this.Padding = new Thickness(0);

//            /// title panel
//            /// 
//            this.labelTitle = new Label()
//            {
//                TextColor = Color.White,
//                FontFamily = Config.FontFamily,
//                FontAttributes = FontAttributes.Bold,
//                FontSize = Config.LargerFontSize,
//                HorizontalTextAlignment = TextAlignment.Center,
//                VerticalTextAlignment = TextAlignment.Center,
//                HeightRequest = Config.TitleHeight,
//                HorizontalOptions = LayoutOptions.FillAndExpand
//            };
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
//            //this.imageOpenMetadata = new Image()
//            //{
//            //    Source = new FileImageSource() { File = "down.png" },
//            //    HeightRequest = 20,
//            //    WidthRequest = 20,
//            //};
//            //this.imageOpenMetadata.GestureRecognizers.Add(new TapGestureRecognizer
//            //{
//            //    Command = new Command(() => { this.openMetadataSlideout(); }),
//            //    NumberOfTapsRequired = 1
//            //});
//            var panelTitle = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(10,0,10,0),
//                Spacing = 0,
//                Children =
//                {
//                    new StackLayout
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        Children =
//                        {
//                            imageBack,
//                            labelTitle,
//                            //imageOpenMetadata
//                        }
//                    },
//                    new BoxView()
//                    {
//                        HorizontalOptions = LayoutOptions.FillAndExpand,
//                        Color = Color.White,
//                        HeightRequest = 1,
//                        Opacity = 0.2,
//                    }
//                }
//            };

//            /// panel with match score
//            /// 
//            this.imageYou = new Image()
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Fill,
//                Source = App.ImagesService.GetImageSource(null, BackgroundEnum.Background1, true),
//                BackgroundColor = Color.Transparent
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

//            //double widthRequest_tablet = Config.IsIOS ? 20 : 55;
//            //int widthRequest_phone = Config.IsIOS ? 15 : 32;
//            double widthRequest_tablet = Config.IsIOS ? 30 : 55;
//            int widthRequest_phone = Config.IsIOS ? 30 : 32;

//			entryMatchScoreA = new LargeNumberEntry("", Color.Black)
//            {
//                Placeholder = "-",
//                WidthRequest = Config.IsTablet ? widthRequest_tablet : widthRequest_phone,
//                VerticalOptions = LayoutOptions.Center
//            };
//			entryMatchScoreB = new LargeNumberEntry("", Color.Black)
//            {
//                Placeholder = "-",
//                WidthRequest = Config.IsTablet ? widthRequest_tablet : widthRequest_phone,
//            };
//            entryMatchScoreA.NumberChanged += entryMatchScore_NumberChanged;
//            entryMatchScoreB.NumberChanged += entryMatchScore_NumberChanged;
//            //entryMatchScoreA.TextChanged += entryMatchScore_TextChanged;
//            //entryMatchScoreB.TextChanged += entryMatchScore_TextChanged;
//            //entryMatchScoreA = new LargeBybPicker()
//            //{
//            //    Title = "-",
//            //    WidthRequest = Config.IsTablet ? widthRequestVal_tablet : widthRequest_phone,
//            //};
//            //entryMatchScoreB = new LargeBybPicker()
//            //{
//            //    Title = "-",
//            //    WidthRequest = Config.IsTablet ? widthRequestVal_tablet : widthRequest_phone,
//            //};
//            //for (int i = 0; i <= 9; ++i)
//            //{
//            //    entryMatchScoreA.Items.Add(i.ToString());
//            //    entryMatchScoreB.Items.Add(i.ToString());
//            //}
//            entryMatchScoreA.FocusedOnNumber += entryMatchScore_Focused;
//            entryMatchScoreA.UnfocusedFromNumber += entryMatchScore_Unfocused;
//            entryMatchScoreB.FocusedOnNumber += entryMatchScore_Focused;
//            entryMatchScoreB.UnfocusedFromNumber += entryMatchScore_Unfocused;
//            //entryMatchScoreA.SelectedIndexChanged += entryMatchScore_SelectedIndexChanged;
//            //entryMatchScoreB.SelectedIndexChanged += entryMatchScore_SelectedIndexChanged;
//            var panelMatchScore = new Grid
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0,0,0,0),
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
//                },
//                ColumnDefinitions = {
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(Config.IsTablet ? 100 : 70, GridUnitType.Absolute) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Absolute) },
//                }
//            };
//            panelMatchScore.Children.Add(imageYou, 1, 0);
//            this.panelMatchScoreInner = new StackLayout
//            {
//                Orientation = StackOrientation.Vertical,
//                VerticalOptions = LayoutOptions.Center,
//                Children =
//                {
//                    new Label
//                    {
//                        Text = "Match score",
//                        TextColor = Config.ColorTextOnBackgroundGrayed,
//                        HorizontalOptions = LayoutOptions.Center,
//                        HorizontalTextAlignment = TextAlignment.Center
//                    },
//                    new StackLayout
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        HorizontalOptions = LayoutOptions.Center,
//                        Children =
//                        {
//                            entryMatchScoreA,
//                            new Label
//                            {
//                                Text = ":",
//                                TextColor = Color.White,
//                                VerticalTextAlignment = TextAlignment.Center,
//                                FontFamily = Config.FontFamily,
//                                FontSize = Config.LargerFontSize,
//                                FontAttributes = FontAttributes.Bold
//                            },
//                            entryMatchScoreB
//                        }
//                    }
//                }
//            };
//            panelMatchScore.Children.Add(this.panelMatchScoreInner, 2, 0);
//            panelMatchScore.Children.Add(imageOpponent, 3, 0);

//            /// panel "caution, entering match score"
//            /// 
//            panelEnteringMatchScore = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                Padding = new Thickness(20,10,20,20),
//                Spacing = 20,
//                IsVisible = false,
//                Children =
//                {
//                    new Label
//                    {
//                        FormattedText = new FormattedString()
//                        {
//                            Spans = 
//                            {
//                                new Span() { Text = "Entering "},
//                                new Span() { Text = "match", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily },
//                                new Span() { Text = " score"},
//                            }
//                        },
//                        //Text = "Entering MATCH score",
//                        TextColor = Config.ColorTextOnBackground,
//                        HorizontalOptions = LayoutOptions.Start,
//                    },
//                    new Label
//                    {
//                        FormattedText = new FormattedString()
//                        {
//                            Spans =
//                            {
//                                new Span() { Text = "Caution: ", FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, ForegroundColor = Color.Red },
//                                new Span() { Text = " Logging a match score manually will remove all frames you might have already entered."},
//                            }
//                        },

//                        //Text = "Caution: Logging a match score manually will remove all frames you might have already entered.",
//                        TextColor = Config.ColorTextOnBackground,
//                        HorizontalOptions = LayoutOptions.Start,
//                    },
//                }
//            };

//            /// previus / next frame buttons
//            /// 
//            this.panelPrevNext = new Grid
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0, 0, 0, 0),
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },
//                    new RowDefinition { Height = new GridLength(50, GridUnitType.Absolute) },
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },
//                },
//                ColumnDefinitions =
//                {
//                    new ColumnDefinition { Width = new GridLength(15, GridUnitType.Absolute) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(15, GridUnitType.Absolute) },
//                }
//            };
//            this.buttonPrevFrame = new Button()
//            {
//                Text = "Previous",
//                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
//                HorizontalOptions = LayoutOptions.Start,
//                TextColor = Color.White//Config.ColorTextOnBackgroundGrayed
//            };
//            this.buttonCurFrame = new Button()
//            {
//                Text = "Frame 1",
//                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
//                HorizontalOptions = LayoutOptions.Center,
//                TextColor = Config.ColorTextOnBackground
//            };
//            this.buttonNextFrame = new Button()
//            {
//                Text = "Next",
//                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
//                HorizontalOptions = LayoutOptions.End,
//                TextColor = Color.White//Config.ColorTextOnBackgroundGrayed
//            };
//            this.buttonPrevFrame.Clicked += buttonPrevFrame_Clicked;
//            this.buttonNextFrame.Clicked += buttonNextFrame_Clicked;
//            this.buttonCurFrame.Clicked += buttonCurFrame_Clicked;
//            panelPrevNext.Children.Add(this.buttonPrevFrame, 1, 1);
//            panelPrevNext.Children.Add(this.buttonCurFrame, 2, 1);
//            panelPrevNext.Children.Add(this.buttonNextFrame, 3, 1);
//            panelPrevNext.Children.Add(new BoxView()
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                Color = Color.White,
//                HeightRequest = 1,
//                Opacity = 0.2,
//            }, 0, 5, 0, 1);
//            //panelPrevNext.Children.Add(new BoxView()
//            //{
//            //    HorizontalOptions = LayoutOptions.FillAndExpand,
//            //    Color = Color.White,
//            //    HeightRequest = 1,
//            //    Opacity = 0.2,
//            //}, 0, 5, 2, 3);

//            /// entering frame score panel
//            /// 
//            FormattedString formattedString = new FormattedString();
//            formattedString.Spans.Add(new Span() { Text = "Entering " });
//            formattedString.Spans.Add(new Span() { Text = "frame score", FontAttributes = FontAttributes.Bold, FontSize = Config.DefaultFontSize });
//            formattedString.Spans.Add(new Span() { Text = ". Tap here when done." });
//            panelEnteringFrameScore = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                HeightRequest = 52,
//                IsVisible = false,
//                Padding = new Thickness(0,10,0,0),
//                Children =
//                {
//                    new Label
//                    {
//                        FormattedText = formattedString,
//                        HorizontalOptions = LayoutOptions.Center,
//                        VerticalOptions = LayoutOptions.Center,
//                        TextColor = Color.White,
//                        HorizontalTextAlignment = TextAlignment.Center,
//                        VerticalTextAlignment = TextAlignment.Center
//                    }
//                }
//            };

//            /// current frame
//            /// 
//            this.panelCurrentFrame = new Grid()
//            {
//                Padding = new Thickness(10,0,10,0),
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                ColumnSpacing = 1,
//                RowSpacing = 1,
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(Config.LargeButtonsHeight * 1.5, GridUnitType.Absolute) },
//                    new RowDefinition { Height = new GridLength(Config.LargeButtonsHeight, GridUnitType.Absolute) },
//                    //new RowDefinition { Height = new GridLength(Config.LargeButtonsHeight, GridUnitType.Absolute) },
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
//                },
//                ColumnDefinitions =
//                {
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                }
//            };
//			this.entryCurrentFrameA = new LargeNumberEntry("", Config.ColorBackground)
//            {
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = Config.IsIOS ? new Thickness(0, 20, 0, 5) : new Thickness(0, 10, 0, 1),
//            };
//			this.entryCurrentFrameB = new LargeNumberEntry("", Config.ColorBackground)
//            {
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = Config.IsIOS ? new Thickness(0, 20, 0, 5) : new Thickness(0, 10, 0, 1),
//            };
//            this.panelCurrentFrame.Children.Add(this.entryCurrentFrameA, 0, 0);
//            this.panelCurrentFrame.Children.Add(this.entryCurrentFrameB, 1, 0);
//            this.buttonNewBreakA = new Button() { Text = "Add a break", Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, TextColor = Color.White, BackgroundColor = Config.ColorBackground, BorderRadius = 0 };
//            this.buttonNewBreakB = new Button() { Text = "Add a break", Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, TextColor = Color.White, BackgroundColor = Config.ColorBackground, BorderRadius = 0 };
//            //this.buttonAddPointsA = new Button() { Text = "+", Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, TextColor = Color.White, BackgroundColor = Config.ColorBackground, BorderRadius = 0 };
//            //this.buttonAddPointsB = new Button() { Text = "+", Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill, TextColor = Color.White, BackgroundColor = Config.ColorBackground, BorderRadius = 0 };
//            this.panelCurrentFrame.Children.Add(this.buttonNewBreakA, 0, 1);
//            this.panelCurrentFrame.Children.Add(this.buttonNewBreakB, 1, 1);
//            //this.panelCurrentFrame.Children.Add(this.buttonAddPointsA, 0, 1);
//            //this.panelCurrentFrame.Children.Add(this.buttonAddPointsB, 1, 1);
//            this.listOfBreaksInMatchControl = new ListOfBreaksInMatchControl();
//            this.listOfBreaksInMatchControl.UserTappedOnBreak += listOfBreaksInMatchControl_UserTappedOnBreak;
//            this.panelCurrentFrame.Children.Add(this.listOfBreaksInMatchControl, 0, 2, 2, 3);

//            this.entryCurrentFrameA.FocusedOnNumber += entryCurrentFrameA_FocusedOnNumber;
//            this.entryCurrentFrameB.FocusedOnNumber += entryCurrentFrameB_FocusedOnNumber;
//            this.entryCurrentFrameA.UnfocusedFromNumber += entryCurrentFrameA_UnfocusedFromNumber;
//            this.entryCurrentFrameB.UnfocusedFromNumber += entryCurrentFrameB_UnfocusedFromNumber;
//            this.entryCurrentFrameA.NumberChanged += entryCurrentFrameA_NumberChanged;
//            this.entryCurrentFrameB.NumberChanged += entryCurrentFrameB_NumberChanged;

//            this.buttonNewBreakA.Clicked += buttonNewBreakA_Clicked;
//            this.buttonNewBreakB.Clicked += buttonNewBreakB_Clicked;
//            //this.buttonAddPointsA.Clicked += buttonAddPointsA_Clicked;
//            //this.buttonAddPointsB.Clicked += buttonAddPointsB_Clicked;

//            /// buttons
//            /// 
//            this.buttonPause = new Button() { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Pause" };
//            this.buttonFinish = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Next or finish" };
//            this.buttonCancelMatch = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel match" };
//            this.buttonPause.Clicked += buttonPause_Clicked;
//            this.buttonFinish.Clicked += buttonFinish_Clicked;
//            this.buttonCancelMatch.Clicked += buttonCancelMatch_Clicked;
//            var panelOkCancel = new StackLayout()
//            {
//                Orientation = StackOrientation.Horizontal,
//                //BackgroundColor = Config.ColorBackground,
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.Fill,
//                HeightRequest = Config.OkCancelButtonsHeight,
//                Padding = new Thickness(Config.OkCancelButtonsPadding),
//                Spacing = 1,
//                Children =
//                {
//                    buttonPause,
//                    buttonFinish,
//                    buttonCancelMatch
//                }
//            };

//            /// Top-level Grid
//            /// 
//            this.grid = new Grid
//            {
//                ColumnSpacing = 0,
//                RowSpacing = 0,
//                Padding = new Thickness(0),
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.Start,//.FillAndExpand,
//                RowDefinitions =
//                {
//                    new RowDefinition { Height = new GridLength(Config.TitleHeight, GridUnitType.Absolute) },                                             // title
//                    new RowDefinition { Height = new GridLength(Config.DeviceScreenHeightInInches < 4 ? 10 : 20, GridUnitType.Absolute) },                // empty space
//                    new RowDefinition { Height = new GridLength(Config.IsTablet ? 150 : 90, GridUnitType.Absolute) },                                     // match score
//                    new RowDefinition { Height = new GridLength(Config.DeviceScreenHeightInInches < 4 ? 10 : 20, GridUnitType.Absolute) },                // empty space
//                    new RowDefinition { Height = new GridLength(52, GridUnitType.Absolute) },                                                             // prev / next frame
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },                                                                  // current frame
//                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },                                                              // empty space
//                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding*2, GridUnitType.Absolute) }, // buttons
//                },
//                ColumnDefinitions =
//                {
//                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
//                }
//            };
//            grid.Children.Add(panelTitle, 0, 0);
//            grid.Children.Add(panelMatchScore, 0, 2);
//            grid.Children.Add(panelPrevNext, 0, 4);
//            grid.Children.Add(this.panelEnteringFrameScore, 0, 4);
//            grid.Children.Add(this.panelEnteringMatchScore, 0, 1, 4, 6);
//            this.scrollViewCurrentFrame = new ScrollView()
//            {
//                HorizontalOptions = LayoutOptions.FillAndExpand,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                Padding = new Thickness(0),
//                Content = this.panelCurrentFrame
//            };
//            grid.Children.Add(this.scrollViewCurrentFrame, 0, 5);
//            grid.Children.Add(panelOkCancel, 0, 7);

//            /// slide-out panel with metadata
//            /// 
//            //this.metadataControl = new SnookerMatchMetadataControl(new SnookerMatchMetadata(), false)
//            //{
//            //    Padding = new Thickness(15, 15, 15, 15)
//            //};
//            //this.metadataControl.VenueSelected += (s1, e1) => { this.removeSlideouts(); };
//            //this.buttonHideMetadata = new Button() { Text = "OK", Style = (Style)App.Current.Resources["BlackButtonStyle"], HeightRequest = Config.OkCancelButtonsHeight, VerticalOptions = LayoutOptions.Start };
//            //this.buttonHideMetadata.Clicked += (s1, e1) => { this.removeSlideouts(); };
//            //this.panelSlideoutMetadata = new StackLayout()
//            //{
//            //    Orientation = StackOrientation.Vertical,
//            //    Padding = new Thickness(15,0,15,0),
//            //    HorizontalOptions = LayoutOptions.FillAndExpand,
//            //    IsVisible = false,
//            //    BackgroundColor = Config.ColorBackground,
//            //    Children =
//            //    {
//            //        this.metadataControl,
//            //        this.buttonHideMetadata
//            //    }
//            //};

//            /// slide-out panel with buttons
//            /// 
//            this.buttonStartNewFrame = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Start next frame" };
//            this.buttonFinishMatch = new Button() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Finish match" };
//            this.buttonCancelSlideout = new Button() { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
//            buttonStartNewFrame.Clicked += buttonStartNewFrame_Clicked;
//            buttonFinishMatch.Clicked += buttonFinishMatch_Clicked;
//            buttonCancelSlideout.Clicked += (s1, e1) => { this.removeSlideouts(); };
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

//            /// content
//            /// 
//            this.absoluteLayout = new AbsoluteLayout()
//            {
//                VerticalOptions = LayoutOptions.FillAndExpand
//            };
//            this.absoluteLayout.Children.Add(grid, new Point(0, 0));
//            this.absoluteLayout.Children.Add(panelSlideoutButtons, new Rectangle(0, this.Height - 100, this.Width, 100));
//            //this.absoluteLayout.Children.Add(panelSlideoutMetadata, new Rectangle(0, -100, this.Width, 100));
//            Content = this.absoluteLayout;
//            this.Padding = new Thickness(0, 0, 0, 0);
//		}

//        private async void listOfBreaksInMatchControl_UserTappedOnBreak(object sender, SnookerBreak snookerBreak)
//        {
//            if (this.IsReadonlyMode)
//            {
//                if (this.MatchScore.YourBreaks != null && this.MatchScore.YourBreaks.Contains(snookerBreak) == false)
//                    return;

//                if (await this.DisplayAlert("Notable break?", "Would you like to make this a notable break?", "Yes", "Cancel") == true)
//                {
//                    int myAthleteID = App.Repository.GetMyAthleteID();
//                    var allResults = App.Repository.GetResults(myAthleteID, false);

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

//                    var result = new Result();
//                    snookerBreak.PostToResult(result);
//                    result.VenueID = MatchScore.VenueID;
//                    result.OpponentAthleteID = MatchScore.OpponentAthleteID;
//                    result.AthleteID = myAthleteID;
//                    result.TimeModified = DateTimeHelper.GetUtcNow();
//                    App.Repository.AddResult(result);

//                    await App.Navigator.NavPage.Navigation.PopModalAsync();
//                    App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Breaks);
//                }
//            }
//            else
//            {
//				string strKeepTheScore = "Delete the break, keep the score";
//				string strSubtractFromTheScore = "Delete the break, subtract " + snookerBreak.Points.ToString() + " from the frame score";
//				string strResult = await this.DisplayActionSheet ("Delete the break?", "Cancel", null, strSubtractFromTheScore, strKeepTheScore);
				
//                //if (await this.DisplayAlert("Delete the break", "Delete the break of " + snookerBreak.Points.ToString() + " points?", "Yes", "Cancel") == true)
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

//					this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//					this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//					this.updateBottomButtons();
//					this.updatePrevNextButtons();
//				}
//            }
//        }

//        private async void buttonCurFrame_Clicked(object sender, EventArgs e)
//        {
//            if (this.MatchScore.IsEmpty)
//                return;

//            string title = this.buttonCurFrame.Text;
//            string cancel = "Cancel";
//            string delete = "Delete this frame";

//            string result = await this.DisplayActionSheet(title, cancel, delete);

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

//                this.ignoreUIevents = true;
//                this.entryMatchScoreA.Number = MatchScore.MatchScoreA;
//                this.entryMatchScoreB.Number = MatchScore.MatchScoreB;
//                //this.entryMatchScoreA.Text = MatchScore.MatchScoreA.ToString();
//                //this.entryMatchScoreB.Text = MatchScore.MatchScoreB.ToString();
//                //this.entryMatchScoreA.SelectedIndex = MatchScore.MatchScoreA;
//                //this.entryMatchScoreB.SelectedIndex = MatchScore.MatchScoreB;
//                this.ignoreUIevents = false;

//                this.goToNextFrame();
//            }
//        }

//        private void buttonNextFrame_Clicked(object sender, EventArgs e)
//        {
//            this.goToNextFrame();
//        }

//        private void buttonPrevFrame_Clicked(object sender, EventArgs e)
//        {
//            this.goToPrevFrame();
//        }

//        private void entryCurrentFrameB_NumberChanged(object sender, EventArgs e)
//        {
//            this.CurrentFrameScore.B = this.entryCurrentFrameB.Number ?? 0;
//            this.updateBottomButtons();
//        }

//        private void entryCurrentFrameA_NumberChanged(object sender, EventArgs e)
//        {
//            this.CurrentFrameScore.A = this.entryCurrentFrameA.Number ?? 0;
//            this.updateBottomButtons();
//        }

//        private void entryCurrentFrameB_FocusedOnNumber(object sender, EventArgs e)
//        {
//            this.panelEnteringFrameScore.IsVisible = true;
//            this.panelMatchScoreInner.IsVisible = false;
//            this.panelPrevNext.IsVisible = false;
//            this.buttonNewBreakA.IsVisible = false;
//            this.buttonNewBreakB.IsVisible = false;
//            //this.buttonAddPointsA.IsVisible = false;
//            //this.buttonAddPointsB.IsVisible = false;
//        }

//        private void entryCurrentFrameA_FocusedOnNumber(object sender, EventArgs e)
//        {
//            this.panelEnteringFrameScore.IsVisible = true;
//            this.panelMatchScoreInner.IsVisible = false;
//            this.panelPrevNext.IsVisible = false;
//            this.buttonNewBreakA.IsVisible = false;
//            this.buttonNewBreakB.IsVisible = false;
//            //this.buttonAddPointsA.IsVisible = false;
//            //this.buttonAddPointsB.IsVisible = false;
//        }

//        private void entryCurrentFrameB_UnfocusedFromNumber(object sender, EventArgs e)
//        {
//            this.panelEnteringFrameScore.IsVisible = false;
//            this.panelMatchScoreInner.IsVisible = true;
//            this.panelPrevNext.IsVisible = true;
//            this.buttonNewBreakA.IsVisible = true;
//            this.buttonNewBreakB.IsVisible = true;
//            //this.buttonAddPointsA.IsVisible = true;
//            //this.buttonAddPointsB.IsVisible = true;
//        }

//        private void entryCurrentFrameA_UnfocusedFromNumber(object sender, EventArgs e)
//        {
//            this.panelEnteringFrameScore.IsVisible = false;
//            this.panelMatchScoreInner.IsVisible = true;
//            this.panelPrevNext.IsVisible = true;
//            this.buttonNewBreakA.IsVisible = true;
//            this.buttonNewBreakB.IsVisible = true;
//            //this.buttonAddPointsA.IsVisible = true;
//            //this.buttonAddPointsB.IsVisible = true;
//        }

//        private async void buttonCancelMatch_Clicked(object sender, EventArgs e)
//        {
//            if (this.IsReadonlyMode)
//            {
//                // just close
//                await App.Navigator.NavPage.Navigation.PopModalAsync();
//                return;
//            }

//            this.finishOrPauseMatch(false);
//        }

//        //void openMetadataSlideout()
//        //{
//        //    if (this.IsReadonlyMode)
//        //        return;
//        //    double slideoutHeight = 270;

//        //    // cover the page with a boxview to avoid taps on other elements
//        //    this.boxViewSlideoutCover = new BoxView() { Color = Color.FromRgba(0, 0, 0, 50) };
//        //    this.boxViewSlideoutCover.GestureRecognizers.Add(new TapGestureRecognizer
//        //    {
//        //        Command = new Command(() => { this.removeSlideouts(); }),
//        //        NumberOfTapsRequired = 1
//        //    });
//        //    this.absoluteLayout.Children.Add(boxViewSlideoutCover, new Rectangle(0, slideoutHeight, this.Width, Height - slideoutHeight));

//        //    // slide-out
//        //    AbsoluteLayout.SetLayoutBounds(this.panelSlideoutMetadata, new Rectangle(0, 0 - slideoutHeight, this.Width, slideoutHeight));
//        //    this.panelSlideoutMetadata.LayoutTo(new Rectangle(0, 20, this.Width, slideoutHeight), 250);
//        //    this.grid.FadeTo(0.5, 250);
//        //    this.panelSlideoutMetadata.IsVisible = true;
//        //}

//        void removeSlideouts()
//        {
//            this.grid.FadeTo(1.0, 250);
//            this.panelSlideoutButtons.IsVisible = false;
//            //this.panelSlideoutMetadata.IsVisible = false;
//            this.boxViewSlideoutCover.IsVisible = false;

//            this.absoluteLayout.Children.Remove(this.boxViewSlideoutCover);
//            this.boxViewSlideoutCover = null;
//        }

//        void openButtonsSlideout()
//        {
//            double slideoutHeight = Config.OkCancelButtonsHeight * 3 + Config.OkCancelButtonsPadding * 2;

//            // cover the page with a boxview to avoid taps on other elements
//            this.boxViewSlideoutCover = new BoxView() { Color = Color.FromRgba(0, 0, 0, 50) };
//            this.boxViewSlideoutCover.GestureRecognizers.Add(new TapGestureRecognizer
//            {
//                Command = new Command(() => { this.removeSlideouts(); }),
//                NumberOfTapsRequired = 1
//            });
//            this.absoluteLayout.Children.Add(boxViewSlideoutCover, new Rectangle(0, 0, this.Width, Height - slideoutHeight));

//            // slide-out
//            AbsoluteLayout.SetLayoutBounds(this.panelSlideoutButtons, new Rectangle(0, this.Height, this.Width, slideoutHeight));
//            this.panelSlideoutButtons.LayoutTo(new Rectangle(0, this.Height - slideoutHeight, this.Width, slideoutHeight), 250);
//            this.grid.FadeTo(0.5, 250);
//            this.panelSlideoutButtons.IsVisible = true;

//			//this.panelSlideoutButtons.BackgroundColor = Color.Yellow;
//			this.buttonStartNewFrame.HeightRequest = 40; // if this is not done, the buttons do not become visisble
//        }

//        private void buttonStartNewFrame_Clicked(object sender, EventArgs e)
//        {
//            this.removeSlideouts();
//            this.startNewFrame();
//        }

//        private void buttonFinishMatch_Clicked(object sender, EventArgs e)
//        {
//            this.removeSlideouts();
//            this.finishOrPauseMatch(false);
//        }

//        private void buttonFinish_Clicked(object sender, EventArgs e)
//        {
//            if (boxViewSlideoutCover != null)
//                return;

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

//            this.openButtonsSlideout();
//        }

//        private void buttonPause_Clicked(object sender, EventArgs e)
//        {
//            finishOrPauseMatch(true);
//        }

//        private void buttonNewBreakA_Clicked(object sender, EventArgs e)
//        {
//            var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);
//            var page = new NewSnookerBreakPage(metadata, false, false);
//            page.Done += (s1, snookerBreak) =>
//            {
//                if (snookerBreak == null)
//                    return;
//                snookerBreak.Date = DateTime.Now;
//                snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;

//                if (this.MatchScore.YourBreaks == null)
//                    this.MatchScore.YourBreaks = new List<SnookerBreak>();
//                if (this.MatchScore.OpponentBreaks == null)
//                    this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//                this.MatchScore.YourBreaks.Add(snookerBreak);
//                this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);

//                this.CurrentFrameScore.A += snookerBreak.Points;
//                this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//                this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//                this.updateBottomButtons();
//                this.updatePrevNextButtons();
                
//            };
//            App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        }

//        private void buttonNewBreakB_Clicked(object sender, EventArgs e)
//        {
//            var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);//.FromScoreForOpponent(this.MatchScore);
//            var page = new NewSnookerBreakPage(metadata, true, false);
//            page.Done += (s1, snookerBreak) =>
//            {
//                if (snookerBreak == null)
//                    return;
//                snookerBreak.Date = DateTime.Now;
//                snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;
//                snookerBreak.OpponentAthleteID = this.MatchScore.YourAthleteID;

//                if (this.MatchScore.YourBreaks == null)
//                    this.MatchScore.YourBreaks = new List<SnookerBreak>();
//                if (this.MatchScore.OpponentBreaks == null)
//                    this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//                this.MatchScore.OpponentBreaks.Add(snookerBreak);
//                this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);

//                this.CurrentFrameScore.B += snookerBreak.Points;
//                this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//                this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//                this.updateBottomButtons();
//                this.updatePrevNextButtons();
//            };
//            App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        }

//        //private void buttonAddPointsA_Clicked(object sender, EventArgs e)
//        //{
//        //    var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);
//        //    var page = new AddPointsPage(metadata, false);
//        //    page.Done += (s1, e1) =>
//        //    {
//        //        this.addPoints(page.SnookerBreak, false);
//        //    };
//        //    App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        //}

//        //private void buttonAddPointsB_Clicked(object sender, EventArgs e)
//        //{
//        //    var metadata = new MetadataHelper().FromScoreForYou(this.MatchScore);
//        //    var page = new AddPointsPage(metadata, true);
//        //    page.Done += (s1, e1) =>
//        //    {
//        //        this.addPoints(page.SnookerBreak, true);
//        //    };
//        //    App.Navigator.NavPage.Navigation.PushModalAsync(page);
//        //}

//        //void addPoints(SnookerBreak snookerBreak, bool isOpponent)
//        //{
//        //    snookerBreak.FrameNumber = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1;

//        //    if (this.MatchScore.YourBreaks == null)
//        //        this.MatchScore.YourBreaks = new List<SnookerBreak>();
//        //    if (this.MatchScore.OpponentBreaks == null)
//        //        this.MatchScore.OpponentBreaks = new List<SnookerBreak>();
//        //    if (isOpponent)
//        //        this.MatchScore.OpponentBreaks.Add(snookerBreak);
//        //    else
//        //        this.MatchScore.YourBreaks.Add(snookerBreak);
//        //    this.listOfBreaksInMatchControl.Fill(this.MatchScore, snookerBreak.FrameNumber);

//        //    if (isOpponent)
//        //        this.CurrentFrameScore.B += snookerBreak.Points;
//        //    else
//        //        this.CurrentFrameScore.A += snookerBreak.Points;
//        //    this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//        //    this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;
//        //    this.updateBottomButtons();
//        //    this.updatePrevNextButtons();
//        //}

//        private void entryMatchScore_NumberChanged(object sender, EventArgs e)
//        {
//            if (this.ignoreUIevents)
//                return;

//            this.MatchScore.FrameScores.Clear();
//            this.MatchScore.MatchScoreA = this.entryMatchScoreA.Number ?? 0;
//            this.MatchScore.MatchScoreB = this.entryMatchScoreB.Number ?? 0;

//            this.ignoreUIevents = true;
//            this.entryCurrentFrameA.Number = null;
//            this.entryCurrentFrameB.Number = null;
//            this.ignoreUIevents = false;

//            this.updatePrevNextButtons();
//            this.updateBottomButtons();
//        }

//        //private void entryMatchScore_TextChanged(object sender, TextChangedEventArgs e)
//        //{
//        //    if (this.ignoreUIevents)
//        //        return;

//        //    double a = 0;
//        //    double.TryParse(this.entryMatchScoreA.Text, out a);
//        //    this.MatchScore.MatchScoreA = (int)a;
//        //    double b = 0;
//        //    double.TryParse(this.entryMatchScoreB.Text, out b);
//        //    this.MatchScore.MatchScoreB = (int)b;

//        //    this.updateBottomButtons();
//        //}

//        //private void entryMatchScore_SelectedIndexChanged(object sender, EventArgs e)
//        //{
//        //    if (this.ignoreUIevents)
//        //        return;
//        //    this.MatchScore.FrameScores.Clear();
//        //    this.MatchScore.MatchScoreA = this.entryMatchScoreA.SelectedIndex;
//        //    this.MatchScore.MatchScoreB = this.entryMatchScoreB.SelectedIndex;
//        //    this.updateBottomButtons();
//        //}

//        private void entryMatchScore_Unfocused(object sender, EventArgs e)
//        {
//            this.panelEnteringMatchScore.IsVisible = false;
//            this.panelPrevNext.IsVisible = true;
//            this.panelCurrentFrame.IsVisible = true;
//        }

//        private void entryMatchScore_Focused(object sender, EventArgs e)
//        {
//            this.panelEnteringMatchScore.IsVisible = true;
//            this.panelPrevNext.IsVisible = false;
//            this.panelCurrentFrame.IsVisible = false;
//        }

//        private void imageYou_Clicked()
//        {
//            if (this.IsReadonlyMode)
//                return;
//            App.Navigator.DisplayAlertRegular("Tap on the opponent picture to select the opponent.");
//        }

//        private async void imageOpponent_Clicked()
//        {
//            if (this.IsReadonlyMode)
//                return;
//            if (this.MatchScore.OpponentAthleteID > 0)
//            {
//                string strCancel = "Cancel";
//                string strPickOther = "Change the opponent";

//                if (await this.DisplayActionSheet("Byb", strCancel, strPickOther) != strPickOther)
//                    return;
//            }

//            PickAthletePage dlg = new PickAthletePage();
//            await App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
//            dlg.UserMadeSelection += (s1, e1) =>
//            {
//                App.Navigator.NavPage.Navigation.PopModalAsync();
//                if (e1.Person != null)
//                {
//                    this.MatchScore.OpponentAthleteID = e1.Person.ID;
//                    this.MatchScore.OpponentName = e1.Person.Name;
//                    this.MatchScore.OpponentPicture = e1.Person.Picture;

//                    this.fillMetadata();
//                }
//            };
//        }

//        protected override void OnDisappearing()
//        {
//            base.OnDisappearing();
//        }

//        protected override void OnAppearing()
//        {
//            base.OnAppearing();
//        }

//        protected override void OnSizeAllocated(double width, double height)
//        {
//            base.OnSizeAllocated(width, height);

//            // make sure that the scrollview takes up all available space
//            double fullHeight = this.Height;
//            double heightForScrollview = this.Height;
//            foreach (var row in this.grid.RowDefinitions)
//                if (row.Height.IsAbsolute)
//                    heightForScrollview -= row.Height.Value;
//            this.scrollViewCurrentFrame.HeightRequest = heightForScrollview;
//            this.grid.HeightRequest = fullHeight;
//            this.grid.WidthRequest = this.Width;
//        }

//        void fill()
//        {
//            this.fillMetadata();

//            this.ignoreUIevents = true;
//            this.entryMatchScoreA.Number = MatchScore.MatchScoreA;
//            this.entryMatchScoreB.Number = MatchScore.MatchScoreB;
//            //this.entryMatchScoreA.Text = MatchScore.MatchScoreA.ToString();
//            //this.entryMatchScoreB.Text = MatchScore.MatchScoreB.ToString();
//            //this.entryMatchScoreA.SelectedIndex = MatchScore.MatchScoreA;
//            //this.entryMatchScoreB.SelectedIndex = MatchScore.MatchScoreB;
//            this.ignoreUIevents = false;

//            this.goToNextFrame();
//            this.updateBottomButtons();

//            if (this.IsReadonlyMode)
//            {
//                this.entryMatchScoreA.IsEnabled = false;
//                this.entryMatchScoreB.IsEnabled = false;
//                //this.imageOpenMetadata.IsEnabled = false;
//                this.entryCurrentFrameA.IsEnabled = false;
//                this.entryCurrentFrameB.IsEnabled = false;
//                this.buttonNewBreakA.IsEnabled = false;
//                this.buttonNewBreakB.IsEnabled = false;
//                //this.buttonAddPointsA.IsEnabled = false;
//                //this.buttonAddPointsB.IsEnabled = false;
//                this.buttonCurFrame.IsEnabled = false;
//            }
//        }

//        void fillMetadata()
//        {
//            this.Title = MatchScore.NameVsNameShortened;

//            this.labelTitle.Text = MatchScore.NameVsNameShortened;
//            this.imageYou.Source = App.ImagesService.GetImageSource(MatchScore.YourPicture, BackgroundEnum.Black);
//            if (MatchScore.OpponentAthleteID == 0)
//                this.imageOpponent.Source = new FileImageSource() { File = "plusBlack.png" };
//            else
//                this.imageOpponent.Source = App.ImagesService.GetImageSource(MatchScore.OpponentPicture, BackgroundEnum.Black);
//        }

//        void startNewFrame()
//        {
//            string validationMessage;
//            if (this.CurrentFrameScore.Validate(out validationMessage) == false)
//            {
//                this.DisplayAlert("Byb", validationMessage, "Cancel");
//                return;
//            }

//            this.MatchScore.CalculateMatchScoreFromFrameScores();
//            this.MatchScore.FrameScores.Add(new SnookerFrameScore());

//            this.ignoreUIevents = true;
//            this.entryMatchScoreA.Number = MatchScore.MatchScoreA;
//            this.entryMatchScoreB.Number = MatchScore.MatchScoreB;
//            //this.entryMatchScoreA.Text = MatchScore.MatchScoreA.ToString();
//            //this.entryMatchScoreB.Text = MatchScore.MatchScoreB.ToString();
//            //this.entryMatchScoreA.SelectedIndex = MatchScore.MatchScoreA;
//            //this.entryMatchScoreB.SelectedIndex = MatchScore.MatchScoreB;
//            this.ignoreUIevents = false;

//            this.goToNextFrame();
//            this.updateBottomButtons();
//        }

//        //async void cancelMatch()
//        //{
//        //    if (this.MatchScore.IsEmpty == false && this.IsReadonlyMode == false)
//        //        return;

//        //    await App.Navigator.NavPage.Navigation.PopModalAsync();
//        //}

//        async void finishOrPauseMatch(bool justPause)
//        {
//            if (this.IsReadonlyMode || this.MatchScore.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
//            {
//                // just close
//                await App.Navigator.NavPage.Navigation.PopModalAsync();
//                return;
//            }

//            if (justPause == false)
//            {
//                // validate all frames
//                foreach (var frame in MatchScore.FrameScores)
//                {
//                    if (frame.IsEmpty == true)
//                        continue;

//                    string validationMessage;
//                    if (frame.Validate(out validationMessage) == false)
//                    {
//                        await this.DisplayAlert("Byb", "Cannot finish the match. Frame #" + (MatchScore.FrameScores.IndexOf(frame) + 1).ToString() + " is invalid. Message: " + validationMessage, "Cancel");
//                        return;
//                    }
//                }

//                // remove empty frames
//                var emptyFrames = this.MatchScore.FrameScores.Where(i => i.IsEmpty).ToList();
//                foreach (var frame in emptyFrames)
//                    this.MatchScore.FrameScores.Remove(frame);

//                if (this.MatchScore.FrameScores.Count > 0)
//                    this.MatchScore.CalculateMatchScoreFromFrameScores();
//            }

//            bool isEmpty = this.MatchScore.MatchScoreA == 0 && this.MatchScore.MatchScoreB == 0 && this.MatchScore.HasFrameScores == false;

//            if (isEmpty == false && justPause == false)
//            {
//                if (MatchScore.OpponentAthleteID <= 0)
//                {
//                    await this.DisplayAlert("Byb", "Cannot 'finish' the match until you select the opponent. You can 'pause' instead.", "Cancel");
//                    return;
//                }

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

//            await App.Navigator.NavPage.Navigation.PopModalAsync();
//            if (justPause || isEmpty)
//                App.Navigator.GoToRecord();
//            else
//            {
//                App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Matches);
//                App.Navigator.StartSyncAndCheckForNotifications();
//            }
//        }

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

//            this.buttonCurFrame.Text = "Frame #" + (this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1).ToString();
//            this.updateBottomButtons();
//            this.updatePrevNextButtons();
//            this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);
//        }

//        void goToPrevFrame()
//        {
//            if (this.CurrentFrameScore == null)
//                return;

//            int index = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore);
//            if (index <= 0)
//                return;

//            this.CurrentFrameScore = this.MatchScore.FrameScores[index - 1];
//            this.entryCurrentFrameA.Number = this.CurrentFrameScore.A;
//            this.entryCurrentFrameB.Number = this.CurrentFrameScore.B;

//            this.buttonCurFrame.Text = "Frame #" + (this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1).ToString();
//            this.updateBottomButtons();
//            this.updatePrevNextButtons();
//            this.listOfBreaksInMatchControl.Fill(this.MatchScore, this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore) + 1);
//        }

//        void updatePrevNextButtons()
//        {
//            if (this.MatchScore.FrameScores.Count == 0 || this.CurrentFrameScore == null)
//            {
//                this.buttonPrevFrame.IsEnabled = false;
//                this.buttonNextFrame.IsEnabled = false;
//                return;
//            }
//            int index = this.MatchScore.FrameScores.IndexOf(this.CurrentFrameScore);
//            this.buttonPrevFrame.IsEnabled = index > 0;
//            this.buttonNextFrame.IsEnabled = index < this.MatchScore.FrameScores.Count - 1;
//        }

//        void updateBottomButtons()
//        {
//            if (this.MatchScore.IsEmpty || this.IsReadonlyMode)
//            {
//                this.buttonCancelMatch.IsVisible = true;
//                this.buttonCancelMatch.Text = "Cancel";// this.IsReadonlyMode ? "Cancel" : "Cancel match";
//                this.buttonPause.IsVisible = false;
//                this.buttonFinish.IsVisible = false;
//                return;
//            }

//            bool isOnLastFrame = true;
//            if (this.MatchScore.FrameScores.Count > 1)
//                isOnLastFrame = this.MatchScore.FrameScores.Last() == this.CurrentFrameScore;

//            this.buttonCancelMatch.IsVisible = false;
//            this.buttonPause.IsVisible = true;
//            this.buttonFinish.IsVisible = true;

//            this.buttonFinish.IsEnabled = isOnLastFrame;
//        }
//    }
//}
