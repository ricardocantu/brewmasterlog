using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Security.Application;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.Utilities
{
    public class Extensions
    {
        public static string IsActive(ViewContext viewContext, string controller, string action)
        {
            var actionRouteData = viewContext.RouteData.Values["action"].ToString();
            var controllerRouteData = viewContext.RouteData.Values["controller"].ToString();
            return ((actionRouteData == action) && (controllerRouteData == controller)) ? "active" : "";
        }

        public static string IsActive(ViewContext viewContext, string controller)
        {
            var controllerRouteData = viewContext.RouteData.Values["controller"].ToString();
            return (controllerRouteData == controller) ? "active" : "";
        }

        public static string IsActiveDropdown(ViewContext viewContext, string controller, string[] actions)
        {
            var actionRouteData = viewContext.RouteData.Values["action"].ToString();
            var controllerRouteData = viewContext.RouteData.Values["controller"].ToString();

            var activeAction = false;

            foreach (var action in actions)
            {
                if (actionRouteData == action)
                {
                    activeAction = true;
                    break;
                }
            }

            return (activeAction && (controllerRouteData == controller)) ? "active" : "";
        }
        
        public static string SanitizeString(string strToSanitize)
        {
            //return Microsoft.Security.Application.Encoder.HtmlEncode(strToSanitize, true);
            if (!string.IsNullOrEmpty(strToSanitize))
            {
                var sanitizeString = strToSanitize.Trim();

                return sanitizeString;
            }
           
            return strToSanitize;
        }

        public static async Task<string> GetUserFullName(string userName, UserManager<BrewMasterUser> manager)
        {
            try
            {
                var user = await manager.FindByNameAsync(userName);

                var fullName = user.FirstName + " " + user.LastName;

                return fullName;
            }
            catch
            {
                return userName;
            }
        }

        public static async Task<string> GetUserFirstName(string userName, UserManager<BrewMasterUser> manager)
        {
            try
            {
                var user = await manager.FindByNameAsync(userName);
                var firstName = user.FirstName;
                return firstName;
            }
            catch
            {
                return userName;
            }
        }

        public static string LocationSearch(string city, string state)
        {
            city = SanitizeString(city);

            city = string.IsNullOrEmpty(city) ? "" : city + ", ";

            return city + SanitizeString(state);
        }
    }
}
