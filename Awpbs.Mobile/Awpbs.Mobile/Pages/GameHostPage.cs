using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class GameHostPage : ContentPage
	{
        public bool AnythingChanged { get; set; }

        ListOfGameHostsControl gameHostsControl;
        ListOfCommentsControl listOfCommentsControl;
        Editor editor;
        Button buttonPostComment;
		Button buttonDelete;
        Button buttonAddPerson;
        BybTitle title;

		Athlete myAthlete;
        GameHostWebModel gameHost;
        bool isExpectedToAcceptInvitation;
        bool isModal;
        bool isAccepted;

        public GameHostPage(bool isExpectedToAcceptInvitation, bool isModal = true)
        {
            this.isExpectedToAcceptInvitation = isExpectedToAcceptInvitation;
            this.isModal = isModal;
            this.isAccepted = false;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.Padding = new Thickness(0);

			this.myAthlete = App.Repository.GetMyAthlete ();

            // ok
            Button buttonClose = new BybButton { Text = "OK", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonClose.Clicked += buttonOk_Clicked;

			// delete
			this.buttonDelete = new BybButton
			{
				Text = "Delete",
				Style = (Style)App.Current.Resources ["LargeButtonStyle"], 
				WidthRequest = Config.IsTablet ? 140 : 100,
				HorizontalOptions = LayoutOptions.Start,
				IsVisible = false,
			};
			buttonDelete.Clicked += buttonDelete_Clicked;

            // add a person
            this.buttonAddPerson = new BybButton
            {
                Text = "Add a player",
                Style = (Style)App.Current.Resources["LargeButtonStyle"],
                WidthRequest = Config.IsTablet ? 160 : 120,
                HorizontalOptions = LayoutOptions.Start,
                IsVisible = false,
            };
            buttonAddPerson.Clicked += buttonAddPerson_Clicked;

            this.gameHostsControl = new ListOfGameHostsControl()
            {
                ShowCommentsCount = false,
                IgnoreSingleItemTaps = true,
            };
            this.gameHostsControl.UserChangedSomething += async (s1, e1) =>
            {
                this.AnythingChanged = true;
				if (isModal)
					await App.Navigator.NavPage.Navigation.PopModalAsync();
				else
					await App.Navigator.NavPage.PopAsync();
            };

            this.listOfCommentsControl = new ListOfCommentsControl();

            this.editor = new BybEditor()
            {
                HeightRequest = 40,
            };
            this.editor.Completed += editor_Completed;
            this.editor.Unfocused += editor_Unfocused;
            this.editor.Focused += editor_Focused;

            this.buttonPostComment = new BybButton { Text = "Done", Style = (Style)App.Current.Resources["SimpleButtonStyle"], IsVisible = false, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            this.buttonPostComment.Clicked += (s1, e1) =>
            {
                this.editor.Unfocus();
            };

            var rootPanel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                Padding = new Thickness(0, 0, 0, 0),
                Children =
                {
                    //new BoxView() { HeightRequest = 20, VerticalOptions = LayoutOptions.Start, BackgroundColor = Color.Transparent },

                    new ScrollView
                    {
                        Orientation = ScrollOrientation.Vertical,
                        Padding = new Thickness(0,0,0,0),
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Padding = new Thickness(10,10,10,0),
                            Children =
                            {
                                gameHostsControl,//panel,
                                listOfCommentsControl,
                                new StackLayout
                                {
                                    BackgroundColor = Config.ColorGrayBackground,
                                    Padding = new Thickness(10,10,0,0),
                                    Children =
                                    {
                                        new BybLabel
                                        {
                                            Text = "Enter your comments here:",
											TextColor = Config.ColorGrayTextOnWhite,
                                        },
                                        buttonPostComment
                                    }
                                },
                                new Frame
                                {
                                    BackgroundColor = Config.ColorGrayBackground,
                                    Padding = new Thickness(10,5,5,5),
                                    Content = editor,
                                }
                            }
                        },
                    },

                    new BoxView() { HeightRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Transparent },

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Config.ColorBackground,
                        HorizontalOptions = LayoutOptions.Fill,
                        HeightRequest = Config.OkCancelButtonsHeight,
                        Padding = new Thickness(Config.OkCancelButtonsPadding),
						Spacing = 1,
                        Children =
                        {
							buttonDelete,
                            buttonAddPerson,
                            buttonClose,
                        }
                    }
                }
            };

            this.title = null;
            if (isModal)
            {
                this.title = new BybTitle("Byb Invite");
                rootPanel.Children.Insert(0, title);
            }
            this.Content = rootPanel;
        }

        public async Task OpenGameHost(int gameHostID)
        {
            this.setTitle("Loading...");
            this.gameHost = await App.WebService.GetGameHost(gameHostID);

            if (this.gameHost == null)
            {
                this.setTitle("Error");
                await DisplayAlert("Byb", "Error. Couldn't load the game info. Internet issues?", "Cancel");
                return;
            }

            // can delete or add a person?
            bool isHost = (Config.IsProduction == false && myAthlete.IsAdmin) || gameHost.HostPersonID == myAthlete.AthleteID;
            this.buttonDelete.IsVisible = isHost;
            this.buttonAddPerson.IsVisible = isHost;

			// fill
            this.gameHostsControl.Fill(new List<GameHostWebModel>() { gameHost });
            await this.loadComments();

            this.setTitle("Event");
        }

        void setTitle(string text)
        {
            if (this.title != null)
                this.title.Text = text;
            this.Title = text;
        }

        async Task loadComments()
        {
            var comments = await App.WebService.GetComments(NewsfeedItemTypeEnum.GameHost, this.gameHost.GameHostID);
            this.listOfCommentsControl.Fill(comments);
        }

        private async void buttonOk_Clicked(object sender, EventArgs e)
        {
            int myAthleteID = App.Repository.GetMyAthleteID();
            this.isAccepted = this.gameHost.IsGoing(myAthleteID);

            if (this.isExpectedToAcceptInvitation == true && this.isAccepted == false)
            {
                bool ok = await DisplayAlert("Byb", "You have not accepted the invite yet.", "Cancel", "Do not accept");
                if (ok == true)
                    return;
            }

            if (isModal)
                await App.Navigator.NavPage.Navigation.PopModalAsync();
            else
                await App.Navigator.NavPage.PopAsync();
        }

        private async void editor_Completed(object sender, EventArgs e)
        {
            string text = this.editor.Text;
            if (text == null)
                text = "";
            text = text.Trim();
            if (text.Length == 0)
                return;
            if (text.Length > 512)
                text = text.Substring(0, 512);

            this.editor.Text = "";

            if (await App.WebService.MakeComment(NewsfeedItemTypeEnum.GameHost, this.gameHost.GameHostID, text) == null)
            {
                this.editor.Text = text;
                App.Navigator.DisplayAlertError("Couldn't post comment. Internet issues?");
                return;
            }
            this.AnythingChanged = true;

            await loadComments();
        }

        private void editor_Unfocused(object sender, EventArgs e)
        {
            this.buttonPostComment.IsVisible = false;
        }

        private void editor_Focused(object sender, EventArgs e)
        {
            this.buttonPostComment.IsVisible = true;
        }

		private async void buttonDelete_Clicked(object sender, EventArgs e)
		{
			bool canDelete = gameHost.AthleteIDs_Going.Count == 0;

			if (canDelete == false)
			{
				await this.DisplayAlert("Byb", "Cannot delete the invite because there is at least one accepted player. Write a comment to notify instead.", "OK");
				return;
			}
			
			if (await this.DisplayAlert("Delete?", "Delete this invite?", "Yes, delete", "Cancel") == false)
				return;

			bool ok = await App.WebService.DeleteGameHost(gameHost.GameHostID);
			if (!ok)
			{
				await this.DisplayAlert("Byb", "Error. Failed to delete.", "OK");
				return;
			}

			this.AnythingChanged = true;

			if (isModal)
				await App.Navigator.NavPage.Navigation.PopModalAsync();
			else
				await App.Navigator.NavPage.PopAsync();
		}

        private async void buttonAddPerson_Clicked(object sender, EventArgs e)
        {
            if (gameHost.When < DateTime.Now)
            {
                await this.DisplayAlert("Byb", "This event is in the past. Cannot add players to it.", "OK");
                return;
            }

            PickAthletePage page = new PickAthletePage();
            page.TitleText = "Invite a Player to the Event";
            page.ShowUnknown = false;
            await this.Navigation.PushModalAsync(page);
            page.UserMadeSelection += (s1, e1) =>
            {
                this.Navigation.PopModalAsync();

                var person = e1.Person;
                if (person == null)
                    return;
                if (gameHost.IsAthleteIncluded(person.ID))
                {
                    this.DisplayAlert("Byb", "This person is already included into this event.", "OK");
                    return;
                }

                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (gameHost.AthleteIDs_Going.Count() >= gameHost.LimitOnNumberOfPlayers)
                    {
                        string increase = "Yes, increase the limit";
                        if (await this.DisplayActionSheet("Increase the limit on number of players?", "Cancel", null, increase) != increase)
                            return;

                        if (await App.WebService.ChangeLimitOnNumberOfPlayers(gameHost.GameHostID, gameHost.LimitOnNumberOfPlayers + 1) == false)
                        {
                            await this.DisplayAlert("Byb", "Couldn't add this player to the event. Internet connection issues?", "OK");
                            return;
                        }
                    }

                    this.AnythingChanged = true;

                    if (await App.WebService.NewGameHostInvite(gameHost.GameHostID, person.ID, DateTime.Now) == null)
                    {
                        await this.DisplayAlert("Byb", "Couldn't add this player to the event. Internet connection issues?", "OK");
                        return;
                    }

                    await this.OpenGameHost(gameHost.GameHostID);
                });
            };
        }
    }
}
