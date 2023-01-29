using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class Slider:Entity
    {
        public string ImageUrl { get; set; }
        public string Slogan { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
    }
}
