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
        IQueryable<Group> GetAll();
        Task<Group> GetById(Guid id);
        Task Save(Group entity);
        Task Delete(Guid id);
        Task Create(Group group);
        Task AddMembers(Guid groupId, List<Guid> userProfileIds);
        Task<ICollection<TargetGroupVM>> ListTargettableGroups(PagingVM paging, Guid userProfileId);
        int GetMembersNumber(Guid groupId);
        IQueryable<Group> GetOptimisedGroups(PagingVM paging, Guid idUser);
        IQueryable<Group> GetMyGroups(PagingVM paging, Guid idUser);
        Task<List<UserProfile>> GetMembersOfGroup(PagingVM paging, Guid groupId);
        Task RemoveMembers(List<Guid> userProfileId, Guid groupId);
        Task<Group> GetGroupWithUserGroup(Guid groupId);
    }
}
