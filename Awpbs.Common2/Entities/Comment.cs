using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Comment
    {
        public int CommentID { get; set; }
        public int AthleteID { get; set; }
        public string CommentText { get; set; }
        public int ObjectType { get; set; }
        public DateTime Time { get; set; }
        public int CommentType { get; set; }

        public int? PostID { get; set; }
        public int? ResultID { get; set; }
        public int? ScoreID { get; set; }
        public int? GameHostID { get; set; }
        public int? NewUserID { get; set; }

        public virtual Athlete Athlete { get; set; }
        public virtual Post Post { get; set; }
        public virtual Result Result { get; set; }
        public virtual Score Score { get; set; }
        public virtual GameHost GameHost { get; set; }
    }
}
