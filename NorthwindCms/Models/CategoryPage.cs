using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;
using System.Collections.Generic;

namespace NorthwindCms.Models
{
    [PageType(Title = "Category Page", UseBlocks = false)]
    [ContentTypeRoute(Title = "Default", Route ="/catalog-category")]
    public class CategoryPage : Page<CategoryPage>
    {
        [Region(Title = "Category detail", Description = "The details for this category")]
        public CategoryRegion CategoryDetail { get; set; }
        [Region(Title = "Category products", Description = "The product for this category")]
        public IList<ProductRegion> Products { get; set; } = new List<ProductRegion>();
    }
}
