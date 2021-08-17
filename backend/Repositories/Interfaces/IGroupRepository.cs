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
        IQueryable<Group> GetAllWithReferences();
        Task<Group> GetByIdWithReferences(Guid id);
        Task InsertGroup(Group entity);
        Task AddMembers(Group group, List<Guid> userProfileIds);
        Task<ICollection<TargetGroupVM>> ListTargettableGroups(PagingVM paging, Guid userProfileId);
        new Task Delete(Group group);
        new Task Update(Group oldEntity, Group entity);
        Task<Group> GetGroupWithUserGroup(Guid groupId);
        int GetMembersNumber(Guid groupId);
        IQueryable<Group> GetOptimisedGroups(PagingVM paging, Guid idUser);
        IQueryable<Group> GetMyGroups(PagingVM paging, Guid idUser);
        Task<List<UserProfile>> GetMembersOfGroup(PagingVM paging, Guid groupId);
        Task RemoveMembers(List<Guid> userProfileIds, Guid groupId);
    }
}
