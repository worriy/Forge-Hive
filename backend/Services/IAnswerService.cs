using Hive.Backend.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IAnswerService
    {
        IQueryable<Answer> GetAll();
        Task<Answer> GetById(Guid id);
        Task Save(Answer entity);
        Task Delete(Guid id);
        bool AnsweredCard(Guid cardId, Guid userProfileId);
        Guid GetChoiceMoodId(string name);
        Task DeleteAnswer(Guid cardId, Guid userProfileId);
    }
}
