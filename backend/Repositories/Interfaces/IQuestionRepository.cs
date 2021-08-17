using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IQuestionRepository : IRepository<Question>
    {
        IQueryable<Question> GetAllWithReferences();
        Task<Question> GetByIdWithReferences(Guid id);
        Task UpdateQuestion(Question oldEntity, Question entity);
        Task<Question> InsertQuestion(Question question);
        Task<EditableQuestionVM> GetEditableQuestion(Guid idQuestion);
    }
}
