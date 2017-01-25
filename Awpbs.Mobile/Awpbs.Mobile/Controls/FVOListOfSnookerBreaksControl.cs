//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Xamarin.Forms;

//namespace Awpbs.Mobile
//{
//    public class FVOListOfSnookerBreaksControl : ListOfItemsControl<SnookerBreak>
//    {
//        public event EventHandler<SnookerEventArgs> UserWantsToDeleteRes;
//        public event EventHandler<SnookerEventArgs> UserWantsToEditRes;

//        //public ListTypeEnum Type { get; set; }
//        public SnookerBreakSortEnum SortType { get; set; }
//        public List<SnookerBreak> AllBreaks { get; private set; }

//        SimpleButtonWithLittleDownArrow buttonSort;

//        public FVOListOfSnookerBreaksControl()
//            : base()
//        {
//            base.MaxCountToShowByDefault = 5;
//            base.MultCountToShow = 10;

//            this.buttonSort = new SimpleButtonWithLittleDownArrow()
//            {
//                HorizontalOptions = LayoutOptions.End,
//            };
//            this.buttonSort.Clicked += buttonSort_Clicked;
//            this.panelTop.Children.Add(this.buttonSort);

//            this.SortType = SnookerBreakSortEnum.ByDate;
//            this.updateSortButton();
//        }

//        public override void Fill(List<SnookerBreak> list)
//        {
//            this.AllBreaks = list;
//            base.Fill(SnookerBreak.SortBy(list, SortType).ToList());
//        }

//        public void Sort(SnookerBreakSortEnum sort)
//        {
//            this.SortType = sort;
//            this.updateSortButton();
//            if (this.AllBreaks != null)
//                base.Fill(SnookerBreak.SortBy(AllBreaks, SortType).ToList());
//        }

//        void updateSortButton()
//        {
//            switch (this.SortType)
//            {
//                case SnookerBreakSortEnum.ByDate: this.buttonSort.Text = "Sort by date"; break;
//                case SnookerBreakSortEnum.ByPoints: this.buttonSort.Text = "Sort by points"; break;
//                case SnookerBreakSortEnum.ByBallCount: this.buttonSort.Text = "Sort by balls"; break;
//                default: this.buttonSort.Text = "Sort by ?"; break;
//            }
//        }

//        async void buttonSort_Clicked(object sender, EventArgs e)
//        {
//            string action1 = "Date";
//            string action2 = "Balls";
//            string action3 = "Points";

//            var action = await App.Navigator.NavPage.DisplayActionSheet("Sort order", "Cancel", null, action1, action2, action3);

//            if (action == action1)
//                this.Sort(SnookerBreakSortEnum.ByDate);
//            else if (action == action2)
//                this.Sort(SnookerBreakSortEnum.ByBallCount);
//            else if (action == action3)
//                this.Sort(SnookerBreakSortEnum.ByPoints);
//        }

//        protected override View createViewForSingleItem(SnookerBreak snookerBreak)
//        {
//            // score
//            Label labelPoints = new BybLabel
//            {
//                Text = snookerBreak.Points.ToString(),
//                TextColor = Config.ColorBlackTextOnWhite,
//                FontSize = Config.LargerFontSize,
//                FontAttributes = FontAttributes.Bold,
//                HorizontalTextAlignment = TextAlignment.Center
//            };
//            Label labelBalls = new BybLabel
//            {
//                Text = snookerBreak.NumberOfBallsDisplay,
//                TextColor = Config.ColorBlackTextOnWhite,
//                FontSize = Config.LargerFontSize,
//                FontAttributes = FontAttributes.Bold,
//                HorizontalTextAlignment = TextAlignment.Center
//            };

