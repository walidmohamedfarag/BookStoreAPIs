
using System.Net;
using System.Net.Mail;

namespace BookStoreAPIs.Utility
{
    public class Emailsender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("wm1336562@gmail.com", "yowp jjzw itwv cqpl")
            };
            return client.SendMailAsync(new MailMessage(from: "wm1336562@gmail.com" , to: email , subject: subject , htmlMessage) { IsBodyHtml = true  });
        }
    }
}
