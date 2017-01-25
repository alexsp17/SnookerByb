using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class NewsfeedItemPage : ContentPage
	{
        public bool AnythingChanged { get; private set; }

        bool isModal;
        NewsfeedItemWebModel item;

        Label labelLoading;
        ListOfCommentsControl listOfCommentsControl;
        Editor editor;
        Button buttonCancel;

        StackLayout panelWithButtons;

        public NewsfeedItemPage(NewsfeedItemWebModel item, bool isModal)
        {
            this.isModal = isModal;
            this.item = item;

            NewsfeedItemBaseControl newsfeedItemControl;
            if (item.ItemType == NewsfeedItemTypeEnum.Post)
                newsfeedItemControl = new NewsfeedItemPostControl(item, false);
            else if (item.ItemType == NewsfeedItemTypeEnum.Result)
                newsfeedItemControl = new NewsfeedItemResultControl(item, false);
            else if (item.ItemType == NewsfeedItemTypeEnum.Score)
                newsfeedItemControl = new NewsfeedItemScoreControl(item, false);
            else if (item.ItemType == NewsfeedItemTypeEnum.GameHost)
                newsfeedItemControl = new NewsfeedItemGameHostControl(item, false);
            else if (item.ItemType == NewsfeedItemTypeEnum.NewUser)
                newsfeedItemControl = new NewsfeedItemNewUserControl(item, true);
            else
                throw new NotImplementedException();
            newsfeedItemControl.ShowCommentsCount = false;
            newsfeedItemControl.TreatAsASingleItem = false;
            newsfeedItemControl.NeedsARefresh += (s1, e1) => { this.AnythingChanged = true; };
            //newsfeedItemControl.Padding = new Thickness(0);

            this.labelLoading = new BybLabel()
            {
                HeightRequest = 40,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center,
				TextColor = Config.ColorBlackTextOnWhite,
                IsVisible = true,
                Text = "Loading...",
            };

            this.listOfCommentsControl = new ListOfCommentsControl()
            {
                Padding = new Thickness(10,10,0,0),
            };

            this.editor = new BybEditor()
            {
                HeightRequest = 40,
            };
			this.editor.Unfocused += editor_Unfocused;
			this.editor.Focused += editor_Focused;
            this.editor.Completed += editor_Completed;

            this.buttonCancel = new BybButton { Text = "Close", Style = (Style)App.Current.Resources["BlackButtonStyle"] };
            buttonCancel.Clicked += buttonCancel_Clicked;

			Button buttonPostComment = new BybButton { Text = "Done", Style = (Style)App.Current.Resources["SimpleButtonStyle"], HorizontalOptions = LayoutOptions.End };
            buttonPostComment.Clicked += buttonPostComment_Clicked;
			this.panelWithButtons = new StackLayout () {
				Orientation = StackOrientation.Horizontal,
				//BackgroundColor = Config.ColorBackground,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				HeightRequest = Config.OkCancelButtonsHeight,
				Padding = new Thickness (10,0,10,0),
				Spacing = 0,
				IsVisible = false,
				Children = {
					buttonPostComment,
					//buttonCancel2,
				}
			};

            var stackLayout = new StackLayout
            {
                Spacing = 0,
                Padding = new Thickness(0,0,0,0),

                Children =
                {
                    new StackLayout
                    {
                        Padding = new Thickness(10,10,10,0),
                        Orientation = StackOrientation.Vertical,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children =
                        {
                            newsfeedItemControl,
                        }
                    },

                    this.labelLoading,
                    new ScrollView
                    {
                        Padding = new Thickness(10, 0, 10, 10),
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        Content = new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                this.listOfCommentsControl,
								new StackLayout
								{
									Orientation = StackOrientation.Horizontal,
									BackgroundColor = Config.ColorGrayBackground,
									Padding = new Thickness(10,10,0,0),
									Children = 
									{
										new BybLabel
										{
											Text = "Enter your comments here:",
											VerticalOptions = LayoutOptions.Center,
											TextColor = Config.ColorGrayTextOnWhite
										},
										panelWithButtons,
									},
								},
//                                new Frame
//                                {
//                                    BackgroundColor = Config.ColorGrayBackground,
//                                    Padding = new Thickness(10,10,0,0),
//                                    Content = new BybLabel
//                                    {
//                                        Text = "Enter your comments here:"
//                                    }
//                                },
								//panelWithButtons,
                                new Frame
                                {
                                    BackgroundColor = Config.ColorGrayBackground,
                                    Padding = new Thickness(15,5,5,5),
                                    Content = editor,
                                },
                            }
                        }
                    },

                    new StackLayout()
                    {
                        Orientation = StackOrientation.Horizontal,
                        //BackgroundColor = Config.ColorBackground,
                        HorizontalOptions = LayoutOptions.Fill,
                        HeightRequest = Config.OkCancelButtonsHeight,
                        Padding = new Thickness(Config.OkCancelButtonsPadding),
                        Children =
                        {
                            buttonCancel,
                        }
                    }
                }
            };

            if (this.isModal)
                stackLayout.Children.Insert(0, new BybTitle("Comments"));

            this.Content = stackLayout;
            this.Padding = new Thickness(0, 0, 0, 0);
            this.BackgroundColor = Config.ColorGrayBackground;
            this.Title = "Comments";
        }

        private void buttonPostComment_Clicked(object sender, EventArgs e)
        {
            this.editor.Unfocus();
        }

        private void editor_Unfocused(object sender, EventArgs e)
		{
			this.panelWithButtons.IsVisible = false;
		}

		private void editor_Focused(object sender, EventArgs e)
		{
			this.panelWithButtons.IsVisible = true;
		}

        private async void editor_Completed(object sender, EventArgs e)
        {
            string text = this.editor.Text;
            if (text == null)
                text = "";
            text = text.Trim();
            if (text.Length == 0)
                return;
            if (text.Length > 512)
                text = text.Substring(0, 512);

            this.editor.Text = "";

            if (await App.WebService.MakeComment(item.ItemType, item.ID, text) == null)
            {
                this.editor.Text = text;
                App.Navigator.DisplayAlertError("Couldn't post comment. Internet issues?");
                return;
            }
            this.AnythingChanged = true;

            await loadComments();
        }

        private async void buttonCancel_Clicked(object sender, EventArgs e)
        {
            if (this.isModal)
                await App.Navigator.NavPage.Navigation.PopModalAsync();
            else
                await App.Navigator.NavPage.PopAsync();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await loadComments();
        }

        async Task loadComments()
        {
            this.listOfCommentsControl.IsVisible = false;
            this.labelLoading.IsVisible = true;

            var comments = await App.WebService.GetComments(item.ItemType, item.ID);
            this.listOfCommentsControl.Fill(comments);
            this.listOfCommentsControl.IsVisible = true;
            this.labelLoading.IsVisible = false;
        }
    }
}