//            // balls
//            StackLayout stackForBalls = new StackLayout()
//            {
//                Orientation = StackOrientation.Horizontal,
//                Spacing = 1,
//                Padding = new Thickness(0)
//            };
//            ScrollView scrollViewForBalls = new ScrollView()
//            {
//                Orientation = ScrollOrientation.Horizontal,
//                Padding = new Thickness(0),
//                Content = stackForBalls
//            };
//            if (true)
//            {
//                List<int> balls = snookerBreak.GetBallsEvenWhenUnknown();
//                for (int iBall = 0; iBall < balls.Count; ++iBall)
//                {
//                    int ball = balls[iBall];
//                    Color color = Config.BallColors[ball];
//                    var btn = new BybButton
//                    {
//                        Text = "",
//                        BackgroundColor = color,
//                        BorderColor = Color.Transparent,
//                        TextColor = Color.White,
//                        BorderWidth = 1,
//                        BorderRadius = (int)(Config.ExtraSmallBallSize / 2),
//                        HeightRequest = Config.ExtraSmallBallSize,
//                        MinimumHeightRequest = Config.ExtraSmallBallSize,
//                        WidthRequest = Config.ExtraSmallBallSize,
//                        MinimumWidthRequest = Config.ExtraSmallBallSize,
//                        VerticalOptions = LayoutOptions.Center
//                    };
//                    if (snookerBreak.NumberOfBalls == 0)
//                        btn.Opacity = (balls.Count - iBall) / ((double)balls.Count + 1);
//                    stackForBalls.Children.Add(btn);
//                }
//            }

//            // date
//            FormattedString formattedString = new FormattedString();
//            formattedString.Spans.Add(new Span() { Text = DateTimeHelper.DateToString(snookerBreak.Date), ForegroundColor = Config.ColorBlackTextOnWhite, });
//            if (snookerBreak.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
//            {
//                //formattedString.Spans.Add(new Span() { Text = "  confirmed", ForegroundColor = Config.ColorGrayTextOnWhite });
//            }
//            else if (snookerBreak.OpponentConfirmation == OpponentConfirmationEnum.Declined)
//                formattedString.Spans.Add(new Span() { Text = "  (declined)", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//            else
//                formattedString.Spans.Add(new Span() { Text = "  (unconfirmed)", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//            Label labelDate = new BybLabel()
//            {
//                HorizontalOptions = LayoutOptions.Start,
//                FormattedText = formattedString,
//                VerticalTextAlignment = TextAlignment.Center
//            };

//            // person name / opponent name
//            int personID;
//            string personName;
//            if (this.Type == ListTypeEnum.Venue)
//            {
//                personID = snookerBreak.AthleteID;
//                personName = string.IsNullOrEmpty(snookerBreak.AthleteName) ? "-" : (snookerBreak.AthleteName);
//            }
//            else
//            {
//                personID = snookerBreak.OpponentAthleteID;
//                personName = string.IsNullOrEmpty(snookerBreak.OpponentName) ? "-" : ("vs. " + snookerBreak.OpponentName);
//            }
//            Label labelPerson = new BybLabel()
//            {
//                Text = personName,
//                VerticalTextAlignment = TextAlignment.Center,
//                TextColor = Config.ColorGrayTextOnWhite
//            };
//            if (personID > 0)
//                labelPerson.GestureRecognizers.Add(new TapGestureRecognizer()
//                {
//                    Command = new Command(async () =>
//                    {
//                        await App.Navigator.GoToPersonProfile(personID);
//                    }),
//                    NumberOfTapsRequired = 1
//                });

//            // venue
//            //string venueName = snookerBreak.VenueName;
//            //if (string.IsNullOrEmpty(venueName))
//            //    venueName = "-";
//            //Label labelVenueName = new BybLabel()
//            //{
//            //    Text = venueName,
//            //    VerticalTextAlignment = TextAlignment.Center,
//            //    HorizontalOptions = LayoutOptions.EndAndExpand,
//            //    HorizontalTextAlignment = TextAlignment.End,
//            //    TextColor = Config.ColorGrayTextOnWhite,
//            //    IsVisible = Type != ListTypeEnum.Venue
//            //};
//            //if (snookerBreak.VenueID > 0)
//            //    labelVenueName.GestureRecognizers.Add(new TapGestureRecognizer()
//            //    {
//            //        Command = new Command(async () =>
//            //        {
//            //            await App.Navigator.GoToVenueProfile(snookerBreak.VenueID);
//            //        }),
//            //        NumberOfTapsRequired = 1
//            //    });

