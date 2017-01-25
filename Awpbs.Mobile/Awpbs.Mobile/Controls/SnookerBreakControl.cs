using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Forms.Controls;

namespace Awpbs.Mobile
{
	public class SnookerBreakControl : StackLayout
	{
        bool isHorizontal = false;// Config.App == MobileAppEnum.SnookerForVenues ? true : false;

        double panelPocketedBallsHeight = Config.IsTablet ? 100 : 60;
		int sizeOfBalls = 45;
		double buttonSpacing = Config.IsTablet ? 7 : 5;
        double padding = Config.IsTablet ? 20 : 5;

        int maxNumberOfBallsPerLine = Config.IsTablet ? 14 : 12;
		int maxNumberOfBallsInABreak = 36;
		int totalBreakBalls = 0;

		public bool isFoul = false;

        StackLayout stack_ballsOnTable;
		Label label_ballsOnTable;

		Label sbcLabelPointsLeft;
        int curA;
        int curB;

        BybButton redBall;
        BybButton coloredBall;
        BybLabel label_redsOnTable;
        BybNoBorderPicker pickerReds;
        BybNoBorderPicker pickerColors;

		private BallsOnTable localBallsOnTable;
		LargeNumberEntry2 framePointsEntryA;
		LargeNumberEntry2 framePointsEntryB;

        public VoiceButtonControl VoiceButtonControl { get; private set; }
        public HelpButtonControl HelpButtonControl { get; private set; }

        public event EventHandler DoneLeft;
        public event EventHandler DoneRight;
        public event EventHandler BallsChanged;

        public List<int> EnteredBalls
        {
            get
            {
                return this.getScoresFromPocketedBalls();
            }
        }

        public void ClearBalls()
        {
            this.panelPocketedBalls1.Children.Clear();
            this.panelPocketedBalls2.Children.Clear();
            this.panelPocketedBalls3.Children.Clear();
			this.updateFoul(false);
            this.updateControls();
            curA = (int)this.framePointsEntryA.Number;
            curB = (int)this.framePointsEntryB.Number;
        }

        SnookerMatchMetadata metadata;

        List<Button> buttonsBalls;
        Label labelPoints;
        Label buttonDelete;
		CheckBox foulCheckbox;
		Label labelNoPoints;

        // swipe panel
		SwipePanel swipePanel;
        StackLayout panelPocketedBalls1;
        StackLayout panelPocketedBalls2;
        StackLayout panelPocketedBalls3;

        // other
        StackLayout panelBalls;
        Grid grid;

