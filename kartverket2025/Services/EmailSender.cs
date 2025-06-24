using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace kartverket2025.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient(_settings.SmtpServer, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.Username, _settings.Password),
                EnableSsl = true
            })
            using (var message = new MailMessage(_settings.FromEmail, email, subject, htmlMessage)
            {
                IsBodyHtml = true
            })
            {
                await client.SendMailAsync(message);
            }
        }
    }
}