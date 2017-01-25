using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Timers;

namespace Awpbs.Mobile
{
	public class SwipePanel : Frame
	{
        static double imageSize = Config.IsTablet ? 25 : 20;

        bool isOpponentsBreak;

		ScrollView scrollView;
		Grid grid;
        StackLayout imageLeftContainer;
        StackLayout imageRightContainer;
        Image imageLeft;
        Image imageRight;


        public event EventHandler breakOwnerChanged;


		public event Action DraggedLeft;
		public event Action DraggedRight;

        public double SwipeButtonsOpacity
        {
            get
            {
                return this.imageLeft.Opacity;
            }
            set
            {
                this.imageLeft.Opacity = value;
                this.imageRight.Opacity = value;
            }
        }
		
		public SwipePanel (Layout panelToDrag, string textLeft, string textRight, double height)
		{
            isOpponentsBreak = false;

			grid = new Grid ()
			{
				HeightRequest = height,
				Padding = new Thickness(0,0,0,0),
				//BackgroundColor = Color.Blue,
				RowDefinitions = 
				{
					new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
				},
				ColumnDefinitions =
				{
					new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
					new ColumnDefinition() { Width = new GridLength(0.15, GridUnitType.Star) },
					new ColumnDefinition() { Width = new GridLength(0.7, GridUnitType.Star) },
					new ColumnDefinition() { Width = new GridLength(0.15, GridUnitType.Star) },
					new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
				}
			};

            this.imageLeft = new Image()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = new FileImageSource() { File = "thinArrow2Left.png" },
                WidthRequest = imageSize,
                HeightRequest = imageSize,
            };
			this.imageLeftContainer = new StackLayout()
            {
				Orientation = StackOrientation.Horizontal,
                BackgroundColor = Color.Green,   // break owner 
                Padding = new Thickness(0,2,0,0),
				HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //Content = this.imageLeft,
				Children = { this.imageLeft }
            };
            this.imageRight = new Image()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = new FileImageSource() { File = "thinArrow2Right.png" },
                WidthRequest = imageSize,
                HeightRequest = imageSize,
            };
			this.imageRightContainer = new StackLayout()
            {
				Orientation = StackOrientation.Horizontal,
                //HasShadow = false,
                BackgroundColor = Color.Black,
                Padding = new Thickness(0,2,0,0),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.FillAndExpand,
                //Content = this.imageRight,
				Children =  { this.imageRight }
            };

			grid.Children.Add(imageLeftContainer, 1, 0);
			grid.Children.Add(panelToDrag, 2, 0);
			grid.Children.Add(imageRightContainer, 3, 0);

            Frame frameLeft = new Frame()
            {
                BackgroundColor = Config.ColorBackground,
                HeightRequest = height,
                Padding = new Thickness(0, 0, 10, 0),
                HorizontalOptions = LayoutOptions.Fill,
                HasShadow = false,
                Content = new BybLabel()
                {
                    Text = textLeft,
                    TextColor = Color.White,
                    BackgroundColor = Config.ColorBackground,
                    HeightRequest = height,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.End,
                    VerticalTextAlignment = TextAlignment.Center,
                }
            };
            Frame frameRight = new Frame()
            {
                BackgroundColor = Config.ColorBackground,
                HeightRequest = height,
                Padding = new Thickness(10, 0, 0, 0),
                HorizontalOptions = LayoutOptions.Fill,
                HasShadow = false,
                Content = new BybLabel()
                {
                    Text = textRight,
                    TextColor = Color.White,
                    BackgroundColor = Config.ColorBackground,
                    HeightRequest = height,
                    HorizontalOptions = LayoutOptions.Fill,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                }
            };

            grid.Children.Add(frameLeft, 0, 0);
            grid.Children.Add(frameRight, 4, 0);

            this.scrollView = new ScrollView () {
				Orientation = ScrollOrientation.Horizontal,
				HeightRequest = height,
				Padding = new Thickness(0),
				HorizontalOptions = LayoutOptions.Fill,
				Content = grid,
			};
			this.scrollView.Scrolled += draggablePanel_Scrolled;
			this.Content = this.scrollView;
			this.HeightRequest = height;
			this.HasShadow = false;

