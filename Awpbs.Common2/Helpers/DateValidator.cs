using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Awpbs
{
    public class DateValidator
    {
        public bool IsOk { get; set; }

        public string ErrorText { get; set; }
        
        public bool Validate(DateTime? date)
        {
            this.ErrorText = "";

            if (date != null)
            {
                if (date.Value.Year < 1901)
                    this.ErrorText = "Long time ago, ugh...";
                if (date.Value.Date > DateTime.Now.AddDays(1))
                    this.ErrorText = "An event in the future?";
            }

            IsOk = this.ErrorText == "";
            return IsOk;
        }
    }
}
