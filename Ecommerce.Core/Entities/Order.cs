using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Order:Entity
    {
        public bool? Status { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public string UserEmail { get; set; }
        public DateTime CreateTime { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
