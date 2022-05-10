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
        Task<UserProfile> GetById(Guid id);
        Task Save(UserProfile entity);
        Task<UserProfile> GetByApplicationUserId(string appUserId);
        Task<IEnumerable<UserProfile>> GetAllUsers();
        Task<UserProfile> GetBestContributor();
        int GetAnswersNumber(Guid idUser);
        int GetBestContributorPostsNumber(Guid idUser);
        int GetLikesNumber(Guid idUser);
    }
}
