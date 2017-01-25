using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Post
    {
        public int PostID { get; set; }
        public int AthleteID { get; set; }
        public string PostText { get; set; }

        public string Country { get; set; }
        public int? MetroID { get; set; }

        public DateTime Time { get; set; }

        public virtual Athlete Athlete { get; set; }
        public virtual Metro Metro { get; set; }
        public virtual List<Comment> Comments { get; set; }
    }
}
