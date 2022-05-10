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
        Task<Card> GetByIdWithReferences(Guid id);
        Task<Card> GetCardById(Guid id);
        Task<IQueryable<Card>> GetFlowCards(Guid userProfileId);
        Task AddView(Guid cardId, Guid userProfileId);
        Task<IQueryable<Guid>> CheckDeletedCards(Guid userProfileId);
        Task<string> GetTargetGroupsString(Guid cardId);
        Task<List<Result>> GetResults(Guid cardId);
        Task<List<Card>> GetLatestPosts(Guid userProfileId);
        new Task Delete(Card card);
        Task Like(Guid cardId, Guid userProfileId);
        Task Dislike(Guid cardId, Guid userProfileId);
        Task<bool> CheckLikedCard(Guid cardId, Guid userId);

        #region Highlights
        Task<List<Reporting>> GetTopPosts();
        Task<Reporting> GetBestPost();
        Task<(Card card, Reporting report)> GetPostDetails(Guid idCard);
        string GetTypeCardFromReportId(Guid idReport);
        (string MoodName, int Value) GetGeneralMood();
        #endregion

        #region Posts
        Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId);
        //Task UpdateCard(Card oldEntity, Card entity);
        #endregion
        Task<Card> CreatePost(Card card);
        Task UpdatePost(Card oldEntity, Card entity);
        Task<Card> GetEditablePost(Guid cardId);
        Task<Picture> GetDefaultPictureAsync(string type);
        Task<string> GetTypeCard(Guid linkedCardId);
    }
}
