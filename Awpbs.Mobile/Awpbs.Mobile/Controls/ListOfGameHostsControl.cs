using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfGameHostsControl : ListOfItemsControl<GameHostWebModel>
	{
        public event EventHandler UserChangedSomething;

        public bool IsForPast { get; set; }
        public bool ShowCommentsCount { get; set; }
        public bool TreatAsASingleItem { get; set; }
        public bool IgnoreSingleItemTaps { get; set; }

        //public string TextToShowWhenEmpty { get; set; }

		int myAthleteID;
		List<KeyValuePair<Label,int>> labelsToUpdate;

        public ListOfGameHostsControl()
        {
			base.MaxCountToShowByDefault = 5;
        }

		public override void Fill (List<GameHostWebModel> list)
		{
			this.myAthleteID = App.Repository.GetMyAthleteID();
			this.labelsToUpdate = new List<KeyValuePair<Label, int>> ();

			base.Fill (list);

			// build a list of IDs of people involved
			List<int> ids = new List<int>();
			if (list != null)
				foreach (var gameHost in list)
				{
					if (gameHost.AthleteIDs_CannotGo != null)
						ids.AddRange(gameHost.AthleteIDs_CannotGo);
					if (gameHost.AthleteIDs_Going != null)
						ids.AddRange(gameHost.AthleteIDs_Going);
					if (gameHost.AthleteIDs_Invited != null)
						ids.AddRange(gameHost.AthleteIDs_Invited);
					if (gameHost.AthleteIDs_WantToGo != null)
						ids.AddRange(gameHost.AthleteIDs_WantToGo);
				}

			// build a list of people that are not in the cache yet, and load them into cache from the web service
			List<int> idsMissing = new List<int> ();
			foreach (int id in ids)
				if (App.Cache.People.Get (id) == null)
					idsMissing.Add (id);
			if (idsMissing.Count > 0)
			{
				new Task (async () =>
				{
					await App.Cache.LoadFromWebserviceIfNecessary_People (ids);

					Device.BeginInvokeOnMainThread(() =>
					{
						foreach (var item in this.labelsToUpdate)
						{
							Label label = item.Key;
							int personID = item.Value;

							var person = App.Cache.People.Get(personID);
							if (person != null && person.Name != null)
								label.Text = person.Name;
						}
						
						this.labelsToUpdate = new List<KeyValuePair<Label, int>>();
					});

				}).Start ();
			}
		}

        ScrollView createScrollViewWithNames(GameHostWebModel gameHost, List<int> ids, string text, bool allowButton, bool canBeInvisible)
        {
            StackLayout panel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Padding = new Thickness(10, 0, 5, 0),
				HeightRequest = 23 + (Config.IsAndroid ? 3 : 0) + (Config.IsTablet ? 4 : 0),
                //HeightRequest = Config.IsIOS ? 20 : 23,
            };

            int count = ids != null ? ids.Count : 0;

            string labelText = text + ": ";
            if (count == 0)
                labelText += " -";

            Label label = new BybLabel()
            {
                Text = labelText,
                TextColor = Config.ColorBlackTextOnWhite,
                VerticalTextAlignment = TextAlignment.Center,
            };
            panel.Children.Add(label);

            if (count > 0)
            {
                foreach (int id in ids)
                {
                    var person = App.Cache.People.Get(id);

					string personName = "unknown";
					if (person != null && string.IsNullOrEmpty (person.Name) == false)
						personName = person.Name;

					Label labelPerson = new BybLabel ()
					{
						Text = personName,
						FontAttributes = FontAttributes.Bold,
						VerticalOptions = LayoutOptions.Center,
						TextColor = Config.ColorBlackTextOnWhite,
					};
					labelPerson.GestureRecognizers.Add (new TapGestureRecognizer ()
					{
						Command = new Command(async () => { await App.Navigator.GoToPersonProfile(id); })
					});
					panel.Children.Add (labelPerson);
					if (person == null)
						this.labelsToUpdate.Add (new KeyValuePair<Label, int> (labelPerson, id));

					if (allowButton)
					{
						Label buttonAllow = new BybLabel () {
							Text = "accept this player",
							FontAttributes = FontAttributes.Bold,
							TextColor = Config.ColorGreen,
							VerticalOptions = LayoutOptions.Center,
						};
						buttonAllow.GestureRecognizers.Add (new TapGestureRecognizer ()
						{
							Command = new Command(() => { this.doOnUserWantsToAcceptPlayer(gameHost, id); })
						});
						panel.Children.Add (buttonAllow);
					}
                }
            }

            ScrollView scrollView = new ScrollView()
            {
                Padding = new Thickness(0),
                BackgroundColor = Color.White,
                Content = panel
            };
            if (canBeInvisible && count == 0)
                scrollView.IsVisible = false;

            return scrollView;
        }

		protected override View createViewForSingleItem (GameHostWebModel gameHost)
		{
			try
			{
                bool youAreHosting = gameHost.HostPersonID == myAthleteID;
                bool youAreInvited = gameHost.IsInvited(myAthleteID);

                // host person
                FormattedString formattedStringPerson = new FormattedString();
                formattedStringPerson.Spans.Add(new Span()
                {
                    Text = gameHost.HostPersonName,
                    ForegroundColor = Config.ColorBlackTextOnWhite,
                    FontAttributes = this.TreatAsASingleItem ? FontAttributes.None : FontAttributes.Bold,
                    FontFamily = Config.FontFamily,
                    FontSize = Config.DefaultFontSize
                });

                //formattedStringPerson.Spans.Add(new Span() { Text = " wants to play", ForegroundColor = Config.ColorGrayTextOnWhite, FontAttributes = FontAttributes.None, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize });
                Label labelPersonHost = new BybLabel()
                {
                    FormattedText = formattedStringPerson,
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = this.TreatAsASingleItem ? FontAttributes.None : FontAttributes.Bold,
                    FontFamily = Config.FontFamily,
					TextColor = Config.ColorBlackTextOnWhite,
                };

                // when
                FormattedString formattedStringWhen = new FormattedString();
                formattedStringWhen.Spans.Add(new Span()
                {
                    Text = "on ",
                    ForegroundColor = Config.ColorGrayTextOnWhite,
                });
                formattedStringWhen.Spans.Add(new Span()
                {
                    Text = DateTimeHelper.DateAndTimeToString(gameHost.When),
                    ForegroundColor = Config.ColorBlackTextOnWhite,
                    FontFamily = Config.FontFamily,
                    FontSize = Config.DefaultFontSize
                });

                // venue
                FormattedString formattedStringVenue = new FormattedString();
                formattedStringVenue.Spans.Add(new Span() { Text = "at ", ForegroundColor = Config.ColorGrayTextOnWhite, FontAttributes = FontAttributes.None, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize });
                formattedStringVenue.Spans.Add(new Span()
                {
                    Text = gameHost.VenueName,
                    ForegroundColor = Config.ColorBlackTextOnWhite,
                    FontAttributes = this.TreatAsASingleItem ? FontAttributes.None : FontAttributes.Bold,
                    FontFamily = Config.FontFamily,
                    FontSize = Config.DefaultFontSize
                });
                Label labelVenue = new BybLabel()
                {
                    FormattedText = formattedStringVenue,
                };

                // lists of people
                ScrollView panel1 = this.createScrollViewWithNames(gameHost, gameHost.AthleteIDs_Going, "Accepted", false, false);
                ScrollView panel2 = this.createScrollViewWithNames(gameHost, gameHost.GetAthleteIDs_Unresponded(), "Not responded", false, true);
                ScrollView panel3 = this.createScrollViewWithNames(gameHost, gameHost.AthleteIDs_CannotGo, "Declined", false, true);
                ScrollView panel4 = this.createScrollViewWithNames(gameHost, gameHost.AthleteIDs_WantToGo, "Wants to be invited", youAreHosting ? true : false, true);

                // accept button
                Button buttonAccept = new BybButton()
                {
                    Style = (Style)App.Current.Resources["ConfirmButtonStyle"],
                    HorizontalOptions = LayoutOptions.Start,
					WidthRequest = 130 + (Config.IsAndroid ? 30 : 0) + (Config.IsTablet ? 30 : 0),
                    HeightRequest = Config.IsAndroid ? 40 : 30,
                };
                if (this.IsForPast)
                {
                    buttonAccept.IsVisible = false;
                }
                else if (youAreHosting)
                {
                    buttonAccept.Text = "You are hosting";
                    buttonAccept.Style = (Style)App.Current.Resources["SimpleButtonStyle"];
                    buttonAccept.IsEnabled = false;
                }
                else if (gameHost.IsGoing(myAthleteID))
                {
                    buttonAccept.Text = "You are going!";
                    buttonAccept.IsEnabled = false;
                }
                else if (youAreInvited)
                {
                    buttonAccept.Text = "Accept invitation";
                }
                else if (gameHost.IsWantToGo(myAthleteID))
                {
                    buttonAccept.Text = "Already asked to be invited";
                    buttonAccept.Style = (Style)App.Current.Resources["SimpleButtonStyle"];
                    buttonAccept.IsEnabled = false;
                }
                else
                {
                    buttonAccept.Text = "Ask to be invited";
                }
				bool buttonAcceptIsDisabled = buttonAccept.IsEnabled == false;
				if (Config.IsAndroid && buttonAcceptIsDisabled)
				{
					// explanation: On Android disabled buttons become invisible on the white background... so make it enabled but make it look differently
					buttonAccept.IsEnabled = true;
					buttonAccept.TextColor = Color.Black;
					buttonAccept.Opacity = 0.5;
				}
                buttonAccept.Clicked += (s1, e1) =>
                {
					if (buttonAcceptIsDisabled)
						return;
                    if (youAreInvited)
                        this.doOnUserWantsToAcceptAnInvite(gameHost);
                    else if (youAreHosting == false)
                        this.doOnUserWantsToBeInvited(gameHost);
                };


                // decline button
                Button buttonDecline = new BybButton()
                {
                    Style = (Style)App.Current.Resources["DenyButtonStyle"],
                    HorizontalOptions = LayoutOptions.Start,
					WidthRequest = 130 + (Config.IsAndroid ? 30 : 0) + (Config.IsTablet ? 30 : 0),
					HeightRequest = Config.IsAndroid ? 40 : 30,
                    Text = "Decline invitation",//buttonText,
                    //IsEnabled = buttonText != ""
                };
                if (this.IsForPast)
                {
                    buttonDecline.IsVisible = false;
                }
                else if (youAreHosting)
                {
                    buttonDecline.Text = "";
                    buttonDecline.IsVisible = false;
                }
                else if (gameHost.IsCannotGo(myAthleteID))
                {
                    buttonDecline.Text = "You declined";
                    buttonDecline.IsEnabled = false;
                }
                else if (youAreInvited || gameHost.IsGoing(myAthleteID))
                {
                    buttonDecline.Text = "Decline invitation";
                }
                else
                {
                    buttonDecline.Text = "";
                    buttonDecline.IsVisible = false;
                }
				bool buttonDeclineIsDisabled = buttonDecline.IsEnabled == false;
				if (Config.IsAndroid && buttonDeclineIsDisabled)
				{
					// explanation: On Android disabled buttons become invisible on the white background... so make it enabled but make it look differently
					buttonDecline.IsEnabled = true;
					buttonDecline.TextColor = Color.Black;
					buttonDecline.Opacity = 0.5;
				}
                buttonDecline.Clicked += (s1, e1) =>
                {
					if (buttonDeclineIsDisabled)
						return;
                    this.doOnUserWantsToDecline(gameHost);
                };

                /// likes and comments
                /// 
                var imageLike = new Image()
                {
                    Source = new FileImageSource() { File = "like1.png" },
                    HeightRequest = Config.LikeImageSize,
                    WidthRequest = Config.LikeImageSize,
                };
                var imageComments = new Image()
                {
                    Source = new FileImageSource() { File = "right.png" },
                    HeightRequest = Config.CommentImageSize,
                    WidthRequest = Config.CommentImageSize,
                };
                var labelLikesCount = new BybLabel
                {
                };
                var panelLikes = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
					Padding = new Thickness(0, 0, 0, 5),
                    Spacing = 3,
                    Children =
                    {
                        labelLikesCount,
                        imageLike
                    }
				};
                var labelCommentsCount = new BybLabel
                {
                };
                var panelComments = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Center,
					Padding = new Thickness(0, 5, 0, 0),
                    Spacing = 5,
                    Children =
                    {
                        labelCommentsCount,
                        imageComments,
                    }
				};
                var panelCommentsCount = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 0,
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
						panelLikes,
						panelComments
                    }
                };
                imageLike.Source = new FileImageSource() { File = gameHost.Liked ? "like2.png" : "like1.png" };
                imageComments.Source = new FileImageSource() { File = gameHost.CommentsCount > 0 ? "right2.png" : "right1.png" };
                labelLikesCount.Text = gameHost.LikesCount.ToString();
                labelLikesCount.TextColor = gameHost.LikesCount > 0 ? Config.ColorBlackTextOnWhite : Config.ColorGrayTextOnWhite;
                labelCommentsCount.Text = gameHost.CommentsCount.ToString();
                labelCommentsCount.TextColor = gameHost.CommentsCount > 0 ? Config.ColorBlackTextOnWhite : Config.ColorGrayTextOnWhite;
				panelComments.IsVisible = ShowCommentsCount;
				panelLikes.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        if (gameHost.Liked)
                            gameHost.LikesCount -= 1;
                        else
                            gameHost.LikesCount += 1;
                        gameHost.Liked = !gameHost.Liked;

                        imageLike.Source = new FileImageSource() { File = gameHost.Liked ? "like2.png" : "like1.png" };
                        labelLikesCount.Text = gameHost.LikesCount.ToString();
                        labelLikesCount.TextColor = gameHost.LikesCount > 0 ? Config.ColorBlackTextOnWhite : Config.ColorGrayTextOnWhite;

                        bool ok = await App.WebService.SetLike(NewsfeedItemTypeEnum.GameHost, gameHost.GameHostID, gameHost.Liked);
                        if (ok == false)
                            App.Navigator.DisplayAlertError("Couldn't set the like. Internet issues?");
                    }),
                    NumberOfTapsRequired = 1
                });

				var labelInfo = new BybLabel
				{
					Text = this.IsForPast ? "Invited to play snooker" : "Invites to play snooker",
					TextColor = Config.ColorGrayTextOnWhite,
					HorizontalOptions = LayoutOptions.Fill,
					HorizontalTextAlignment = TextAlignment.Start
				};

                Label labelInfo2 = new BybLabel
                {
                    Text = (gameHost.EventType == EventTypeEnum.Private ? "Private event" : "Public event") + 
                            (gameHost.LimitOnNumberOfPlayers > 0 ? (", max. " + gameHost.LimitOnNumberOfPlayers.ToString() + " can join") : ""),
                    TextColor = Config.ColorGrayTextOnWhite,
                    HorizontalOptions = LayoutOptions.Fill,
                    HorizontalTextAlignment = TextAlignment.Start
                };

                var labelWhen = new BybLabel
				{
					FormattedText = formattedStringWhen,
					//Text = DateTimeHelper.DateAndTimeToString(gameHost.When),// gameHost.When.ToShortDateString() + " - " + gameHost.When.ToShortTimeString(),
					TextColor = Config.ColorBlackTextOnWhite
				};

				var panel0 = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Spacing = 10,
					Padding = new Thickness(5,5,5,5),
					BackgroundColor = Color.White,
					Children =
					{
						new Grid
						{
							//BackgroundColor = Color.Transparent,
							Padding = new Thickness(Config.IsTablet ? 10 : 0, 0, Config.IsTablet ? 10 : 0, 0),
							Children = 
							{
								new Image()
								{
									Source = App.ImagesService.GetImageSource(gameHost.HostPersonPicture, BackgroundEnum.White),
									WidthRequest = Config.PersonImageSize,
									HeightRequest = Config.PersonImageSize,
								}
							}
						},
						new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							VerticalOptions = LayoutOptions.Center,
							Spacing = 5,
							Children =
							{
								labelPersonHost,
								labelInfo,
								labelWhen,
								labelVenue,
                                labelInfo2,
                            },
						},
						panelCommentsCount
					}
				};

				var panelButtons = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.Fill,
					Padding = new Thickness(12,5,5,5),
					BackgroundColor = Color.White,
					Spacing = 0,
					Children =
					{
						buttonAccept,
						buttonDecline
					}
				};

                var stackGameHost = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 1,
                    //Opacity = IsForPast ? 0.5 : 1.0,
                    Children =
                    {
                        panel0,
                        panel1,
                        panel2,
                        panel3,
                        panel4,
						panelButtons,
                    }
                };

				/// event handlers
				/// 

				var recognizer = new TapGestureRecognizer
				{
					Command = new Command(async () => { await this.openGameHostPage(gameHost.GameHostID); })
				};

				panel0.GestureRecognizers.Add(recognizer);
				panel1.Content.GestureRecognizers.Add(recognizer);
				panel2.Content.GestureRecognizers.Add(recognizer);
				panel3.Content.GestureRecognizers.Add(recognizer);
				panel4.Content.GestureRecognizers.Add(recognizer);
				panelButtons.GestureRecognizers.Add(recognizer);

				if (this.TreatAsASingleItem == false)
				{
					labelPersonHost.GestureRecognizers.Add(new TapGestureRecognizer()
						{
							Command = new Command(async () => { await App.Navigator.GoToPersonProfile(gameHost.HostPersonID); })
						});
					labelVenue.GestureRecognizers.Add(new TapGestureRecognizer()
						{
							Command = new Command(async () => { await App.Navigator.GoToVenueProfile(gameHost.VenueID); })
						});
				}

				return stackGameHost;
			}
            catch (Exception exc)
            {
                return new BybLabel() { Text = TraceHelper.ExceptionToString(exc) };
            }
        }

        async void doOnUserWantsToAcceptAnInvite(GameHostWebModel gameHost)
        {
            if (gameHost.When < DateTime.Now)
            {
                App.Navigator.DisplayAlertRegular("This event is in the past!");
                return;
            }

            if (gameHost.LimitOnNumberOfPlayers > 0 && gameHost.AthleteIDs_Going.Count() >= gameHost.LimitOnNumberOfPlayers)
            {
                App.Navigator.DisplayAlertRegular("Cannot accept the invite because the maximum allowed number of players already accepted.");
                return;
            }

            if (await App.Navigator.NavPage.DisplayAlert("Byb", "Accept the invitation?", "Yes I'll come", "Cancel") != true)
                return;

            bool ok = await App.WebService.AskToJoinGameHost(gameHost.GameHostID);

			if (this.UserChangedSomething != null)
				this.UserChangedSomething(this, EventArgs.Empty);

            if (!ok)
                await App.Navigator.DisplayAlertErrorAsync("Couldn't accept the invitation. Internet issues?");
            else
                await App.Navigator.DisplayAlertRegularAsync("You accepted the invitation. Please don't forget to go to the event :) !");
        }

        async void doOnUserWantsToBeInvited(GameHostWebModel gameHost)
        {
            if (gameHost.When < DateTime.Now)
            {
                App.Navigator.DisplayAlertRegular("This event is in the past!");
                return;
            }

            if (gameHost.LimitOnNumberOfPlayers > 0 && gameHost.AthleteIDs_Going.Count() >= gameHost.LimitOnNumberOfPlayers)
            {
                App.Navigator.DisplayAlertRegular("Cannot join the event because the maximum allowed number of players already accepted.");
                return;
            }

            if (await App.Navigator.NavPage.DisplayAlert("Byb", "Ask " + gameHost.HostPersonName + " to be invited?", "Yes, please ask", "Cancel") != true)
                return;

            bool ok = await App.WebService.AskToJoinGameHost(gameHost.GameHostID);

			if (this.UserChangedSomething != null)
				this.UserChangedSomething(this, EventArgs.Empty);

            if (!ok)
				await App.Navigator.DisplayAlertErrorAsync("'Ask to be invited' failed. Internet issues?");
            else
				await App.Navigator.DisplayAlertRegularAsync("You asked to be invited. The organizer needs to accept you. This invite is now in your schedule.");
        }

        async void doOnUserWantsToDecline(GameHostWebModel gameHost)
        {
            if (gameHost.When < DateTime.Now)
            {
                App.Navigator.DisplayAlertRegular("This event is in the past!");
                return;
            }

            if (await App.Navigator.NavPage.DisplayAlert("Byb", "Decline the invitation?", "Decline invitation", "Cancel") != true)
                return;

            bool ok = await App.WebService.DeclineGameHostInvite(gameHost.GameHostID);

			if (this.UserChangedSomething != null)
				this.UserChangedSomething(this, EventArgs.Empty);

            if (!ok)
                await App.Navigator.DisplayAlertErrorAsync("Couldn't decline the invitation. Internet issues?");
            else
                await App.Navigator.DisplayAlertRegularAsync("Done.");
        }

		async void doOnUserWantsToAcceptPlayer(GameHostWebModel gameHost, int personID)
        {
            if (gameHost.When < DateTime.Now)
            {
                App.Navigator.DisplayAlertRegular("This event is in the past!");
                return;
            }

			var person = App.Cache.People.Get (personID);
			string personName = "unknown";
			if (person != null && string.IsNullOrEmpty(person.Name) == false)
				personName = person.Name;

            if (await App.Navigator.NavPage.DisplayAlert("Byb", "Accept '" + personName + ".", "Accept", "Cancel") != true)
                return;

            bool ok = await App.WebService.ApproveGameHostInvitee(gameHost.GameHostID, personID);

			if (this.UserChangedSomething != null)
				this.UserChangedSomething(this, EventArgs.Empty);

            if (!ok)
                await App.Navigator.DisplayAlertErrorAsync("Couldn't accept the player. Internet issues?");
            else
                await App.Navigator.DisplayAlertRegularAsync("Done.");
        }

        async Task openGameHostPage(int gameHostID)
        {
            if (this.IgnoreSingleItemTaps)
                return;
			if (App.Navigator.GetOpenedPage (typeof(GameHostPage)) != null)
				return;

            var page = new GameHostPage(false, false);
            await page.OpenGameHost(gameHostID);
            await App.Navigator.NavPage.Navigation.PushAsync(page);
            page.Disappearing += (s1, e1) =>
            {
                if (this.UserChangedSomething != null && page.AnythingChanged)
                    this.UserChangedSomething(this, EventArgs.Empty);
            };
        }
    }
}