//            var panel1 = new StackLayout
//            {
//                Orientation = StackOrientation.Horizontal,
//                BackgroundColor = Color.White,
//                Padding = new Thickness(0, 10, 0, 5),
//                Children =
//                {
//                    new StackLayout
//                    {
//                        Padding = new Thickness(10,0,0,0),
//                        Orientation = StackOrientation.Vertical,
//                        HorizontalOptions = LayoutOptions.FillAndExpand,
//                        VerticalOptions = LayoutOptions.Start,
//                        Children =
//                        {
//                            labelDate,
//                            scrollViewForBalls
//                        }
//                    },
//                    new StackLayout
//                    {
//                        Orientation = StackOrientation.Vertical,
//                        Padding = new Thickness(0, 0, 0, 0),
//                        WidthRequest = Config.IsTablet ? 80 : 40,
//                        MinimumWidthRequest = Config.IsTablet ? 80 : 40,
//                        HorizontalOptions = LayoutOptions.Center,
//                        VerticalOptions = LayoutOptions.Start,
//                        Spacing = 2,
//                        Children =
//                        {
//                            new BybLabel { Text = "Balls", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
//                            labelBalls
//                        }
//                    },
//                    new StackLayout
//                    {
//                        Orientation = StackOrientation.Vertical,
//                        Padding = new Thickness(0, 0, 10, 0),
//                        WidthRequest = Config.IsTablet ? 80 : 40,
//                        MinimumWidthRequest = Config.IsTablet ? 80 : 40,
//                        HorizontalOptions = LayoutOptions.Center,
//                        VerticalOptions = LayoutOptions.Start,
//                        Spacing = 2,
//                        Children =
//                        {
//                            new BybLabel { Text = "Points", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
//                            labelPoints
//                        }
//                    }
//                }
//            };

//            //if (this.Type == ListTypeEnum.PrimaryAthlete)
//            //{
//            //    var recognizer = new TapGestureRecognizer
//            //    {
//            //        Command = new Command(() => { this.showMenu(snookerBreak); }),
//            //        NumberOfTapsRequired = 1
//            //    };
//            //    panel1.GestureRecognizers.Add(recognizer);
//            //}

//            var panel = new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                Spacing = 1,
//                Children =
//                {
//                    panel1,
//                    new StackLayout
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        BackgroundColor = Color.White,
//                        Padding = new Thickness(10,5,10,5),
//                        Children =
//                        {
//                            labelPerson,
//                            //labelVenueName,
//                        }
//                    }
//                }
//            };

//            return panel;
//        }

//        async void showMenu(SnookerBreak snookerBreak)
//        {
//            if (this.Type == ListTypeEnum.PrimaryAthlete || this.List == null)
//                return;

//            string action1 = "Delete this break";
//            string action2 = "Edit this break";
//            string action3 = "Share on Facebook";
//            var action = await App.Navigator.NavPage.DisplayActionSheet("Byb", "Cancel", action1, action2, action3);
//            if (action == action1 && UserWantsToDeleteRes != null)
//                UserWantsToDeleteRes(this, new SnookerEventArgs() { SnookerBreak = snookerBreak });
//            if (action == action2 && UserWantsToEditRes != null)
//                UserWantsToEditRes(this, new SnookerEventArgs() { SnookerBreak = snookerBreak });

//            if (action == action3)
//            {
//                string text = "Notable snooker break: " + snookerBreak.Points.ToString() + " points, " + snookerBreak.NumberOfBalls.ToString() + " balls, scored on " + snookerBreak.Date.Date.ToShortDateString();

