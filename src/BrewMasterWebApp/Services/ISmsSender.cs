using System.Threading.Tasks;

namespace BrewMasterWebApp.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
