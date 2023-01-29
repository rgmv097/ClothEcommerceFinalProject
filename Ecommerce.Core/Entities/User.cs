using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.Entities
{
    public class User:IdentityUser
    {
        string Firsname { get; set; }
        string Lastname { get; set; }

    }
}