//                App.FacebookService.Share(text, (s1, e1) =>
//                {
//                    if (e1 == false)
//                        App.Navigator.DisplayAlertError("Could not share on Facebook.");
//                    else
//                        App.Navigator.DisplayAlertRegular("Thanks!");
//                });
//            }
//        }
//    }

//    public class FVOListOfSnookerBreaksControlOld : StackLayout
//	{
//        //public bool ShowPrimaryAthleteName { get; set; }

//        public SnookerBreakSortEnum SortType { get; set; }
//        //public string AthleteName { get; private set; }
//        //public int AthleteID { get; private set; }
//        //public bool IsMyAthlete { get; private set; }
//        public List<SnookerBreak> Breaks { get; private set; }
//        public List<SnookerBreak> SortedBreaks { get; private set; }

//        StackLayout panelTop;
//        SimpleButtonWithLittleDownArrow buttonSort;

//        public FVOListOfSnookerBreaksControlOld()
//        {
//            this.BackgroundColor = Config.ColorGrayBackground;
//            this.Spacing = 4;
//            this.Padding = new Thickness(5, 5, 5, 5);

//            this.buttonSort = new SimpleButtonWithLittleDownArrow()
//            {
//                IsVisible = false,
//            };
//            this.buttonSort.Clicked += buttonSort_Clicked;
//            this.panelTop = new StackLayout
//            {
//                Orientation = StackOrientation.Horizontal,
//                HorizontalOptions = LayoutOptions.EndAndExpand,
//                Padding = new Thickness(5, 0, 5, 0),
//                Children =
//                {
//                    buttonSort,
//                }
//            };
//            this.Children.Add(this.panelTop);

//            this.SortType = SnookerBreakSortEnum.ByDate;
//            this.updateSortButton();
//        }

//        void updateSortButton()
//        {
//            switch (this.SortType)
//            {
//                case SnookerBreakSortEnum.ByDate: this.buttonSort.Text = "Sort by date"; break;
//                case SnookerBreakSortEnum.ByPoints: this.buttonSort.Text = "Sort by points"; break;
//                case SnookerBreakSortEnum.ByBallCount: this.buttonSort.Text = "Sort by balls"; break;
//                default: this.buttonSort.Text = "Sort by ?"; break;
//            }
//        }

//        async void buttonSort_Clicked(object sender, EventArgs e)
//        {
//            string action1 = "Date";
//            string action2 = "Balls";
//            string action3 = "Points";

//            var action = await App.Navigator.NavPage.DisplayActionSheet("Sort order", "Cancel", null, action1, action2, action3);

//            if (action == action1)
//                this.Sort(SnookerBreakSortEnum.ByDate);
//            else if (action == action2)
//                this.Sort(SnookerBreakSortEnum.ByBallCount);
//            else if (action == action3)
//                this.Sort(SnookerBreakSortEnum.ByPoints);
//        }

//        public void Sort(SnookerBreakSortEnum sort)
//        {
//            this.SortType = sort;
//            this.updateSortButton();
//            if (this.Breaks != null)
//                this.Fill(Breaks);
//        }

//        public void Fill(List<SnookerBreak> breaks)
//        {
//            DateTime timeBegin = DateTime.Now;

//            try
//            {
//                this.Breaks = breaks;

//                this.updateSortButton();

//                var itemsToRemove = this.Children.ToList();
//                itemsToRemove.Remove(panelTop);

//                if (this.Breaks == null)
//                    this.SortedBreaks = new List<SnookerBreak>();
//                else
//                    this.SortedBreaks = SnookerBreak.SortBy(Breaks, SortType);

//                if (Breaks == null)
//                {
//                    string text = "Couldn't load data. Internet issues?";
//                    this.Children.Add(new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"] });
//                    this.Children.Add(new BybLabel
//                    {
//                        Text = text,
//                        HorizontalOptions = LayoutOptions.Center
//                    });
//                    this.Children.Add(new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"] });
//                }
//                else if (SortedBreaks.Count == 0)
//                {
//                    string text = "No notable breaks have been recorded yet";
//                    this.Children.Add(new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"] });
//                    this.Children.Add(new BybLabel
//                    {
//                        Text = text,
//                        HorizontalOptions = LayoutOptions.Center,
//                        HorizontalTextAlignment = TextAlignment.Center
//                    });
//                    this.Children.Add(new BoxView { Style = (Style)App.Current.Resources["BoxViewPadding1Style"] });
//                }

