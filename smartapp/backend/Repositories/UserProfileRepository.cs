using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public class UserProfileRepository : Repository<UserProfile>, IUserProfileRepository
    {
        private readonly int maxLimit = 50;
        public UserProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<UserProfile> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<UserProfile>()
                .Include(u => u.User)
                .Include(g => g.GroupUser)
                .AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<UserProfile> GetByApplicationUserId(string appUserId)
        {
            return await DbContext.UserProfiles.FirstOrDefaultAsync(user => user.UserId == appUserId);
        }


        public async Task<IEnumerable<UserProfile>> GetUsersWithReferences()
        {
            return await DbContext.Set<UserProfile>()
                .Include(u => u.User)
                .Take(maxLimit)
                .ToListAsync();
        }

        /// <summary>
        /// Returns the best contributor of the application
        /// Best contributor stands for the user with the most created posts
        /// It's meant to be shown in the Highlights tab of the application
        /// </summary>
        /// <returns></returns>
        public async Task<UserProfile> GetBestContributor()
        {
            var members = DbContext.Users.Count();
            //Retrieve the reports with the best score published in the last 30 days and group them by creator
            //score = ((Likes + Answers + Views) / Total members) * 100

            var bestReportByAuthor = DbContext.Set<Reporting>()
                .Where(report => report.PublicationDate >= DateTime.Today.Date.AddDays(-30))
                .Select(report => new
                {
                    creatorId = report.CreatedById,
                    score = ((report.Likes + report.Answers + report.Views) / members) * 100
                })
                .OrderByDescending(summary => summary.score)
                .AsEnumerable()
                .GroupBy(summary => summary.creatorId)
                .FirstOrDefault();

            return await DbContext.UserProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == bestReportByAuthor.Key);
        }
        
        #region Helpers
        
        /// <summary>
        /// Returns the number of posts the best contributor idUser created
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public int GetBestContributorPostsNumber(Guid idUser)
        {
            //Getting the first day of the current week
            DateTime firstDayOfWeek = DateTimeExtension.GetFirstDayOfWeek(DateTime.Today);


            return DbContext.Cards
                            .Where(m => m.Type != CardTypes.Reporting.ToString()
                                && m.PublicationDate >= DateTime.Today.Date.AddDays(-30))
                            .Count(a => a.CreatedById == idUser);
        }

        /// <summary>
        /// Returns the number of answers created by an idUser
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public int GetAnswersNumber(Guid idUser)
        {
            return DbContext.Answers.Count(a => a.AnsweredById == idUser);
        }

        public int GetLikesNumber(Guid idUser)
        {
            int numberLikes = 0; 
            var posts = DbContext.Cards
                .Where(c => c.Type != CardTypes.Reporting.ToString()
                && c.CreatedById == idUser
                //&& DbContext.Set<UserCardLikes>().Where(ucl => ucl.cardId == c.Id).ToList()
                ).ToList();
            //.Select(DbContext.Set<UserCardLikes>().Where(ucl => ucl.cardId == c.Id)
            posts.ForEach(post =>
            {
                if (DbContext.Set<UserCardLikes>().Where(ucl => ucl.CardId == post.Id).Any())
                    numberLikes += DbContext.Set<UserCardLikes>().Where(ucl => ucl.CardId == post.Id).Count();
            });

            return numberLikes;
        }

        #endregion
    }
}
