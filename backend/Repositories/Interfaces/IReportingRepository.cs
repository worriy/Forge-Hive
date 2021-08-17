
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Repositories
{ 
    public interface IReportingRepository : IRepository<Reporting>
    { 
		IQueryable<Reporting> GetAllWithReferences();
		Task<Reporting> GetByIdWithReferences(Guid id);
        Reporting FillInfos(Reporting card);
        Task AddAnswer(Answer answer);

} 
}