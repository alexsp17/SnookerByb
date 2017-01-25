using System;
using System.Collections.Generic;
using System.Text;

namespace Awpbs.Mobile
{
    public class CommunitySelection
    {
        public static CommunitySelection CreateDefault(Athlete myAthlete)
        {
            var myCountry = Country.Get(myAthlete.Country);

            if (myAthlete.MetroID > 0)
            {
                var myMetro = App.Cache.Metroes.Get(myAthlete.MetroID);
                if (myMetro == null)
                    myMetro = new MetroWebModel() { ID = myAthlete.MetroID, Name = "Your city", Country = myCountry != null ? myCountry.ThreeLetterCode : "?" };
                var selection = CommunitySelection.CreateAsMetro(myMetro);
                selection.IsMyMetro = true;
                return selection;
            }
            else if (myCountry != null)
            {
                return CommunitySelection.CreateAsCountry(myCountry);
            }
            else
            {
                return CommunitySelection.CreateAsPlanetEarth();
            }
        }

        public static CommunitySelection CreateFriendsOnly()
        {
            return new CommunitySelection()
            {
                IsFriendsOnly = true,
                Country = null,
                MetroID = 0,
            };
        }

        public static CommunitySelection CreateAsPlanetEarth()
        {
            return new CommunitySelection()
            {
                Country = null,
                MetroID = 0,
            };
        }

        public static CommunitySelection CreateAsCountry(Country country)
        {
            return new CommunitySelection()
            {
                Country = country,
                MetroID = 0,
            };
        }

        public static CommunitySelection CreateAsMetro(MetroWebModel metro)
        {
            return new CommunitySelection()
            {
                Country = Country.Get(metro.Country),
                MetroID = metro.ID,
                MetroName = metro.Name,
            };
        }

        private CommunitySelection()
        {
        }

        public bool IsPlanetEarth
        {
            get
            {
                return this.Country == null;
            }
        }

        public bool IsMyMetro { get; private set; }

        public bool IsFriendsOnly { get; private set; }

        public Country Country { get; private set; }

        public int MetroID { get; private set; }

        public string MetroName { get; private set; }

        public string Name
        {
            get
            {
                if (IsFriendsOnly)
                    return "Friends only";

                if (MetroID > 0)
                {
                    if (!string.IsNullOrEmpty(MetroName))
                        return MetroName;
                    else
                        return "Metro #" + MetroID;
                }
                if (Country != null)
                    return Country.ToString();
                return "Planet Earth";
            }
        }

        public bool Compare(CommunitySelection selection)
        {
            if (this.Country == selection.Country && this.MetroID == selection.MetroID && this.IsFriendsOnly == selection.IsFriendsOnly)
                return true;
            else
                return false;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
