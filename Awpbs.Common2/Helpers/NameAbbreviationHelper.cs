using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public class NameAbbreviationHelper
    {
        public string GetAbbreviation(string name)
        {
            string []strs = name.Split(' ');

            string abbr = "";
            for (int i = 0; i < strs.Length; ++i)
            {
                if (i > 3)
                    break;
				if (string.IsNullOrEmpty(strs[i]) == false)
                	abbr += strs[i].ToUpper()[0];
            }

            return abbr;
        }
    }
}
