using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.GroupController
{
	/// <summary>
	/// Controller responsible for all data interactions with the GroupController
	/// </summary>
	[Route("api/group")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class GroupController : Controller
	{
		private readonly IGroupService _groupService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.GroupControllerController"/> class.
        /// </summary>
        public GroupController(IGroupService groupService, ILogger<GroupController> logger)
		{
			_groupService = groupService;
            _logger = logger;
        }

		[HttpPost]
		public async Task<IActionResult> Create([FromBody]CreateGroupVM groupVM)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
                var membersToAdd = groupVM.UserIds != null ? groupVM.UserIds.Select(Guid.Parse).ToList() : new List<Guid>();
                await _groupService.Create(ConvertTo(groupVM), membersToAdd);

				return this.Ok();
			}
			catch (Exception xcp) 
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        //add your list members 
        [HttpPut("addMembers")]
        public async Task<IActionResult> AddMembers([FromBody]UpdateMembersVM AddMembers)
        {
            var canParse = Guid.TryParse(AddMembers.GroupId, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest(ModelState);

            try
            {
                var entity = await _groupService.GetById(groupGuid);

                if (AddMembers.UserIds != null)
                {
                    var membersToAdd = AddMembers.UserIds.Select(Guid.Parse).ToList();
                    await _groupService.AddMembers(groupGuid, membersToAdd);
                }

                return this.Ok();
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{groupId}")]
        public async Task<IActionResult> Delete(string groupId)
		{
            bool canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                //delete the group from the database
                await _groupService.Delete(groupGuid);

                return Ok();
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

		[HttpPut]
		public async Task<IActionResult> Update([FromBody] EditGroupVM groupVM)
		{
            bool canParse = Guid.TryParse(groupVM.Id, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
				return BadRequest(ModelState);

			try
			{
                await _groupService.Save(ConvertTo(groupVM));

                return this.Ok();
			}
			catch (Exception xcp) 
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        [HttpGet("{groupId}")]
        [ProducesResponseType(typeof(GroupVM), 200)]
		public async Task<IActionResult> Get(string groupId)
		{
            var canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var group = await _groupService.GetById(groupGuid);

                if (group == null)
                    return NotFound();


                return this.Ok(await ConvertTo(group));
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

		[HttpGet("list/{userProfileId}")]
		[ProducesResponseType(typeof(GroupVM), 200)]
		public async Task<IActionResult> List(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var groups = await _groupService.GetGroups(userProfileGuid);

                if (groups == null)
                    return NotFound();

                var result = groups.Select(g => ConvertTo(g, userProfileId, _groupService))
                    .ToList();

                return this.Ok(result);
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <GroupVM> ().AsQueryable());
			}
		}
        
        [HttpGet("listTargetableGroups/{userProfileId}")]
		[ProducesResponseType(typeof(TargetGroupVM), 200)]
		public async Task<IActionResult> ListTargetableGroups(string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();

			try
			{
                var groups = await _groupService.GetTargetGroups(userProfileGuid);

                if (groups == null)
                    return NotFound();

                var result = groups.Select(g => ConvertToTargetGroup(g))
                    .ToList();
                
				return this.Ok(result);
			}
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <TargetGroupVM> ().AsQueryable());
			}
		}

        [HttpGet("getMembers/{groupId}")]
		[ProducesResponseType(typeof(UserVM), 200)]
		public async Task<IActionResult> GetMembers(string groupId)
		{
            var canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();

			try
			{
                var users = await _groupService.GetMembersOfGroup(groupGuid);

                if (users == null)
                    return NotFound();

                var result = users.Select(m => Convert(m))
                    .ToList();

                return Ok(result);
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <UserVM> ().AsQueryable());
			}
		}

        [HttpPut("removeMembers")]
		public async Task<IActionResult> RemoveMembers([FromBody]UpdateMembersVM removeMembers)
		{
            var canParse = Guid.TryParse(removeMembers.GroupId, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
				return BadRequest(ModelState);

			try
			{
                var membersToRemove = removeMembers.UserIds.Select(Guid.Parse).ToList();

                await _groupService.RemoveMembers(membersToRemove, groupGuid);
                return this.Ok();
			}
			catch (Exception xcp) {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        #region HELPER
        private UserVM Convert(UserProfile userProfile) => new UserVM
        {
            Firstname = userProfile.User.FirstName,
            Lastname = userProfile.User.LastName,
            Picture = userProfile.User.PictureUrl,
            Email = userProfile.User.Email,
            Job = userProfile.Job,
            UserProfileId = userProfile.Id.ToString()
        };

        private Group ConvertTo(CreateGroupVM group) => new Group
        {
            Name = group.Name,
            City = group.City,
            Country = group.Country,
            CreatedById = Guid.Parse(group.CreatedbyId)
        };

        private static GroupVM ConvertTo(Group group, string userProfileId, IGroupService groupService) => new GroupVM
        {
            IdGroup = group.Id.ToString(),
            Name = group.Name,
            City = group.City,
            Country = group.Country,
            CreatedbyId = group.CreatedById.ToString(),
            NumberMembers = groupService.GetMembersNumber(group.Id).GetAwaiter().GetResult(),
            IsAuthor = group.CreatedById.ToString() == userProfileId ? true : false
        };

        private async Task<GroupVM> ConvertTo(Group group) => new GroupVM
        {
            IdGroup = group.Id.ToString(),
            Name = group.Name,
            City = group.City,
            Country = group.Country,
            CreatedbyId = group.CreatedById.ToString(),
            NumberMembers = await _groupService.GetMembersNumber(group.Id),
            IsAuthor = true
        };

        private static TargetGroupVM ConvertToTargetGroup(Group group) => new TargetGroupVM
        {
            Id = group.Id.ToString(),
            Name = group.Name
        };

        private static Group ConvertTo(EditGroupVM group) => new Group
        {
            Name = group.Name,
            City = group.City,
            Country = group.Country,
            Id = Guid.Parse(group.Id)
        };

        #endregion
    }
}