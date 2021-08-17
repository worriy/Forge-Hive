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
        public UserProfileRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<UserProfile> GetAllWithReferences()
        {
            return DbContext.Set<UserProfile>().AsNoTracking();
        }

        public async Task<UserProfile> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<UserProfile>().Include(u => u.User).Include(g => g.GroupUser).AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        }
        public bool IsInRole(string appUserId, string roleName)
        {
            string roleId = DbContext.Roles.Where(role => role.NormalizedName == roleName.ToUpper()).Select(role => role.Id).FirstOrDefault();

            return DbContext.UserRoles.Where(userRole => userRole.UserId == appUserId &&
                                                    userRole.RoleId == roleId).Any();
        }

        public async Task RemoveFromRoleAsync(string appUserId, string roleName)
        {
            string roleId = DbContext.Roles.Where(role => role.NormalizedName == roleName.ToUpper()).Select(role => role.Id).FirstOrDefault();

            var userRole = DbContext.UserRoles.Where(ur => ur.UserId == appUserId && ur.RoleId == roleId).FirstOrDefault();

            DbContext.UserRoles.Remove(userRole);

            await DbContext.SaveChangesAsync();
        }

        public async Task<UserProfile> GetByApplicationUserId(string appUserId)
        {
            return await DbContext.UserProfiles.FirstOrDefaultAsync(user => user.UserId == appUserId);
        }

        public async Task<UserProfile> GetByUserIdWithReferences(string userProfileId)
        {
            return await DbContext.Set<UserProfile>().Include(user => user.User).AsNoTracking().FirstOrDefaultAsync(p => p.User.Id == userProfileId);
        }

        public async Task<ApplicationUser> GetApplicationUser(string appUserId)
        {
            return await DbContext.Users.FirstOrDefaultAsync(user => user.Id == appUserId);
        }

        public async Task<IEnumerable<UserProfile>> GetUsersWithReferences(PagingVM paging)
        {
            var result = await DbContext.Set<UserProfile>()
                //.Where(u => u.Id > paging.LastId)
                .Include(u => u.User)
                //Start taking from paging.Page id 
                //.Skip(paging.Page * paging.Step)
                //Take only the Step number of cards
                //.Take(paging.Step)
                .ToListAsync();
            return result; 
        }

        public new async Task Delete(UserProfile entity)
        {
            //TODO Verifier le bon fonctionnement avec des données

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            //Delete the user's answers
            var answers = DbContext.Answers.Where(answer => answer.AnsweredById == entity.Id).ToArray();
            DbContext.Answers.RemoveRange(answers);

            //retrieve the user's cards
            var cards = DbContext.Cards.Where(card => card.CreatedById == entity.Id).ToArray();
            
            foreach(Card card in cards)
            {
                //delete the choices associated to these cards
                DbContext.Choices.RemoveRange(card.Choices);
            }

            //delete the cards
            DbContext.Cards.RemoveRange(cards);

            //delete the GroupUser entities
            var groupUsersEntities = DbContext.Set<GroupUser>().Where(gu => gu.UserId == entity.Id).ToArray();
            DbContext.Set<GroupUser>().RemoveRange(groupUsersEntities);

            //If the user has a profile picture, delete it
            if(entity.PictureId != Guid.Empty)
            {
                var picture = DbContext.Pictures.Where(pic => pic.Id == entity.PictureId).FirstOrDefault();
                DbContext.Pictures.Remove(picture);
            }

            //Delete the userCardViews entties
            var userCardViews = DbContext.Set<UserCardViews>().Where(ucv => ucv.UserId == entity.Id).ToArray();
            DbContext.Set<UserCardViews>().RemoveRange(userCardViews);
            
            //finally, remove the UserProfile entity
            DbContext.UserProfiles.Remove(entity);
            await DbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Returns the best contributor of the application
        /// Best contributor stands for the user with the most created posts
        /// It's meant to be shown in the Highlights tab of the application
        /// </summary>
        /// <returns></returns>
        public async Task<UserProfile> GetBestContributor()
        {
            //Retrieve the reports with the best score published in the last 30 days and group them by creator
            //score = ((number of answers) / (number of views) * (number of answers))

            var reportsGroupedByAuthor = DbContext.Set<Reporting>()
                .Where(report => report.PublicationDate >= DateTime.Today.Date.AddDays(-30))
                .Select(report => new
                {
                    creatorId = report.CreatedById,
                    //score = report.Views == 0 ? 0 : ((double)report.Answers / (double)report.Views) * (double)report.Answers
                    score = report.Views == 0 ? 0 : ((((double)report.Answers * 4) + ((double)report.Likes * 3) + ((double)report.Views * 2)) / 9)
                })
                .OrderByDescending(summary => summary.score)
                .AsEnumerable()
                .GroupBy(summary => summary.creatorId)
                .ToList();

            
            var roleAdminId = await DbContext.Roles.Where(r => r.Name == "Admin").Select(r => r.Id).FirstOrDefaultAsync();
            var userId = await DbContext.UserRoles.Where(ur => ur.RoleId == roleAdminId).Select(ur => ur.UserId).FirstOrDefaultAsync();
            var userProfileIDAdmin = await DbContext.UserProfiles.Where(u => u.UserId == userId).Select(u => u.Id).FirstOrDefaultAsync();
            //Get the sum of the scores of the card for each user and select the user with the best score
            var max = new { scoreSum = 0.0, creatorId = Guid.Empty };
            reportsGroupedByAuthor.ForEach(reports =>
            {
                var postCount = 0;
                var sum = 0.0;
                var creatorId = Guid.Empty;
                reports.ToList().ForEach(reportSum =>
                {
                    postCount++;
                    // Exclude the Admin
                    if(reportSum.creatorId != userProfileIDAdmin)
                        creatorId = reportSum.creatorId;
                    sum += reportSum.score;
                });

                if (sum / postCount > max.scoreSum)
                    max = new { scoreSum = sum / postCount, creatorId };
            });

            return await DbContext.UserProfiles
                .Include(u => u.User)
                .FirstOrDefaultAsync(u => u.Id == max.creatorId);
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

            // Get the Questions of the Survey
            var questionsSurveyIds =  DbContext.Set<Question>()
                .Where(m => m.PublicationDate >= DateTime.Today.Date.AddDays(-30) && m.SurveyId != Guid.Empty)
                .Select(m => m.Id)
                .ToList();

            return DbContext.Cards
                            .Where(m => m.Type != CardTypes.Reporting.ToString()
                                && m.PublicationDate >= DateTime.Today.Date.AddDays(-30)
                                && !questionsSurveyIds.Contains(m.Id))
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
