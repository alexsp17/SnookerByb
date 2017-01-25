using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Score
    {
        public int ScoreID { get; set; }
        public int AthleteAID { get; set; }
        public int AthleteBID { get; set; }
        public int AthleteBConfirmation { get; set; }
        public DateTime Date { get; set; }
        public bool IsUnfinished { get; set; }

        public int SportID { get; set; }
        public int? VenueID { get; set; }
        public int? Type1 { get; set; }

        // example - snooker match score 3:1
        public int PointsA { get; set; }
        public int PointsB { get; set; }

        // example - snooker frame score 76:43
        public int InnerPoints1A { get; set; }
        public int InnerPoints1B { get; set; }
        public int InnerPoints2A { get; set; }
        public int InnerPoints2B { get; set; }
        public int InnerPoints3A { get; set; }
        public int InnerPoints3B { get; set; }
        public int InnerPoints4A { get; set; }
        public int InnerPoints4B { get; set; }
        public int InnerPoints5A { get; set; }
        public int InnerPoints5B { get; set; }
        public int InnerPoints6A { get; set; }
        public int InnerPoints6B { get; set; }
        public int InnerPoints7A { get; set; }
        public int InnerPoints7B { get; set; }
        public int InnerPoints8A { get; set; }
        public int InnerPoints8B { get; set; }
        public int InnerPoints9A { get; set; }
        public int InnerPoints9B { get; set; }
        public int InnerPoints10A { get; set; }
        public int InnerPoints10B { get; set; }

        public string ExtraData { get; set; }

        public DateTime TimeModified { get; set; }
        public bool IsDeleted { get; set; }
        public Guid Guid { get; set; }

        public Score Clone()
        {
            var score = new Score()
            {
                ScoreID = this.ScoreID,
                AthleteAID = this.AthleteAID,
                AthleteBID = this.AthleteBID,
                AthleteBConfirmation = this.AthleteBConfirmation,
                Date = this.Date,
                IsUnfinished = this.IsUnfinished,

                SportID = this.SportID,
                VenueID = this.VenueID,
                Type1 = this.Type1,

                PointsA = this.PointsA,
                PointsB = this.PointsB,

                ExtraData = this.ExtraData,

                TimeModified = this.TimeModified,
                IsDeleted = this.IsDeleted,
                Guid = this.Guid,
            };

            for (int i = 0; i < InnerPointsA.Length; ++i)
            {
                score.UpdateInnerPoints(i + 1, this.InnerPointsA[i], this.InnerPointsB[i]);
            }

            return score;
        }

        public bool IsDifferent(Score score, bool compareConfirmation)
        {
            if (AthleteAID != score.AthleteAID || AthleteBID != score.AthleteBID)
                return true;
            if (System.Math.Abs((Date - score.Date).TotalSeconds) > 1)
                return true;
            if (IsUnfinished != score.IsUnfinished)
                return true;
            if (SportID != score.SportID)
                return true;
            if (VenueID != score.VenueID)
                return true;
            if (Type1 != score.Type1)
                return true;
            if (PointsA != score.PointsA || PointsB != score.PointsB)
                return true;
            if (string.Compare(ExtraData, score.ExtraData) != 0)
                return true;

            if (IsDeleted != score.IsDeleted)
                return true;

            if (compareConfirmation)
                if (this.AthleteBConfirmation != score.AthleteBConfirmation)
                    return true;

            for (int i = 0; i < InnerPointsA.Length; ++i)
            {
                if (InnerPointsA[i] != score.InnerPointsA[i])
                    return true;
                if (InnerPointsB[i] != score.InnerPointsB[i])
                    return true;
            }
            return false;
        }

        public void UpdateInnerPoints(int numberFrom1, int a, int b)
        {
            switch (numberFrom1)
            {
                case 1: InnerPoints1A = a; InnerPoints1B = b; break;
                case 2: InnerPoints2A = a; InnerPoints2B = b; break;
                case 3: InnerPoints3A = a; InnerPoints3B = b; break;
                case 4: InnerPoints4A = a; InnerPoints4B = b; break;
                case 5: InnerPoints5A = a; InnerPoints5B = b; break;
                case 6: InnerPoints6A = a; InnerPoints6B = b; break;
                case 7: InnerPoints7A = a; InnerPoints7B = b; break;
                case 8: InnerPoints8A = a; InnerPoints8B = b; break;
                case 9: InnerPoints9A = a; InnerPoints9B = b; break;
                case 10: InnerPoints10A = a; InnerPoints10B = b; break;
                default: throw new Exception("numberFrom1=" + numberFrom1 + " and must be 0 to 10");
            }
        }

        public void ReverseAandB()
        {
            int oldA = this.AthleteAID;
            this.AthleteAID = this.AthleteBID;
            this.AthleteBID = oldA;

            oldA = this.PointsA;
            this.PointsA = this.PointsB;
            this.PointsB = oldA;

            for (int i = 1; i <= InnerPointsA.Length; ++i)
                UpdateInnerPoints(i, InnerPointsB[i - 1], InnerPointsA[i - 1]);
        }

        public int[] InnerPointsA
        {
            get
            {
                int[] pnts = new int[] { InnerPoints1A, InnerPoints2A, InnerPoints3A, InnerPoints4A, InnerPoints5A, InnerPoints6A, InnerPoints7A, InnerPoints8A, InnerPoints9A, InnerPoints10A };
                return pnts;
            }
        }

        public int[] InnerPointsB
        {
            get
            {
                int[] pnts = new int[] { InnerPoints1B, InnerPoints2B, InnerPoints3B, InnerPoints4B, InnerPoints5B, InnerPoints6B, InnerPoints7B, InnerPoints8B, InnerPoints9B, InnerPoints10B };
                return pnts;
            }
        }

        public virtual Athlete AthleteA { get; set; }
        public virtual Athlete AthleteB { get; set; }
        public virtual Venue Venue { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public override string ToString()
        {
            return PointsA.ToString() + " : " + PointsB.ToString();
        }
    }
}
