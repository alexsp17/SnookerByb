using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs
{
    public enum SnookerBreakSortEnum
    {
        ByDate = 0,
        ByPoints = 1,
        ByBallCount = 2,
    }

    public enum SnookerMatchSortEnum
    {
        ByDate = 0,
        ByFrameCount = 1,
        ByWinFirst = 2,
        ByLossFirst = 3,
        ByBestFrame = 4,
        ByOpponent = 5
    }

    public enum SnookerOpponentSortEnum
    {
        ByName = 0,
        ByWinFirst = 1,
        ByLossFirst = 2,
		ByMatchCount = 3,
    }

    public enum SnookerVenuesPlayedSortEnum
    {
        ByName = 0,
        ByCount = 1,
    }

    public enum SnookerTableSizeEnum
    {
        Unknown = 0,
        Table10Ft = 10,
        Table12Ft = 12
    }

    public class SnookerTableSizes
    {
        public static readonly Dictionary<SnookerTableSizeEnum, string> All = new Dictionary<SnookerTableSizeEnum, string>
        {
            { SnookerTableSizeEnum.Table10Ft, "10' table" },
            { SnookerTableSizeEnum.Table12Ft, "12' table" },
        };

        public static string ToString(SnookerTableSizeEnum obj)
        {
            if (obj == SnookerTableSizeEnum.Unknown)
                return "-";
            return All[obj];
        }

        public static SnookerTableSizeEnum FromString(string str)
        {
            if (All.Where(i => i.Value == str).Count() == 0)
                return SnookerTableSizeEnum.Unknown;
            return All.Where(i => i.Value == str).Select(i => i.Key).Single();
        }
    }
}
