using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.ViewModels.LocationsViewModels
{
    public class LocationInfoViewModel
    {
        public Location Location { get; set; }

        public string GoogleMapsSrc { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }
    }
}