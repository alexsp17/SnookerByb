using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Awpbs.Web
{
    /// <summary>
    /// This service "Sends emails"
    /// You can create multiple instances of this if needed
    /// </summary>
    public class EmailService
    {
        public async Task SendEmailToAthlete(Athlete athlete, string subject, string body)
        {
            string emailAddressOfAthlete = athlete.UserName;
            if (string.IsNullOrEmpty(athlete.RealEmail) == false)
                emailAddressOfAthlete = athlete.RealEmail;

            await SendEmail(emailAddressOfAthlete, subject, body);
        }

        public async Task SendEmail(string emailAddress, string subject, string body)
        {
            SendGrid.SendGridMessage message = new SendGrid.SendGridMessage();
            message.From = new System.Net.Mail.MailAddress("donotreply@snookerbyb.com");
            message.To = new System.Net.Mail.MailAddress[]
            {
                new System.Net.Mail.MailAddress(emailAddress)
            };
            message.Subject = subject;
            message.Html = body;
            message.DisableClickTracking();
            message.DisableOpenTracking();

            string userName = System.Web.Configuration.WebConfigurationManager.AppSettings["SendGridUserName"];
            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["SendGridPassword"];
            var credentials = new NetworkCredential(userName, password);
            await new SendGrid.Web(credentials).DeliverAsync(message);
        }
    }
}
