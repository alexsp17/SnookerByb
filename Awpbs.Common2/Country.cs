/*
 * ISO 3166-1 country codes
 * 
 * Data source: http://en.wikipedia.org/wiki/ISO_3166-1
 * License: http://creativecommons.org/licenses/by-sa/3.0/
 */
using System.Collections.Generic;
using System.Linq;

namespace Awpbs
{
    public enum CountrySizeEnum
    {
        //Small = 1,
        Regular = 5,
        VeryLarge = 10,
    }

    public enum CountryImportanceEnum
    {
        Importance0 = 0,
        Importance1 = 1,
        Importance5 = 5,
        Importance9 = 9
    }

    public class Country
    {
        public string Name { get; private set; }
        public string TwoLetterCode { get; private set; }
        public string ThreeLetterCode { get; private set; }
        public string NumericCode { get; private set; }
        public string UrlName { get; private set; }
        public string LocalizedName { get; private set; }

        public CountryImportanceEnum Snooker { get; private set; }
        public bool CouldBeIgnored { get; private set; }
        public CountrySizeEnum AreaSize { get; private set; }

        public string Code
        {
            get { return ThreeLetterCode; }
        }

        public bool IsUSA
        {
            get { return ThreeLetterCode == "USA"; }
        }

        public bool IsBritain
        {
            get { return ThreeLetterCode == "GBR"; }
        }

        public bool IsCanada
        {
            get { return ThreeLetterCode == "CAN"; }
        }

        private Country(string name, string twoLetterCode, string threeLetterCode, string numericCode, CountryImportanceEnum snooker = CountryImportanceEnum.Importance1)
        {
            Name = name;
            TwoLetterCode = twoLetterCode;
            ThreeLetterCode = threeLetterCode;
            NumericCode = numericCode;
            UrlName = threeLetterCode;
            LocalizedName = name;

            CouldBeIgnored = false;
            AreaSize = CountrySizeEnum.Regular;
            Snooker = snooker;
        }

        public static Country Get(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            if (name.Length == 2)
                return List.Where(i => i.TwoLetterCode == name.ToUpper()).SingleOrDefault();
            if (name.Length == 3)
                return List.Where(i => i.ThreeLetterCode == name.ToUpper()).SingleOrDefault();
            return List.Where(i => i.Name.ToUpper() == name.ToUpper()).SingleOrDefault();
        }

        public static Country USA { get { return List.Where(i => i.IsUSA).Single(); } }
        public static Country Britain { get { return List.Where(i => i.IsBritain).Single(); } }
        public static Country Canada { get { return List.Where(i => i.IsCanada).Single(); } }

        public static List<Country> ListWithoutImportance0
        {
            get
            {
                return (from i in List
                        where i.Snooker != CountryImportanceEnum.Importance0
                        select i).ToList();
            }
        }

