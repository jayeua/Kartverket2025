using System.Threading.Tasks;

namespace kartverket2025.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}