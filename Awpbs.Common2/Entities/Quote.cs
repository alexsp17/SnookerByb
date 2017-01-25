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
    public class Quote
    {
        public int QuoteID { get; set; }
        public string QuoteText { get; set; }
        public string Author { get; set; }
        public string AuthorCredentials { get; set; }
        public string Url { get; set; }
        public int SportID { get; set; }

        public override string ToString()
        {
            return QuoteText;
        }
    }
}
