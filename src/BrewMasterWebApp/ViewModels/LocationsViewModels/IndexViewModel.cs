
using System.ComponentModel.DataAnnotations;

namespace BrewMasterWebApp.ViewModels.LocationsViewModels
{   
    public class IndexViewModel
    {        
        public string FirstName { get; set; }
        
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string City { get; set; }

        [Required]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string State { get; set; }

        public string GoogleSrc { get; set; }
        
    }
}