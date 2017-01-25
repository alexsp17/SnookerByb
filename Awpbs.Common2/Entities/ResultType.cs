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
    public class ResultType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ResultTypeID { get; set; }

        public int SportID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public double? Distance { get; set; }
        public double? Time { get; set; }

        public bool IsCountRequired { get; set; } // =true for snooker
        public bool IsCount2Available { get; set; } // =true for snooker

        public string CountName { get; set; }
        public string Count2Name { get; set; }

        public virtual Sport Sport { get; set; }
        public virtual List<Result> Results { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
