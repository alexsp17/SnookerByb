using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using RoundedBoxView.Forms.Plugin.Abstractions;

namespace Awpbs.Mobile
{
	public class ListOfBreaksInMatchControl : StackLayout
	{
        readonly int maxNumberOfBallsPerLine = Config.IsTablet ? 10 : 8;

        public event EventHandler<SnookerBreak> UserTappedOnBreak;

		public bool IsMatchMode { get; private set; }

		public List<SnookerBreak> Breaks { get; private set; }

		int frameScoreYou;
		int frameScoreOpponent;
		bool frameScoreValid;

		public ListOfBreaksInMatchControl(bool isMatchMode = false)
        {
			this.IsMatchMode = isMatchMode;
			
            this.Padding = new Thickness(0, 0, 0, 0);
            this.Spacing = 0;
            this.BackgroundColor = Config.ColorBackground;
        }

        //
        // Fill the list of breaks in a frame
        //
		public void Fill(SnookerMatchScore match, int frameNumber)
        {
			string debugString = String.Format ("ListOfBreaksInMatchControl::Fill(): frame {0}, Enter", frameNumber);
			TraceHelper.TraceInfoForResponsiveness(debugString);

            this.Children.Clear();

			if (match == null || match.YourBreaks == null || match.OpponentBreaks == null)
			{
				this.Breaks = new List<SnookerBreak> ();
				return;
			}

            // list of all breaks
			this.Breaks = new List<SnookerBreak>();
			this.Breaks.AddRange(match.YourBreaks.Where(i => i.FrameNumber == frameNumber).ToList());
			this.Breaks.AddRange(match.OpponentBreaks.Where(i => i.FrameNumber == frameNumber).ToList());
			this.Breaks = this.Breaks.OrderByDescending(i => i.Date).ToList();

            // Check that the Frame score corresponds to the list of breaks
            // and we can display the running frame score with each break
			frameScoreYou = 0;
			frameScoreOpponent = 0;
			foreach (var snookerBreak in this.Breaks)
			{
				if (match.YourAthleteID == snookerBreak.AthleteID)
					frameScoreYou += snookerBreak.Points;
				else
					frameScoreOpponent += snookerBreak.Points;
			}

			SnookerFrameScore frameScore = match.FrameScores.ElementAt(frameNumber-1);
			if ((frameScore.A == frameScoreYou) && (frameScore.B == frameScoreOpponent))
                // Frame score corresponds to all the breaks entered,
                // display the running frame score after each break
				frameScoreValid = true;  
			else
				frameScoreValid = false; // don't display running frame score

            // add elements
			foreach (var snookerBreak in this.Breaks)
            {
			    //TraceHelper.TraceInfoForResponsiveness(String.Format("break {0}", snookerBreak.Balls.Count));

                this.Children.Add(new BoxView()
                {
					BackgroundColor = Config.ColorBackground,
                    HeightRequest = 1,
                });

                bool isOpponentsBreak = match.OpponentBreaks.Contains(snookerBreak);
                addSnookerBreakControls(snookerBreak, isOpponentsBreak);

                // if displaying running frame score, update it
				if (frameScoreValid)
                {
                  if (isOpponentsBreak)
                  {
                      frameScoreOpponent -= snookerBreak.Balls.Sum ();
                  }
                  else
                  {
                      frameScoreYou -= snookerBreak.Balls.Sum ();
                  }
                }
            }

			debugString = String.Format ("ListOfBreaksInMatchControl::Fill(): frame {0}, Exit", frameNumber);
			TraceHelper.TraceInfoForResponsiveness(debugString);
		}

		void addSnookerBreakControls(SnookerBreak snookerBreak, bool isOpponentsBreak)
        {
			StackLayout[] horizontalStack = new StackLayout[5];
			int lineIdx;

            // Determine number of lines needed to display all balls in a break
            int numLines = (int) (snookerBreak.Balls.Count / maxNumberOfBallsPerLine) + 1;

            for (lineIdx = 0; lineIdx < numLines; lineIdx++)
            {
                horizontalStack[lineIdx] = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(1,0,1,0),
					BackgroundColor = Config.ColorBackground,
                    Spacing = 2,
                    HorizontalOptions = (!isOpponentsBreak || (lineIdx > 0)) ? LayoutOptions.Start : LayoutOptions.End
                };
            }

