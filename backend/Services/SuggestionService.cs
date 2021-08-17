using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _repository;

        public SuggestionService(ISuggestionRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Suggestion> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Suggestion> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Suggestion entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdateSuggestion(oldEntity, entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            //await _repository.Delete(entity);
        }

        public async Task<Suggestion> InsertSuggestion(Suggestion entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.InsertSuggestion(entity);
        }

        public async Task<EditableSuggestionVM> GetEditableSuggestion(Guid idSuggestion)
        {
            return await _repository.GetEditableSuggestion(idSuggestion);
        }
    }
}
