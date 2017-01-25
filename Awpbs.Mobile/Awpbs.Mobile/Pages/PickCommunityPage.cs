using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class PickCommunityPage : ContentPage
	{
        readonly int itemHeight = Config.OkCancelButtonsHeight - 3;

        Athlete myAthlete;
        Country myCountry;

        BybTitle title;
        StackLayout stackTop;
        StackLayout stackCountries;
        StackLayout stackMetros;

        public event EventHandler SelectionChanged;

        public CommunitySelection Selection
        {
            get;
            private set;
        }

        public bool NameAsCommunity
        {
            get
            {
                return this.nameAsCommunity;
            }
            set
            {
                this.nameAsCommunity = value;
                this.title.Text = this.NameAsCommunity ? "Pick the Community" : "Pick From";
            }
        }
        private bool nameAsCommunity;

        public bool AllowFriendsSelection
        {
            get;
            set;
        }

		public bool DoNotCheckSelection
		{
			get;
			set;
		}

		public PickCommunityPage()
		{
            this.myAthlete = App.Repository.GetMyAthlete();
            this.myCountry = Country.Get(myAthlete.Country);

            // title
            this.title = new BybTitle("") { BackgroundColor = Config.ColorBlackBackground };
            this.NameAsCommunity = true;

            // ok, cancel
            Button buttonCancel = new BybButton() { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += buttonCancel_Clicked;

            // stacks
            this.stackTop = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Config.ColorBackground,
                Spacing = 1,
            };
            this.stackCountries = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Config.ColorBackground,
                Spacing = 1,
            };
            this.stackMetros = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Config.ColorBackground,
                Spacing = 1,
            };

            // content
            Grid grid = new Grid()
            {
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(Config.TitleHeight, GridUnitType.Absolute) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding + Config.OkCancelButtonsPadding, GridUnitType.Absolute) },
                },
            };
            grid.Children.Add(title, 0, 0);
            grid.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    //buttonOk,
                }
            }, 0, 2);
            grid.Children.Add(new ScrollView()
            {
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(0),
                Content = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    Spacing = 0,
                    Padding = new Thickness(0),
                    Children =
                    {
                        this.stackTop,
                        this.createDivider("Countries"),
                        this.stackCountries,
                        this.createDivider("Cities"),
                        this.stackMetros,
                    }
                }
            }, 0, 1);
            this.BackgroundColor = Config.ColorBlackBackground;
            this.Padding = new Thickness(0,0,0,0);
            this.Content = grid;
        }

        public async Task Initialize(CommunitySelection selection)
        {
            this.Selection = selection;

            this.fillTop();
            this.fillCountries(false);
            await this.fillMetros();
        }

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        void fillTop()
        {
            this.stackTop.Children.Clear();

            // friends
            if (AllowFriendsSelection)
                this.stackTop.Children.Add(this.createItem(CommunitySelection.CreateFriendsOnly()));

            // planet Earth
            this.stackTop.Children.Add(this.createItem(CommunitySelection.CreateAsPlanetEarth()));

            // your city
            if (myAthlete.MetroID > 0)
            {
                var myMetro = App.Cache.Metroes.Get(myAthlete.MetroID);
                if (myMetro == null)
                    myMetro = new MetroWebModel() { ID = myAthlete.MetroID, Name = "Your city", Country = myCountry != null ? myCountry.ThreeLetterCode : "?" };
                this.stackTop.Children.Add(this.createItem(CommunitySelection.CreateAsMetro(myMetro)));
            }
        }

        void fillCountries(bool showAll)
        {
            // list of countries
            List<Country> listOfCountries;
            if (showAll)
            {
                listOfCountries = Country.ListWithoutImportance0.ToList();
                if (myCountry != null)
                    listOfCountries.Insert(0, myCountry);
            }
            else
            {
                listOfCountries = Country.List.Where(i => i.Snooker == CountryImportanceEnum.Importance9).ToList();
                if (this.Selection != null && this.Selection.Country != null && listOfCountries.Contains(this.Selection.Country) == false)
                    listOfCountries.Insert(0, this.Selection.Country);
                if (myCountry != null && listOfCountries.Contains(myCountry) == false)
                    listOfCountries.Insert(0, myCountry);
            }

            // fill the stack
            this.stackCountries.Children.Clear();
            foreach (var country in listOfCountries)
                this.stackCountries.Children.Add(this.createItem(CommunitySelection.CreateAsCountry(country)));

            if (showAll == false)
            {
                var labelShowAll = new BybLabel()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HeightRequest = itemHeight,
					FontSize = Config.LargerFontSize,
                    TextColor = Color.White,
                    Text = "More countries...",
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                };
                this.stackCountries.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Config.ColorBlackBackground,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(15, 0, 0, 0),
                    HeightRequest = itemHeight,
                    Children =
                    {
                        labelShowAll
                    }
                });
                labelShowAll.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() => { this.fillCountries(true); })
                });
            }
        }

        async Task fillMetros()
        {
            this.stackMetros.Children.Clear();

            if (this.Selection == null || this.Selection.Country == null)
            {
                this.stackMetros.Children.Add(this.createInfoLabel("Cities populate when the country is picked", false));
                return;
            }

            // load metros
            this.stackMetros.Children.Add(this.createInfoLabel("Loading cities...", false));
            var metros = await App.WebService.GetMetros(this.Selection.Country.ThreeLetterCode);
            if (metros == null)
            {
                this.stackMetros.Children.Clear();
                this.stackMetros.Children.Add(this.createInfoLabel("Couldn't load cities. Internet issues?", false));
                return;
            }

            // save metros to cache
            App.Cache.Metroes.Put(metros);

            // fill metros
            metros = (from i in metros
                      orderby i.Name
                      select i).ToList();
            this.stackMetros.Children.Clear();
            foreach (var metro in metros)
                this.stackMetros.Children.Add(this.createItem(CommunitySelection.CreateAsMetro(metro)));
        }

        StackLayout createDivider(string text)
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(15, 0, 0, 0),
                HeightRequest = itemHeight,
                Children =
                {
                    new BybLabel()
                    {
                        Text = text,
						FontSize = Config.LargerFontSize,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Config.ColorTextOnBackgroundGrayed,
                    }
                }
            };
        }

        StackLayout createInfoLabel(string text, bool error)
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Padding = new Thickness(15, 0, 0, 0),
                HeightRequest = itemHeight,
                Children =
                {
                    new BybLabel()
                    {
                        Text = text,
						FontSize = Config.LargerFontSize,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = error ? Color.Red : Color.White,
                    }
                }
            };
        }

        StackLayout createItem(CommunitySelection item)
        {
            StackLayout stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HeightRequest = itemHeight,
                Padding = new Thickness(15,0,15,0),
            };

			if (DoNotCheckSelection == false && Selection != null && 
				item.Country == Selection.Country && item.MetroID == Selection.MetroID && item.IsFriendsOnly == Selection.IsFriendsOnly)
            {
                var image = new Image()
                {
                    Source = new FileImageSource() { File = "checkmarkRed.png" },
                    HeightRequest = itemHeight * 0.4,
                    WidthRequest = itemHeight * 0.4,
                };
                stack.Children.Add(image);
            }

            stack.Children.Add(new BybLabel()
            {
				FontSize = Config.LargerFontSize,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                BackgroundColor = Config.ColorBlackBackground,
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalTextAlignment = TextAlignment.Center,
                Text = item.ToString(),
            });

            stack.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    this.Selection = item;
                    await App.Navigator.NavPage.Navigation.PopModalAsync();
                    if (this.SelectionChanged != null)
                        this.SelectionChanged(this, EventArgs.Empty);
                })
            });

            return stack;
        }
	}
}
