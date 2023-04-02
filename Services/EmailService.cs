using GuestSystemBack.Interfaces;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace GuestSystemBack.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string recipientAddress, string emailSubject, string emailBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("guestservice@something.com"));
            email.To.Add(MailboxAddress.Parse(recipientAddress));
            email.Subject = emailSubject;
            email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect(_configuration.GetSection("AppSettings:EmailHost").Value,
                int.Parse(_configuration.GetSection("AppSettings:EmailPort").Value),
                MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("AppSettings:EmailUsername").Value,
                _configuration.GetSection("AppSettings:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
