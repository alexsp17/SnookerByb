using Awpbs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class SeederQuotes
    {
        private readonly ApplicationDbContext db;

        public SeederQuotes(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void Seed()
        {
            seedAthleticQuotes();
            seedSnookerQuotes();
        }

        private void seedSnookerQuotes()
        {
            var quotes = db.Quotes.Where(i => i.SportID == (int)SportEnum.Snooker).ToList();

            if (quotes.Count > 0)
                return;

            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Snooker,
                QuoteText = "Whoever called snooker 'chess with balls' was rude, but right.",
                Author = "Clive James",
                AuthorCredentials = "",
                Url = ""
            });
            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Snooker,
                QuoteText = "Looking for perfection is the only way to motivate yourself.",
                Author = "Ronnie O'Sullivan",
                AuthorCredentials = "5-time world champion in snooker",
                Url = ""
            });
            db.SaveChanges();
        }

        private void seedAthleticQuotes()
        {
            var quotes = db.Quotes.ToList();

            if (quotes.Count > 0)
                return;

            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Running,
                QuoteText = "Any day I am too busy to run is a day that I am too busy.",
                Author = "John Bryant",
                AuthorCredentials = "",
                Url = ""
            });
            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Unknown,
                QuoteText = "It's supposed to be hard... the hard is what makes it great.",
                Author = "unknown athlete",
                AuthorCredentials = "",
                Url = ""
            });
            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Running,
                QuoteText = "If you are losing faith in human nature, go out and watch a marathon.",
                Author = "Kathrine Switzer",
                AuthorCredentials = "First woman to run a Boston marathon as a numbered entry.",
                Url = "http://en.wikipedia.org/wiki/Kathrine_Switzer"
            });
            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Unknown,
                QuoteText = "You must expect great things from yourself before you can do them.",
                Author = "Michael Jordan",
                AuthorCredentials = "Basketball legend",
                Url = "http://en.wikipedia.org/wiki/Michael_Jordan"
            });
            db.Quotes.Add(new Quote()
            {
                SportID = (int)SportEnum.Running,
                QuoteText = "To give anything but your best is to sacrifice the gift.",
                Author = "Steve Prefontaine",
                AuthorCredentials = "Holder of 7 American records in track events",
                Url = "http://en.wikipedia.org/wiki/Steve_Prefontaine"
            });
            db.SaveChanges();
        }
    }
}
