using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IIdeaService
    {
        IQueryable<Idea> GetAll();
        Task<Idea> GetById(Guid id);
        Task Save(Idea entity);
        Task Delete(Guid id);
        Task UpdateIdea(Idea entity);
        Task<EditableIdeaVM> GetEditableIdea(Guid idIdea);
        Task<Idea> InsertIdea(Idea entity);
    }
}
