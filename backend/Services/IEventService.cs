using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IEventService
    {
        IQueryable<Event> GetAll();
        Task<Event> GetById(Guid id);
        Task Save(Event entity);
        Task Delete(Guid id);
        Task<Event> InsertEvent(Event entity);
        Task<EditableEventVM> GetEditableEvent(Guid idEvent);
    }
}
