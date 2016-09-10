using System.Threading.Tasks;

namespace BrewMasterWebApp.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
