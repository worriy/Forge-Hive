using System; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Services
{ 
    public interface IResultService
    { 
		IQueryable<Result> GetAll(); 
		Task<Result> GetById(Guid id);      
        Task Save(Result entity); 
        Task Delete(Guid id); 
    } 
}