			// if more than one line, start adding opponent's balls from the left
			if (snookerBreak.Balls.Count >= maxNumberOfBallsPerLine)
				horizontalStack[0].HorizontalOptions = LayoutOptions.Start;
			
			double sizeOfPocketedBalls = Config.ExtraSmallBallSize;

            // Fill horizonal stack lines with the balls 
            lineIdx = 0;
            if (snookerBreak.HasBalls)
            {
                int ballIdx = 0;
                foreach (var ballScore in snookerBreak.Balls)
                {
                    Color color = Config.BallColors[ballScore];
                    Color borderColor = color;
                    if (ballScore == 7)
                        borderColor = Color.Gray;

					RoundedBoxView.Forms.Plugin.Abstractions.RoundedBoxView ball = new RoundedBoxView.Forms.Plugin.Abstractions.RoundedBoxView {
						WidthRequest = sizeOfPocketedBalls,
						HeightRequest = sizeOfPocketedBalls,
						MinimumWidthRequest = sizeOfPocketedBalls,
						MinimumHeightRequest = sizeOfPocketedBalls,
						Color = color,
						BorderColor = borderColor,
						BackgroundColor = color,
						BorderThickness = 1,
						CornerRadius = (int)(sizeOfPocketedBalls/2),
						VerticalOptions = LayoutOptions.Center
					};

                    // if 8 balls in each line:
                    //   0/8 - 0, 7/8 - 0, 8/8 - 1, 9/8 - 1 ...
                    // 
                    lineIdx = (int)(ballIdx / maxNumberOfBallsPerLine);
					horizontalStack[lineIdx].Children.Insert(horizontalStack[lineIdx].Children.Count, ball);

                    ballIdx++;
                }
            }

			if (snookerBreak.IsFoul)
			{
				// add "foul":
				//   if on right side "foul xxx 15"
				//   if on left side  "xxx 15 foul"

				string foulLabelString = isOpponentsBreak ? 
					"foul ": // if on right side "foul xxx 15"
					" foul"; // if on left side  "xxx 15 foul"
				
				BybLabel foulLabel = new BybLabel () {
					Text = foulLabelString,
					TextColor = Config.ColorRed,
					WidthRequest = Config.IsTablet ? 35 : 27,
					//VerticalTextAlignment = TextAlignment.Center,
					HorizontalTextAlignment = TextAlignment.Start
				};

				if (isOpponentsBreak) 
					horizontalStack[0].Children.Insert(0, foulLabel); // insert "foul " in the beginning "foul xxx 15"
				else 
					horizontalStack[0].Children.Add(foulLabel);    // add " foul" to the end "15 xxx foul"
			}

            // This grid hold all the break info
			BybBreakGrid breakGrid = new BybBreakGrid(isOpponentsBreak);

			// Add all the lines of balls to stack
            for (lineIdx = 0; lineIdx < numLines; lineIdx++)
            {
                breakGrid.ballsStack.Children.Add(horizontalStack[lineIdx]);
            }
						
            // Break score - 3 character string 
			string breakPointsString = String.Format("{0,3}", snookerBreak.Points.ToString());
			BybLabel breakPointsLabel = new BybLabel()
			{
				Text = breakPointsString, 
				BackgroundColor = Config.ColorBackground,
				TextColor = snookerBreak.IsFoul ? Config.ColorRed : Color.White,
				VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = (isOpponentsBreak) ? TextAlignment.End : TextAlignment.Start
			};

