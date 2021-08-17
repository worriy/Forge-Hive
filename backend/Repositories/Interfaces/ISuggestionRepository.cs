using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface ISuggestionRepository: IRepository<Suggestion>
    {
        IQueryable<Suggestion> GetAllWithReferences();
        Task<Suggestion> GetByIdWithReferences(Guid id);
        Task UpdateSuggestion(Suggestion oldEntity, Suggestion entity);
        Task<Suggestion> InsertSuggestion(Suggestion suggestion);
        Task<EditableSuggestionVM> GetEditableSuggestion(Guid idSuggestion);
    }
}
