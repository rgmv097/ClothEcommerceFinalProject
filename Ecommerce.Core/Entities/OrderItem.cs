using Ecommerce.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class OrderItem : Entity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Proudct { get; set; }
        public int Count { get; set; }
        public ProductSizes Size { get; set; }
        public ProductColors Color { get; set; }

    }
}
