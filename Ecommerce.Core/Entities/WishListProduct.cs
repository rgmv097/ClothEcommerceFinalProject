using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class WishListProduct:Entity
    {
        public int ProductId { get; set; } 
        public Product Product { get; set; }
        public int WishlistId { get; set; }
        public WishList WishList { get; set; }

    }
}
