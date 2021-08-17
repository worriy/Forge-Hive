
using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using  Hive.Backend.Models;
using  Hive.Backend.DataModels;

namespace Hive.Backend.Repositories
{ 
    public class PictureRepository : Repository<Picture>, IPictureRepository
    { 
		public PictureRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IQueryable<Picture> GetAllWithReferences()
        { 
            return DbContext.Set<Picture>().AsNoTracking(); 
        }

		public async Task<Picture> GetByIdWithReferences(Guid id)
        {
            return await DbContext.Set<Picture>().FirstOrDefaultAsync(p=>p.Id == id);
        }
    } 
}