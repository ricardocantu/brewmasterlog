using BrewMasterWebApp.Models;
using System.Collections.Generic;

namespace BrewMasterWebApp.ViewModels.BeersViewModels
{
    public class BreweryViewModel
    {
        public Brewery Brewery { get; set; }

        public List<Beer> Beers { get; set; }

        public string Message { get; set; }

        public bool Success { get; set; }
    }
}
