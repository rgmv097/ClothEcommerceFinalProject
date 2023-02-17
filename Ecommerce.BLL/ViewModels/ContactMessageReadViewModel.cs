using Ecommerce.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
      public class ContactMessageReadViewModel
        {
            public List<ContactMessage> contactMessages = new List<ContactMessage>();
            public bool isReadAllMessage { get; set; }
        }
}
