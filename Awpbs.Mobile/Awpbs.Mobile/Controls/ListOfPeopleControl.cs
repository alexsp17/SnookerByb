using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfPeopleControl : ListOfItemsControl<PersonBasicWebModel>
	{
		public event EventHandler<SelectedPersonEventArgs> UserClickedOnPerson;
		public event EventHandler<SelectedPersonEventArgs> UserClickedRemoveOnPerson;

		public bool ShowRemoveButton { get; set; }

		public ListOfPeopleControl()
		{
			this.MaximumPossibleCount = PersonBasicWebModel.MaxItems;
			this.TextForMaxPossibleShown = "showing random " + this.MaximumPossibleCount.ToString() + " people only";
		}  

		protected void onUserClickedOnPerson(PersonBasicWebModel person)
		{
			if (this.UserClickedOnPerson != null)
				this.UserClickedOnPerson(this, new SelectedPersonEventArgs() { Person = person });
		}

		protected void onUserClickedRemoveOnPerson(PersonBasicWebModel person)
		{
			if (this.UserClickedRemoveOnPerson != null)
				this.UserClickedRemoveOnPerson(this, new SelectedPersonEventArgs() { Person = person });
		}

		protected override View createViewForSingleItem(PersonBasicWebModel person)
		{
			try
			{
				string personPicture = person.Picture;
				var imageSource = App.ImagesService.GetImageSource(person.Picture, Config.App == MobileAppEnum.SnookerForVenues ? BackgroundEnum.Background1 : BackgroundEnum.White);

				var image = new Image()
				{
					Source = imageSource,
					WidthRequest = Config.PersonImageSize,
					HeightRequest = Config.PersonImageSize,
				};

				var stackPerson = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
					Spacing = 10,
					Padding = new Thickness(5,5,5,5),
					BackgroundColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorBackground : Config.ColorBackgroundWhite,
					HorizontalOptions = LayoutOptions.Fill,
					Children =
					{
						new BoxView() { WidthRequest = 0, BackgroundColor = Color.Transparent },
						image,
						new StackLayout
						{
							Orientation = StackOrientation.Vertical,
							HorizontalOptions = LayoutOptions.FillAndExpand,
							VerticalOptions = LayoutOptions.Center,
							Spacing = 3,
							Children =
							{
								new BybLabel
								{
									Text = person.Name,
									HeightRequest = Config.IsTablet ? 25: 18,
									TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackground : Config.ColorBlackTextOnWhite,
									HorizontalOptions = LayoutOptions.Start,
									FontAttributes = FontAttributes.Bold
								},
								new BybLabel
								{
									Text = person.HasMetro ? person.Metro : "Unknown location",
									TextColor = Config.App == MobileAppEnum.SnookerForVenues ? Config.ColorTextOnBackgroundGrayed : Config.ColorGrayTextOnWhite,
								},
							}
							},
					}
					};

				if (this.ShowRemoveButton)
				{
					Button removeButton = new BybButton() { Text = "x", WidthRequest = 30, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.FillAndExpand, Style = (Style)App.Current.Resources["SimpleButtonStyle"] };
					removeButton.Clicked += (s1, e1) => { this.onUserClickedRemoveOnPerson(person); };
					stackPerson.Children.Add(removeButton);
				}

				image.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(() => { this.onUserClickedOnPerson(person); }),
				});
				stackPerson.GestureRecognizers.Add(new TapGestureRecognizer
				{
					Command = new Command(() => { this.onUserClickedOnPerson(person); }),
				});

				return stackPerson;
			}
			catch (Exception exc)
			{
				return new StackLayout ()
				{
					Children = {
						new BybLabel () { Text = TraceHelper.ExceptionToString (exc) }
					}
				};
			}
		}
	}
}
