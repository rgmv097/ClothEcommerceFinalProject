using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Order:Entity
    {
        public int Id { get; set; }
        public bool Published { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }
        public DateTime CreateTime { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }
}
