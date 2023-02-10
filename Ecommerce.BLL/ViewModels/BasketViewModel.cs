using Ecommerce.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class BasketViewModel
    {
        public int Id { get; set; }
        public int Count { get; set; }
        public ProductSizes Size { get; set; }
        public ProductColors Color { get; set; }
    }
}
