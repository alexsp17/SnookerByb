using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public class BybVersion
    {
		public BybVersion(DateTime releaseDate, int number, string versionName, string versionComments)//string name, string comments)
        {
            this.ReleaseDate = releaseDate;
            this.Number = number;
			this.VersionName = versionName;
			this.VersionComments = versionComments;
        }

        public DateTime ReleaseDate { get; set; }
        public int Number { get; set; }
		public string VersionName { get; set; }
		public string VersionComments { get; set; }

		public override string ToString ()
		{
			return VersionName + " (#" + Number + ")";
		}
    }

    /// <summary>
    /// Snooker Byb mobile app versions
    /// </summary>
    public class SnookerBybMobileVersions
    {
        public static BybVersion Current { get { return All[0]; } }

        public static List<BybVersion> All = new List<BybVersion>()
        {
            new BybVersion(new DateTime(2016, 03, 18), 54, "1.0.14", "FVO beta"),
            new BybVersion(new DateTime(2016, 03, 12), 53, "1.0.13", "refactored, bug fixes"),
            new BybVersion(new DateTime(2016, 03, 01), 52, "1.0.12", "bug fixes and small improvements"),
            new BybVersion(new DateTime(2016, 02, 26), 51, "1.0.11", "Snooker Byb for Venues beta + updated App Store"),
            new BybVersion(new DateTime(2016, 01, 09), 47, "1.0.10", "redesigned record match, fixed deep links"),
			new BybVersion(new DateTime(2015, 12, 11), 43, "1.0.9", "bug fixes"),
			new BybVersion(new DateTime(2015, 12, 10), 42, "1.0.8", "new record match"),
            new BybVersion(new DateTime(2015, 12, 07), 41, "1.0.7", "redesigned community"),
            new BybVersion(new DateTime(2015, 11, 25), 40, "1.0.6", "bug fixes"),
            new BybVersion(new DateTime(2015, 11, 20), 39, "1.0.5", "first release to Google Play"),
            new BybVersion(new DateTime(2015, 11, 12), 38, "1.0.4", "a test release to Google Play"),
            new BybVersion(new DateTime(2015, 11, 05), 36, "1.0.3", "released to the App Store"),
            new BybVersion(new DateTime(2015, 10, 01), 31, "1.0.2", "released to the App Store"),
            new BybVersion(new DateTime(2015,  9,  4), 24, "1.0.1", "released to the App Store"),
            new BybVersion(new DateTime(2015,  8, 26), 23, "1.0.0", "released to the App Store, issues with Facebook login"),
            new BybVersion(new DateTime(2015,  8, 21), 20, "1.0", "pre-release, iOS only"),
            new BybVersion(new DateTime(2015,  8,  7), 16, "1.0", "alpha 2, iOS only"),
            new BybVersion(new DateTime(2015,  8,  6), 15, "1.0", "alpha 1, iOS only"),
        };
    }

    /// <summary>
    /// Byb API versions
    /// </summary>
    public class BybApiVersions
    {
        public static BybVersion Current { get { return All[0]; } }

        public static List<BybVersion> All = new List<BybVersion>()
        {
            new BybVersion(new DateTime(2016, 03, 12), 53, "1.0.13", "bug fixes, auto-confirm matches with friends"),
            new BybVersion(new DateTime(2016, 03, 01), 52, "1.0.12", "bug fixes and small improvements"),
            new BybVersion(new DateTime(2016, 02, 24), 50, "1.0.11", "Snooker Byb for Venues beta"),
            new BybVersion(new DateTime(2016, 01, 09), 46, "1.0.10", "improved emailing"),
            new BybVersion(new DateTime(2015, 12, 10), 42, "1.0.8", "improved venues search"),
            new BybVersion(new DateTime(2015, 12, 07), 41, "1.0.7", "new type of news item - new user"),
            new BybVersion(new DateTime(2015, 11, 05), 36, "1.0.3", "1.0.3"),
            new BybVersion(new DateTime(2015,  9,  4), 24, "1.0.1", "1.0.1"),
            new BybVersion(new DateTime(2015,  8, 26), 23, "1.0.0", "1.0.0"),
            new BybVersion(new DateTime(2015,  8, 21), 20, "1.0.0", "1.0 pre-release"),
        };
    }
}
