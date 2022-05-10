using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class CardService : ICardService
    {
        private readonly ICardRepository _repository;

        public CardService(ICardRepository repository)
        {
            _repository = repository;
        }

        public async Task<Card> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task<Card> GetCardById(Guid id)
        {
            return await _repository.GetCardById(id);
        }

        public async Task Save(Card entity)
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
            var entity = await GetCardById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _repository.Delete(entity);

        }

        #region FLOW

        public async Task<IQueryable<Card>> GetFlowCards(Guid userProfileId)
        {
            return await _repository.GetFlowCards(userProfileId);
        }

        public async Task AddView(Guid cardId, Guid userProfileId)
        {
            await _repository.AddView(cardId, userProfileId);
        }

        public async Task<IQueryable<Guid>> CheckDeletedCards(Guid userProfileId)
        {
            return await _repository.CheckDeletedCards(userProfileId);
        }

        public async Task<string> GetTargetGroupsString(Guid cardId)
        {
            return await _repository.GetTargetGroupsString(cardId);
        }

        public async Task<List<Result>> GetResults(Guid cardId)
        {
            return await _repository.GetResults(cardId);
        }

        public async Task Like(Guid cardId, Guid userProfileId)
        {
            await _repository.Like(cardId, userProfileId);
        }

        public async Task Dislike(Guid cardId, Guid userProfileId)
        {
            await _repository.Dislike(cardId, userProfileId);
        }

        public async Task<bool> CheckLikedCard(Guid cardId, Guid userId)
        {
            return await _repository.CheckLikedCard(cardId, userId);
        }


        #endregion

        #region Highlights 

        public async Task<Reporting> GetBestPost()
        {
            return await _repository.GetBestPost();
        }

        public Task<List<Reporting>> GetTopPosts()
        {
            return _repository.GetTopPosts();
        }

        public string GetTypeCardFromReportId(Guid idReport)
        {
            return _repository.GetTypeCardFromReportId(idReport);
        }

        public async Task<(Card card, Reporting report)> GetPostDetails(Guid idCard)
        {
            return await _repository.GetPostDetails(idCard);
        }

        public (string MoodName, int Value) GetGeneralMood()
        {
            return _repository.GetGeneralMood();
        }

        #endregion

        #region POSTS 

        public async Task<List<Card>> GetLatestPosts(Guid userProfileId)
        {
            return await _repository.GetLatestPosts(userProfileId);
        }

        public Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId)
        {
            return _repository.GetTopPostsByUser(userProfileId);
        }

        #endregion

        #region CRUD POST
        public async Task<Card> CreatePost(Card card)
        {
            if (card == null)
                throw new ArgumentNullException(nameof(card));

            return await _repository.CreatePost(card);

        }

        public async Task UpdatePost(Card entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (GetStatus(entity) != "unpublished")
                throw new Exception();

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdatePost(oldEntity, entity);
        }

        public async Task<Card> GetEditablePost(Guid cardId) =>
            await _repository.GetEditablePost(cardId);

        public async Task<Picture> GetDefaultPictureAsync(string type) =>
            await _repository.GetDefaultPictureAsync(type);
        #endregion

        #region helpers

        public string GetStatus(Card card)
        {
            if (card.PublicationDate > DateTime.Now)
                return "unpublished";
            else if (DateTime.Now > card.EndDate)
                return "closed";
            else
                return "published";
        }

        public async Task<string> GetTypeCard(Guid linkedCardId)
        {
            return await _repository.GetTypeCard(linkedCardId);
        }

        #endregion
    }
}
