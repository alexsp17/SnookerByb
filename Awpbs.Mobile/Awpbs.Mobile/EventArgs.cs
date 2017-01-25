using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class SelectedPersonEventArgs : EventArgs
    {
        public PersonBasicWebModel Person { get; set; }

        public bool IsUnknown { get; set; }
    }

//    public class SelectedVenueEventArgs : EventArgs
//    {
//        public VenueWebModel Venue { get; set; }
//    }

    public class SnookerEventArgs : EventArgs
    {
        public SnookerBreak SnookerBreak { get; set; }
        public SnookerMatchScore MatchScore { get; set; }
    }
}
