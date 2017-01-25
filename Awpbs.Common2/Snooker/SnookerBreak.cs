using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class SnookerBreak
    {
        public int ID { get; set; }

        public OpponentConfirmationEnum OpponentConfirmation { get; set; }
        
        public int AthleteID { get; set; }
        public string AthleteName { get; set; }
        public int OpponentAthleteID { get; set; }
        public string OpponentName { get; set; }
        public int VenueID { get; set; }
        public string VenueName { get; set; }

        public DateTime Date { get; set; }
        public SnookerTableSizeEnum TableSize { get; set; }

        public int Points { get; set; }
        public int NumberOfBalls { get; set; }

        public string NumberOfBallsDisplay { get { return NumberOfBalls <= 0 ? "-" : NumberOfBalls.ToString(); } }

        public List<int> Balls { get; set; }
        public bool HasBalls { get { return Balls != null && Balls.Count > 0; } }

        public List<int> GetBallsEvenWhenUnknown()
        {
            if (this.HasBalls)
                return Balls;

            List<int> balls = new List<int>();
            for (int i = 0; i < System.Math.Max(NumberOfBalls, 7); ++i)
                balls.Add(0);
            return balls;
        }

        /// <summary>
        /// The frame number this break was scored at, starting from 0. 0 means undefined
        /// </summary>
        public int FrameNumber { get; set; }

        public bool IsFoul { get; set; }

        public void CalcFromBalls()
        {
            this.Points = Balls.Sum();
            this.NumberOfBalls = Balls.Count();
        }

        public SnookerBreak Clone()
        {
            SnookerBreak obj = new SnookerBreak();
            obj.ID = this.ID;
            obj.AthleteID = this.AthleteID;
            obj.AthleteName = this.AthleteName;
            obj.OpponentConfirmation = this.OpponentConfirmation;
            obj.OpponentAthleteID = this.OpponentAthleteID;
            obj.OpponentName = this.OpponentName;
            obj.VenueID = this.VenueID;
            obj.VenueName = this.VenueName;
            obj.Date = this.Date;
            obj.TableSize = this.TableSize;
            obj.Points = this.Points;
            obj.NumberOfBalls = this.NumberOfBalls;
            if (this.HasBalls)
                obj.Balls = this.Balls.ToList();
            obj.FrameNumber = this.FrameNumber;
            obj.IsFoul = this.IsFoul;
            return obj;
        }

        public void PostToResult(Result res)
        {
            if (HasBalls)
                this.CalcFromBalls();

            res.ResultID = this.ID;
            if (this.AthleteID > 0)
                res.AthleteID = this.AthleteID;
            res.Count = this.Points;
            res.Count2 = this.NumberOfBalls;
            res.ResultType = new SportsAndResultTypesRepository().GetSport(SportEnum.Snooker).ResultTypes.First();
            res.ResultTypeID = res.ResultType.ResultTypeID;
            res.Date = this.Date;
            res.VenueID = VenueID;
            res.Type1 = (int)this.TableSize;
            res.OpponentAthleteID = this.OpponentAthleteID;
            res.OpponentConfirmation = (int)this.OpponentConfirmation;

            res.Details1 = null;
            if (this.IsFoul)
            {
                res.Details1 = "foul";
            }
            else if (HasBalls)
            {
                res.Details1 = "";
                foreach (var ball in Balls)
                {
                    if (res.Details1.Length > 0)
                        res.Details1 += ",";
                    res.Details1 += ball.ToString();
                }
            }
        }

        public static SnookerBreak FromResult(Result res)
        {
            SnookerTableSizeEnum tableSize = SnookerTableSizeEnum.Unknown;
            if (res.Type1 != null)
                tableSize = (SnookerTableSizeEnum)res.Type1.Value;

            List<int> balls = null;
            bool isFoul = false;
            if (string.IsNullOrEmpty(res.Details1) == false)
            {
                if (res.Details1 == "foul")
                {
                    isFoul = true;
                }
                else
                {
                    balls = new List<int>();
                    string[] strs = res.Details1.Split(',');
                    foreach (string str in strs)
                        balls.Add(int.Parse(str));
                }
            }

            SnookerBreak snookerBreak = new SnookerBreak()
            {
                ID = res.ResultID,
                AthleteID = res.AthleteID,
                Points = res.Count ?? 0,
                NumberOfBalls = res.Count2 ?? 0,
                Date = res.Date ?? new DateTime(2000,1,1),
                VenueID = res.VenueID ?? 0,
                TableSize = tableSize,
                Balls = balls,
                IsFoul = isFoul,
                OpponentAthleteID = res.OpponentAthleteID ?? 0,
                OpponentConfirmation = (OpponentConfirmationEnum)res.OpponentConfirmation
            };
            return snookerBreak;
        }

        public static List<SnookerBreak> SortBy(List<SnookerBreak> list, SnookerBreakSortEnum sort)
        {
            if (sort == SnookerBreakSortEnum.ByDate)
            {
                return (from i in list
                        orderby i.Date descending, i.Points descending, i.NumberOfBalls descending
                        select i).ToList();
            }

            if (sort == SnookerBreakSortEnum.ByBallCount)
            {
                return (from i in list
                        orderby i.NumberOfBalls descending, i.Points descending, i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerBreakSortEnum.ByPoints)
            {
                return (from i in list
                        orderby i.Points descending, i.NumberOfBalls descending, i.Date descending
                        select i).ToList();
            }

            return list;
        }

        public bool Validate(out string message)
        {
            if (IsFoul == true)
            {
                if (Points < 4)
                {
                    message = "A foul cannot be smaller than 4 points.";
                    return false;
                }
                if (Points > 7)
                {
                    message = "A foul cannot be larger than 7 points.";
                    return false;
                }

                message = "OK";
                return true;
            }

            if (Points > 155)
            {
                message = "Have you exceeded the super-maximum score of 155? :)";
                return false;
            }
            if (Points < 10)
            {
                message = "Please avoid recording breaks of less than 10 points as notable.";
                return false;
            }
            if (NumberOfBalls == 0)
            {
                message = "OK. User chose to not record number of balls";
                return true;
            }
            if (NumberOfBalls <= 2)
            {
                message = "Please avoid recording breaks of less than 3 balls as notable.";
                return false;
            }

            // note: 8/20/2015: algorithms needs improvement...

            int countColored = NumberOfBalls / 2;
            int countReds = NumberOfBalls - countColored;

            int minPossiblePoints = countReds + countColored * 2;
            if (Points < minPossiblePoints)
            {
                message = "Ball count and points do not match up. Minimum points for this ball count is " + minPossiblePoints;
                return false;
            }

            int maxPossibleScore = countReds + countColored * 7;
            if (NumberOfBalls == 3)
                maxPossibleScore = System.Math.Max(maxPossibleScore, 18); // 7,6,5
            if (NumberOfBalls == 4)
                maxPossibleScore = System.Math.Max(maxPossibleScore, 22); // 7,6,5,4
            if (NumberOfBalls == 5)
                maxPossibleScore = System.Math.Max(maxPossibleScore, 25); // 7,6,5,4,3
            if (NumberOfBalls == 6)
                maxPossibleScore = System.Math.Max(maxPossibleScore, 27); // 7,6,5,4,3,2
            if (NumberOfBalls >= 7)
                maxPossibleScore += 27; // 8/20/2015: to avoid trouble...
            if (Points > maxPossibleScore)
            {
                message = "Ball count and score do not match up. Maximum points for this ball count is " + maxPossibleScore;
                return false;
            }

            message = "OK";
            return true;
        }

        public override string ToString()
        {
            return "{" + NumberOfBalls.ToString() + ", " + Points.ToString() + "}";
        }
    }
}
