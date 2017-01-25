//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Awpbs.Web.App.Models
//{
//    public class MyProfileViewModel
//    {
//        public string UserName { get; set; }
//        public string Name { get; set; }
//        public int AthleteID { get; set; }
//        public string PictureUrlToUseForUploads { get { return "athlete" + AthleteID; } }

//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        public DateTime? DOB { get; set; }
//        public GenderEnum Gender { get; set; }

//        public string NameOrUserName { get { return string.IsNullOrEmpty(Name) ? UserName : Name;  } }
//        public string GenderDisplay { get { return Gender.ToString(); } }

//        public string FacebookId { get; set; }
//        public string PictureUrl { get; set; }

//        public CloudinaryDotNet.Cloudinary Cloudinary { get; set; }
//    }
//}
