
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Services
{ 
    public interface IPictureService
    { 
		IQueryable<Picture> GetAll(); 
		Task<Picture> GetById(Guid id);      
        Task Save(Picture entity); 
        Task Delete(Guid id); 
    } 
}