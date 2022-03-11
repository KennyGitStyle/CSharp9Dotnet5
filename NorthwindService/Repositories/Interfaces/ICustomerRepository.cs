using NorthwindEntitiesLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthwindService.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer);
        Task<IEnumerable<Customer>> RetrieveAllAsync();
        Task<Customer> RetrieveByIdAsync(string id);
        Task<Customer> UpdateAsync(string id, Customer customer);
        Task<bool?> DeleteAsync(string id);
    }
}
