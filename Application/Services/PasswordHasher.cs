using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class PasswordHasher
{
	public string Generate(string password)
	{
		return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
	}
	public bool Verify(string password, string passwordHash)
	{
		return BCrypt.Net.BCrypt.Verify(password, passwordHash);
	}
}
