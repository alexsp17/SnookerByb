using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Awpbs
{
    /// <summary>
    /// A cloud database entity
    /// </summary>
    public class Sport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int SportID { get; set; }

        public string Name { get; set; }
        public string AthleteThatDoesThisSport { get; set; }
        public bool IsActualSport { get; set; }
        public bool IsPrimarilyDistance { get; set; }

        public bool IsSnooker { get { return SportID == (int)SportEnum.Snooker; } }

        public virtual List<ResultType> ResultTypes { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
