using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Hive.Backend.Auth;
using Hive.Backend.Controllers;
using Hive.Backend.DataModels;
using Hive.Backend.Helpers;
using Hive.Backend.Infrastructure.Services;
using Hive.Backend.Models;
using Hive.Backend.Models.Helpers;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Hive.Backend.Api
{
	[Route("api/[controller]")]
    [Authorize(Roles = Constants.Strings.Roles.AdminOrUser)]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole<string>> _roleManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IEmailSender _emailSender;
        private readonly IUserProfileService _userProfileService;
        private readonly IGroupService _groupService;
        private readonly ILogger _logger;
        private static readonly HttpClient Client = new HttpClient();
        private readonly AuthSettings _authSettings;
        private readonly INotificationService _NotificationService;

        private readonly AppSettings _appSettings;

        private readonly Guid publicGroupId = Program.PublicGroupId;

        public AuthController(IOptions<AuthSettings> authSettingsAccessor, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole<string>> roleManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IEmailSender emailSender,
            IUserProfileService userProfileService,
            IGroupService groupService,
            ILoggerFactory loggerFactory,
            IOptions<AppSettings> appSettings)
        {
            _authSettings = authSettingsAccessor.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _emailSender = emailSender;
            _userProfileService = userProfileService;
            _groupService = groupService;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _NotificationService = new NotificationService();
            _appSettings = appSettings.Value;
        }

        #region register

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Creation Username
            ApplicationUser userToVerify = await _userManager.FindByNameAsync(user.Email);
            if (userToVerify != null)
                return Ok(false);

            await _userManager.CreateAsync(new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = user.Email, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName });
            //Creation Password
            ApplicationUser newUser = await _userManager.FindByNameAsync(user.Email);
            var checkPassword = await _userManager.CheckPasswordAsync(newUser, user.Password);
            if (checkPassword)
                return Ok(false);

            await _userManager.AddPasswordAsync(newUser, user.Password);

            List<string> roles = new List<string>
            {
                "User"
            };

            await _userManager.AddToRolesAsync(newUser, roles);
            
            //If CreateUser == true SignIn
            var identity = await GetClaimsIdentity(user.Email, user.Password);
            if (identity == null)
            {
                var res = await _userManager.DeleteAsync(newUser);
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

            UserProfile userProfile = new UserProfile()
            {
                User = newUser
            };
            await _userProfileService.Save(userProfile);

            var groupPublic = await _groupService.GetById(publicGroupId);

            List<Guid> userProfileId = new List<Guid>
            {
                userProfile.Id
            };
            await _groupService.AddMembers(groupPublic.Id, userProfileId);

            // Send an email to confirm his mail
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = newUser.Id, code }, protocol: HttpContext.Request.Scheme);
            //await _emailSender.SendEmailAsync(user.Email, "Confirm your account",
            //    $"Please confirm your account by clicking this <a href='{callbackUrl}'>link</a>");

            //await _NotificationService.SendNotification(null, user.FirstName + " " + user.LastName + " has joined the company, wish him/her luck!", PushActionTypes.no_action);

            var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, user.Email, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);
            //return Ok(true);

        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            try
            {
                ApplicationUser user = await _userManager.FindByIdAsync(refreshToken.Id);

                if (user == null)
                {
                    return BadRequest(Errors.AddErrorToModelState("login_failure", "Failed to refresh Token.", ModelState));
                }
                var userRoles = await _userManager.GetRolesAsync(user);
                var jwt = await Tokens.GenerateJwt(_jwtFactory.GenerateClaimsIdentity(user.UserName, user.Id.ToString(), userRoles.FirstOrDefault()),
                      _jwtFactory, user.UserName, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });

                return new OkObjectResult(jwt);
            }
            catch (Exception xcp)
            {
                // The token failed validation!
                // TODO: Log it or display an error.
                throw new Exception("Exception " + xcp);
            }
        }

        // Register User with Send Email to Confirm
        [HttpPost("registerUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // Send an email with this link
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account",
                        new { userId = user.Id, code }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                $"Please confirm your account by clicking this <a href='{callbackUrl}'>link</a>");

                    // Comment out following line to prevent a new user automatically logged on.
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: View of ConfirmEmail
        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return BadRequest(Errors.AddErrorToModelState("wrong_user", "This user doesn't correspond to any user.", ModelState));
            }
            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            if (result.Succeeded)
            {
                //await _NotificationService.SendNotification(null, user.FirstName + " " + user.LastName + " has joined the company, wish him/her luck!", PushActionTypes.no_action);
                return Ok(true);
            }
            

            return BadRequest(Errors.AddErrorToModelState("invalid_token", "This token is invalid", ModelState));
        }

        [HttpGet("ExistEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ExistEmail(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
                return Ok(true);
            else
                return Ok(false);
        }

        #endregion

        #region login

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]SignInVM credentials)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var identity = await GetClaimsIdentity(credentials.Email, credentials.Password);
            if (identity == null)
            {
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }
            
			var jwt = await Tokens.GenerateJwt(identity, _jwtFactory, credentials.Email, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
			return new OkObjectResult(jwt);
        }

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest();
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var roles = await _userManager.GetRolesAsync(user);
                var result = roles;

                if (result != null)
                    return new OkObjectResult(result);

                return NoContent();
            }
            catch(Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        
        #endregion

        #region Forgot password

        [AllowAnonymous]
        [HttpPost("checkEmail")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel forgotPassword)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(forgotPassword.Email);
                if (user == null)
                {
                    return BadRequest(Errors.AddErrorToModelState("wrong_mail", "This email adress doesn't correspond to any user.", ModelState));
                }

                var confirmEmail = await _userManager.IsEmailConfirmedAsync(user);
                if (!confirmEmail)
                {
                    return BadRequest(Errors.AddErrorToModelState("not_confirm_mail", "Please confirm your Email.", ModelState));
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { code }, protocol: HttpContext.Request.Scheme);
                //var newUrl = "hive://hiveredlab.azurewebsites.net/resetPassword/{code}";
                //var callbackUrl = "Hive://resetPassword/{forgotPassword.Email}/{code}";//Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(forgotPassword.Email, "Reset Password",
                    $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
                //return View("ForgotPasswordConfirmation");
                return Ok(true);

            }
            return Ok(false);
        }


        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return BadRequest(Errors.AddErrorToModelState("wrong_mail", "This email adress doesn't correspond to any user.", ModelState));
            }

            var confirmEmail = await _userManager.IsEmailConfirmedAsync(user);
            if (!confirmEmail)
            {
                return BadRequest(Errors.AddErrorToModelState("not_confirm_mail", "Please confirm your Email.", ModelState));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return Ok(true);
            }
            return BadRequest(Errors.AddErrorToModelState("invalid_token", "This token is invalid", ModelState));
        }

        #endregion

        #region Update InformationUser

        //PUT api/auth/updateUser
        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                ApplicationUser userToVerify = await _userManager.FindByEmailAsync(user.Email);

                if (userToVerify == null)
                    return Ok(false);

                // Update PhoneNumber pour le moment
                userToVerify.FirstName = user.FirstName;
                userToVerify.LastName = user.LastName;
                userToVerify.Email = user.Email;
                userToVerify.PhoneNumber = user.PhoneNumber;

                user.Id = userToVerify.Id;

                var userProfile = await _userProfileService.GetByApplicationUserId(userToVerify.Id);
                userProfile.Country = user.Country;
                userProfile.City = user.City;
                userProfile.Department = user.Department;
                userProfile.Job = user.Job;
                
                var userUpdate = await _userManager.UpdateAsync(userToVerify);

                if (userUpdate.Succeeded)
                {
                    await _userProfileService.Save(userProfile);
                    return Ok(userUpdate.Succeeded);
                }

                return Ok(userUpdate.Errors);
            }
            catch (Exception xcp)
            {
                //log Exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        //
        // POST: /Account/ResetPassword
        [AllowAnonymous]
        [HttpPut("updatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            { 
                var user = await _userManager.FindByEmailAsync(model.Email);               
                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.Password);

                if (result.Succeeded)
                    return Ok(result.Succeeded);

                    return Ok(result.Errors); 
            }
            catch (Exception xcp)
            {
                //log Exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }
        #endregion

        #region Helpers 

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return await Task.FromResult<ClaimsIdentity>(null);

            // get the user to verifty
            var userToVerify = await _userManager.FindByEmailAsync(userName);

            if (userToVerify == null) return await Task.FromResult<ClaimsIdentity>(null);

            // check the credentials
            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                var userRoles = await _userManager.GetRolesAsync(userToVerify);
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id, userRoles.FirstOrDefault()));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }


        #endregion

    }
}
