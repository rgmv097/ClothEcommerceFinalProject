
using Ecommerce.Core.Enums;

namespace Ecommerce.BLL.ViewModels
{
    public class ProductOptionCreateViewModel : BaseViewModel
    {
        public ProductSizes Size { get; set; }
        public ProductColors Color { get; set; }
        public int Count { get; set; }
    }
}
