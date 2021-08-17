using Hive.Backend.DataModels;
using Hive.Backend.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public interface IQuoteService
    {
        IQueryable<Quote> GetAll();
        Task<Quote> GetById(Guid id);
        Task Save(Quote entity);
        Task Delete(Guid id);
        Task<Quote> InsertQuote(Quote entity);
        Task<EditableQuoteVM> GetEditableQuote(Guid idQuote);
    }
}
