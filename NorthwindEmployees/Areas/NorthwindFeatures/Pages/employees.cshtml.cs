using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NorthwindEntitiesLib;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NorthwindEmployees.Areas.NorthwindFeatures.Pages
{
    public class EmployeePageModel : PageModel
    {
        private readonly Northwind _context;

        public IEnumerable<Employee> Employees { get; set; }
        public EmployeePageModel(Northwind context)
        {
            _context = context;
        }
        public async Task OnGet()
        {
            Employees = await _context.Employees.ToArrayAsync();
        }
    }
}