		public SnookerBreakControl(SnookerMatchMetadata metadata, Label labelPointsLeft, BallsOnTable ballsOnTable, LargeNumberEntry2 entryA, LargeNumberEntry2 entryB)
        {
            this.metadata = metadata;
			this.sbcLabelPointsLeft = labelPointsLeft;
			this.localBallsOnTable = ballsOnTable;
			this.framePointsEntryA = entryA;
			this.framePointsEntryB = entryB;

			if ((this.framePointsEntryA.Number != null) &&
			    (this.framePointsEntryB.Number != null)) 
            {
                curA = (int)this.framePointsEntryA.Number;
                curB = (int)this.framePointsEntryB.Number;
            }
            else
            {
                curA = 0;
                curB = 0;
            }

            updatePointsDiff();

            /// pocketed balls
            /// 

            this.panelPocketedBalls1 = new StackLayout()
            {
                HeightRequest = Config.SmallBallSize + (Config.IsTablet ? 5 : 2),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                Spacing = Config.IsTablet ? 3 : 1,
                Padding = new Thickness(0),
            };
            this.panelPocketedBalls2 = new StackLayout()
            {
                HeightRequest = Config.SmallBallSize + (Config.IsTablet ? 5 : 2),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                Spacing = Config.IsTablet ? 3 : 1,
                Padding = new Thickness(0),
            };
            this.panelPocketedBalls3 = new StackLayout()
            {
                HeightRequest = Config.SmallBallSize + (Config.IsTablet ? 5 : 2),
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.Center,
                Spacing = Config.IsTablet ? 3 : 1,
                Padding = new Thickness(0),
            };
            this.labelPoints = new BybLabel()
            {
                Text = "",
                TextColor = Config.ColorTextOnBackground,
                VerticalOptions = LayoutOptions.Center,
            };
			this.labelNoPoints = new BybLabel()
			{
				IsVisible = true,

                Text = "Tap on balls, then swipe here to finish break",
				//Text = "Tap on balls\r\nThen swipe here to assign to a player",
                //Text = "Tap balls, then swipe here to assign to player",
				TextColor = Config.ColorGrayTextOnWhite,
                //FontFamily = Config.FontFamily,
                //FontSize = Config.DefaultFontSize - 1,
				VerticalOptions = LayoutOptions.Fill,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = TextAlignment.Center,
			};
			this.buttonDelete = new BybLabel ()
			{
				Text = "X",
				TextColor = Config.ColorTextOnBackground,
				WidthRequest = Config.IsTablet ? 35 : 25,
				HeightRequest = Config.IsTablet ? 35 : 30,
				HorizontalTextAlignment = TextAlignment.Center,
				VerticalTextAlignment = TextAlignment.Center,
			};
			this.buttonDelete.GestureRecognizers.Add (new TapGestureRecognizer () { Command = new Command (() => { this.buttonDelete_Clicked (this, EventArgs.Empty); }) });

			this.foulCheckbox = new CheckBox () {
				Checked = false,
				DefaultText   = "- tap here if a foul -",
				UncheckedText = "- tap here if a foul -",
				CheckedText = "Foul",
                FontSize = Config.DefaultFontSize,
                FontName = Config.FontFamily,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
			};
			this.foulCheckbox.CheckedChanged += (s1, e1) =>
			{
				updateFoul(this.foulCheckbox.Checked);
			};

            // container for balls and delete button
            var panelPocketedBallsActualBallsContainer = new StackLayout()
            {
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.FillAndExpand,
				HorizontalOptions = LayoutOptions.Center,
                Spacing = 0,
                Padding = new Thickness(0),
                //BackgroundColor = Color.Yellow,
                Children =
                {
					this.labelNoPoints,
					this.labelPoints,
                    new StackLayout()
                    {
                        Orientation = StackOrientation.Vertical,
                        Padding = new Thickness(0),
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            this.panelPocketedBalls1,
                            this.panelPocketedBalls2,
                            this.panelPocketedBalls3,
                        }
                    },
                    this.buttonDelete,
					this.foulCheckbox
                }
            };

			/// the panel with pocketed balls and tips
			/// 

			// draggable panel
			this.swipePanel = new SwipePanel (
				panelPocketedBallsActualBallsContainer,
				"Add to " + (this.metadata.OpponentAthleteName ?? "Opponent"),
                "Add to " + (this.metadata.PrimaryAthleteName ?? "You"),
                this.panelPocketedBallsHeight)
			{
                Opacity = 0.01,
				HeightRequest = this.panelPocketedBallsHeight,
				Padding = new Thickness(0, 0, 0, 0),
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Start,
				BackgroundColor = Config.ColorBlackBackground,
			};
            this.swipePanel.breakOwnerChanged += swipePanel_breakOwnerChanged;

			this.swipePanel.DraggedLeft += () =>
            {
			    if (true == this.swipePanel.getIsOpponentBreak())
                {
                    // if Swiped left, but was counting for opponent:
                    //   - change the break owner first 
                    //   - and update the frame score
			        this.swipePanel.setIsOpponentBreak(false);
                    updateOwnerChanged();
                }
                localBallsOnTable.breakFinished();
                updatePointsDiff();

				if (this.DoneLeft != null)
					this.DoneLeft(this, EventArgs.Empty);
			};
			this.swipePanel.DraggedRight += () =>
            {
			    if (false == this.swipePanel.getIsOpponentBreak())
                {
                    // if Swiped right, but was counting for "me":
                    //   - change the break owner first 
                    //   - and update the frame score
			        this.swipePanel.setIsOpponentBreak(true);
                    updateOwnerChanged();
                }
                localBallsOnTable.breakFinished();
                updatePointsDiff();

				if (this.DoneRight != null)
					this.DoneRight(this, EventArgs.Empty);
			};

