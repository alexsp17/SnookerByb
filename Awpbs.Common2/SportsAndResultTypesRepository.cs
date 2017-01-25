using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Awpbs
{
    public enum SportEnum
    {
        Unknown = 0,

        Running = 1,
        Cycling = 2,
        Swimming = 3,

        Weightlifting = 4,
        Powerlifting = 5,
        Pullups = 6,
        Pushups = 7,

        Snooker = 101
    }

    public class SportsAndResultTypesRepository
    {
        public SportsAndResultTypesRepository()
        {
            init();
        }

        public Sport GetSport(int sport)
        {
            return Sports.Where(i => i.SportID == (int)sport).Single();
        }

        public Sport GetSport(SportEnum sport)
        {
            return Sports.Where(i => i.SportID == (int)sport).Single();
        }

        public List<Sport> Sports
        {
            get
            {
                return (from s in sports
                        select s).ToList();
            }
        }

        public List<ResultType> ResultTypes
        {
            get
            {
                return (from s in sports
                        from r in s.ResultTypes
                        select r).ToList();
            }
        }

        private static List<Sport> sports;

        private static void init()
        {
            if (sports != null)
                return;

            sports = new List<Sport>();

            var enums = Enum.GetValues(typeof(SportEnum));
            foreach (var e in enums)
            {
                if ((int)e == (int)SportEnum.Unknown)
                    continue;

                var sport = new Sport();
                sports.Add(sport);
                sport.SportID = (int)e;
                sport.IsPrimarilyDistance = false;
                switch ((SportEnum)e)
                {
                    case SportEnum.Running:
                        sport.Name = "Running";
                        sport.AthleteThatDoesThisSport = "Runner";
                        sport.IsActualSport = true;
                        sport.IsPrimarilyDistance = true;
                        sport.ResultTypes = running;
                        break;
                    case SportEnum.Swimming:
                        sport.Name = "Swimming";
                        sport.AthleteThatDoesThisSport = "Swimmer";
                        sport.IsActualSport = true;
                        sport.IsPrimarilyDistance = true;
                        sport.ResultTypes = swimming;
                        break;
                    case SportEnum.Cycling:
                        sport.Name = "Cycling";
                        sport.AthleteThatDoesThisSport = "Cyclist";
                        sport.IsActualSport = true;
                        sport.IsPrimarilyDistance = true;
                        sport.ResultTypes = cycling;
                        break;
                    case SportEnum.Weightlifting:
                        sport.Name = "Weightlifting";
                        sport.AthleteThatDoesThisSport = "Weightlifter";
                        sport.IsActualSport = true;
                        sport.ResultTypes = weightlifting;
                        break;
                    case SportEnum.Powerlifting:
                        sport.Name = "Powerlifting";
                        sport.AthleteThatDoesThisSport = "Powerlifter";
                        sport.IsActualSport = true;
                        sport.ResultTypes = powerlifting;
                        break;
                    case SportEnum.Pullups:
                        sport.Name = "Pull-ups";
                        sport.AthleteThatDoesThisSport = "Athlete";
                        sport.IsActualSport = false;
                        sport.ResultTypes = pullups;
                        break;
                    case SportEnum.Pushups:
                        sport.Name = "Push-ups";
                        sport.AthleteThatDoesThisSport = "Athlete";
                        sport.IsActualSport = false;
                        sport.ResultTypes = pushups;
                        break;
                    case SportEnum.Snooker:
                        sport.Name = "Snooker";
                        sport.AthleteThatDoesThisSport = "Snooker player";
                        sport.IsActualSport = true;
                        sport.ResultTypes = snooker;
                        break;
                    default:
                        sport.Name = "Unknown (error)";
                        sport.ResultTypes = new List<ResultType>();
                        break;
                }
                foreach (var resultType in sport.ResultTypes)
                {
                    resultType.SportID = sport.SportID;
                    resultType.Sport = sport;
                }
            }
        }

        private static List<ResultType> running = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 1000400,
                Name = "400 meters",
                ShortName = "400m",
                Distance = 400,
            },
            new ResultType()
            {
                ResultTypeID = 1000800,
                Name = "800 meters",
                ShortName = "800m",
                Distance = 800,
            },
            new ResultType()
            {
                ResultTypeID = 1001500,
                Name = "1500 meters",
                ShortName = "1500m",
                Distance = 1500,
            },
            new ResultType()
            {
                ResultTypeID = 1001600,
                Name = "1 mile",
                ShortName = "1mile",
                Distance = 1600,
            },
            new ResultType()
            {
                ResultTypeID = 1005000,
                Name = "5 km",
                ShortName = "5k",
                Distance = 5000,
            },
            new ResultType()
            {
                ResultTypeID = 1010000,
                Name = "10 km",
                ShortName = "10k",
                Distance = 10000,
            },
           new ResultType()
            {
                ResultTypeID = 1021100,
                Name = "Half marathon",
                ShortName = "half",
                Distance = 21097,
            },
            new ResultType()
            {
                ResultTypeID = 1042200,
                Name = "Marathon",
                ShortName = "full",
                Distance = 42195,
            },
        };

        private static List<ResultType> cycling = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 2000010,
                Name = "10 km",
                ShortName = "10k",
                Distance = 10000,
            },
            new ResultType()
            {
                ResultTypeID = 2000020,
                Name = "20 km",
                ShortName = "20k",
                Distance = 20000,
            },
            new ResultType()
            {
                ResultTypeID = 2000050,
                Name = "50 km",
                ShortName = "50k",
                Distance = 50000,
            },
            new ResultType()
            {
                ResultTypeID = 2000100,
                Name = "100 km",
                ShortName = "100k",
                Distance = 100000,
            },
            new ResultType()
            {
                ResultTypeID = 2000160,
                Name = "100 miles",
                ShortName = "100miles",
                Distance = 100000 * 1.60934,
            }
        };

        private static List<ResultType> swimming = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 3000050,
                Name = "50 yards",
                ShortName = "50yd",
                Distance = 50 * 0.9144,
            },
            new ResultType()
            {
                ResultTypeID = 3000100,
                Name = "100 yards",
                ShortName = "100yd",
                Distance = 100 * 0.9144,
            },
            new ResultType()
            {
                ResultTypeID = 3000200,
                Name = "200 yards",
                ShortName = "200yd",
                Distance = 200 * 0.9144,
            },
            new ResultType()
            {
                ResultTypeID = 3000400,
                Name = "400 yards",
                ShortName = "400yd",
                Distance = 400 * 0.9144,
            },
            new ResultType()
            {
                ResultTypeID = 3000450,
                Name = "450 yards",
                ShortName = "450yd",
                Distance = 450 * 0.9144,
            },
            new ResultType()
            {
                ResultTypeID = 3001650,
                Name = "1650 yards",
                ShortName = "1650yd",
                Distance = 1650 * 0.9144,
            }
        };

        private static List<ResultType> weightlifting = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 4000001,
                Name = "Snatch",
                ShortName = "Snatch",
                Distance = null,
                IsCountRequired = true
            },
            new ResultType()
            {
                ResultTypeID = 4000002,
                Name = "Jerk",
                ShortName = "Jerk",
                Distance = null,
                IsCountRequired = true
            },
        };

        private static List<ResultType> powerlifting = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 5000001,
                Name = "Squat",
                ShortName = "Squat",
                Distance = null,
                IsCountRequired = true
            },
            new ResultType()
            {
                ResultTypeID = 5000002,
                Name = "Bench press",
                ShortName = "Bench press",
                Distance = null,
                IsCountRequired = true
            },
            new ResultType()
            {
                ResultTypeID = 5000003,
                Name = "Deadlift",
                ShortName = "Deadlift",
                Distance = null,
                IsCountRequired = true
            },
        };

        private static List<ResultType> pullups = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 6000001,
                Name = "Pull-ups total",
                ShortName = "Pull-ups total",
                Distance = null,
                IsCountRequired = true,
                CountName = "Count"
            },
            new ResultType()
            {
                ResultTypeID = 6000002,
                Name = "Pull-ups in 1 minute",
                ShortName = "Pull-ups in 1 minute",
                Distance = null,
                IsCountRequired = true,
                CountName = "Count"
            },
        };

        private static List<ResultType> pushups = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 7000001,
                Name = "Push-ups",
                ShortName = "Push-ups",
                Distance = null,
                IsCountRequired = true,
                CountName = "Count"
            },
        };

        private static List<ResultType> snooker = new List<ResultType>()
        {
            new ResultType()
            {
                ResultTypeID = 101000001,
                Name = "Break",
                ShortName = "Break",
                Distance = null,
                IsCountRequired = true,
                IsCount2Available = true,
                CountName = "Break score",
                Count2Name = "Ball count"
            },
        };
    }
}
