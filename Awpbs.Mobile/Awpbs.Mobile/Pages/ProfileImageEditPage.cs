using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Awpbs.Mobile
{
	public class ProfileImageEditPage : ContentPage
	{
        Athlete myAthlete;

        Image imageSmall;
        Image imageLarge;
        Label labelWait;

        public ProfileImageEditPage()
		{
            this.myAthlete = App.Repository.GetMyAthlete();

            /// ok
            /// 
            Button buttonOk = new BybButton { Text = "Close", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonOk.Clicked += buttonOk_Clicked;
            var panelOk = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorDarkGrayBackground,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding*2,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Children =
                {
                    buttonOk,
                }
            };

            /// Other buttons
            /// 
            var buttonUseFacebookPicture = new BybButton() { Text = "Facebook", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            buttonUseFacebookPicture.Clicked += buttonUseFacebookPicture_Clicked;
            var buttonTakePicture = new BybButton() { Text = "Take", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            buttonTakePicture.Clicked += buttonTakePicture_Clicked;
            var buttonSelectPicture = new BybButton() { Text = "Select", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            buttonSelectPicture.Clicked += buttonSelectPicture_Clicked;
            var buttonDeletePicture = new BybButton() { Text = "x", Style = (Style)App.Current.Resources["LargeButtonStyle"] };
            buttonDeletePicture.Clicked += buttonDeletePicture_Clicked;
            var panelButtons = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                //BackgroundColor = Config.ColorDarkGrayBackground,
                HorizontalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding, Config.OkCancelButtonsPadding, Config.OkCancelButtonsPadding, 0),
                Spacing = 1,//Config.OkCancelButtonsPadding,
                Children =
                {
                    buttonUseFacebookPicture,
                    buttonTakePicture,
                    buttonSelectPicture,
                    buttonDeletePicture
                }
            };

            /// Images
            /// 

            imageSmall = new Image() { WidthRequest = Config.MyImageSize, HeightRequest = Config.MyImageSize, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            imageLarge = new Image() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand };
            labelWait = new BybLabel() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "loading...", IsVisible = false };

            /// content
            /// 
            Title = "Picture";
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
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(Config.MyImageSize + 20, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(Config.OkCancelButtonsHeight + Config.OkCancelButtonsPadding*2, GridUnitType.Absolute) },
                },
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                }
            };
            grid.Children.Add(new BybTitle("Picture"), 0, 0);
            grid.Children.Add(new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    BackgroundColor = Config.ColorBackground,
                    Padding = new Thickness(0,0,0,0),
                    Children =
                    {
                        imageSmall
                    }
                }, 0, 1);
            grid.Children.Add(imageLarge, 0, 2);
            grid.Children.Add(labelWait, 0, 2);
            grid.Children.Add(panelButtons, 0, 3);
            grid.Children.Add(panelOk, 0, 4);
            this.Content = grid;

            this.updateImages();
		}

        void switchWaitState(bool isWaiting)
        {
            if (isWaiting)
            {
                this.labelWait.IsVisible = true;
                this.imageLarge.Opacity = 0.25;
            }
            else
            {
                this.labelWait.IsVisible = false;
                this.imageLarge.Opacity = 1.0;
            }
        }

        async void buttonUseFacebookPicture_Clicked(object sender, EventArgs e)
        {
            this.switchWaitState(true);

            string pictureUrl = await App.WebService.UseFacebookPicture(true);
            if (string.IsNullOrEmpty(pictureUrl))
            {
                this.updateImages();
                this.switchWaitState(false);
                await DisplayAlert("Byb", "Error. Couldn't talk to the server. Internet issues?", "OK");
                return;
            }

            this.myAthlete = App.Repository.GetMyAthlete();
            this.myAthlete.Picture = pictureUrl;
            App.Repository.UpdateAthlete(this.myAthlete);

            this.updateImages();
            this.switchWaitState(false);
        }

        async void buttonDeletePicture_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.myAthlete.Picture) == false)
            {
                if (await DisplayAlert("Byb", "Delete the picture?", "Delete", "Cancel") != true)
                    return;
            }

            this.switchWaitState(true);

            string pictureUrl = await App.WebService.UseFacebookPicture(false);
            if (pictureUrl == null)
            {
                this.updateImages();
                this.switchWaitState(false);
                await DisplayAlert("Byb", "Error. Couldn't talk to the server. Internet issues?", "OK");
                return;
            }

            this.myAthlete = App.Repository.GetMyAthlete();
            this.myAthlete.Picture = pictureUrl;
            App.Repository.UpdateAthlete(this.myAthlete);

            this.updateImages();
            this.switchWaitState(false);
        }

        async void uploadPicture(System.IO.Stream imageStream)
        {
            this.switchWaitState(true);

            if (imageStream.Length > 1*1024*1024) // resize anything bigger than 1Mb
            {
                imageStream = ImageResizer.ResizeImage(imageStream, 500);
                if (imageStream == null)
                {
                    this.switchWaitState(false);
                    await DisplayAlert("Byb", "Error. Couldn't properly scale the picture.", "OK");
                    return;
                }
            }

            string pictureUrl = await App.WebService.UploadPicture(imageStream);
            if (string.IsNullOrEmpty(pictureUrl))
            {
                this.updateImages();
                this.switchWaitState(false);
                await DisplayAlert("Byb", "Error. Couldn't talk to the server. Internet issues?", "OK");
                return;
            }

            this.myAthlete = App.Repository.GetMyAthlete();
            this.myAthlete.Picture = pictureUrl;
            App.Repository.UpdateAthlete(this.myAthlete);

            this.updateImages();
            this.switchWaitState(false);
        }

        IMediaPicker getMediaPicker()
        {
            var device = Resolver.Resolve<IDevice>();
            var mediaPicker = DependencyService.Get<XLabs.Platform.Services.Media.IMediaPicker>();
            if (mediaPicker == null)
                mediaPicker = device.MediaPicker;
            return mediaPicker;
        }
        
        async void buttonSelectPicture_Clicked(object sender, EventArgs e)
        {
            System.IO.Stream imageStream = null; 
            bool error = false;
            try
            {
                var mediaFile = await getMediaPicker().SelectPhotoAsync(
                    new CameraMediaStorageOptions {
                        DefaultCamera = CameraDevice.Front,
                        MaxPixelDimension = 400
                    });
                imageStream = mediaFile.Source;
            }
            catch (Exception)
            {
                error = true;
            }
            if (error)
            {
                return;
            }
            if (imageStream == null)
                return;

            uploadPicture(imageStream);
        }

        async void buttonTakePicture_Clicked(object sender, EventArgs e)
        {
            System.IO.Stream imageStream = null;
            bool error = false;
            try
            {
                var mediaFile = await getMediaPicker().TakePhotoAsync(
                    new CameraMediaStorageOptions {
                        DefaultCamera = CameraDevice.Front,
                        MaxPixelDimension = 400
                    });
                imageStream = mediaFile.Source;
            }
            catch (Exception)
            {
                error = true;
            }
            if (error)
            {
                return;
            }
            if (imageStream == null)
                return;

            uploadPicture(imageStream);
        }

        async void buttonOk_Clicked(object sender, EventArgs e)
        {
            await App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        void updateImages()
        {
            imageSmall.Source = App.ImagesService.GetImageSource(myAthlete.Picture, BackgroundEnum.Background1, false);

            if (string.IsNullOrEmpty(myAthlete.Picture))
                imageLarge.Source = null;
            else
                imageLarge.Source = App.ImagesService.GetImageSource(myAthlete.Picture, BackgroundEnum.Background1, true);
        }
	}
}
