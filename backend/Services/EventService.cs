using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _repository;

        public EventService(IEventRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Event> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Event> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Event entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdateEvent(oldEntity, entity);
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

        public async Task<Event> InsertEvent(Event entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.InsertEvent(entity);
        }

        public async Task<EditableEventVM> GetEditableEvent(Guid idEvent)
        {
            return await _repository.GetEditableEvent(idEvent);
        }
    }
}
