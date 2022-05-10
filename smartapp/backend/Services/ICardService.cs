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
        Task<Card> GetById(Guid id);
        Task<Card> GetCardById(Guid id);
        Task Save(Card entity);
        Task Delete(Guid id);
        #region Flow
        Task<IQueryable<Card>> GetFlowCards(Guid userProfileId);
        Task AddView(Guid cardId, Guid userProfileId);
        Task<IQueryable<Guid>> CheckDeletedCards(Guid userProfileId);
        #endregion

        Task<string> GetTargetGroupsString(Guid cardId);
        Task<List<Result>> GetResults(Guid cardId);
        Task Like(Guid cardId, Guid userProfileId);
        Task Dislike(Guid cardId, Guid userProfileId);
        Task<bool> CheckLikedCard(Guid cardId, Guid userId);

        #region Highlights 
        Task<Reporting> GetBestPost();
        Task<List<Reporting>> GetTopPosts();
        string GetTypeCardFromReportId(Guid idReport);
        (string MoodName, int Value) GetGeneralMood();
        #endregion

        #region Posts
        Task<List<Card>> GetLatestPosts(Guid userProfileId);
        Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId);
        Task<(Card card, Reporting report)> GetPostDetails(Guid idCard);
        #endregion

        Task<Card> CreatePost(Card card);
        Task UpdatePost(Card entity);
        Task<Card> GetEditablePost(Guid cardId);
        Task<Picture> GetDefaultPictureAsync(string type);

        #region helpers

        string GetStatus(Card card);
        Task<string> GetTypeCard(Guid linkedCardId);

        #endregion
    }
}
