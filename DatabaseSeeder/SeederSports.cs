using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class SeederSports
    {
        private readonly ApplicationDbContext db;

        public SeederSports(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void SeedSportsAndResultTypes()
        {
            seedSports();
            seedResultTypes();
        }

        private void seedSports()
        {
            var sportsInDb = db.Sports.ToList();
            var defaultSports = new SportsAndResultTypesRepository().Sports.ToList();

            foreach (var sport in defaultSports)
            {
                if (sport.SportID == (int)SportEnum.Unknown)
                    continue;

                var sportRow = sportsInDb.Where(i => i.SportID == sport.SportID).FirstOrDefault();

                if (sportRow == null)
                {
                    sportRow = new Sport();
                    sportRow.SportID = sport.SportID;
                    db.Sports.Add(sportRow);
                }
                else
                {
                    sportRow.Name = sport.Name;
                }
            }
            db.SaveChanges();
        }

        private void seedResultTypes()
        {
            var resultTypesInDb = db.ResultTypes.ToList();
            var defaultResultTypes = new SportsAndResultTypesRepository().ResultTypes.ToList();

            foreach (var resultType in defaultResultTypes)
            {
                var resultTypeInDb = resultTypesInDb.Where(i => i.ResultTypeID == resultType.ResultTypeID).FirstOrDefault();

                if (resultTypeInDb == null)
                {
                    resultTypeInDb = new ResultType();
                    db.ResultTypes.Add(resultTypeInDb);
                }

                resultTypeInDb.Distance = resultType.Distance;
                resultTypeInDb.IsCountRequired = resultType.IsCountRequired;
                resultTypeInDb.IsCount2Available = resultType.IsCount2Available;
                resultTypeInDb.CountName = resultType.CountName;
                resultTypeInDb.Count2Name = resultType.Count2Name;
                resultTypeInDb.Name = resultType.Name;
                resultTypeInDb.ResultTypeID = resultType.ResultTypeID;
                resultTypeInDb.ShortName = resultType.ShortName;
                resultTypeInDb.SportID = resultType.SportID;
                resultTypeInDb.Time = resultType.Time;
            }

            db.SaveChanges();
        }
    }
}
