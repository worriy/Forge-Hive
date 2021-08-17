using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IQuestionService
    {
        IQueryable<Question> GetAll();
        Task<Question> GetById(Guid id);
        Task Save(Question entity);
        Task Delete(Guid id);
        Task<Question> InsertQuestion(Question entity);
        Task<EditableQuestionVM> GetEditableQuestion(Guid idQuestion);
    }
}
