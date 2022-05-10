using Microsoft.EntityFrameworkCore;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hive.Backend.DataModels;
using Hive.Backend.Models;

namespace Hive.Backend.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        private readonly Guid publicGroup = Program.PublicGroupId;
        private readonly int maxLimit = 50;

        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IQueryable<Group>> GetAllWithReferences()
        {
            var group = DbContext.Set<Group>().Include(m => m.CreatedBy).AsNoTracking();
            return await Task.FromResult(group);
        }
        public async Task<Group> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Group>()
                .Include(m => m.CreatedBy)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task InsertGroup(Group entity, List<Guid> userProfileIds)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            DbContext.Groups.Add(entity);

            var groupUser = new GroupUser()
            {
                GroupId = entity.Id,
                UserId = entity.CreatedById.Value
            };

            entity.GroupUser = new List<GroupUser>
            {
                groupUser
            };
            //DbContext.Set<GroupUser>().Add(groupUser);

            // add the other members
            /*if (!userProfileIds.Any())
                throw new ArgumentNullException(nameof(userProfileIds));*/
            if (userProfileIds.Any())
            {
                var groupUsers = userProfileIds.Select(u => new GroupUser
                {
                    GroupId = entity.Id,
                    UserId = u
                });

                entity.GroupUser.AddRange(groupUsers);
                //DbContext.Set<GroupUser>().AddRange(groupUsers);
            }
            
            await DbContext.SaveChangesAsync();
        }
        public async Task AddMembers(Group group, List<Guid> userProfileIds)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            if (!userProfileIds.Any())
                throw new ArgumentNullException(nameof(userProfileIds));

            var groupUsers = userProfileIds.Select(u => new GroupUser
            {
                GroupId = group.Id,
                UserId = u
            });

            group.GroupUser.AddRange(groupUsers);
            //DbContext.Set<GroupUser>().AddRange(groupUsers);

            await DbContext.SaveChangesAsync();
        }
        public async Task Delete(Group group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            //Remove the rows in the join table GroupUsers
            DbContext.Set<GroupUser>()
                .RemoveRange(DbContext.Set<GroupUser>()
                    .Where(g => g.GroupId == group.Id)
                    .ToList());

            //Remove the rows in the join table CardGroups
            DbContext.Set<CardGroup>()
                .RemoveRange(DbContext.Set<CardGroup>()
                    .Where(g => g.GroupId == group.Id)
                    .ToList());

            //Remove the corresponding Group
            DbContext.Set<Group>().Remove(DbContext.Set<Group>()
                    .Where(g => g.Id == group.Id)
                    .FirstOrDefault());

            await DbContext.SaveChangesAsync();
        }
        public async Task Update(Group oldEntity, Group entity)
        {
            if (oldEntity == null || entity == null)
                throw new ArgumentNullException(nameof(entity));

            var contextEntry = DbContext.Entry<Group>(oldEntity);

            entity.CreatedBy = oldEntity.CreatedBy;
            entity.CreatedById = oldEntity.CreatedById;
            contextEntry.CurrentValues.SetValues(entity);

            await DbContext.SaveChangesAsync();
        }
        public async Task<int> GetMembersNumber(Guid groupId)
        {
            var members = DbContext.Set<GroupUser>()
                .Where(gu => gu.GroupId == groupId)
                .Count();
            return await Task.FromResult(members);
        }
        public async Task<IQueryable<Group>> GetGroups(Guid userProfileId)
        {
            var result = DbContext.Set<GroupUser>()
                .Where(g => g.UserId == userProfileId && g.GroupId != publicGroup)
                .Select(g => g.Group)
                .Take(maxLimit);

            return await Task.FromResult(result);
        }
        public async Task<IQueryable<Group>> GetTargetGroups(Guid userProfileId)
        {
            var result = DbContext.Set<GroupUser>()
                .Where(g => g.UserId == userProfileId)
                .Select(g => g.Group);

            return await Task.FromResult(result);
        }
        public async Task<List<UserProfile>> GetMembersOfGroup(Guid groupId)
        {
            return await DbContext.UserProfiles
                .Where(u => DbContext.Set<GroupUser>().Any(gu => gu.UserId == u.Id && gu.GroupId == groupId))
                .Include(u => u.User)
                .Take(maxLimit)
                .ToListAsync();
        }
        public async Task RemoveMembers(List<Guid> userProfileIds, Guid groupId)
        {
            if (!userProfileIds.Any())
                throw new ArgumentNullException();

            foreach (var userProfileId in userProfileIds)
            {
                DbContext.Set<GroupUser>().RemoveRange(DbContext.Set<GroupUser>()
                    .Where(g => g.GroupId == groupId && g.UserId == userProfileId));
            }

            await DbContext.SaveChangesAsync();
        }

        #region Helpers 
        public async Task<Group> GetGroupWithUserGroup(Guid groupId)
        {
            return await DbContext.Set<Group>()
                .Include(m => m.CreatedBy)
                .Include(m => m.GroupUser)
                .FirstOrDefaultAsync(p => p.Id == groupId);
        }
        #endregion
    }
}
