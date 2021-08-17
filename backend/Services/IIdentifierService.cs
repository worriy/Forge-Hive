using System; 
using System.Linq; 
using System.Threading.Tasks;
using Hive.Backend.DataModels;
 
namespace Hive.Backend.Services
{ 
    public interface IIdentifierService
    { 
		IQueryable<Identifier> GetAll(); 
		Task<Identifier> GetById(Guid id);      
        Task Save(Identifier entity); 
        Task Delete(Guid id); 
    } 
}