using System;
using System.Collections.Generic;

namespace Awpbs
{
	public class FindVenuesWebModel
	{
		public List<VenueWebModel> Venues { get; set; }

        public MetroWebModel ClosestMetro { get; set; }

        public int TotalCountAvailable { get; set; }
    }
}

