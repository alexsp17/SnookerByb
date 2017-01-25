using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace Awpbs.Mobile
{
	public class ListOfCommentsControl : StackLayout
	{
        public ListOfCommentsControl()
        {
            this.Orientation = StackOrientation.Vertical;
            this.BackgroundColor = Config.ColorGrayBackground;
            this.Spacing = 1;
            this.Padding = new Thickness(0,0,0,0);
        }

        public void Fill(List<CommentWebModel> comments)
        {
            this.Children.Clear();

            if (comments == null)
            {
                this.Children.Add(new BybLabel()
                {
                    Text = "Couldn't load comments. Internet issues?",
					TextColor = Config.ColorBlackTextOnWhite,
                });
                return;
            }

            if (comments.Count == 0)
            {
                //this.Children.Add(new BybLabel()
                //{
                //    Text = "No comments",
                //});
                return;
            }

            comments = comments.OrderBy(i => i.Time).ToList();

            foreach (var comment in comments)
            {
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span() { Text = DateTimeHelper.DateToString(comment.Time), ForegroundColor = Config.ColorTextOnBackgroundGrayed, FontSize = Config.SmallFontSize });
                formattedString.Spans.Add(new Span() { Text = " " + comment.AthleteName, ForegroundColor = Config.ColorBlackTextOnWhite });
                var label = new BybLabel()
                {
                    FormattedText = formattedString,
                    TextColor = Config.ColorGrayTextOnWhite,
                    FontSize = Config.SmallFontSize
                };
                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        await App.Navigator.GoToPersonProfile(comment.AthleteID);
                    }),
                    NumberOfTapsRequired = 1
                });

                var panel = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    BackgroundColor = Color.White,
                    Padding = new Thickness(0,3,0,3),
                    Children =
                    {
                        new Frame
                        {
                            BackgroundColor = Color.White,
                            Padding = new Thickness(0),
                            Content = new Image()
                            {
                                Source = App.ImagesService.GetImageSource(comment.AthletePicture, BackgroundEnum.White),
                                WidthRequest = Config.PersonImageSize / 2,
                                HeightRequest = Config.PersonImageSize / 2,
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                label,
                                new BybLabel
                                {
                                    Text = comment.Text,
									TextColor = Config.ColorBlackTextOnWhite,
                                },
                            }
                        },
                    }
                };
                this.Children.Add(panel);

                panel.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(async () =>
                    {
                        if (comment.AthleteID != App.Repository.GetMyAthleteID())
                            return;

                        if (await App.Navigator.NavPage.DisplayAlert("Your comment", "Do you want to delete this comment?", "Delete", "Cancel") != true)
                            return;

                        PleaseWaitPage waitPage = new PleaseWaitPage();
                        await App.Navigator.NavPage.Navigation.PushModalAsync(waitPage);
                        bool ok = await App.WebService.DeleteComment(comment.ID);
                        await App.Navigator.NavPage.Navigation.PopModalAsync();

                        if (ok == false)
                            App.Navigator.DisplayAlertRegular("Failed to delete the comment");
                        else
                        {
                            comments.Remove(comment);
                            this.Fill(comments);
                        }
                    })
                });
            }
		}
	}
}
