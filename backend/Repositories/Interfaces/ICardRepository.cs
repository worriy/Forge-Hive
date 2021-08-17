using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface ICardRepository : IRepository<Card>
    {
        IQueryable<Card> GetAllWithReferences();
        Task<Card> GetByIdWithReferences(Guid id);
        Task<IQueryable<Card>> GetFlowCards(PagingVM paging, Guid userProfileId);
        Task AddView(Guid cardId, Guid userProfileId);
        Task<IQueryable<Guid>> CheckDeletedCards(PagingVM paging, Guid userProfileId);
        Task<string> GetTargetGroupsString(Guid cardId);
        Task<HashSet<Result>> GetResults(Guid cardId);
        Task<List<Card>> GetLatestPosts(PagingVM paging, Guid userProfileId);
        new Task Delete(Card card);
        Task Like(Guid cardId, Guid userProfileId);
        Task Dislike(Guid cardId, Guid userProfileId);
        Task<bool> CheckLikedCard(Guid cardId, Guid userId);
        Task<Mood> CreateMoodCard(Mood mood);

        #region Highlights
            Task<List<Reporting>> GetTopPosts();
            Task<Reporting> GetBestPost();
            Task<PostDetailsVM> GetPostDetails(Guid idCard);
            Task<int> GetViewsNumber(Guid idCard);
            Task<int> GetAnswersNumber(Guid idCard);
            Task<Card> FillUserInfos(Card card);
            string GetTypeCardFromReportId(Guid idReport);
            GeneralMoodVM GetGeneralMood();
        #endregion

        #region Posts
        Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId);
        //Task UpdateCard(Card oldEntity, Card entity);
        #endregion
    }
}
