using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfNewsfeedItemsControl : ListOfItemsControl<NewsfeedItemWebModel>
	{
        public ListOfNewsfeedItemsControl()
        {
			this.TextForEmpty = "Nothing is happening";
        }

		public void Fill(NewsfeedWebModel newsfeed, bool isCanada)
		{
			if (newsfeed == null)
			{
				this.Fill (null);
				return;
			}
			
			this.Fill (newsfeed.Items);

			// add Snooker Canada's logo
			if (isCanada)
			{
				var image = new Image()
				{
					Source = new FileImageSource() { File = "SnookerCanada.jpg" },
					HeightRequest = 100,
					WidthRequest = 100,
				};
				image.GestureRecognizers.Add(new TapGestureRecognizer()
				{
					Command = new Command(() =>
					{
						App.Navigator.OpenBrowserApp("http://www.snookercanada.ca");
					})
				});

				this.Children.Add(new StackLayout()
				{
					BackgroundColor = Config.ColorBackgroundWhite, // the image has white background, so use white background here
					Orientation = StackOrientation.Vertical,
					HorizontalOptions = LayoutOptions.Fill,
					Padding = new Thickness(0,10,0,10),
					Children =
					{
						image,
					}
				});
			}
		}

		protected override View createViewForSingleItem(NewsfeedItemWebModel item)
		{
			try
			{
				NewsfeedItemBaseControl ctrl = null;
				if (item.ItemType == NewsfeedItemTypeEnum.Post)
					ctrl = new NewsfeedItemPostControl(item, true);
				else if (item.ItemType == NewsfeedItemTypeEnum.Result)
					ctrl = new NewsfeedItemResultControl(item, true);
				else if (item.ItemType == NewsfeedItemTypeEnum.Score)
					ctrl = new NewsfeedItemScoreControl(item, true);
				else if (item.ItemType == NewsfeedItemTypeEnum.GameHost)
					ctrl = new NewsfeedItemGameHostControl(item, true);
				else if (item.ItemType == NewsfeedItemTypeEnum.NewUser)
					ctrl = new NewsfeedItemNewUserControl(item, true);
				else
					throw new Exception("Unknown type of NewsfeedItem");

				ctrl.NeedsARefresh += (s1, e1) => { this.onNeedsARefresh(); };
				return ctrl;
			}
			catch (Exception exc)
			{
				return new BybLabel () { Text = TraceHelper.ExceptionToString (exc) };
			}
		}
    }
}
