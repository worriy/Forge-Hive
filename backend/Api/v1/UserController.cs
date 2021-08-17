using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
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
	[Route("api/v1/HiveUserController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class UserController : Controller
	{
		private readonly IPictureService _PictureService;
		private readonly IUserProfileService _UserProfileService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.UserControllerController"/> class.
        /// </summary>
        public UserController(IPictureService PictureService,IUserProfileService UserProfileService, UserManager<ApplicationUser> userManager, ILogger<UserController> logger)
		{
			_PictureService = PictureService;
			_UserProfileService = UserProfileService;
            _userManager = userManager;
            _logger = logger;
        }

		[HttpGet]
		[Route("/api/user/getFullInfos")]
		public async Task<IActionResult> GetFullInfos(string userProfileId)
		{
            if (string.IsNullOrEmpty(userProfileId))
                return BadRequest();
			
			try
			{
                var user = await _UserProfileService.GetByUserId(userProfileId);

				return this.Ok(ConvertToFullUserVM(user));
			}
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet]
		[Route("/api/user/list")]
		public async Task<IActionResult> List(PagingVM paging)
		{
            try
            {
                var list = new List<UserVM>();
                var result = await _UserProfileService.GetAllUsers(paging);
                if (result != null)
                {
                    foreach (UserProfile item in result)
                    {
                        list.Add(Convert(item));
                    }

                    return this.Ok(await Task.FromResult(list));
                }

                return NoContent();
            }
            catch (Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<UserVM>().AsQueryable());
            }
        }

		[HttpPut]
		[Route("/api/user/update")]
		public async Task<IActionResult> Update([FromBody] UserVM user)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
                if (user.Picture != null)
                {
                    var Picture = new Picture
                    {
                        PicBase64 = user.Picture
                    };
                    await _PictureService.Save(Picture);
                }
				
				await _UserProfileService.Save(ConvertToUserProfile(user));

				return this.Ok(true);
			}
			catch (Exception xcp) 
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        [HttpPut]
        [Route("/api/user/updatePicture")]
        public async Task<IActionResult> UpdatePicture([FromBody] UpdateProfilePictureVM updateProfilePictureVM)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                ApplicationUser userToVerify = await _userManager.FindByIdAsync(updateProfilePictureVM.IdUser);
                if (userToVerify == null)
                    return Ok(false);

                userToVerify.PictureUrl = updateProfilePictureVM.Picture;
                await _userManager.UpdateAsync(userToVerify);

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
        

        [HttpGet]
		[Route("/api/user/get")]
		public async Task<IActionResult> Get(string applicationUserId)
		{
            if (string.IsNullOrEmpty(applicationUserId))
                return BadRequest();

			try
			{
                var userProfile = await _UserProfileService.GetByApplicationUserId(applicationUserId);
                var applicationUser = await _UserProfileService.GetApplicationUser(applicationUserId);

                var userVM = Convert(userProfile);
                userVM.Firstname = applicationUser.FirstName;
                userVM.Lastname = applicationUser.LastName;
                userVM.Email = applicationUser.Email;
                userVM.Picture = applicationUser.PictureUrl;

                return this.Ok(userVM);
			}
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
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
                Picture = user.User.PictureUrl,
                PhoneNumber = user.User.PhoneNumber,
                Email = user.User.Email,
                Country = user.Country,
                City = user.City,
                Department = user.Department,
                Job = user.Job,
                UserProfileId = user.Id.ToString()
            };
        }

        private UserProfile ConvertToUserProfile(UserVM user)
        {
            return new UserProfile
            {
                Job = user.Job,
                Id = Guid.Parse(user.UserProfileId)
            };
        }

        #endregion

    }
}