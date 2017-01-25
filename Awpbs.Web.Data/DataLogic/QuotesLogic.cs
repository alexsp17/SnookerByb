using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class QuotesLogic
    {
        private readonly ApplicationDbContext db;

        public QuotesLogic(ApplicationDbContext context)
        {
            db = context;
        }

        public Quote GetRandomForSnooker()
        {
            List<Quote> quotes;
            quotes = db.Quotes.Where(i => i.SportID == (int)SportEnum.Snooker).ToList();

            return pickRandom(quotes);
        }

        public Quote GetRandom(SportEnum? sport, bool includeSnooker = false, bool includeUnknown = true)
        {
            List<Quote> quotes;
            if (sport == null || sport == SportEnum.Unknown)
            {
                if (includeSnooker)
                    quotes = db.Quotes.ToList();
                else
                    quotes = db.Quotes.Where(i => i.SportID != (int)SportEnum.Snooker).ToList();
            }
            else
                quotes = db.Quotes.Where(i => i.SportID == (int)sport || i.SportID == (int)SportEnum.Unknown).ToList();

            return pickRandom(quotes);
        }

        private Quote pickRandom(List<Quote> quotes)
        {
            if (quotes.Count == 0)
                return null;
            int pos = (int)(quotes.Count * new Random().NextDouble());
            if (pos >= quotes.Count)
                return quotes[0];
            return quotes[pos];
        }
    }
}
