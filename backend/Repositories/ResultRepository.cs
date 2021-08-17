
using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using  Hive.Backend.Models;
using  Hive.Backend.DataModels;

namespace Hive.Backend.Repositories
{ 
    public class ResultRepository : Repository<Result>, IResultRepository
    { 
		public ResultRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Result> GetAllWithReferences()
        { 
            return DbContext.Set<Result>().Include(m=>m.Choice).Include(m=>m.Reporting).AsNoTracking(); 
        }

		public async Task<Result> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Result>().Include(m=>m.Choice).Include(m=>m.Reporting).FirstOrDefaultAsync(p=>p.Id == id);
        }
    } 
}