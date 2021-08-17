
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Repositories
{ 
    public interface IIdentifierRepository : IRepository<Identifier>
    { 
		IQueryable<Identifier> GetAllWithReferences();
		Task<Identifier> GetByIdWithReferences(Guid id);
    } 
}