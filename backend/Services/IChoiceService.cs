
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Services
{ 
    public interface IChoiceService
    { 
		IQueryable<Choice> GetAll(); 
		Task<Choice> GetById(Guid id);      
        Task Save(Choice entity); 
        Task Delete(Guid id); 
    } 
}