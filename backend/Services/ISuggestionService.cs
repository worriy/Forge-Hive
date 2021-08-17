using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface ISuggestionService
    {
        IQueryable<Suggestion> GetAll();
        Task<Suggestion> GetById(Guid id);
        Task Save(Suggestion entity);
        Task Delete(Guid id);
        Task<Suggestion> InsertSuggestion(Suggestion entity);
        Task<EditableSuggestionVM> GetEditableSuggestion(Guid idSuggestion);
    }
}
