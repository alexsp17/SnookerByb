//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Awpbs.Web.App.Models
//{
//    public class ResultsViewModel
//    {
//        public SportEnum Sport { get; set; }
//        public string SportName { get { return Sport.ToString(); } }
//        public string Type { get; set; }
//        public string Time { get; set; }
//        public DateTime? Date { get; set; }

//        [StringLength(200)]
//        public string Notes { get; set; }
//        public string ErrorTextForTime { get; set; }
//        public string ErrorTextForDate { get; set; }


//        public ResultType ActiveType { get; set; }
//        public string ExampleResult { get; set; }
//        public Athlete Athlete { get; set; }

//        public List<ResultType> Types { get; set; }
//        public List<int> TypesCounts { get; set; }

//        public List<SingleResultViewModel> ResultsByYou { get; set; }
//        public List<SingleResultViewModel> ResultsByPros { get; set; }
//        public List<SingleResultViewModel> ResultsByOthers { get; set; }
//    }

//    public class SingleResultViewModel
//    {
//        public Athlete Athlete { get; set; }
//        public Result Result { get; set; }

//        public string Notes
//        {
//            get
//            {
//                if (Athlete == null)
//                {
//                    if (Result != null)
//                        return Result.Notes;
//                    return "";
//                }
//                if (Athlete.IsPro == false)
//                {
//                    return Result.Notes;
//                }

//                string text = "";
//                //if (Result.WorldRecord == (int)WorldRecordEnum.WorldRecord)
//                //    text = "World record";
//                //if (Result.WorldRecord == (int)WorldRecordEnum.FormerWorldRecord)
//                //    text = "Former world record";
//                if (text.Length > 0)
//                    if (!(AgeGroup == AgeGroupEnum.FOpen || AgeGroup == AgeGroupEnum.MOpen))
//                        text += " (" + AgeGroupDisplay + ")";
//                return text;
//            }
//        }

//        public string AthleteNameDisplay
//        {
//            get
//            {
//                if (Athlete == null)
//                    return "You";
//                string athleteName = Athlete.Name;
//                if (Athlete.IsPro && string.IsNullOrEmpty(Athlete.Country) == false)
//                    athleteName += " (" + Athlete.Country + ")";
//                return athleteName;
//            }
//        }

//        public AgeGroupEnum AgeGroup
//        {
//            get
//            {
//                DateTime date = Result.Date ?? DateTime.Now;
//                DateTime? dob = null;
//                GenderEnum gender = GenderEnum.Unknown;
//                if (Athlete != null)
//                {
//                    dob = Athlete.DOB;
//                    gender = (GenderEnum)Athlete.Gender;
//                }

//                return new AgeCalculator().CalcAgeGroup(date, dob, gender);
//            }
//        }

//        public string GenderDisplay
//        {
//            get
//            {
//                var gender = Athlete != null ? (GenderEnum)Athlete.Gender : GenderEnum.Unknown;
//                return gender.ToString();
//            }
//        }

//        public string AgeGroupDisplay
//        {
//            get
//            {
//                return AgeGroup.ToString();
//            }
//        }

//        public string DateDisplay
//        {
//            get
//            {
//                if (Result.Date == null)
//                    return "-";
//                return Result.Date.Value.ToString("d");
//            }
//        }

//        public string ResultDisplay
//        {
//            get
//            {
//                if (Result.Time != null)
//                {
//                    return new TimeHelper().TimeToString(Result.Time.Value);
//                }
//                else if (Result.Count != null)
//                {
//                    string str = Result.ResultType.CountName + ": " + Result.Count.Value.ToString();
//                    if (Result.Count2 != null)
//                        str += ", " + Result.ResultType.Count2Name + ": " + Result.Count2.Value.ToString();
//                    return str;
                    
//                }
//                return "?";
//            }
//        }
//    }
//}
