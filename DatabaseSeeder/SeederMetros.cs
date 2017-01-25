using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    public class SeederMetros
    {
        private readonly ApplicationDbContext db;

        public SeederMetros(ApplicationDbContext db)
        {
            this.db = db;
        }

        public List<Metro> LoadMetrosFromFile(System.IO.Stream file)
        {
            var reader = new System.IO.StreamReader(file);

            // first line is the header
            reader.ReadLine();

            // load venues line by line
            int lineNumber = 0;
            List<Metro> metros = new List<Metro>();
            for (; ; )
            {
                lineNumber++;

                var line = reader.ReadLine();
                if (line == null)
                    break;

                string[] strs = line.Split('\t');
                if (strs == null || strs.Length == 1)
                    strs = line.Split(',');
                if (strs.Length < 5)
                {
                    throw new Exception("Incorrect format. lineNumber=" + lineNumber);
                }

                int id = 0;
                int.TryParse(strs[0], out id);

                string name = strs[1].Trim();
                if (name.StartsWith("\"") && name.EndsWith("\""))
                    name = name.Substring(1, name.Length - 2).Trim();

                Country country = Country.Get(strs[2]);

                metros.Add(new Metro()
                {
                    MetroID = id,
                    Name = name,
                    Country = country.ThreeLetterCode,
                    Latitude = double.Parse(strs[3]),
                    Longitude = double.Parse(strs[4])
                });
            }

            if (metros.Count < 1)
                throw new Exception("No metros in the file.");
            return metros;
        }

        public void SaveMetrosToFile(List<Metro> metros, System.IO.Stream file)
        {
            var writer = new System.IO.StreamWriter(file);

            writer.WriteLine("ID\tName\tCountry\tLatitude\tLongitude");

            foreach (var metro in metros)
            {
                string line = metro.MetroID.ToString() + "\t" + metro.Name + "\t" + metro.Country + "\t" + metro.Latitude.ToString() + "\t" + metro.Longitude;
                writer.WriteLine(line);
                writer.Flush();
            }
        }

        public void PopulateMetrosIntoTheDatabase(List<Metro> metros, out int countNew, out int countModified, out int countError)
        {
            countNew = 0;
            countModified = 0;
            countError = 0;

            foreach (var metro in metros)
            {
                try
                {
                    Metro metroInDb = null;
                    if (metro.MetroID != 0)
                        metroInDb = db.Metros.Where(i => i.MetroID == metro.MetroID).FirstOrDefault();
                    if (metroInDb == null)
                        metroInDb = db.Metros.Where(i => i.Name == metro.Name && i.Latitude == metro.Latitude && i.Longitude == metro.Longitude).FirstOrDefault();

                    if (metroInDb == null)
                    {
                        metroInDb = new Metro()
                        {
                            Name = metro.Name,
                            Country = metro.Country,
                            Latitude = metro.Latitude,
                            Longitude = metro.Longitude,
                        };
                        db.Metros.Add(metroInDb);
                        db.SaveChanges();
                        countNew++;
                    }
                    else if (metroInDb.Name != metro.Name ||
                            metroInDb.Country != metro.Country ||
                            metroInDb.Latitude != metro.Latitude ||
                            metroInDb.Longitude != metro.Longitude)
                    {
                        metroInDb.Name = metro.Name;
                        metroInDb.Country = metro.Country;
                        metroInDb.Latitude = metro.Latitude;
                        metroInDb.Longitude = metro.Longitude;
                        db.SaveChanges();
                        countModified++;
                    }

                    metro.MetroID = metroInDb.MetroID;
                }
                catch (Exception)
                {
                    countError++;
                }
            }
        }
    }
}
