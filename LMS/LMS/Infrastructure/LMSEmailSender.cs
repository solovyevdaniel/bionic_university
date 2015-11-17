using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Threading.Tasks;
using LMS.Models;
namespace LMS.Infrastructure
{
    public class LMSEmailSender
    {
        private String from = "lms.confirmation@gmail.com";
        private String pass = "Bionic2015";
        private SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
        public static String AppliedMessage(String userName, String courseName)
        {
            return String.Format("Hello!! We are glad to say that your candidature is allowed to apply to course {0}. Thank you for your joining!\n Best regards, LMS Easy Study Team", courseName);
        }
        public static String DisappliedMessage(String userName, String courseName)
        {
            return String.Format("Hello!! Sorry, your candidature is not allowed to apply to course {0}. We hope that you will join other courses!\n Best regards, LMS Easy Study Team", courseName);
        }
        public async void WriteEmailsAsync(IEnumerable<String> usersMails, ApplicationDbContext appContext, String subj, String message)
        {
            if (usersMails != null && usersMails.Count() > 0)
            {
                foreach (var uMail in usersMails)
                {
                    MailMessage mailMessage = CreateMessage(subj, message, uMail);
                    if (mailMessage != null)
                        await client.SendMailAsync(mailMessage);
                }
            }
        }
        private MailMessage CreateMessage(String subject, String message, String to)
        {
            if (subject != null && message != null)
            {
                MailMessage mailMessage = new MailMessage(from, to, subject, message);
                return mailMessage;
            }
            return null;
        }
            // адрес и порт smtp-сервера, с которого мы и будем отправлять письмо
       // private SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

            /*client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(from, pass);
            client.EnableSsl = true;*/

            // создаем письмо: message.Destination - адрес получателя
            /*var mail = new MailMessage(from, message.Destination);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            return client.SendMailAsync(mail);*/
    }
}