using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfVenuesPlayedControl : ListOfItemsControl<SnookerVenuePlayed>
	{
		double widthOfLeftColumn = 200;
		double widthOfRightColumns = Config.IsTablet ? 80 : 60;
		
        public SnookerVenuesPlayedSortEnum SortType { get; private set; }
        public List<SnookerVenuePlayed> AllVenues { get; private set; }

		SimpleButtonWithLittleDownArrow buttonSort;
        List<StackLayout> panelsToResize;

        public ListOfVenuesPlayedControl()
			:base()
        {
			base.MaxCountToShowByDefault = 5;
			base.MultCountToShow = 10;

			this.buttonSort = new SimpleButtonWithLittleDownArrow ()
			{
				HorizontalOptions = LayoutOptions.End,
			};
			this.buttonSort.Clicked += buttonSort_Clicked;
			this.panelTop.Children.Add (this.buttonSort);

			this.SortType = SnookerVenuesPlayedSortEnum.ByCount;
			this.updateSortButton();
        }

		public override void Fill (List<SnookerVenuePlayed> list)
		{
			this.AllVenues = list;
			this.panelsToResize = new List<StackLayout> ();
			base.Fill(SnookerVenuePlayed.Sort(list, SortType).ToList());
		}

		public void Sort(SnookerVenuesPlayedSortEnum sort)
		{
			this.SortType = sort;
			this.updateSortButton();
			if (this.AllVenues != null)
				base.Fill(SnookerVenuePlayed.Sort(AllVenues, SortType).ToList());
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
                case SnookerVenuesPlayedSortEnum.ByName: this.buttonSort.Text = "Sort by name"; break;
                case SnookerVenuesPlayedSortEnum.ByCount: this.buttonSort.Text = "Sort by count played"; break;
                default: this.buttonSort.Text = "Sort by ?"; break;
            }
        }

        async void buttonSort_Clicked(object sender, EventArgs e)
        {
            string action1 = "Name";
            string action2 = "Count played";

            var action = await App.Navigator.NavPage.DisplayActionSheet("Sort order", "Cancel", null, action1, action2);

            if (action == action1)
                this.Sort(SnookerVenuesPlayedSortEnum.ByName);
            else if (action == action2)
                this.Sort(SnookerVenuesPlayedSortEnum.ByCount);
        }

		protected override View createViewForSingleItem (SnookerVenuePlayed venue)
		{
			try
			{
                string strMetro = "-";
                if (venue.Venue.MetroName != null)
                    strMetro = venue.Venue.MetroName;

                string strType = "-";
                if (venue.Venue.IsInvalid)
                    strType = "Closed-down";
				else if (venue.Venue.LastContributorID > 0 && venue.Venue.LastContributorDate != null)
					strType = "Verified on " + venue.Venue.LastContributorDate.Value.ToShortDateString ();
                else
                    strType = "Unverified venue";

                var panelVenueMetro = new StackLayout()
                {
                    Padding = new Thickness(10,0,0,0),
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    WidthRequest = widthOfLeftColumn,
                    Children = 
                    {
						new BybLabel { Text = venue.Venue.Name, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.FillAndExpand, FontAttributes = FontAttributes.Bold },
                        new BybLabel { Text = strMetro, HorizontalOptions = LayoutOptions.FillAndExpand, TextColor = Config.ColorGrayTextOnWhite },
                    }
                };

                this.panelsToResize.Add(panelVenueMetro);

				var panel1 = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					BackgroundColor = Color.White,
					Padding = new Thickness(0,10,0,10),
					Spacing = 0,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Children =
					{
						panelVenueMetro,
						new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							Padding = new Thickness(0, 0, 0, 0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Start,
							WidthRequest = widthOfRightColumns,
							Spacing = 2,
							Children = 
							{
								new BybLabel { Text = "Breaks", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
								new BybLabel { Text = venue.CountBests.ToString(), TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center },
							}
							},
						new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							Padding = new Thickness(0, 0, 0, 0),
							VerticalOptions = LayoutOptions.Center,
							HorizontalOptions = LayoutOptions.Start,
							WidthRequest = widthOfRightColumns,
							Spacing = 2,
							Children = 
							{
								new BybLabel { Text = "Matches", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
								new BybLabel { Text = venue.CountMatches.ToString(), TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center }
							}
							}
					}
				};

				var panel2 = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					BackgroundColor = Color.White,
					Padding = new Thickness(10,5,5,5),
					HorizontalOptions = LayoutOptions.FillAndExpand,
					Children =
					{
						new BybLabel { Text = strType, HorizontalOptions = LayoutOptions.End, TextColor = Config.ColorGrayTextOnWhite },
					}
				};

                var stackVenue = new StackLayout()
                {
                    Orientation = StackOrientation.Vertical,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    Spacing = 1,
					BackgroundColor = Config.ColorGrayBackground,
                    Children =
                    {
                        panel1,
                        panel2,
                    }
                };

				panel1.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(async () => { await App.Navigator.GoToVenueProfile(venue.Venue.ID); }),
				});

				panel2.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(async () => { await App.Navigator.GoToVenueProfile(venue.Venue.ID); }),
				});


//				panelVenueMetro.GestureRecognizers.Add(new TapGestureRecognizer
//				{
//					Command = new Command(() =>
//					{
//						App.Navigator.GoToVenueProfile(venue.Venue.ID);
//					}),
//					NumberOfTapsRequired = 1
//				});
//                stackVenue.GestureRecognizers.Add(new TapGestureRecognizer
//                {
//                    Command = new Command(() =>
//                    {
//                        App.Navigator.GoToVenueProfile(venue.Venue.ID);
//                    }),
//                });

				return stackVenue;
            }
            catch (Exception exc)
            {
				return new BybLabel() { Text = "Error: " + TraceHelper.ExceptionToString(exc) };
            }
        }
	}
}
