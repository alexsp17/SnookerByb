using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class SnookerOpponent
    {
        public PersonBasicWebModel Person { get; set; }

        public int CountLosses { get; set; }
        public int CountDraws { get; set; }
        public int CountWins { get; set; }

		public int CountMatches { get { return this.CountLosses + this.CountDraws + this.CountWins; } }

        public static List<SnookerOpponent> Sort(List<SnookerOpponent> list, SnookerOpponentSortEnum sort)
        {
            if (sort == SnookerOpponentSortEnum.ByName)
            {
                return (from i in list
                        orderby i.Person.Name
                        select i).ToList();
            }
			if (sort == SnookerOpponentSortEnum.ByMatchCount)
			{
				return (from i in list
						orderby i.CountMatches descending, i.Person.Name
					    select i).ToList();
			}
            if (sort == SnookerOpponentSortEnum.ByWinFirst)
            {
                return (from i in list
                        orderby i.CountWins - i.CountLosses descending, i.Person.Name
                        select i).ToList();
            }
            if (sort == SnookerOpponentSortEnum.ByLossFirst)
            {
                return (from i in list
                        orderby i.CountWins - i.CountLosses ascending, i.Person.Name
                        select i).ToList();
            }
            return list;
        }
    }
}
