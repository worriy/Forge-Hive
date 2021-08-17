using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IUserProfileRepository : IRepository<UserProfile>
    {
        IQueryable<UserProfile> GetAllWithReferences();
        Task<UserProfile> GetByIdWithReferences(Guid id);
        bool IsInRole(string appUserId, string roleName);
        Task RemoveFromRoleAsync(string appUserId, string roleName);
        Task<UserProfile> GetByApplicationUserId(string appUserId);
        Task<UserProfile> GetByUserIdWithReferences(string userProfileId);
        Task<ApplicationUser> GetApplicationUser(string appUserId);
        Task<IEnumerable<UserProfile>> GetUsersWithReferences(PagingVM paging);
        new Task Delete(UserProfile entity);
        Task<UserProfile> GetBestContributor();
        int GetBestContributorPostsNumber(Guid idUser);
        int GetAnswersNumber(Guid idUser);
        int GetLikesNumber(Guid idUser);
    }
}
