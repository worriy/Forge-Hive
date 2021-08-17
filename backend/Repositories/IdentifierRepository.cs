
using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using  Hive.Backend.Models;
using  Hive.Backend.DataModels;

namespace Hive.Backend.Repositories
{ 
    public class IdentifierRepository : Repository<Identifier>, IIdentifierRepository
    { 
		public IdentifierRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Identifier> GetAllWithReferences()
        { 
            return DbContext.Set<Identifier>().AsNoTracking(); 
        }

		public async Task<Identifier> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Identifier>().FirstOrDefaultAsync(p=>p.Id == id);
        }
    } 
}