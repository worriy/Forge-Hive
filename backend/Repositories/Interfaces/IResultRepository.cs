
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Repositories
{ 
    public interface IResultRepository : IRepository<Result>
    { 
		IQueryable<Result> GetAllWithReferences();
		Task<Result> GetByIdWithReferences(Guid id);
    } 
}