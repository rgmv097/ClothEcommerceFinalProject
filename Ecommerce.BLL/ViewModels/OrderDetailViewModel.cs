using Ecommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }


    }
}
