
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Services
{ 
    public interface IReportingService
    { 
		IQueryable<Reporting> GetAll(); 
		Task<Reporting> GetById(Guid id);      
        Task Save(Reporting entity); 
        Task Delete(Guid id);
        Reporting FillInfos(Reporting card);
        Task AddAnswer(Answer answer);

} 
}