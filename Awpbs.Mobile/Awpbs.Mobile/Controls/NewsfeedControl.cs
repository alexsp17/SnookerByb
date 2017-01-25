using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class NewsfeedControl : StackLayout
    {
		public event Action LoadStarted;
		public event Action LoadCompleted;
		
        public CommunitySelection CurrentCommunity
        {
            get { return this.communitySelectorControl.Selection; }
        }

        CommunitySelectorControl communitySelectorControl;
		ListOfNewsfeedItemsControl list;
		NewsfeedWebModel newsfeed;

		public void ReloadAsync(CommunitySelection communitySelection, bool onlyIfItsBeenAwhile)
        {
			bool isCommunitySelectionChanged = this.CurrentCommunity.Compare (communitySelection) == false;
			if (isCommunitySelectionChanged)
				this.communitySelectorControl.Selection = communitySelection;

			if (isCommunitySelectionChanged)
				onlyIfItsBeenAwhile = false;

			this.reloadAsync(onlyIfItsBeenAwhile);
        }

        public NewsfeedControl()
		{
            this.Spacing = 0;
			this.BackgroundColor = Config.ColorGrayBackground;
            this.Padding = new Thickness(0);

            /// community selector
            /// 
            this.communitySelectorControl = new CommunitySelectorControl();
            this.communitySelectorControl.SelectionChanged += communitySelectorControl_SelectionChanged;
            this.Children.Add(new StackLayout()
            {
                Padding = new Thickness(0, 5, 0, 10),
                HeightRequest = Config.LargeButtonsHeight,// 40,
                Spacing = 1,
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorGrayBackground,
                Children =
                {
                    communitySelectorControl,
                }
            });

            /// buttons
            /// 
            Button buttonPost = new BybButton() { Text = "Post", Style = (Style)App.Current.Resources["BlackButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
            Button buttonGameHost = new BybButton() { Text = "New event", Style = (Style)App.Current.Resources["BlackButtonStyle"], HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonPost.Clicked += (s1, e1) =>
            {
                if (this.CurrentCommunity == null || this.CurrentCommunity.Country == null)
                {
                    App.Navigator.DisplayAlertRegular("You don't have enough contributions to post on the Planet Earth level. Select a smaller community to post.");
                    return;
                }

				if (App.Navigator.GetOpenedPage(typeof(NewPostPage)) != null)
					return;
				
                NewPostPage dlg = new NewPostPage(this.CurrentCommunity.Country, this.CurrentCommunity.MetroID);
                App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
                dlg.Disappearing += (s2, e2) =>
                {
					this.reloadAsync(!dlg.Posted);
                };
            };
            buttonGameHost.Clicked += (s1, e1) =>
            {
				if (App.Navigator.GetOpenedPage(typeof(NewGameHostPage)) != null)
					return;
				
                NewGameHostPage dlg = new NewGameHostPage();
                App.Navigator.NavPage.Navigation.PushModalAsync(dlg);
                dlg.Disappearing += (s2, e2) =>
                {
					this.reloadAsync(!dlg.GameHostCreated);
                };
            };
            this.Children.Add(new StackLayout()
            {
                Spacing = 1,
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(10, 0, 10, 0),
                Children =
                {
                    buttonGameHost,
                    buttonPost,
                }
            });

			this.list = new ListOfNewsfeedItemsControl ();
			this.list.NeedsARefresh += () =>
			{
				this.reloadAsync(false);
			};
            this.Children.Add(list);
        }

        private void communitySelectorControl_SelectionChanged(object sender, EventArgs e)
        {
            this.reloadAsync(false);
        }

        bool isLoadingNow = false;
        bool shouldDoAnotherLoad = false;
        DateTime timeOfLastReload = DateTime.MinValue;

		void reloadAsync(bool onlyIfItsBeenAwhile = false)
		{
			if (onlyIfItsBeenAwhile)
			{
				if ((DateTime.Now - timeOfLastReload).TotalMinutes < 2)
					return;
			}
			
			if (isLoadingNow == true)
			{
				shouldDoAnotherLoad = true;
				return;
			}
			shouldDoAnotherLoad = false;
			
			App.Navigator.SetRootPageTitleToLoading ();
			if (this.LoadStarted != null)
				this.LoadStarted ();
			
			new Task(async () =>
			{
				// load data from web
				for (int iter = 0; iter < 3; ++iter)
				{
					await this.loadNewsfeedFromWeb();
					if (shouldDoAnotherLoad == false)
						break;
				}

				// fill the list
				Device.BeginInvokeOnMainThread(() =>
				{
					bool isCanada = this.CurrentCommunity.Country != null && this.CurrentCommunity.Country.IsCanada;
					list.Fill (newsfeed, isCanada);
					App.Navigator.SetRootPageTitleToNormal();

					if (this.LoadCompleted != null)
						this.LoadCompleted();
				});
			}).Start();
		}

        async Task loadNewsfeedFromWeb()
		{
			isLoadingNow = true;
			
			// query the web service
			if (this.CurrentCommunity.MetroID > 0)
				newsfeed = await App.WebService.GetNewsfeed (this.CurrentCommunity.MetroID);
			else if (this.CurrentCommunity.Country != null)
				newsfeed = await App.WebService.GetNewsfeed (this.CurrentCommunity.Country.ThreeLetterCode);
			else
				newsfeed = await App.WebService.GetNewsfeed ("all");
            
			isLoadingNow = false;
			this.timeOfLastReload = DateTime.Now;
		}
    }
}
