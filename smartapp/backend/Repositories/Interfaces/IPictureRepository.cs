
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Repositories
{ 
    public interface IPictureRepository : IRepository<Picture>
    { 
		IQueryable<Picture> GetAllWithReferences();
		Task<Picture> GetByIdWithReferences(Guid id);
    } 
}