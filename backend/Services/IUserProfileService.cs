using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public partial interface IUserProfileService
    {
        IQueryable<UserProfile> GetAll();
        Task<UserProfile> GetById(Guid id);
        Task Save(UserProfile entity);
        Task Delete(Guid id);
        bool IsInRole(string appUserId, string roleName);
        Task RemoveFromRoleAsync(string appUserId, string roleName);
        Task<UserProfile> GetByApplicationUserId(string appUserId);
        Task<UserProfile> GetByUserId(string userProfileId);
        Task<ApplicationUser> GetApplicationUser(string appUserId);
        Task<IEnumerable<UserProfile>> GetAllUsers(PagingVM paging);
        Task<UserProfile> GetBestContributor();
        int GetAnswersNumber(Guid idUser);
        int GetBestContributorPostsNumber(Guid idUser);
        int GetLikesNumber(Guid idUser);
    }
}