            /// balls
            /// 
            buttonsBalls = new List<Button>();
            foreach (var color in Config.BallColors)
            {
                Color textColor = Color.White;
                if (Config.BallColors.IndexOf(color) == 0)
                    textColor = Color.Gray;
                if (Config.BallColors.IndexOf(color) == 2)
                    textColor = Color.Gray;
                Color borderColor = Color.Black;
                if (Config.BallColors.IndexOf(color) == 7)
                    borderColor = Config.ColorTextOnBackgroundGrayed;

                var buttonBall = new BybButton
                {
                    Text = Config.BallColors.IndexOf(color) == 0 ? "x" : Config.BallColors.IndexOf(color).ToString(),
                    BackgroundColor = color,
                    BorderColor = borderColor,
                    TextColor = textColor,
                    BorderWidth = 1,
                    BorderRadius = (int)(sizeOfBalls / 2),
                    HeightRequest = sizeOfBalls,
                    MinimumHeightRequest = sizeOfBalls,
                    WidthRequest = sizeOfBalls,
                    MinimumWidthRequest = sizeOfBalls,

                    FontFamily = Config.FontFamily,
                    FontSize = Config.LargerFontSize,
                    FontAttributes = Config.BallColors.IndexOf(color) == 1 ? FontAttributes.Bold : FontAttributes.None
                };
                buttonBall.Clicked += buttonBall_Clicked;
                buttonsBalls.Add(buttonBall);
            }

