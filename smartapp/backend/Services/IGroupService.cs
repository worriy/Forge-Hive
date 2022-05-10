using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IGroupService
    {
        Task<IQueryable<Group>> GetAll();
        Task<Group> GetById(Guid id);
        Task Save(Group entity);
        Task Delete(Guid id);
        Task Create(Group group, List<Guid> userProfileIds);
        Task AddMembers(Guid groupId, List<Guid> userProfileIds);
        Task<int> GetMembersNumber(Guid groupId);
        Task<IQueryable<Group>> GetGroups(Guid userProfileId);
        Task<IQueryable<Group>> GetTargetGroups(Guid userProfileId);
        Task<List<UserProfile>> GetMembersOfGroup(Guid groupId);
        Task RemoveMembers(List<Guid> userProfileId, Guid groupId);
        Task<Group> GetGroupWithUserGroup(Guid groupId);
    }
}
