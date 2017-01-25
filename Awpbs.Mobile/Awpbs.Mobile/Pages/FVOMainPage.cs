using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class FVOMainPage : ContentPage
	{
        double imageSize = 150;

        Label labelTitle;
        Label labelSyncStatus;

        // selected players
        BybPersonImage imageA;
        Label labelA;
        BybPersonImage imageB;
        Label labelB;

        // start/reset buttons
        Button buttonStartMatch;
        Button buttonReset;

        // bottom panel
        Button buttonSettings;
        Button buttonHistory;

        // picking athletes panel
        BybButtonWithNumber buttonExisting;
        BybButtonWithNumber buttonRegister;
        FindPeopleControl findPeopleControl;
        FVORegisterControl registerControl;

        PersonBasicWebModel personA;
        PersonBasicWebModel personB;

        public enum PickingAthleteStatusEnum
        {
            Existing = 0,
            Register = 1,
        }

        public PickingAthleteStatusEnum PickingAthleteStatus
        {
            get
            {
                if (this.registerControl.IsVisible)
                    return PickingAthleteStatusEnum.Register;
                return PickingAthleteStatusEnum.Existing;
            }
            set
            {
                this.findPeopleControl.IsVisible = value == PickingAthleteStatusEnum.Existing;
                this.registerControl.IsVisible = value == PickingAthleteStatusEnum.Register;

                this.buttonExisting.IsSelected = value == PickingAthleteStatusEnum.Existing;
                this.buttonRegister.IsSelected = value == PickingAthleteStatusEnum.Register;
            }
        }

        public enum ActivePlayerStatusEnum
        {
            None,
            PlayerA,
            PlayerB,
        }

        private ActivePlayerStatusEnum activePlayerStatus = ActivePlayerStatusEnum.PlayerA;

        public FVOMainPage()
        {
            this.labelTitle = new BybLabel()
            {
                Text = "Pick Opponents",
                TextColor = Config.ColorTextOnBackgroundGrayed,
                FontSize = Config.VeryLargeFontSize,
            };

            this.labelSyncStatus = new BybLabel()
            {
                Text = "",
                TextColor = Config.ColorTextOnBackground,
                FontSize = Config.LargerFontSize,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
            };

            /// selected players
            /// 

            // player A
            imageA = new BybPersonImage()
            {
                WidthRequest = imageSize,
                HeightRequest = imageSize,
                HorizontalOptions = LayoutOptions.End,//.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,//.FillAndExpand,
                BackgroundColor = Config.ColorBlackBackground,
            };
            imageA.SetImagePickOpponent();
            labelA = new BybLabel()
            {
                Text = "",
                TextColor = Config.ColorTextOnBackground,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            var panelA = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.End,
                Padding = new Thickness(0),
                Spacing = 0,
                Children =
                {
                    imageA,
                    labelA,
                }
            };
            panelA.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.panelA_Clicked(); }) });

            // player B
            imageB = new BybPersonImage()
            {
                WidthRequest = imageSize,
                HeightRequest = imageSize,
                HorizontalOptions = LayoutOptions.Start,//.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,//.FillAndExpand,
                BackgroundColor = Config.ColorBlackBackground,
            };
            imageB.SetImagePickOpponent();
            labelB = new BybLabel()
            {
                Text = "Select Player 2",
                TextColor = Config.ColorTextOnBackground,
                HeightRequest = 50,
                HorizontalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
            };
            var panelB = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Config.ColorBlackBackground,
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(0),
                Spacing = 0,
                Children =
                {
                    imageB,
                    labelB,
                }
            };
            panelB.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.panelB_Clicked(); }) });

            Grid gridSelectedPlayers = new Grid()
            {
                //BackgroundColor = Color.Red,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Padding = new Thickness(0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(0.01, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1.00, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(0.01, GridUnitType.Star) },
                }
            };
            gridSelectedPlayers.Children.Add(panelA, 1, 0);
            gridSelectedPlayers.Children.Add(new Frame()
            {
                Padding = new Thickness(0,0,0,50),
                HasShadow = false,
                BackgroundColor = Color.Transparent,
                Content = new BybLabel()
                {
                    Text = "vs.",
                    FontSize = Config.VeryLargeFontSize + 20,
                    TextColor = Config.ColorTextOnBackgroundGrayed,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                }
            }, 2, 0);
            gridSelectedPlayers.Children.Add(panelB, 3, 0);

            /// start/reset buttons
            /// 
            this.buttonStartMatch = new BybButton()
            {
                Text = "Start Match",
                Style = (Style)App.Current.Resources["LargeButtonStyle"]
            };
            buttonStartMatch.Clicked += buttonStartMatch_Clicked;
            this.buttonReset = new BybButton()
            {
                Text = "Reset",
                Style = (Style)App.Current.Resources["BlackButtonStyle"]
            };
            buttonReset.Clicked += buttonReset_Clicked;

            /// bottom panel
            /// 
            this.buttonSettings = new BybButton()
            {
                Text = "Settings",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                TextColor = Config.ColorTextOnBackground,
                VerticalOptions = LayoutOptions.Center,
            };
            buttonSettings.Clicked += buttonSettings_Clicked;
            this.buttonHistory = new BybButton()
            {
                Text = "History",
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                TextColor = Config.ColorTextOnBackground,
                VerticalOptions = LayoutOptions.Center,
            };
            this.buttonHistory.Clicked += buttonHistory_Clicked;
            //this.buttonHistory = new BybLabel()
            //{
            //    Text = "History",
            //    TextColor = Config.ColorTextOnBackground,
            //    VerticalOptions = LayoutOptions.Center,
            //    WidthRequest = 100,
            //    HeightRequest = 40,
            //    HorizontalTextAlignment = TextAlignment.End,
            //    VerticalTextAlignment = TextAlignment.Center,
            //};
            Label labelInfo = new BybLabel()
            {
                Text = "Tip: Install 'Snooker Byb' app on your personal mobile device.",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Config.ColorGrayTextOnWhite,
            };
            StackLayout panelBottom = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = Config.TitleHeight,
                Padding = new Thickness(20,0,20,0),
                BackgroundColor = Config.ColorBackground,
                Children =
                {
                    buttonSettings,
                    labelInfo,
                    buttonHistory,
                }
            };

            /// picking athletes panel
            /// 

            // tab buttons
            this.buttonExisting = new BybButtonWithNumber("Existing") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight };
            this.buttonExisting.Clicked += (s1, e1) =>
            {
                this.registerControl.Clear();
                this.PickingAthleteStatus = PickingAthleteStatusEnum.Existing;
            };
            this.buttonRegister = new BybButtonWithNumber("Register") { IsNumberVisible = false, HeightRequest = Config.OkCancelButtonsHeight };
            this.buttonRegister.Clicked += (s1, e1) =>
            {
                if (this.alertAboutSettingsIfNecessary())
                    return;
                this.PickingAthleteStatus = PickingAthleteStatusEnum.Register;
            };

            Grid panelPickingAthletes = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Config.ColorBackground,
            };

            this.findPeopleControl = new FindPeopleControl(false);
            this.findPeopleControl.UserClickedOnPerson += findPeopleControl_UserClickedOnPerson;
            this.findPeopleControl.BackgroundColor = Config.ColorBackground;
            this.findPeopleControl.Padding = new Thickness(0, 60, 10, 0);

            this.registerControl = new FVORegisterControl();
            this.registerControl.UserClickedCancel += registerControl_UserClickedCancel;
            this.registerControl.UserRegistered += registerControl_UserRegistered;
            this.registerControl.Padding = new Thickness(20, 80, 20, 20);

            panelPickingAthletes.Children.Add(this.findPeopleControl);
            panelPickingAthletes.Children.Add(this.registerControl);

            panelPickingAthletes.Children.Add(new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, 0, 0, 0),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start,
                Children =
                {
                    this.buttonExisting,
                    this.buttonRegister,
                }
            });

            /// layout
            /// 
            Grid panelRoot = new Grid()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(0),
                ColumnSpacing = 0,
                RowSpacing = 0,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(Config.TitleHeight, GridUnitType.Absolute) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(Config.TitleHeight - 20, GridUnitType.Absolute) },
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) },
                }
            };
            panelRoot.Children.Add(
                new BybLabel()
                {
                    Text = "Snooker Byb",
                    FontSize = Config.VeryLargeFontSize,
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }, 0, 2, 0, 1);
            panelRoot.Children.Add(
                new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.End,
                    Padding = new Thickness(0,0,20,0),
                    Children =
                    {
                        this.labelSyncStatus
                    }
                }, 0, 2, 0, 1);
            panelRoot.Children.Add(
                new BoxView()
                {
                    BackgroundColor = Config.ColorBackground,
                    HeightRequest = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.End,
                }, 0, 2, 0, 1);
            panelRoot.Children.Add(new Frame()
            {
                HasShadow = false,
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(40, 40, 0, 0),
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Content = labelTitle
            }, 0, 1);
            panelRoot.Children.Add(panelBottom, 0, 2, 2, 3);
            panelRoot.Children.Add(
                new BoxView()
                {
                    BackgroundColor = Config.ColorBlackBackground,
                    HeightRequest = 2,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Start,
                }, 0, 2, 2, 3);
            panelRoot.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.End,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Padding = new Thickness(20,0,20,20),
                Spacing = 0,
                Children =
                {
                    buttonReset,
                    buttonStartMatch,
                }
            }, 0, 1);
            panelRoot.Children.Add(gridSelectedPlayers, 0, 1);
            panelRoot.Children.Add(panelPickingAthletes, 1, 1);

            this.BackgroundColor = Config.ColorBlackBackground;
            this.Content = panelRoot;
            NavigationPage.SetHasNavigationBar(this, false);

            this.fill();
            this.fillPickingAthletesPanel();

            this.PickingAthleteStatus = PickingAthleteStatusEnum.Existing;

            App.Sync.StatusChanged += sync_StatusChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

			this.findPeopleControl.ReloadAsync(this.findPeopleControl.CurrentCommunity, false);
        }

        private void sync_StatusChanged(object sender, SyncStatus e)
        {
            if (e.Error)
            {
                this.labelSyncStatus.Text = "Sync Error. Internet Issues?";
                this.labelSyncStatus.IsVisible = true;
                this.labelSyncStatus.TextColor = Config.ColorRed;
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    this.labelSyncStatus.IsVisible = false;
                    return false;
                });
                return;
            }

            this.labelSyncStatus.TextColor = Config.ColorTextOnBackground;

            if (e.Completed)
            {
                this.labelSyncStatus.Text = "Sync completed";
                this.labelSyncStatus.IsVisible = true;
                Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                {
                    this.labelSyncStatus.IsVisible = false;
                    return false;
                });
                return;
            }

            string text = "Sync in progress";
            if (e.TotalCount > 0)
                text += " " + e.Current.ToString() + " / " + e.TotalCount.ToString();
            else
                text += "...";
            this.labelSyncStatus.Text = text;
            this.labelSyncStatus.IsVisible = true;
        }

        private void registerControl_UserRegistered(PersonBasicWebModel obj)
        {
            this.userTapped(obj, true);
            this.registerControl.Clear();
            this.PickingAthleteStatus = PickingAthleteStatusEnum.Existing;
        }

        private void registerControl_UserClickedCancel()
        {
            this.PickingAthleteStatus = PickingAthleteStatusEnum.Existing;
        }

        private void findPeopleControl_UserClickedOnPerson(object sender, SelectedPersonEventArgs e)
        {
            if (e.Person == null)
                return;
            userTapped(e.Person, false);
        }

        async void userTapped(PersonBasicWebModel person, bool justRegistered)
        {
            if (string.IsNullOrEmpty(person.Name))
                return;
            if (this.alertAboutSettingsIfNecessary())
                return;

            if (person == personA)
            {
                this.activePlayerStatus = ActivePlayerStatusEnum.PlayerA;
                this.fillPickingAthletesPanel();
                return;
            }

            if (person == personB)
            {
                this.activePlayerStatus = ActivePlayerStatusEnum.PlayerB;
                this.fillPickingAthletesPanel();
                return;
            }

            if (justRegistered)
            {
                this.takePerson(person);
                return;
            }

            this.labelTitle.Text = "Please wait...";
            bool? hasPin = await App.WebService.HasPin(person.ID);
            this.labelTitle.Text = "";

            if (hasPin == false)
            {
                await this.DisplayAlert(person.Name, "This account does not have a PIN yet. Set the PIN in the 'Snooker Byb' app on your personal mobile device (under the 'Profile' page). Meanwhile you can proceed without the PIN.", "OK");
                this.takePerson(person);
                return;
            }

            if (hasPin == null)
            {
                await this.DisplayAlert("Byb", "No internet connection?", "OK");
                return;
            }

            EnterPinPage enterPinPage = new EnterPinPage(true);
            enterPinPage.TheTitle = person.Name;
            await this.Navigation.PushModalAsync(enterPinPage);
            enterPinPage.UserClickedCancel += () =>
            {
                this.Navigation.PopModalAsync();
            };
            enterPinPage.UserEnteredPin += async () =>
            {
                await this.Navigation.PopModalAsync();

                if (enterPinPage.IsPinOk == false)
                    return;

                this.labelTitle.Text = "Please wait...";
                bool? verified = await App.WebService.VerifyPin(person.ID, enterPinPage.Pin);
                this.labelTitle.Text = "";
                if (verified == false)
                {
                    await this.DisplayAlert("Byb", "Incorrect PIN", "OK");
                }
				else if (verified == null && App.WebService.IsLastExceptionDueToInternetIssues)
				{
					await this.DisplayAlert("Byb", "Couldn't verify the PIN. Internet connection issues.", "OK");
				}
                else if (verified == null)
                {
                    await this.DisplayAlert("Byb", "Couldn't verify the PIN. Unspecified error.", "OK");
                }
                else
                {
                    this.takePerson(person);
                }
            };
        }

        void takePerson(PersonBasicWebModel person)
        {
            if (this.activePlayerStatus == ActivePlayerStatusEnum.PlayerA)
                this.personA = person;
            else
                this.personB = person;

            if (personA == null)
                this.activePlayerStatus = ActivePlayerStatusEnum.PlayerA;
            else if (personB == null)
                this.activePlayerStatus = ActivePlayerStatusEnum.PlayerB;
            else
                this.activePlayerStatus = ActivePlayerStatusEnum.None;

            this.fillPickingAthletesPanel();
        }

        private void buttonReset_Clicked(object sender, EventArgs e)
        {
            this.personA = null;
            this.personB = null;
            this.activePlayerStatus = ActivePlayerStatusEnum.PlayerA;
            this.fillPickingAthletesPanel();
        }

        private void buttonSettings_Clicked(object sender, EventArgs e)
        {
            FVOConfigPage page = new FVOConfigPage();
            this.Navigation.PushModalAsync(page);

            page.Disappearing += (s1, e1) =>
            {
                if (page.ChangesMade)
                {
                    var me = App.Repository.GetMyAthlete();
					this.findPeopleControl.ReloadAsync(CommunitySelection.CreateDefault(me), false);
                }
            };
        }

        private async void buttonHistory_Clicked(object sender, EventArgs e)
        {
            FVOHistoryPage page = new FVOHistoryPage();
            await this.Navigation.PushModalAsync(page);
            await page.Fill();
        }

        private void buttonStartMatch_Clicked(object sender, EventArgs e)
        {
            if (alertAboutSettingsIfNecessary())
                return;

            this.registerControl.Clear();

            FVOConfig config = FVOConfig.LoadFromKeyChain(App.KeyChain);
            if (config.IsOk == false)
                return;

            if (personA == null || personB == null)
            {
                this.DisplayAlert("Byb", "Select both players before starting the match.", "OK");
                return;
            }

            SnookerMatchMetadata metadata = new SnookerMatchMetadata();
            metadata.Date = DateTime.Now;
            if (this.personA != null)
            {
                metadata.PrimaryAthleteID = this.personA.ID;
                metadata.PrimaryAthleteName = this.personA.Name;
                metadata.PrimaryAthletePicture = this.personA.Picture;
            }
            if (this.personB != null)
            {
                metadata.OpponentAthleteID = this.personB.ID;
                metadata.OpponentAthleteName = this.personB.Name;
                metadata.OpponentPicture = this.personB.Picture;
            }
            metadata.TableSize = config.TableSize;
            metadata.VenueID = config.VenueID;
            metadata.VenueName = config.VenueName;

            RecordMatchPage page = new RecordMatchPage(metadata);
            this.Navigation.PushModalAsync(page);
            page.Disappearing += (s1, e1) =>
            {
                this.buttonReset_Clicked(this, EventArgs.Empty);
                this.PickingAthleteStatus = PickingAthleteStatusEnum.Existing;
            };
        }

        void panelA_Clicked()
        {
            if (alertAboutSettingsIfNecessary())
                return;

            this.activePlayerStatus = ActivePlayerStatusEnum.PlayerA;
            this.fillPickingAthletesPanel();
        }

        void panelB_Clicked()
        {
            if (alertAboutSettingsIfNecessary())
                return;

            this.activePlayerStatus = ActivePlayerStatusEnum.PlayerB;
            this.fillPickingAthletesPanel();
        }

        void fillPickingAthletesPanel()
        {
            this.labelA.Text = this.personA != null ? personA.Name : "";
            this.labelB.Text = this.personB != null ? personB.Name : "";

            this.imageA.Opacity = this.activePlayerStatus == ActivePlayerStatusEnum.PlayerB ? 0.5 : 1.0;
            this.imageB.Opacity = this.activePlayerStatus == ActivePlayerStatusEnum.PlayerA ? 0.5 : 1.0;

            if (personA != null)
                this.imageA.SetImage(personA.Name, personA.Picture);
            else
                this.imageA.SetImage("A", null);

            if (personB != null)
                this.imageB.SetImage(personB.Name, personB.Picture);
            else
                this.imageB.SetImage("B", null);

            switch (this.activePlayerStatus)
            {
                case ActivePlayerStatusEnum.PlayerA: this.labelTitle.Text = "Pick Player A"; break;
                case ActivePlayerStatusEnum.PlayerB: this.labelTitle.Text = "Pick Player B"; break;
                default: this.labelTitle.Text = "Ready to Start the Match"; break;
            }
        }

        void fill()
        {
            //FVOConfig config = FVOConfig.LoadFromKeyChain(App.KeyChain);
            //this.labelTitle.Text = config.VenueName ?? "";
            //this.buttonVenueName.Text = config.VenueName ?? "";
        }

        bool alertAboutSettingsIfNecessary()
        {
            FVOConfig config = FVOConfig.LoadFromKeyChain(App.KeyChain);
            if (config.IsOk == false)
            {
                this.DisplayAlert("Byb", "First, tap on 'Settings' in the bottom-left corner of the screen, to set things up.", "OK");
                return true;
            }
            return false;
        }
    }
}