            this.grid = new Grid()
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                ColumnSpacing = 0,
                RowSpacing = 0,
                Padding = new Thickness(0, padding, 0, 0),
                BackgroundColor = Config.ColorBlackBackground,
                ColumnDefinitions =
                {
                    //new ColumnDefinition() { Width = new GridLength(90, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    //new ColumnDefinition() { Width = new GridLength(90, GridUnitType.Absolute) },
                },
                RowDefinitions =
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },
            };

            this.buildPanelBalls();
            //grid.Children.Add(panelBalls, 0, 0);//1, 0);

            this.HelpButtonControl = new HelpButtonControl()
            {
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(0, 0, 0, 0),
            };

			this.label_ballsOnTable = new BybLabel()
			{
                Text = "On table",
				TextColor = Config.ColorGrayTextOnWhite,
				VerticalOptions = LayoutOptions.Fill,
				VerticalTextAlignment = TextAlignment.Start,
				HorizontalOptions = LayoutOptions.Fill,
				HorizontalTextAlignment = TextAlignment.Start,
			};

            // add a ball
            this.label_redsOnTable = new BybLabel()
            {
                Text = "15",
                TextColor = Config.ColorTextOnBackground,
                VerticalOptions = LayoutOptions.Center,
            };
            int ballScore = 1; // or for lowest colored
            Color color2 = Config.BallColors[ballScore];
            Color borderColor2 = color2;
            Color textColor2 = Color.Black;
            int ballSizeMedium = (int)(Config.SmallBallSize * 1.5);

            redBall = new BybButton
            {
                IsEnabled = true,
                Text = "",
                BackgroundColor = color2,
                BorderColor = borderColor2,
                FontFamily = Config.FontFamily,
                FontSize = Config.LargerFontSize,
                TextColor = textColor2,
                BorderWidth = 1,
                BorderRadius = (int)(ballSizeMedium / 2),
                HeightRequest = ballSizeMedium,
                MinimumHeightRequest = ballSizeMedium,
                WidthRequest = ballSizeMedium,
                MinimumWidthRequest = ballSizeMedium,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            redBall.Clicked += (object sender, EventArgs e) =>
            {
                Console.WriteLine("Red ball clicked ");
                pickerReds.IsEnabled = true;
                pickerReds.Focus();
            };

            this.pickerReds = new BybNoBorderPicker()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
                IsVisible = false,
                IsEnabled = false
            };
            this.pickerReds.Items.Add("0");
            this.pickerReds.Items.Add("1");
            this.pickerReds.Items.Add("2");
            this.pickerReds.Items.Add("3");
            this.pickerReds.Items.Add("4");
            this.pickerReds.Items.Add("5");
            this.pickerReds.Items.Add("6");
            this.pickerReds.Items.Add("7");
            this.pickerReds.Items.Add("8");
            this.pickerReds.Items.Add("9");
            this.pickerReds.Items.Add("10");
            this.pickerReds.Items.Add("11");
            this.pickerReds.Items.Add("12");
            this.pickerReds.Items.Add("13");
            this.pickerReds.Items.Add("14");
            this.pickerReds.Items.Add("15");
            this.pickerReds.SelectedIndex = 0;
            this.pickerReds.SelectedIndexChanged += pickerReds_SelectedIndexChanged;

            this.pickerColors = new BybNoBorderPicker()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.Center,
                //HeightRequest = 50
            };
            this.pickerColors.Items.Add("2");
            this.pickerColors.Items.Add("3");
            this.pickerColors.Items.Add("4");
            this.pickerColors.Items.Add("5");
            this.pickerColors.Items.Add("6");
            this.pickerColors.Items.Add("7");
            this.pickerColors.SelectedIndex = 0;
            this.pickerColors.SelectedIndexChanged += pickerColors_SelectedIndexChanged;

            // add colored ball
            ballScore = 2; // or for lowest colored
            color2 = Config.BallColors[ballScore];
            borderColor2 = color2;
            textColor2 = Color.Gray;

            coloredBall = new BybButton
            {
                IsEnabled = true,
                Text = "",
                BackgroundColor = color2,
                BorderColor = borderColor2,
                FontFamily = Config.FontFamily,
                FontSize = Config.LargerFontSize,
                TextColor = textColor2,
                BorderWidth = 1,
                BorderRadius = (int)(ballSizeMedium / 2),
                HeightRequest = ballSizeMedium,
                MinimumHeightRequest = ballSizeMedium,
                WidthRequest = ballSizeMedium,
                MinimumWidthRequest = ballSizeMedium,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            coloredBall.Clicked += (object sender, EventArgs e) =>
            {
                if (0 != localBallsOnTable.numberOfReds)
                {
                    Console.WriteLine("Colored ball clicked: but there are reds on the table, so ignore");
                }
                else
                {
                    Console.WriteLine("Colored ball clicked ");
                    pickerColors.IsEnabled = true;
                    pickerColors.Focus();
                }
            };
            this.stack_ballsOnTable = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(Config.IsTablet ? 30 : 15,0,0,0),
                Spacing = 10,
                //HorizontalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
			        label_ballsOnTable,
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Start,
                        Spacing = buttonSpacing,
                        Padding = new Thickness(0),
                        Children =
                        {
			                redBall, 
                            label_redsOnTable, 
                            pickerReds
                        }
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Start,
                        Spacing = buttonSpacing,
                        Padding = new Thickness(0),
                        Children =
                        {
			                coloredBall, 
                            pickerColors
                        }
                    },
                }
            };

            this.VoiceButtonControl = new VoiceButtonControl()
            {
                HorizontalOptions = LayoutOptions.Start,
                Padding = new Thickness(0, 0, 0, 0),
            };
            grid.Children.Add(new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Padding = new Thickness(Config.IsTablet ? 30 : 15,0,0,0),
                Spacing = 10,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    this.stack_ballsOnTable,
                    (new BoxView()
                    {
                        HeightRequest = 40,
                        BackgroundColor = Config.ColorBlackBackground,
                    }),
                    this.HelpButtonControl,
                    this.VoiceButtonControl,
                }
            }, 0, 0);

            /// content
            /// 
            this.BackgroundColor = Config.ColorBlackBackground;
            this.Padding = new Thickness(0);
            this.Spacing = 0;
            this.Orientation = StackOrientation.Vertical;

            this.Children.Add(new BoxView()
            {
                HeightRequest = 1,
                BackgroundColor = Config.ColorBackground,
            });
            this.Children.Add(this.swipePanel);
            this.Children.Add(new BoxView()
            {
                HeightRequest = 1,
                BackgroundColor = Config.ColorBackground,
            });
            this.Children.Add(grid);

            this.updateControls();

            // update pickers 
            updateBallsOnTable_ballsChanged(); 

        }

        System.Timers.Timer timerToMakeSwipePanelVisible;

        protected override void OnSizeAllocated(double width, double height)
        {
			if (height > 100)
			{
                // height of the balls panel
				double totalHeightAvailableForBalls = height - panelPocketedBallsHeight - this.Padding.Top - this.Padding.Bottom;
				totalHeightAvailableForBalls -= Config.IsTablet ? 15 : 5; // padding below
				this.grid.HeightRequest = totalHeightAvailableForBalls;

				// set an optimal size for balls and spacing between balls
				double spaceForEach = (totalHeightAvailableForBalls - padding) / (this.isHorizontal ? 3 : 5);
				this.sizeOfBalls = (int)(spaceForEach * 0.95);
				this.buttonSpacing = spaceForEach - this.sizeOfBalls;
				if (this.sizeOfBalls > 100)
					this.sizeOfBalls = 100;
				foreach (var ball in buttonsBalls)
                {
					ball.WidthRequest = ball.MinimumWidthRequest = ball.HeightRequest = ball.MinimumHeightRequest = this.sizeOfBalls;
					ball.BorderRadius = this.sizeOfBalls / 2;
					if (ball.Parent as StackLayout != null)
						((StackLayout)ball.Parent).Spacing = this.buttonSpacing;
				}
				this.panelBalls.Spacing = this.buttonSpacing;
			}

            if (this.timerToMakeSwipePanelVisible == null && this.swipePanel.Opacity < 0.1)
            {
                this.timerToMakeSwipePanelVisible = new System.Timers.Timer(2500);
                this.timerToMakeSwipePanelVisible.Elapsed += (s1, e1) =>
                {
                    this.timerToMakeSwipePanelVisible.Stop();
                    Device.BeginInvokeOnMainThread(() => { this.swipePanel.Opacity = 1.0; });
                };
                this.timerToMakeSwipePanelVisible.Start();
            }

            base.OnSizeAllocated(width, height);
        }

        void buildPanelBalls()
        {
            if (this.panelBalls != null)
                this.grid.Children.Remove(this.panelBalls);

            if (this.isHorizontal == true)
            {
                this.panelBalls = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(0, 0, 0, 0),
                    Spacing = buttonSpacing,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Start,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[3],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[4],
                                buttonsBalls[5],
                                buttonsBalls[6],
                                buttonsBalls[1],
                                buttonsBalls[7],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Start,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[2],
                            }
                        },
                    }
                };
            }
            else
            {
                this.panelBalls = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Padding = new Thickness(0, 0, 0, 0),
                    Spacing = buttonSpacing,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[7],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[1],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[6],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[5],
                            }
                        },
                        new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            HorizontalOptions = LayoutOptions.Center,
                            Spacing = buttonSpacing,
                            Padding = new Thickness(0),
                            Children =
                            {
                                buttonsBalls[3],
                                buttonsBalls[4],
                                buttonsBalls[2],
                            }
                        },
                    }
                };
            }

            grid.Children.Add(panelBalls, 0, 0);
        }

        void buttonBall_Clicked(object sender, EventArgs e)
        {
			// Wait until initialization completes and swipe panel becomes visible
			// before processing ball taps.
			//
			// This fixes the problem when you tap on a ball immediately after starting a frame
			// and it gets assigned to the player on the right for some reason
			// (draggablePanel_Scrolled() gets called)
			if (this.swipePanel.Opacity < 1.0)
				return;

			// Limit the number of balls to enter in a break (36), if it's more - do nothing
			if (totalBreakBalls >= maxNumberOfBallsInABreak)
				return;

			totalBreakBalls++;

            Button buttonBall = sender as Button;
            int ballScore = 0;
            int.TryParse(buttonBall.Text, out ballScore);
            if (ballScore == 0)
                return;

			this.addPocketedBall(ballScore);

            this.doOnBallsChanged();
			this.pronounceScore ();
        }

        void buttonDelete_Clicked(object sender, EventArgs e)
        {
            StackLayout curPanel;
            int ballIdx;

            if (this.panelPocketedBalls3.Children.Count > 0)
            {
                curPanel = this.panelPocketedBalls3;
                ballIdx = this.panelPocketedBalls3.Children.Count - 1;
            }
            else if (this.panelPocketedBalls2.Children.Count > 0)
            {
                curPanel = this.panelPocketedBalls2;
                ballIdx = this.panelPocketedBalls2.Children.Count - 1;
            }
            else if (this.panelPocketedBalls1.Children.Count > 0)
            {
                curPanel = this.panelPocketedBalls1;
                ballIdx = this.panelPocketedBalls1.Children.Count - 1;
            }
            else
            {
                return;
            }

            var view = curPanel.Children.ElementAt(ballIdx);
            Button button = view as Button;
            int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);

            Console.WriteLine("buttonDelete_Clicked ballScore" + ballScore);

            curPanel.Children.RemoveAt(ballIdx);

			if (totalBreakBalls != 0)
				totalBreakBalls--;
			
            this.doOnBallsChanged();

            // update ballsOnTable
            int prevBall = 0;
            if (ballIdx > 0)
            {
                view = curPanel.Children.ElementAt(ballIdx-1);
                button = view as Button;
                prevBall = Config.BallColors.IndexOf(button.BackgroundColor);

            }
			localBallsOnTable.ballRemovedFromBreak(ballScore, prevBall);
            updateBallsOnTable_ballsChanged();
        }

        void doOnBallsChanged()
        {
            if (this.BallsChanged != null)
                this.BallsChanged(this, EventArgs.Empty);
            this.updateControls();
            this.panelPocketedBalls3.IsVisible = this.panelPocketedBalls3.Children.Count() > 0;
            this.panelPocketedBalls2.IsVisible = this.panelPocketedBalls2.Children.Count() > 0;
            updateFrameScore();
        }

        void addPocketedBall(int ballScore)
        {
            // update ballsOnTable
			localBallsOnTable.ballPocketed (ballScore);
            updateBallsOnTable_ballsChanged();

			if ((this.framePointsEntryA.Number != null) &&
			    (this.framePointsEntryB.Number != null)) 
            {
			    if (this.swipePanel.getIsOpponentBreak())
                {
                    this.framePointsEntryB.Number += ballScore;
                }
                else
                {
                    this.framePointsEntryA.Number += ballScore;
                }
            }
            updatePointsDiff();

            Color color = Config.BallColors[ballScore];
            Color borderColor = color;
            if (ballScore == 7)
                borderColor = Color.Gray;
            Color textColor = Color.White;
            if (ballScore == 2)
                textColor = Color.Black;

            var ball = new BybButton
            {
                IsEnabled = false,
                Text = "",
                BackgroundColor = color,
                BorderColor = borderColor,
                FontFamily = Config.FontFamily,
                FontSize = Config.LargerFontSize,
                TextColor = textColor,
                BorderWidth = 1,
                BorderRadius = (int)(Config.SmallBallSize / 2),
                HeightRequest = Config.SmallBallSize,
                MinimumHeightRequest = Config.SmallBallSize,
                WidthRequest = Config.SmallBallSize,
                MinimumWidthRequest = Config.SmallBallSize,
                VerticalOptions = LayoutOptions.Center
            };
            if (this.panelPocketedBalls1.Children.Count() < maxNumberOfBallsPerLine)
                this.panelPocketedBalls1.Children.Add(ball);
            else if (this.panelPocketedBalls2.Children.Count() < maxNumberOfBallsPerLine)
                this.panelPocketedBalls2.Children.Add(ball);
            else
                this.panelPocketedBalls3.Children.Add(ball);

            this.updateControls();
        }

        List<int> getScoresFromPocketedBalls()
        {
            List<int> ballScores = new List<int>();
            foreach (var view in this.panelPocketedBalls1.Children)
            {
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);
                ballScores.Add(ballScore);
            }
            foreach (var view in this.panelPocketedBalls2.Children)
            {
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);
                ballScores.Add(ballScore);
            }
            foreach (var view in this.panelPocketedBalls3.Children)
            {
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);
                ballScores.Add(ballScore);
            }
            return ballScores;
        }

        void updateControls()
        {
			totalBreakBalls = EnteredBalls.Count();
            var points = EnteredBalls.Sum();

            if (points > 0)
                this.labelPoints.Text = " " + points.ToString() + " ";
            this.labelPoints.IsVisible = points > 0;
            this.buttonDelete.IsVisible = points > 0;

			// Show "Foul" check box if only there is only one ball in a break and it's between 4-7
			if ((EnteredBalls.Count == 1) && (EnteredBalls.ElementAt (0) > 3)) {
				this.foulCheckbox.IsVisible = true;
			} else {
				this.foulCheckbox.IsVisible = false;
				updateFoul (false); // don't think it's really needed, just in case
			}
			this.labelNoPoints.IsVisible = points == 0;
            this.swipePanel.SwipeButtonsOpacity = points == 0 ? 0.5 : 1.0;
        }

		void updateFoul(bool localIsFoul)
		{
			if (localIsFoul)
			{
				this.foulCheckbox.Checked = true;
				this.foulCheckbox.TextColor = Config.ColorRed;
				this.isFoul = true;
			}
			else
			{
				this.foulCheckbox.Checked = false;
				this.foulCheckbox.TextColor = Color.White;
				this.isFoul = false;
			}

            // update ballsOnTable and points remaining
            if (this.panelPocketedBalls1.Children.Count == 1) 
            {
                var view = this.panelPocketedBalls1.Children.ElementAt(0);
                Button button = view as Button;
                int ballScore = Config.BallColors.IndexOf(button.BackgroundColor);

                if (ballScore > 3)
                { 
                    if (localIsFoul) 
                    {
                        localBallsOnTable.markFoul();
                    }
                    else
                    {
                        localBallsOnTable.ballPocketed(ballScore);
                    }
                }
            }

            updateBallsOnTable_ballsChanged();
            updatePointsDiff();
		}

        void pronounceScore()
        {
			if (!App.UserPreferences.IsVoiceOn)
				return;
				
            var ballScores = this.getScoresFromPocketedBalls();
            int score = ballScores.Sum();
            if (score == 0)
                return;

			if (App.UserPreferences.IsVoiceOn) 
			{
				App.ScorePronouncer.Pronounce (score.ToString(), App.UserPreferences.Voice, App.UserPreferences.VoiceRate, App.UserPreferences.VoicePitch);
			}
        }

        private void swipePanel_breakOwnerChanged(object sender, EventArgs e)
        {
            updateOwnerChanged();   
        }

        //
        // If break owner has been changed in the middle of a break,
        // update frame score and points remaining
        //
        private void updateOwnerChanged()
        {
			if (0 == EnteredBalls.Count())
            {
               return;
            }

            int curBreakPoints = EnteredBalls.Sum();

			if (this.swipePanel.getIsOpponentBreak())
            {
                Console.WriteLine("SwipePanel.isOpponentsBreak is now TRUE\n"); 
                this.framePointsEntryA.Number -= curBreakPoints; 
                this.framePointsEntryB.Number += curBreakPoints; 
            }
            else
            {
                Console.WriteLine("SwipePanel.isOpponentsBreak is now FALSE\n"); 
                this.framePointsEntryA.Number += curBreakPoints; 
                this.framePointsEntryB.Number -= curBreakPoints; 
            }
            updatePointsDiff();
        }

        //
        // Update frame score based on the balls in current break
        //
        private void updateFrameScore()
        {
            int curBreakPoints = EnteredBalls.Sum();

			if (this.swipePanel.getIsOpponentBreak())
            {
                this.framePointsEntryA.Number = curA;
                this.framePointsEntryB.Number = curB + curBreakPoints;
            }
            else
            {
                this.framePointsEntryA.Number = curA + curBreakPoints;
                this.framePointsEntryB.Number = curB;
            }
            updatePointsDiff();
        }

        // update frame score because one of the past breaks changed
        public void updateFrameScoreOnBreakEdit(int scoreA, int scoreB)
        {
           Console.WriteLine("updateFrameScoreOnBreakEdit " + scoreA + " " + scoreB);
           curA = scoreA;
           curB = scoreB;

           // then add points from current break
           updateFrameScore();
        }

        private void updatePointsDiff()
        {
			int scoreDifference = 0;

          	if ((this.framePointsEntryA.Number != null) &&
			    (this.framePointsEntryB.Number != null)) 
            { 
                scoreDifference = (int)this.framePointsEntryA.Number - (int)this.framePointsEntryB.Number;
			    scoreDifference = System.Math.Abs (scoreDifference);
            }

			int pointsLeft = localBallsOnTable.getPointsRemaining ();
			string pointsLeftString = "Remaining: " + Convert.ToString(pointsLeft);
			pointsLeftString += " Diff: " + Convert.ToString (scoreDifference);
			sbcLabelPointsLeft.Text = pointsLeftString;
			sbcLabelPointsLeft.TextColor = (scoreDifference > pointsLeft) ? Config.ColorRedBackground : Config.ColorTextOnBackgroundGrayed;
        }

        private void coloredBall_changeColor(int ballScore)
        {
           if ( (ballScore < 2) && (ballScore > 7) ) 
           {
               Console.WriteLine("coloredBall_changeColor ERROR: ballScore " + ballScore);
           }
            // Change colored ball color if needed
            Color color3 = Config.BallColors[ballScore];
            Color borderColor3 = color3;
            if (ballScore == 7)
                borderColor3 = Color.Gray;

            coloredBall.BackgroundColor = color3;
            coloredBall.BorderColor = borderColor3;
        }

        private void updateBallsOnTable_ballsChanged()
        {
            int savedColor = this.pickerColors.SelectedIndex;

            int newRedsIdx = localBallsOnTable.numberOfReds;
			if ( (newRedsIdx < 0 ) && (newRedsIdx > 14) )
			{
			    Console.WriteLine ("updateBallsOnTable_ballsChanged(): ERROR");
			    Console.WriteLine ("newRedsIdx is invalid");
			}
            else
            {
                this.pickerReds.SelectedIndex = newRedsIdx;
            }

            int newColorIdx = localBallsOnTable.lowestColor - 2;
			if ( (newColorIdx < 0 ) && (newColorIdx > 5) )
			{
			    Console.WriteLine ("updateBallsOnTable_ballsChanged(): ERROR");
			    Console.WriteLine ("newColorIdx is invalid");
			}
            else
            {
                this.pickerColors.SelectedIndex = newColorIdx;
            }

            if (savedColor != this.pickerColors.SelectedIndex)
            {
                coloredBall_changeColor(localBallsOnTable.lowestColor);
            }

            Console.WriteLine("updateBallsOnTable_ballsChanged:");
            Console.WriteLine("numberOfReds " + localBallsOnTable.numberOfReds);
            Console.WriteLine("lowestColor " + localBallsOnTable.lowestColor);
        }

        public void breakEdited(SnookerBreak oldBreak, SnookerBreak newBreak)
        {
            // to make it simple, lets remove ALL the balls that were in the break
            // BEFORE it was edited, 
            // then add all new balls AFTER it was edited
            List<int> ballScoreList= new List<int>();
            ballScoreList = oldBreak.Balls;
            int numBalls = ballScoreList.Count();
            for (int ballIdx = numBalls-1; ballIdx >= 0; ballIdx--)
            {
                int prevBall = 0;
                int curBall = ballScoreList.ElementAt(ballIdx);
                if (ballIdx > 0)
                {
                    prevBall = ballScoreList.ElementAt(ballIdx-1);
                }
			    localBallsOnTable.ballRemovedFromBreak(curBall, prevBall);
            }

            ballScoreList.Clear();
            ballScoreList = newBreak.Balls;
            numBalls = ballScoreList.Count();
            for (int ballIdx = 0; ballIdx < numBalls; ballIdx++)
            {
                int curBall = ballScoreList.ElementAt(ballIdx);
			    localBallsOnTable.ballPocketed (curBall);
            }

            updateBallsOnTable_ballsChanged();
            updatePointsDiff();
        }

        public void breakDeleted(SnookerBreak deletedBreak)
        {
            // Remove ALL the balls that were in the break
            List<int> ballScoreList= new List<int>();
            ballScoreList = deletedBreak.Balls;
            int numBalls = ballScoreList.Count();
            for (int ballIdx = numBalls-1; ballIdx >= 0; ballIdx--)
            {
                int prevBall = 0;
                int curBall = ballScoreList.ElementAt(ballIdx);
                if (ballIdx > 0)
                {
                    prevBall = ballScoreList.ElementAt(ballIdx-1);
                }
			    localBallsOnTable.ballRemovedFromBreak(curBall, prevBall);
            }

            updateBallsOnTable_ballsChanged();
            updatePointsDiff();
        }

        private void pickerReds_SelectedIndexChanged(object sender, EventArgs e)
        {
            localBallsOnTable.numberOfReds = this.pickerReds.SelectedIndex;

            this.label_redsOnTable.Text = localBallsOnTable.numberOfReds.ToString();
            Console.WriteLine("numberOfReds " + localBallsOnTable.numberOfReds);
            pickerReds.IsEnabled = false;
            pickerReds.IsVisible = false;

            updatePointsDiff();
        }

        private void pickerColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            // index 0: 2, etc.
            localBallsOnTable.lowestColor = this.pickerColors.SelectedIndex + 2;

            Console.WriteLine("lowestColor " + localBallsOnTable.lowestColor);


            pickerColors.IsEnabled = false;
            pickerColors.IsVisible = false;

            coloredBall_changeColor(localBallsOnTable.lowestColor);
            updatePointsDiff();
        }


    }
}
