using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<IQueryable<Group>> GetAllWithReferences();
        Task<Group> GetByIdWithReferences(Guid id);
        Task InsertGroup(Group entity, List<Guid> userProfileIds);
        Task AddMembers(Group group, List<Guid> userProfileIds);
        Task Delete(Group group);
        Task Update(Group oldEntity, Group entity);
        Task<Group> GetGroupWithUserGroup(Guid groupId);
        Task<int> GetMembersNumber(Guid groupId);
        Task<IQueryable<Group>> GetGroups(Guid userProfileId);
        Task<IQueryable<Group>> GetTargetGroups(Guid userProfileId);
        Task<List<UserProfile>> GetMembersOfGroup(Guid groupId);
        Task RemoveMembers(List<Guid> userProfileIds, Guid groupId);
    }
}
