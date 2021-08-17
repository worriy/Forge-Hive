using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class IdeaService : IIdeaService
    {
        private readonly IIdeaRepository _repository;

        public IdeaService(IIdeaRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Idea> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Idea> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Idea entity)
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
        }
        public async Task UpdateIdea(Idea entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await _repository.GetByIdWithReferences(entity.Id);

            if (oldEntity != null)
                await _repository.UpdateIdea(oldEntity, entity);
        }

        public async Task<EditableIdeaVM> GetEditableIdea(Guid idIdea)
        {
            return await _repository.GetEditableIdea(idIdea);
        }

        public async Task<Idea> InsertIdea(Idea entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            return await _repository.InsertIdea(entity);
        }
    }
}
