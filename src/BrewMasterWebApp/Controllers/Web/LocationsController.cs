using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.Models;
using BrewMasterWebApp.Services;
using BrewMasterWebApp.ViewModels.LocationsViewModels;
using BrewMasterWebApp.Utilities;
using BrewMasterWebApp.Data;
using System.Collections.Generic;
using System;

namespace BrewMasterWebApp.Controllers.Web
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly UserManager<BrewMasterUser> _userManager;
        private ILogger _logger;
        private BeerApiService _beerApiService;
        private IBrewMasterRepository _repository;
        private MapsService _mapService;

        public LocationsController (UserManager<BrewMasterUser> userManager,
        ILoggerFactory loggerFactory,
        BeerApiService beerApiService,
        IBrewMasterRepository repository,
        MapsService mapService)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<LocationsController>();
            _beerApiService = beerApiService;
            _repository = repository;
            _mapService = mapService;
        }

        //
        //GET: /Locations/Index
        [HttpGet]
        public async Task<IActionResult> Index(){            
            
            var user = await GetCurrentUserAsync();

            var viewModel = new IndexViewModel
            { 
                FirstName = Extensions.SanitizeString(user.FirstName)
            };
            
            return View(viewModel);
        }

        //
        //GET: /Locations/LocationInfo?LocationId=
        [HttpGet]
        public async Task<IActionResult> LocationInfo(string LocationId)
        {
            //If parameters are empty redirect to beers
            if (string.IsNullOrEmpty(LocationId))
            {
                return RedirectToAction(nameof(Index));
            }

            var locationId = Extensions.SanitizeString(LocationId);
            
            var breweryApiResult = await _beerApiService.GetLocation(locationId);

            var locationInfoViewModel = new LocationInfoViewModel
            {                
                Message = breweryApiResult.Message,
                Success = breweryApiResult.Success,
                Location = breweryApiResult.Location
            };
            var location = locationInfoViewModel.Location;
            locationInfoViewModel.GoogleMapsSrc = _mapService
                .GetMapSrc(location.Brewery.Name,location.StreetAddress,
                        location.City,location.State);
            
            return View(locationInfoViewModel);
        }

        //
        //GET: /Locations/AddLocation?LocationId=&BreweryName=
        [HttpGet]
        public IActionResult AddLocation(string LocationId, string BreweryName)
        {   
            var locationId = Extensions.SanitizeString(LocationId);
            var breweryName = Extensions.SanitizeString(BreweryName);
            
            var addLocationViewModel = new AddLocationViewModel()
            {
                LocationId = locationId,
                BreweryName = breweryName,
                Success = true,
                Message = $"Would you like to add {breweryName} to your locations?",
                Location = new Location(),
                Post = false
            };

            return View(addLocationViewModel);
        }

        //
        //POST: /Locations/AddLocation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(AddLocationViewModel addLocationViewModel)
        {
            var locationId = Extensions.SanitizeString(addLocationViewModel.LocationId);

            var beerApiResult = await _beerApiService.GetLocation(locationId);

            addLocationViewModel.Success = beerApiResult.Success;
            addLocationViewModel.Message = beerApiResult.Message;
            addLocationViewModel.Location = beerApiResult.Location;
            addLocationViewModel.Post = true;
            addLocationViewModel.Location.UserName = User.Identity.Name;
            addLocationViewModel.BreweryName = addLocationViewModel.Location.Brewery.Name;            
                        
            if(!beerApiResult.Success)
            {
                return View(addLocationViewModel);
            }            
            var user = await GetCurrentUserAsync();
            addLocationViewModel.UserFirstName = user.FirstName;

            try
            {
                if(_repository.AddLocation(addLocationViewModel.Location,User.Identity.Name)
                    && _repository.SaveAll())
                {
                    addLocationViewModel.Message = $"{addLocationViewModel.BreweryName} was succesfully added!";
                    addLocationViewModel.Success = true;                   
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                addLocationViewModel.Success = false;
                addLocationViewModel.Message = $"{addLocationViewModel.BreweryName} is already in your list or we had trouble adding it.";                
            }

            return View(addLocationViewModel);
        }

         //
        //GET: /Beers/DeleteBeer?BeerId=
        [HttpGet]
        public IActionResult DeleteLocation(string LocationId, string BreweryName)
        {
            var locationId = Extensions.SanitizeString(LocationId);
            var breweryName = Extensions.SanitizeString(BreweryName);

            var deleteLocationViewModel = new DeleteLocationViewModel()
            {
                LocationId = locationId,
                BreweryName = breweryName,
                Success = true,
                Message = $"Would you like to delete {breweryName} from your list?",
                Location = new Location(),
                Deleted = false
            };

            return View(deleteLocationViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLocation(DeleteLocationViewModel deleteLocationViewModel)
        {
            var locationId = Extensions.SanitizeString(deleteLocationViewModel.LocationId);
            var userName = User.Identity.Name;
            var breweryName = Extensions.SanitizeString(deleteLocationViewModel.BreweryName);

            deleteLocationViewModel.Success = _repository.DeleteLocation(locationId, userName);
            deleteLocationViewModel.Deleted = true;
            deleteLocationViewModel.BreweryName = breweryName;
            deleteLocationViewModel.LocationId = locationId;

            if (deleteLocationViewModel.Success)
            {
                deleteLocationViewModel.Message = $"{breweryName} was deleted succesfully from your list.";
            }
            else
            {
                deleteLocationViewModel.Message = $"{breweryName} is not in your list or we had trouble deleting it.";
            }

            var user = await GetCurrentUserAsync();
            deleteLocationViewModel.UserFirstName = user.FirstName;

            return View(deleteLocationViewModel);
        }

        //
        //GET: /Locations/SearchLocation
        [HttpGet]
        public async Task<IActionResult> SearchLocation(string state, string city, string pagenum)
        {            
            var region = Extensions.SanitizeString(state);
            var locality = Extensions.SanitizeString(city);
            
            int pageNumber;
            if(!int.TryParse(pagenum,out pageNumber))
            {
                pageNumber = 1;                
            }

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (region != string.Empty)
            {                
                var locationsViewModel = new SearchLocationViewModel()
                {
                    SearchState = region,
                    SearchCity = locality,
                    Locations = new List<Location>()
                };
                
                var beerApiResult = await _beerApiService.GetLocations(region,locality,pageNumber);
                
                locationsViewModel.Message = beerApiResult.Message;
                locationsViewModel.PageNumber = beerApiResult.PageNumber;
                locationsViewModel.Success = beerApiResult.Success;
                locationsViewModel.MaxPageNumber = beerApiResult.MaxPageNumber;
                
                if (beerApiResult.Success)
                {                    
                    foreach (var location in beerApiResult.Locations)
                    {                       
                        locationsViewModel.Locations.Add(location);
                    }
                }               
                
                return View(locationsViewModel);
            }

            return RedirectToAction(nameof(Index));
        } 

        //
        //GET: /Beers/Index
        [HttpGet]

        #region Helpers

        //This method adds errors to the ModelState the data from the IdentityResult param
        //is passed to the ModelState Errors        
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<BrewMasterUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}