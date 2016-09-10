using Microsoft.AspNetCore.Mvc;

namespace BrewMasterWebApp.Controllers.Web
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
   
        public IActionResult Error()
        {
            return View();
        }
        
    }
}
