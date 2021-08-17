using Hive.Backend.DataModels;
using Hive.Backend.Repositories;
using Hive.Backend.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend.Services
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _repository;

        public QuoteService(IQuoteRepository repository)
        {
            _repository = repository;
        }

        public IQueryable<Quote> GetAll()
        {
            return _repository.GetAllWithReferences();
        }

        public async Task<Quote> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Quote entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var oldEntity = await GetById(entity.Id);
            await _repository.UpdateQuote(oldEntity, entity);
        }

        public async Task Delete(Guid id)
        {
            var entity = await GetById(id);

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            //await _repository.Delete(entity);
        }

        public async Task<Quote> InsertQuote(Quote entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return await _repository.InsertQuote(entity);
        }

        public async Task<EditableQuoteVM> GetEditableQuote(Guid idQuote)
        {
            return await _repository.GetEditableQuote(idQuote);
        }
    }
}