//                DateTime timeBeforeForeach = DateTime.Now;
//                foreach (SnookerBreak snookerBreak in SortedBreaks)
//                {
//                    // score
//                    Label labelPoints = new BybLabel { Text = snookerBreak.Points.ToString(), FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };
//                    Label labelBalls = new BybLabel { Text = snookerBreak.NumberOfBallsDisplay, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center };

//                    // balls
//                    StackLayout stackForBalls = new StackLayout()
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        Spacing = 1,
//                        Padding = new Thickness(0)
//                    };
//                    ScrollView scrollViewForBalls = new ScrollView()
//                    {
//                        Orientation = ScrollOrientation.Horizontal,
//                        Padding = new Thickness(0),
//                        Content = stackForBalls
//                    };
//                    if (true)
//                    {
//                        List<int> balls = snookerBreak.GetBallsEvenWhenUnknown();
//                        for (int iBall = 0; iBall < balls.Count; ++iBall)
//                        {
//                            int ball = balls[iBall];
//                            Color color = Config.BallColors[ball];
//                            var btn = new BybButton
//                            {
//                                Text = "",
//                                BackgroundColor = color,
//                                BorderColor = Color.Transparent,
//                                TextColor = Color.White,
//                                BorderWidth = 1,
//                                BorderRadius = (int)(Config.ExtraSmallBallSize / 2),
//                                HeightRequest = Config.ExtraSmallBallSize,
//                                MinimumHeightRequest = Config.ExtraSmallBallSize,
//                                WidthRequest = Config.ExtraSmallBallSize,
//                                MinimumWidthRequest = Config.ExtraSmallBallSize,
//                                VerticalOptions = LayoutOptions.Center
//                            };
//                            if (snookerBreak.NumberOfBalls == 0)
//                                btn.Opacity = (balls.Count - iBall) / ((double)balls.Count + 1);
//                            stackForBalls.Children.Add(btn);
//                        }
//                    }

//                    // names
//                    FormattedString formattedString = new FormattedString();
//                    formattedString.Spans.Add(new Span() { Text = snookerBreak.AthleteName ?? "-" });
//                    formattedString.Spans.Add(new Span() { Text = "  vs. ", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//                    formattedString.Spans.Add(new Span() { Text = snookerBreak.OpponentName ?? "-", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//                    //formattedString.Spans.Add(new Span() { Text = DateTimeHelper.DateToString(snookerBreak.Date) });

//                    //if (snookerBreak.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
//                    //{
//                    //}
//                    //else if (snookerBreak.OpponentConfirmation == OpponentConfirmationEnum.Declined)
//                    //    formattedString.Spans.Add(new Span() { Text = "  (declined)", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//                    //else
//                    //    formattedString.Spans.Add(new Span() { Text = "  (unconfirmed)", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
//                    Label labelNames = new BybLabel()
//                    {
//                        HorizontalOptions = LayoutOptions.Start,
//                        FormattedText = formattedString,
//                        VerticalTextAlignment = TextAlignment.Center
//                    };
//                    Label labelDate = new BybLabel()
//                    {
//                        HorizontalOptions = LayoutOptions.Start,
//                        Text = DateTimeHelper.DateToString(snookerBreak.Date),
//                        TextColor = Config.ColorTextOnBackgroundGrayed,
//                    };

//                    // venue
//                    string venueName = snookerBreak.VenueName;
//                    if (string.IsNullOrEmpty(venueName))
//                        venueName = "-";
//                    Label labelVenueName = new BybLabel() { Text = venueName, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorGrayTextOnWhite };
//                    if (snookerBreak.VenueID > 0)
//                        labelVenueName.GestureRecognizers.Add(new TapGestureRecognizer()
//                        {
//                            Command = new Command(async () =>
//                            {
//                                await App.Navigator.GoToVenueProfile(snookerBreak.VenueID);
//                            }),
//                            NumberOfTapsRequired = 1
//                        });

