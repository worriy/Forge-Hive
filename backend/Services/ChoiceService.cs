
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Repositories;

namespace Hive.Backend.Services
{ 
    public class ChoiceService : IChoiceService
    { 
        private readonly IChoiceRepository _repository; 
 
        public ChoiceService(IChoiceRepository repository) 
        { 
            _repository = repository; 
        }

        public IQueryable<Choice> GetAll() 
        { 
            return _repository.GetAllWithReferences(); 
        } 
        
		public async Task<Choice> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Choice entity) 
        { 
            if (entity == null) 
            { 
                throw new ArgumentNullException(nameof(entity)); 
            }

			var oldEntity = await GetById(entity.Id);

			if(oldEntity == null)
           		await _repository.Insert(entity); 
			else
            	await _repository.Update(oldEntity, entity); 
        } 
 
        public async Task Delete(Guid id) 
        { 
			var entity = await GetById(id);

            if (entity == null) 
            { 
                throw new ArgumentNullException(nameof(entity)); 
            } 

            await _repository.Delete(entity); 
        } 
    } 
}