using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfVenuesControl : ListOfItemsControl<VenueWebModel>
	{
		public event EventHandler<VenueWebModel> UserClickedOnVenue;

		public bool IsDistanceFromCurrentLocation { get; set; }

		public ListOfVenuesControl ()
		{
		}

		public void DoOnVenueEdited(int venueID)
		{
//			if (dictionaryStackVenues == null || dictionaryStackVenues.ContainsKey(venueID) == false || venues == null)
//				return;
//			VenueWebModel venue = App.Cache.Venues.Get(venueID);
//			if (venue == null)
//				return;
//			VenueWebModel venueInTheList = venues.Where(i => i.ID == venueID).FirstOrDefault();
//			if (venueInTheList == null)
//				return;
//			venue.DistanceInMeters = venueInTheList.DistanceInMeters;
//
//			StackLayout stackVenue = dictionaryStackVenues[venueID];
//			if (stackVenue == null)
//				return;
//
//			StackLayout newStackVenue = createStackVenue(venue);
//			newStackVenue.BackgroundColor = Color.Lime;
//
//			int index = this.Children.IndexOf(stackVenue);
//			this.Children.Remove(stackVenue);
//			this.Children.Insert(index, newStackVenue);
//
//			dictionaryStackVenues.Remove(venueID);
//			dictionaryStackVenues.Add(venueID, newStackVenue);
		}

		protected override View createViewForSingleItem (VenueWebModel venue)
		{
			string str10f = "-";
			if (venue.NumberOf10fSnookerTables != null)
				str10f = venue.NumberOf10fSnookerTables.Value.ToString();
			string str12f = "-";
			if (venue.NumberOf12fSnookerTables != null)
				str12f = venue.NumberOf12fSnookerTables.Value.ToString();

			string strDistance = Distance.FromMeters(venue.DistanceInMeters).ToString();
			if (this.IsDistanceFromCurrentLocation)
				strDistance += " from you";
			else
				strDistance += " from map center";

			string strType = "";
			if (venue.IsInvalid)
				strType = "Closed-down";
			else if (venue.LastContributorID > 0 && venue.LastContributorDate != null)
				strType = "Verified on " + venue.LastContributorDate.Value.ToShortDateString ();
			else
				strType = "Not verified";

			StackLayout stackVenue_NameAndMetro = new StackLayout
			{
				Padding = new Thickness(10,0,0,0),
				Orientation = StackOrientation.Vertical,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				Children = 
				{
					new BybLabel { Text = venue.Name, HorizontalOptions = LayoutOptions.Start, FontAttributes = FontAttributes.Bold, TextColor = Config.ColorBlackBackground, },
					new BybLabel { Text = venue.MetroName ?? "", HorizontalOptions = LayoutOptions.Start, TextColor = Config.ColorGrayTextOnWhite, },
				}
				};

			stackVenue_NameAndMetro.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(() =>
						{
							if (this.UserClickedOnVenue != null)
								this.UserClickedOnVenue(this, venue);
						}),
					NumberOfTapsRequired = 1
				});

			// Create stackLayout for this venue, to be returned by this method
			//
			var panel1 = new StackLayout
            {
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness (0, 10, 0, 5),
				Spacing = 0,
				Children = {
					stackVenue_NameAndMetro,

					new StackLayout {
						Orientation = StackOrientation.Vertical,
						Padding = new Thickness (0, 0, 0, 0),
						WidthRequest = Config.IsTablet ? 100 : 60,
						MinimumWidthRequest = Config.IsTablet ? 100 : 60,
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.Start,
						Spacing = 2,
						Children = {
							new BybLabel {
								Text = "10' tables",
								TextColor = Config.ColorGrayTextOnWhite,
								HorizontalOptions = LayoutOptions.Center,
							},
							new BybLabel {
								Text = str10f,
								FontSize = Config.LargerFontSize,
								FontAttributes = FontAttributes.Bold,
								HorizontalTextAlignment = TextAlignment.Center,
								TextColor = Config.ColorBlackTextOnWhite,
							},
						}
					},
					new StackLayout {
						Orientation = StackOrientation.Vertical,
						Padding = new Thickness (0, 0, 0, 0),
						WidthRequest = Config.IsTablet ? 100 : 60,
						MinimumWidthRequest = Config.IsTablet ? 100 : 60,
						HorizontalOptions = LayoutOptions.End,
						VerticalOptions = LayoutOptions.Start,
						Spacing = 2,
						Children = {
							new BybLabel {
								Text = "12' tables",
								TextColor = Config.ColorGrayTextOnWhite,
								HorizontalOptions = LayoutOptions.Center
							},
							new BybLabel {
								Text = str12f,
								FontSize = Config.LargerFontSize,
								FontAttributes = FontAttributes.Bold,
								HorizontalTextAlignment = TextAlignment.Center,
								TextColor = Config.ColorBlackTextOnWhite,
							}
						}
					}
				}
			};
			var panel2 = new StackLayout
            {
				Orientation = StackOrientation.Horizontal,
				BackgroundColor = Color.White,
				Padding = new Thickness (10, 0, 5, 0),
				Children =
				{
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Padding = new Thickness (0, 5, 0, 5),
						Children = {
							new BybLabel {
								Text = strDistance,
								HorizontalOptions = LayoutOptions.StartAndExpand,
								TextColor = Config.ColorGrayTextOnWhite
							},
							new BybLabel {
								Text = strType,
								HorizontalOptions = LayoutOptions.EndAndExpand,
								TextColor = Config.ColorGrayTextOnWhite
							},
						}
					}
				}
			};
			var stackVenue = new StackLayout()
			{
				Orientation = StackOrientation.Vertical,
				Spacing = 1,
				Children =
				{
					panel1,
					panel2,
				}
			};

			var recognizer = new TapGestureRecognizer
            {
				Command = new Command (() => {
					if (this.UserClickedOnVenue != null)
						this.UserClickedOnVenue (this, venue);
				}),
				NumberOfTapsRequired = 1
			};
			panel1.GestureRecognizers.Add(recognizer);
			panel2.GestureRecognizers.Add(recognizer);

			return stackVenue;
		}
	}
}

