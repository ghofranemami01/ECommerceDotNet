using System.Threading.Tasks;

namespace WebApplication2.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
    }
}