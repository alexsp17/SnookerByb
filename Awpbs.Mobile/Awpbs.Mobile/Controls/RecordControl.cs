using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class RecordControl : ScrollView
    {
        StackLayout stackTopLevel;

        StackLayout panelNewMatch;
        SnookerMatchMetadata metadata;
        SnookerMatchMetadataControl metadataControl;
        BybButton buttonNewMatch;
        BybButton buttonNewBreak;

        StackLayout panelPausedMatches;

        public RecordControl()
        {
            this.metadata = new MetadataHelper().CreateDefault();

			this.Padding = new Thickness(0);
			this.BackgroundColor = Config.ColorGrayBackground;

            // new match panel
            this.metadataControl = new SnookerMatchMetadataControl(this.metadata, true);
            this.metadataControl.Padding = new Thickness(0, 0, 0, 0);
            this.buttonNewMatch = new BybButton()
            {
                Text = "Record a match",
                Style = (Style)App.Current.Resources["BlackButtonStyle"],
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            this.buttonNewBreak = new BybButton()
            {
                Text = "Record a break",
                Style = (Style)App.Current.Resources["BlackButtonStyle"],
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            this.buttonNewMatch.Clicked += buttonNewMatch_Clicked;
            this.buttonNewBreak.Clicked += buttonNewBreak_Clicked;
            this.panelNewMatch = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Padding = new Thickness(0),
                Children =
                {
                    this.metadataControl,
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Padding = new Thickness(0, 12, 0, 0),
						Spacing = Config.SpaceBetweenButtons,
                        Children =
                        {
                            this.buttonNewBreak,
                            this.buttonNewMatch,
                        }
                    }
                }
            };

            // paused matches panel
            this.panelPausedMatches = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Padding = new Thickness(0, 0, 0, 0)
            };

            // the top level stack
            this.stackTopLevel = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(12,12,12,12),
                Spacing = 0,
                Children =
                {
                    this.panelPausedMatches,
                    this.panelNewMatch,
                }
            };
            this.Content = this.stackTopLevel;

			this.stackTopLevel.GestureRecognizers.Add (new TapGestureRecognizer () {
				Command = new Command(() =>
					{
						App.Navigator.ShowInternalOptions();
					}),
				NumberOfTapsRequired = 2,
			});

        }

		protected override void OnSizeAllocated (double width, double height)
		{
			if (Config.IsTablet && width > 0)
			{
				double stackWidth = System.Math.Min (450, width);
				double padding = System.Math.Abs((width - stackWidth) / 2);
				this.stackTopLevel.WidthRequest = stackWidth;
				this.stackTopLevel.HorizontalOptions = LayoutOptions.Center;
				this.Padding = new Thickness (0, padding, 0, 0);
			}
			
			base.OnSizeAllocated (width, height);
		}

        LocationHelper locationHelper;

        public void DoOnOpen()
        {
			// if there is an unsaved match on the keychain, persist it into the database
			new TempSavedMatchHelper(App.KeyChain).SaveToDbAndRemove(App.Repository);

            this.metadata.Date = DateTime.Now;
            this.metadataControl.Fill(this.metadata);
            this.fillPausedMatches();

            if (locationHelper == null)
                locationHelper = new LocationHelper(App.LocationService, App.WebService);
            locationHelper.CheckForClosestVenue((s1, e1) =>
            {
                var closestVenue = locationHelper.ClosestVenue;
                if (closestVenue != null)
                {
                    this.metadata.VenueID = closestVenue.ID;
                    this.metadata.VenueName = closestVenue.Name;
                    this.metadataControl.Fill(this.metadata);
                }
            });
        }

        private void buttonNewBreak_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(RecordBreakPage)) != null)
				return;
			
			var page = new RecordBreakPage(this.metadata, false, true);
            App.Navigator.NavPage.Navigation.PushModalAsync(page);
            page.Done += async (s1, e1) =>
            {
                SnookerBreak snookerBreak = e1;
                this.metadataControl.Fill(this.metadata);
                if (snookerBreak == null)
                    return;

                if (page.IsOpponentsBreak == false)
                {
                    // save this as a notable break
                    Result result = new Result();
                    snookerBreak.PostToResult(result);
                    result.AthleteID = App.Repository.GetMyAthleteID();
                    result.TimeModified = DateTimeHelper.GetUtcNow();
                    App.Repository.AddResult(result);

                    await App.Navigator.GoToMyProfile(ProfilePersonStateEnum.Breaks, true);
                    App.Navigator.StartSyncAndCheckForNotifications();
                }
                else
                {
                    // save this as opponent's break
                    Result result = new Result();
                    snookerBreak.PostToResult(result);
                    result.AthleteID = metadata.OpponentAthleteID;
                    result.OpponentAthleteID = App.Repository.GetMyAthleteID();
                    result.TimeModified = DateTimeHelper.GetUtcNow();
                    result.IsNotAcceptedByAthleteYet = true;
                    App.Repository.AddResult(result);

                    App.Navigator.StartSyncAndCheckForNotifications();
                    App.Navigator.DisplayAlertRegular("The break was recorded as a notable break for '" + metadata.OpponentAthleteName + "'. Once the data is synced with snookerbyb.com, '" + metadata.OpponentAthleteName + "' will be able to accept it.");
                }
            };
        }

        private async void buttonNewMatch_Clicked(object sender, EventArgs e)
        {
			if (App.Navigator.GetOpenedPage (typeof(RecordMatchPage)) != null)
				return;
			
			var page2 = new RecordMatchPage(this.metadata);
            await App.Navigator.NavPage.Navigation.PushModalAsync(page2);
            page2.Disappearing += (s1, e1) =>
            {
                this.metadata = new MetadataHelper().FromScoreForYou(page2.MatchScore);
                this.metadataControl.Fill(this.metadata);
            };
        }

        void fillPausedMatches()
        {
			// incompleted matches in the db
            var scoreObjs = (from i in App.Repository.GetScores(false)
                             where i.IsUnfinished == true
							 orderby i.TimeModified descending
							 select i).ToList();

			this.panelNewMatch.IsVisible = scoreObjs.Count() == 0;
			this.panelPausedMatches.Children.Clear();

            foreach (var scoreObj in scoreObjs)
            {
                var match = SnookerMatchScore.FromScore(scoreObj.AthleteAID, scoreObj);
                var matchMetadata = new MetadataHelper().FromScoreForYou(match);

                var metadataControl = new SnookerMatchMetadataControl(matchMetadata, true)
                {
                    Padding = new Thickness(0, 0, 0, 0)
                };
                this.panelPausedMatches.Children.Add(metadataControl);

                var buttonDelete = new BybButton()
                {
                    Text = "Cancel the match",
                    Style = (Style)App.Current.Resources["BlackButtonStyle"],
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                buttonDelete.Clicked += async (s1, e1) =>
                {
                    if (await App.Navigator.NavPage.DisplayAlert("Byb", "Delete this match?", "Yes, delete", "Cancel") == true)
                    {
                        App.Repository.SetIsDeletedOnScore(scoreObj.ScoreID, true);
                        this.DoOnOpen();
                    }
                };

                var buttonContinue = new BybButton()
                {
                    Text = "Continue the match",
                    Style = (Style)App.Current.Resources["LargeButtonStyle"],
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                buttonContinue.Clicked += async (s1, e1) =>
                {
                    new MetadataHelper().ToScore(matchMetadata, match);

					var page = new RecordMatchPage(match, false);
                    await App.Navigator.NavPage.Navigation.PushModalAsync(page);
                };

                this.panelPausedMatches.Children.Add(new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = new Thickness(0, 12, 0, 0),
                    Spacing = 1,
                    Children =
                    {
                        buttonDelete,
                        buttonContinue
                    }
                });
                this.panelPausedMatches.Children.Add(new BoxView() { HeightRequest = 25 });

				// load opponent names from web if it's not loaded yet
				if (matchMetadata.OpponentAthleteID > 0 && string.IsNullOrEmpty (matchMetadata.OpponentAthleteName) == true)
				{
					Task.Run (async () =>
					{
						await App.Cache.LoadFromWebserviceIfNecessary_People (new List<int> () { matchMetadata.OpponentAthleteID });
						var person = App.Cache.People.Get (matchMetadata.OpponentAthleteID);
						if (person != null)
						{
							Device.BeginInvokeOnMainThread (() =>
							{
								match.OpponentName = person.Name;
								match.OpponentPicture = person.Picture;
								matchMetadata.OpponentAthleteName = person.Name;
								matchMetadata.OpponentPicture = person.Picture;
								metadataControl.Refill ();
							});
						}
					});
				};
            }
        }
    }
}
