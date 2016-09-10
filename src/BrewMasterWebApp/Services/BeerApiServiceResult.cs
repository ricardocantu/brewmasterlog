using BrewMasterWebApp.Models;
using System.Collections.Generic;

namespace BrewMasterWebApp.Services
{
    public class BeerApiServiceResult
    {
        public bool Success { get; set; }

        public List<Beer> Beers { get; set; }

        public List<Brewery> Breweries { get; set; }

        public List<Location> Locations { get; set; }

        public Location Location { get; set; }

        public Brewery Brewery { get; set; }

        public Beer Beer { get; set; }

        public string Message { get; set; }

        public int PageNumber { get; set; }

        public int MaxPageNumber { get; set; }
    }
}
