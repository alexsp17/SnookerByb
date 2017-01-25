using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Timers;

namespace Awpbs.Mobile
{
    public class CommunitySelectorControl : StackLayout
    {
        public event EventHandler SelectionChanged;

        public CommunitySelection Selection
        {
            get
            {
                return this.selection;
            }
            set
            {
                this.selection = value;
                if (this.labelCommunityName != null)
                    this.labelCommunityName.Text = this.selection.ToString();
                this.updateLabelLabel();
            }
        }
        private CommunitySelection selection;

        Label labelLabel;
        Label labelCommunityName;
		Label labelAskToTap;

        public bool NameAsCommunity
        {
            get
            {
                return this.nameAsCommunity;
            }
            set
            {
                this.nameAsCommunity = value;
                this.updateLabelLabel();
            }
        }
        private bool nameAsCommunity;

        public bool AllowFriendsSelection
        {
            get;
            set;
        }

        void updateLabelLabel()
        {
            if (this.labelLabel == null)
                return;

            if (this.NameAsCommunity == false && this.selection.IsFriendsOnly == false)
                this.labelLabel.Text = "From: ";
            else
                this.labelLabel.Text = "Community: ";
        }

		public void AnimateWithRed()
		{
			int timerCount = 0;
			Timer timer = new Timer();
			timer.Interval = 250;
			timer.Elapsed += (s1, e1) =>
			{
				timerCount++;
				if (timerCount > 9)
				{
					timer.Stop();
					timer.Dispose();
					return;
				}

				Device.BeginInvokeOnMainThread(() =>
					{
						if (timerCount % 2 == 0)
							this.labelCommunityName.TextColor = Color.Red;
						else
							this.labelCommunityName.TextColor = Color.Black;
					});
			};
			timer.Start();
		}

		public bool IsAskToTapVisible
		{
			get
			{
				return this.labelAskToTap.IsVisible == true;
			}
			set
			{
				this.labelAskToTap.IsVisible = value;
                if (value == true)
                    this.labelCommunityName.Text = "---";
			}
		}

        public CommunitySelectorControl()
        {
            this.Selection = CommunitySelection.CreateDefault(App.Repository.GetMyAthlete());

            this.Orientation = StackOrientation.Horizontal;
            this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Config.ColorBackgroundWhite;
            this.HeightRequest = Config.LargeButtonsHeight;// 40;
            this.Padding = new Thickness(15,0,10,0);
            this.Spacing = 5;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;
            this.VerticalOptions = LayoutOptions.Start;

            this.labelLabel = new BybLabel()
            {
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackgroundGrayed : Config.ColorGrayTextOnWhite,
                VerticalOptions = LayoutOptions.Center,
            };
            this.NameAsCommunity = true;
            this.Children.Add(this.labelLabel);
            this.labelCommunityName = new BybLabel()
            {
                Text = Selection.ToString(),
                FontFamily = Config.FontFamily,
                FontAttributes = FontAttributes.Bold,
                TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Color.White : Color.Black,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            this.Children.Add(this.labelCommunityName);
			this.labelAskToTap = new BybLabel()
			{
				IsVisible = false,
				Text = "(tap here)",
				FontFamily = Config.FontFamily,
				TextColor = Color.Red,
				VerticalOptions = LayoutOptions.Center,
			};
			this.Children.Add(this.labelAskToTap);

            Image image = new Image()
            {
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = Config.RedArrowImageSize,
                HeightRequest = Config.RedArrowImageSize,
                Source = new FileImageSource() { File = "arrowRight.png" }
            };
            this.Children.Add(image);

            this.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
					if (App.Navigator.GetOpenedModalPage(typeof(PickCommunityPage)) != null)
						return;
					
                    PickCommunityPage page = new PickCommunityPage();
                    page.AllowFriendsSelection = AllowFriendsSelection;
					page.DoNotCheckSelection = this.IsAskToTapVisible;
                    await App.Navigator.NavPage.Navigation.PushModalAsync(page);
                    page.SelectionChanged += (s1, e1) =>
                    {
                        this.Selection = page.Selection;
                        if (this.SelectionChanged != null)
                            this.SelectionChanged(this, EventArgs.Empty);
                    };
					await page.Initialize(this.Selection);
                })
            });
        }
    }
}
