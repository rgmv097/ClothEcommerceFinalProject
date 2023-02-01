﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
