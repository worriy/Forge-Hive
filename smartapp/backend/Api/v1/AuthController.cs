using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hive.Backend.Auth;
using Hive.Backend.DataModels;
using Hive.Backend.Helpers;
using Hive.Backend.Models;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hive.Backend.Api
{
	[Route("api/auth")]
    [Authorize(Roles = Constants.Strings.Roles.AdminOrUser)]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IUserProfileService _userProfileService;
        private readonly IGroupService _groupService;
        private readonly ILogger _logger;


        private readonly Guid publicGroupId = Program.PublicGroupId;

        public AuthController(UserManager<ApplicationUser> userManager,
            IJwtFactory jwtFactory,
            IOptions<JwtIssuerOptions> jwtOptions,
            IUserProfileService userProfileService,
            IGroupService groupService,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _userProfileService = userProfileService;
            _groupService = groupService;
            _logger = loggerFactory.CreateLogger<AuthController>();
        }

        #region Register

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Creation Username
            var userToVerify = await _userManager.FindByNameAsync(user.Email);
            if (userToVerify != null)
                return BadRequest(Errors.AddErrorToModelState("register_failure", "User exist.", ModelState));

            await _userManager.CreateAsync(new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = user.Email, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName });
            //Creation Password
            var newUser = await _userManager.FindByNameAsync(user.Email);
            var checkPassword = await _userManager.CheckPasswordAsync(newUser, user.Password);
            if (checkPassword)
                return BadRequest(Errors.AddErrorToModelState("register_failure", "Invalid password.", ModelState));

            await _userManager.AddPasswordAsync(newUser, user.Password);

            var roles = new List<string>
            {
                "User"
            };

            await _userManager.AddToRolesAsync(newUser, roles);
            
            var identity = await GetClaimsIdentity(user.Email, user.Password);
            if (identity == null)
            {
                await _userManager.DeleteAsync(newUser);
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));
            }

            var userProfile = new UserProfile()
            {
                User = newUser
            };
            await _userProfileService.Save(userProfile);

            var groupPublic = await _groupService.GetById(publicGroupId);

            var userProfileId = new List<Guid>
            {
                userProfile.Id
            };
            await _groupService.AddMembers(groupPublic.Id, userProfileId);

            var jwt = await Tokens.GenerateJwt(userProfile.Id, identity, _jwtFactory, user.Email, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
            return new OkObjectResult(jwt);

        }

        [HttpGet("existEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ExistEmail(string email)
        {
            var user = await _userManager.FindByNameAsync(email);
            if (user != null)
                return Ok(true);
            
            return Ok(false);
        }

        #endregion

        #region Login

        // POST api/auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]SignInVM credentials)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var identity = await GetClaimsIdentity(credentials.Email, credentials.Password);
            if (identity == null)
                return BadRequest(Errors.AddErrorToModelState("login_failure", "Invalid username or password.", ModelState));

            var userProfile = await _userProfileService.GetByApplicationUserId(identity.Claims.Single(c => c.Type == "id").Value);
            
			var jwt = await Tokens.GenerateJwt(userProfile.Id, identity, _jwtFactory, credentials.Email, _jwtOptions, new JsonSerializerSettings { Formatting = Formatting.Indented });
			return new OkObjectResult(jwt);
        }
        
        #endregion

        #region Helpers 
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
