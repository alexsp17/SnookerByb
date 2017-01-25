using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Awpbs.Mobile
{
	public abstract class ListOfItemsControl<T> : StackLayout
	{
		public event Action NeedsARefresh;

		protected void onNeedsARefresh()
		{
			if (this.NeedsARefresh != null)
				this.NeedsARefresh();
		}

        public bool IsDarkBackground { get; set; }
		
		public int MaxCountToShowByDefault { get; set; }
		public int MultCountToShow { get; set; }
		public int MaximumPossibleCount { get; set; }
		
		public string TextForFailedToLoad { get; set; }
		public string TextForEmpty { get; set; }
		public string TextForShowMore { get; set; }
		public string TextForMaxPossibleShown { get; set; }

		public List<T> List { get; private set; }
		public List<T> ListShown { get; private set; }

		protected StackLayout panelTop { get; private set; }

		BybLabel labelFailedToLoad;
		BybLabel labelEmpty;
		BybButton buttonShowMore;
		BybLabel labelMaxPossibleCount;

		public ListOfItemsControl ()
		{
			MaxCountToShowByDefault = 15;
			MultCountToShow = 15;
			MaximumPossibleCount = 1000000;
			
			TextForFailedToLoad = "Couldn't load from snookerbyb.com. Internet issues?";
			TextForEmpty = "None";
			TextForShowMore = "Show more";
			TextForMaxPossibleShown = "Older items are not shown";

            //this.BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBlackBackground : Config.ColorGrayBackground;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.Spacing = 5;
			this.Padding = new Thickness(5,5,5,5);
			this.HorizontalOptions = LayoutOptions.FillAndExpand;

			this.panelTop = new StackLayout ()
			{
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.Fill,
				Padding = new Thickness(0,0,10,0),
			};
			this.Children.Add (this.panelTop);
		}

		public virtual void Fill(List<T> list)
		{
			this.List = list;
			this.ListShown = null;
			this.buttonShowMore = null;

			this.Children.Clear ();
			this.Children.Add (this.panelTop);
			
			if (list == null)
			{
				this.labelFailedToLoad = new BybLabel ()
				{
                    TextColor = IsDarkBackground ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite,
                    HeightRequest = 60,
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = this.TextForFailedToLoad,
				};
				this.Children.Add(this.labelFailedToLoad);
				return;
			}

			if (list.Count() == 0)
			{
				this.labelEmpty = new BybLabel ()
				{
                    TextColor = IsDarkBackground ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite,
                    HeightRequest = 60,
					HorizontalOptions = LayoutOptions.Center,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					Text = this.TextForEmpty,
				};
				this.Children.Add (this.labelEmpty);
				return;
			}

			this.ListShown = List.Take(MaxCountToShowByDefault).ToList ();

			foreach (var item in this.ListShown)
			{
				this.Children.Add (this.createViewForSingleItem(item));
			}

			this.addOrRemoveButtonShowMore();
		}

		protected abstract View createViewForSingleItem(T item);

		void addOrRemoveButtonShowMore()
		{
			if (this.buttonShowMore != null)
			{
				this.Children.Remove (buttonShowMore);
				this.buttonShowMore = null;
			}

			if (this.ListShown == null || this.List == null || this.ListShown.Count >= this.List.Count)
				return;

            this.buttonShowMore = new BybButton()
            {
                Text = TextForShowMore,
                Style = (Style)App.Current.Resources["SimpleButtonStyle"],
                HorizontalOptions = LayoutOptions.Center,
                HeightRequest = 50,
                TextColor = IsDarkBackground ? Color.White : Config.ColorBlackTextOnWhite,
			};
			buttonShowMore.Clicked += buttonShowMore_Clicked;
			this.Children.Add(this.buttonShowMore);
		}

		void addOrRemoveLabelMaximumPossibleCount()
		{
			if (this.labelMaxPossibleCount != null)
			{
				this.Children.Remove(labelMaxPossibleCount);
				this.labelMaxPossibleCount = null;
			}

			if (this.ListShown == null || this.List == null || this.ListShown.Count != this.List.Count || this.List.Count != MaximumPossibleCount)
				return;

			this.labelMaxPossibleCount = new BybLabel ()
			{
				Text = TextForMaxPossibleShown,
				TextColor = IsDarkBackground ? Config.ColorTextOnBackgroundGrayed : Config.ColorGrayTextOnWhite,
				HeightRequest = 50,
				HorizontalOptions = LayoutOptions.Center,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
			};
			this.Children.Add (this.labelMaxPossibleCount);
		}

		private void buttonShowMore_Clicked(object sender, EventArgs e)
		{
			if (this.List == null || this.ListShown == null)
				return;

			for (int i = 0; i < MultCountToShow; ++i)
			{
				if (this.ListShown.Count() >= this.List.Count())
					break;
				var item = this.List [this.ListShown.Count];
				this.ListShown.Add (item);
				this.Children.Add (this.createViewForSingleItem(item));
			}

			this.addOrRemoveButtonShowMore ();
			this.addOrRemoveLabelMaximumPossibleCount ();
		}
	}
}

