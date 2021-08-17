using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Hive.Backend.DataModels;
using Hive.Backend.Services;
using Hive.Backend.ViewModels;
using Hive.Backend.Models.Helpers;
using Hive.Backend.Models.JoinTables;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Hive.Backend.Api.GroupController
{
	/// <summary>
	/// Controller responsible for all data interactions with the GroupController
	/// </summary>
	[Route("api/v1/HiveGroupController")]
    [Authorize(Roles = Helpers.Constants.Strings.Roles.AdminOrUser)]
    public class GroupController : Controller
	{
		private readonly IGroupService _GroupService;
        private readonly INotificationService _NotificationService;
        private readonly ILogger _logger;
        /// <summary>
        /// Initialize a new instance of <see cref="Hive.Backend.Api.GroupControllerController"/> class.
        /// </summary>
        public GroupController(IGroupService GroupService, ILogger<GroupController> logger)
		{
			_GroupService = GroupService;
            _NotificationService = new NotificationService();
            _logger = logger;
        }

		[HttpPost]
		[Route("/api/group/create")]
		public async Task<IActionResult> Create([FromBody]CreateGroupVM groupVM)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			try
			{
				var group = groupVM.GetGroupFromViewModel();
                await _GroupService.Create(group);

				return this.Ok(true);
			}
			catch (Exception xcp) 
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        //add your list members 
        [HttpPut]
        [Route("/api/group/addMembers")]
        public async Task<IActionResult> AddMembers([FromBody]UpdateMembersVM AddMembers)
        {
            var canParse = Guid.TryParse(AddMembers.GroupId, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
                return BadRequest(ModelState);

            try
            {
                var entity = await _GroupService.GetById(groupGuid);

                if (AddMembers.UserIds != null)
                {
                    var membersToAdd = AddMembers.UserIds.Select(Guid.Parse).ToList();
                    await _GroupService.AddMembers(groupGuid, membersToAdd);
                    //add the added members to the list of users to notify

                    //await _NotificationService.SendNotification(membersToAdd, "You have been added to group: " + entity.Name, PushActionTypes.group_details, groupGuid);
                }

                return this.Ok(true);
            }
            catch (Exception xcp)
            {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
		[Route("/api/group/delete")]
		public async Task<IActionResult> Delete(string groupId)
		{
            bool canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                var entity = await _GroupService.GetGroupWithUserGroup(groupGuid);

                if (entity == null)
                {
                    return NotFound();
                }
                //get all the group users to notify
                var toNotify = entity.GroupUser.Select(g => g.UserId).ToList();

                //delete the group from the database
                await _GroupService.Delete(groupGuid);

                //await _NotificationService.SendNotification(toNotify, "The group " + entity.Name + " has been deleted.", PushActionTypes.group_list);
                return Ok(true);
            }
            catch (Exception xcp)
            {
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

		[HttpPut]
		[Route("/api/group/update")]
		public async Task<IActionResult> Update([FromBody] EditGroupVM groupVM)
		{
            bool canParse = Guid.TryParse(groupVM.Id, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
				return BadRequest(ModelState);

			try
			{
                //Get the old entity before updating
                var oldGroup = await _GroupService.GetGroupWithUserGroup(groupGuid);

                //keep the old infos
                var oldName = oldGroup.Name;
                var oldCity = oldGroup.City;
                var oldCountry = oldGroup.Country;

                var group = groupVM.GetGroupFromViewModel();
                //Save the new group
                await _GroupService.Save(group);

                var notifiers = new List<Guid>();

                //create the notification targets list
                foreach (GroupUser gu in oldGroup.GroupUser)
                    if (gu.UserId != group.CreatedById)
                        notifiers.Add(gu.UserId);

                //Create the notification message
                var notificationMessage = "The group " + oldName;

                //if the group name changed
                if (oldName != group.Name)
                {
                    notificationMessage += " has became " + group.Name;
                    if (oldCity != group.City || oldCountry != group.Country)
                        notificationMessage += " and";
                }
                //if the group location changed
                if (oldCity != group.City || oldCountry != group.Country)
                    notificationMessage += " moved to " + group.City + " in " + group.Country;

                //Notify the users that this group is updated
                //await this._NotificationService.SendNotification(notifiers, notificationMessage, PushActionTypes.group_details, group.Id);

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
		[Route("/api/group/get")]
		[ProducesResponseType(typeof(GroupVM), 200)]
		public async Task<IActionResult> Get(string groupId)
		{
            var canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                var group = await _GroupService.GetById(groupGuid);

                if (group == null)
                    return NotFound();

                var groupVm = new GroupVM().ConvertFromModel(group);
                groupVm.NumberMembers = _GroupService.GetMembersNumber(groupGuid);

                return this.Ok(await Task.FromResult(groupVm));
            }
			catch (Exception xcp)
			{
                //log exception
                _logger.LogError(xcp.ToString());
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
			}
		}

        // Get Groups where Group.createdById == userProfileId (les groupes que j'ai créé)
		[HttpGet]
		[Route("/api/group/list")]
		[ProducesResponseType(typeof(GroupVM), 200)]
		public async Task<IActionResult> List(PagingVM paging, string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();
			try
			{
                List<GroupVM> groups = new List<GroupVM>();
                var results = _GroupService.GetOptimisedGroups(paging, userProfileGuid).ToHashSet();

                if (results == null)
                    return NotFound();

                foreach (var group in results)
                {
                    var optimisedGroup = new GroupVM();
                    optimisedGroup = optimisedGroup.ConvertFromModel(group);
                    optimisedGroup.NumberMembers = _GroupService.GetMembersNumber(group.Id);
                    groups.Add(optimisedGroup);
                }
                return this.Ok(await Task.FromResult(groups));
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <GroupVM> ().AsQueryable());
			}
		}

        // Get Groups où j'appartients
        [HttpGet]
        [Route("/api/group/listMyGroups")]
        [ProducesResponseType(typeof(GroupVM), 200)]
        public async Task<IActionResult> ListMyGroups(PagingVM paging, string userProfileId)
        {
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();

            try
            {
                List<GroupVM> groups = new List<GroupVM>();
                var results = _GroupService.GetMyGroups(paging, userProfileGuid).ToHashSet();

                if (results == null)
                    return NoContent();

                foreach (var group in results)
                {
                    var optimisedGroup = new GroupVM();
                    optimisedGroup.ConvertFromModel(group);
                    optimisedGroup.NumberMembers = _GroupService.GetMembersNumber(group.Id);
                    groups.Add(optimisedGroup);
                }
                return this.Ok(await Task.FromResult(groups));
            }
            catch(Exception xcp)
            {
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty<GroupVM>().AsQueryable());
            }
        }
        
        [HttpGet]
		[Route("/api/group/listTargetableGroups")]
		[ProducesResponseType(typeof(TargetGroupVM), 200)]
		public async Task<IActionResult> ListTargetableGroups(PagingVM paging, string userProfileId)
		{
            var canParse = Guid.TryParse(userProfileId, out var userProfileGuid);
            if (!canParse)
                return BadRequest();

			try
			{
                var list = await _GroupService.ListTargettableGroups(paging, userProfileGuid);
				return this.Ok(await Task.FromResult(list));
			}
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <TargetGroupVM> ().AsQueryable());
			}
		}

		[HttpGet]
		[Route("/api/group/getMembers")]
		[ProducesResponseType(typeof(UserVM), 200)]
		public async Task<IActionResult> GetMembers(PagingVM paging, string groupId)
		{
            var canParse = Guid.TryParse(groupId, out var groupGuid);
            if (!canParse)
                return BadRequest();

			try
			{
                var users = await _GroupService.GetMembersOfGroup(paging, groupGuid);
                List<UserVM> members = new List<UserVM>();

                if (users == null)
                    return NoContent();

                foreach( var user in users)
                {
                    members.Add(Convert(user));
                }

                return Ok(members);
            }
			catch (Exception xcp)
			{
                //log error
                _logger.LogError(xcp.ToString());
                return Ok(Enumerable.Empty <UserVM> ().AsQueryable());
			}
		}

		[HttpPut]
		[Route("/api/group/removeMembers")]
		public async Task<IActionResult> RemoveMembers([FromBody]UpdateMembersVM removeMembers)
		{
            var canParse = Guid.TryParse(removeMembers.GroupId, out var groupGuid);
            if (!ModelState.IsValid || !canParse)
				return BadRequest(ModelState);

			try
			{
                var entity = await _GroupService.GetById(groupGuid);
                var membersToRemove = removeMembers.UserIds.Select(Guid.Parse).ToList();

                await _GroupService.RemoveMembers(membersToRemove, groupGuid);
                //add the added members to the list of users to notify

                //await _NotificationService.SendNotification(membersToRemove, "You have been removed to group: " + entity.Name, PushActionTypes.group_details, groupGuid);
                return this.Ok(true);
			}
			catch (Exception xcp) {
                //log exception
                _logger.LogError(xcp.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError);
			}
        }

        #region HELPER
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
        #endregion
    }
}