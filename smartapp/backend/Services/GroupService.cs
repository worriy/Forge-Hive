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

        public async Task<IQueryable<Group>> GetAll()
        {
            return await _repository.GetAllWithReferences();
        }
        public async Task<Group> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }
        public async Task Save(Group entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
                throw new ArgumentNullException(nameof(entity));

            await _repository.Delete(entity);
        }
        public async Task Create(Group group, List<Guid> userProfileIds)
        {
            await _repository.InsertGroup(group, userProfileIds);
        }
        public async Task AddMembers(Guid groupId, List<Guid> userProfileIds)
        {
            var group = await _repository.GetGroupWithUserGroup(groupId);
            if (group == null || !group.GroupUser.Any())
                throw new ArgumentNullException(nameof(group));

            if (!userProfileIds.Any())
                throw new ArgumentNullException(nameof(userProfileIds));

            foreach (GroupUser groupUser in group.GroupUser)
            {
                userProfileIds.RemoveAll(u => u == groupUser.UserId);
            }
            await _repository.AddMembers(group, userProfileIds);
        }
        public async Task<IQueryable<Group>> GetGroups(Guid userProfileId)
        {
            return await _repository.GetGroups(userProfileId);
        }
        public async Task<IQueryable<Group>> GetTargetGroups(Guid userProfileId)
        {
            return await _repository.GetTargetGroups(userProfileId);
        }
        public async Task<List<UserProfile>> GetMembersOfGroup(Guid groupId)
        {
            return await _repository.GetMembersOfGroup(groupId);
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

        public async Task<int> GetMembersNumber(Guid groupId) =>
            await _repository.GetMembersNumber(groupId);

        #endregion
    }
}
