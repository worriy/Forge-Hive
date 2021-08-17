using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IEventRepository : IRepository<Event>
    {
        IQueryable<Event> GetAllWithReferences();
        Task<Event> GetByIdWithReferences(Guid id);
        Task UpdateEvent(Event oldEntity, Event entity);
        Task<Event> InsertEvent(Event ev);
        Task<EditableEventVM> GetEditableEvent(Guid idEvent);
    }
}
