using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    //
    // Keep track of the balls remaining on the table and use it for calculating
    // "points remaining".
    //
    // We need the following:
    //   - number reds
    //   - lowest colored ball
    //   - previous ball scored in current break
    //   - is a break in progress
    //     (frame score and points remaining are now updated after each ball scored)
    //
	public class BallsOnTable
	{
        public int numberOfReds;
        public int lowestColor;   
		public int previousBall;     // set to zero when break is finished
		int saved_lowestColor;
		public bool breakInProgress; 

		public BallsOnTable()
		{
			numberOfReds = 15;
			lowestColor = 2;
			previousBall = 0;
			saved_lowestColor = 2;
            breakInProgress = false;
		}

        //
        // Based on number of reds and the lowest color on the table, calculate
        // the number of points remaining:
        //   Points for all colors: 27
        //   Points for every red on a table: 8, which is 1 + 7 (red followed by black) 
        //
		public int getPointsRemaining()
		{
			int points = numberOfReds * 8;

			for (int colorIdx = lowestColor; colorIdx <= 7; colorIdx++) 
            {
                points += colorIdx;
			}

			// if number of reds is zero, but the last red was just pocketed,
            // then extra 7 points are still available
            //
			if (breakInProgress && (previousBall == 1))
            {
			    points += 7;
            }

			return points;
		}

        //
        // Update balls on table when a new ball is pocketed
        //   If it's a red, simply decrement number of reds
        //   If it's a color:
        //     if previous ball was a red, do nothing, bcs this color ball will be put back on table
        //     else 
        //       // it's the first ball in a break or a previous ball was color
        //       assume it will remain in the pocket and set the lowestColor to 
        //       this ball + 1
        //       
		public void ballPocketed(int currentBall)
		{
            this.breakInProgress = true;

			if (currentBall == 1) 
			{
                // 
                // Red ball
                // 
				if (numberOfReds == 0)
                {
					Console.WriteLine("ballPocketed(): ERROR Number of reds already zero, exit");
                    return;
                }

				numberOfReds--;
			} 
			else 
			{
                //
                // Colored ball
                //
				if ( (currentBall < 2) && (currentBall > 7) )
				{
			     	Console.WriteLine ("ballPocketed(): ERROR");
                    return;
				}

				// Color ball stays in pocket when: 
				// - number of reds on the table is zero AND 
                //   - this colored ball is the first of this break
                //     OR
                //   - previous ball pocketed in this break was not red
				if ( (numberOfReds == 0) && (previousBall != 1) )
                {
				    if ((previousBall == 0) && (currentBall >= 4))
                    {
                        // If it's the first ball in current break, it may be a foul.
                        // Save the current lowestColor, in case use taps "foul"
                        // and it will need to be reset back to whatever it was
                        saved_lowestColor = lowestColor; 
                    }

				    lowestColor = currentBall + 1;
                }
			}

			Console.WriteLine ("ballPocketed(): curBall " + currentBall); 

			previousBall = currentBall;
		}

        //
        // Mark the last entered ball as "foul". 
        // Reset the lowestColor back to whatever it was before this last color ball
        // was processed.
        //
		public void markFoul()
        {
            previousBall = 0;
            lowestColor = saved_lowestColor;
        }

        //
        //  Break is finished, initialize what's needed:
        //
		public void breakFinished()
		{
            this.breakInProgress = false;
            previousBall = 0;

            if (numberOfReds == 0)
            {
                saved_lowestColor = lowestColor; 
            }
        }

        //
        //  When a ball is deleted from the current break, or 
        //  a past break is edited
        //
        //  prevBall == 0 means it's the first ball in a break
        //
		public void ballRemovedFromBreak(int lastBall, int prevBall)
		{
			if (lastBall == 1)
			{
                // Red ball
			    if (numberOfReds < 15)
				    numberOfReds++;
			}
			else 
			{
                // Colored ball
				if ( (lastBall < 2) && (lastBall > 7) )
				{
			     	Console.WriteLine ("ballRemovedFromScore(): invalid ball");
                    return;
				}

				if ( (numberOfReds != 0) || (prevBall == 1) )
                {
				    lowestColor = 2;
                }
                else if (prevBall != 1)
                {
				    lowestColor = lastBall;
                }
			}
			Console.WriteLine ("ballRemovedFromScore(): lastBall " + lastBall + ", prevBall " + prevBall);

			previousBall = prevBall;
        }
	}

    public class SnookerFrameScore
    {
        public BallsOnTable ballsOnTable { get; set; }
        public int A { get; set; }
        public int B { get; set; }

		public SnookerFrameScore()
        {
            A = 0;
            B = 0;
			ballsOnTable = new BallsOnTable();
        }
        public bool IsValid
        {
            get
            {
                string message;
                return Validate(out message);
            }
        }

        public bool IsEmpty
        {
            get
            {
                return this.A == 0 && this.B == 0;
            }
        }

        public SnookerFrameScore Clone()
        {
            SnookerFrameScore newObj = new SnookerFrameScore();
            newObj.A = this.A; 
            newObj.B = this.B; 
			newObj.ballsOnTable.lowestColor = this.ballsOnTable.lowestColor;
			newObj.ballsOnTable.numberOfReds = this.ballsOnTable.numberOfReds;

            return newObj;
        }

        public bool Validate(out string message)
        {
            if (A < 0 || B < 0)
            {
                message = "Negative score?";
                return false;
            }
            if (A > 155 || B > 155)
            {
                message = "Having hard time believing you scored above the super maximum 155 :)";
                return false;
            }
            if (A < 20 && B < 20)
            {
                message = "The scores are impossibly low.";
                return false;
            }
            if (A == B)
            {
                message = "Draw frame? Snooker rules do not allow frame draws!";
                return false;
            }

            message = "ok";
            return true;
        }
    }

    public class SnookerBreakDataCompressed
    {
        public SnookerBreakDataCompressed()
        {
        }

        public int AthleteID { get; set; }
        public int Frame { get; set; }
        public DateTime Date { get; set; }
        public int Points { get; set; }
        public int Number { get; set; }
        public List<int> Balls { get; set; }
		public bool IsFoul { get; set; }
    }
}
