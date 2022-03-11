using System.Collections.Generic;

namespace NorthwindML.Models
{
    public class Cart
    {
        public IEnumerable<CartItem> CartItems { get; set; }
    }
}
