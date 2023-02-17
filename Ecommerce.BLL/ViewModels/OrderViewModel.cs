using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }
        public string Name { get; set; }
        public bool? Status { get; set; }
        public List<OrderItem> Items { get; set; }
        public int TotalCount { get; set; }

    }
}
