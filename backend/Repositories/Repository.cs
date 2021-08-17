using Microsoft.EntityFrameworkCore; 
using System; 
using System.Linq; 
using System.Threading.Tasks; 
using  Hive.Backend.Models;

namespace Hive.Backend.Repositories
{ 
    public class Repository<T> : IRepository<T> where T : class
    { 
        protected readonly ApplicationDbContext DbContext;
 
        public Repository(ApplicationDbContext context) 
        { 
            DbContext = context; 
        }

        public IQueryable<T> GetAll() 
        { 
            return DbContext.Set<T>().AsNoTracking(); 
        } 
        
		public async Task<T> Get(object[] param)
        {
            return await DbContext.Set<T>().FindAsync(param);
        }

        public async Task Insert(T entity) 
        { 
            if (entity == null) 
            { 
                throw new ArgumentNullException(nameof(entity)); 
            } 

           	await DbContext.Set<T>().AddAsync(entity); 
            await DbContext.SaveChangesAsync(); 
        } 
 
        public async Task Update(T entity) 
        { 
            if (entity == null) 
            { 
                throw new ArgumentNullException(nameof(entity)); 
            } 
			
			DbContext.Set<T>().Update(entity); 
            await DbContext.SaveChangesAsync(); 
        }

		public async Task Update(T oldEntity, T entity) 
		{ 
			if (entity == null) 
			{ 
				throw new ArgumentNullException(nameof(entity)); 
			}

			var contextEntry = DbContext.Entry<T>(oldEntity);
			contextEntry.State = EntityState.Detached;
			DbContext.Attach(entity);
			
			DbContext.Set<T>().Update(entity); 
			await DbContext.SaveChangesAsync(); 
        } 
 
        public async Task Delete(T entity) 
        { 
            if (entity == null) 
            { 
                throw new ArgumentNullException(nameof(entity)); 
            } 

            DbContext.Set<T>().Remove(entity); 
            await DbContext.SaveChangesAsync(); 
        } 
    } 
}