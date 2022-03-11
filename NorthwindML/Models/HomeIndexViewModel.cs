using NorthwindEntitiesLib;
using System.Collections.Generic;

namespace NorthwindML.Models
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public bool GermanyDatasetExists { get; set; }
        public bool UKDatasetExists { get; set; }
        public bool USADatasetExists { get; set; }
        public long MillisSeconds { get; set; }
    }
}
