using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class WishList:Entity
    {
        public string UserId { get; set; }
        public ICollection<WishListProduct> WishListProducts { get; set; }
    }
}
