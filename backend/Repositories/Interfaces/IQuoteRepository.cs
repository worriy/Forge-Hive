using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Repositories
{
    public interface IQuoteRepository : IRepository<Quote>
    {
        IQueryable<Quote> GetAllWithReferences();
        Task<Quote> GetByIdWithReferences(Guid id);
        Task UpdateQuote(Quote oldEntity, Quote entity);
        Task<Quote> InsertQuote(Quote quote);
        Task<EditableQuoteVM> GetEditableQuote(Guid idQuote);
    }
}