//                    var panel1 = new StackLayout
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        BackgroundColor = Color.White,
//                        Padding = new Thickness(0, 10, 0, 5),
//                        Children =
//                        {
//                            new StackLayout
//                            {
//                                Padding = new Thickness(10,0,0,0),
//                                Orientation = StackOrientation.Vertical,
//                                HorizontalOptions = LayoutOptions.FillAndExpand,
//                                VerticalOptions = LayoutOptions.Start,
//                                Children = 
//                                {
//                                    labelNames,
//                                    scrollViewForBalls
//                                }
//                            },
//                            new StackLayout
//                            {
//                                Orientation = StackOrientation.Vertical,
//                                Padding = new Thickness(0, 0, 0, 0),
//                                WidthRequest = Config.IsTablet ? 80 : 40,
//                                MinimumWidthRequest = Config.IsTablet ? 80 : 40,
//                                HorizontalOptions = LayoutOptions.Center,
//                                VerticalOptions = LayoutOptions.Start,
//                                Spacing = 2,
//                                Children = 
//                                {
//                                    new BybLabel { Text = "Balls", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
//                                    labelBalls
//                                }
//                            },
//                            new StackLayout
//                            {
//                                Orientation = StackOrientation.Vertical,
//                                Padding = new Thickness(0, 0, 10, 0),
//                                WidthRequest = Config.IsTablet ? 80 : 40,
//                                MinimumWidthRequest = Config.IsTablet ? 80 : 40,
//                                HorizontalOptions = LayoutOptions.Center,
//                                VerticalOptions = LayoutOptions.Start,
//                                Spacing = 2,
//                                Children = 
//                                {
//                                    new BybLabel { Text = "Points", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
//                                    labelPoints
//                                }
//                            }
//                        }
//                    };

//                    //if (this.IsMyAthlete)
//                    //{
//                    //    var recognizer = new TapGestureRecognizer
//                    //    {
//                    //        Command = new Command(() => { this.showMenu(snookerBreak); }),
//                    //        NumberOfTapsRequired = 1
//                    //    };
//                    //    panel1.GestureRecognizers.Add(recognizer);
//                    //}
                    

//                    this.Children.Add(new StackLayout()
//                    {
//                        Orientation = StackOrientation.Vertical,
//                        Spacing = 1,
//                        Children =
//                        {
//                            panel1,
//                            new StackLayout
//                            {
//                                Orientation = StackOrientation.Horizontal,
//                                BackgroundColor = Color.White,
//                                Padding = new Thickness(5,0,5,0),
//                                Children =
//                                {
//                                    new Frame
//                                    {
//                                        Padding = new Thickness(5,5,0,5),
//                                        HorizontalOptions = LayoutOptions.StartAndExpand,
//                                        Content = labelDate,
//                                    },
//                                    new Frame
//                                    {
//                                        Padding = new Thickness(0,5,0,5),
//                                        HorizontalOptions = LayoutOptions.EndAndExpand,
//                                        Content = labelVenueName,
//                                    },
//                                }
//                            }
//                        }
//                    });
//                }
//                DateTime timeAfterForeach = DateTime.Now;

//                // remove previous items now, to avoid unnecessary scrolling
//                foreach (var item in itemsToRemove)
//                    this.Children.Remove(item);

//                TraceHelper.TraceTimeSpan(timeBegin, timeBeforeForeach, timeAfterForeach, DateTime.Now, "ListOfSnookerBreaksControl.Fill");
//            }
//            catch (Exception exc)
//            {
//                this.Children.Add(new BybLabel() { Text = "Error: " + TraceHelper.ExceptionToString(exc) });
//            }
//            finally
//            {
//            }
//		}
//	}
//}
