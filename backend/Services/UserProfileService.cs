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

        public IQueryable<UserProfile> GetAll()
        {
            return _repository.GetAllWithReferences();
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

        public async Task Delete(Guid id)
        {
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _repository.Delete(entity);
        }
        public bool IsInRole(string appUserId, string roleName)
        {
            return _repository.IsInRole(appUserId, roleName);
        }

        public async Task RemoveFromRoleAsync(string appUserId, string roleName)
        {
            await _repository.RemoveFromRoleAsync(appUserId, roleName);
        }

        public async Task<UserProfile> GetByApplicationUserId(string appUserId)
        {
            return await _repository.GetByApplicationUserId(appUserId);
        }

        public async Task<UserProfile> GetByUserId(string userProfileId)
        {
            return await _repository.GetByUserIdWithReferences(userProfileId);
        }

        public async Task<ApplicationUser> GetApplicationUser(string appUserId)
        {
            return await _repository.GetApplicationUser(appUserId);
        }

        public async Task<IEnumerable<UserProfile>> GetAllUsers(PagingVM paging)
        {
            return await _repository.GetUsersWithReferences(paging);
        }

        public string GeneratePassword()
        {
            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$?_-"                        // non-alphanumeric
            };

            Random random = new Random();
            int j;

            List<char> chars = new List<char>();

            for (int i = 0; i < 6; i++)
            {
                j = random.Next(1, 400) / 100;
                switch (j)
                {
                    case 1:
                        chars.Insert(random.Next(0, chars.Count),
                            randomChars[0][random.Next(0, randomChars[0].Length)]);
                        break;
                    case 2:
                        chars.Insert(random.Next(0, chars.Count),
                            randomChars[1][random.Next(0, randomChars[1].Length)]);
                        break;
                    case 3:
                        chars.Insert(random.Next(0, chars.Count),
                            randomChars[2][random.Next(0, randomChars[2].Length)]);
                        break;
                    case 4:
                        chars.Insert(random.Next(0, chars.Count),
                            randomChars[3][random.Next(0, randomChars[3].Length)]);
                        break;
                    default:
                        chars.Insert(random.Next(0, chars.Count),
                            randomChars[random.Next(0, randomChars.Length)][random.Next(0, randomChars[0].Length)]);
                        break;
                }
            }



            return new string(chars.ToArray());
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