            /// Events
            /// 
			imageLeftContainer.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                // 
                // Set "me" to be the break owner
                // 
                Command = new Command(async () =>
                {
                    bool changedFlag = false;

                    if (true == isOpponentsBreak)
                    {
                        changedFlag = true;
                    }

                    isOpponentsBreak = false;
				    this.imageLeftContainer.BackgroundColor = Color.Green;
					this.imageRightContainer.BackgroundColor = Color.Black;
                    await scrollToCenter(false);

                    if (changedFlag)
                    {
                        // set event handler
                        // to update frame score and points remaining
                        if (this.breakOwnerChanged != null)
                            this.breakOwnerChanged(this, EventArgs.Empty);
                    }

                })
            });
			imageRightContainer.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                // 
                // Set "opponent" to be the break owner
                // 
                Command = new Command(async () =>
                {
                    bool changedFlag = false;
                    if (false == isOpponentsBreak)
                    {
                        changedFlag = true;
                    }

                    isOpponentsBreak = true;
					this.imageLeftContainer.BackgroundColor = Color.Black;
				    this.imageRightContainer.BackgroundColor = Color.Green;
                    await scrollToCenter(false);

                    if (changedFlag)
                    {
                        // set event handler
                        // to update frame score and points remaining
                        if (this.breakOwnerChanged != null)
                            this.breakOwnerChanged(this, EventArgs.Empty);
                    }
                })
            });
            frameLeft.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (this.DraggedLeft != null)
                        this.DraggedLeft();
                    await scrollToCenter(false);
                })
            });
            frameRight.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (this.DraggedRight != null)
                        this.DraggedRight();
                    await scrollToCenter(false);
                })
            });
        }

		public bool getIsOpponentBreak()
		{
			return this.isOpponentsBreak;
        }

		public void setIsOpponentBreak(bool opponentsBreak)
		{
			this.isOpponentsBreak = opponentsBreak;
            if (opponentsBreak)
            {
				this.imageLeftContainer.BackgroundColor = Color.Black;
				this.imageRightContainer.BackgroundColor = Color.Green;
            }
            else
            {
                this.imageLeftContainer.BackgroundColor = Color.Green;
                this.imageRightContainer.BackgroundColor = Color.Black;
            }
        }

		System.Timers.Timer timer1;
		System.Timers.Timer timer2;

		protected override void OnSizeAllocated (double width, double height)
		{
			if (width > 100)
			{
				grid.WidthRequest = width * 3;

				if (timer1 == null)
				{
					timer1 = new Timer (1000);
					timer1.Elapsed += (s1,e1) =>
					{
						timer1.Stop();
						timer1 = null;
						Device.BeginInvokeOnMainThread(async () => { await this.scrollToCenter(true); });
					};
					timer1.Start ();
				}

			}

			base.OnSizeAllocated (width, height);
		}

		double previousScrollPosition = -1;
		DateTime timeScrollPositionChanged = DateTime.Now;

		async void draggablePanel_Scrolled (object sender, ScrolledEventArgs e)
		{
			double gridWidth = this.grid.WidthRequest;

            double transparency = System.Math.Min(1, System.Math.Abs(this.scrollView.ScrollX - centerPosition) / 50.0);
			this.imageLeftContainer.Opacity = (1 - transparency);
			this.imageRightContainer.Opacity = (1 - transparency);

            if (this.scrollView.ScrollX < gridWidth * 0.20) {
				if (this.DraggedRight != null)
					this.DraggedRight ();
				await scrollToCenter (false);
				return;
			}
			if (this.scrollView.ScrollX > gridWidth * 0.45) {
				if (this.DraggedLeft != null)
					this.DraggedLeft ();
				await scrollToCenter (false);
				return;
			}

			if (this.timer2 == null) {
				this.timer2 = new Timer (250);
				this.timer2.Elapsed += (s2, e2) =>
				{
					Device.BeginInvokeOnMainThread(async () => { await this.timer2Tick(); });
				};
				this.timer2.Start ();
			}
		}

		async Task timer2Tick()
		{
			if (System.Math.Abs(this.scrollView.ScrollX - centerPosition) < 1) {
				// back to center
                if (this.timer2 != null)
				    this.timer2.Stop();
				this.timer2 = null;
				return;
			}

			double newScrollPosition = scrollView.ScrollX;
			if (newScrollPosition != previousScrollPosition)
				timeScrollPositionChanged = DateTime.Now;
			previousScrollPosition = newScrollPosition;

			if ((DateTime.Now - timeScrollPositionChanged).TotalSeconds > 1.5) {
                if (this.timer2 != null)
				    this.timer2.Stop();
				this.timer2 = null;
				await this.scrollToCenter(true);
				return;
			}
		}

		async Task scrollToCenter(bool animated)
		{
			await this.scrollView.ScrollToAsync (this.centerPosition, 0, animated);
		}

		double centerPosition
		{
			get
			{
				return grid.WidthRequest / 3.0;
			}
		}
	}
}

