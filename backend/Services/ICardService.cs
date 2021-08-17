using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface ICardService
    {
        IQueryable<Card> GetAll();
        Task<Card> GetById(Guid id);
        Task Save(Card entity);
        Task Delete(Guid id);
        #region Flow
        Task<IQueryable<Card>> GetFlowCards(PagingVM paging, Guid userProfileId);
        Task AddView(Guid cardId, Guid userProfileId);
        Task<IQueryable<Guid>> CheckDeletedCards(PagingVM paging, Guid userProfileId);
        #endregion

        Task<string> GetTargetGroupsString(Guid cardId);
        Task<HashSet<Result>> GetResults(Guid cardId);
        Task Like(Guid cardId, Guid userProfileId);
        Task Dislike(Guid cardId, Guid userProfileId);
        Task<bool> CheckLikedCard(Guid cardId, Guid userId);
        Task<Mood> CreateMoodCard(Mood mood);

        #region Highlights 
        Task<Reporting> GetBestPost();
            Task<List<Reporting>> GetTopPosts();
            //Task<PostDetailsVM> GetPostDetails();
            Task<int> GetAnswersNumber(Guid idCard);
            Task<int> GetViewsNumber(Guid idCard);
            string GetTypeCardFromReportId(Guid idReport);
            GeneralMoodVM GetGeneralMood();
        #endregion

        #region Posts
        Task<List<Card>> GetLatestPosts(PagingVM paging, Guid userProfileId);
        Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId);
        Task<PostDetailsVM> GetPostDetails(Guid idCard);
        #endregion

        #region helpers

        string GetStatus(Card card);

        #endregion
    }
}
