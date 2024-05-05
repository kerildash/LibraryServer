using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto.Account
{
	public class UserResponse
	{
        public string Email { get; set; }
		public string UserName { get; set; }
        public string Token { get; set; }
    }
}
