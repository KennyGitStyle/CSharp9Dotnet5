using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NorthwindEntitiesLib;
using NorthwindMvc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace NorthwindMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Northwind _context;
        private readonly IHttpClientFactory _httpClient;

        public HomeController(ILogger<HomeController> logger, Northwind context, IHttpClientFactory httpClient)
        {
            _logger = logger;
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            HomeIndexViewModel model = new()
            {
                VisitorCount = new Random().Next(1, 1000),
                Categories = await _context.Categories.ToListAsync(),
                Products = await _context.Products.ToListAsync()
            };
            
            return View(model);
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (!id.HasValue)
                return NotFound();
            
            var model = await _context.Products.SingleOrDefaultAsync(p => p.ProductID == id);
            
            if (model == null)
                return NotFound($"Product with ID of {id} does not exist!");
            
            return View(model);
        }
        public IActionResult ModelBinding()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new()
            {
                Thing = thing,
                HasErrors = !ModelState.IsValid,
                ValidationErrors = ModelState.Values
                .SelectMany(state => state.Errors)
                .Select(error => error.ErrorMessage)
            };
            return View(model);
        }

        public async Task<IActionResult> ProductsThatCostMoreThan(decimal? price)
        {
            if (!price.HasValue)
            {
                NotFound("Please enter a correct value and try again");
            }

            IEnumerable<Product> model = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.UnitPrice > price).ToListAsync();
           
            if (model.Count() == 0)
                return NotFound($"o products cost more than {price:c}.");
            ViewData["MaxPrice"] = price.Value.ToString("c");

            return View(model);
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;
            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }

            var client = _httpClient.CreateClient(
              name: "NorthwindService");

            var request = new HttpRequestMessage(
              method: HttpMethod.Get, requestUri: uri);

            HttpResponseMessage response = await client.SendAsync(request);

            var model = await response.Content
              .ReadFromJsonAsync<IEnumerable<Customer>>();

            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
