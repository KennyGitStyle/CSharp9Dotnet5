using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NorthwindEntitiesLib;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindService.Repositories.Implementations
{
    public class CustomerRepository : ICustomerRepository
    {
        private static ConcurrentDictionary<string, Customer> customersCache;
        private readonly Northwind _context;

        public CustomerRepository(Northwind context)
        {
            _context = context;

            //cache check if ID already exist, to to fetch what already is cached
            if (customersCache == null)
            {
                customersCache = 
                    new(_context.Customers.ToDictionary(c => c.CustomerID));
            }
        }
        //Create a Customer...
        public async Task<Customer> CreateAsync(Customer customer)
        {
            customer.CustomerID = customer.CustomerID.ToUpper();
            EntityEntry<Customer> added = await _context.Customers.AddAsync(customer);

            return await _context.SaveChangesAsync() == 1
                ? customersCache.AddOrUpdate(customer.CustomerID, customer, UpdateCache)
                : null;

        }
        //Helper to update cache
        private Customer UpdateCache(string id, Customer customer)
        {
            Customer old;
            if (customersCache.TryGetValue(id, out old))
            {
                if (customersCache.TryUpdate(id, customer, old))
                {
                    return customer;
                }
            }
            return null;
        }
        //Get all Customer records
        public async Task<IEnumerable<Customer>> RetrieveAllAsync()
        {
            var searchVals = await _context.Customers.ToListAsync();

            return searchVals != null ? customersCache.Values : null;
        }
        //Get a Customer by ID
        public async Task<Customer> RetrieveByIdAsync(string id)
        {
            var retrievedVal = 
                await _context.Customers.SingleOrDefaultAsync(c => c.CustomerID.ToUpper() == id);

            return retrievedVal != null ? UpdateCache(id, retrievedVal) : null;
        
        }
        //Update a Customer
        public async Task<Customer> UpdateAsync(string id, Customer customer)
        {
            id = id.ToUpper();
            customer.CustomerID = customer.CustomerID.ToUpper();
            _context.Customers.Update(customer);

            return await _context.SaveChangesAsync() == 1
                ? UpdateCache(id, customer)
                : null;
        }
        //Delete a Customer
        public async Task<bool?> DeleteAsync(string id)
        {
            id = id.ToUpper();
            var checkRecord = await _context.Customers.FindAsync(id);
            _context.Remove(checkRecord);

            //Check status and update cache removal...
            return await _context.SaveChangesAsync() == 1
                ? customersCache.TryRemove(id, out checkRecord) : null;
        }
    }
}
