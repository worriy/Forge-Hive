
﻿using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
 
namespace Hive.Backend.Repositories
{ 
    public interface IRepository<T>
    { 
		IQueryable<T> GetAll();
		Task<T> Get(object[] param);
        Task Insert(T entity);   
        Task Update(T entity);
		Task Update(T oldEntity, T entity);
        Task Delete(T entity); 
    } 
}