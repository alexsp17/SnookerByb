using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class NewGameHostPage : ContentPage
	{
        public bool GameHostCreated { get; private set; }

        Label buttonVenue;
        BybDatePicker datePicker;
        BybTimePicker timePicker;

        BybNoBorderPicker eventTypePicker;
        BybNoBorderPicker playersLimitPicker;

        Button buttonAddAllFriends;
        Button buttonAddPerson;
        ListOfPeopleControl listOfPeopleControl;
		Label labelExplanations1;
		Label labelExplanations2;
        StackLayout panelExplanations2;

        Entry entryComments;

		List<int> playersLimitOptions = new List<int>()
		{
			0,1,2,3,4,5,6,7,8,9,10,16,32,64
		};

        VenueWebModel selectedVenue;
        List<PersonBasicWebModel> peopleToInvite;

        public NewGameHostPage()
        {
            init();
        }

        public void AddPerson(PersonBasicWebModel person)
        {
            peopleToInvite.Add(person);
            this.fillListOfPeople();
        }

        public void SetVenue(VenueWebModel venue)
        {
            this.selectedVenue = venue;
            if (this.selectedVenue == null)
                this.buttonVenue.Text = "Pick a venue";
            else
                this.buttonVenue.Text = this.selectedVenue.Name;
        }

        void init()
		{
            this.BackgroundColor = Color.White;
            double labelWidth1 = Config.IsTablet ? 120 : 90;
            double labelWidth2 = Config.IsTablet ? 120 : 90;

            this.selectedVenue = null;
            this.peopleToInvite = new List<PersonBasicWebModel>();

            // date
            datePicker = new BybDatePicker() { MinimumDate = DateTime.Today.Date, MaximumDate = DateTime.Today.Date.AddDays(300), HorizontalOptions = LayoutOptions.FillAndExpand, Format = "ddd, MMM d, yyyy" };
            Image imageDate = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelDate = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
                Spacing = 0,
                Padding = new Thickness(15, 0, 5, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Date", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    datePicker,
                    imageDate,
                }
            };
            panelDate.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.datePicker.Focus(); })
            });
            imageDate.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.datePicker.Focus(); })
            });

            // time
            timePicker = new BybTimePicker();
            Image imageTime = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelTime = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
                Spacing = 0,
                Padding = new Thickness(15, 0, 5, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Time", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    timePicker,
                    imageTime,
                }
            };
            panelTime.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.timePicker.Focus(); })
            });
            imageTime.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.timePicker.Focus(); })
            });

            // venue
            this.buttonVenue = new BybLabel() { Text = "Pick a venue", FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center };
            Image imageVenue = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelVenue = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
                Spacing = 0,
                Padding = new Thickness(15, 0, 5, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Where", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                    buttonVenue,
                    imageVenue,
                }
            };
            panelVenue.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.buttonVenue_Clicked(); })
            });

			// players limit
			this.playersLimitPicker = new BybNoBorderPicker() { HorizontalOptions = LayoutOptions.FillAndExpand };
			foreach (int number in playersLimitOptions)
			{
				string str = "-";
				if (number != 0)
				{
					str = number.ToString ();
					if (number == 1)
						str = "Only 1";
					else if (number == 2)
						str = "1 or 2";
					else
						str = "Up to " + number.ToString ();
				}
				this.playersLimitPicker.Items.Add(str);
			}
			this.playersLimitPicker.SelectedIndex = 0;
			this.playersLimitPicker.SelectedIndexChanged += (s, e) => { this.updateExplanations(); };
			Image imagePlayersLimit = new Image()
			{
				VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
				Source = new FileImageSource() { File = "arrowRight.png" }
			};
			var panelPlayersLimit = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = 40,
				Spacing = 0,
				Padding = new Thickness(15, 0, 5, 0),
				BackgroundColor = Color.White,
				Children =
				{
					new BybLabel { Text = "Max. # of players (optional)  ", TextColor = Config.ColorGrayTextOnWhite, VerticalTextAlignment = TextAlignment.Center },
					playersLimitPicker,
					imagePlayersLimit,
				}
			};
            panelPlayersLimit.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.playersLimitPicker.Focus(); })
            });

            // event type
            this.eventTypePicker = new BybNoBorderPicker() { HorizontalOptions = LayoutOptions.FillAndExpand };
            this.eventTypePicker.Items.Add("Public");
			//this.eventTypePicker.Items.Add("Public");
			this.eventTypePicker.Items.Add("Private");
            this.eventTypePicker.SelectedIndex = 0;
			this.eventTypePicker.SelectedIndexChanged += (s, e) => { this.updateExplanations(); };
            Image imageEventType = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
				WidthRequest = Config.RedArrowImageSize,
				HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            var panelEventType = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = 40,
                Spacing = 0,
                Padding = new Thickness(15, 0, 5, 0),
                BackgroundColor = Color.White,
                Children =
                {
                    new BybLabel { Text = "Event type", WidthRequest = labelWidth2, TextColor = Config.ColorGrayTextOnWhite, VerticalTextAlignment = TextAlignment.Center },
                    eventTypePicker,
                    imageEventType,
                }
            };
            panelEventType.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { this.eventTypePicker.Focus(); })
            });

            // add buttons
            this.buttonAddPerson = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                //WidthRequest = 90,
                Text = "Add a person"
            };
            this.buttonAddAllFriends = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                //WidthRequest = 90,
                Text = "Add all friends"
            };
            this.buttonAddPerson.Clicked += buttonAddPerson_Clicked;
            this.buttonAddAllFriends.Clicked += buttonAddAllFriends_Clicked;

			// explanations
			this.labelExplanations1 = new BybLabel
			{
				Text = "",
				TextColor = Config.ColorGrayTextOnWhite,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			this.labelExplanations2 = new BybLabel
			{
				Text = "",
				TextColor = Config.ColorGrayTextOnWhite,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HorizontalTextAlignment = TextAlignment.Start,
			};
            this.panelExplanations2 = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.White,
                Padding = new Thickness(15, 0, 0, 10),
                Children =
                {
                    labelExplanations2,
                }
            };

            // list of people
			this.listOfPeopleControl = new ListOfPeopleControl();
			this.listOfPeopleControl.Spacing = 0;
            this.listOfPeopleControl.ShowRemoveButton = true;
            this.listOfPeopleControl.UserClickedRemoveOnPerson += listOfPeopleControl_UserClickedRemoveOnPerson;

            // comments
            this.entryComments = new Entry()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };

            // ok, cancel
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK" };
            Button buttonCancel = new BybButton { Text = "Cancel", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked;

            var stackLayout = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0),
                BackgroundColor = Config.ColorGrayBackground,

                Children =
                {
                    new BybTitle("New Event") { VerticalOptions = LayoutOptions.Start },

                    new ScrollView
                    {
                        Padding = 0,

                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Padding = new Thickness(15),
                            Spacing = 0,
                            Children =
                            {
                                panelDate,
                                panelTime,
                                panelVenue,

                                new BoxView() { BackgroundColor = Config.ColorGrayBackground, HeightRequest = 2 },
                                new StackLayout
                                {
                                    Orientation = StackOrientation.Horizontal,
                                    HorizontalOptions = LayoutOptions.FillAndExpand,
                                    Padding = new Thickness(15,0,5,0),
									BackgroundColor = Color.White,
                                    Children =
                                    {
										new BybLabel { Text = "Players to invite:", TextColor = Config.ColorGrayTextOnWhite, VerticalTextAlignment = TextAlignment.Center },
										buttonAddPerson,
										buttonAddAllFriends,
                                    }
                                },
								new StackLayout
								{
									Orientation = StackOrientation.Horizontal,
									BackgroundColor = Color.White,
									Padding = new Thickness(15,0,0,10),
									Children = 
									{
										new BybLabel
										{
											Text = "Invited players will be sent a push notification and an email once you click the OK button.",
											TextColor = Config.ColorGrayTextOnWhite,
											HorizontalOptions = LayoutOptions.FillAndExpand,
											HorizontalTextAlignment = TextAlignment.Start,
										},
									}
								},
								this.listOfPeopleControl,

								new BoxView() { BackgroundColor = Config.ColorGrayBackground, HeightRequest = 2 },
                                panelEventType,
								new StackLayout
								{
									Orientation = StackOrientation.Horizontal,
									BackgroundColor = Color.White,
									Padding = new Thickness(15,0,0,10),
									Children = 
									{
										labelExplanations1,
									}
								},

                                new BoxView() { BackgroundColor = Config.ColorGrayBackground, HeightRequest = 2 },
                                panelPlayersLimit,
                                panelExplanations2,

                                new BoxView() { BackgroundColor = Config.ColorGrayBackground, HeightRequest = 2 },
                                new BybLabel { Text = "Comments about the event (optional):", TextColor = Config.ColorGrayTextOnWhite, WidthRequest = labelWidth1, VerticalTextAlignment = TextAlignment.Center },
                                this.entryComments,
                            }
                        }
                    },

                    new BoxView() { HeightRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Transparent },

                    new StackLayout()
                    {
                        Spacing = 1,
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Fill,
                        HeightRequest = Config.OkCancelButtonsHeight,
                        Padding = new Thickness(Config.OkCancelButtonsPadding),
                        Children =
                        {
                            buttonCancel,
                            buttonOk,
                        }
                    }
                }
            };

            this.Content = stackLayout;
            this.Padding = new Thickness(0, 0, 0, 0);

            this.fillListOfPeople();
			this.updateExplanations ();
		}

        private void listOfPeopleControl_UserClickedRemoveOnPerson(object sender, SelectedPersonEventArgs e)
        {
            this.peopleToInvite.Remove(e.Person);
            this.listOfPeopleControl.Fill(this.peopleToInvite);
        }

        void fillListOfPeople()
        {
            this.listOfPeopleControl.Fill(this.peopleToInvite);
            if (this.peopleToInvite.Count > 0)
            {
                this.listOfPeopleControl.IsVisible = true;
            }
            else
            {
                this.listOfPeopleControl.IsVisible = false;
            }
        }

		void updateExplanations()
		{
			string text1 = "";
			if (this.eventTypePicker.SelectedIndex == 0)
				text1 = "Players in the community will learn about this event from the Snooker Byb community page and the daily email. They will request to be 'accepted' to this event.";
			//else if (this.eventTypePicker.SelectedIndex == 1)
			//	text1 = "Players in the community will learn about this event from the Snooker Byb community page and the daily email. They will be able to 'join' the event.";
			else if (this.eventTypePicker.SelectedIndex == 1)
				text1 = "Only the invited players will know about this event.";
			else if (this.eventTypePicker.SelectedIndex == 2)
				text1 = "As a user of 'Snooker Byb for Venues' you can invite people to participate in a tournament using this option.";
			this.labelExplanations1.Text = text1;

			string text2 = "";
			int number = this.playersLimitOptions [this.playersLimitPicker.SelectedIndex];
			if (number <= 0)
				text2 = "";
			else if (number == 1)
				text2 = "Only the first player will be allowed to 'join'.";
			else
				text2 = "Only the first " + number + " players will be allowed to 'join'.";
			this.labelExplanations2.Text = text2;
			this.panelExplanations2.IsVisible = text2.Length > 0;
		}

        private async void buttonAddAllFriends_Clicked(object sender, EventArgs e)
        {
            this.IsBusy = true;

            var friends = await App.WebService.GetFriends();

            this.IsBusy = false;

            if (friends == null)
                return;

            foreach (var person in friends)
                if (this.peopleToInvite.Where(i => i.ID == person.ID).Count() == 0)
                    this.peopleToInvite.Add(person);
            this.fillListOfPeople();
        }

        private void buttonAddPerson_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(PickAthletePage)) != null)
				return;
			
            var pickAthletePage = new PickAthletePage()
            {
                TitleText = "Pick a Person to Invite"
            };
            pickAthletePage.UserMadeSelection += (s1, e1) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();

                var person = e1.Person;
                if (person == null)
                    return;
                if (peopleToInvite.Where(i => i.ID == person.ID).Count() > 0)
                    return;
                peopleToInvite.Add(person);
                this.fillListOfPeople();
            };
            App.Navigator.NavPage.Navigation.PushModalAsync(pickAthletePage);
        }

        async void buttonVenue_Clicked()
        {
			if (App.Navigator.GetOpenedPage (typeof(PickVenuePage)) != null)
				return;
			
            var pickVenuePage = new PickVenuePage();
            pickVenuePage.UserMadeSelection += (s1, venue) =>
            {
                App.Navigator.NavPage.Navigation.PopModalAsync();

                this.SetVenue(venue);
            };
            await App.Navigator.NavPage.Navigation.PushModalAsync(pickVenuePage);
        }

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        private async void buttonOk_Clicked(object sender, EventArgs e)
        {
            if (this.selectedVenue == null)
            {
                await App.Navigator.DisplayAlertRegularAsync("Pick a venue you want to play at.");
                return;
            }

            // the user's selection
            DateTime date = this.datePicker.Date;
            var time = this.timePicker.SelectedTime;
            DateTime when = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, 0, 0);
            if (when < DateTime.Now)
            {
                await App.Navigator.DisplayAlertRegularAsync("Cannot create an event in the past.");
                return;
            }
            EventTypeEnum eventType = EventTypeEnum.PublicAcceptRequired;
            //if (this.eventTypePicker.SelectedIndex == 1)
            //    eventType = EventTypeEnum.PublicAcceptNotRequired;
            if (this.eventTypePicker.SelectedIndex == 1)
                eventType = EventTypeEnum.Private;
            int limitOnNumberOfPlayers = (int)this.playersLimitOptions[this.playersLimitPicker.SelectedIndex];
            List<int> invitees = new List<int>();
            foreach (var person in this.listOfPeopleControl.List)
                invitees.Add(person.ID);
            if (invitees.Count > 50)
            {
                await App.Navigator.DisplayAlertRegularAsync("You are inviting too many people. The limit is 50.");
                return;
            }
            if (eventType == EventTypeEnum.Private && invitees.Count == 0)
            {
                await App.Navigator.DisplayAlertRegularAsync("You are starting a private event, yet nobody is invited. Add players to the event.");
                return;
            }
            string comments = this.entryComments.Text;

            // open "please wait"
            PleaseWaitPage pleaseWaitPage = new PleaseWaitPage();
            await App.Navigator.NavPage.Navigation.PushModalAsync(pleaseWaitPage);

            // send a request to the cloud
            int? newGameHostID = await App.WebService.NewGameHost(this.selectedVenue.ID, when, eventType, limitOnNumberOfPlayers, invitees, comments);
            this.GameHostCreated = true;

            if (newGameHostID == null)
            {
                // close "please wait"
                await App.Navigator.NavPage.Navigation.PopModalAsync();

                await App.Navigator.DisplayAlertErrorAsync("Couldn't create a new game host. Internet issues?");
                return;
            }

            // close "please wait"
            await App.Navigator.NavPage.Navigation.PopModalAsync();

            // close this dialog
            await App.Navigator.NavPage.Navigation.PopModalAsync();

            if (App.MobileNotificationsService != null)
                App.MobileNotificationsService.AddReminder(
                    when.AddHours(-0.5),
                    "Snooker game reminder", 
                    "A snooker game is scheduled for " + when.ToShortTimeString());
        }

        bool pageDisappeared = false;

        protected override void OnDisappearing()
        {
            this.pageDisappeared = true;
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // get person's location
            App.LocationService.RequestLocationAsync((s1, e1) =>
            {
                if (this.pageDisappeared)
                    return;

                // see if we are close to one of the venues in the cache
                Location currentLocation = App.LocationService.Location;
            }, false);
        }
	}
}
