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
    public class Friendship
    {
        public int FriendshipID { get; set; }
        public int Athlete1ID { get; set; }
        public int Athlete2ID { get; set; }
        public int FriendshipStatus { get; set; }

        public virtual Athlete Athlete1 { get; set; }
        public virtual Athlete Athlete2 { get; set; }
    }
}
