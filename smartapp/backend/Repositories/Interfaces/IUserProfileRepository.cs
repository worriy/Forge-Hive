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
        Task<UserProfile> GetByIdWithReferences(Guid id);
        Task<UserProfile> GetByApplicationUserId(string appUserId);
        Task<IEnumerable<UserProfile>> GetUsersWithReferences();
        Task<UserProfile> GetBestContributor();
        int GetBestContributorPostsNumber(Guid idUser);
        int GetAnswersNumber(Guid idUser);
        int GetLikesNumber(Guid idUser);
    }
}
