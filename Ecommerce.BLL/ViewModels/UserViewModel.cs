using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.BLL.ViewModels
{
	public class UserViewModel
	{
		public string Id { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string? ProfileImage { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Role { get; set; }

	}
}
