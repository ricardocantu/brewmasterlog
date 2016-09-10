using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrewMasterWebApp.Models;
using BrewMasterWebApp.ViewModels.AccountViewModels;
using BrewMasterWebApp.Utilities;
using BrewMasterWebApp.Services;

namespace BrewMasterWebApp.Controllers.Web
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<BrewMasterUser> _userManager;
        private readonly SignInManager<BrewMasterUser> _signInManager;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<BrewMasterUser> userManager,
            SignInManager<BrewMasterUser> signInManager,
            ILoggerFactory loggerFactory,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _emailSender = emailSender;       
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string alertId)
        {
            int alertNum;

            var hasValidAlert = int.TryParse(alertId, out alertNum);

            if (hasValidAlert)
            {
                if (alertNum == 1)
                {
                    ViewBag.ConfirmEmailAlert = "Please confirm your email address in order to log in!";
                }
            }

            return View();
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //LoginViewModel model is the source of the data flow
        public async Task<IActionResult> Login(LoginViewModel model)
        {                        
            if (ModelState.IsValid)
            {

                //Required to confirm email
                var user = await _userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        ModelState.AddModelError(string.Empty, "You must have a confirmed email to log in.");
                        return View(model);
                    }
                }

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                //LoginViewModel model data is being used to sign in this method
                //returns a SignInresult object 
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");
                    return RedirectToAction("Index","Home");
                }
                
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    //The model is being return so the data inputed by the user
                    //may be redisplay this will be the sink in the data flow
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            //The model is being return so the data inputed by the user
            //may be redisplay this will be the sink in the data flow
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {            
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //DataFlow Source: The the model being passed as a parameter in this post action
        //contains different properties with data inputed by a user in a form from 
        //the View of the action GET: /Account/Register
        //RegisterViewModel model is a source
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            //ModelState verifies if the model data annotations rules
            //where passed succesfully this was verified
            //by the jquery-validation and jquery-validation scripts in the view            
            if (ModelState.IsValid)
            {
                //the model's FirstName and LastName props data is being passed to
                //A private method to sanitize XSS the data flows from the model prop
                //to the method
                //data is being return and the firstName and lastName variables store
                //this sanitize data now
                var firstName = Extensions.SanitizeString(model.FirstName);
                var lastName = Extensions.SanitizeString(model.LastName);
                
                if(firstName != string.Empty && lastName != string.Empty){

                    var user = new BrewMasterUser 
                    { 
                        //Data from the model is being passed to the SanitizeString method
                        //as a paremeter then data is being return and stored in a
                        //ApplicationUser model props
                        UserName = Extensions.SanitizeString(model.Email), 
                        Email = Extensions.SanitizeString(model.Email),
                        PhoneNumber = Extensions.SanitizeString(model.PhoneNumber),
                        FirstName = firstName,
                        LastName = lastName                        
                    };
                    //The ApplicationUser model then is being passed to a methdo to store
                    //its data in a db as well as the RegisterViewModel model password
                    //this method retruns a IdentityResult obj
                    var result = await _userManager.CreateAsync(user, model.Password);

                    //the IdentityResult obj has a bool prop Succeeded which is
                    //use to verifye if the data was stored correctly
                    if (result.Succeeded)
                    {
                        //the ApplicationUser user model is then use to sign in to the
                        //web app
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                        await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                            $"Please confirm your account by <a href='{callbackUrl}'>Clicking This Link!</a>");
                        // await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(3, "User created a new account with password.");

                        return RedirectToAction("Login", "Account", new { alertId = 1 });
                    }
                    AddErrors(result);
                }
                else
                {                    
                    ModelState.AddModelError(string.Empty,"Blocked insecure data!");                    
                }
            }

            // If we got this far, something failed, redisplay form
            //the RegisterViewModel model is return to the view to display previous
            //This will be the data flow sink
            return View(model);
        }


        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                  $"Please reset your password by <a href='{callbackUrl}'>Clicking This Link!</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(Web.HomeController.Index), "Home");
        }


        #region Helpers

        //The source in DataFlow is the IdentityResult object passed as a parameter
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                //IdentityResult Error obj description data is pass to the ModelState
                //Modal Error collection
                ModelState.AddModelError(string.Empty, Extensions.SanitizeString(error.Description));
            }
        }

        private Task<BrewMasterUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
