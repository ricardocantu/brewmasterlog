using BrewMasterWebApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewMasterWebApp.ViewModels.LocationsViewModels
{
    public class SearchLocationViewModel
    {
        [Required]
        public string SearchState { get; set; }

        public string SearchCity { get; set; }

        public List<Location> Locations { get; set; }

        public string Message { get; set; }

        public int PageNumber { get; set; }

        public int MaxPageNumber { get; set; }

        public bool Success { get; set; }
    }
}