using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.UserController
{
	/// <summary>
	/// Controller responsible for all data interactions with the UserController
	/// </summary>
	[Route("api/user")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class UserController : Controller
	{
		private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.UserControllerController"/> class.
        /// </summary>
        public UserController(IUserProfileService userProfileService, UserManager<ApplicationUser> userManager, ILogger<UserController> logger)
		{
			_userProfileService = userProfileService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("{userProfileId}")]
        public async Task<IActionResult> Get(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var user = await _userProfileService.GetById(userProfileGuid);

                if (user == null)
                    return NotFound();

				return this.Ok(ConvertToFullUserVM(user));
			}
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        [HttpGet("GetPicture/{userProfileId}")]
        public async Task<IActionResult> GetProfilePicture(string userProfileId)
        {
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
            try
            {
                var user = await _userProfileService.GetById(userProfileGuid);
                if (user == null)
                    return NotFound();

                var pictureVM = new PictureVM()
                {
                    Picture = user.User.PictureUrl
                };
                return this.Ok(pictureVM);
                
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
		{
            try
            {
                var users = await _userProfileService.GetAllUsers();
                if (users == null)
                    return NoContent();

                var result = users.Select(u => Convert(u))
                    .ToList();

                return this.Ok(result);
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<UserVM>().AsQueryable());
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserViewModel user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userToVerify = await _userManager.FindByEmailAsync(user.Email);

                if (userToVerify == null)
                    return Ok(false);

                userToVerify.FirstName = user.FirstName;
                userToVerify.LastName = user.LastName;
                userToVerify.Email = user.Email;
                userToVerify.PhoneNumber = user.PhoneNumber;
                var userUpdate = await _userManager.UpdateAsync(userToVerify);

                if (!userUpdate.Succeeded)
                    return BadRequest();

                await _userProfileService.Save(ConvertToUserProfile(user, userToVerify.Id));
                return Ok();
            }
            catch (Exception xcp)
            {
                //log Exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("updatePicture")]
        public async Task<IActionResult> UpdatePicture([FromBody] UpdateProfilePictureVM updateProfilePictureVM)
        {
            var canParse = Guid.TryParse(updateProfilePictureVM.UserProfileId, out var userProfileGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest(ModelState);

            try
            {
                var user = await _userProfileService.GetById(userProfileGuid);
                var userToVerify = await _userManager.FindByIdAsync(user.UserId);
                if (userToVerify == null)
                    return NotFound();

                userToVerify.PictureUrl = updateProfilePictureVM.Picture;
                await _userManager.UpdateAsync(userToVerify);

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

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
                    return Ok();

                return BadRequest();
            }
            catch (Exception xcp)
            {
                //log Exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("getRoles")]
        public async Task<IActionResult> GetRoles(string userProfileId)
        {
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
            try
            {
                var user = await _userProfileService.GetById(userProfileGuid);
                var roles = await _userManager.GetRolesAsync(user.User);
                var result = roles;

                if (result != null)
                    return new OkObjectResult(result);

                return NoContent();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        #region HELPERS
        private UserVM Convert(UserProfile userProfile)
        {
            return new UserVM
            {
                Firstname = userProfile.User.FirstName,
                Lastname = userProfile.User.LastName,
                Picture = userProfile.User.PictureUrl,
                Email = userProfile.User.Email,
                Job = userProfile.Job,
                UserProfileId = userProfile.Id.ToString()
            };
        }

        private FullUserVM ConvertToFullUserVM(UserProfile user)
        {
            return new FullUserVM
            {
                Firstname = user.User.FirstName,
                Lastname = user.User.LastName,
                PhoneNumber = user.User.PhoneNumber,
                Email = user.User.Email,
                Country = user.Country,
                City = user.City,
                Department = user.Department,
                Job = user.Job,
                UserProfileId = user.Id.ToString()
            };
        }

        private UserProfile ConvertToUserProfile(UpdateUserViewModel user, string userId)
        {
            return new UserProfile
            {
                Id = Guid.Parse(user.Id),
                Job = user.Job,
                Country = user.Country,
                City = user.City,
                Department = user.Department,
                UserId = userId
            };
        }

        #endregion

    }
}