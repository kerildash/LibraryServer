using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Database.RepositoryInterfaces;

namespace Application.Services;

public class UserService(PasswordHasher hasher, IUserRepository repository)
{
	public async Task Register(string userName, string password, string email)
	{
		string passwordHash = hasher.Generate(password);
		var User = new User(Guid.NewGuid(), userName, passwordHash, email);
		await repository.Add(User);
	}
	public async Task<string> Login(string email, string password)
	{
		var user = await repository.GetByEmail(email);
		var logged = hasher.Verify(password, user.PasswordHash);
		if (!logged)
		{
			throw new Exception("failed to log in");
		}
	}
}
