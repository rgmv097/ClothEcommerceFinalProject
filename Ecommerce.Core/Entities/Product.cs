using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Product:Entity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal? Discount { get; set; }
        public string Description { get; set; }
        public string DescriptionTitle { get; set; }
        public string LongDescription { get; set; }
        public string Availability { get; set; }
        public string Sku { get; set; }
        public string MainImageUrl { get; set; }
        public List<ProductOption> ProductOptions { get; set; }     
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductCategory>? ProductCategories { get; set; }
        public ICollection<WishListProduct> WishListProducts { get; set; }


    }
}
