using BrewMasterWebApp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BrewMasterWebApp.ViewModels.BeersViewModels
{
    public class SearchBeerViewModel
    {
        [Required]
        public string SearchName { get; set; }

        public List<Beer> Beers { get; set; }

        public string Message { get; set; }

        public int PageNumber { get; set; }

        public int MaxPageNumber { get; set; }

        public bool Success { get; set; }
    }
}
