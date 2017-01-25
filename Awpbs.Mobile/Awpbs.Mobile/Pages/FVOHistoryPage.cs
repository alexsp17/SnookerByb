using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class FVOHistoryPage : ContentPage
	{
        Button buttonSync;
        Label labelTop;
        FVOListOfSnookerMatchesControl listOfMatchesControl;
        ListOfSnookerBreaksControl listOfBreaksControl;

        public FVOHistoryPage()
        {
            this.BackgroundColor = Config.ColorBackground;
            this.Padding = new Thickness(0, 0, 0, 0);

            /// top panel
            /// 

            Grid panelTop = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = Config.TitleHeight,
                BackgroundColor = Config.ColorBlackBackground,
                Padding = new Thickness(20,0,20,0),
            };
            this.labelTop = new BybLabel()
            {
                Text = "History",
                FontSize = Config.LargerFontSize,
                TextColor = Config.ColorTextOnBackground,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
            };
            panelTop.Children.Add(labelTop);
            var imageBack = new Image()
            {
                Source = new FileImageSource() { File = "thinArrow1Left.png" },
                HeightRequest = Config.IsTablet ? 25 : 20,
                WidthRequest = Config.IsTablet ? 25 : 20,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
            };
            Frame buttonBack = new Frame()
            {
                Padding = new Thickness(0, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HasShadow = false,
                BackgroundColor = Config.ColorBlackBackground,
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 10,
                    Children =
                    {
                        imageBack,
                        new BybLabel()
                        {
                            Text = "Back",
                            FontSize = Config.LargerFontSize,
                            TextColor = Config.ColorTextOnBackground,
                            VerticalOptions = LayoutOptions.Center,
                        }
                    }
                },
            };
            panelTop.Children.Add(buttonBack);
            buttonBack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    this.Navigation.PopModalAsync();
                }),
                NumberOfTapsRequired = 1
            });
            buttonSync = new BybButton()
            {
                Text = "Sync",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                FontSize = Config.LargerFontSize,
                TextColor = Config.ColorTextOnBackground,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
            };
            panelTop.Children.Add(buttonSync);
            buttonSync.Clicked += buttonSync_Clicked;

            /// lists
            /// 

            this.listOfMatchesControl = new FVOListOfSnookerMatchesControl();
            listOfMatchesControl.HorizontalOptions = LayoutOptions.FillAndExpand;
            listOfMatchesControl.VerticalOptions = LayoutOptions.FillAndExpand;
            listOfMatchesControl.BackgroundColor = Config.ColorGrayBackground;
            listOfMatchesControl.Padding = new Thickness(5);

            this.listOfBreaksControl = new ListOfSnookerBreaksControl();
            this.listOfBreaksControl.Type = ListTypeEnum.FVO;
            listOfBreaksControl.HorizontalOptions = LayoutOptions.FillAndExpand;
            listOfBreaksControl.VerticalOptions = LayoutOptions.FillAndExpand;
            listOfBreaksControl.BackgroundColor = Config.ColorGrayBackground;
            listOfBreaksControl.Padding = new Thickness(5);

            Grid grid = new Grid()
            {
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                //VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                }
            };
            grid.Children.Add(listOfMatchesControl, 0, 0);
            grid.Children.Add(listOfBreaksControl, 1, 0);

            ScrollView scrollView = new ScrollView()
            {
                Padding = new Thickness(0),
                Content = grid,
            };

            /// root panel
            /// 

            Grid gridRoot = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                },
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(Config.TitleHeight, GridUnitType.Absolute) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                }
            };
            gridRoot.Children.Add(panelTop, 0, 0);
            gridRoot.Children.Add(scrollView, 0, 1);
            //grid.Children.Add(panelTop, 0, 2, 0, 1);
            //grid.Children.Add(listOfMatchesControl, 0, 1);
            //grid.Children.Add(listOfBreaksControl, 1, 1);

            this.Content = gridRoot;
        }

        public async Task Fill()
        {
            this.labelTop.Text = "loading...";

            int venueID = FVOConfig.LoadFromKeyChain(App.KeyChain).VenueID;

            var resultsWeb = await App.WebService.GetResultsAtVenue(venueID);
            var results = resultsWeb.Select(r => r.ToResult()).ToList();
            var scores = await App.WebService.GetScoresAtVenue(venueID);
            bool failedToLoadFromWeb = results == null || scores == null;

            if (failedToLoadFromWeb)
            {
                scores = App.Repository.GetScores(true);
                results = App.Repository.GetResults(true).ToList();
            }
            
            var matches = (from score in scores
                           select SnookerMatchScore.FromScore(score.AthleteAID, score)).ToList();
            var breaks = (from result in results
                          select SnookerBreak.FromResult(result)).ToList();

            await new CacheHelper().LoadFromWebserviceIfNecessary_People(App.Cache, results, scores);
            new CacheHelper().LoadNamesFromCache(App.Cache, breaks);
            new CacheHelper().LoadNamesFromCache(App.Cache, matches);

            listOfMatchesControl.Fill(matches);
            listOfBreaksControl.Fill(breaks);

            this.labelTop.Text = failedToLoadFromWeb ? "Failed to load. Internet issues?" : "History";
        }

        private async void buttonSync_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync();
            App.Sync.StartAsync();
        }
    }
}
