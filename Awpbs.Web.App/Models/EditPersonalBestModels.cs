//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Mvc;

//namespace Awpbs.Web.App.Models
//{
//    public class EditPersonalBestViewModel
//    {
//        [Required]
//        public string ResultType { get; set; }
//        public string Time { get; set; }

//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
//        public DateTime? Date { get; set; }
//        public string Notes { get; set; }

//        public string Sport { get; set; }

//        public ICollection<SelectListItem> AllResultTypes { get; set; }
//    }
//}
