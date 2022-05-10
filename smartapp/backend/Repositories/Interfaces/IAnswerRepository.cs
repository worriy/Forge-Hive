using Hive.Backend.DataModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IAnswerRepository : IRepository<Answer>
    {
        Task<Answer> GetByIdWithReferences(Guid id);
        bool AnsweredCard(Guid cardId, Guid userProfileId);
        Guid GetChoiceMoodId(string name);
        Task AddAnswer(Answer answer);
        Task DeleteAnswer(Guid cardId, Guid userProfileId);
    }
}
