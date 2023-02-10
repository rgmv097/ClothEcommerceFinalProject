using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Order:Entity
    {
        public string UserId { get; set; }
        public List<OrderItem> OrderItems{ get; set; }
        public DateTime DateTime { get; set; }
    }
}
