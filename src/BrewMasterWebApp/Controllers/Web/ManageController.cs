using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.Models;
using BrewMasterWebApp.ViewModels.ManageViewModels;
using BrewMasterWebApp.Utilities;

namespace BrewMasterWebApp.Controllers.Web
{

    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<BrewMasterUser> _userManager;
        private readonly SignInManager<BrewMasterUser> _signInManager;
        private readonly ILogger _logger;

        public ManageController(
        UserManager<BrewMasterUser> userManager,
        SignInManager<BrewMasterUser> signInManager,
        ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {            
            //An ApplicationUser model is instantitiated data is
            //retrieve from a db so this will be the start of the dataflow
            //this will be the source
            var user = await GetCurrentUserAsync();
            
            //An IndexViewModel model is instantitiated with data from the user object
            //The data is sanitize before being put to the model
            //the data flow will be from the source to the model object
            var viewModel = new IndexViewModel
            {                
                PhoneNumber = Extensions.SanitizeString(await _userManager.GetPhoneNumberAsync(user)),
                Email = Extensions.SanitizeString(await _userManager.GetEmailAsync(user)),
                FirstName = Extensions.SanitizeString(user.FirstName),
                LastName = Extensions.SanitizeString(user.LastName)
            };

            //the model object is passed to the view
            //this will be the sink of the data flow
            return View(viewModel);
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        //ChangePasswordViewModel is being pass as a paramater on the controller
        //this paramater will be the source in the data flow
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {            
            //ModelState verifies if the model data annotations rules
            //where passed succesfully this was verified
            //by the jquery-validation and jquery-validation scripts in the view 
            if (!ModelState.IsValid)
            {   
                //the model is being passed to the view this will be the sink in the data flow
                return View(viewModel);
            }

            //A ApplicationUser object name user is created with data from the db 
            //this will be the key to access the db row which will be the sink
            //in the data flow
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                //Here the sink is the db user is the key, the model.oldpassword is data and new password too
                //oldpassword is use to verified if it is in fact that user and new password replaces the password
                //in the db
                var result = await _userManager.ChangePasswordAsync(user, viewModel.OldPassword, viewModel.NewPassword);
                if (result.Succeeded)
                {
                    //this signs in the user with the new password
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);

                //the ChangePasswordViewModel model is return because there was an error
                //or an unsuccesfull attempt to change the password
                //this will be a sink of the data flow
                return View(viewModel);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        //GET: /Manage/EditInformation
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditInformation()
        {            
            //An ApplicationUser user object is created with data
            //from a db this will be the source
            var user = await GetCurrentUserAsync();
            
            //An edit information model is created with data from the user object
            //the data flow goes from the user object to the model object
            var viewModel = new EditInformationViewModel
            {                
                //Data from the user obj is pass to a SanitizeString method as a paramater
                //this will be a propagator then the data is return and stored in the prop
                //of the view model already sanitize
                PhoneNumber = Extensions.SanitizeString(await _userManager.GetPhoneNumberAsync(user)),
                Email = Extensions.SanitizeString(await _userManager.GetEmailAsync(user)),
                FirstName = Extensions.SanitizeString(user.FirstName),
                LastName = Extensions.SanitizeString(user.LastName)
            };

            //This will be the sink of the data flow the viewmodel is passed to the view
            return View(viewModel);
        }

        //
        // POST: /Manage/EditInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        //The EditInformationViewModel viewModel is the source of the data flow this object
        //contains data inputed from a user
        public async Task<IActionResult> EditInformation(EditInformationViewModel viewModel)
        {
            //ModelState verifies if the model data annotations rules
            //where passed succesfully this was verified
            //by the jquery-validation and jquery-validation scripts in the view 
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //ApplicationUser user is retrieve with data from a db
            var user = await GetCurrentUserAsync();

            if (user != null)
            {               
                //Data from the viewModel is passed to a Extensions.SanitizeString method
                //as a parameter data flows from the viewModel to this method this method
                //sanitize the string and returns it and the data is stored in firstName and lastName
                var firstName = Extensions.SanitizeString(viewModel.FirstName);
                var lastName = Extensions.SanitizeString(viewModel.LastName);

                if(firstName != string.Empty && lastName != string.Empty)
                {
                    //The data from the viewModel goes to the same Extensions.SanitizeString
                    //method as before and then the method returns the sanitize data and
                    //sotre on the user object
                    user.UserName = Extensions.SanitizeString(viewModel.Email);
                    user.Email = Extensions.SanitizeString(viewModel.Email);
                    user.PhoneNumber = Extensions.SanitizeString(viewModel.PhoneNumber);
                    user.FirstName = firstName;
                    user.LastName = lastName;

                    //this will be the sink of the data flow the user object
                    //is pass to updateAsync method where it updates the data in the db
                    //it returns an IdentityResult object
                    var updateUserResult = await _userManager.UpdateAsync(user);
               
                    //The IdentityResult object is use to verify if the updated succeeded
                    if (updateUserResult.Succeeded)
                    {
                        //The user object is used to sign in the user with the new data
                        //becasue the email is the username
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(3, "Information changed succesfully.");
                        return RedirectToAction(nameof(Index), new {Message = "Edited information successfully!"});
                    }

                    AddErrors(updateUserResult);
                }
                else{
                    ModelState.AddModelError(string.Empty,"Blocked insecure data!"); 
                }

                //If there was an error the viewModel will be pass to the view to redisplay the data
                return View(viewModel);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        #region Helpers

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
