using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NorthwindEntitiesLib;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NorthwindWeb.Pages
{
    public class SuppliersModel : PageModel
    {
        private readonly Northwind _context;

        public SuppliersModel(Northwind context)
        {
            _context = context;
        }
        public IEnumerable<string> Suppliers { get; set; }

        public async Task OnGet()
        {
            ViewData["Title"] = "Northwind Web Site - Suppliers";

            Suppliers = await _context.Suppliers.Select(s => s.CompanyName).ToListAsync();

      
        }

        [BindProperty]
        public Supplier Supplier { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                await _context.Suppliers.AddAsync(Supplier);
                await _context.SaveChangesAsync();
                return RedirectToPage("/suppliers");
            }
            return Page();
        }
    }
}