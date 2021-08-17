
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Repositories
{ 
    public interface IChoiceRepository : IRepository<Choice>
    { 
		IQueryable<Choice> GetAllWithReferences();
		Task<Choice> GetByIdWithReferences(Guid id);
    } 
}