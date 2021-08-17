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
        private readonly ISurveyRepository _surveyRepository;

        public CardService(ICardRepository repository, ISurveyRepository surveyRepository)
        {
            _repository = repository;
            _surveyRepository = surveyRepository;
        }

        public IQueryable<Card> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Card> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
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
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _repository.Delete(entity);
            // If the Card is a Survey We delete all His questions
            var questions = await _surveyRepository.GetSurveyquestions(id);
            if (questions == null)
            {
                throw new ArgumentNullException(nameof(questions));
            }
            foreach (var question in questions)
            {
                await _repository.Delete(question);
            }

        }

        #region FLOW

        public async Task<IQueryable<Card>> GetFlowCards(PagingVM paging, Guid userProfileId)
        {
            return await _repository.GetFlowCards(paging, userProfileId);
        }

        public async Task AddView(Guid cardId, Guid userProfileId)
        {
            await _repository.AddView(cardId, userProfileId);
        }

        public async Task<IQueryable<Guid>> CheckDeletedCards(PagingVM paging, Guid userProfileId)
        {
            return await _repository.CheckDeletedCards(paging, userProfileId);
        }

        public async Task<string> GetTargetGroupsString(Guid cardId)
        {
            return await _repository.GetTargetGroupsString(cardId);
        }

        public async Task<HashSet<Result>> GetResults(Guid cardId)
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

        public async Task<Mood> CreateMoodCard(Mood mood)
        {
            return await _repository.CreateMoodCard(mood);
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

        public async Task<PostDetailsVM> GetPostDetails(Guid idCard)
        {
            return await _repository.GetPostDetails(idCard);
        }

        public async Task<int> GetAnswersNumber(Guid idCard)
        {
            return await _repository.GetAnswersNumber(idCard);
        }

        public async Task<int> GetViewsNumber(Guid idCard)
        {
            return await _repository.GetViewsNumber(idCard);
        }

        public GeneralMoodVM GetGeneralMood()
        {
            return _repository.GetGeneralMood();
        }

        #endregion

        #region POSTS 

        public async Task<List<Card>> GetLatestPosts(PagingVM paging, Guid userProfileId)
        {
            return await _repository.GetLatestPosts(paging, userProfileId);
        }

        public Task<ICollection<Reporting>> GetTopPostsByUser(Guid userProfileId)
        {
            return _repository.GetTopPostsByUser(userProfileId);
        }
        
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

        #endregion
    }
}
