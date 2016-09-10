using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.ViewModels.BeersViewModels
{
    public class IndexViewModel
    {
        public IEnumerator<Beer> Beers { get; set; }

        public string FirstName { get; set; }

        [Required(ErrorMessage="Beer name is required")]
        [Display(Name = "Beer Name")]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string BeerName { get; set; }

        [Required(ErrorMessage="Brewery name is required")]
        [Display(Name = "Brewery Name")]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string BreweryName { get; set; }

    }
}