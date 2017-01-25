using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// these are the web api models

namespace Awpbs
{
    public class RegisterWebModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string FacebookId { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public int Gender { get; set; }
        public string FacebookAccessToken { get; set; }
    }

    public class RegisterWebModelFVO
    {
        public string Email { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public int VenueID { get; set; }
    }

    public class VerifyPinWebModel
    {
        public int AthleteID { get; set; }
        public string Pin { get; set; }
    }

    public class ChangePinWebModel
    {
        public string Pin { get; set; }
    }

    public class ChangePinResultWebModel
    {
        public bool PinChanged { get; set; }
        public bool PasswordChanged { get; set; }
    }
}
