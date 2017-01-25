//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Xamarin.Forms;

//namespace Awpbs.Mobile
//{
//	public class AddPointsPage : ContentPage
//	{
//        public event EventHandler Done;

//        public SnookerBreak SnookerBreak
//        {
//            get;
//            private set;
//        }

//        BybLargeEntry2 entryPoints;

//        public AddPointsPage(SnookerMatchMetadata metadata, bool isOpponentsBreak)
//        {
//            this.SnookerBreak = new SnookerBreak();
//            this.SnookerBreak.Date = DateTime.Now;
//            this.SnookerBreak.AthleteID = isOpponentsBreak ? metadata.OpponentAthleteID : metadata.PrimaryAthleteID;
//            this.SnookerBreak.AthleteName = isOpponentsBreak ? metadata.OpponentAthleteName : metadata.PrimaryAthleteName;
//            this.SnookerBreak.OpponentAthleteID = !isOpponentsBreak ? metadata.OpponentAthleteID : metadata.PrimaryAthleteID;
//            this.SnookerBreak.OpponentName = !isOpponentsBreak ? metadata.OpponentAthleteName : metadata.PrimaryAthleteName;
//            this.SnookerBreak.VenueID = metadata.VenueID;
//            this.SnookerBreak.VenueName = metadata.VenueName;
//            this.SnookerBreak.TableSize = metadata.TableSize;

//            this.entryPoints = new BybLargeEntry2()
//            {
//                KeepOnlyNumbers = true,
//                WidthRequest = 100,
//                Keyboard = Keyboard.Numeric,
//                BackgroundColor = Color.Transparent,
//                VerticalOptions = LayoutOptions.Center,
//                HorizontalOptions = LayoutOptions.Center,
//                Placeholder = "--",
//                Text = null
//            };

//            Button buttonOk1 = new Button()
//            {
//                Style = (Style)App.Current.Resources["LargeButtonStyle"],
//                Text = "Add points",
//            };
//            Button buttonOk2 = new Button()
//            {
//                Style = (Style)App.Current.Resources["LargeButtonStyle"],
//                Text = "Add as a foul",
//            };
//            Button buttonCancel = new Button()
//            {
//                Style = (Style)App.Current.Resources["BlackButtonStyle"],
//                Text = "Cancel",
//            };
//            buttonOk1.Clicked += buttonOk1_Clicked;
//            buttonOk2.Clicked += buttonOk2_Clicked;
//            buttonCancel.Clicked += buttonCancel_Clicked;

//            var grid = new Grid()
//            {
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                Padding = new Thickness(0),
//                RowDefinitions =
//                {
//                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
//                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
//                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
//                }
//            };

//            grid.Children.Add(new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                Padding = new Thickness(0,30,0,10),
//                Spacing = 5,
//                Children =
//                {
//                    new Label { Text = "Add Points For", Style = (Style)App.Current.Resources["Header1Style"], HorizontalOptions = LayoutOptions.Center, TextColor = Config.ColorTextOnBackground },
//                    //new Label { Text = "for", Style = (Style)App.Current.Resources["Header2Style"], HorizontalOptions = LayoutOptions.Center, TextColor = Config.ColorTextOnBackground },
//                    new Label { Text = string.IsNullOrEmpty(SnookerBreak.AthleteName) ? "Unknown Player" : SnookerBreak.AthleteName, Style = (Style)App.Current.Resources["Header1Style"], HorizontalOptions = LayoutOptions.Center, TextColor = Config.ColorTextOnBackground },
//                }
//            }, 0, 0);
//            grid.Children.Add(new Frame()
//            {
//                HorizontalOptions = LayoutOptions.Fill,
//                VerticalOptions = LayoutOptions.FillAndExpand,
//                BackgroundColor = Config.ColorDarkGrayBackground,
//                Padding = new Thickness(60,20,0,20),
//                Content = this.entryPoints,
//            }, 0, 1);
//            grid.Children.Add(new StackLayout()
//            {
//                Orientation = StackOrientation.Vertical,
//                Padding = new Thickness(10,10,10,10),
//                Spacing = 1,
//                Children =
//                {
//                    new StackLayout()
//                    {
//                        Orientation = StackOrientation.Horizontal,
//                        Spacing = 1,
//                        Children =
//                        {
//                            buttonOk2,
//                            buttonOk1,
//                        }
//                    },
//                    buttonCancel,
//                }
//            }, 0, 2);

//            this.Content = grid;
//            this.Padding = new Thickness(0, 0, 0, 0);
//		}

//        private void buttonOk2_Clicked(object sender, EventArgs e)
//        {
//            int points = 0;
//            if (!int.TryParse(this.entryPoints.Text, out points) || points <= 0 || points > 147)
//            {
//                DisplayAlert("Byb", "Enter proper points to add to the frame", "OK");
//                return;
//            }
//            if (points > 7)
//            {
//                DisplayAlert("Byb", "A foul cannot be larger than 7 points.", "OK");
//                return;
//            }
//            if (points < 4)
//            {
//                DisplayAlert("Byb", "A foul cannot be smaller than 4 points.", "OK");
//                return;
//            }
//            this.SnookerBreak.Points = points;
//            this.SnookerBreak.IsFoul = true;

//            if (this.Done != null)
//                this.Done(this, EventArgs.Empty);
//            App.Navigator.NavPage.Navigation.PopModalAsync();
//        }

//        private void buttonOk1_Clicked(object sender, EventArgs e)
//        {
//            int points = 0;
//            if (!int.TryParse(this.entryPoints.Text, out points) || points <= 0 || points > 147)
//            {
//                DisplayAlert("Byb", "Enter a proper points to add to the frame", "OK");
//                return;
//            }
//            this.SnookerBreak.Points = points;

//            if (this.Done != null)
//                this.Done(this, EventArgs.Empty);
//            App.Navigator.NavPage.Navigation.PopModalAsync();
//        }

//        private void buttonCancel_Clicked(object sender, EventArgs e)
//        {
//            App.Navigator.NavPage.Navigation.PopModalAsync();
//        }

//        protected override void OnAppearing()
//        {
//            base.OnAppearing();

//            this.entryPoints.Focus();
//        }
//    }
//}
