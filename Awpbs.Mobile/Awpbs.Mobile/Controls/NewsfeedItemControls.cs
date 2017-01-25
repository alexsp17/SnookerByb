using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Awpbs.Mobile
{
    public class NewsfeedItemBaseControl : StackLayout
    {
        public event EventHandler NeedsARefresh;

        public bool TreatAsASingleItem { get; set; }

        protected NewsfeedItemWebModel item;

        protected Image imageAthlete;

        protected StackLayout panelCommentsCount;
        protected Label labelDate;
        protected Label labelAthleteName;
        protected Label labelAthlete2Name;
        protected Grid frameAthleteImage;

        protected Label labelVenue;

        protected StackLayout panelLikes;
        protected StackLayout panelComments;
        protected Image imageLike;
        protected Image imageComments;
        protected Label labelLikesCount;
        protected Label labelCommentsCount;

        protected double imageSize = Config.IsTablet ? Config.PersonImageSize : Config.PersonImageSize / 2;
        protected double imageExtraPadding = Config.IsTablet ? 10 : 0;

        public bool ShowCommentsCount
        {
            get
            {
                return this.panelComments.IsVisible;
            }
            set
            {
                this.panelComments.IsVisible = value;
            }
        }

        public NewsfeedItemBaseControl(NewsfeedItemWebModel item, bool treatAsASingleItem)
        {
            this.TreatAsASingleItem = treatAsASingleItem;

            this.Orientation = StackOrientation.Horizontal;
            this.Spacing = 0;
            this.Padding = new Thickness(5, 5, 5, 5);
            this.BackgroundColor = Color.White;
            this.HorizontalOptions = LayoutOptions.FillAndExpand;

            // athlete image
            this.imageAthlete = new Image()
            {
                WidthRequest = imageSize,
                HeightRequest = imageSize,
            };
            this.frameAthleteImage = new Grid
            {
                BackgroundColor = Color.White,
                Padding = new Thickness(this.imageExtraPadding, 0, this.imageExtraPadding, 0),
				Children =
				{
					this.imageAthlete,
				}
                //Content = this.imageAthlete,
            };

            // date
            this.labelDate = new BybLabel
            {
                TextColor = Config.ColorGrayTextOnWhite,
                FontSize = Config.SmallFontSize,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
            };

            // likes panel
            this.imageLike = new Image()
            {
                Source = new FileImageSource() { File = "like1.png" },
                HeightRequest = Config.LikeImageSize,
                WidthRequest = Config.LikeImageSize,
            };
            this.imageComments = new Image()
            {
                Source = new FileImageSource() {  File = "right.png" },
                HeightRequest = Config.CommentImageSize,
                WidthRequest = Config.CommentImageSize,
            };
            this.labelLikesCount = new BybLabel
            {
            };
			this.panelLikes = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				//HorizontalOptions = LayoutOptions.Center,
				Padding = new Thickness(0, 0, Config.IsTablet ? 5 : 0, 5),
				Spacing = 3,
				Children =
				{
					this.labelLikesCount,
					this.imageLike
				}
			};
//			this.panelLikes = new Frame()
//            {
//                Padding = new Thickness(0, 0, Config.IsTablet ? 5 : 0, 5),
//                Content = new StackLayout
//                {
//                    Orientation = StackOrientation.Horizontal,
//                    HorizontalOptions = LayoutOptions.Center,
//                    Spacing = 3,
//                    Children =
//                        {
//                            this.labelLikesCount,
//                            this.imageLike
//                        }
//                }
//            };
            this.labelCommentsCount = new BybLabel
            {
            };
			this.panelComments = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.Center,
				Padding = new Thickness(0,5,Config.IsTablet ? 5 : 0,0),
				Spacing = 5,
				Children =
				{
					this.labelCommentsCount,
					this.imageComments,
				}
			};
//            this.frameComments = new Frame()
//            {
//                Padding = new Thickness(0,5,Config.IsTablet ? 5 : 0,0),
//                Content = new StackLayout
//                {
//                    Orientation = StackOrientation.Horizontal,
//                    HorizontalOptions = LayoutOptions.Center,
//                    Spacing = 5,
//                    Children =
//                    {
//                        this.labelCommentsCount,
//                        this.imageComments,
//                    }
//                }
//            };
            this.panelCommentsCount = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 0,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
					panelLikes,
					panelComments,
                }
            };

            // athlete name
            this.labelAthleteName = new BybLabel()
            {
                HorizontalOptions = LayoutOptions.Start,
                FontAttributes = this.TreatAsASingleItem ? FontAttributes.None : FontAttributes.Bold,
				TextColor = Config.ColorBlackTextOnWhite,
            };

            // athlete 2 name
            if (item.Athlete2ID > 0)
            {
                this.labelAthlete2Name = new BybLabel()
                {
                    HorizontalOptions = LayoutOptions.Start,
                    FontAttributes = this.TreatAsASingleItem ? FontAttributes.Bold : FontAttributes.None,
					TextColor = Config.ColorBlackTextOnWhite,
                };
            }

            // venue
            if (string.IsNullOrEmpty(item.VenueName) == false)
            {
                FormattedString formattedString = new FormattedString();
                formattedString.Spans.Add(new Span() { 
					Text = "@ ", 
					ForegroundColor = Config.ColorTextOnBackgroundGrayed 
				});
                formattedString.Spans.Add(new Span() { 
					Text = item.VenueName, 
					ForegroundColor = Config.ColorBlackTextOnWhite 
				});
                this.labelVenue = new BybLabel()
                {
                    FormattedText = formattedString
                };
            }

            /// events
            /// 
            this.imageAthlete.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (this.TreatAsASingleItem)
                        await this.onOpenItem();
                    else
                        await App.Navigator.GoToPersonProfile(item.AthleteID);
                }),
                NumberOfTapsRequired = 1
            });
            this.labelAthleteName.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (this.TreatAsASingleItem)
                        await this.onOpenItem();
                    else
                        await App.Navigator.GoToPersonProfile(item.AthleteID);
                }),
                NumberOfTapsRequired = 1
            });
            if (this.labelAthlete2Name != null)
                this.labelAthlete2Name.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        if (this.TreatAsASingleItem)
                            await this.onOpenItem();
                        else
                            await App.Navigator.GoToPersonProfile(item.Athlete2ID);
                    }),
                    NumberOfTapsRequired = 1
                });
            if (this.labelVenue != null)
                this.labelVenue.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(async () =>
                    {
                        if (this.TreatAsASingleItem)
                            await this.onOpenItem();
                        else
                            await App.Navigator.GoToVenueProfile(item.VenueID);
                    }),
                    NumberOfTapsRequired = 1
                });
			panelLikes.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    if (item.Liked)
                        item.LikesCount -= 1;
                    else
                        item.LikesCount += 1;

                    item.Liked = !item.Liked;
                    
                    this.Fill(this.item);

                    bool ok = await App.WebService.SetLike(item.ItemType, item.ID, item.Liked);
                    if (ok == false)
                        App.Navigator.DisplayAlertError("Couldn't set the like. Internet issues?");
                }),
                NumberOfTapsRequired = 1
            });

            panelComments.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

			this.GestureRecognizers.Add(new TapGestureRecognizer
			{
				Command = new Command(async () =>
					{
						await this.onOpenItem();
					}),
				NumberOfTapsRequired = 1
			});

            this.Fill(item);
        }

        public void Fill(NewsfeedItemWebModel item)
        {
            this.item = item;

            this.imageAthlete.Source = App.ImagesService.GetImageSource(item.AthletePicture, BackgroundEnum.White);
            this.labelDate.Text = DateTimeHelper.DateToString(item.Time);
            this.labelAthleteName.Text = item.AthleteName;
            if (this.labelAthlete2Name != null)
                this.labelAthlete2Name.Text = item.Athlete2Name;

            this.imageLike.Source = new FileImageSource() { File = item.Liked ? "like2.png" : "like1.png" };
            this.imageComments.Source = new FileImageSource() { File = item.CommentsCount > 0 ? "right2.png" : "right1.png" };
            this.labelLikesCount.Text = item.LikesCount.ToString();
            this.labelLikesCount.TextColor = item.LikesCount > 0 ? Config.ColorBlackTextOnWhite : Config.ColorGrayTextOnWhite;
            this.labelCommentsCount.Text = item.CommentsCount.ToString();
            this.labelCommentsCount.TextColor = item.CommentsCount > 0 ? Config.ColorBlackTextOnWhite : Config.ColorGrayTextOnWhite;
        }

        protected virtual async Task onOpenItem()
        {
            if (TreatAsASingleItem == false)
                return;
            //if (this.IgnoreTapping)
            //    return;

			if (App.Navigator.GetOpenedPage (typeof(NewsfeedItemPage)) != null)
				return;

            var page = new NewsfeedItemPage(item, false);
            await App.Navigator.NavPage.PushAsync(page);
            page.Disappearing += (s1, e1) =>
            {
                if (page.AnythingChanged)
                    this.fireNeedsARefresh();
            };
        }

        protected void fireNeedsARefresh()
        {
            if (this.NeedsARefresh != null)
                this.NeedsARefresh(this, EventArgs.Empty);
        }
    }

    public class NewsfeedItemPostControl : NewsfeedItemBaseControl
    {
        public NewsfeedItemPostControl(NewsfeedItemWebModel item, bool treatAsASingleItem) : base(item, treatAsASingleItem)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span() { Text = "Posted \"", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
            formattedString.Spans.Add(new Span() { Text = item.Text, ForegroundColor = Config.ColorBlackTextOnWhite });
            formattedString.Spans.Add(new Span() { Text = "\"", ForegroundColor = Config.ColorTextOnBackgroundGrayed });

            Label labelContent = new BybLabel
            {
                FormattedText = formattedString,
                TextColor = Config.ColorBlackTextOnWhite,
                //HorizontalOptions = LayoutOptions.FillAndExpand,
                WidthRequest = 200,
            };
            labelContent.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(this.frameAthleteImage);
            var panel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Spacing = 3,
                Padding = new Thickness(5, 0, 0, 0),
                Children =
                {
                    labelDate,
                    this.labelAthleteName,
                    labelContent,
                }
            };
            this.Children.Add(panel);
            this.Children.Add(panelCommentsCount);
        }
    }

    public class NewsfeedItemResultControl : NewsfeedItemBaseControl
    {
        public NewsfeedItemResultControl(NewsfeedItemWebModel item, bool treatAsASingleItem) : base(item, treatAsASingleItem)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span() { Text = "Recorded a break of ", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
            formattedString.Spans.Add(new Span() { Text = item.Text, ForegroundColor = Config.ColorBlackTextOnWhite, FontAttributes = FontAttributes.Bold, FontSize = Config.DefaultFontSize });
            Label labelContent = new BybLabel
            {
                FormattedText = formattedString
            };
            labelContent.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(this.frameAthleteImage);
            var panel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(5, 0, 0, 0),
                Spacing = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    labelDate,
                    this.labelAthleteName,
                    labelContent,
                }
            };
            if (this.labelVenue != null)
                panel.Children.Add(labelVenue);
            this.Children.Add(panel);
            this.Children.Add(panelCommentsCount);
        }
    }

    public class NewsfeedItemScoreControl : NewsfeedItemBaseControl
    {
        public NewsfeedItemScoreControl(NewsfeedItemWebModel item, bool treatAsASingleItem) : base(item, treatAsASingleItem)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span() { Text = "Recorded a score ", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
            formattedString.Spans.Add(new Span() { Text = item.Text, ForegroundColor = Config.ColorBlackTextOnWhite, FontSize = Config.DefaultFontSize });
            Label labelContent = new BybLabel
            {
                FormattedText = formattedString
            };
            labelContent.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(new BoxView() { WidthRequest = imageSize + imageExtraPadding*2 } );
            var panel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(0, 0, 0, 0),
                Spacing = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    labelDate,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Center,
                        Spacing = 3,
                        Children =
                        {
                            this.labelAthleteName,
                            new BybLabel { Text = "vs.", TextColor = Config.ColorGrayTextOnWhite, HorizontalOptions = LayoutOptions.Start, VerticalTextAlignment = TextAlignment.Center },
                            this.labelAthlete2Name,
                        }
                    },
                    labelContent,
                }
            };
            if (this.labelVenue != null)
                panel.Children.Add(labelVenue);
            this.Children.Add(panel);
            this.Children.Add(panelCommentsCount);
        }
    }

    public class NewsfeedItemGameHostControl : NewsfeedItemBaseControl
    {
        public NewsfeedItemGameHostControl(NewsfeedItemWebModel item, bool treatAsASingleItem) : base(item, treatAsASingleItem)
        {
            DateTime when = item.TimeOfEvent.Value.ToLocalTime();

            Label labelText = new BybLabel
            {
                Text = "Invites to play snooker",
                TextColor = Config.ColorGrayTextOnWhite,
                HorizontalOptions = LayoutOptions.FillAndExpand,
            };
            labelText.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span() { Text = "On ", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
            formattedString.Spans.Add(new Span() { Text = DateTimeHelper.DateAndTimeToString(when), ForegroundColor = Config.ColorBlackTextOnWhite, FontSize = Config.DefaultFontSize });
            Label labelWhen = new BybLabel { FormattedText = formattedString };
            labelWhen.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(this.frameAthleteImage);
            var panel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(5, 0, 0, 0),
                Spacing = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    labelDate,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        VerticalOptions = LayoutOptions.Center,
                        Spacing = 3,
                        Children =
                        {
                            this.labelAthleteName,
                        }
                    },
                    labelText,
                    labelWhen,
                }
            };
            if (this.labelVenue != null)
                panel.Children.Add(labelVenue);
            this.Children.Add(panel);
            this.Children.Add(panelCommentsCount);
        }

        protected override async Task onOpenItem()
        {
			if (App.Navigator.GetOpenedPage (typeof(GameHostPage)) != null)
				return;
			
            var page = new GameHostPage(false, false);
            page.OpenGameHost(this.item.ID);
            await App.Navigator.NavPage.Navigation.PushAsync(page);
            page.Disappearing += (s1, e1) =>
            {
                if (page.AnythingChanged)
                    this.fireNeedsARefresh();
            };
        }
    }

    public class NewsfeedItemNewUserControl : NewsfeedItemBaseControl
    {
        public NewsfeedItemNewUserControl(NewsfeedItemWebModel item, bool treatAsASingleItem) : base(item, treatAsASingleItem)
        {
            FormattedString formattedString = new FormattedString();
            formattedString.Spans.Add(new Span() { Text = "New community member", ForegroundColor = Config.ColorTextOnBackgroundGrayed });
            Label labelContent = new BybLabel
            {
                FormattedText = formattedString
            };
            labelContent.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    await this.onOpenItem();
                }),
                NumberOfTapsRequired = 1
            });

            this.Children.Add(this.frameAthleteImage);
            var panel = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(5, 0, 0, 0),
                Spacing = 3,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Children =
                {
                    labelDate,
                    this.labelAthleteName,
                    labelContent,
                }
            };
            this.Children.Add(panel);
            this.Children.Add(panelCommentsCount);
        }
    }
}
