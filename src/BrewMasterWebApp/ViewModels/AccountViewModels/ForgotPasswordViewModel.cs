using System.ComponentModel.DataAnnotations;

namespace BrewMasterWebApp.ViewModels.AccountViewModels
{    
        public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    
    }
}