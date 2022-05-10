using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository _repository;

        public AnswerService(IAnswerRepository repository)
        {
            _repository = repository;
        }


        public async Task<Answer> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Answer entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

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
        public bool AnsweredCard(Guid cardId, Guid userProfileId)
        {
            return _repository.AnsweredCard(cardId, userProfileId);
        }
        
        public Guid GetChoiceMoodId(string name)
        {
            return _repository.GetChoiceMoodId(name);
        }

        public async Task AddAnswer(Answer answer)
        {
            await _repository.AddAnswer(answer);
        }
        public Task DeleteAnswer(Guid cardId, Guid userProfileId)
        {
            return _repository.DeleteAnswer(cardId, userProfileId);
        }
    }
}
