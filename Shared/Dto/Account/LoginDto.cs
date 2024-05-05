using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dto.Account;

public class LoginDto
{
	[JsonProperty("username")]
	public string UserName { get; set; }
	[JsonProperty("password")]
	[MinLength(8)]
	public string Password { get; set; }
}
