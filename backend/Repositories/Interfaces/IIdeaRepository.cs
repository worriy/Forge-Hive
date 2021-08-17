using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IIdeaRepository : IRepository<Idea>
    {
        IQueryable<Idea> GetAllWithReferences();
        Task<Idea> GetByIdWithReferences(Guid id);
        new Task Insert(Idea idea);
        Task UpdateIdea(Idea oldEntity, Idea entity);   
        Task<EditableIdeaVM> GetEditableIdea(Guid idIdea);
        Task<Idea> InsertIdea(Idea idea);
    }
}
