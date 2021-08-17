using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _repository;

        public GroupService(IGroupRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Group> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Group> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Group entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);

            if (oldEntity == null)
                await _repository.Insert(entity);
            else
                await _repository.Update(oldEntity, entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _repository.Delete(entity);
        }
        public async Task Create(Group group)
        {
            await _repository.InsertGroup(group);
        }

        public async Task AddMembers(Guid groupId, List<Guid> userProfileIds)
        {
            Group group = await _repository.GetGroupWithUserGroup(groupId);
           
            if (group != null)
            {
                List<Guid> newMembers = userProfileIds.ToList();
                foreach (GroupUser groupUser in group.GroupUser)
                {
                    foreach (var userProfileId in userProfileIds)
                    {
                        if (groupUser.UserId == userProfileId)
                            newMembers.Remove(userProfileId);
                    }
                }
                await _repository.AddMembers(group, newMembers);
            }
        }

        public async Task<ICollection<TargetGroupVM>> ListTargettableGroups(PagingVM paging, Guid userProfileId) =>
            await _repository.ListTargettableGroups(paging, userProfileId);

        public IQueryable<Group> GetOptimisedGroups(PagingVM paging, Guid idUser)
        {
            return _repository.GetOptimisedGroups(paging,idUser);
        }

        public IQueryable<Group> GetMyGroups(PagingVM paging, Guid idUser)
        {
            return _repository.GetMyGroups(paging, idUser);
        }

        public Task<List<UserProfile>> GetMembersOfGroup(PagingVM paging, Guid groupId)
        {
            return _repository.GetMembersOfGroup(paging, groupId);
        }

        public async Task RemoveMembers(List<Guid> userProfileId, Guid groupId)
        {
            await _repository.RemoveMembers(userProfileId, groupId);
        }

        public async Task<Group> GetGroupWithUserGroup(Guid groupId)
        {
            return await _repository.GetGroupWithUserGroup(groupId);
        }

        #region Helpers

        public int GetMembersNumber(Guid groupId)
            {
                return _repository.GetMembersNumber(groupId);
            }

        #endregion
    }
}