        public static readonly Country[] List = new[]
        {
            new Country("Afghanistan", "AF", "AFG", "004", CountryImportanceEnum.Importance0),
            new Country("Åland Islands", "AX", "ALA", "248", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Albania", "AL", "ALB", "008", CountryImportanceEnum.Importance0),
            new Country("Algeria", "DZ", "DZA", "012", CountryImportanceEnum.Importance0),
            new Country("American Samoa", "AS", "ASM", "016", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Andorra", "AD", "AND", "020", CountryImportanceEnum.Importance0),
            new Country("Angola", "AO", "AGO", "024", CountryImportanceEnum.Importance0),
            new Country("Anguilla", "AI", "AIA", "660", CountryImportanceEnum.Importance0),
            new Country("Antarctica", "AQ", "ATA", "010", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Antigua and Barbuda", "AG", "ATG", "028", CountryImportanceEnum.Importance0),
            new Country("Argentina", "AR", "ARG", "032", CountryImportanceEnum.Importance1),
            new Country("Armenia", "AM", "ARM", "051", CountryImportanceEnum.Importance5),
            new Country("Aruba", "AW", "ABW", "533", CountryImportanceEnum.Importance0),
            new Country("Australia", "AU", "AUS", "036", CountryImportanceEnum.Importance5) { UrlName = "Australia" },
            new Country("Austria", "AT", "AUT", "040", CountryImportanceEnum.Importance5) { AreaSize = CountrySizeEnum.VeryLarge },
            new Country("Azerbaijan", "AZ", "AZE", "031", CountryImportanceEnum.Importance1),
            new Country("Bahamas", "BS", "BHS", "044", CountryImportanceEnum.Importance0),
            new Country("Bahrain", "BH", "BHR", "048", CountryImportanceEnum.Importance0),
            new Country("Bangladesh", "BD", "BGD", "050", CountryImportanceEnum.Importance0),
            new Country("Barbados", "BB", "BRB", "052", CountryImportanceEnum.Importance0),
            new Country("Belarus", "BY", "BLR", "112", CountryImportanceEnum.Importance5),
            new Country("Belgium", "BE", "BEL", "056", CountryImportanceEnum.Importance5),
            new Country("Belize", "BZ", "BLZ", "084", CountryImportanceEnum.Importance0),
            new Country("Benin", "BJ", "BEN", "204", CountryImportanceEnum.Importance0),
            new Country("Bermuda", "BM", "BMU", "060", CountryImportanceEnum.Importance0),
            new Country("Bhutan", "BT", "BTN", "064", CountryImportanceEnum.Importance0),
            new Country("Bolivia", "BO", "BOL", "068", CountryImportanceEnum.Importance0), // Bolivia, Plurinational State of
            new Country("Bonaire", "BQ", "BES", "535", CountryImportanceEnum.Importance0), // Bonaire, Sint Eustatius and Saba
            new Country("Bosnia and Herzegovina", "BA", "BIH", "070", CountryImportanceEnum.Importance0),
            new Country("Botswana", "BW", "BWA", "072", CountryImportanceEnum.Importance0),
            new Country("Bouvet Island", "BV", "BVT", "074", CountryImportanceEnum.Importance0),
            new Country("Brazil", "BR", "BRA", "076", CountryImportanceEnum.Importance5) { AreaSize = CountrySizeEnum.VeryLarge },
            new Country("British Indian Ocean Territory", "IO", "IOT", "086", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Brunei Darussalam", "BN", "BRN", "096", CountryImportanceEnum.Importance0),
            new Country("Bulgaria", "BG", "BGR", "100", CountryImportanceEnum.Importance0),
            new Country("Burkina Faso", "BF", "BFA", "854", CountryImportanceEnum.Importance0),
            new Country("Burundi", "BI", "BDI", "108", CountryImportanceEnum.Importance0),
            new Country("Cambodia", "KH", "KHM", "116", CountryImportanceEnum.Importance0),
            new Country("Cameroon", "CM", "CMR", "120", CountryImportanceEnum.Importance0),
            new Country("Canada", "CA", "CAN", "124", CountryImportanceEnum.Importance9) { AreaSize = CountrySizeEnum.VeryLarge,UrlName = "Canada" },
            new Country("Cabo Verde", "CV", "CPV", "132", CountryImportanceEnum.Importance0),
            new Country("Cayman Islands", "KY", "CYM", "136", CountryImportanceEnum.Importance0),
            new Country("Central African Republic", "CF", "CAF", "140", CountryImportanceEnum.Importance0),
            new Country("Chad", "TD", "TCD", "148", CountryImportanceEnum.Importance0),
            new Country("Chile", "CL", "CHL", "152", CountryImportanceEnum.Importance1),
            new Country("China", "CN", "CHN", "156", CountryImportanceEnum.Importance5) { AreaSize = CountrySizeEnum.VeryLarge,UrlName = "China" },
            new Country("Christmas Island", "CX", "CXR", "162", CountryImportanceEnum.Importance0)  { CouldBeIgnored = true },
            new Country("Cocos (Keeling) Islands", "CC", "CCK", "166", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Colombia", "CO", "COL", "170", CountryImportanceEnum.Importance1),
            new Country("Comoros", "KM", "COM", "174", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Congo", "CG", "COG", "178", CountryImportanceEnum.Importance0),
            new Country("Congo, the Democratic Republic", "CD", "COD", "180", CountryImportanceEnum.Importance0), // Congo, the Democratic Republic of the
            new Country("Cook Islands", "CK", "COK", "184", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Costa Rica", "CR", "CRI", "188", CountryImportanceEnum.Importance0),
            new Country("Côte d'Ivoire", "CI", "CIV", "384", CountryImportanceEnum.Importance0),
            new Country("Croatia", "HR", "HRV", "191", CountryImportanceEnum.Importance1),
            new Country("Cuba", "CU", "CUB", "192", CountryImportanceEnum.Importance0),
            new Country("Curaçao", "CW", "CUW", "531", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Cyprus", "CY", "CYP", "196", CountryImportanceEnum.Importance1),
            new Country("Czech Republic", "CZ", "CZE", "203", CountryImportanceEnum.Importance5),
            new Country("Denmark", "DK", "DNK", "208", CountryImportanceEnum.Importance5),
            new Country("Djibouti", "DJ", "DJI", "262", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Dominica", "DM", "DMA", "212", CountryImportanceEnum.Importance0),
            new Country("Dominican Republic", "DO", "DOM", "214", CountryImportanceEnum.Importance0),
            new Country("Ecuador", "EC", "ECU", "218", CountryImportanceEnum.Importance0),
            new Country("Egypt", "EG", "EGY", "818", CountryImportanceEnum.Importance1),
            new Country("El Salvador", "SV", "SLV", "222", CountryImportanceEnum.Importance0),
            new Country("Equatorial Guinea", "GQ", "GNQ", "226", CountryImportanceEnum.Importance0),
            new Country("Eritrea", "ER", "ERI", "232", CountryImportanceEnum.Importance0),
            new Country("Estonia", "EE", "EST", "233", CountryImportanceEnum.Importance5),
            new Country("Ethiopia", "ET", "ETH", "231", CountryImportanceEnum.Importance0),
            new Country("Falkland Islands (Malvinas)", "FK", "FLK", "238", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Faroe Islands", "FO", "FRO", "234", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Fiji", "FJ", "FJI", "242", CountryImportanceEnum.Importance0),
            new Country("Finland", "FI", "FIN", "246", CountryImportanceEnum.Importance5),
            new Country("France", "FR", "FRA", "250", CountryImportanceEnum.Importance5),
            new Country("French Guiana", "GF", "GUF", "254", CountryImportanceEnum.Importance0),
            new Country("French Polynesia", "PF", "PYF", "258", CountryImportanceEnum.Importance0),
            new Country("French Southern Territories", "TF", "ATF", "260", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Gabon", "GA", "GAB", "266", CountryImportanceEnum.Importance0),
            new Country("Gambia", "GM", "GMB", "270", CountryImportanceEnum.Importance0),
            new Country("Georgia", "GE", "GEO", "268", CountryImportanceEnum.Importance1),
			new Country("Germany", "DE", "DEU", "276", CountryImportanceEnum.Importance9) { LocalizedName = "Deutschland" },
            new Country("Ghana", "GH", "GHA", "288", CountryImportanceEnum.Importance0),
            new Country("Gibraltar", "GI", "GIB", "292", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Greece", "GR", "GRC", "300", CountryImportanceEnum.Importance5),
            new Country("Greenland", "GL", "GRL", "304", CountryImportanceEnum.Importance0),
            new Country("Grenada", "GD", "GRD", "308", CountryImportanceEnum.Importance0),
            new Country("Guadeloupe", "GP", "GLP", "312", CountryImportanceEnum.Importance0),
            new Country("Guam", "GU", "GUM", "316", CountryImportanceEnum.Importance0),
            new Country("Guatemala", "GT", "GTM", "320", CountryImportanceEnum.Importance0),
            new Country("Guernsey", "GG", "GGY", "831", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Guinea", "GN", "GIN", "324", CountryImportanceEnum.Importance0),
            new Country("Guinea-Bissau", "GW", "GNB", "624", CountryImportanceEnum.Importance0),
            new Country("Guyana", "GY", "GUY", "328", CountryImportanceEnum.Importance0),
            new Country("Haiti", "HT", "HTI", "332", CountryImportanceEnum.Importance0),
            new Country("Heard Island and McDonald Islands", "HM", "HMD", "334", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Holy See (Vatican City State)", "VA", "VAT", "336", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Honduras", "HN", "HND", "340", CountryImportanceEnum.Importance0),
            new Country("Hong Kong", "HK", "HKG", "344", CountryImportanceEnum.Importance5),
            new Country("Hungary", "HU", "HUN", "348", CountryImportanceEnum.Importance5),
            new Country("Iceland", "IS", "ISL", "352", CountryImportanceEnum.Importance5),
            new Country("India", "IN", "IND", "356", CountryImportanceEnum.Importance5),
            new Country("Indonesia", "ID", "IDN", "360", CountryImportanceEnum.Importance5),
            new Country("Iran", "IR", "IRN", "364", CountryImportanceEnum.Importance5), //Iran, Islamic Republic of
            new Country("Iraq", "IQ", "IRQ", "368", CountryImportanceEnum.Importance5),
            new Country("Ireland", "IE", "IRL", "372", CountryImportanceEnum.Importance5),
            new Country("Isle of Man", "IM", "IMN", "833", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Israel", "IL", "ISR", "376", CountryImportanceEnum.Importance5),
            new Country("Italy", "IT", "ITA", "380", CountryImportanceEnum.Importance5),
            new Country("Jamaica", "JM", "JAM", "388", CountryImportanceEnum.Importance0),
            new Country("Japan", "JP", "JPN", "392", CountryImportanceEnum.Importance5),
            new Country("Jersey", "JE", "JEY", "832", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Jordan", "JO", "JOR", "400", CountryImportanceEnum.Importance5),
            new Country("Kazakhstan", "KZ", "KAZ", "398", CountryImportanceEnum.Importance5),
            new Country("Kenya", "KE", "KEN", "404", CountryImportanceEnum.Importance0),
            new Country("Kiribati", "KI", "KIR", "296", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Korea (North Korea)", "KP", "PRK", "408", CountryImportanceEnum.Importance0), //Korea, Democratic People's Republic of
            new Country("Korea, Republic of", "KR", "KOR", "410", CountryImportanceEnum.Importance5),
            new Country("Kuwait", "KW", "KWT", "414", CountryImportanceEnum.Importance5),
            new Country("Kyrgyzstan", "KG", "KGZ", "417", CountryImportanceEnum.Importance1),
            new Country("Lao People's Democratic Republic", "LA", "LAO", "418", CountryImportanceEnum.Importance0),
            new Country("Latvia", "LV", "LVA", "428", CountryImportanceEnum.Importance5),
            new Country("Lebanon", "LB", "LBN", "422", CountryImportanceEnum.Importance5),
            new Country("Lesotho", "LS", "LSO", "426", CountryImportanceEnum.Importance0),
            new Country("Liberia", "LR", "LBR", "430", CountryImportanceEnum.Importance0),
            new Country("Libya", "LY", "LBY", "434", CountryImportanceEnum.Importance0),
            new Country("Liechtenstein", "LI", "LIE", "438", CountryImportanceEnum.Importance0),
            new Country("Lithuania", "LT", "LTU", "440", CountryImportanceEnum.Importance9) { UrlName = "Lietuva", LocalizedName = "Lietuva" },
            new Country("Luxembourg", "LU", "LUX", "442", CountryImportanceEnum.Importance1),
            new Country("Macao", "MO", "MAC", "446", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Macedonia", "MK", "MKD", "807", CountryImportanceEnum.Importance0), //Macedonia, the former Yugoslav Republic of
            new Country("Madagascar", "MG", "MDG", "450", CountryImportanceEnum.Importance0),
            new Country("Malawi", "MW", "MWI", "454", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Malaysia", "MY", "MYS", "458", CountryImportanceEnum.Importance0),
            new Country("Maldives", "MV", "MDV", "462", CountryImportanceEnum.Importance0),
            new Country("Mali", "ML", "MLI", "466", CountryImportanceEnum.Importance0),
            new Country("Malta", "MT", "MLT", "470", CountryImportanceEnum.Importance0),
            new Country("Marshall Islands", "MH", "MHL", "584", CountryImportanceEnum.Importance0),
            new Country("Martinique", "MQ", "MTQ", "474", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Mauritania", "MR", "MRT", "478", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Mauritius", "MU", "MUS", "480", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Mayotte", "YT", "MYT", "175", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Mexico", "MX", "MEX", "484", CountryImportanceEnum.Importance5),
            new Country("Micronesia", "FM", "FSM", "583", CountryImportanceEnum.Importance0) { CouldBeIgnored = true }, //Micronesia, Federated States of
            new Country("Moldova", "MD", "MDA", "498", CountryImportanceEnum.Importance1), //Moldova, Republic of
            new Country("Monaco", "MC", "MCO", "492", CountryImportanceEnum.Importance0),
            new Country("Mongolia", "MN", "MNG", "496", CountryImportanceEnum.Importance0),
            new Country("Montenegro", "ME", "MNE", "499", CountryImportanceEnum.Importance0),
            new Country("Montserrat", "MS", "MSR", "500", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Morocco", "MA", "MAR", "504", CountryImportanceEnum.Importance0),
            new Country("Mozambique", "MZ", "MOZ", "508", CountryImportanceEnum.Importance0),
            new Country("Myanmar", "MM", "MMR", "104", CountryImportanceEnum.Importance1),
            new Country("Namibia", "NA", "NAM", "516", CountryImportanceEnum.Importance0),
            new Country("Nauru", "NR", "NRU", "520", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Nepal", "NP", "NPL", "524", CountryImportanceEnum.Importance0),
            new Country("Netherlands", "NL", "NLD", "528", CountryImportanceEnum.Importance5),
            new Country("New Caledonia", "NC", "NCL", "540", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("New Zealand", "NZ", "NZL", "554", CountryImportanceEnum.Importance5),
            new Country("Nicaragua", "NI", "NIC", "558", CountryImportanceEnum.Importance0),
            new Country("Niger", "NE", "NER", "562", CountryImportanceEnum.Importance0),
            new Country("Nigeria", "NG", "NGA", "566", CountryImportanceEnum.Importance0),
            new Country("Niue", "NU", "NIU", "570", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Norfolk Island", "NF", "NFK", "574", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Northern Mariana Islands", "MP", "MNP", "580", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Norway", "NO", "NOR", "578", CountryImportanceEnum.Importance5),
            new Country("Oman", "OM", "OMN", "512", CountryImportanceEnum.Importance0),
            new Country("Pakistan", "PK", "PAK", "586", CountryImportanceEnum.Importance5),
            new Country("Palau", "PW", "PLW", "585", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Palestine", "PS", "PSE", "275", CountryImportanceEnum.Importance0), //Palestine, State of
            new Country("Panama", "PA", "PAN", "591", CountryImportanceEnum.Importance0),
            new Country("Papua New Guinea", "PG", "PNG", "598", CountryImportanceEnum.Importance0),
            new Country("Paraguay", "PY", "PRY", "600", CountryImportanceEnum.Importance0),
            new Country("Peru", "PE", "PER", "604", CountryImportanceEnum.Importance0),
            new Country("Philippines", "PH", "PHL", "608", CountryImportanceEnum.Importance5),
            new Country("Pitcairn", "PN", "PCN", "612", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Poland", "PL", "POL", "616", CountryImportanceEnum.Importance5),
            new Country("Portugal", "PT", "PRT", "620", CountryImportanceEnum.Importance5),
            new Country("Puerto Rico", "PR", "PRI", "630", CountryImportanceEnum.Importance0),
            new Country("Qatar", "QA", "QAT", "634", CountryImportanceEnum.Importance5),
            new Country("Réunion", "RE", "REU", "638", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Romania", "RO", "ROU", "642", CountryImportanceEnum.Importance5),
            new Country("Russian Federation", "RU", "RUS", "643", CountryImportanceEnum.Importance5) { AreaSize = CountrySizeEnum.VeryLarge },
            new Country("Rwanda", "RW", "RWA", "646", CountryImportanceEnum.Importance0),
            new Country("Saint Barthélemy", "BL", "BLM", "652", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Saint Helena", "SH", "SHN", "654", CountryImportanceEnum.Importance0) { CouldBeIgnored = true }, //Saint Helena, Ascension and Tristan da Cunha
            new Country("Saint Kitts and Nevis", "KN", "KNA", "659", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Saint Lucia", "LC", "LCA", "662", CountryImportanceEnum.Importance0),
            new Country("Saint Martin", "MF", "MAF", "663", CountryImportanceEnum.Importance0), //Saint Martin (French part)
            new Country("Saint Pierre and Miquelon", "PM", "SPM", "666", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Saint Vincent and the Grenadines", "VC", "VCT", "670", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Samoa", "WS", "WSM", "882", CountryImportanceEnum.Importance0),
            new Country("San Marino", "SM", "SMR", "674", CountryImportanceEnum.Importance0),
            new Country("Sao Tome and Principe", "ST", "STP", "678", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Saudi Arabia", "SA", "SAU", "682", CountryImportanceEnum.Importance5),
            new Country("Senegal", "SN", "SEN", "686", CountryImportanceEnum.Importance0),
            new Country("Serbia", "RS", "SRB", "688", CountryImportanceEnum.Importance5),
            new Country("Seychelles", "SC", "SYC", "690", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Sierra Leone", "SL", "SLE", "694", CountryImportanceEnum.Importance0),
            new Country("Singapore", "SG", "SGP", "702", CountryImportanceEnum.Importance5),
            new Country("Sint Maarten (Dutch part)", "SX", "SXM", "534", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Slovakia", "SK", "SVK", "703", CountryImportanceEnum.Importance5),
            new Country("Slovenia", "SI", "SVN", "705", CountryImportanceEnum.Importance5),
            new Country("Solomon Islands", "SB", "SLB", "090", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Somalia", "SO", "SOM", "706", CountryImportanceEnum.Importance0),
            new Country("South Africa", "ZA", "ZAF", "710", CountryImportanceEnum.Importance5),
            new Country("South Georgia and the South Sandwich Islands", "GS", "SGS", "239", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("South Sudan", "SS", "SSD", "728", CountryImportanceEnum.Importance0),
            new Country("Spain", "ES", "ESP", "724", CountryImportanceEnum.Importance5),
            new Country("Sri Lanka", "LK", "LKA", "144", CountryImportanceEnum.Importance0),
            new Country("Sudan", "SD", "SDN", "729", CountryImportanceEnum.Importance0),
            new Country("Suriname", "SR", "SUR", "740", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Svalbard and Jan Mayen", "SJ", "SJM", "744", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Swaziland", "SZ", "SWZ", "748", CountryImportanceEnum.Importance0),
            new Country("Sweden", "SE", "SWE", "752", CountryImportanceEnum.Importance5),
            new Country("Switzerland", "CH", "CHE", "756", CountryImportanceEnum.Importance5),
            new Country("Syrian Arab Republic", "SY", "SYR", "760", CountryImportanceEnum.Importance0),
            new Country("Taiwan, Province of China", "TW", "TWN", "158", CountryImportanceEnum.Importance5),
            new Country("Tajikistan", "TJ", "TJK", "762", CountryImportanceEnum.Importance0),
            new Country("Tanzania, United Republic of", "TZ", "TZA", "834", CountryImportanceEnum.Importance0),
            new Country("Thailand", "TH", "THA", "764", CountryImportanceEnum.Importance5),
            new Country("Timor-Leste", "TL", "TLS", "626", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Togo", "TG", "TGO", "768", CountryImportanceEnum.Importance0),
            new Country("Tokelau", "TK", "TKL", "772", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Tonga", "TO", "TON", "776", CountryImportanceEnum.Importance0),
            new Country("Trinidad and Tobago", "TT", "TTO", "780", CountryImportanceEnum.Importance0),
            new Country("Tunisia", "TN", "TUN", "788", CountryImportanceEnum.Importance0),
            new Country("Turkey", "TR", "TUR", "792", CountryImportanceEnum.Importance5),
            new Country("Turkmenistan", "TM", "TKM", "795", CountryImportanceEnum.Importance1),
            new Country("Turks and Caicos Islands", "TC", "TCA", "796", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Tuvalu", "TV", "TUV", "798", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Uganda", "UG", "UGA", "800", CountryImportanceEnum.Importance0),
            new Country("Ukraine", "UA", "UKR", "804", CountryImportanceEnum.Importance5),
            new Country("United Arab Emirates", "AE", "ARE", "784", CountryImportanceEnum.Importance5),
            new Country("United Kingdom", "GB", "GBR", "826", CountryImportanceEnum.Importance9),
            new Country("United States", "US", "USA", "840", CountryImportanceEnum.Importance9) { AreaSize = CountrySizeEnum.VeryLarge },
            new Country("United States Minor Outlying Islands", "UM", "UMI", "581", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Uruguay", "UY", "URY", "858", CountryImportanceEnum.Importance1),
            new Country("Uzbekistan", "UZ", "UZB", "860", CountryImportanceEnum.Importance1),
            new Country("Vanuatu", "VU", "VUT", "548", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Venezuela, Bolivarian Republic of", "VE", "VEN", "862", CountryImportanceEnum.Importance1),
            new Country("Viet Nam", "VN", "VNM", "704", CountryImportanceEnum.Importance1),
            new Country("Virgin Islands, British", "VG", "VGB", "092", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Virgin Islands, U.S.", "VI", "VIR", "850", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Wallis and Futuna", "WF", "WLF", "876", CountryImportanceEnum.Importance0) { CouldBeIgnored = true },
            new Country("Western Sahara", "EH", "ESH", "732", CountryImportanceEnum.Importance0),
            new Country("Yemen", "YE", "YEM", "887", CountryImportanceEnum.Importance0),
            new Country("Zambia", "ZM", "ZMB", "894", CountryImportanceEnum.Importance0),
            new Country("Zimbabwe", "ZW", "ZWE", "716", CountryImportanceEnum.Importance0),
        };

        public override string ToString()
        {
            return this.Name;
        }
    }
}