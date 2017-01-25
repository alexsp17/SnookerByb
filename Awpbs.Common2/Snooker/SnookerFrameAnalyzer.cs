using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Awpbs
{
    public enum SnookerFrameAnalysisEnum
    {
        FullyTracked = 1,
        NotFullyTracked = 2,
        Issues = 3
    }

    public class SnookerFrameAnalysis
    {
        public SnookerFrameAnalysisEnum Status { get; set; }

        public string Issue { get; set; }

        public int NumberOfFouls { get; set; }
        public int NumberOfBreaks { get; set; }
        public int NumberOfRedsLeft { get; set; }
        public int NumberOfPointsLeft { get; set; }
        public int ScoreDifference { get; set; }
        public int SnookersRequired { get; set; }
    }

    public class SnookerFrameAnalyzer
    {
        public SnookerFrameAnalysis Analyze(SnookerMatchScore match, int frameNumber)
        {
            SnookerFrameAnalysis analysis = new SnookerFrameAnalysis();
            analysis.Status = SnookerFrameAnalysisEnum.Issues;
            analysis.Issue = "";

            // all breaks
            List<SnookerBreak> breaks = match.GetBreaksForFrame(frameNumber).OrderBy(i => i.Date).ToList();

            // do the frame scores total the breaks?
            if (match.FrameScores == null || frameNumber < match.FrameScores.Count)
            {
                analysis.Status = SnookerFrameAnalysisEnum.NotFullyTracked;
                analysis.Issue = "No frame scores";
                return analysis;
            }
            SnookerFrameScore frameScore = match.FrameScores[frameNumber - 1];
            int pointsA = breaks.Where(i => i.AthleteID == match.YourAthleteID).Sum(i => i.Points);
            int pointsB = breaks.Where(i => i.AthleteID == match.OpponentAthleteID).Sum(i => i.Points);
            if (frameScore.A != pointsA || frameScore.B != pointsB)
            {
                analysis.Status = SnookerFrameAnalysisEnum.NotFullyTracked;
                analysis.Issue = "Frame score edited";
                return analysis;
            }

            // score difference
            int scoreDifference = System.Math.Abs(frameScore.A - frameScore.B);
            analysis.ScoreDifference = scoreDifference;

            /// go thru the breaks
            /// 
            int numberOfRedsLeft = 15;
            int numberOfFouls = 0;
            int lastColoredPocketedBall = 1;
            foreach (var b in breaks)
            {
                int index = breaks.IndexOf(b) + 1;

                if (b.NumberOfBalls < 1)
                {
                    analysis.Issue = "Empty break #" + index;
                    return analysis;
                }

                if (b.NumberOfBalls == 1 && b.Points >= 4 && b.Points <= 7)
                {
                    numberOfFouls++;
                    continue;
                }

                bool expectingColoredAfterRed = false;
                for (int i = 0; i < b.Balls.Count; ++i)
                {
                    int ball = b.Balls[i];
                    if (numberOfRedsLeft > 0 || expectingColoredAfterRed)
                    {
                        // red, colored, red, colored

                        if (expectingColoredAfterRed == true && ball == 1)
                        {
                            if (numberOfRedsLeft <= 0)
                            {
                                analysis.Issue = "Invalid break #" + index;
                                return analysis;
                            }

                            // several reds at the same time (happens when more than one red pocketed at the same time)
                            numberOfRedsLeft--;
                            continue;
                        }

                        if (expectingColoredAfterRed)
                        {
                            if (ball < 2 || ball > 7)
                            {
                                analysis.Issue = "Invalid break #" + index;
                                return analysis;
                            }
                            expectingColoredAfterRed = false;
                        }
                        else
                        {
                            if (ball != 1)
                            {
                                analysis.Issue = "Invalid break #" + index;
                                return analysis;
                            }

                            numberOfRedsLeft--;
                            expectingColoredAfterRed = true;
                        }
                    }
                    else
                    {
                        // last colored

                        if (ball < 2 || ball > 7)
                        {
                            analysis.Issue = "Invalid break #" + index;
                            return analysis;
                        }

                        if (ball > lastColoredPocketedBall + 1 || ball == lastColoredPocketedBall)
                        {
                            numberOfFouls++;
                            continue;
                        }

                        if (ball != lastColoredPocketedBall + 1)
                        {
                            analysis.Issue = "Invalid break #" + index;
                            return analysis;
                        }

                        lastColoredPocketedBall = ball;

                        if (ball == 7)
                        {
                            bool ok = false;
                            if (i == b.Balls.Count - 1)
                                ok = true;
                            if (i == b.Balls.Count - 2 && b.Balls[i + 1] == 7)
                                ok = true;
                            if (ok == false)
                            {
                                analysis.Issue = "Breaks after last ball potted";
                                return analysis;
                            }
                        }
                    }
                }
            }

            /// Calculate number of points left
            /// 
            int numberOfLastColoredBallPointsLeft = 0; // maximum=2+3+4+5+6+7=27
            for (int ball = 7; ball >= 2; --ball)
            {
                if (ball <= lastColoredPocketedBall)
                    break;
                numberOfLastColoredBallPointsLeft += ball;
            }
            int numberOfPointsLeft = numberOfLastColoredBallPointsLeft;
            numberOfPointsLeft += numberOfRedsLeft * (1 + 7);

            /// Calculate snookers required
            /// 
            int snookersRequired = 0;
            if (numberOfPointsLeft < scoreDifference)
            {
                int snookerPoints = 4;
                if (lastColoredPocketedBall > 4)
                    snookerPoints = lastColoredPocketedBall;
                int extraPoints = scoreDifference - numberOfPointsLeft;
                snookersRequired = extraPoints / snookerPoints;
                if (extraPoints % snookerPoints > 0)
                    snookersRequired++;
            }

            analysis.Status = SnookerFrameAnalysisEnum.FullyTracked;
            analysis.Issue = "";
            analysis.NumberOfBreaks = breaks.Count - numberOfFouls;
            analysis.NumberOfFouls = numberOfFouls;
            analysis.NumberOfRedsLeft = numberOfRedsLeft;
            analysis.NumberOfPointsLeft = numberOfPointsLeft;
            analysis.SnookersRequired = snookersRequired;
            return analysis;
        }
    }
}
