
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Repositories;

namespace Hive.Backend.Services
{ 
    public class ResultService : IResultService
    { 
        private readonly IResultRepository _repository; 
 
        public ResultService(IResultRepository repository) 
        { 
            _repository = repository; 
        }

        public IQueryable<Result> GetAll() 
        { 
            return _repository.GetAllWithReferences(); 
        } 
        
		public async Task<Result> GetById(Guid id)
        {
            return await _repository.GetByIdWithReferences(id);
        }

        public async Task Save(Result entity) 
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