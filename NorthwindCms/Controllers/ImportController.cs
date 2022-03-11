using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindCms.Models;
using Piranha;
using Piranha.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NorthwindCms.Controllers
{
    public class ImportController : Controller
    {
        private readonly IApi _api;
        private readonly Northwind _context;

        public ImportController(IApi api, Northwind context)
        {
            _api = api;
            _context = context;
        }

        [Route("/import")]
        public async Task<IActionResult> Index()
        {
            int importCount = 0;
            int existCount = 0;

            var site = await _api.Sites.GetDefaultAsync();

            var catalog = await _api.Pages
              .GetBySlugAsync<CatalogPage>("catalog");

            foreach (Category c in
              _context.Categories.Include(c => c.Products))
            {
                // if the category page already exists,
                // then skip to the next iteration of the loop
                CategoryPage cp = await _api.Pages.GetBySlugAsync<CategoryPage>(
                  $"catalog/{c.CategoryName.ToLower().Replace(' ', '-')}");

                if (cp == null)
                {
                    importCount++;

                    cp = await CategoryPage.CreateAsync(_api);

                    cp.Id = Guid.NewGuid();
                    cp.SiteId = site.Id;
                    cp.ParentId = catalog.Id;
                    cp.CategoryDetail.CategoryID = c.CategoryID;
                    cp.CategoryDetail.CategoryName = c.CategoryName;
                    cp.CategoryDetail.Description = c.Description;

                    // find the media folder named Categories
                    Guid categoriesFolderID =
                      (await _api.Media.GetAllFoldersAsync())
                      .First(folder => folder.Name == "Categories").Id;

                    // find image with correct filename for category id
                    var image = (await _api.Media
                      .GetAllByFolderIdAsync(categoriesFolderID))
                      .First(media => media.Type == MediaType.Image
                      && media.Filename == $"category{c.CategoryID}.jpeg");

                    cp.CategoryDetail.CategoryImage = image;


                    if (cp.Products.Count == 0)
                    {
                        // convert the products for this category into
                        // a list of instances of ProductRegion
                        cp.Products = c.Products
                          .Select(p => new ProductRegion
                          {
                              ProductID = p.ProductID,
                              ProductName = p.ProductName,
                              UnitPrice = p.UnitPrice.HasValue
                              ? p.UnitPrice.Value.ToString("c") : "n/a",
                              UnitsInStock = p.UnitsInStock ?? 0
                          }).ToList();
                    }

                    cp.Title = c.CategoryName;
                    cp.MetaDescription = c.Description;
                    cp.NavigationTitle = c.CategoryName;
                    cp.Published = DateTime.Now;

                    await _api.Pages.SaveAsync(cp);
                }
                else
                {
                    existCount++;
                }
            }

            TempData["import_message"] = $"{existCount} categories already existed. {importCount} new categories imported.";

            return Redirect("~/");
        }
    }
}
