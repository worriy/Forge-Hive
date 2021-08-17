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

        public GroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Group> GetAllWithReferences()
        {
            return DbContext.Set<Group>().Include(m => m.CreatedBy).AsNoTracking();
        }

        public async Task<Group> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Group>().Include(m => m.CreatedBy).FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task InsertGroup(Group entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            entity.GroupUser = new List<GroupUser>();
            
            DbContext.Groups.Add(entity);

            var groupUser = new GroupUser()
            {
                GroupId = entity.Id,
                UserId = entity.CreatedById.Value
            };

            entity.GroupUser.Add(groupUser);

            DbContext.Set<GroupUser>().Add(groupUser);

            await DbContext.SaveChangesAsync();
            
        }
        
        public async Task AddMembers(Group group, List<Guid> userProfileIds)
        {
            foreach(var userProfileId in userProfileIds)
            {
                GroupUser member = new GroupUser
                {
                    GroupId = group.Id,
                    UserId = userProfileId
                };
                group.GroupUser.Add(member);
                DbContext.Set<GroupUser>().Add(member);
            }

            await DbContext.SaveChangesAsync();
        }

        public async Task<ICollection<TargetGroupVM>> ListTargettableGroups(PagingVM paging, Guid userProfileId) =>
            await DbContext.Set<GroupUser>()
                .Where(gu => gu.UserId == userProfileId)
                .Join(DbContext.Groups,
                    a => a.GroupId,
                    b => b.Id,
                    (a, b) => new TargetGroupVM(b.Id.ToString(), b.Name))
                .ToListAsync();

        new public async Task Delete(Group group)
        {
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

        new public async Task Update(Group oldEntity, Group entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var contextEntry = DbContext.Entry<Group>(oldEntity);

            entity.CreatedBy = oldEntity.CreatedBy;
            entity.CreatedById = oldEntity.CreatedById;
            contextEntry.CurrentValues.SetValues(entity);
            await DbContext.SaveChangesAsync();
        }

        public int GetMembersNumber(Guid groupId)
        {
            var numberMembers = DbContext.Set<GroupUser>()
                .Where(gu => gu.GroupId == groupId)
                .Count();
            
            return numberMembers;
        }

        public IQueryable<Group> GetOptimisedGroups(PagingVM paging, Guid idUser)
        {
            //Get all the groups 
            var result = DbContext.Set<GroupUser>()
                .Where(g => g.UserId == idUser)
                .Where(g => g.GroupId != publicGroup)
                .Select(g => g.Group)
                .Where(g => g.CreatedById == idUser/* && g.Id > paging.LastId*/)
                //Start taking from paging.Page id 
                //.Skip(paging.Page * paging.Step)
                //Take only the Step number of cards
                //.Take(paging.Step)
                .ToList();
            
            return result.AsQueryable();
        }

        public IQueryable<Group> GetMyGroups(PagingVM paging, Guid idUser)
        {
            var result = DbContext.Set<GroupUser>()
                .Where(g => g.UserId == idUser)
                .Where(g => g.GroupId != publicGroup)
                .Select(g => g.Group)
                .Where(g => g.CreatedById != idUser/* && g.Id > paging.LastId*/)
                //Start taking from paging.Page id 
                //.Skip(paging.Page * paging.Step)
                //Take only the Step number of cards
                //.Take(paging.Step)
                .ToList();

            return result.AsQueryable();

        }

        public async Task<List<UserProfile>> GetMembersOfGroup(PagingVM paging, Guid groupId)
        {
            var group = DbContext.Set<Group>().Where(g => g.Id == groupId).FirstOrDefaultAsync().Result;

            return await DbContext.UserProfiles
                .Where(u => DbContext.Set<GroupUser>().Any(gu => gu.UserId == u.Id && gu.GroupId == groupId))
                .Include(u => u.User)
                .ToListAsync();
        }

        

        public async Task RemoveMembers(List<Guid> userProfileIds, Guid groupId)
        {
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
            return await DbContext.Set<Group>().Include(m => m.CreatedBy).Include(m => m.GroupUser).FirstOrDefaultAsync(p => p.Id == groupId);
            
        }
        #endregion
    }
}
