using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.ViewModels.BeersViewModels;
using System;
using BrewMasterWebApp.Models;
using BrewMasterWebApp.Services;
using BrewMasterWebApp.Data;
using BrewMasterWebApp.Utilities;

namespace WHWebApp.Controllers.Web
{
    [Authorize]
    public class BeersController : Controller
    {

        private readonly UserManager<BrewMasterUser> _userManager;
        private ILogger _logger;
        private BeerApiService _beerApiService;
        private IBrewMasterRepository _repository;

        public BeersController(
        UserManager<BrewMasterUser> userManager,
        ILoggerFactory loggerFactory,
        BeerApiService beerApiService,
        IBrewMasterRepository repository)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<BeersController>();
            _beerApiService = beerApiService;
            _repository = repository;
        }

        //
        //GET: /Beers/DeleteBeer?BeerId=
        [HttpGet]
        public IActionResult DeleteBeer(string BeerId, string BeerName)
        {
            var beerId = Extensions.SanitizeString(BeerId);
            var beerName = Extensions.SanitizeString(BeerName);

            var deleteBeerViewModel = new DeleteBeerViewModel()
            {
                BeerId = beerId,
                BeerName = beerName,
                Success = true,
                Message = $"Would you like to delete {beerName} from your list?",
                Beer = new Beer(),
                Deleted = false
            };

            return View(deleteBeerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBeer(DeleteBeerViewModel deleteBeerViewModel)
        {
            var beerId = Extensions.SanitizeString(deleteBeerViewModel.BeerId);
            var userName = User.Identity.Name;
            var beerName = Extensions.SanitizeString(deleteBeerViewModel.BeerName);

            deleteBeerViewModel.Success = _repository.DeleteBeer(beerId, userName);
            deleteBeerViewModel.Deleted = true;
            deleteBeerViewModel.BeerName = beerName;
            deleteBeerViewModel.BeerId = beerId;

            if (deleteBeerViewModel.Success)
            {
                deleteBeerViewModel.Message = $"{beerName} was deleted succesfully from your list.";
            }
            else
            {
                deleteBeerViewModel.Message = $"{beerName} is not in your list or we had trouble deleting it.";
            }

            var user = await GetCurrentUserAsync();
            deleteBeerViewModel.UserFirstName = user.FirstName;

            return View(deleteBeerViewModel);
        }

        //
        //GET: /Beers/AddBeer?BeerId=
        [HttpGet]
        public IActionResult AddBeer(string BeerId, string BeerName)
        {   
            var beerId = Extensions.SanitizeString(BeerId);
            var beerName = Extensions.SanitizeString(BeerName);
            
            var addBeerViewModel = new AddBeerViewModel()
            {
                BeerId = beerId,
                BeerName = beerName,
                Success = true,
                Message = $"Would you like to add {beerName} to your list?",
                Beer = new Beer(),
                Post = false
            };
                        
            return View(addBeerViewModel);
        }

        //
        //POST: /Beers/AddBeer?BeerId=
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBeer(AddBeerViewModel addBeerViewModel)
        {
            var beerId = Extensions.SanitizeString(addBeerViewModel.BeerId);

            var beerApiResult = await _beerApiService.GetBeerInformation(beerId);

            addBeerViewModel.Success = beerApiResult.Success;
            addBeerViewModel.Message = beerApiResult.Message;
            addBeerViewModel.Beer = beerApiResult.Beer;
            addBeerViewModel.Post = true;
            addBeerViewModel.Beer.UserName = User.Identity.Name;
            addBeerViewModel.BeerName = addBeerViewModel.Beer.Name;            
            
            
            if(!beerApiResult.Success)
            {
                return View(addBeerViewModel);
            }            
            var user = await GetCurrentUserAsync();
            addBeerViewModel.UserFirstName = user.FirstName;

            try
            {
                if(_repository.AddBeer(addBeerViewModel.Beer,User.Identity.Name)
                    && _repository.SaveAll())
                {
                    addBeerViewModel.Message = $"{addBeerViewModel.Beer.Name} was succesfully added!";
                    addBeerViewModel.Success = true;                   
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                addBeerViewModel.Success = false;
                addBeerViewModel.Message = $"{addBeerViewModel.Beer.Name} is already in your list or we had trouble adding it.";                
            }

            return View(addBeerViewModel);
        }

        //
        //GET: /Beers/Index
        [HttpGet]
        public async Task<IActionResult> Index(){
            
            //A ApplicationUser user object is created with data from the db
            //this will be the source of the data stored in the user object
            var user = await GetCurrentUserAsync();
            
            //A viewModel is created with data from the user object being
            //pass to a sanitizestring method which retruns that same data
            //sanitized from xss
            var viewModel = new IndexViewModel
            { 
                FirstName = Extensions.SanitizeString(user.FirstName)
            };

            //The viewModel object is pass the the view where the data
            //from the object will be display this is the sink in the data flow
            return View(viewModel);
        }

        //
        //GET: /App/BeerInfo?BeerId=
        //This controller takes in a string parameter call beerid this will be the source
        //of the data flow
        public async Task<IActionResult> BeerInfo(string BeerId)
        {
            //If parameters are empty redirect to beers
            if (string.IsNullOrEmpty(BeerId))
            {
                return RedirectToAction(nameof(Index));
            }
            
            //The beerid string is passed to a method to sanitize it from
            //XSS then it is return sanitized and store in a variable inside
            //the scope of this method so the data goes from the param to the method
            //to the variable
            var beerId = Extensions.SanitizeString(BeerId);

            //This beerId variable is then passed to an api service method
            //the method will return an api result object with data acquired from the
            //api service
            var breweryApiResult = await _beerApiService.GetBeerInformation(beerId);

            //A BeerInfoViewModel obejct is created
            var beerInfoViewModel = new BeerInfoViewModel
            {
                //Data from the breweryApiResult is passed into the viewmodel obejct
                Message = breweryApiResult.Message,
                Success = breweryApiResult.Success,
                Beer = breweryApiResult.Beer
            };

            //the view model obejct is return into the View with the data retrieve
            //by the api service this will be the sink of the data flow becasue the data will
            //display to the user
            return View(beerInfoViewModel);
        }

        //
        //GET: /Beers/Beer
        //Controller methdo with two parameters in the url query beername and pagenum
        //they are used to retrieve the beers that contain the beername string in their name
        //and the page number for the api call
        [HttpGet]
        public async Task<IActionResult> SearchBeer(string beername,string pagenum)
        {
            //the beername parameter is passed to a sanitize method
            //a sanitize string of the parameter is return and stored in the beerName variable
            var beerName = Extensions.SanitizeString(beername);

            //The pagenum param is parse to an int if fail a default pageNumber 1 is set
            int pageNumber;
            if(!int.TryParse(pagenum,out pageNumber))
            {
                pageNumber = 1;                
            }

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (beerName != string.Empty)
            {
                //A SearchBeerViewModel obejct is created data is going to be set from
                //beerName and from the api result
                var beerViewModel = new SearchBeerViewModel()
                {
                    //the beerName string is stored in the SearchName prop 
                    SearchName = beerName,                    
                    Beers = new List<Beer>()
                };

                //This beerName and pageNumber variable are then passed to an api service method
                //the method will return an api result object with data acquired from the
                //api service
                var beerApiResult = await _beerApiService.GetBeers(beerName,pageNumber);

                //the viewModel's are set with the data from the beerApiResult
                //data has been sanitize in the service so the data flow is safe
                //from one object's prop to another
                beerViewModel.Message = beerApiResult.Message;
                beerViewModel.PageNumber = beerApiResult.PageNumber;
                beerViewModel.Success = beerApiResult.Success;
                beerViewModel.MaxPageNumber = beerApiResult.MaxPageNumber;
                
                if (beerApiResult.Success)
                {
                    //All the beers in the list in the beerApiResult.Beers list
                    //are pass to the viewModel's beer list
                    foreach (var beer in beerApiResult.Beers)
                    {
                        //the beer object is added to the Beers list on the viewModel
                        beerViewModel.Beers.Add(beer);
                    }
                }
                
                //the beerViewModel is pass to the view so this will be the sink of the data flow
                //this data wil be display to the user once in the view
                return View(beerViewModel);
            }

            return RedirectToAction(nameof(Index));
        }      

        //
        //GET: /App/SarchBrewery
        //This controller has to parameters in the url query
        //BreweryName and the PageNum this are use in the api service call to retrieve data
        public async Task<IActionResult> SearchBrewery(string BreweryName, string PageNum)
        {
            //If parameters are empty redirect to beers
            if (string.IsNullOrEmpty(BreweryName) || string.IsNullOrEmpty(PageNum))
            {
                return RedirectToAction(nameof(Index));
            }

            //the BreweryName parameter is passed to a methed as a paremter to sanitize
            //the string then that sanitize string is return and stored in the breweryName variable
            var breweryName = Extensions.SanitizeString(BreweryName);

            //the PageNum param is parse into an int if it fails then it sets the pageNumber
            //variable to 1
            int pageNumber;
            if (!int.TryParse(PageNum, out pageNumber))
            {
                pageNumber = 1;
            }

            if(pageNumber < 1)
            {
                pageNumber = 1;
            }

            //the breweryName and pageNumber variables are used to retrieve BeerApiServiceResult object
            //this object contains data retrieve by an api request call and it is going to be used
            //in the brewerySearchModel viewModel
            var breweryApiResult = await _beerApiService.GetBreweries(breweryName, pageNumber);

            //A new SearchBreweryViewModel is created with data from the breweryApiResult
            var brewerySearchModel = new SearchBreweryViewModel()
            {
                //the data from the breweryName is passed to the SearchName prop of the viewModel
                SearchName = breweryName,

                //all the data from the breweryApiResult is pass to the props of the viewmodel
                MaxPageNumber = breweryApiResult.MaxPageNumber,
                Message = breweryApiResult.Message,
                PageNumber = breweryApiResult.PageNumber,
                Success = breweryApiResult.Success,
                Breweries = new List<Brewery>()
            };

            if (breweryApiResult.Success)
            {
                foreach(var brewery in breweryApiResult.Breweries)
                {
                    //The breweries from the beerApiResult are added to the breweries list of the viewmodel
                    brewerySearchModel.Breweries.Add(brewery);
                }
            }

            //the viewModel with the retreived data is passed to the View the data will be display to the user
            //this is the sink of the data flow
            return View(brewerySearchModel);
        }

         //
        //GET: /App/Brewery?BreweryId=

        //This controller takes a parameter from the url query this breweryId is use to retrieve data
        public async Task<IActionResult> Brewery(string BreweryId)
        {
            //If parameters are empty redirect to beers
            if (string.IsNullOrEmpty(BreweryId))
            {
                return RedirectToAction(nameof(Index));
            }

            //the BreweryId parameter is pass to a method that sanitizes the string
            //the method returns a sanitize string of the passed parameter and it is stored in breweryId
            var breweryId = Extensions.SanitizeString(BreweryId);

            //the breweryId variable are used to retrieve BeerApiServiceResult object
            //this object contains data retrieve by an api request call and it is going to be used
            //in the breweryViewModel
            var breweryApiResult = await _beerApiService.GetBeersFromBrewery(breweryId);

            //a new BreweryViewModel object is created with data from BeerApiServiceResult object
            var breweryViewModel = new BreweryViewModel
            {
                Brewery = new Brewery(),
                Beers = new List<Beer>(),
                Message = breweryApiResult.Message,
                Success = breweryApiResult.Success
            };

            if (breweryApiResult.Success)
            {
                //the Brewery object data from the BeerApiServiceResult object
                //is passed to the breweryViewModel Brewery data
                breweryViewModel.Brewery = breweryApiResult.Brewery;

                foreach(var beer in breweryApiResult.Beers)
                {
                    //All the beers in the BeerApiServiceResult Beers list are
                    //added to the viewModel Beers list so the data is passed trough
                    breweryViewModel.Beers.Add(beer);
                }
            }

            //the viewModel is passed to the view with all the data retrieved from
            //the api so this will be the sink of the data flow because in the view
            //the data is going to be display to the user
            return View(breweryViewModel);
        }

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

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<BrewMasterUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}