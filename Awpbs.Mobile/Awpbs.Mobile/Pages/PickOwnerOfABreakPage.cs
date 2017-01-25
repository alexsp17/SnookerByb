using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class PickOwnerOfABreakPage : ContentPage
    {
        public event EventHandler<int?> UserMadeSelection;

        SnookerMatchMetadata metadata;

        BybTitle title;
        Image imageYou;
        Image imageOpponent;
        Label labelYou;
        Label labelOpponent;

        public PickOwnerOfABreakPage(SnookerMatchMetadata metadata)
        {
            this.metadata = metadata;

            this.title = new BybTitle("Whose Break Is It?") { VerticalOptions = LayoutOptions.Start };

            double imageSize = Config.DeviceScreenHeightInInches < 4 ? 70 : 100;

            // images
            this.imageYou = new Image()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = App.ImagesService.GetImageSource(null)
            };
            this.imageOpponent = new Image()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Source = App.ImagesService.GetImageSource(null)
            };
            this.labelYou = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "You"
            };
            this.labelOpponent = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
            };
            Grid gridWithImages = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                BackgroundColor = Color.White,
            };
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Absolute) });
            gridWithImages.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(imageSize, GridUnitType.Absolute) });
            gridWithImages.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40, GridUnitType.Absolute) });
            gridWithImages.Children.Add(this.imageYou, 0, 1);
            gridWithImages.Children.Add(this.imageOpponent, 2, 1);
            gridWithImages.Children.Add(this.labelYou, 0, 2);
            gridWithImages.Children.Add(this.labelOpponent, 2, 2);

            this.imageYou.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { youClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.labelYou.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { youClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.imageOpponent.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { opponentClicked(); }),
                NumberOfTapsRequired = 1
            });
            this.labelOpponent.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(() => { opponentClicked(); }),
                NumberOfTapsRequired = 1
            });

            // cancel button
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "Cancel" };
            buttonCancel.Clicked += (s1, e1) =>
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, null);
            };
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorBackground,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel
                }
            };

            // content
            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Children = {
                    new ScrollView
                    {
                        Padding = new Thickness(0),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Content = new StackLayout
                        {
                            Padding = new Thickness(0),
                            Spacing = 0,
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                title,
                                gridWithImages
                            }
                        }
                    },

                    panelOkCancel
                }
            };
            Padding = new Thickness(0, 0, 0, 0);

            this.labelYou.Text = metadata.PrimaryAthleteName;
            this.imageYou.Source = App.ImagesService.GetImageSource(metadata.PrimaryAthletePicture);
            if (this.metadata.HasOpponentAthlete == false)
            {
                this.imageOpponent.Source = new FileImageSource() { File = "plus.png" };
                this.labelOpponent.Text = "Pick";
            }
            else
            {
                this.imageOpponent.Source = App.ImagesService.GetImageSource(this.metadata.OpponentPicture);
                this.labelOpponent.Text = this.metadata.OpponentAthleteName;
            }
        }

        void youClicked()
        {
            if (this.UserMadeSelection != null)
                this.UserMadeSelection(this, metadata.PrimaryAthleteID);
        }

        void opponentClicked()
        {
            if (this.metadata.OpponentAthleteID > 0)
            {
                if (this.UserMadeSelection != null)
                    this.UserMadeSelection(this, metadata.OpponentAthleteID);
            }
            else
            {
				if (App.Navigator.GetOpenedPage (typeof(PickAthletePage)) != null)
					return;
				
                var page = new PickAthletePage();
                page.UserMadeSelection += (s1, e1) =>
                {
                    this.Navigation.PopModalAsync();
                    if (e1 == null || e1.Person == null)
                        return;
                    int athleteID = e1.Person.ID;
                    if (athleteID == 0)
                        return;
                    if (athleteID == this.metadata.PrimaryAthleteID)
                    {
                        this.DisplayAlert("Byb", "Cannot play against yourself", "OK");
                        return;
                    }
                    App.Cache.People.Put(e1.Person);
                    if (this.UserMadeSelection != null)
                        this.UserMadeSelection(this, athleteID);
                };
				App.Navigator.NavPage.Navigation.PushModalAsync(page);
            }
        }
    }
}
