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
	public class VoicePreferencesPage : ContentPage
	{
        Slider sliderRate;
        Slider sliderPitch;
        Button buttonDefaultRateAndPitch;
        Button buttonTry;
        BybNoBorderPicker pickerVoice;

        List<Voice> voices;

        public VoicePreferencesPage()
		{
            /// ok, cancel
            /// 
            Button buttonOk = new BybButton { Style = (Style)App.Current.Resources["LargeButtonStyle"], Text = "OK" };
            Button buttonCancel = new BybButton { Style = (Style)App.Current.Resources["BlackButtonStyle"], Text = "Cancel" };
            buttonOk.Clicked += buttonOk_Clicked;
            buttonCancel.Clicked += buttonCancel_Clicked; ;
            var panelOkCancel = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorGrayBackground,//Config.ColorDarkGrayBackground,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = Config.OkCancelButtonsHeight,
                Padding = new Thickness(Config.OkCancelButtonsPadding),
                Spacing = 1,
                Children =
                {
                    buttonCancel,
                    buttonOk,
                }
            };

            /// stack
            /// 

            this.pickerVoice = new BybNoBorderPicker()
            {
                BackgroundColor = Color.White,
                HeightRequest = 30,
            };
            this.pickerVoice.Items.Add("Default");

            this.voices = App.ScorePronouncer.GetVoices().ToList();
            foreach (var voice in voices)
                this.pickerVoice.Items.Add(voice.ToString());
            var voiceInPreferences = App.UserPreferences.Voice;
            var voiceToSelect = (from i in voices
                                 where i.ToString() == voiceInPreferences
                                 select i).FirstOrDefault();
            if (voiceToSelect == null)
                this.pickerVoice.SelectedIndex = 0;
            else
                this.pickerVoice.SelectedIndex = voices.IndexOf(voiceToSelect) + 1;

            sliderRate = new Slider()
            {
				Minimum = UserPreferences.MinVoiceRate,
				Maximum = UserPreferences.MaxVoiceRate,
                Value = App.UserPreferences.VoiceRate
            };

            sliderPitch = new Slider()
            {
				Minimum = UserPreferences.MinVoicePitch,
				Maximum = UserPreferences.MaxVoicePitch,
                Value = App.UserPreferences.VoicePitch
            };

            buttonDefaultRateAndPitch = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                Text = "Default rate and pitch",
            };
            buttonDefaultRateAndPitch.Clicked += buttonDefaultRateAndPitch_Clicked;
            buttonTry = new BybButton()
            {
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                WidthRequest = 100,
                Text = "Try"
            };
            buttonTry.Clicked += buttonTry_Clicked;

            StackLayout stack = new StackLayout()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(20,20,20,20),
                Children =
                {
					new BybLabel { Text = "Voice & language", TextColor = Config.ColorBlackTextOnWhite, },
                    pickerVoice,
					new BybLabel { Text = "Rate", TextColor = Config.ColorBlackTextOnWhite, },
                    sliderRate,
					new BybLabel { Text = "Pitch", TextColor = Config.ColorBlackTextOnWhite, },
                    sliderPitch,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Center,
                        Spacing = 20,
                        Padding = new Thickness(0,20,0,0),
                        Children =
                        {
                            buttonDefaultRateAndPitch,
                            buttonTry
                        }
                    }
                    
                }
            };

            /// content
            /// 
            this.Padding = new Thickness(0);
            this.BackgroundColor = Config.ColorGrayBackground;
            var grid = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                ColumnSpacing = 0,
                RowSpacing = 0,
                Padding = new Thickness(0),
                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
                },
                ColumnDefinitions = 
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
                }
            };
            grid.Children.Add(new BybTitle("Voice Configuration"), 0, 0);
            grid.Children.Add(stack, 0, 1);
            grid.Children.Add(panelOkCancel, 0, 2);
            this.Content = grid;
		}

        private void buttonDefaultRateAndPitch_Clicked(object sender, EventArgs e)
        {
            App.UserPreferences.VoiceRate = UserPreferences.DefaultVoiceRate;
            App.UserPreferences.VoicePitch = UserPreferences.DefaultVoicePitch;

            this.sliderRate.Value = App.UserPreferences.VoiceRate;
            this.sliderPitch.Value = App.UserPreferences.VoicePitch;
        }

        private void buttonTry_Clicked(object sender, EventArgs e)
        {
            Voice voice = null;
            if (this.pickerVoice.SelectedIndex > 0)
                voice = voices[this.pickerVoice.SelectedIndex - 1];

			App.ScorePronouncer.Pronounce("147", voice != null ? voice.FullName : null, this.sliderRate.Value, this.sliderPitch.Value);
        }

        private void buttonCancel_Clicked(object sender, EventArgs e)
        {
            App.Navigator.NavPage.Navigation.PopModalAsync();
        }

        private void buttonOk_Clicked(object sender, EventArgs e)
        {
            if (this.pickerVoice.SelectedIndex == 0)
                App.UserPreferences.Voice = "";
            else
                App.UserPreferences.Voice = this.voices[this.pickerVoice.SelectedIndex - 1].ToString();

            App.UserPreferences.VoiceRate = this.sliderRate.Value;
            App.UserPreferences.VoicePitch = this.sliderPitch.Value;
            App.UserPreferences.IsVoiceOn = true;

            App.Navigator.NavPage.Navigation.PopModalAsync();
        }
    }
}
