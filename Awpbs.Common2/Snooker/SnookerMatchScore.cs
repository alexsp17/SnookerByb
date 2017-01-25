using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class SnookerMatchScore
    {
        public int ID { get; set; }

        public OpponentConfirmationEnum OpponentConfirmation { get; set; }
        public bool IsUnfinished { get; set; }

        public int YourAthleteID { get; set; }
        public string YourName { get; set; }
        public string YourPicture { get; set; }
        public int OpponentAthleteID { get; set; }
        public string OpponentName { get; set; }
        public string OpponentPicture { get; set; }
        public int VenueID { get; set; }
        public string VenueName { get; set; }

        public DateTime Date { get; set; }
        public SnookerTableSizeEnum TableSize { get; set; }

        public int MatchScoreA { get; set; }
        public int MatchScoreB { get; set; }

        public List<SnookerFrameScore> FrameScores { get; set; }

        public List<SnookerBreak> YourBreaks { get; set; }
        public List<SnookerBreak> OpponentBreaks { get; set; }

        public string NameVsName
        {
            get
            {
                return YourName + " vs. " + OpponentName;
            }
        }

        public string NameVsNameShortened
        {
            get
            {
                string yourName = YourName ?? "Unknown";

                if (yourName.Length > 15)
                    yourName = yourName.Substring(0, 13) + "...";
                string opponentName = OpponentName ?? "?";
                if (opponentName.Length > 15)
                    opponentName = opponentName.Substring(0, 13) + "...";

                return yourName + " vs. " + opponentName;
            }
        }

        public int MaxFrameScoreA
        {
            get
            {
                if (HasFrameScores == false)
                    return 0;
                return FrameScores.Max(i => i.A);
            }
        }

        public int MaxFrameScoreB
        {
            get
            {
                if (HasFrameScores == false)
                    return 0;
                return FrameScores.Max(i => i.B);
            }
        }

        public bool HasFrameScores
        {
            get
            {
                if (FrameScores == null || FrameScores.Count == 0)
                    return false;
                if (FrameScores.Where(i => i.A > 0 || i.B > 0).Count() == 0)
                    return false;
                return true;
            }
        }

        public bool IsEmpty
        {
            get
            {
                if (MatchScoreA > 0 || MatchScoreB > 0)
                    return false;
                if (FrameScores.Count > 1)
                    return false;
                if (FrameScores.Count == 1 && FrameScores[0].IsEmpty == false)
                    return false;
                return true;
            }
        }

        public void CalculateMatchScoreFromFrameScores()
        {
            if (HasFrameScores == true)
            {
                MatchScoreA = FrameScores.Where(i => i.A >= i.B).Count();
                MatchScoreB = FrameScores.Where(i => i.A < i.B).Count();
            }
        }

        public SnookerMatchScore Clone()
        {
            SnookerMatchScore obj = new SnookerMatchScore();
            obj.ID = this.ID;
            obj.YourAthleteID = this.YourAthleteID;
            obj.YourName = this.YourName;
            obj.YourPicture = this.YourPicture;
            obj.OpponentAthleteID = this.OpponentAthleteID;
            obj.OpponentName = this.OpponentName;
            obj.OpponentPicture = this.OpponentPicture;
            obj.OpponentConfirmation = this.OpponentConfirmation;
            obj.IsUnfinished = this.IsUnfinished;
            obj.OpponentName = this.OpponentName;
            obj.VenueID = this.VenueID;
            obj.VenueName = this.VenueName;
            obj.Date = this.Date;
            obj.TableSize = this.TableSize;
            obj.MatchScoreA = this.MatchScoreA;
            obj.MatchScoreB = this.MatchScoreB;
            if (this.HasFrameScores)
                obj.FrameScores = (from i in this.FrameScores
                                   select i.Clone()).ToList();
            if (this.YourBreaks != null)
                obj.YourBreaks = (from i in this.YourBreaks
                                  select i.Clone()).ToList();
            if (this.OpponentBreaks != null)
                obj.OpponentBreaks = (from i in this.OpponentBreaks
                                      select i.Clone()).ToList();
            return obj;
        }

        public void PostToScore(Score score)
        {
            this.CalculateMatchScoreFromFrameScores();

            score.ScoreID = ID;
            score.AthleteAID = YourAthleteID;
            score.AthleteBID = OpponentAthleteID;
            score.VenueID = VenueID;
            score.Date = Date;
            score.IsUnfinished = IsUnfinished;
            score.Type1 = (int)TableSize;
            score.PointsA = MatchScoreA;
            score.PointsB = MatchScoreB;
            score.AthleteBConfirmation = (int)this.OpponentConfirmation;

            for (int i = 1; i <= 10; ++i)
            {
                var frameScore = new SnookerFrameScore();
                if (HasFrameScores && FrameScores.Count >= i)
                    frameScore = FrameScores[i - 1];
                score.UpdateInnerPoints(i, frameScore.A, frameScore.B);
            }

            // breaks
            try
            {
                List<SnookerBreakDataCompressed> breaks = new List<SnookerBreakDataCompressed>();
                if (this.YourBreaks != null)
                    foreach (var b in this.YourBreaks)
                        breaks.Add(new SnookerBreakDataCompressed()
                        {
                            AthleteID = this.YourAthleteID,
                            Date = b.Date,
                            Points = b.Points,
                            Number = b.NumberOfBalls,
                            Frame = b.FrameNumber,
                            Balls = b.Balls.ToList(),
							IsFoul = b.IsFoul
                        });
                if (this.OpponentBreaks != null)
                    foreach (var b in this.OpponentBreaks)
                        breaks.Add(new SnookerBreakDataCompressed()
                        {
                            AthleteID = this.OpponentAthleteID,
                            Date = b.Date,
                            Points = b.Points,
                            Number = b.NumberOfBalls,
                            Frame = b.FrameNumber,
                            Balls = b.Balls.ToList(),
							IsFoul = b.IsFoul
                        });
                score.ExtraData = Newtonsoft.Json.JsonConvert.SerializeObject(breaks);
            }
            catch (Exception)
            {
                score.ExtraData = null;
            }
        }

        public static SnookerMatchScore FromScore(int athleteID, Score score)
        {
            SnookerMatchScore match = new SnookerMatchScore();
            match.ID = score.ScoreID;
            match.YourAthleteID = score.AthleteAID;
            match.OpponentAthleteID = score.AthleteBID;
            match.VenueID = score.VenueID ?? 0;
            match.Date = score.Date;
            match.IsUnfinished = score.IsUnfinished;
            if (score.Type1 == null)
                match.TableSize = SnookerTableSizeEnum.Unknown;
            else
                match.TableSize = (SnookerTableSizeEnum)score.Type1.Value;
            match.MatchScoreA = score.PointsA;
            match.MatchScoreB = score.PointsB;
            match.OpponentConfirmation = (OpponentConfirmationEnum)score.AthleteBConfirmation;

            match.FrameScores = new List<SnookerFrameScore>();
            for (int i = 1; i <= 10; ++i)
            {
                int a = score.InnerPointsA[i - 1];
                int b = score.InnerPointsB[i - 1];
                if (a > 0 || b > 0)
                    match.FrameScores.Add(new SnookerFrameScore() { A = a, B = b });
            }

            if (score.AthleteBID == athleteID)
            {
                // invert all
                match.YourAthleteID = score.AthleteBID;
                match.OpponentAthleteID = score.AthleteAID;
                match.MatchScoreA = score.PointsB;
                match.MatchScoreB = score.PointsA;
                foreach (var frame in match.FrameScores)
                {
                    var a = frame.A;
                    frame.A = frame.B;
                    frame.B = a;
                }
            }

            // breaks
            if (score.ExtraData != null)
            {
                try
                {
                    List<SnookerBreakDataCompressed> breaks = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SnookerBreakDataCompressed>>(score.ExtraData);
                    match.YourBreaks = new List<SnookerBreak>();
                    match.OpponentBreaks = new List<SnookerBreak>();
                    foreach (SnookerBreakDataCompressed compressed in breaks)
                    {
                        SnookerBreak snookerBreak = new SnookerBreak()
                        {
                            Date = compressed.Date,
                            Points = compressed.Points,
                            NumberOfBalls = compressed.Number,
                            Balls = compressed.Balls.ToList(),
                            FrameNumber = compressed.Frame,
							IsFoul = compressed.IsFoul
                        };
                        if (compressed.AthleteID == match.OpponentAthleteID)
                        {
                            snookerBreak.AthleteID = match.OpponentAthleteID;
                            snookerBreak.OpponentAthleteID = match.YourAthleteID;
                            match.OpponentBreaks.Add(snookerBreak);
                        }
                        else
                        {
                            snookerBreak.AthleteID = match.YourAthleteID;
                            snookerBreak.OpponentAthleteID = match.OpponentAthleteID;
                            match.YourBreaks.Add(snookerBreak);
                        }
                    }
                }
                catch (Exception)
                {
                    match.YourBreaks = null;
                    match.OpponentBreaks = null;
                }
            }

            return match;
        }

        public static List<SnookerMatchScore> Sort(List<SnookerMatchScore> matches, SnookerMatchSortEnum sort)
        {
            if (sort == SnookerMatchSortEnum.ByDate)
            {
                return (from i in matches
                        orderby i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerMatchSortEnum.ByFrameCount)
            {
                return (from i in matches
                        orderby i.MatchScoreA + i.MatchScoreB descending, i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerMatchSortEnum.ByWinFirst)
            {
                return (from i in matches
                        orderby i.MatchScoreA - i.MatchScoreB descending, i.MatchScoreA descending, i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerMatchSortEnum.ByLossFirst)
            {
                return (from i in matches
                        orderby i.MatchScoreA - i.MatchScoreB ascending, i.MatchScoreB descending, i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerMatchSortEnum.ByBestFrame)
            {
                return (from i in matches
                        orderby i.MaxFrameScoreA descending, i.Date descending
                        select i).ToList();
            }

            if (sort == SnookerMatchSortEnum.ByOpponent)
            {
                return (from i in matches
                        orderby i.OpponentName, i.OpponentAthleteID, i.Date descending
                        select i).ToList();
            }

            return matches;
        }

        public bool Validate(out string message)
        {
            if (this.MatchScoreA == 0 && this.MatchScoreB == 0)
            {
                message = "What a short game, 0:0 :)";
                return false;
            }

            if (this.MatchScoreA + this.MatchScoreB > 21)
            {
                message = "I'm sorry, but I have hard time believing you played more than 21 frames in a single match!";
                return false;
            }

            if (this.MatchScoreA < 0 || this.MatchScoreB < 0)
            {
                message = "Match score cannot be negative";
                return false;
            }

            if (this.HasFrameScores)
            {
                for (int index = 0; index < FrameScores.Count; ++index )
                {
                    string frameMessage;
                    if (!FrameScores[index].Validate(out frameMessage))
                    {
                        message = "Frame #" + (index + 1).ToString() + " - " + frameMessage;
                        return false;
                    }
                }
            }

            var dateValidator = new DateValidator();
            if (!dateValidator.Validate(Date))
            {
                message = "Date - " + dateValidator.ErrorText;
                return false;
            }

            message = "ok";
            return true;
        }

		public List<SnookerBreak> GetBreaksForFrame(int frameNumber)
		{
			List<SnookerBreak> breaks = new List<SnookerBreak> ();
			if (this.YourBreaks != null)
				breaks.AddRange(this.YourBreaks.Where(i => i.FrameNumber == frameNumber).ToList());
			if (this.OpponentBreaks != null)
				breaks.AddRange (this.OpponentBreaks.Where (i => i.FrameNumber == frameNumber).ToList ());
			return breaks;
		}

		public void DeleteFrame(SnookerFrameScore frame)
		{
			int frameNumber = FrameScores.IndexOf (frame) + 1;
			if (frameNumber <= 0)
				return;

			// remove breaks
			List<SnookerBreak> breaksToDelete = this.GetBreaksForFrame (frameNumber);
			foreach (var b in breaksToDelete) {
				this.YourBreaks.Remove (b);
				this.OpponentBreaks.Remove (b);
			}
			foreach (var b in this.YourBreaks)
				if (b.FrameNumber > frameNumber)
					b.FrameNumber -= 1;
			foreach (var b in this.OpponentBreaks)
				if (b.FrameNumber > frameNumber)
					b.FrameNumber -= 1;

			// remove the frame
			FrameScores.Remove(frame);
		}

        public override string ToString()
        {
            return "{" + MatchScoreA + " x " + MatchScoreB + "}";
        }
    }
}
