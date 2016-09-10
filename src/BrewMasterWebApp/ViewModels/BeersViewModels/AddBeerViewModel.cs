using System.ComponentModel.DataAnnotations;
using BrewMasterWebApp.Models;

namespace BrewMasterWebApp.ViewModels.BeersViewModels
{
    public class AddBeerViewModel
    {
        public Beer Beer { get; set; }
        
        public bool Success { get; set; }

        public string Message { get; set; }
        
        public string BeerName { get; set; }
       
       [Required]
        public string BeerId { get; set; }

        public bool Post { get; set; }

        public string UserFirstName { get; set; }
    }
}