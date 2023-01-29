using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public bool isMain { get; set; }
        public Category Parent { get; set; }
        public int? ParentId { get; set; }
        public string? Icon { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Category> Children { get; set; }
        public List<ProductCategory> ProductCategories { get; set; }



    }
}
