﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class ContactMessage : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ProfileImage { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; }
    }
}
