using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class NotificationsPage : ContentPage
    {
        readonly Color colorGreen = Color.FromRgb(58, 181, 74);
        readonly Color colorRed = Color.FromRgb(241, 89, 41);
        readonly Color colorGray = Color.Gray;

        const int sizeOfBall = 12;

        NotificationsData data;
        bool needToSync;

        StackLayout panel1;
        StackLayout panel2;
        //StackLayout panel3;

        BybButtonWithNumber button1;
        BybButtonWithNumber button2;
        BybButtonWithNumber button3;

        StackLayout panelFriendRequestsToMe;
        StackLayout panelFriendRequestsByMe;
        StackLayout panelMyResultsToAccept;
        StackLayout panelResultsToConfirm;
        StackLayout panelScoresToConfirm;

        public NotificationsPage(NotificationsData data)
        {
            this.data = data;

//            if (Config.IsTablet)
//                this.Padding = new Thickness(50, 20, 50, 50);
//            else
                this.Padding = new Thickness(0);
            this.BackgroundColor = Config.ColorGrayBackground;

            this.panelFriendRequestsToMe = new StackLayout()
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0, 10, 0, 10),
                Spacing = 5,
            };
            this.panelFriendRequestsByMe = new StackLayout()
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0, 10, 0, 10),
                Spacing = 5,
            };
            this.panelMyResultsToAccept = new StackLayout()
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0, 10, 0, 10),
                Spacing = 5,
            };
            this.panelResultsToConfirm = new StackLayout()
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0, 10, 0, 10),
                Spacing = 5,
            };
            this.panelScoresToConfirm = new StackLayout()
            {
                BackgroundColor = Config.ColorGrayBackground,
                Padding = new Thickness(0, 10, 0, 10),
                Spacing = 5,
            };

            this.button1 = new BybButtonWithNumber("Friend requests") { IsNumberVisible = true };
            this.button2 = new BybButtonWithNumber("Confirmations") { IsNumberVisible = true };
            this.button3 = new BybButtonWithNumber("Game invitations") { IsNumberVisible = true };
            this.button1.Clicked += button1_Clicked;
            this.button2.Clicked += button2_Clicked;

            this.panel1 = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10, 15, 10, 0),
                Spacing = 0,
                BackgroundColor = Config.ColorGrayBackground,
                Children = 
                {
                    new BybLabel { Text = "Sent to you", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                    panelFriendRequestsToMe,
                    new BoxView() { HeightRequest = 5 },

                    new BybLabel { Text = "Sent by you", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                    panelFriendRequestsByMe,
                }
            };

            this.panel2 = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(10, 15, 10, 0),
                Spacing = 0,
                BackgroundColor = Config.ColorGrayBackground,
                Children = 
                {
                    new BybLabel { Text = "Your breaks recorded by others", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                    panelMyResultsToAccept,
                    new BoxView() { HeightRequest = 5 },

                    new BybLabel { Text = "Breaks awaiting your confirmation", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                    panelResultsToConfirm,
                    new BoxView() { HeightRequest = 5 },

                    new BybLabel { Text = "Matches awaiting your confirmation", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                    panelScoresToConfirm,
                }
            };

            this.Content = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0,0,0,0),
                Spacing = 0,
                Children =
                {
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        BackgroundColor = Config.ColorGrayBackground,
                        Padding = new Thickness(0, 0, 0, 0),
                        Spacing = 1,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            button1,
                            button2,
                            //button3
                        }
                    },
                    new ScrollView
                    {
                        Padding = new Thickness(0,0,0,20),
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Padding = new Thickness(0),
                            Spacing = 0,
                            Children = 
                            {
                                this.panel1,
                                this.panel2,
                                //this.panel3
                            }
                        }
                    }
                }
            };
            this.Title = "Alerts";

            this.button1_Clicked(this, EventArgs.Empty);
        }

        private void button1_Clicked(object sender, EventArgs e)
        {
            this.button1.IsSelected = true;
            this.button2.IsSelected = false;
            this.panel1.IsVisible = true;
            this.panel2.IsVisible = false;
        }

        private void button2_Clicked(object sender, EventArgs e)
        {
            this.button1.IsSelected = false;
            this.button2.IsSelected = true;
            this.panel1.IsVisible = false;
            this.panel2.IsVisible = true;
        }

        private void button3_Clicked(object sender, EventArgs e)
        {
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            fill();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (needToSync)
                App.Navigator.StartSyncAndCheckForNotifications();
        }

        async void acceptOrDeclineFriendRequest(bool accept, FriendRequestWebModel request, Label labelStatus, Button buttonAccept, Button buttonDecline)
        {
            labelStatus.Text = "Contacting server...";
            buttonAccept.IsVisible = false;
            buttonDecline.IsVisible = false;

            bool ok;
            if (accept)
                ok = await App.WebService.AcceptFriendRequest(request.FriendshipID);
            else
                ok = await App.WebService.DeclineFriendRequest(request.FriendshipID);

            if (ok == false)
            {
                labelStatus.Text = "Couldn't contact the server. Internet issues?";
            }
            else
            {
                data.FriendRequestsToMe.Remove(request);
                labelStatus.Text = accept ? "Accepted as a friend." : "Denied friendship";
            }
        }

        async void withdrawFriendRequest(FriendRequestWebModel request, Label labelStatus, Button buttonWithdraw)
        {
            labelStatus.Text = "Contacting server...";
            buttonWithdraw.IsVisible = false;

            bool ok = await App.WebService.WithdrawFriendRequest(request.FriendshipID);

            if (ok == false)
            {
                labelStatus.Text = "Couldn't contact the server. Internet issues?";
            }
            else
            {
                this.data.FriendRequestsByMe.Remove(request);
                labelStatus.Text = "Friend request withdrawn.";
            }
        }

        async void acceptOrDeclineMyResult(bool accept, ResultWebModel result, Label labelStatus, Button buttonAccept, Button buttonDecline)
        {
            labelStatus.Text = "Contacting server...";
            buttonAccept.IsVisible = false;
            buttonDecline.IsVisible = false;

            bool ok = await App.WebService.AcceptResultNotYetAcceptedByMe(result.ResultID, accept);

            if (ok != true)
            {
                labelStatus.Text = "Couldn't contact the server. Internet issues?";
            }
            else
            {
                data.ResultsToConfirm.Remove(result);
                labelStatus.Text = accept ? "Done." : "Declined.";
            }

            this.needToSync = true;
        }

        async void confirmOrDeclineResult(bool confirm, ResultWebModel result, Label labelStatus, Button buttonConfirm, Button buttonDecline)
        {
            labelStatus.Text = "Contacting server...";
            buttonConfirm.IsVisible = false;
            buttonDecline.IsVisible = false;

            bool ok = await App.WebService.ConfirmResult(result.ResultID, confirm);

            if (ok == false)
            {
                labelStatus.Text = "Couldn't contact the server. Internet issues?";
            }
            else
            {
                data.ResultsToConfirm.Remove(result);
                labelStatus.Text = confirm ? "Confirmed." : "Declined.";
            }

            this.needToSync = true;
        }

        async void confirmOrDeclineScore(bool confirm, Score score, Label labelStatus, Button buttonConfirm, Button buttonDecline)
        {
            labelStatus.Text = "Contacting server...";
            buttonConfirm.IsVisible = false;
            buttonDecline.IsVisible = false;

            bool ok = await App.WebService.ConfirmScore(score.ScoreID, confirm);

            if (ok == false)
            {
                labelStatus.Text = "Couldn't contact the server. Internet issues?";
            }
            else
            {
                data.ScoresToConfirm.Remove(score);
                labelStatus.Text = confirm ? "Confirmed." : "Declined.";
            }

            this.needToSync = true;
        }

        void fill()
        {
            if (data.FriendRequestsByMe == null && data.FriendRequestsToMe == null && data.ResultsToConfirm == null)
            {
                this.button1.Number = null;
                this.button2.Number = null;

				this.panelFriendRequestsByMe.Children.Add(new BybLabel { Text = "no internet connection", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
				this.panelFriendRequestsToMe.Children.Add(new BybLabel { Text = "no internet connection", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
				this.panelMyResultsToAccept.Children.Add(new BybLabel { Text = "no internet connection", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
				this.panelResultsToConfirm.Children.Add(new BybLabel { Text = "no internet connection", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
				this.panelScoresToConfirm.Children.Add(new BybLabel { Text = "no internet connection", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
                return;
            }

            this.button1.Number = data.FriendRequestsToMe.Count;
            this.button2.Number = data.MyResultsToAccept.Count + data.ResultsToConfirm.Count + data.ScoresToConfirm.Count;

            this.panelFriendRequestsByMe.Children.Clear();
            if (data.FriendRequestsByMe == null)
            {
                // keep empty
            }
            else if (data.FriendRequestsByMe.Count == 0)
            {
				panelFriendRequestsByMe.Children.Add(new BybLabel { Text = "None", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
            }
            else
            {
                this.fillFriendRequestsByMe(data.FriendRequestsByMe);
            }

            this.panelFriendRequestsToMe.Children.Clear();
            if (data.FriendRequestsToMe == null)
            {
                // keep empty
            }
            else if (data.FriendRequestsToMe.Count == 0)
            {
				panelFriendRequestsToMe.Children.Add(new BybLabel { Text = "None", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
            }
            else
            {
                this.fillFriendRequests(data.FriendRequestsToMe);
            }

            this.panelMyResultsToAccept.Children.Clear();
            if (data.MyResultsToAccept == null)
            {
                // keep empty
            }
            else if (data.MyResultsToAccept.Count == 0)
            {
				this.panelMyResultsToAccept.Children.Add(new BybLabel { Text = "None", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
            }
            else
            {
                this.fillMyResultsToAccept(data.MyResultsToAccept);
            }

            this.panelResultsToConfirm.Children.Clear();
            if (data.ResultsToConfirm == null)
            {
                // keep empty
            }
            else if (data.ResultsToConfirm.Count == 0)
            {
				panelResultsToConfirm.Children.Add(new BybLabel { Text = "None", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
            }
            else
            {
                this.fillResultsToConfirm(data.ResultsToConfirm);
            }

            this.panelScoresToConfirm.Children.Clear();
            if (data.ScoresToConfirm == null)
            {
                // keep empty
            }
            else if (data.ScoresToConfirm.Count == 0)
            {
				panelScoresToConfirm.Children.Add(new BybLabel { Text = "None", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorBlackTextOnWhite, });
            }
            else
            {
                this.fillScoresToConfirm(data.ScoresToConfirm);
            }
        }

        async void fillFriendRequests(List<FriendRequestWebModel> friendRequests)
        {
            List<int> peopleIDs = (from r in friendRequests
                                   select r.AthleteID).Distinct().ToList();
            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);

            foreach (var request in friendRequests)
            {
                var person = App.Cache.People.Get(request.AthleteID);
                string personPicture = person != null ? person.Picture : "";
                string personName = person != null ? person.Name : "- internet issues -";
                if (string.IsNullOrEmpty(personName))
                    personName = "- name not shared -";

				Label labelStatus = new BybLabel() { Text = "", HeightRequest = 30, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, };
                Button buttonAccept = new BybButton() { Text = "Accept as a friend", Style = (Style)App.Current.Resources["ConfirmButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
                Button buttonDecline = new BybButton() { Text = "Decline", Style = (Style)App.Current.Resources["DenyButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };

                var stackPerson = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(0, 0, 0, 0),
                    Spacing = 1,
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0,5,0,0),
                            Spacing = 0,
                            Children =
                            {
                                new Frame
                                {
                                    Padding = new Thickness(10,3,0,3),
                                    Content = new Image() { Source = App.ImagesService.GetImageSource(personPicture), WidthRequest = Config.PersonImageSize, HeightRequest = Config.PersonImageSize }
                                },
                                new StackLayout
                                {
                                    Padding = new Thickness(10,0,0,0),
                                    Orientation = StackOrientation.Vertical,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.Center,
                                    Children = 
                                    {
										new BybLabel { Text = personName, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.None, TextColor = Config.ColorBlackTextOnWhite, },
                                        new BybLabel { Text = person.HasMetro ? person.Metro : "Unknown location", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                                    }
                                },
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Padding = new Thickness(10,0,0,0),
                            Spacing = 10,
                            BackgroundColor = Color.White,
                            Children =
                            {
                                buttonDecline,
                                buttonAccept,
                                labelStatus,
                            }
                        },
                    }
                };

                this.panelFriendRequestsToMe.Children.Add(stackPerson);

                buttonAccept.Clicked += (s, e) => { this.acceptOrDeclineFriendRequest(true, request, labelStatus, buttonAccept, buttonDecline); };
                buttonDecline.Clicked += (s, e) => { this.acceptOrDeclineFriendRequest(false, request, labelStatus, buttonAccept, buttonDecline); };

                stackPerson.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () => { await App.Navigator.GoToPersonProfile(request.AthleteID); }),
                    NumberOfTapsRequired = 1
                });
            }
        }

        async void fillFriendRequestsByMe(List<FriendRequestWebModel> friendRequests)
        {
            List<int> peopleIDs = (from r in friendRequests
                                   select r.AthleteID).Distinct().ToList();
            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);

            foreach (var request in friendRequests)
            {
                var person = App.Cache.People.Get(request.AthleteID);
                string personPicture = person != null ? person.Picture : "";
                string personName = person != null ? person.Name : "- internet issues -";
                if (string.IsNullOrEmpty(personName))
                    personName = "- name not shared -";

				Label labelStatus = new BybLabel() { Text = "", HeightRequest = 30, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, };
                Button buttonWithdraw = new BybButton() { Text = "Withdraw", Style = (Style)App.Current.Resources["DenyButtonStyle"], WidthRequest = 100, HorizontalOptions = LayoutOptions.Start };

                var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(10, 0, 10, 0),
                    Spacing = 0,
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0,5,0,0),
                            Spacing = 0,
                            Children =
                            {
                                new Frame
                                {
                                    Padding = new Thickness(10,3,0,3),
                                    Content = new Image() { Source = App.ImagesService.GetImageSource(personPicture), WidthRequest = Config.PersonImageSize, HeightRequest = Config.PersonImageSize }
                                },
                                new StackLayout
                                {
                                    Padding = new Thickness(10,0,0,0),
                                    Orientation = StackOrientation.Vertical,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.Center,
                                    Children = 
                                    {
										new BybLabel { Text = personName, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.None, TextColor = Config.ColorBlackTextOnWhite, },
                                        //buttonName,
                                        new BybLabel { Text = person.HasMetro ? person.Metro : "Unknown location", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
                                        new StackLayout
                                        {
                                            Orientation = StackOrientation.Horizontal,
                                            Padding = new Thickness(10,0,30,5),
                                            Spacing = 10,
                                            Children =
                                            {
                                                buttonWithdraw,
                                                labelStatus,
                                            }
                                        }
                                    }
                                },
                            }
                        },
                    }
                };

                this.panelFriendRequestsByMe.Children.Add(panel);

                buttonWithdraw.Clicked += (s, e) => { this.withdrawFriendRequest(request, labelStatus, buttonWithdraw); };

                panel.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () => { await App.Navigator.GoToPersonProfile(request.AthleteID); }),
                    NumberOfTapsRequired = 1
                });
            }
        }

        async void fillMyResultsToAccept(List<ResultWebModel> myResultsToAccept)
        {
            var me = App.Repository.GetMyAthlete();

            List<int> peopleIDs = (from r in myResultsToAccept
                                   where r.OpponentAthleteID != null && r.OpponentAthleteID.Value > 0
                                   select r.OpponentAthleteID.Value).Distinct().ToList();
            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);

            foreach (var result in myResultsToAccept)
            {
                SnookerBreak snookerBreak = SnookerBreak.FromResult(result.ToResult());

                PersonBasicWebModel opponent = null;
                if (snookerBreak.OpponentAthleteID != 0)
                    opponent = App.Cache.People.Get(snookerBreak.OpponentAthleteID);
                string opponentName = opponent != null ? opponent.Name : "- internet issues -";
                //string personPicture = person != null ? person.Picture : "";

                // balls
                StackLayout stackForBalls = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 1,
                    Padding = new Thickness(0)
                };
                ScrollView scrollViewForBalls = new ScrollView()
                {
                    Orientation = ScrollOrientation.Horizontal,
                    Padding = new Thickness(0),
                    Content = stackForBalls
                };
                if (true)
                {
                    List<int> balls = snookerBreak.GetBallsEvenWhenUnknown();
                    for (int iBall = 0; iBall < balls.Count; ++iBall)
                    {
                        int ball = balls[iBall];
                        Color color = Config.BallColors[ball];
                        var btn = new BybButton
                        {
                            Text = "",
                            BackgroundColor = color,
                            BorderColor = Color.Transparent,// Color.Black,
                            TextColor = Color.White,
                            BorderWidth = 1,
                            BorderRadius = (int)(sizeOfBall / 2),
                            HeightRequest = sizeOfBall,
                            MinimumHeightRequest = sizeOfBall,
                            WidthRequest = sizeOfBall,
                            MinimumWidthRequest = sizeOfBall,
                            VerticalOptions = LayoutOptions.Center
                        };
                        if (snookerBreak.NumberOfBalls == 0)
                            btn.Opacity = (balls.Count - iBall) / ((double)balls.Count + 1);
                        stackForBalls.Children.Add(btn);
                    }
                }

                // buttons
				Label labelStatus = new BybLabel() { Text = "", HeightRequest = 30, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, };
                Button buttonAccept = new BybButton() { Text = "Yes, I made this break", Style = (Style)App.Current.Resources["ConfirmButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
                Button buttonDecline = new BybButton() { Text = "Nope", Style = (Style)App.Current.Resources["DenyButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };

                var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 1,
                    Padding = new Thickness(0, 0, 0, 0),
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0,5,0,5),
                            Children =
                            {
                                new Frame
                                {
                                    Padding = new Thickness(3,3,0,3),
                                    Content = new Image() { Source = App.ImagesService.GetImageSource(me.Picture), WidthRequest = Config.PersonImageSize, HeightRequest = Config.PersonImageSize }
                                },
                                new StackLayout
                                {
                                    Padding = new Thickness(10,0,0,0),
                                    Orientation = StackOrientation.Vertical,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.Start,
                                    Children =
                                    {
										new BybLabel() { Text = "vs. " + opponentName, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, },
                                        new BybLabel()
                                        {
                                            Text = DateTimeHelper.DateToString(snookerBreak.Date),//snookerBreak.Date.ToShortDateString(),
                                            VerticalTextAlignment = TextAlignment.Center,
                                            TextColor = Config.ColorGrayTextOnWhite
                                        },
                                        scrollViewForBalls
                                    }
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Vertical,
                                    Padding = new Thickness(0, 0, 0, 0),
                                    WidthRequest = Config.IsTablet ? 80 : 40,
                                    MinimumWidthRequest = Config.IsTablet ? 80 : 40,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Start,
                                    Spacing = 2,
                                    Children =
                                    {
										new BybLabel { Text = "Balls", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center, },
										new BybLabel { Text = snookerBreak.NumberOfBallsDisplay, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, }
                                    }
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Vertical,
                                    Padding = new Thickness(0, 0, 10, 0),
                                    WidthRequest = Config.IsTablet ? 80 : 40,
                                    MinimumWidthRequest = Config.IsTablet ? 80 : 40,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Start,
                                    Spacing = 2,
                                    Children =
                                    {
										new BybLabel { Text = "Points", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center, },
										new BybLabel { Text = snookerBreak.Points.ToString(), FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, }
                                    }
                                }
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            //Padding = new Thickness(65,0,Config.IsTablet ? 200 : 60,5),
                            Padding = new Thickness(10,0,0,0),
                            Spacing = 10,
                            Children =
                            {
                                buttonDecline,
                                buttonAccept,
                                labelStatus
                            }
                        }
                    }
                };
                this.panelMyResultsToAccept.Children.Add(panel);

                panel.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        //if (opponent != null)
                        //App.Navigator.GoToPersonProfile(opponent.ID);
                    }),
                    NumberOfTapsRequired = 1
                });
                buttonAccept.Clicked += (s, e) => { this.acceptOrDeclineMyResult(true, result, labelStatus, buttonAccept, buttonDecline); };
                buttonDecline.Clicked += (s, e) => { this.acceptOrDeclineMyResult(false, result, labelStatus, buttonAccept, buttonDecline); };
            }
        }

        async void fillResultsToConfirm(List<ResultWebModel> resultsToConfirm)
        {
            List<int> peopleIDs = (from r in resultsToConfirm
                                   select r.AthleteID).Distinct().ToList();
            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);

            foreach (var result in resultsToConfirm)
            {
                SnookerBreak snookerBreak = SnookerBreak.FromResult(result.ToResult());

                PersonBasicWebModel person = App.Cache.People.Get(result.AthleteID);
                string personName = person != null ? person.Name : "- internet issues -";
                string personPicture = person != null ? person.Picture : "";

                // balls
                StackLayout stackForBalls = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 1,
                    Padding = new Thickness(0)
                };
                ScrollView scrollViewForBalls = new ScrollView()
                {
                    Orientation = ScrollOrientation.Horizontal,
                    Padding = new Thickness(0),
                    Content = stackForBalls
                };
                if (snookerBreak.HasBalls)
                {
                    foreach (var ball in snookerBreak.Balls)
                    {
                        Color color = Config.BallColors[ball];
                        var btn = new BybButton
                        {
                            Text = "",
                            BackgroundColor = color,
                            BorderColor = Color.Transparent,// Color.Black,
                            TextColor = Color.White,
                            BorderWidth = 1,
                            BorderRadius = (int)(sizeOfBall / 2),
                            HeightRequest = sizeOfBall,
                            MinimumHeightRequest = sizeOfBall,
                            WidthRequest = sizeOfBall,
                            MinimumWidthRequest = sizeOfBall,
                            VerticalOptions = LayoutOptions.Center
                        };
                        stackForBalls.Children.Add(btn);
                    }
                }

                // buttons
				Label labelStatus = new BybLabel() { Text = "", HeightRequest = 30, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, };
                Button buttonConfirm = new BybButton() { Text = "Confirm", Style = (Style)App.Current.Resources["ConfirmButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
                Button buttonDecline = new BybButton() { Text = "Decline", Style = (Style)App.Current.Resources["DenyButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };

                var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 1,
                    Padding = new Thickness(0, 0, 0, 0),
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0,5,0,5),
                            Children =
                            {
                                new Frame
                                {
                                    Padding = new Thickness(3,3,0,3),
                                    Content = new Image() { Source = App.ImagesService.GetImageSource(personPicture), WidthRequest = Config.PersonImageSize, HeightRequest = Config.PersonImageSize }
                                },
                                new StackLayout
                                {
                                    Padding = new Thickness(10,0,0,0),
                                    Orientation = StackOrientation.Vertical,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.Start,
                                    Children = 
                                    {
										new BybLabel() { Text = personName, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, },
                                        new BybLabel()
                                        {
                                            Text = DateTimeHelper.DateToString(snookerBreak.Date),//snookerBreak.Date.ToShortDateString(),
                                            VerticalTextAlignment = TextAlignment.Center,
                                            TextColor = Config.ColorGrayTextOnWhite
                                        },
                                        scrollViewForBalls
                                    }
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Vertical,
                                    Padding = new Thickness(0, 0, 0, 0),
                                    WidthRequest = Config.IsTablet ? 80 : 40,
                                    MinimumWidthRequest = Config.IsTablet ? 80 : 40,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Start,
                                    Spacing = 2,
                                    Children = 
                                    {
										new BybLabel { Text = "Balls", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center, },
										new BybLabel { Text = snookerBreak.NumberOfBalls.ToString(), FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, }
                                    }
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Vertical,
                                    Padding = new Thickness(0, 0, 10, 0),
                                    WidthRequest = Config.IsTablet ? 80 : 40,
                                    MinimumWidthRequest = Config.IsTablet ? 80 : 40,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Start,
                                    Spacing = 2,
                                    Children = 
                                    {
										new BybLabel { Text = "Points", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center, },
										new BybLabel { Text = snookerBreak.Points.ToString(), FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, }
                                    }
                                }
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(10,0,0,0),
                            Spacing = 10,
                            Children =
                            {
                                buttonDecline,
                                buttonConfirm,
                                labelStatus
                            }
                        }
                    }
                };
                this.panelResultsToConfirm.Children.Add(panel);

                panel.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () => { await App.Navigator.GoToPersonProfile(person.ID); }),
                    NumberOfTapsRequired = 1
                });
                buttonConfirm.Clicked += (s, e) => { this.confirmOrDeclineResult(true, result, labelStatus, buttonConfirm, buttonDecline); };
                buttonDecline.Clicked += (s, e) => { this.confirmOrDeclineResult(false, result, labelStatus, buttonConfirm, buttonDecline); };
            }
        }

        async void fillScoresToConfirm(List<Score> scoresToConfirm)
        {
            int myAthleteID = App.Repository.GetMyAthleteID();

            List<int> peopleIDs = (from r in scoresToConfirm
                                   select r.AthleteAID).Distinct().ToList();
            await App.Cache.LoadFromWebserviceIfNecessary_People(peopleIDs);

            foreach (var score in scoresToConfirm)
            {
                SnookerMatchScore match = SnookerMatchScore.FromScore(myAthleteID, score);

                PersonBasicWebModel person = App.Cache.People.Get(score.AthleteAID);
                string personName = person != null ? person.Name : "- internet issues -";
                string personPicture = person != null ? person.Picture : "";

                // frames
                StackLayout stackForFrames = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 8,
                    Padding = new Thickness(0, 0, 0, 0)
                };
                ScrollView scrollViewForFrames = new ScrollView()
                {
                    Orientation = ScrollOrientation.Horizontal,
                    Padding = new Thickness(0),
                    Content = stackForFrames
                };
                if (match.HasFrameScores)
                {
                    foreach (var frame in match.FrameScores)
                    {
                        Color color = colorGray;
                        if (frame.B < frame.A)
                            color = colorGreen;
                        else if (frame.B > frame.A)
                            color = colorRed;

                        Label label = new BybLabel()
                        {
                            Text = frame.A + ":" + frame.B,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = color,
                            VerticalOptions = LayoutOptions.Center
                        };
                        stackForFrames.Children.Add(label);
                    }
                }

                // title
                string title = "Draw";
                if (match.MatchScoreA > match.MatchScoreB)
                    title = "You Won";
                else if (match.MatchScoreA < match.MatchScoreB)
                    title = "You Lost";

                // buttons
				Label labelStatus = new BybLabel() { Text = "", HeightRequest = 30, VerticalTextAlignment = TextAlignment.Center, TextColor = Config.ColorBlackTextOnWhite, };
                Button buttonConfirm = new BybButton() { Text = "Confirm", Style = (Style)App.Current.Resources["ConfirmButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
                Button buttonDecline = new BybButton() { Text = "Decline", Style = (Style)App.Current.Resources["DenyButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };

                var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 1,
                    Padding = new Thickness(0,0,0,0),
                    Children =
                    {
                        new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0, 0, 0, 0),
                            Children =
                            {
                                new Frame
                                {
                                    Padding = new Thickness(3,3,0,3),
                                    Content = new Image() { Source = App.ImagesService.GetImageSource(personPicture), WidthRequest = Config.PersonImageSize, HeightRequest = Config.PersonImageSize }
                                },
                                new StackLayout
                                {
                                    Padding = new Thickness(10,10,0,5),
                                    Orientation = StackOrientation.Vertical,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    VerticalOptions = LayoutOptions.Start,
                                    Children = 
                                    {
										new BybLabel() { Text = personName, VerticalTextAlignment = TextAlignment.Center, FontAttributes = FontAttributes.None, TextColor = Config.ColorBlackTextOnWhite, },
                                        new BybLabel()
                                        {
                                            Text = DateTimeHelper.DateToString(match.Date),//match.Date.ToShortDateString(),
                                            VerticalTextAlignment = TextAlignment.Center,
                                            TextColor = Config.ColorGrayTextOnWhite
                                        },
                                        scrollViewForFrames,
                                    }
                                },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Vertical,
                                    Padding = new Thickness(0, 8, 0, 0),
                                    WidthRequest = Config.IsTablet ? 120 : 80,
                                    MinimumWidthRequest = Config.IsTablet ? 100 : 80,
                                    HeightRequest = 45,
                                    HorizontalOptions = LayoutOptions.Center,
                                    VerticalOptions = LayoutOptions.Start,
                                    BackgroundColor = match.MatchScoreB < match.MatchScoreA ? colorGreen : (match.MatchScoreB > match.MatchScoreA ? colorRed : colorGray),
                                    Spacing = 2,
                                    Children = 
                                    {
                                        new BybLabel { Text = title, TextColor = Color.White, HorizontalOptions = LayoutOptions.Center },
                                        new BybLabel
                                        {
                                            Text = match.MatchScoreA.ToString() + " : " + match.MatchScoreB.ToString(),
                                            FontSize = Config.LargerFontSize,
                                            FontAttributes = Xamarin.Forms.FontAttributes.Bold,
                                            TextColor = Color.White,
                                            HorizontalOptions = LayoutOptions.Center
                                        },
                                    }
                                },
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            BackgroundColor = Color.White,
                            Padding = new Thickness(10,0,0,0),
                            Spacing = 10,
                            Children =
                            {
                                buttonDecline,
                                buttonConfirm,
                                labelStatus
                            }
                        }
                    }
                };
                this.panelScoresToConfirm.Children.Add(panel);

                // this produces an exception. fix it some time...
      //          panel.GestureRecognizers.Add(new TapGestureRecognizer()
      //          {
      //              Command = new Command(async () =>
      //              {
	  //			      RecordMatchPage page = new RecordMatchPage(match, true);
      //                  await App.Navigator.NavPage.Navigation.PushModalAsync(page);
      //              }),
      //              NumberOfTapsRequired = 1
      //          });
                buttonConfirm.Clicked += (s, e) => { this.confirmOrDeclineScore(true, score, labelStatus, buttonConfirm, buttonDecline); };
                buttonDecline.Clicked += (s, e) => { this.confirmOrDeclineScore(false, score, labelStatus, buttonConfirm, buttonDecline); };
            }
        }
    }
}
