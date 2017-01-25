using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfSnookerMatchesControl : ListOfItemsControl<SnookerMatchScore>
	{
		double widthOfLeftColumn = 200;
		double widthOfRightColumns = Config.IsTablet ? 70 : 45;
		
        public event EventHandler<SnookerEventArgs> UserWantsToDeleteScore;
        public event EventHandler<SnookerEventArgs> UserWantsToEditScore;
        public event EventHandler<SnookerEventArgs> UserWantsToViewScore;

		//public bool IsForPrimaryAthlete { get; set; }
		public ListTypeEnum Type { get; set; }
		public SnookerMatchSortEnum SortType { get; private set; }
		public List<SnookerMatchScore> AllMatches { get; private set; }

        List<StackLayout> panelsToResize;
		SimpleButtonWithLittleDownArrow buttonSort;

        public ListOfSnookerMatchesControl()
			: base()
        {
			base.MaxCountToShowByDefault = 5;
			base.MultCountToShow = 10;
			
			this.buttonSort = new SimpleButtonWithLittleDownArrow ()
			{
				HorizontalOptions = LayoutOptions.End,
			};
			this.buttonSort.Clicked += buttonSort_Clicked;
			this.panelTop.Children.Add (this.buttonSort);

			this.SortType = SnookerMatchSortEnum.ByDate;
			this.updateSortButton();
        }

		public override void Fill(List<SnookerMatchScore> list)
		{
			this.AllMatches = list;
			this.panelsToResize = new List<StackLayout> ();
			base.Fill(SnookerMatchScore.Sort(list, SortType).ToList());
		}

		public void Sort(SnookerMatchSortEnum sort)
		{
			this.SortType = sort;
			this.updateSortButton();
			if (this.AllMatches != null)
				base.Fill (SnookerMatchScore.Sort(AllMatches, SortType).ToList());
		}

        protected override void OnSizeAllocated(double width, double height)
        {
            // this is important, otherwise this column is shortened when the name wraps
            this.widthOfLeftColumn = width - widthOfRightColumns * 2 - 10;
            if (this.panelsToResize != null)
                foreach (var panel in this.panelsToResize)
                    panel.WidthRequest = this.widthOfLeftColumn;

            base.OnSizeAllocated(width, height);
        }

        void updateSortButton()
        {
            switch (this.SortType)
            {
                case SnookerMatchSortEnum.ByDate: this.buttonSort.Text = "Sort by date"; break;
                case SnookerMatchSortEnum.ByFrameCount: this.buttonSort.Text = "Sort by frame count"; break;
                case SnookerMatchSortEnum.ByWinFirst: this.buttonSort.Text = "Sort by wins first"; break;
                case SnookerMatchSortEnum.ByLossFirst: this.buttonSort.Text = "Sort by losses first"; break;
                case SnookerMatchSortEnum.ByBestFrame: this.buttonSort.Text = "Sort by best frame"; break;
                case SnookerMatchSortEnum.ByOpponent: this.buttonSort.Text = "Sort by opponent"; break;
                default: this.buttonSort.Text = "Sort by ?"; break;
            }
        }

        async void buttonSort_Clicked(object sender, EventArgs e)
        {
            string action1 = "Date";
            string action2 = "Frame count";
            string action3 = "Wins first";
            string action4 = "Losses first";
            string action5 = "Best frame";
            string action6 = "Opponent";

            var action = await App.Navigator.NavPage.DisplayActionSheet("Sort order", "Cancel", null, action1, action2, action3, action4, action5, action6);

            if (action == action1)
                this.Sort(SnookerMatchSortEnum.ByDate);
            else if (action == action2)
                this.Sort(SnookerMatchSortEnum.ByFrameCount);
            else if (action == action3)
                this.Sort(SnookerMatchSortEnum.ByWinFirst);
            else if (action == action4)
                this.Sort(SnookerMatchSortEnum.ByLossFirst);
            else if (action == action5)
                this.Sort(SnookerMatchSortEnum.ByBestFrame);
            else if (action == action6)
                this.Sort(SnookerMatchSortEnum.ByOpponent);
        }

		protected override View createViewForSingleItem (SnookerMatchScore match)
		{
			// opponent
			FormattedString formattedString = new FormattedString();
			if (this.Type == ListTypeEnum.Venue)
				formattedString.Spans.Add(new Span() { 
					Text = (string.IsNullOrEmpty(match.YourName) ? "-" : match.YourName) + " ", 
					FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize, ForegroundColor = Config.ColorBlackBackground 
				});
			formattedString.Spans.Add(new Span() { 
				Text = "vs. ", 
				FontAttributes = FontAttributes.None, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize, ForegroundColor = Config.ColorGrayTextOnWhite, 
			});
			formattedString.Spans.Add(new Span() { 
				Text = string.IsNullOrEmpty(match.OpponentName) ? "-" : match.OpponentName, 
				FontAttributes = FontAttributes.Bold, FontFamily = Config.FontFamily, FontSize = Config.DefaultFontSize, ForegroundColor = Config.ColorBlackBackground 
			});
			if (match.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
			{
				// nothing
				//formattedString.Spans.Add(new Span() { Text = "  confirmed", ForegroundColor = Config.ColorGrayTextOnWhite });
			}
			else if (match.OpponentConfirmation == OpponentConfirmationEnum.Declined)
				formattedString.Spans.Add(new Span() {
					Text = "  (declined)",
					ForegroundColor = Config.ColorTextOnBackgroundGrayed 
				});
			else
				formattedString.Spans.Add(new Span() { 
					Text = "  (unconfirmed)", 
					ForegroundColor = Config.ColorTextOnBackgroundGrayed 
				});
			Label labelOpponent = new BybLabel() { FormattedText = formattedString, VerticalTextAlignment = TextAlignment.Center };
			if (match.OpponentAthleteID > 0)
				labelOpponent.GestureRecognizers.Add(new TapGestureRecognizer()
				{
					Command = new Command(async () =>
					{
						await App.Navigator.GoToPersonProfile(match.OpponentAthleteID);
					}),
					NumberOfTapsRequired = 1
				});

			// venue
			string venueName = match.VenueName;
			if (string.IsNullOrEmpty(venueName))
				venueName = "-";
			Label labelVenue = new BybLabel()
			{
				Text = venueName,
				VerticalTextAlignment = TextAlignment.Center,
				TextColor = Config.ColorGrayTextOnWhite,
				IsVisible = this.Type != ListTypeEnum.Venue
			};
			if (match.VenueID > 0)
				labelVenue.GestureRecognizers.Add(new TapGestureRecognizer()
				{
					Command = new Command(async () =>
					{
						await App.Navigator.GoToVenueProfile(match.VenueID);
					}),
					NumberOfTapsRequired = 1
				});

			// frames
			Label labelForFrames = new BybLabel()
			{
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = TextAlignment.Start,
			};
			if (match.HasFrameScores)
			{
				FormattedString formattedStringScores = new FormattedString();

				foreach (var frame in match.FrameScores)
				{
					if (match.FrameScores.First() != frame)
						formattedStringScores.Spans.Add(new Span() {
							Text = " , ",
							FontAttributes = FontAttributes.None,
							FontSize = Config.DefaultFontSize,
							FontFamily = Config.FontFamily,
							ForegroundColor = Config.ColorGrayTextOnWhite,
						});

					Color color = Config.ColorGray;
					if (frame.A > frame.B)
						color = Config.ColorGreen;
					else if (frame.A < frame.B)
						color = Config.ColorRed;

					formattedStringScores.Spans.Add(new Span() {
						Text = frame.A + ":" + frame.B,
						FontAttributes = FontAttributes.Bold,
						FontSize = Config.DefaultFontSize,
						FontFamily = Config.FontFamily,
						ForegroundColor = color,
					});
				}

				labelForFrames.FormattedText = formattedStringScores;
			}

			// match color
			Color matchColor = match.MatchScoreA > match.MatchScoreB ? Config.ColorGreen : (match.MatchScoreA < match.MatchScoreB ? Config.ColorRed : Config.ColorGray);
			if (match.IsUnfinished)
				matchColor = Config.ColorGray;

			var panel1_1 = new StackLayout
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0, 8, 0, 0),
				WidthRequest = Config.IsTablet ? 120 : 80,
				MinimumWidthRequest = Config.IsTablet ? 100 : 80,
				HeightRequest = Config.IsTablet ? 55 : 45,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = matchColor,
				Spacing = 2,
				Children = 
				{
					new BybLabel
					{
						Text = match.IsUnfinished ? "PAUSED" : "Score",
						FontAttributes = match.IsUnfinished ? Xamarin.Forms.FontAttributes.Bold : FontAttributes.None,
						TextColor = match.IsUnfinished ? Config.ColorBlackBackground : Color.White,
						HorizontalOptions = LayoutOptions.Center
					},
					new BybLabel
					{
						Text = match.MatchScoreA.ToString() + " : " + match.MatchScoreB.ToString(),
						FontSize = Config.LargerFontSize,
						FontAttributes = Xamarin.Forms.FontAttributes.Bold,
						TextColor = Color.White,
						HorizontalOptions = LayoutOptions.Center
					},
				}
			};

			var panel1_0 = new StackLayout
			{
				Padding = new Thickness(10, 10, 0, 5),
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				WidthRequest = widthOfLeftColumn,
				VerticalOptions = LayoutOptions.Start,
				Children =
				{
					labelOpponent,
					labelForFrames,
				}
			};
			this.panelsToResize.Add(panel1_0);

			var panel1 = new StackLayout()
			{
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness(0, 0, 0, 0),
				Children =
				{
					panel1_0,
					panel1_1,
				}
			};
			var panel2 = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness(5, 0, 5, 0),
				Children =
				{
					new Grid
					{
						Padding = new Thickness(5,5,0,5),
						HorizontalOptions = LayoutOptions.StartAndExpand,
						Children = 
						{
							new BybLabel()
							{
								Text = DateTimeHelper.DateToString(match.Date),//match.Date.ToShortDateString(),
								VerticalTextAlignment = TextAlignment.Center,
								TextColor = Config.ColorGrayTextOnWhite
							}
						}
					},
					new Grid
					{
						Padding = new Thickness(0,5,5,5),
						HorizontalOptions = LayoutOptions.EndAndExpand,
						Children =
						{
							labelVenue
						}
					},
				}
			};

			panel1.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.showMenu(match); }) });
			panel2.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.showMenu(match); }) });
			panel1_1.GestureRecognizers.Add(new TapGestureRecognizer() { Command = new Command(() => { this.showMenu(match); }) });

			var panel = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 1,
				Children =
				{
					panel1,
					panel2,
				}
			};

			return panel;
		}

        async void showMenu(SnookerMatchScore match)
        {
            string strOption1 = "Delete this match";
            string strOption2 = "View this match";
            string strOption3 = "Edit this match";

            string answer = "";
			if (this.Type != ListTypeEnum.PrimaryAthlete)//this.IsForPrimaryAthlete == false)
            {
                answer = await App.Navigator.NavPage.DisplayActionSheet("Byb", "Cancel", null, strOption2);
            }
            else if (match.IsUnfinished)
            {
                strOption3 = "Continue this match";
                answer = await App.Navigator.NavPage.DisplayActionSheet("Byb", "Cancel", null, strOption1, strOption3);
            }
            else if (match.OpponentConfirmation == OpponentConfirmationEnum.Confirmed)
            {
                answer = await App.Navigator.NavPage.DisplayActionSheet("Byb", "Cancel", null, strOption1, strOption2);
            }
            else
            {
                answer = await App.Navigator.NavPage.DisplayActionSheet("Byb", "Cancel", null, strOption1, strOption2, strOption3);
            }

            if (answer == strOption1 && UserWantsToDeleteScore != null)
                UserWantsToDeleteScore(this, new SnookerEventArgs() { MatchScore = match });
            if (answer == strOption2 && UserWantsToViewScore != null)
                UserWantsToViewScore(this, new SnookerEventArgs() { MatchScore = match });
            if (answer == strOption3 && UserWantsToEditScore != null)
                UserWantsToEditScore(this, new SnookerEventArgs() { MatchScore = match });
        }
	}
}
