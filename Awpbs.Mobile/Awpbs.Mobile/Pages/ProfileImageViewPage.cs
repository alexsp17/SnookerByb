using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Awpbs.Mobile
{
	public class ProfileImageViewPage : ContentPage
	{
        Image imageLarge;

        public ProfileImageViewPage(PersonFullWebModel person)
		{
            /// ok
            /// 
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK" };
            buttonOk.Clicked += buttonOk_Clicked;
            var panelOk = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorDarkGrayBackground,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Children =
                {
                    buttonOk,
                }
            };

            /// Images
            /// 

            imageLarge = new Image() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            /// content
            /// 
            Title = person.Name;
            this.Padding = new Thickness(0);
            this.BackgroundColor = Config.ColorGrayBackground;
            var grid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                }
            };
            grid.Children.Add(imageLarge, 0, 0);
            this.Content = grid;

            imageLarge.Source = App.ImagesService.GetImageSource(person.Picture, BackgroundEnum.Background1, true);
		}

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
            await App.Navigator.NavPage.Navigation.PopAsync();
        }
	}
}
