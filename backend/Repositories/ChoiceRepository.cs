
using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using  Hive.Backend.Models;
using  Hive.Backend.DataModels;

namespace Hive.Backend.Repositories
{ 
    public class ChoiceRepository : Repository<Choice>, IChoiceRepository
    { 
		public ChoiceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Choice> GetAllWithReferences()
        { 
            return DbContext.Set<Choice>().AsNoTracking(); 
        }

		public async Task<Choice> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Choice>().FirstOrDefaultAsync(p=>p.Id == id);
        }
    } 
}