﻿using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Data.DAL
{
   public class ClothDbContext:IdentityDbContext<User>
    {
        public ClothDbContext(DbContextOptions options):base(options)
        {
        }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductOption> ProductOptions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        
        public DbSet<ProductCategory> ProductCategories { get; set; }


    }
}