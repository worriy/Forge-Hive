using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _repository;

        public QuestionService(IQuestionRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Question> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Question> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Question entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdateQuestion(oldEntity, entity);
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

        public async Task<Question> InsertQuestion(Question entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.InsertQuestion(entity);
        }

        public async Task<EditableQuestionVM> GetEditableQuestion(Guid idQuestion)
        {
            return await _repository.GetEditableQuestion(idQuestion);
        }
    }
}
