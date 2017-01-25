using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfOpponentsControl : ListOfItemsControl<SnookerOpponent>
	{
		double widthOfImageColumn = Config.PersonImageSize;
		double widthOfLeftColumn = 200;
		double widthOfRightColumns = Config.IsTablet ? 70 : 45;

        public SnookerOpponentSortEnum SortType { get; private set; }
		public List<SnookerOpponent> AllOpponents { get; private set; }

		SimpleButtonWithLittleDownArrow buttonSort;
        List<StackLayout> panelsToResize;

        public ListOfOpponentsControl()
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

			this.SortType = SnookerOpponentSortEnum.ByMatchCount;
			this.updateSortButton();
        }

		public override void Fill (List<SnookerOpponent> list)
		{
			this.AllOpponents = list;
			this.panelsToResize = new List<StackLayout> ();
			base.Fill(SnookerOpponent.Sort(list, SortType).ToList());
		}

		public void Sort(SnookerOpponentSortEnum sort)
		{
			this.SortType = sort;
			this.updateSortButton();
			if (this.AllOpponents != null)
				base.Fill(SnookerOpponent.Sort(AllOpponents, SortType).ToList());
		}

        protected override void OnSizeAllocated(double width, double height)
        {
            // this is important, otherwise this column is shortened when the name wraps
            this.widthOfLeftColumn = width - widthOfImageColumn - widthOfRightColumns * 2 - 10;
            if (this.panelsToResize != null)
                foreach (var panel in this.panelsToResize)
                    panel.WidthRequest = this.widthOfLeftColumn;

            base.OnSizeAllocated(width, height);
        }

        void updateSortButton()
        {
            switch (this.SortType)
            {
                case SnookerOpponentSortEnum.ByName: this.buttonSort.Text = "Sort by name"; break;
                case SnookerOpponentSortEnum.ByWinFirst: this.buttonSort.Text = "Sort by wins first"; break;
                case SnookerOpponentSortEnum.ByLossFirst: this.buttonSort.Text = "Sort by losses first"; break;
				case SnookerOpponentSortEnum.ByMatchCount: this.buttonSort.Text = "Sort by match count"; break;
                default: this.buttonSort.Text = "Sort by ?"; break;
            }
        }

        async void buttonSort_Clicked(object sender, EventArgs e)
        {
			string action0 = "Match count";
            string action1 = "Name";
            string action2 = "Wins first";
            string action3 = "Losses first";

            var action = await App.Navigator.NavPage.DisplayActionSheet("Sort order", "Cancel", null, action0, action1, action2, action3);

			if (action == action0)
				this.Sort(SnookerOpponentSortEnum.ByMatchCount);
			else if (action == action1)
                this.Sort(SnookerOpponentSortEnum.ByName);
            else if (action == action2)
                this.Sort(SnookerOpponentSortEnum.ByWinFirst);
            else if (action == action3)
                this.Sort(SnookerOpponentSortEnum.ByLossFirst);
        }

		protected override View createViewForSingleItem (SnookerOpponent opponent)
		{
			var person = opponent.Person;

			var panelPersonMetro = new StackLayout()
			{
				Padding = new Thickness(10, 0, 0, 0),
				Orientation = StackOrientation.Vertical,
				WidthRequest = this.widthOfLeftColumn,
				VerticalOptions = LayoutOptions.Center,
				Children = 
				{
					new BybLabel { Text = person.Name, TextColor = Config.ColorBlackTextOnWhite, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold },
					new BybLabel { Text = person.HasMetro ? person.Metro : "Unknown location", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite },
				}
			};

			panelPersonMetro.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () =>
				{
					await App.Navigator.GoToPersonProfile(person.ID);
				}),
				NumberOfTapsRequired = 1
			});

			this.panelsToResize.Add(panelPersonMetro);

			var stackPerson = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Padding = new Thickness(0, 0, 0, 0),
				Spacing = 1,
				BackgroundColor = Color.White,
				Children =
				{
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness(5,5,0,5),
						Spacing = 0,
						Children =
						{
							new Image()
							{
								Source = App.ImagesService.GetImageSource(person.Picture),
								WidthRequest = this.widthOfImageColumn,
								HeightRequest = Config.PersonImageSize,
							},
							panelPersonMetro,
							new StackLayout
							{
								Orientation = StackOrientation.Vertical,
								Padding = new Thickness(0, 0, 0, 0),
								VerticalOptions = LayoutOptions.Center,
								Spacing = 2,
								WidthRequest = widthOfRightColumns,
								Children =
								{
									new BybLabel { Text = "Win", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
									new BybLabel { Text = opponent.CountWins.ToString(), TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center },
								}
							},
							new StackLayout
							{
								Orientation = StackOrientation.Vertical,
								Padding = new Thickness(0, 0, 0, 0),
								VerticalOptions = LayoutOptions.Center,
								Spacing = 2,
								WidthRequest = widthOfRightColumns,
								Children = 
								{
									new BybLabel { Text = "Loss", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
									new BybLabel { Text = opponent.CountLosses.ToString(), TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center }
								}
							},
							new StackLayout
							{
								Orientation = StackOrientation.Vertical,
								Padding = new Thickness(0, 0, 0, 0),
								VerticalOptions = LayoutOptions.Center,
								Spacing = 2,
								WidthRequest = widthOfRightColumns,
								Children = 
								{
									new BybLabel { Text = "Draw", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Center },
									new BybLabel { Text = opponent.CountDraws.ToString(), TextColor = Config.ColorBlackTextOnWhite, FontSize = Config.LargerFontSize, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center }
								}
							}
						}
					},
				}
			};

			stackPerson.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () =>
				{
					await App.Navigator.GoToPersonProfile(person.ID);
				}),
				NumberOfTapsRequired = 1
			});

			return stackPerson;
		}
    }
}