            breakGrid.ballsStack.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (this.UserTappedOnBreak != null)
                        this.UserTappedOnBreak(this, snookerBreak);
                }),
                NumberOfTapsRequired = 1
            });
            breakPointsLabel.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    if (this.UserTappedOnBreak != null)
                        this.UserTappedOnBreak(this, snookerBreak);
                }),
                NumberOfTapsRequired = 1
            });

            // Add to the player who scored, 
            // leave the other player's corresponding fields empty:
            //  - balls 
            //  - break score field 
			if (isOpponentsBreak) 
            {
			  breakGrid.Children.Add(breakGrid.ballsStack, 3, 0);
			  breakGrid.Children.Add(breakPointsLabel, 4, 0);
            }
            else
            {
			  breakGrid.Children.Add(breakPointsLabel, 0, 0);
			  breakGrid.Children.Add(breakGrid.ballsStack, 1, 0);
            }
			int childIdx = breakGrid.Children.Count ();

            // If running frame score is valid, add it in the middle 
			if (frameScoreValid)
            {
              Grid frameScoreGrid = new CurrentBreakScoreGrid(frameScoreYou, frameScoreOpponent);
			  breakGrid.Children.Add(frameScoreGrid, 2, 0);

            }            

            // I think this might not be necessary
			breakGrid.RaiseChild (breakGrid.Children.ElementAt(childIdx-1));
			breakGrid.RaiseChild (breakGrid.Children.ElementAt(childIdx-2));

            // add the new break to the list of breaks
            this.Children.Add(breakGrid);

        } // addSnookerBreakControls()

	}


    // Grid that holds all the infor for current break
    // 
    // Five columns:
    //   You scored:
    //   " 12 ***   12:0            "
    //   Opponent scored:
    //   "           0:12    *** 12 "
    //
    //   - you break points
    //   - you break balls
    //   - running frame score (if valid)
    //   - opponent break balls
    //   - opponent break points
	class BybBreakGrid : Grid
	{
        public StackLayout ballsStack;

		public BybBreakGrid(bool isOpponentsBreak)
		{
		    // HorizontalOptions = LayoutOptions.Fill - default
            // NOTE: I've read that using it, even though it's a default,
            //       increases execution time, so it's better to avoid it
            //       Alex
			VerticalOptions = LayoutOptions.Center;
			Padding = new Thickness(2, 0, 2, 0);
			ColumnSpacing = 0;
			RowSpacing = 0;
			BackgroundColor = Config.ColorBackground;

			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });   
			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) });   
			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });   
			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) });   
			this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });   

            ballsStack = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
				HorizontalOptions = (isOpponentsBreak) ? LayoutOptions.End : LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Config.ColorBackground,
                Padding = new Thickness(0, Config.IsTablet ? 10 : 5, 0, Config.IsTablet ? 10 : 5),
                Spacing = 2,
            };
			
		}
    }

    //
    // "Running" frame score in the middle, e.g.
    //          18:23 
    //
	class CurrentBreakScoreGrid : Grid
	{
		public CurrentBreakScoreGrid (int pointsYou, int pointsOpponent)
		{
            //HorizontalOptions = LayoutOptions.Fill; - default
            VerticalOptions = LayoutOptions.Center;
            Padding = new Thickness(1);
            ColumnSpacing = 0;
            RowSpacing = 0;
			BackgroundColor = Config.ColorBackground;

			string a = pointsYou.ToString ();
			a.PadLeft (3);
			string b = pointsOpponent.ToString ();
			b.PadRight (3);
			string c = a + ":" + b;

            // make sure frameScore string is always 7 characters 
            // (including spaces): "xxx:xxx", e.g "  1:22 "
            BybLabel frameScoreA_label = new BybLabel()
            {
                Text = pointsYou.ToString(),
				BackgroundColor = Config.ColorBackground,
				TextColor = Color.White,
                //VerticalTextAlignment = TextAlignment.Center - default
                HorizontalTextAlignment = TextAlignment.End
            };

            BybLabel frameScoreMiddle_label = new BybLabel()
            {
                Text = ":", 
				BackgroundColor = Config.ColorBackground,
				TextColor = Color.White,
                //VerticalTextAlignment = TextAlignment.Center  - default
                HorizontalTextAlignment = TextAlignment.Center
            };

            BybLabel frameScoreB_label = new BybLabel()
            {
                Text = pointsOpponent.ToString(),
				BackgroundColor = Config.ColorBackground,
                TextColor = Color.White,
                //VerticalTextAlignment = TextAlignment.Center - default
                HorizontalTextAlignment = TextAlignment.Start
            };

             this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });   
             this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });   
             this.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });   

             this.Children.Add(frameScoreA_label, 0, 0);
             this.Children.Add(frameScoreMiddle_label, 1, 0);
             this.Children.Add(frameScoreB_label, 2, 0);
        }
    }
}

