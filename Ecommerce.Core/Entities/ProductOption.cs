using Ecommerce.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    
    public class ProductOption:Entity
    {
        public ProductSizes Size { get; set; }
        public ProductColors Color { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }             
        public Product Product { get; set; }


    }
}
