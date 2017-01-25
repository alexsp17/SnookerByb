//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Awpbs.Web
//{
//    public class SeederPros
//    {
//        private readonly ApplicationDbContext db;

//        public SeederPros(ApplicationDbContext db)
//        {
//            this.db = db;
//        }

//        public void DeletePros()
//        {

//        }

//        public void Seed()
//        {
//            seedRunningPros();
//        }

//        private void seedRunningPros()
//        {
//            var athletes = db.Athletes.Where(i => i.IsPro == true).ToList();
//            if (athletes.Count > 0)
//                return;

//            /// Pros
//            /// 

//            createProRecord("Michael Johnson", "USA", GenderEnum.Male, new DateTime(1967, 9, 13), SportEnum.Running, "400m", "43.18", new DateTime(1999, 8, 26), WorldRecordEnum.WorldRecord);
//            createProRecord("Marita Koch", "Germany", GenderEnum.Female, new DateTime(1957, 2, 18), SportEnum.Running, "400m", "47.60", new DateTime(1985, 10, 6), WorldRecordEnum.WorldRecord);

//            createProRecord("David Rudisha", "Kenya", GenderEnum.Male, new DateTime(1988, 12, 17), SportEnum.Running, "800m", "1:40.91", new DateTime(2012, 8, 9), WorldRecordEnum.WorldRecord);
//            createProRecord("Jarmila Kratochvílová", "Czechoslovakia", GenderEnum.Female, new DateTime(1988, 12, 17), SportEnum.Running, "800m", "1:53.28", new DateTime(1983, 7, 26), WorldRecordEnum.WorldRecord);

//            createProRecord("Hicham El Guerrouj", "Morocco", GenderEnum.Male, new DateTime(1974, 09, 14), SportEnum.Running, "1mile", "3:43.13", new DateTime(1999, 7, 7), WorldRecordEnum.WorldRecord);
//            createProRecord("Kenenisa Bekele", "Ethiopia", GenderEnum.Male, new DateTime(1982, 06, 13), SportEnum.Running, "5k", "12:37.35", new DateTime(2004, 5, 31), WorldRecordEnum.WorldRecord);
//            createProRecord("Zersenay Tadese", "Eritrea", GenderEnum.Male, new DateTime(1982, 02, 8), SportEnum.Running, "half", "58:23", new DateTime(2010, 3, 21), WorldRecordEnum.WorldRecord);
//            createProRecord("Dennis Kipruto Kimetto", "Kenya", GenderEnum.Male, new DateTime(1984, 01, 22), SportEnum.Running, "full", "2:02:57", new DateTime(2014, 9, 28), WorldRecordEnum.WorldRecord);

//            /// Masters - Male
//            ///

//            // http://en.wikipedia.org/wiki/List_of_world_records_in_masters_athletics

//            createProRecord("Enrico Saraceni", "Italy", GenderEnum.Male, new DateTime(1964, 05, 19), SportEnum.Running, "400m", "47.81", new DateTime(2004, 7, 25), WorldRecordEnum.WorldRecord);
//            createProRecord("Fred Sowerby", "USA", GenderEnum.Male, new DateTime(1948, 12, 11), SportEnum.Running, "400m", "51.39", new DateTime(1999, 8, 27), WorldRecordEnum.WorldRecord);

//            createProRecord("Tony Whiteman", "Great Britain", GenderEnum.Male, new DateTime(1971, 11, 13), SportEnum.Running, "800m", "1:48.22", new DateTime(2012, 6, 6), WorldRecordEnum.WorldRecord);
//            createProRecord("Nolan Shaheed", "USA", GenderEnum.Male, new DateTime(1949, 07, 18), SportEnum.Running, "800m", "1:58.65", new DateTime(2000, 5, 13), WorldRecordEnum.WorldRecord);
//        }

//        private void createProRecord(string name, string country, GenderEnum gender, DateTime dob, SportEnum sport, string typeShortName, string time, DateTime date, WorldRecordEnum worldRecord)
//        {
//            var athlete = db.Athletes.Where(i => i.Name == name && i.DOB == dob).FirstOrDefault();
//            if (athlete != null)
//                return;
//            var resultType = db.ResultTypes.Single(i => i.SportID == (int)sport && i.ShortName == typeShortName);

//            AgeGroupEnum ageGroup = new AgeCalculator().CalcAgeGroup(date, dob, gender);

//            athlete = new Athlete()
//            {
//                IsPro = true,
//                Name = name,
//                DOB = dob.Date,
//                Gender = (int)gender,
//                Country = country,
//                TimeModified = DateTime.UtcNow
//            };
//            db.Athletes.Add(athlete);

//            db.Results.Add(new Result()
//            {
//                Athlete = athlete,
//                Date = date.Date,
//                ResultType = resultType,
//                Time = new TimeHelper().Parse(time),
//                Distance = resultType.Distance,
//                //WorldRecord = (int)worldRecord,
//                //AgeGroup = (int)ageGroup,

//                Guid = Guid.NewGuid(),
//                TimeModified = DateTime.UtcNow
//            });

//            db.SaveChanges();
//        }
//    }
//}
