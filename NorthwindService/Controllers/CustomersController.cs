using Microsoft.AspNetCore.Mvc;
using NorthwindService.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // ProblemDetails
using NorthwindEntitiesLib;

namespace NorthwindService.Controllers
{
    
    
    public class CustomersController : DefaultController
    {
        private ICustomerRepository _repo;

        // constructor injects repository registered in Startup
        public CustomersController(ICustomerRepository repo)
        {
            _repo = repo;
        }

        // GET: api/customers
        // GET: api/customers/?country=[country]
        // this will always return a list of customers even if its empty
        [HttpGet]
        [ProducesResponseType(200,
          Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(
          string country)
        {
            if (string.IsNullOrWhiteSpace(country))
            {
                return await _repo.RetrieveAllAsync();
            }
            else
            {
                return (await _repo.RetrieveAllAsync())
                  .Where(customer => customer.Country == country);
            }
        }

        // GET: api/customers/[id] 
        [HttpGet("{id}", Name = nameof(GetCustomer))] // named route
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(string id)
        {
            Customer c = await _repo.RetrieveByIdAsync(id);
            if (c == null)
            {
                return NotFound(); // 404 Resource not found
            }
            return Ok(c); // 200 OK with customer in body
        }

        // POST: api/customers
        // BODY: Customer (JSON, XML) 
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCustomer([FromBody] Customer c)
        {
            if (c == null)
            {
                return BadRequest(); // 400 Bad request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad request
            }
            Customer added = await _repo.CreateAsync(c);
            return CreatedAtRoute( // 201 Created
              routeName: nameof(GetCustomer),
              routeValues: new { id = added.CustomerID.ToLower() },
              value: added);
        }

        // PUT: api/customers/[id]
        // BODY: Customer (JSON, XML) 
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomer(
          string id, [FromBody] Customer c)
        {
            id = id.ToUpper();
            c.CustomerID = c.CustomerID.ToUpper();

            if (c == null || c.CustomerID != id)
            {
                return BadRequest(); // 400 Bad request
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // 400 Bad request
            }

            var existing = await _repo.RetrieveByIdAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }
            await _repo.UpdateAsync(id, c);
            return new NoContentResult(); // 204 No content
        }

        // DELETE: api/customers/[id] 
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            // take control of problem details
            if (id == "bad")
            {
                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://localhost:5001/customers/failed-to-delete",
                    Title = $"Customer ID {id} found but failed to delete.",
                    Detail = "More details like Company Name, Country and so on.",
                    Instance = HttpContext.Request.Path
                };
                return BadRequest(problemDetails); // 400 Bad request
            }

            var existing = await _repo.RetrieveByIdAsync(id);
            if (existing == null)
            {
                return NotFound(); // 404 Resource not found
            }

            bool? deleted = await _repo.DeleteAsync(id);
            if (deleted.HasValue && deleted.Value) // short circuit AND
            {
                return new NoContentResult(); // 204 No content
            }
            else
            {
                return BadRequest( // 400 Bad request
                  $"Customer {id} was found but failed to delete.");
            }
        }
    }
}