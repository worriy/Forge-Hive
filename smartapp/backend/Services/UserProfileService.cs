using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserProfileRepository _repository;

        public UserProfileService(IUserProfileRepository repository)
        {
            _repository = repository;
        }


        public async Task<UserProfile> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(UserProfile entity)
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

        public async Task<UserProfile> GetByApplicationUserId(string appUserId)
        {
            return await _repository.GetByApplicationUserId(appUserId);
        }


        public async Task<IEnumerable<UserProfile>> GetAllUsers()
        {
            return await _repository.GetUsersWithReferences();
        }

        public async Task<UserProfile> GetBestContributor()
        {
            return await _repository.GetBestContributor();
        }

        public int GetAnswersNumber(Guid idUser)
        {
            return _repository.GetAnswersNumber(idUser);
        }

        public int GetBestContributorPostsNumber(Guid idUser)
        {
            return _repository.GetBestContributorPostsNumber(idUser);
        }

        public int GetLikesNumber(Guid idUser)
        {
            return _repository.GetLikesNumber(idUser);
        }
    }
}
