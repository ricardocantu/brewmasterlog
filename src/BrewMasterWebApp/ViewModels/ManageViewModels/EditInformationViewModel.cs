using System.ComponentModel.DataAnnotations;

namespace BrewMasterWebApp.ViewModels.ManageViewModels
{
    public class EditInformationViewModel
    {
        [Required(ErrorMessage="Email address is required")]
        [EmailAddress(ErrorMessage="Please enter a valid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage="First name is required")]
        [Display(Name = "First Name")]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last name is required")]
        [Display(Name = "Last Name")]
        [StringLength(100,MinimumLength = 3,ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string LastName { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please input a valid phone number format ex. (281) 123-4567")]
        public string PhoneNumber { get; set; }
    }

}