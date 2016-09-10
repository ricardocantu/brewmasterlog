using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.ViewModels.BeersViewModels
{
    public class BeerInfoViewModel
    {
        public Beer Beer { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}
