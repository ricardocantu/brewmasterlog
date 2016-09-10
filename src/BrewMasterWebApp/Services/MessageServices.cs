using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BrewMasterWebApp.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private ILogger _logger; 

        public AuthMessageSender (ILoggerFactory loggerFactory)
        {
          _logger = loggerFactory.CreateLogger<AuthMessageSender>();
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            var emailMessage = new SendGrid.SendGridMessage();
            var to = new MailAddress(email);
            emailMessage.AddTo(to.Address);
            emailMessage.From = new MailAddress("no-reply@brewmasterlog.com");
            emailMessage.Subject = subject;
            emailMessage.Text = message;
            emailMessage.Html = message;

            var user = Startup.Configuration["SendGridUser"];
            var key = Startup.Configuration["SendGridKey"];
            var credentials = new NetworkCredential(user,key);

            var transportWeb = new SendGrid.Web(credentials);

            if(transportWeb != null)
            {
                return transportWeb.DeliverAsync(emailMessage);
            }
            else
            {            
                _logger.LogError(56,"Failed to send email using SendGrid");
                return Task.FromResult(0);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
